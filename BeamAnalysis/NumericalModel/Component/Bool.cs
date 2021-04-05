using BeamAnalysis.Abstract;
using BeamAnalysis.MatrixHelper;
using System.Collections.Generic;

namespace BeamAnalysis.NumericalModel
{
    internal class Bool : IComponentBuilder<Bool>, IBool
    {
        private readonly Model BoolModel;

        public List<int[,]> Matrix { get; private set; }
        public List<int[,]> MatrixTransposed { get; private set; }

        public Bool(Model model)
        {
            BoolModel = model;
            Matrix = new List<int[,]>();
            MatrixTransposed = new List<int[,]>();
        }

        public void SetComponent()
        {
            for (int i = 0; i < BoolModel.IntModelElements; i++)
            {
                int[,] currentBooleanMatrix = CreateBooleanMatrix(BoolModel.Topology.Matrix[i]);
                Matrix.Add(currentBooleanMatrix);
            }
        }

        public void SetDedicatedData()
        {
            for (int i = 0; i < BoolModel.IntModelElements; i++)
            {
                MatrixTransposed.Add(MatrixTransformation.Transpose<int, int>(Matrix[i]));
            }
        }

        public Bool Get()
        {
            return this;
        }

        private int[,] CreateBooleanMatrix(int[] currentTopologyMatrix)
        {
            int[,] currentBoolean = new int[BoolModel.IntNodesPerElement * BoolModel.IntDOFPerNode, BoolModel.IntModelNodes * BoolModel.IntDOFPerNode];
            for (int x = 1; x <= 2; x++)
            {
                currentBoolean[x - 1, (2 * (currentTopologyMatrix[0] - 1) + x) - 1] = 1;
                currentBoolean[(x + 2) - 1, (2 * (currentTopologyMatrix[1] - 1) + x) - 1] = 1;
            }
            return currentBoolean;
        }
    }
}
