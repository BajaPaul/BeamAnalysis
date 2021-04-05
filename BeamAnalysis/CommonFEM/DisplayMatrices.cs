using BeamAnalysis.Abstract;
using BeamAnalysis.Common;
using System.Text;

// Output strings from this class require a fixed width font to align output vertically. Best choice is Consolas.
// Options are Consolas, Courier New, Lucida Console, Lucida Sans Typewriter.

namespace BeamAnalysis.CommonFEM
{
    /// <summary>
    /// This class creates and outputs matrix data from various Interfaces to a string.
    /// These methods are isolated here since should not require changes for various beam configurations.
    /// </summary>
    public static class DisplayMatrices
    {
        /// <summary>
        /// Enum used to set number of digits to display after decimal.
        /// </summary>
        public enum EnumOutputFormat { Integer, Double1, Double2, Double3, Double4 };


        //*** Public methods ***************************************************************************************************/

        /// <summary>
        /// Return string that contains matrix data from IModel class.
        /// </summary>
        /// <param name="iModel"></param>
        public static string DisplayResultsIModel(IModel iModel)
        {
            string stringOut = "\n\n****** Boolean Matrix (Integer) ******\n";
            foreach (int[,] matrix in iModel.Boolean.Matrix)
            {
                stringOut += DisplayMatrix2D(matrix, EnumOutputFormat.Integer);
            }
            stringOut += "\n****** Boolean Matrix Transposed (Integer) ******\n";
            foreach (int[,] matrix in iModel.Boolean.MatrixTransposed)
            {
                stringOut += DisplayMatrix2D(matrix, EnumOutputFormat.Integer);
            }
            stringOut += "\n****** Beam Matrix (Double1) ******\n";
            foreach (double[,] matrix in iModel.Beam.Matrix)
            {
                stringOut += DisplayMatrix2D(matrix, EnumOutputFormat.Double1);
            }
            stringOut += "\n****** Support Matrix (Integer) ******\n";
            stringOut += DisplayMatrix2D(iModel.Support.IntMatrixSupportDOFs, EnumOutputFormat.Integer);
            stringOut += "\n****** Load Matrix (Double1) ******\n";
            stringOut += DisplayMatrix2D(iModel.Load.Matrix, EnumOutputFormat.Double1);
            stringOut += "\n****** Identity Matrix (Integer) ******\n";
            stringOut += DisplayMatrix2D(iModel.Identity.Matrix, EnumOutputFormat.Integer);
            stringOut += "\n****** Id Matrix (Integer) ******\n";
            stringOut += DisplayMatrix2D(iModel.Identity.Id, EnumOutputFormat.Integer);
            stringOut += "\n****** Ip Matrix (Integer) ******\n";
            stringOut += DisplayMatrix2D(iModel.Identity.Ip, EnumOutputFormat.Integer);
            return stringOut;
        }

        /// <summary>
        /// Return string that contains matrix data from ISolver class.
        /// </summary>
        /// <param name="iSolver"></param>
        public static string DisplayResultsISolver(ISolver iSolver)
        {
            string stringOut =  "\n****** Global stiffness matrix (Double1) ******\n";
            stringOut += DisplayMatrix2D(iSolver.DoubleMatrixStiffnessGlobal, EnumOutputFormat.Double1);
            stringOut += "\n****** Global stiffness matrix with applied boundary conditions (Double1) ******\n";
            stringOut += DisplayMatrix2D(iSolver.DoubleMatrixBoundaryGlobal, EnumOutputFormat.Double1);
            stringOut += "\n****** F matrix (Double1) ******\n";
            stringOut += DisplayMatrix2D(iSolver.DoubleMatrixLoads, EnumOutputFormat.Double1);
            stringOut += "\n****** Results - Displacement matrix (Double4) ******\n";
            stringOut += DisplayMatrix2D(iSolver.DoubleMatrixDisplacements, EnumOutputFormat.Double4);
            stringOut += "\n****** Results - Reactions matrix (Double1) ******\n";
            stringOut += DisplayMatrix2D(iSolver.DoubleMatrixReactions, EnumOutputFormat.Double1);
            return stringOut;
        }

        /// <summary>
        /// Return string that contains matrix data from PostProcessor class.
        /// </summary>
        /// <returns></returns>
        public static string DisplayResultsPostProcessor()
        {
            string stringOut = "\n****** Nodal Forces Matrix for each element (Double1) ******\n";
            for (int i = 1; i <= PostProcessor.ElementBoundaryForces.Count; i++)
            {
                stringOut += DisplayMatrix2D(PostProcessor.ElementBoundaryForces[i], EnumOutputFormat.Double1);
            }
            stringOut += "\n****** Nodal Displacement Matrix for each element (Double4) ******\n";
            for (int i = 1; i <= PostProcessor.ElementBoundaryDisplacements.Count; i++)
            {
                stringOut += DisplayMatrix2D(PostProcessor.ElementBoundaryDisplacements[i], EnumOutputFormat.Double4);
            }
            stringOut += "\n****** Nodal Displacement Matrix for each node (Double4) ******\n";
            for (int i = 1; i <= PostProcessor.NodalDisplacements.Count; i++)
            {
                stringOut += DisplayMatrix2D(PostProcessor.NodalDisplacements[i], EnumOutputFormat.Double4);
            }
            return stringOut;
        }

        //*** Private methods ***************************************************************************************************/

        /// <summary>
        /// Return string that contains formatted matrix data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static string DisplayMatrix2D<T>(T[,] matrix, EnumOutputFormat enumOutputType)
        {
            string stringOut = string.Empty;
            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int y = 0; y <= matrix.GetUpperBound(1); y++)
                {
                    switch (enumOutputType)
                    {
                        case EnumOutputFormat.Integer:
                            stringBuilder.Append($"{matrix[i, y]:0} ");
                            break;
                        case EnumOutputFormat.Double1:
                            stringBuilder.Append($"{matrix[i, y],CommonItems.intPadOutput:0.0} ");
                            break;
                        case EnumOutputFormat.Double2:
                            stringBuilder.Append($"{matrix[i, y],CommonItems.intPadOutput:0.00} ");
                            break;
                        case EnumOutputFormat.Double3:
                            stringBuilder.Append($"{matrix[i, y],CommonItems.intPadOutput:0.000} ");
                            break;
                        case EnumOutputFormat.Double4:
                            stringBuilder.Append($"{matrix[i, y],CommonItems.intPadOutput:0.0000} ");
                            break;
                    }
                }
                if (enumOutputType.Equals(EnumOutputFormat.Integer))
                {
                    stringOut += $"\n           {stringBuilder}";      // Offest integer output with spaces to align with after decimal in double output.
                }
                else
                {
                    stringOut += $"\n{stringBuilder}";
                }
            }
            stringOut += "\n";
            return stringOut;
        }

    }
}
