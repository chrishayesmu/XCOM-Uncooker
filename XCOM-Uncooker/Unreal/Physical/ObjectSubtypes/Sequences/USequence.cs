using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Sequences
{
    /// <summary>
    /// Represents a Kismet sequence. We don't need any special logic to deserialize these. Instead,
    /// this class is used to perform layout on the objects which make up this sequence, so that when
    /// opened in the editor, they aren't all piled on top of each other.
    /// </summary>
    /// <param name="archive"></param>
    /// <param name="tableEntry"></param>
    public class USequence(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        public override void PostArchiveCloneComplete()
        {
            base.PostArchiveCloneComplete();

            var sequenceObjectsProp = GetSerializedProperty("SequenceObjects") as USerializedArrayProperty;

            if (sequenceObjectsProp == null)
            {
                return;
            }

            var sequenceObjects = new List<UObject>();
            var sequenceVariables = new List<UObject>();

            foreach (var serializedProp in sequenceObjectsProp.Data)
            {
                var serializedObjProp = serializedProp as USerializedObjectProperty;
                
                if (serializedObjProp.ObjectIndex != 0)
                {
                    var sequenceObject = Archive.GetObjectByIndex(serializedObjProp.ObjectIndex);

#if DEBUG
                    if (sequenceObject == null)
                    {
                        Debugger.Break();
                        continue;
                    }
#endif

                    if (sequenceObject.TableEntry.ClassObj.IsChildClassOf("SequenceVariable"))
                    {
                        sequenceVariables.Add(sequenceObject);
                    }
                    else
                    {
                        sequenceObjects.Add(sequenceObject);
                    }
                }
            }


            foreach (var sequenceObject in sequenceObjects)
            {
                // TODO: try to account for object width/height where possible
                // TODO: some objects don't have direct hints, but we can usually position
                //       them relative to their input/output links
                int? objPosX = BestGuessForObjectX(sequenceObject);
                int? objPosY = BestGuessForObjectY(sequenceObject);

                if (objPosX != null)
                {
                    AddSerializedIntProperty(sequenceObject, "ObjPosX", objPosX.Value);
                }

                if (objPosY != null)
                {
                    AddSerializedIntProperty(sequenceObject, "ObjPosY", objPosY.Value);
                }

                ArrangeLinkedVariables(sequenceObject, objPosY ?? 0);
            }

            // Find sequence variable "declarations" and move them to a common space; these are variables with
            // names assigned, which aren't connected to any node, but simply make a variable available for reference
            // and initialize it in the sequence
            const int varsPerRow = 6;
            const int columnWidth = 72;
            const int rowHeight = 72;
            const int varsStartX = -600;
            const int varsStartY = 300;
            int curVarIndex = 0;

            foreach (var seqVar in sequenceVariables)
            {
                var varNameProp = seqVar.GetSerializedProperty("VarName") as USerializedNameProperty;

                if (varNameProp == null || varNameProp.Value == "None")
                {
                    continue;
                }

                if (seqVar.GetSerializedProperty("ObjPosX") != null || seqVar.GetSerializedProperty("ObjPosY") != null)
                {
                    continue;
                }

                int column = curVarIndex % varsPerRow;
                int row = curVarIndex / varsPerRow;

                int xPos = column * columnWidth + varsStartX;
                int yPos = row * rowHeight + varsStartY;

                AddSerializedIntProperty(seqVar, "ObjPosX", xPos);
                AddSerializedIntProperty(seqVar, "ObjPosY", yPos);

                curVarIndex++;
            }
        }

        private void AddSerializedIntProperty(UObject obj, string propName, int value)
        {
            var newTag = new FPropertyTag()
            {
                Name = Archive.GetOrCreateName(propName),
                Type = Archive.GetOrCreateName("IntProperty"),
                Size = 4,
                ArrayIndex = 0
            };

            var newProp = new USerializedIntProperty(Archive, /* backingProperty */ null, newTag);
            newProp.Value = value;

            obj.SerializedProperties.Add(newProp);
        }

        private void ArrangeLinkedVariables(UObject sequenceObject, int parentY)
        {
            var variableLinksProp = sequenceObject.GetSerializedProperty("VariableLinks") as USerializedArrayProperty;

            if (variableLinksProp == null)
            {
                return;
            }

            foreach (var variableLinkProp in variableLinksProp.Data)
            {
                var linkProp = variableLinkProp as USerializedStructProperty;

                // SeqVarLink.LinkedVariables is array<SequenceVariable>
                var linkedVariablesProp = linkProp.GetSerializedProperty("LinkedVariables") as USerializedArrayProperty;

                if (linkedVariablesProp == null)
                {
                    continue;
                }

                var drawXProp = linkProp.GetSerializedProperty("DrawX") as USerializedIntProperty;
                int drawX = drawXProp?.Value ?? 0;

                foreach (var variableProp in linkedVariablesProp.Data)
                {
                    var seqVarProp = variableProp as USerializedObjectProperty;

                    if (seqVarProp.ObjectIndex == 0)
                    {
                        continue;
                    }

                    var seqVarObject = Archive.GetObjectByIndex(seqVarProp.ObjectIndex);
                    AddSerializedIntProperty(seqVarObject, "ObjPosX", drawX + 32);
                    AddSerializedIntProperty(seqVarObject, "ObjPosY", parentY + 96);
                }
            }
        }

        private int? BestGuessForObjectX(UObject sequenceObject)
        {
            // Hints at X can be found in SequenceOp.VariableLinks and SequenceOp.EventLinks
            var variableLinksProp = sequenceObject.GetSerializedProperty("VariableLinks") as USerializedArrayProperty;
            var eventLinksProp = sequenceObject.GetSerializedProperty("EventLinks") as USerializedArrayProperty;
            var linksPropToUse = variableLinksProp ?? eventLinksProp;

            if (linksPropToUse == null)
            {
                return null;
            }

            if (linksPropToUse != null && linksPropToUse.Data.Length > 0)
            {
                int sumX = 0;
                int numProps = 0;

                foreach (var link in linksPropToUse.Data)
                {
                    var linkProp = link as USerializedStructProperty;
                    var drawXProp = linkProp.GetSerializedProperty("DrawX") as USerializedIntProperty;

                    if (drawXProp != null)
                    {
                        sumX += drawXProp.Value;
                        numProps++;
                    }
                }

                return numProps == 0 ? 0 : sumX / numProps;
            }

            return null;
        }

        private int? BestGuessForObjectY(UObject sequenceObject)
        {
            // Hints at Y can be found in SequenceOp.InputLinks and SequenceOp.OutputLinks
            var inputLinksProp = sequenceObject.GetSerializedProperty("InputLinks") as USerializedArrayProperty;
            var outputLinksProp = sequenceObject.GetSerializedProperty("OutputLinks") as USerializedArrayProperty;
            var linksPropToUse = inputLinksProp ?? outputLinksProp;

            if (linksPropToUse == null)
            {
                return null;
            }

            if (linksPropToUse != null && linksPropToUse.Data.Length > 0)
            {
                int sumY = 0;
                int numProps = 0;

                foreach (var link in linksPropToUse.Data)
                {
                    var linkProp = link as USerializedStructProperty;
                    var drawYProp = linkProp.GetSerializedProperty("DrawY") as USerializedIntProperty;

                    if (drawYProp != null)
                    {
                        sumY += drawYProp.Value;
                        numProps++;
                    }
                }

                return numProps == 0 ? 0 : sumY / numProps;
            }

            return null;
        }

        private List<UObject> GetConnectedObjectsFromOutputLinks(UObject sequenceObject)
        {
            var objects = new List<UObject>();

            // SequenceOp.OutputLinks is an array<SeqOpOutputLink>
            var outputLinksProp = sequenceObject.GetSerializedProperty("OutputLinks") as USerializedArrayProperty;

            if (outputLinksProp == null)
            {
                return objects;
            }

            foreach (var outputLinkProp in outputLinksProp.Data)
            {
                var linkProp = outputLinkProp as USerializedStructProperty;

                // SeqOpOutputLink.Links is array<SeqOpOutputInputLink>
                var linksProp = linkProp.GetSerializedProperty("Links") as USerializedArrayProperty;

                if (linksProp == null)
                {
                    continue;
                }

                foreach (var seqOpOutputInputLinkProp in linksProp.Data)
                {
                    // SeqOpOutputInputLink.LinkedOp has type SequenceOp
                    var seqOpProp = (seqOpOutputInputLinkProp as USerializedStructProperty).GetSerializedProperty("LinkedOp") as USerializedObjectProperty;

                    if (seqOpProp != null && seqOpProp.ObjectIndex != 0)
                    {
                        objects.Add(Archive.GetObjectByIndex(seqOpProp.ObjectIndex));
                    }
                }
            }

            return objects;
        }
    
        private List<UObject> GetConnectedObjectsFromVariableLinks(UObject sequenceObject)
        {
            var objects = new List<UObject>();

            // SequenceOp.VariableLinks is array<SeqVarLink>
            var variableLinksProp = sequenceObject.GetSerializedProperty("VariableLinks") as USerializedArrayProperty;

            if (variableLinksProp == null)
            {
                return objects;
            }

            foreach (var variableLinkProp in variableLinksProp.Data)
            {
                var linkProp = variableLinkProp as USerializedStructProperty;

                // SeqVarLink.LinkedVariables is array<SequenceVariable>
                var linkedVariablesProp = linkProp.GetSerializedProperty("LinkedVariables") as USerializedArrayProperty;

                if (linkedVariablesProp == null)
                {
                    continue;
                }

                foreach (var variableProp in linkedVariablesProp.Data)
                {
                    var seqVar = variableProp as USerializedObjectProperty;

                    if (seqVar.ObjectIndex != 0)
                    {
                        objects.Add(Archive.GetObjectByIndex(seqVar.ObjectIndex));
                    }
                }
            }
            
            return objects;
        }

    }
}
