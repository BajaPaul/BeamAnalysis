using BeamAnalysis.Abstract;

namespace BeamAnalysis.NumericalModel
{
    internal class BcLoad : IComponentBuilder<BcLoad>, IBcLoad
    {
        private readonly Model BcLoadModel;

        public double[,] Matrix { get; private set; }

        public BcLoad(Model model)
        {
            BcLoadModel = model;
        }

        public void SetComponent()
        {
            Matrix = new double[BcLoadModel.IntModelNodes * BcLoadModel.IntDOFPerNode, 1];

            foreach (ILoad currentLoad in BcLoadModel.AbstractIScheme.ListILoad)
            {
                Matrix = ApplyNodalLoad(Matrix, currentLoad);
            }
        }

        public void SetDedicatedData() { }

        public BcLoad Get()
        {
            return this;
        }

        private double[,] ApplyNodalLoad(double[,] matrix, ILoad currentLoad)
        {
            int nodeNumber = currentLoad.IntNode;
            int loadDoF_1 = nodeNumber * BcLoadModel.IntDOFPerNode - 2;
            int loadDoF_2 = nodeNumber * BcLoadModel.IntDOFPerNode - 1;
            
            matrix[loadDoF_1, 0] = currentLoad.DoubleForce;
            matrix[loadDoF_2, 0] = currentLoad.DoubleMoment;
           
            return matrix;
        }
    }
}
