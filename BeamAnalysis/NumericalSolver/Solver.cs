using BeamAnalysis.Abstract;
using BeamAnalysis.MatrixHelper;
using System.Diagnostics;

namespace BeamAnalysis.NumericalSolver
{

    /// <summary>
    /// Solver class methods and variables.
    /// Public names in this class implement the signatures defined in interface ISolver.
    /// </summary>
    public class Solver : ISolver
    {
        internal IModel SolverModel { get; private set; }

        /// <summary>
        /// Class constructor method.
        /// </summary>
        /// <param name="model"></param>
        public Solver(IModel model)
        {
            SolverModel = model;
        }

        /*** ISolver interface methods and variables. ***************************************************************************/

        public double[,] DoubleMatrixStiffnessGlobal { get; private set; }

        public double[,] DoubleMatrixBoundaryGlobal { get; private set; }

        public double[,] DoubleMatrixLoads { get; private set; }

        public double[,] DoubleMatrixDisplacements { get; private set; }

        public double[,] DoubleMatrixReactions { get; private set; }

        public void AssembleFiniteElementModel()
        {
            AggregateGlobalStiffnessMatrix();
            ApplyBCs();
            SetSubstitutiveMatrix();
            DetermineParticularEquations();
        }

        public void Run()
        {
            DoubleMatrixDisplacements = Solve(rows);
            DoubleMatrixReactions = CalculateMatrixReactions();
        }

        /*** Private methods ***************************************************************************************************/

        // Helper fields
        private double[,] equation;
        private double[][] rows;

        private void AggregateGlobalStiffnessMatrix()
        {
            DoubleMatrixStiffnessGlobal = new double[SolverModel.IntDOFPerNode * SolverModel.IntModelNodes, SolverModel.IntDOFPerNode * SolverModel.IntModelNodes];
            double[,] k;

            // Debug.WriteLine($"Solver.cs AggregateGlobalStiffnessMatrix(): Total number of elements in model = SolverModel.IntModelElements={SolverModel.IntModelElements}");
            for (int i = 0; i < SolverModel.IntModelElements; i++)
            {
                k = MatrixTransformation.Multiply(SolverModel.Boolean.MatrixTransposed[i], SolverModel.Beam.Matrix[i]);
                k = MatrixTransformation.Multiply(k, SolverModel.Boolean.Matrix[i]);
                DoubleMatrixStiffnessGlobal = MatrixTransformation.Sum(DoubleMatrixStiffnessGlobal, k);
            }
        }

        private void ApplyBCs()
        {
            DoubleMatrixBoundaryGlobal = new double[SolverModel.IntDOFPerNode * SolverModel.IntModelNodes, SolverModel.IntDOFPerNode * SolverModel.IntModelNodes];

            DoubleMatrixBoundaryGlobal = MatrixTransformation.Multiply(SolverModel.Identity.Ip, DoubleMatrixStiffnessGlobal);
            DoubleMatrixBoundaryGlobal = MatrixTransformation.Multiply(DoubleMatrixBoundaryGlobal, SolverModel.Identity.Ip);
            DoubleMatrixBoundaryGlobal = MatrixTransformation.Sum(DoubleMatrixBoundaryGlobal, MatrixTransformation.ToDouble(SolverModel.Identity.Id));

            DoubleMatrixLoads = new double[SolverModel.IntDOFPerNode * SolverModel.IntModelNodes, 1];
            DoubleMatrixLoads = MatrixTransformation.Multiply(SolverModel.Identity.Ip, SolverModel.Load.Matrix);
        }

        private void SetSubstitutiveMatrix()
        {
            equation = new double[SolverModel.IntDOFPerNode * SolverModel.IntModelNodes, SolverModel.IntDOFPerNode * SolverModel.IntModelNodes + 1];
            equation = MatrixTransformation.Sum(equation, DoubleMatrixBoundaryGlobal);

            for(int i = 0; i < equation.GetLength(0); i++)
            {
                equation[i, equation.GetLength(0)] = DoubleMatrixLoads[i, 0];
            }
        }

        private void DetermineParticularEquations()
        {
            int rowsCount = SolverModel.IntDOFPerNode * SolverModel.IntModelNodes;

            double[][] rows = new double[rowsCount][];

            for (int i = 0; i < rowsCount; i++)
            {
                double[] currentRow = new double[rowsCount + 1];
                for(int y = 0; y < currentRow.GetLength(0); y++)
                {
                    currentRow[y] = equation[i, y];
                }

                rows[i] = currentRow;
            }

            this.rows = rows;
        }

        private double[,] Solve(double[][] rows)
        {
            int length = rows[0].Length;

            for (int i = 0; i < rows.Length - 1; i++)
            {
                if (rows[i][i] == 0 && !Sweep(rows, i, i))
                {
                    return null;
                }

                for (int j = i; j < rows.Length; j++)
                {
                    double[] d = new double[length];
                    for (int x = 0; x < length; x++)
                    {
                        d[x] = rows[j][x];
                        if (rows[j][i] != 0)
                        {
                            d[x] = d[x] / rows[j][i];
                        }
                    }
                    rows[j] = d;
                }

                for (int y = i + 1; y < rows.Length; y++)
                {
                    double[] f = new double[length];
                    for (int g = 0; g < length; g++)
                    {
                        f[g] = rows[y][g];
                        if (rows[y][i] != 0)
                        {
                            f[g] = f[g] - rows[i][g];
                        }

                    }
                    rows[y] = f;
                }
            }

            return GetResult(rows);
        }

        private bool Sweep(double[][] rows, int row, int column)
        {
            bool changed = false;
            double[] temp;
            for (int z = rows.Length - 1; z > row; z--)
            {
                if (rows[z][row] != 0)
                {
                    temp = rows[z];
                    rows[z] = rows[column];
                    rows[column] = temp;
                    changed = true;
                }
            }
            return changed;
        }

        private double[,] GetResult(double[][] rows)
        {
            double val;
            int length = rows[0].Length;
            double[,] doubleMatrix = new double[rows.Length,1];

            for (int i = rows.Length - 1; i >= 0; i--)
            {
                val = rows[i][length - 1];
                for (int x = length - 2; x > i - 1; x--)
                {
                    val -= rows[i][x] * doubleMatrix[x, 0];
                }
                doubleMatrix[i, 0] = val / rows[i][i];
            }
            return doubleMatrix;
        }

        /// <summary>
        /// Calculate and return reaction matrix.
        /// </summary>
        /// <returns></returns>
        private double[,] CalculateMatrixReactions()
        {
            double[,] doubleMatrix;
            doubleMatrix = MatrixTransformation.Multiply(DoubleMatrixStiffnessGlobal, DoubleMatrixDisplacements);
            doubleMatrix = MatrixTransformation.Substract(doubleMatrix, SolverModel.Load.Matrix);
            return doubleMatrix;
        }
    }
}
