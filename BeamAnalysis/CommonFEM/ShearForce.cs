using BeamAnalysis.Abstract;
using BeamAnalysis.MatrixHelper;
using BeamAnalysis.NumericalPostProcessor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BeamAnalysis.CommonFEM
{
    internal static class ShearForce
    {
        /*** Public methods ****************************************************************************************************/

        /// <summary>
        /// Get shear result at doublePosition.
        /// </summary>
        /// <param name="boolIsNode">If true then return then return shear values left and right of doublePosition, otherwise return shear value at doublePosition.</param>
        /// <param name="scheme"></param>
        /// <param name="model"></param>
        /// <param name="elementBoundaryDisplacements"></param>
        /// <param name="doublePosition">Position is distance from left end of beam.</param>
        /// <returns></returns>
        public static ValueResult GetResultShear(bool boolIsNode, IScheme scheme, IModel model, Dictionary<int, double[,]> elementBoundaryDisplacements, double doublePosition)
        {
            double doubleValueLeft;
            double doubleValueRight;
            if (boolIsNode)
            {
                doubleValueLeft = CalculateResultShear(scheme, model, elementBoundaryDisplacements, doublePosition - PostProcessor.doubleOffsetDistance);
                doubleValueRight = CalculateResultShear(scheme, model, elementBoundaryDisplacements, doublePosition + PostProcessor.doubleOffsetDistance);
            }
            else
            {
                doubleValueLeft = CalculateResultShear(scheme, model, elementBoundaryDisplacements, doublePosition);
                doubleValueRight = doubleValueLeft;
            }
            //Debug.WriteLine($"ShearForce.GetResultShear(): boolIsNode={boolIsNode}, doublePosition={doublePosition}, doubleValueLeft={doubleValueLeft}, doubleValueRight={doubleValueRight}");
            return new ValueResult(boolIsNode, doubleValueLeft, doubleValueRight);
        }

        /*** Private methods ***************************************************************************************************/

        private static double CalculateResultShear(IScheme scheme, IModel model, Dictionary<int, double[,]> elementBoundaryDisplacements, double doublePosition)
        {
            ResultHelper.GetBoundaryNodes(scheme, model, doublePosition, out int startNodeNumber, out int endNodeNumber);
            int elementNumber = -1;
            if (startNodeNumber >= 1 && endNodeNumber >= 1)
            {
                elementNumber = ResultHelper.GetElementNumberContainingNodes(scheme, startNodeNumber, endNodeNumber);
            }
            if (elementNumber >= 1)
            {
                return GetAlongTheMemberResult(model, elementBoundaryDisplacements, elementNumber);
            }
            return 0;
        }

        private static double GetAlongTheMemberResult(IModel model, Dictionary<int, double[,]> elementBoundaryDisplacements, int elementNumber)
        {
            IBeamProperty beamProperty = model.Beam.Properties.Where(beam => beam.Number == elementNumber).FirstOrDefault();
            double L = beamProperty.Lb;
            double[,] currentShapeFunctionsVector = GetShapeFunctionsVector(L);
            double[,] result;
            result = MatrixTransformation.Multiply(currentShapeFunctionsVector, elementBoundaryDisplacements[elementNumber]);
            double resultFactor = beamProperty.Ey * beamProperty.Iy / Math.Pow(beamProperty.Lb, 3);
            result = MatrixTransformation.Multiply(result, resultFactor);
            return result[0, 0];
        }

        private static double[,] GetShapeFunctionsVector(double L)
        {
            double[,] currentShapeFunctionVector = new double[1, 4];
            currentShapeFunctionVector[0, 0] = -12;
            currentShapeFunctionVector[0, 1] = -6 * L;
            currentShapeFunctionVector[0, 2] = 12;
            currentShapeFunctionVector[0, 3] = -6 * L;
            return currentShapeFunctionVector;
        }

    }
}
