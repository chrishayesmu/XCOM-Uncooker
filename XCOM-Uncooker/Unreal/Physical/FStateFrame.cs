using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public struct FPushedState
    {
        [Index(typeof(UState))]
        public int State;

        [Index(typeof(UStruct))]
        public int Node;

        public int Offset;
    }

    public class FStateFrame
    {
        [Index(typeof(UStruct))]
        public int Node;

        [Index(typeof(UState))]
        public int StateNode;

        public int ProbeMask;

        public short LatentAction;

        public FPushedState[] StateStack;

        public int Offset;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out Node);
            stream.Int32(out StateNode);
            stream.Int32(out ProbeMask);
            stream.Int16(out LatentAction);
            stream.PushedStateArray(out StateStack);

            if (Node != 0)
            {
                stream.Int32(out Offset);
            }
        }

        public void CloneFromOtherArchive(FStateFrame sourceFrame, FArchive sourceArchive, FArchive destArchive)
        {
            Node = destArchive.MapIndexFromSourceArchive(sourceFrame.Node, sourceArchive);
            StateNode = destArchive.MapIndexFromSourceArchive(sourceFrame.StateNode, sourceArchive);
            ProbeMask = sourceFrame.ProbeMask;
            LatentAction = sourceFrame.LatentAction;
            Offset = sourceFrame.Offset;

            if (sourceFrame.StateStack != null)
            {
                StateStack = new FPushedState[sourceFrame.StateStack.Length];

                for (int i = 0; i < StateStack.Length; i++)
                {
                    StateStack[i] = new FPushedState()
                    {
                        State = destArchive.MapIndexFromSourceArchive(sourceFrame.StateStack[i].State, sourceArchive),
                        Node = destArchive.MapIndexFromSourceArchive(sourceFrame.StateStack[i].Node, sourceArchive),
                        Offset = sourceFrame.StateStack[i].Offset
                    };
                }
            }
        }
    }
}
