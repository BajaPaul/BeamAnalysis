using BeamAnalysis.Abstract;
using BeamAnalysis.MatrixHelper;
using BeamAnalysis.NumericalPostProcessor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BeamAnalysis.CommonFEM
{
    internal static class Deflection
    {
        /*** Public methods ********************************************************************************************************/

        public static double GetExactResult(Dictionary<int, double[,]> elementBoundaryDisplacements, INode node)
        {
            double[,] currentNodeDisplacement = elementBoundaryDisplacements[node.IntNumber];
            return currentNodeDisplacement[0,0];
        }

        public static double GetApproximatedResult(IScheme scheme, IModel model, Dictionary<int, double[,]> elementBoundaryDisplacements, double x)
        {
            ResultHelper.GetBoundaryNodes(scheme, model, x, out int startNodeNumber, out int endNodeNumber);
            int elementNumber = -1;
            if (startNodeNumber >= 1 && endNodeNumber >= 1)
            {
                elementNumber = ResultHelper.GetElementNumberContainingNodes(scheme, startNodeNumber, endNodeNumber);
            }
            if (elementNumber >= 1)
            {
                return GetAlongTheMemberResult(scheme, model, elementBoundaryDisplacements, x, startNodeNumber, elementNumber);
            }
            return 0;
        }

        /*** Private methods ***************************************************************************************************/

        private static double GetAlongTheMemberResult(IScheme scheme, IModel model, Dictionary<int, double[,]> elementBoundaryDisplacements, double x, int startNodeNumber, int elementNumber)
        {
            IBeamProperty beamProperty = model.Beam.Properties.Where(beam => beam.Number == elementNumber).FirstOrDefault();
            double L = beamProperty.Lb;
            double s = ResultHelper.GetScalingParameter(scheme, x, startNodeNumber, L);
            double[,] currentShapeFunctionsVector = GetShapeFunctionsVector(s, L);
            double[,] result = MatrixTransformation.Multiply(currentShapeFunctionsVector, elementBoundaryDisplacements[elementNumber]);
            return result[0, 0];
        }
        
        private static double[,] GetShapeFunctionsVector(double s, double L)
        {
            double[,] currentShapeFunctionVector = new double[1, 4];
            currentShapeFunctionVector[0, 0] = 1 - 3 * Math.Pow(s, 2) + 2 * Math.Pow(s, 3);
            currentShapeFunctionVector[0, 1] = L * (s - 2 * Math.Pow(s, 2) + Math.Pow(s, 3));
            currentShapeFunctionVector[0, 2] = 3 * Math.Pow(s, 2) - 2 * Math.Pow(s, 3);
            currentShapeFunctionVector[0, 3] = L * (-1 * Math.Pow(s, 2) + Math.Pow(s, 3));
            return currentShapeFunctionVector;
        }

    }
}
