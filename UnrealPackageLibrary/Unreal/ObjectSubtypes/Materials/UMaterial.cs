using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.SerializedProperties;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Materials
{
    public class UMaterial(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FMaterial MaterialResource_MSP_SM3 = new FMaterial();

        public byte[] UnknownData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            MaterialResource_MSP_SM3.Serialize(stream);

            if (stream.IsRead)
            {
                // There's some data at the end that we don't understand; just store it opaquely for now. It doesn't appear in
                // the public UDK, so we don't write it back out during uncooking.
                long extraBytes = ExportTableEntry.SerialOffset + ExportTableEntry.SerialSize - stream.Position;
                stream.Bytes(ref UnknownData, (int)extraBytes);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UMaterial) sourceObj;

            MaterialResource_MSP_SM3.CloneFromOtherArchive(other.MaterialResource_MSP_SM3, other.Archive, Archive);
            UnknownData = other.UnknownData;

            var expressionsProp = GetSerializedProperty("Expressions") as USerializedArrayProperty;
            var validExpressions = new List<USerializedObjectProperty>();

            // Remove any null expression object references
            if (expressionsProp != null)
            {
                for (int i = expressionsProp.NumElements - 1; i >= 0; i--)
                {
                    var objProp = expressionsProp.Data[i] as USerializedObjectProperty;

                    if (objProp.ObjectIndex != 0)
                    {
                        validExpressions.Add(objProp);
                    }
                }

                expressionsProp.Data = validExpressions.ToArray();
                expressionsProp.NumElements = expressionsProp.Data.Length;
            }
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            MaterialResource_MSP_SM3.PopulateDependencies(dependencyIndices);
        }

        public override void PostArchiveCloneComplete()
        {
            base.PostArchiveCloneComplete();

            // When the same input is used multiple times, we're just using the first one that matches
            TryPopulateInputIfNeeded("DiffuseColor", "Diffuse");
            TryPopulateInputIfNeeded("DiffuseColor", "Diffuse_Base");
            TryPopulateInputIfNeeded("DiffuseColor", "Diffuse Map");
            TryPopulateInputIfNeeded("DiffuseColor", "Damage_Diffuse");
            TryPopulateInputIfNeeded("DiffuseColor", "Diffuse_Damage");
            TryPopulateInputIfNeeded("SpecularPower", "SpcColor_SpcGloss");
            TryPopulateInputIfNeeded("OpacityMask", "Spc_Ems_Mpc_Opc");
            TryPopulateInputIfNeeded("OpacityMask", "Spc_Ems_Ref_Opc");
            TryPopulateInputIfNeeded("Normal", "Normal");
            TryPopulateInputIfNeeded("Normal", "Normal Map");
            TryPopulateInputIfNeeded("Normal", "Damage_Normal");
        }

        /// <summary>
        /// Attempts to fill in one of the main material inputs using a parameter expression.
        /// The input must be unpopulated, and the parameter must exist on this material.
        /// </summary>
        /// <param name="inputName"></param>
        /// <param name="parameterName"></param>
        private void TryPopulateInputIfNeeded(string inputName, string parameterName)
        {
            var inputProp = GetSerializedProperty(inputName) as USerializedStructProperty;

            // If there was originally an input but it got stripped during cooking, then there
            // will be a property but its Expression will be null. If the property itself isn't there,
            // then this property was never set in the first place.
            if (inputProp == null)
            {
                return;
            }

            var inputExpression = inputProp.GetSerializedProperty("Expression") as USerializedObjectProperty;

            if (inputExpression != null && inputExpression.ObjectIndex != 0)
            {
                // TODO: even with an expression set, it might need replacing (e.g. a switch param that has no inputs anymore)
                return;
            }

            // Now look for an expression with the matching parameter name
            var expressionsProp = GetSerializedProperty("Expressions") as USerializedArrayProperty;

            if (expressionsProp == null)
            {
                return;
            }

            int matchingParameterExpressionId = 0;

            for (int i = 0; i < expressionsProp.NumElements; i++)
            {
                var objProp = expressionsProp.Data[i] as USerializedObjectProperty;

                if (objProp.ObjectIndex != 0)
                {
                    var expressionObj = Archive.GetObjectByIndex(objProp.ObjectIndex);

                    if (expressionObj == null)
                    {
                        continue;
                    }

                    var parameterNameProp = expressionObj.GetSerializedProperty("ParameterName") as USerializedNameProperty;

                    if (parameterNameProp != null && parameterNameProp.Value == parameterName)
                    {
                        matchingParameterExpressionId = objProp.ObjectIndex;
                        break;
                    }
                }
            }

            if (matchingParameterExpressionId == 0)
            {
                return;
            }

            if (inputExpression == null)
            {
                var tag = new FPropertyTag() { 
                    Name = Archive.GetOrCreateName("Expression"),
                    Type = Archive.GetOrCreateName("ObjectProperty"),
                    Size = 4,
                    ArrayIndex = 0
                };

                inputExpression = new USerializedObjectProperty(Archive, /* backingProperty */ null, tag);
                inputProp.TaggedProperties.Add(inputExpression);
            }

            inputExpression.ObjectIndex = matchingParameterExpressionId;
        }
    }
}
