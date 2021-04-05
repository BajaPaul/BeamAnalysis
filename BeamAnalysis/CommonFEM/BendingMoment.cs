using BeamAnalysis.Abstract;
using BeamAnalysis.MatrixHelper;
using BeamAnalysis.NumericalPostProcessor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BeamAnalysis.CommonFEM
{
    internal static class BendingMoment
    {
        /*** Public methods ****************************************************************************************************/

        /// <summary>
        /// Get moment result at doublePosition.
        /// </summary>
        /// <param name="boolIsNode">If true then return then return moment values left and right of doublePosition, otherwise return moment value at doublePosition.</param>
        /// <param name="scheme"></param>
        /// <param name="model"></param>
        /// <param name="elementBoundaryDisplacements"></param>
        /// <param name="doublePosition">Position is distance from left end of beam.</param>
        /// <returns></returns>
        public static ValueResult GetResultMoment(bool boolIsNode, IScheme scheme, IModel model, Dictionary<int, double[,]> elementBoundaryDisplacements, double doublePosition)
        {
            double doubleValueLeft;
            double doubleValueRight;
            if (boolIsNode)
            {
                doubleValueLeft = CalculateResultMoment(scheme, model, elementBoundaryDisplacements, doublePosition - PostProcessor.doubleOffsetDistance);
                doubleValueRight = CalculateResultMoment(scheme, model, elementBoundaryDisplacements, doublePosition + PostProcessor.doubleOffsetDistance);
            }
            else
            {
                doubleValueLeft = CalculateResultMoment(scheme, model, elementBoundaryDisplacements, doublePosition);
                doubleValueRight = doubleValueLeft;
            }
            //Debug.WriteLine($"BendingMoment.GetResultMoment(): boolIsNode={boolIsNode}, doublePosition={doublePosition}, doubleValueLeft={doubleValueLeft}, doubleValueRight={doubleValueRight}");
            return new ValueResult(boolIsNode, doubleValueLeft, doubleValueRight);
        }

        /*** Private methods ***************************************************************************************************/

        private static double CalculateResultMoment(IScheme scheme, IModel model, Dictionary<int, double[,]> elementBoundaryDisplacements, double doublePosition)
        {
            ResultHelper.GetBoundaryNodes(scheme, model, doublePosition, out int startNodeNumber, out int endNodeNumber);
            int elementNumber = -1;
            if (startNodeNumber >= 1 && endNodeNumber >= 1)
            {
                elementNumber = ResultHelper.GetElementNumberContainingNodes(scheme, startNodeNumber, endNodeNumber);
            }
            if (elementNumber >= 1)
            {
                return GetAlongTheMemberResult(scheme, model, elementBoundaryDisplacements, doublePosition, startNodeNumber, elementNumber);
            }
            return 0;
        }

        private static double GetAlongTheMemberResult(IScheme scheme, IModel model, Dictionary<int, double[,]> elementBoundaryDisplacements, double x, int startNodeNumber, int elementNumber)
        {
            IBeamProperty beamProperty = model.Beam.Properties.Where(beam => beam.Number == elementNumber).FirstOrDefault();
            double L = beamProperty.Lb;
            double s = ResultHelper.GetScalingParameter(scheme, x, startNodeNumber, L);
            double[,] currentShapeFunctionsVector = GetShapeFunctionsVector(s, L);
            double[,] result;
            result = MatrixTransformation.Multiply(currentShapeFunctionsVector, elementBoundaryDisplacements[elementNumber]);
            double resultFactor = beamProperty.Ey * beamProperty.Iy / Math.Pow(beamProperty.Lb, 2);
            result = MatrixTransformation.Multiply(result, resultFactor);
            return result[0, 0];
        }

        private static double[,] GetShapeFunctionsVector(double s, double L)
        {
            double[,] currentShapeFunctionVector = new double[1, 4];
            currentShapeFunctionVector[0, 0] = -6 + 12 * s;
            currentShapeFunctionVector[0, 1] = L * (-4 + 6 * s);
            currentShapeFunctionVector[0, 2] = 6 - 12 * s;
            currentShapeFunctionVector[0, 3] = L * (-2 + 6 * s);
            return currentShapeFunctionVector;
        }

    }
}
