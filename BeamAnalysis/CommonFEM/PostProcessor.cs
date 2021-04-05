using BeamAnalysis.Abstract;
using BeamAnalysis.Common;
using BeamAnalysis.MatrixHelper;
using BeamAnalysis.NumericalPostProcessor;
using LibraryCoder.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BeamAnalysis.CommonFEM
{
    public class PostProcessor
    {
        // Constructor parameters.
        public static IScheme PostProcessorScheme;
        public static IModel PostProcessorModel;
        public static ISolver PostProcessorSolver;

        /// <summary>
        /// Class constructor method.
        /// </summary>
        /// <param name="iScheme"></param>
        /// <param name="iModel"></param>
        /// <param name="iSolver"></param>
        public PostProcessor(IScheme iScheme, IModel iModel, ISolver iSolver)
        {
            PostProcessorScheme = iScheme;
            PostProcessorModel = iModel;
            PostProcessorSolver = iSolver;
        }

        /// <summary>
        /// Value of nodal forces matrix for each element.
        /// </summary>
        public static Dictionary<int, double[,]> ElementBoundaryForces;

        /// <summary>
        /// Value of nodal displacement matrix for each element.
        /// </summary>
        public static Dictionary<int, double[,]> ElementBoundaryDisplacements;

        /// <summary>
        /// Value of nodal displacement matrix for each node.
        /// </summary>
        public static Dictionary<int, double[,]> NodalDisplacements;

        /// <summary>
        /// Offset distance used subtract and/or add to a node position.
        /// Result calculations are returned left and right of position by this distance. Current value 0.0001.
        /// </summary>
        public const double doubleOffsetDistance = 0.0001;

        /// <summary>
        /// Offset distance used subtract and/or add to a node position.
        /// If load is on a support then it will be offset by this distance to allow calculation or exception will occur.
        /// This value MUST be two times the value of doubleOffsetDistance. Current value 0.0002.
        /// </summary>
        public const double doubleOffsetDistanceSupport = 0.0002;

        /*** Public methods ****************************************************************************************************/

        /// <summary>
        /// Calculate nodal matrices.
        /// </summary>
        public static void CalculateNodalResults()
        {
            ElementBoundaryForces = CalculateElementBoundaryForces();
            ElementBoundaryDisplacements = CalculateElementBoundaryDisplacements();
            NodalDisplacements = CalculateNodalDisplacements();
        }

        /// <summary>
        /// Return dictionary that contains plot values of shear results. Dictionary values are in form of (position, shear).
        /// </summary>
        /// <param name="intSegments">Divide beam length by this value to set default interval length to calculate output coordinates.</param>
        /// <returns></returns>
        public static Dictionary<double, double> GetDictionaryShear(int intSegments)
        {
            List<INode> listINode = PostProcessorScheme.ListINode;
            int intINodes = listINode.Count;
            // Get doubleBeamLength from last node found in PostProcessorScheme via index.
            double doubleBeamLength = listINode[intINodes - 1].DoubleNodePosition;
            //Debug.WriteLine($"\n\nPostProcessor.GetDictionaryShear(): doubleBeamLength={doubleBeamLength}, intSegments={intSegments}");
            Dictionary<double, double> dictionaryShear = new Dictionary<double, double>();
            List<double> listLoadsOnSupports = new List<double>();
            ValueResult valueResult;
            double doublePosition;      // Position on beam from left to right.
            double doublePositionLeft;
            double doublePositionRight;
            double doubleValueResultLeft;
            double doubleValueResultRight;
            /* Calculate and add intermediate results derived from value of intSegments.
             * These values do not include beam endpoints, supports, and loads. Those values are added below.
             * These values need to be added to dictionaryShear first since may be removed or overwritten below. */
            for (int i = 1; i < intSegments; i++)
            {
                doublePosition = Convert.ToDouble(i) / Convert.ToDouble(intSegments) * doubleBeamLength;   // Ratio multiplied by beam length.
                valueResult = GetShearAtPosition(doublePosition);
                if (valueResult.DoubleValueDifference == 0d)
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): doublePosition={doublePosition}, valueResult.DoubleValueDifference={valueResult.DoubleValueDifference}, DoubleValueLeft={valueResult.DoubleValueLeft} equals DoubleValueRight={valueResult.DoubleValueRight}");
                    dictionaryShear.Add(doublePosition, valueResult.DoubleValueLeft);
                }
                else if (LibNum.EqualByRounding(valueResult.DoubleValueLeft, valueResult.DoubleValueRight, CommonItems.intRoundDigits))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): doublePosition={doublePosition}, valueResult.DoubleValueDifference!=0d, DoubleValueLeft={valueResult.DoubleValueLeft}, DoubleValueRight={valueResult.DoubleValueRight}");
                    dictionaryShear.Add(doublePosition, (valueResult.DoubleValueLeft + valueResult.DoubleValueRight) / 2d);
                }
                else
                {
                    // Output left and right values.
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): doublePosition={doublePosition}, DoubleValueLeft={valueResult.DoubleValueLeft}, DoubleValueRight={valueResult.DoubleValueRight}");
                    dictionaryShear.Add(doublePosition - doubleOffsetDistance, valueResult.DoubleValueLeft);
                    dictionaryShear.Add(doublePosition + doubleOffsetDistance, valueResult.DoubleValueRight);
                }
            }
            // Next line is debug code so comment out when done debugging.
            //DebugSamples.DebugShowLibraryContents(dictionaryShear, "GetDictionaryShear", "Added intermediate value @");
            //
            /* Now add dictionaryShear values for all nodes in listINode. This will add values at beam endpoints. */
            for (int i = 0; i < intINodes; i++)
            {
                //Debug.WriteLine($"\nPostProcessor.GetDictionaryShear(): For loop: [i]={i}, IntNumber={listINode[i].IntNumber}, DoubleNodePosition={listINode[i].DoubleNodePosition}, BoolNodeSupport={listINode[i].BoolNodeSupport}");
                doublePosition = listINode[i].DoubleNodePosition;
                doublePositionLeft = doublePosition - doubleOffsetDistance;
                doublePositionRight = doublePosition + doubleOffsetDistance;
                valueResult = GetShearAtPosition(doublePosition);
                doubleValueResultLeft = valueResult.DoubleValueLeft;
                doubleValueResultRight = valueResult.DoubleValueRight;
                //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): valueResult values for i={i}, doublePosition={doublePosition}: doublePositionLeft={doublePositionLeft}, doublePositionRight={doublePositionRight}, doubleValueResultLeft={doubleValueResultLeft}, doubleValueResultRight={doubleValueResultRight}");
                if (LibNum.EqualByRounding(doublePosition, 0d, CommonItems.intRoundDigits))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Node is beam start point so add result, i={i}, doublePosition={doublePosition}");
                    // Use right result value but set it's position to 0d.
                    dictionaryShear.Add(0d, doubleValueResultRight);
                }
                else if (LibNum.EqualByRounding(doublePosition, doubleBeamLength, CommonItems.intRoundDigits))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Node is beam end point so add result, i={i}, doublePosition={doublePosition}");
                    // Use left result value but set it's position to doubleBeamLength.
                    dictionaryShear.Add(doubleBeamLength, doubleValueResultLeft);
                }
                else if (LibNum.EqualByRounding(doublePosition, doubleOffsetDistanceSupport, CommonItems.intRoundDigits))
                {
                    // Node position indicates this load was placed on support at beam start point and was offset to allow calculation.
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Node position indicates this load was placed on support at beam start point and was offset to allow calculation, i={i}, doublePosition={doublePosition}");
                    dictionaryShear.Add(doublePositionLeft, doubleValueResultRight);
                }
                else if (LibNum.EqualByRounding(doublePosition, (doubleBeamLength - doubleOffsetDistanceSupport), CommonItems.intRoundDigits))
                {
                    // Node position indicates this load was placed on support at beam end point and was offset to allow calculation.
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Node position indicates this load was placed on support at beam end point and was offset to allow calculation. i={i}, doublePosition={doublePosition}");
                    dictionaryShear.Add(doublePositionRight, doubleValueResultLeft);
                }
                else if (listINode[i].BoolNodeSupport)
                {
                    // Node position is an intermediate support so add values left and right of support position.
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Node position is an intermediate support so add values left and right of support position. i={i}, doublePosition={doublePosition}");
                    /* If exist, remove KeyValuePairs at key positions of doublePosition, doublePositionLeft, and doublePositionRight 
                     * from dictionaryShear since they will be added again below. Key doublePosition is a shear transistion position so
                     * do not show results there. */
                    RemoveDictionaryPositions(dictionaryShear, doublePosition, doublePositionLeft, doublePositionRight);
                    dictionaryShear.Add(doublePositionLeft, doubleValueResultLeft);
                    dictionaryShear.Add(doublePositionRight, doubleValueResultRight);
                }
                else if (IsConcentratedLoad(listINode[i]))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Node is a concentrated load. i={i}, doublePosition={doublePosition}");
                    /* If exist, remove KeyValuePairs at key positions of doublePosition, doublePositionLeft, and doublePositionRight 
                     * from dictionaryShear since they will be added again below. Key doublePosition is a shear transistion position so
                     * do not show results there. */
                    RemoveDictionaryPositions(dictionaryShear, doublePosition, doublePositionLeft, doublePositionRight);
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Node is an intermediate load so check if it was originally on a support. i={i}, doublePosition={doublePosition}");
                    double doubleDifference = listINode[i + 1].DoubleNodePosition - doublePosition;
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): i={i}, doubleDifference={doubleDifference}");
                    if (LibNum.EqualByRounding(doubleDifference, doubleOffsetDistanceSupport, CommonItems.intRoundDigits))
                    {
                        // Load was originally on a support but was offset to allow calculation.
                        // Add support position to listLoadsOnSupports for additional proccessing below.
                        //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Add value to listLoadsOnSupports of {doublePosition + doubleOffsetDistanceSupport}");
                        listLoadsOnSupports.Add(doublePosition + doubleOffsetDistanceSupport);
                    }
                    else
                    {
                        doubleDifference = doublePosition - listINode[i - 1].DoubleNodePosition;
                        //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): i={i}, doubleDifference={doubleDifference}");
                        if (LibNum.EqualByRounding(doubleDifference, doubleOffsetDistanceSupport, CommonItems.intRoundDigits))
                        {
                            // Load was originally on a support but was offset to allow calculation.
                            // Add support position to listLoadsOnSupports for additional proccessing below.
                            //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Add value to listLoadsOnSupports of {doublePosition - doubleOffsetDistanceSupport}");
                            listLoadsOnSupports.Add(doublePosition - doubleOffsetDistanceSupport);
                        }
                    }
                    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Intermediate node so add result values slightly left and right of position, i={i}, doublePosition={doublePosition}");
                    dictionaryShear.Add(doublePositionLeft, doubleValueResultLeft);
                    dictionaryShear.Add(doublePositionRight, doubleValueResultRight);
                }
                //else    // Node is a uniform load value so did not add value.
                //{
                //    //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Node is a uniform load value so did not add value. i={i}, doublePosition={doublePosition}");
                //}
                //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): End of 'for' loop.\n");
            }
            if (listLoadsOnSupports.Count > 0)
            {
                //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Found load on intermediate support since listLoadsOnSupports.Count={listLoadsOnSupports.Count} > 0");
                LoadOnIntermediateSupport(listLoadsOnSupports, ref dictionaryShear);
            }
            // Next line is debug code so comment out when done debugging.
            //DebugSamples.DebugShowLibraryContents(dictionaryShear, "GetDictionaryShear", "Values before final sort");
            IEnumerable<KeyValuePair<double, double>> sortedShearCollection = dictionaryShear.OrderBy(i => i.Key);
            //Debug.WriteLine($"PostProcessor.GetDictionaryShear(): Sorted final values in dictionaryShear by position from left to right");
            return sortedShearCollection.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Return dictionary that contains plot values of moment results. Dictionary values are in form of (position, moment).
        /// </summary>
        /// <param name="intSegments">Divide beam length by this value to set default interval length to calculate output coordinates.</param>
        /// <returns></returns>
        public static Dictionary<double, double> GetDictionaryMoment(int intSegments)
        {
            List<INode> listINode = PostProcessorScheme.ListINode;
            int intINodes = listINode.Count;
            // Get doubleBeamLength from last node found in PostProcessorScheme via index.
            double doubleBeamLength = listINode[intINodes - 1].DoubleNodePosition;
            //Debug.WriteLine($"\n\nPostProcessor.GetDictionaryMoment(): doubleBeamLength={doubleBeamLength}, intSegments={intSegments}");
            Dictionary<double, double> dictionaryMoment = new Dictionary<double, double>();
            List<double> listLoadsOnSupports = new List<double>();
            ValueResult valueResult;
            double doublePosition;      // Position on beam from left to right.
            double doublePositionLeft;
            double doublePositionRight;
            double doubleValueResultLeft;
            double doubleValueResultRight;
            /* Calculate and add intermediate results derived from value of intSegments.
             * These values do not include beam endpoints, supports, and loads. These values are added below.
             * These values need to be added to dictionaryMoment first since may be removed or overwritten below. */
            for (int i = 1; i < intSegments; i++)
            {
                doublePosition = Convert.ToDouble(i) / Convert.ToDouble(intSegments) * doubleBeamLength;   // Ratio multiplied by beam length.
                valueResult = GetMomentAtPosition(doublePosition);
                if (valueResult.DoubleValueDifference == 0d)
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): doublePosition={doublePosition}, valueResult.DoubleValueDifference={valueResult.DoubleValueDifference}, DoubleValueLeft={valueResult.DoubleValueLeft} equals DoubleValueRight={valueResult.DoubleValueRight}");
                    dictionaryMoment.Add(doublePosition, valueResult.DoubleValueLeft);
                }
                else if (LibNum.EqualByRounding(valueResult.DoubleValueLeft, valueResult.DoubleValueRight, CommonItems.intRoundDigits))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): doublePosition={doublePosition}, valueResult.DoubleValueDifference!=0d, DoubleValueLeft={valueResult.DoubleValueLeft}, DoubleValueRight={valueResult.DoubleValueRight}");
                    dictionaryMoment.Add(doublePosition, (valueResult.DoubleValueLeft + valueResult.DoubleValueRight) / 2d);
                }
                else
                {
                    // Output left and right values.
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): doublePosition={doublePosition}, DoubleValueLeft={valueResult.DoubleValueLeft}, DoubleValueRight={valueResult.DoubleValueRight}");
                    dictionaryMoment.Add(doublePosition - doubleOffsetDistance, valueResult.DoubleValueLeft);
                    dictionaryMoment.Add(doublePosition + doubleOffsetDistance, valueResult.DoubleValueRight);
                }
            }
            // Next line is debug code so comment out when done debugging.
            //DebugSamples.DebugShowLibraryContents(dictionaryMoment, "GetDictionaryMoment", "Intermediate values added @");
            //
            /* Now add dictionaryMoment values for all nodes in listINode. This will add values at beam endpoints. */
            for (int i = 0; i < intINodes; i++)
            {
                //Debug.WriteLine($"\nPostProcessor.GetDictionaryMoment():  For loop: [i]={i}, IntNumber={listINode[i].IntNumber}, DoubleNodePosition={listINode[i].DoubleNodePosition}, BoolNodeSupport={listINode[i].BoolNodeSupport}");
                doublePosition = listINode[i].DoubleNodePosition;
                doublePositionLeft = doublePosition - doubleOffsetDistance;
                doublePositionRight = doublePosition + doubleOffsetDistance;
                //
                valueResult = GetMomentAtPosition(doublePosition);
                doubleValueResultLeft = valueResult.DoubleValueLeft;
                doubleValueResultRight = valueResult.DoubleValueRight;
                //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): valueResult values for i={i}, doublePosition={doublePosition}: doublePositionLeft={doublePositionLeft}, doublePositionRight={doublePositionRight}, doubleValueResultLeft={doubleValueResultLeft}, doubleValueResultRight={doubleValueResultRight}");
                if (LibNum.EqualByRounding(doublePosition, 0d, CommonItems.intRoundDigits))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Node is beam start point so add result, i={i}, doublePosition={doublePosition}, BoolNodeSupport={listINode[i].BoolNodeSupport}");
                    double doubleValueResult = doubleValueResultLeft;
                    if (listINode[i].BoolNodeSupport && listINode[i].IntNodeDofRotation == 1)
                    {
                        // If node is support and rotation is restrained at support. Use doubleValueResultRight but set it's position to 0d.
                        doubleValueResult = doubleValueResultRight;
                    }
                    // If node is support and rotation is not restrained at support. Use doubleValueResultLeft but set it's position to 0d.
                    // If node is not support. Use doubleValueResultLeft but set it's position to 0d.
                    dictionaryMoment.Add(0d, doubleValueResult);
                }
                else if (LibNum.EqualByRounding(doublePosition, doubleBeamLength, CommonItems.intRoundDigits))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Node is beam end point so add result, i={i}, doublePosition={doublePosition}, BoolNodeSupport={listINode[i].BoolNodeSupport}");
                    double doubleValueResult = doubleValueResultRight;
                    if (listINode[i].BoolNodeSupport && listINode[i].IntNodeDofRotation == 1)
                    {
                        // If node is support and rotation is restrained at support. Use doubleValueResultLeft but set it's position to doubleBeamLength.
                        doubleValueResult = doubleValueResultLeft;
                    }
                    // If node is support and rotation is not restrained at support. Use doubleValueResultRight but set it's position to doubleBeamLength.
                    // If node is not support. Use doubleValueResultRight but set it's position to doubleBeamLength.
                    dictionaryMoment.Add(doubleBeamLength, doubleValueResult);
                }
                else if (LibNum.EqualByRounding(doublePosition, doubleOffsetDistanceSupport, CommonItems.intRoundDigits))
                {
                    // Node position indicates this load was placed on support at beam start point and was offset to allow calculation.
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Node position indicates this load was placed on support at beam start point and was offset to allow calculation, i={i}, doublePosition={doublePosition}");
                    dictionaryMoment.Add(doublePositionLeft, doubleValueResultRight);
                }
                else if (LibNum.EqualByRounding(doublePosition, (doubleBeamLength - doubleOffsetDistanceSupport), CommonItems.intRoundDigits))
                {
                    // Node position indicates this load was placed on support at beam end point and was offset to allow calculation.
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Node position indicates this load was placed on support at beam end point and was offset to allow calculation., i={i}, doublePosition={doublePosition}");
                    dictionaryMoment.Add(doublePositionRight, doubleValueResultLeft);
                }
                else if (listINode[i].BoolNodeSupport)
                {
                    // Node position is an intermediate support so add values left and right of support position.
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Node position is an intermediate support so add values left and right of support position. i={i}, doublePosition={doublePosition}");
                    /* If exist, remove KeyValuePairs at key positions of doublePosition, doublePositionLeft, and doublePositionRight 
                     * from dictionaryMoment since they will be added again below. Key doublePosition is a moment transistion position so
                     * do not show results there. */
                    RemoveDictionaryPositions(dictionaryMoment, doublePosition, doublePositionLeft, doublePositionRight);
                    dictionaryMoment.Add(doublePositionLeft, doubleValueResultLeft);
                    dictionaryMoment.Add(doublePositionRight, doubleValueResultRight);
                }
                else if (IsConcentratedLoad(listINode[i]))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Node is a concentrated load. i={i}, doublePosition={doublePosition}");
                    /* If exist, remove KeyValuePairs at key positions of doublePosition, doublePositionLeft, and doublePositionRight 
                     * from dictionaryMoment since they will be added again below. Key doublePosition is a moment transistion position so
                     * do not show results there. */
                    RemoveDictionaryPositions(dictionaryMoment, doublePosition, doublePositionLeft, doublePositionRight);
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Node is an intermediate load so check if it was originally on a support. i={i}, doublePosition={doublePosition}");
                    double doubleDifference = listINode[i + 1].DoubleNodePosition - doublePosition;
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): i={i}, doubleDifferenceNext={doubleDifference}");
                    if (LibNum.EqualByRounding(doubleDifference, doubleOffsetDistanceSupport, CommonItems.intRoundDigits))
                    {
                        // Load was originally on a support but was offset to allow calculation.
                        // Add support position to listLoadsOnSupports for additional proccessing below.
                        listLoadsOnSupports.Add(doublePosition + doubleOffsetDistanceSupport);
                    }
                    else
                    {
                        doubleDifference = doublePosition - listINode[i - 1].DoubleNodePosition;
                        //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): i={i}, doubleDifference={doubleDifference}");
                        if (LibNum.EqualByRounding(doubleDifference, doubleOffsetDistanceSupport, CommonItems.intRoundDigits))
                        {
                            // Load was originally on a support but was offset to allow calculation.
                            // Add support position to listLoadsOnSupports for additional proccessing below.
                            listLoadsOnSupports.Add(doublePosition - doubleOffsetDistanceSupport);
                        }
                    }
                    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Intermediate node so add result values slightly left and right of position, i={i}, doublePosition={doublePosition}");
                    dictionaryMoment.Add(doublePositionLeft, doubleValueResultLeft);
                    dictionaryMoment.Add(doublePositionRight, doubleValueResultRight);
                }
                //else    // Node is a uniform load value so did not add value.
                //{
                //    //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Node is a uniform load value so did not add value. i={i}, doublePosition={doublePosition}");
                //}
                //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): End of 'for' loop.\n");
            }
            if (listLoadsOnSupports.Count > 0)
            {
                //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Found load on intermediate support since listLoadsOnSupports.Count={listLoadsOnSupports.Count} > 0");
                LoadOnIntermediateSupport(listLoadsOnSupports, ref dictionaryMoment);
            }
            // Next line is debug code so comment out when done debugging.
            //DebugSamples.DebugShowLibraryContents(dictionaryMoment, "GetDictionaryMoment", "Values before final sort");
            IEnumerable<KeyValuePair<double, double>> sortedMomentCollection = dictionaryMoment.OrderBy(i => i.Key);
            //Debug.WriteLine($"PostProcessor.GetDictionaryMoment(): Sorted final values in dictionaryMoment by position from left to right");
            return sortedMomentCollection.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Return dictionary that contains plot values of deflection results. Dictionary values are in form of (position, deflection).
        /// </summary>
        /// <param name="intSegments">Divide beam length by this value to set default interval length to calculate output coordinates.</param>
        /// <returns></returns>
        public static Dictionary<double, double> GetDictionaryDeflection(int intSegments)
        {
            List<INode> listINode = PostProcessorScheme.ListINode;
            int intINodes = listINode.Count;
            // Get doubleBeamLength from last node found in PostProcessorScheme via index.
            double doubleBeamLength = listINode[intINodes - 1].DoubleNodePosition;
            //Debug.WriteLine($"\n\nPostProcessor.GetDictionaryDeflection(): doubleBeamLength={doubleBeamLength}, intSegments={intSegments}");
            Dictionary<double, double> dictionaryDeflection = new Dictionary<double, double>();
            double doublePosition;      // Position on beam from left to right.
            double doubleResultDeflection;
            /* Calculate and add intermediate results derived from value of intSegments.
             * These values do not include beam endpoints, supports, and loads. These values are added below.
             * These values need to be added to dictionaryDeflection first since may be removed or overwritten below. */
            for (int i = 1; i < intSegments; i++)
            {
                doublePosition = Convert.ToDouble(i) / Convert.ToDouble(intSegments) * doubleBeamLength;   // Ratio multiplied by beam length.
                doubleResultDeflection = GetDeflectionAtPosition(doublePosition);
                dictionaryDeflection.Add(doublePosition, doubleResultDeflection);
            }
            // Next line is debug code so comment out when done debugging.
            //DebugSamples.DebugShowLibraryContents(dictionaryDeflection, "GetDictionaryDeflection", "Intermediate values added");
            /* Now add dictionaryDeflection values for all nodes in listINode. This will add values at beam endpoints. */
            for (int i = 0; i < intINodes; i++)
            {
                //Debug.WriteLine($"\nPostProcessor.GetDictionaryDeflection(): For loop: [i]={i}, IntNumber={listINode[i].IntNumber}, DoubleNodePosition={listINode[i].DoubleNodePosition}, BoolNodeSupport={listINode[i].BoolNodeSupport}");
                doublePosition = listINode[i].DoubleNodePosition;
                doubleResultDeflection = GetDeflectionAtPosition(doublePosition);
                //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): doubleResultDeflection value for i={i} and doublePosition={doublePosition}: doubleResultDeflection={doubleResultDeflection}");
                if (LibNum.EqualByRounding(doublePosition, 0d, CommonItems.intRoundDigits))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): Node is beam start point so add result. i={i}, doublePosition={doublePosition}");
                    dictionaryDeflection.Add(0d, doubleResultDeflection);
                }
                else if (LibNum.EqualByRounding(doublePosition, doubleBeamLength, CommonItems.intRoundDigits))
                {
                   //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): Node is beam end point so add result. i={i}, doublePosition={doublePosition}");
                    dictionaryDeflection.Add(doubleBeamLength, doubleResultDeflection);
                }
                else if (listINode[i].BoolNodeSupport)
                {
                    // Node position is an intermediate support so add value of support position.
                    //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): Node position is an intermediate support so add value. i={i}, doublePosition={doublePosition}");
                    /* If exist, remove KeyValuePair at key position of doublePosition from dictionaryDeflection since will be added again below. 
                     * Key doublePosition is a deflection transistion position so show results there. */
                    RemoveDictionaryPositions(dictionaryDeflection, doublePosition, doublePosition, doublePosition);
                    dictionaryDeflection.Add(doublePosition, doubleResultDeflection);
                }
                else if (IsConcentratedLoad(listINode[i]))
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): Node is a concentrated load. i={i}, doublePosition={doublePosition}");
                    /* If exist, remove KeyValuePair at key position of doublePosition from dictionaryDeflection since will be added again below. 
                     * Key doublePosition is a deflection transistion position so show results there. */
                    RemoveDictionaryPositions(dictionaryDeflection, doublePosition, doublePosition, doublePosition);
                    //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): Node is an intermediate load so check if it was originally on a support. i={i}, doublePosition={doublePosition}");
                    bool boolDictionaryDeflectionAddValue = true;
                    double doubleDifferenceLeft = listINode[i + 1].DoubleNodePosition - doublePosition;
                    //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): i={i}, doubleDifferenceLeft={doubleDifferenceLeft}");
                    if (LibNum.EqualByRounding(doubleDifferenceLeft, doubleOffsetDistanceSupport, CommonItems.intRoundDigits))
                    {
                        // Load was originally on a support but was offset to allow calculation.
                        // Do not add this load position since was on a support.
                        boolDictionaryDeflectionAddValue = false;
                    }
                    else
                    {
                        double doubleDifferenceRight = doublePosition - listINode[i - 1].DoubleNodePosition;
                        //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): i={i}, doubleDifferenceRight={doubleDifferenceRight}");
                        if (LibNum.EqualByRounding(doubleDifferenceRight, doubleOffsetDistanceSupport, CommonItems.intRoundDigits))
                        {
                            // Load was originally on a support but was offset to allow calculation.
                            // Do not add this load position since was on a support.
                            boolDictionaryDeflectionAddValue = false;
                        }
                    }
                    //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): boolDictionaryDeflectionAddValue={boolDictionaryDeflectionAddValue}");
                    if (boolDictionaryDeflectionAddValue)
                    {
                        //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): Intermediate node so add result value., i={i}, doublePosition={doublePosition}");
                        dictionaryDeflection.Add(doublePosition, doubleResultDeflection);
                    }
                }
                else    // Node is a uniform load value so did not add value.
                {
                    //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): Node is a uniform load value so did not add value. i={i}, doublePosition={doublePosition}");
                }
                //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): End of 'for' loop.\n");
            }
            // Next line is debug code so comment out when done debugging.
            //DebugSamples.DebugShowLibraryContents(dictionaryDeflection, "GetDictionaryDeflection", "Values before final sort");
            //
            IEnumerable<KeyValuePair<double, double>> sortedShearDeflectionCollection = dictionaryDeflection.OrderBy(i => i.Key);
            //Debug.WriteLine($"PostProcessor.GetDictionaryDeflection(): Sorted final values in dictionaryDeflection by position from left to right");
            return sortedShearDeflectionCollection.ToDictionary(x => x.Key, x => x.Value);
        }

        /*** Private methods ***************************************************************************************************/

        /// <summary>
        /// Load was placed on an intermediate support and was offset slightly left and right to allow calculation. This code will cleanup redundant values.
        /// </summary>
        /// <param name="listLoadsOnSupports">List of load positions found on intermediate supports.</param>
        /// <param name="dictionaryType">Dictionary Type: dictionaryShear or dictionaryMoment.</param>
        private static void LoadOnIntermediateSupport(List<double> listLoadsOnSupports, ref Dictionary<double, double> dictionaryType)
        {
            int intLoadsOnSupports = listLoadsOnSupports.Count;
            //Debug.WriteLine($"PostProcessor.LoadOnIntermediateSupport(): Start of method, intLoadsOnSupports={intLoadsOnSupports}");
            if (intLoadsOnSupports > 0)
            {
                // Sort current values in dictionaryType by position.
                IEnumerable<KeyValuePair<double, double>> sortedCollectionLoadsOnSupports = dictionaryType.OrderBy(i => i.Key);
                dictionaryType = sortedCollectionLoadsOnSupports.ToDictionary(x => x.Key, x => x.Value);
                // Next line is debug code so comment out when done debugging.
                //DebugSamples.DebugShowLibraryContents(dictionaryType, "LoadOnIntermediateSupport", "Values before method start.");
                //
                // Found one or more loads on intermediate support.
                //Debug.WriteLine($"\nPostProcessor.LoadOnIntermediateSupport(): intLoadsOnSupports={intLoadsOnSupports} > 0, so found one or more loads on intermediate supports so clean up results.");
                double doublePosition;
                double doublePositionLeft;
                double doublePositionRight;
                double doublePositionFarLeft;
                double doublePositionFarRight;
                double doubleValueResultLeft;
                double doubleValueResultRight;
                for (int j = 0; j < intLoadsOnSupports; j++)
                {
                    //Debug.WriteLine($"\nPostProcessor.LoadOnIntermediateSupport(): listLoadsOnSupports[{j}]={listLoadsOnSupports[j]}");
                    int intNext = j + 1;
                    if (intNext < intLoadsOnSupports)
                    {
                        // NOTE: listLoadsOnSupports should have duplicate values if a load was placed on an intermediate support. Otherwise load not placed on a support.
                        if (listLoadsOnSupports[j] == listLoadsOnSupports[intNext])
                        {
                            // Find duplicate support position values and cleanup result values.
                            doublePosition = listLoadsOnSupports[j];
                            doublePositionLeft = doublePosition - doubleOffsetDistance;
                            doublePositionRight = doublePosition + doubleOffsetDistance;
                            doublePositionFarLeft = doublePositionLeft - doubleOffsetDistanceSupport;
                            doublePositionFarRight = doublePositionRight + doubleOffsetDistanceSupport;
                            //Debug.WriteLine($"PostProcessor.LoadOnIntermediateSupport(): doublePosition={doublePosition}, doublePositionLeft={doublePositionLeft}, doublePositionRight={doublePositionRight}, doublePositionFarLeft={doublePositionFarLeft}, doublePositionFarRight={doublePositionFarRight}");
                            doubleValueResultLeft = 0d;
                            doubleValueResultRight = 0d;
                            /* If exist, remove KeyValuePairs at key positions of doublePositionFarLeft, doublePositionFarRight, doublePositionLeft, and doublePositionRight
                             * from dictionaryMoment since adjust values will be added below. Copy keys in dictionaryMoment to a new list to scan. */
                            List<double> dictionaryKeys = new List<double>(dictionaryType.Keys);
                            double doubleDictionaryKey;
                            int intDictionaryKeys = dictionaryKeys.Count;
                            for (int k = 0; k < intDictionaryKeys; k++)
                            {
                                doubleDictionaryKey = dictionaryKeys[k];
                                if (LibNum.EqualByRounding(doubleDictionaryKey, doublePositionFarLeft, 4))
                                {
                                    doubleValueResultLeft = dictionaryType.GetValueOrDefault(doubleDictionaryKey);
                                    //Debug.WriteLine($"PostProcessor.LoadOnIntermediateSupport(): doubleDictionaryKey={doubleDictionaryKey} == doublePositionFarLeft={doublePositionFarLeft}, doubleValueResultLeft={doubleValueResultLeft}, removed KeyValuePair @ key {doubleDictionaryKey}");
                                    dictionaryType.Remove(doubleDictionaryKey);
                                }
                                else if (LibNum.EqualByRounding(doubleDictionaryKey, doublePositionFarRight, 4))
                                {
                                    doubleValueResultRight = dictionaryType.GetValueOrDefault(doubleDictionaryKey);
                                    //Debug.WriteLine($"PostProcessor.LoadOnIntermediateSupport(): doubleDictionaryKey={doubleDictionaryKey} == doublePositionFarRight={doublePositionFarRight}, doubleValueResultRight={doubleValueResultRight}, removed KeyValuePair @ key {doubleDictionaryKey}");
                                    dictionaryType.Remove(doubleDictionaryKey);
                                }
                                else if (LibNum.EqualByRounding(doubleDictionaryKey, doublePositionLeft, 4))
                                {
                                    double doubleTempLeft = dictionaryType.GetValueOrDefault(doubleDictionaryKey);
                                    //Debug.WriteLine($"PostProcessor.LoadOnIntermediateSupport(): doubleDictionaryKey={doubleDictionaryKey} == doublePositionLeft={doublePositionLeft}, doubleTempLeft={doubleTempLeft}, removed KeyValuePair @ key {doubleDictionaryKey}");
                                    dictionaryType.Remove(doubleDictionaryKey);
                                }
                                else if(LibNum.EqualByRounding(doubleDictionaryKey, doublePositionRight, 4))    // Rounding error occurring here so round to 4 decimals.
                                {
                                    double doubleTempRight = dictionaryType.GetValueOrDefault(doubleDictionaryKey);
                                    //Debug.WriteLine($"PostProcessor.LoadOnIntermediateSupport(): doubleDictionaryKey={doubleDictionaryKey} == doublePositionRight={doublePositionRight}, doubleTempRight={doubleTempRight}, removed KeyValuePair @ key {doubleDictionaryKey}");
                                    dictionaryType.Remove(doubleDictionaryKey);
                                }
                                //else
                                //{
                                //    Debug.WriteLine($"PostProcessor.LoadOnIntermediateSupport(): Skip since no match found at doubleDictionaryKey={doubleDictionaryKey}");
                                //}
                            }
                            dictionaryKeys.Clear();
                            //Debug.WriteLine($"PostProcessor.LoadOnIntermediateSupport(): Add values: ({doublePositionLeft},{doubleValueResultLeft}) and ({doublePositionRight},{doubleValueResultRight})");
                            dictionaryType.Add(doublePositionLeft, doubleValueResultLeft);
                            dictionaryType.Add(doublePositionRight, doubleValueResultRight);
                        }
                    }
                    //else
                    //{
                    //    // Reached last item in list.
                    //    //Debug.WriteLine($"PostProcessor.LoadOnIntermediateSupport(): Reached last item in list, j={j}, intNext={intNext}, intLoadsOnSupports={intLoadsOnSupports}");
                    //}
                }
            }
            // Sort current values in dictionaryType by position.
            IEnumerable<KeyValuePair<double, double>> sortedShearCollectionLoadsOnSupports = dictionaryType.OrderBy(i => i.Key);
            dictionaryType = sortedShearCollectionLoadsOnSupports.ToDictionary(x => x.Key, x => x.Value);
            // Next line is debug code so comment out when done debugging.
            //DebugSamples.DebugShowLibraryContents(dictionaryType, "LoadOnIntermediateSupport", "Values before method end.");
        }

        /// <summary>
        /// Get shear result at doublePosition. Method checks if position is node.
        /// </summary>
        /// <param name="doublePosition">Position is distance from left end of beam.</param>
        /// <returns></returns>
        private static ValueResult GetShearAtPosition(double doublePosition)
        {
            bool boolIsNode = false;
            if (ResultHelper.ExactResultExists(PostProcessorScheme, doublePosition) > 0)    // Returns INode number if exact match found, otherwise returns -1.
                boolIsNode = true;
            return ShearForce.GetResultShear(boolIsNode, PostProcessorScheme, PostProcessorModel, ElementBoundaryDisplacements, doublePosition);
        }

        /// <summary>
        /// Get moment result at doublePosition. Method checks if position is node.
        /// </summary>
        /// <param name="doublePosition">Position is distance from left end of beam.</param>
        /// <returns></returns>
        private static ValueResult GetMomentAtPosition(double doublePosition)
        {
            bool boolIsNode = false;
            if (ResultHelper.ExactResultExists(PostProcessorScheme, doublePosition) > 0)    // Returns INode number if exact match found, otherwise returns -1.
                boolIsNode = true;
            return BendingMoment.GetResultMoment(boolIsNode, PostProcessorScheme, PostProcessorModel, ElementBoundaryDisplacements, doublePosition);
        }

        /// <summary>
        /// Get deflection result at doublePosition.
        /// </summary>
        /// <param name="doublePosition">Position is distance from left end of beam.</param>
        /// <returns></returns>
        private static double GetDeflectionAtPosition(double doublePosition)
        {
            if (ResultHelper.ExactResultExists(PostProcessorScheme, doublePosition) > 0)    // Returns INode number if exact match found, otherwise returns -1.
            {
                INode currentNode = PostProcessorScheme.ListINode.Where(node => node.DoubleNodePosition == doublePosition).FirstOrDefault();
                return Deflection.GetExactResult(NodalDisplacements, currentNode);
            }
            else
            {
                return Deflection.GetApproximatedResult(PostProcessorScheme, PostProcessorModel, ElementBoundaryDisplacements, doublePosition);
            }
        }

        /// <summary>
        /// Return true if node position is same as a concentrated load position, false othewise.
        /// Objective is not show simulated concentrated loads that were derived from uniform loads.
        /// </summary>
        /// <param name="iNode"></param>
        /// <returns></returns>
        private static bool IsConcentratedLoad(INode iNode)
        {
            //Debug.WriteLine($"PostProcessor.IsConcentratedLoad(): Entered method.");
            bool boolConcentratedLoad = false;
            if (!iNode.BoolNodeSupport)
            {
                //Debug.WriteLine($"PostProcessor.IsConcentratedLoad(): iNode @ {iNode.DoubleNodePosition} not support so check if concentrated load");
                if (CommonItems.listLoadConcentratedValues.Count > 0)
                {
                    double doubleNodePosition = iNode.DoubleNodePosition;
                    foreach (LoadConcentratedValues loadConcentratedValue in CommonItems.listLoadConcentratedValues)
                    {
                        //Debug.WriteLine($"PostProcessor.IsConcentratedLoad((): doubleNodePosition={doubleNodePosition}, loadConcentratedValue.DoubleLoadConcentratedPosition={loadConcentratedValue.DoubleLoadConcentratedPosition}");
                        if (doubleNodePosition == loadConcentratedValue.DoubleLoadConcentratedPosition)
                        {
                            boolConcentratedLoad = true;
                            //Debug.WriteLine($"PostProcessor.IsConcentratedLoad((): Found concentrated load since doubleNodePosition={doubleNodePosition} == {loadConcentratedValue.DoubleLoadConcentratedPosition}");
                            break;
                        }
                        else
                        {
                            //Debug.WriteLine($"PostProcessor.IsConcentratedLoad((): Checking offset distances. doubleNodePosition={doubleNodePosition}");
                            double doubleOffsetLeft = doubleNodePosition - doubleOffsetDistanceSupport;
                            double doubleOffsetRight = doubleNodePosition + doubleOffsetDistanceSupport;
                            if (doubleOffsetLeft == loadConcentratedValue.DoubleLoadConcentratedPosition || doubleOffsetRight == loadConcentratedValue.DoubleLoadConcentratedPosition)
                            {
                                boolConcentratedLoad = true;
                                //Debug.WriteLine($"PostProcessor.IsConcentratedLoad((): Found concentrated load since doubleNodePosition={doubleNodePosition} equal to doubleOffsetLeft={doubleOffsetLeft} || doubleOffsetRight={doubleOffsetRight}");
                                break;
                            }
                        }
                    }
                }
            }
            //Debug.WriteLine($"PostProcessor.IsConcentratedLoad(): Returned boolConcentratedLoad={boolConcentratedLoad} at iNode.DoubleNodePosition{iNode.DoubleNodePosition}");
            return boolConcentratedLoad;
        }

        /// <summary>
        /// Scan parameter dictionary and check if keys exist at doublePosition1, doublePosition2, doublePosition3.
        /// Remove KeyValuePair from parameter dictionary if position found.
        /// </summary>
        /// <param name="dictionary">Dictionary to scan.</param>
        /// <param name="doublePosition1">Remove KeyValuePair at this position if found.</param>
        /// <param name="doublePosition2">Remove KeyValuePair at this position if found.</param>
        /// <param name="doublePosition3">Remove KeyValuePair at this position if found.</param>
        private static void RemoveDictionaryPositions(Dictionary<double, double> dictionary, double doublePosition1, double doublePosition2, double doublePosition3)
        {
            // Copy just the keys in dictionary to a new list of keys to scan.
            List<double> dictionaryOfKeys = new List<double>(dictionary.Keys);
            foreach (double doubleKey in dictionaryOfKeys)
            {
                if (LibNum.EqualByRounding(doubleKey, doublePosition1, CommonItems.intRoundDigits))
                {
                    dictionary.Remove(doubleKey);
                    //Debug.WriteLine($"PostProcessor.RemoveDictionaryPositions(): Removed KeyValuePair @ key doublePosition1={doubleKey}");
                }
                else if (LibNum.EqualByRounding(doubleKey, doublePosition2, CommonItems.intRoundDigits))
                {
                    dictionary.Remove(doubleKey);
                    //Debug.WriteLine($"PostProcessor.RemoveDictionaryPositions(): Removed KeyValuePair @ key doublePosition2={doubleKey}");
                }
                else if (LibNum.EqualByRounding(doubleKey, doublePosition3, CommonItems.intRoundDigits))
                {
                    dictionary.Remove(doubleKey);
                    //Debug.WriteLine($"PostProcessor.RemoveDictionaryPositions(): Removed KeyValuePair @ key doublePosition3={doubleKey}");
                }
            }
        }

        /// <summary>
        /// Calculate nodal forces matrix for each element.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, double[,]> CalculateElementBoundaryForces()
        {
            Dictionary<int, double[,]> elementBoundaryForces = new Dictionary<int, double[,]>();
            double[,] nodalForces;
            for (int i = 0; i < PostProcessorModel.IntModelElements; i++)
            {
                nodalForces = MatrixTransformation.Multiply(PostProcessorModel.Beam.Matrix[i], PostProcessorModel.Boolean.Matrix[i]);
                nodalForces = MatrixTransformation.Multiply(nodalForces, PostProcessorSolver.DoubleMatrixDisplacements);
                elementBoundaryForces.Add(i + 1, nodalForces);
            }
            return elementBoundaryForces;
        }

        /// <summary>
        /// Calculate nodal displacement matrix for each element.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, double[,]> CalculateElementBoundaryDisplacements()
        {
            Dictionary<int, double[,]> elementBoundaryDisplacements = new Dictionary<int, double[,]>();
            double[,] currentElementDisplacements;
            for (int i = 0; i < PostProcessorModel.IntModelElements; i++)
            {
                currentElementDisplacements = MatrixTransformation.Multiply(PostProcessorModel.Boolean.Matrix[i], PostProcessorSolver.DoubleMatrixDisplacements);
                elementBoundaryDisplacements.Add(i + 1, currentElementDisplacements);
            }
            return elementBoundaryDisplacements;
        }

        /// <summary>
        /// Calculate nodal displacement matrix for each node.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<int, double[,]> CalculateNodalDisplacements()
        {
            Dictionary<int, double[,]> nodalDisplacements = new Dictionary<int, double[,]>();
            for (int i = 0; i < PostProcessorScheme.ListINode.Count; i++)
            {
                double[,] NodeDisplacements = new double[2, 1];
                NodeDisplacements[0, 0] = PostProcessorSolver.DoubleMatrixDisplacements[2 * i, 0];
                NodeDisplacements[1, 0] = PostProcessorSolver.DoubleMatrixDisplacements[2 * i + 1, 0];
                nodalDisplacements.Add(i + 1, NodeDisplacements);
            }
            return nodalDisplacements;
        }

    }
}
