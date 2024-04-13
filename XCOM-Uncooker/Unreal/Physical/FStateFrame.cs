using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public struct FPushedState : IUnrealSerializable
    {
        #region Serialized data

        [Index(typeof(UState))]
        public int State;

        [Index(typeof(UStruct))]
        public int Node;

        public int Offset;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref State);
            stream.Int32(ref Node);
            stream.Int32(ref Offset);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FPushedState) sourceObj;

            State = destArchive.MapIndexFromSourceArchive(other.State, sourceArchive);
            Node = destArchive.MapIndexFromSourceArchive(other.Node, sourceArchive);
            Offset = other.Offset;
        }
    }

    public class FStateFrame : IUnrealSerializable
    {
        #region Serialized data

        [Index(typeof(UStruct))]
        public int Node;

        [Index(typeof(UState))]
        public int StateNode;

        public int ProbeMask;

        public short LatentAction;

        public FPushedState[] StateStack;

        public int Offset;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref Node);
            stream.Int32(ref StateNode);
            stream.Int32(ref ProbeMask);
            stream.Int16(ref LatentAction);
            stream.Array(ref StateStack);

            if (Node != 0)
            {
                stream.Int32(ref Offset);
            }
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStateFrame) sourceObj;

            Node = destArchive.MapIndexFromSourceArchive(other.Node, sourceArchive);
            StateNode = destArchive.MapIndexFromSourceArchive(other.StateNode, sourceArchive);
            ProbeMask = other.ProbeMask;
            LatentAction = other.LatentAction;
            Offset = other.Offset;

            if (other.StateStack != null)
            {
                StateStack = new FPushedState[other.StateStack.Length];

                for (int i = 0; i < StateStack.Length; i++)
                {
                    StateStack[i] = new FPushedState()
                    {
                        State = destArchive.MapIndexFromSourceArchive(other.StateStack[i].State, sourceArchive),
                        Node = destArchive.MapIndexFromSourceArchive(other.StateStack[i].Node, sourceArchive),
                        Offset = other.StateStack[i].Offset
                    };
                }
            }
        }
    }
}
