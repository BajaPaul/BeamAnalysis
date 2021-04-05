using System;

namespace BeamAnalysis.MatrixHelper
{
    public static class MatrixTransformation
    {
        public static void SetIdentityMatrix(int[,] matrix)
        {
            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
            {
                matrix[i, i] = 1;
            }
        }

        public static void SetOnMatrixDiagonal(int[,] matrix, int[,] matrixWithValues)
        {
            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
            {
                matrix[i, i] = matrixWithValues[i, 0];
            }
        }

        public static double[,] Sum(double[,] matrix, double[,] matrixToAdd)
        {
            double[,] sumMatrix = new double[matrix.GetLength(0), matrix.GetLength(1)];

            for (int i = 0; i <= matrixToAdd.GetUpperBound(0); i++)
            {
                for (int y = 0; y <= matrixToAdd.GetUpperBound(1); y++)
                {
                    sumMatrix[i, y] = matrix[i, y] + matrixToAdd[i, y];
                }
            }

            return sumMatrix;
        }

        public static double[,] Substract(double[,] matrix, double[,] matrixToSubstract)
        {
            double[,] currentMatrix = new double[matrix.GetLength(0), matrix.GetLength(1)];

            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
            {
                for (int y = 0; y <= matrix.GetUpperBound(1); y++)
                {
                    currentMatrix[i, y] = matrix[i, y] - matrixToSubstract[i, y];
                }
            }

            return currentMatrix;
        }

        public static int[,] SubstractOnMatrixDiagonal(int[,] matrix, int[,] matrixToSubstract)
        {
            int[,] currentMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];

            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
            {
                currentMatrix[i, i] = matrix[i, i] - matrixToSubstract[i, i];
            }

            return currentMatrix;
        }

        public static double[,] Multiply<T, G>(T[,] A, G[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);

            double temp;
            double[,] matrix = new double[rA, cB];

            if (cA != rB)
            {
                return null;
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += Convert.ToDouble(A[i, k]) * Convert.ToDouble(B[k, j]);
                        }
                        matrix[i, j] = temp;
                    }
                }

                return matrix;
            }
        }

        public static double[,] Multiply(double[,] matrix, double factor)
        {
            double[,] newMatrix = new double[matrix.GetLength(0), matrix.GetLength(1)];

            for (int i = 0; i <= newMatrix.GetUpperBound(0); i++)
            {
                for (int y = 0; y <= newMatrix.GetUpperBound(1); y++)
                {
                    newMatrix[i, y] = factor * matrix[i, y];
                }
            }

            return newMatrix;
        }

        public static G[,] Transpose<T, G>(T[,] matrix)
        {
            int row = matrix.GetLength(0);
            int col = matrix.GetLength(1);

            G[,] transposedMatrix = new G[col, row];

            for (int i = 0; i < row; i++)
            {
                for (int y = 0; y < col; y++)
                {
                    transposedMatrix[y, i] = (G)(object)matrix[i, y];
                }
            }

            return transposedMatrix;
        }

        public static double[,] ToDouble<T>(T[,] matrix)
        {
            double[,] newMatrix = new double[matrix.GetLength(0), matrix.GetLength(1)];

            for (int i = 0; i <= matrix.GetUpperBound(0); i++)
            {
                newMatrix[i, i] = Convert.ToDouble(matrix[i, i]);
            }

            return newMatrix;
        }
    }
}
