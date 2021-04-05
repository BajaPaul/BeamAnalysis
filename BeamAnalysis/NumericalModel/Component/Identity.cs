using BeamAnalysis.Abstract;
using BeamAnalysis.MatrixHelper;

namespace BeamAnalysis.NumericalModel
{
    public class Identity : IComponentBuilder<Identity>, IIdentity
    {
        private readonly Model IdentityModel;

        public int[,] Matrix { get; private set; }
        public int[,] Id { get; private set; }
        public int[,] Ip { get; private set; }

        public Identity(Model model)
        {
            IdentityModel = model;
        }

        public void SetComponent()
        {
            Matrix = new int[IdentityModel.IntDOFPerNode * IdentityModel.IntModelNodes, IdentityModel.IntDOFPerNode * IdentityModel.IntModelNodes];
            MatrixTransformation.SetIdentityMatrix(Matrix);
        }

        public void SetDedicatedData()
        {
            Id = new int[IdentityModel.IntDOFPerNode * IdentityModel.IntModelNodes, IdentityModel.IntDOFPerNode * IdentityModel.IntModelNodes];
            MatrixTransformation.SetOnMatrixDiagonal(Id, IdentityModel.Support.IntMatrixSupportDOFs);

            Ip = new int[IdentityModel.IntDOFPerNode * IdentityModel.IntModelNodes, IdentityModel.IntDOFPerNode * IdentityModel.IntModelNodes];
            Ip = MatrixTransformation.SubstractOnMatrixDiagonal(Matrix, Id);
        }

        public Identity Get()
        {
            return this;
        }
    }
}
