using BeamAnalysis.Abstract;
using BeamAnalysis.CommonFEM;
using BeamAnalysis.NumericalModel;
using BeamAnalysis.NumericalSolver;
using BeamAnalysis.StaticScheme;
using BeamAnalysis.StaticScheme.Component;
using LibraryCoder.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BeamAnalysis.Common
{
    /// <summary>
    /// Class that calculates beam results.
    /// </summary>
    class CalculateResults
    {
        /*** Public methods ****************************************************************************************************/

        /// <summary>
        /// Public method that calculates beam results using beam input values, supports, and loads, entered by User.
        /// Method saves results to public string CommonItems.stringBeamResultsthat can be retrieved elsewhere.
        /// </summary>
        public static void CalculateBeamResults()
        {
            //Debug.WriteLine("CalculateResults.CalculateBeamResults(): Method entered.");
            // Initialize output string with beam properties. Output results will be added to this string as they are calculated.
            CommonItems.stringOutputResults = CreateBeamPropertiesString();
            // Create a new instance of Scheme class with static scheme information.
            IScheme iScheme = new Scheme();
            // Create a new instance of Material class.
            IMaterial iMaterial = iScheme.IMaterialNew();
            iMaterial.IntNumber = 1;    // Potentially can use multiple materials across beam length.
            iMaterial.StringName = CommonItems.stringNameMaterial;
            iMaterial.DoubleYoungsModulus = CommonItems.doubleYoungsModulus;
            iMaterial.DoublePoissonsRatio = CommonItems.doublePoissonsRatio;
            iScheme.ListIMaterial.Add(iMaterial);   // Add an instance of Material class to the list of materials
            // Create a new instance of CrossSection class.
            ICrossSection iCrossSection = iScheme.ICrossSectionNew();
            iCrossSection.IntNumber = 1;    // Potentially can use multiple cross sections across beam length.
            iCrossSection.StringName = CommonItems.stringNameCrossSection;
            iCrossSection.DoubleAreaMomentOfInertia = CommonItems.doubleInertia;
            iScheme.ListICrossSection.Add(iCrossSection);   // Add an instance of CrossSection class to the list of cross sections
            // Create sorted list of INodes that contains all supports and all concentrated loads.
            List<INode> listINodesSorted = BuildNodeList();
            // Following is dubug code that shows listINodesSorted by position to verify if nodes sorted properly.
            //int intNodeIndex = 0;
            //foreach (INode iNodeItem in listINodesSorted)
            //{
            //    Debug.WriteLine($"iNodeItem[{intNodeIndex}]: DoubleNodePosition={iNodeItem.DoubleNodePosition}, BoolNodeSupport={iNodeItem.BoolNodeSupport}");
            //    intNodeIndex++;
            //}
            //
            // Create multiple new instances of INode from listINodesSorted.
            int intINodeCounter = 1;    // First node number alway is 1.
            INode iNode;
            foreach (INode iNodeItem in listINodesSorted)
            {
                iNode = iScheme.INodeNew();
                iNode.DoubleNodePosition = iNodeItem.DoubleNodePosition;
                iNode.IntNumber = intINodeCounter++;
                iNode.BoolNodeSupport = iNodeItem.BoolNodeSupport;
                iNode.IntNodeDofDisplacement = iNodeItem.IntNodeDofDisplacement;
                iNode.IntNodeDofRotation = iNodeItem.IntNodeDofRotation;
                iNode.DoubleNodeForce = iNodeItem.DoubleNodeForce;
                iNode.DoubleNodeMoment = iNodeItem.DoubleNodeMoment;
                iScheme.ListINode.Add(iNode);
            }
            listINodesSorted.Clear();     // Clean up some by clearing original list values since not needed after copy to iScheme.ListINode.
            // Send node output values to Debug!
            //Debug.WriteLine($"\nCalculateResults.cs.CalculateResults(): List iScheme.ListINode should be sorted by DoubleNodePosition. scheme.ListINode.Count={iScheme.ListINode.Count}");
            //foreach (INode iNodeItem in iScheme.ListINode)
            //{
            //    Debug.WriteLine($"  DoubleNodePosition={iNodeItem.DoubleNodePosition}, IntNumber={iNodeItem.IntNumber}, BoolNodeSupport={iNodeItem.BoolNodeSupport}, IntNodeDofDisplacement={iNodeItem.IntNodeDofDisplacement}, IntNodeDofRotation={iNodeItem.IntNodeDofRotation}, DoubleNodeForce={iNodeItem.DoubleNodeForce}, DoubleNodeMoment={iNodeItem.DoubleNodeMoment}");
            //}
            //Debug.WriteLine("");
            //
            // Create multiple new instances of IElement. Create a new element between each INode in iScheme.ListINode.
            for (int i = 1; i < iScheme.ListINode.Count; i++)
            {
                IElement iElement = iScheme.IElementNew();
                iElement.IntNumber = i;
                iElement.IntNodeLeft = i;
                iElement.IntNodeRight = i + 1;
                iElement.IntMaterial = 1;                   // Note: Underlying FEM methods allow use of different material per element.
                iElement.IntCrossSection = 1;               // Note: Underlying FEM methods allow use of different CrossSection per element.
                iScheme.ListIElement.Add(iElement);
            }
            //Debug.WriteLine($"\n\nCalculateResults.cs.CalculateResults(): iScheme.ListINode.Count={iScheme.ListINode.Count}, scheme.ListIElement.Count={iScheme.ListIElement.Count}");
            //foreach (IElement iElement in iScheme.ListIElement)
            //{
            //    Debug.WriteLine($"  iElement Values: IntNumber={iElement.IntNumber}, IntNodeLeft={iElement.IntNodeLeft}, IntNodeRight={iElement.IntNodeRight}");
            //}
            //
            // Create multiple new instances of ILoad.
            int intLoadCounter = 1;
            foreach (INode iNodeItem in iScheme.ListINode)
            {
                if (!iNodeItem.BoolNodeSupport)     // Skip following if not a load node.
                {
                    ILoad iLoad = iScheme.ILoadNew();
                    iLoad.IntNumber = intLoadCounter++;
                    iLoad.IntNode = iNodeItem.IntNumber;                    // Assigned load to corresponding INode.
                    iLoad.DoubleForce = iNodeItem.DoubleNodeForce;          // Downward forces are negative.
                    iLoad.DoubleMoment = iNodeItem.DoubleNodeMoment;        // Clockwise moments are negative.
                    iScheme.ListILoad.Add(iLoad);
                }
            }
            //Debug.WriteLine($"\n\nCalculateResults.cs.CalculateResults(): scheme.ListILoad.Count={iScheme.ListILoad.Count}");
            //foreach (ILoad iLoad in iScheme.ListILoad)
            //{
            //    Debug.WriteLine($"  iLoad Values: IntNumber={iLoad.IntNumber}, IntNode={iLoad.IntNode}, DoubleForce={iLoad.DoubleForce}, DoubleMoment={iLoad.DoubleMoment}");
            //    Debug.WriteLine($"  iLoad Values: IntNumber={iLoad.IntNumber}, StringName={iLoad.StringName}, IntNode={iLoad.IntNode}, DoubleForce={iLoad.DoubleForce}, DoubleMoment={iLoad.DoubleMoment}");
            //}
            //
            // Create new instance of IModel.
            IModel iModel = new Model(iScheme);
            iModel.AssemblyModel();
            if (CommonItems.boolMatricesShow)
            {
                // Display calculated matrix data for iMmodel.
                CommonItems.stringOutputResults += DisplayMatrices.DisplayResultsIModel(iModel);
            }
            // Create new instance of ISolver.
            ISolver iSolver = new Solver(iModel);
            iSolver.AssembleFiniteElementModel();
            iSolver.Run();
            if (CommonItems.boolMatricesShow)
            {
                // Display calculated matrix data for iSolver.
                CommonItems.stringOutputResults += DisplayMatrices.DisplayResultsISolver(iSolver);
            }
            // Create a new instance of PostProcessor to initialize values. Discard result since not used.
            _ = new PostProcessor(iScheme, iModel, iSolver);
            PostProcessor.CalculateNodalResults();
            if (CommonItems.boolMatricesShow)
            {
                // Display calculated data for iPostProcessor.
                CommonItems.stringOutputResults += DisplayMatrices.DisplayResultsPostProcessor();
            }
            //
            // Next 3 foreach loops output shear, moment, and deflection results.
            //
            int intIntervals = Convert.ToInt32(CommonItems.doubleBeamLength / CommonItems.doubleOutputSegmentLength);
            //Debug.WriteLine($"\nCalculateResults(): intIntervals={intIntervals}");
            // Save absolute maximum shear, moment, and deflection values found for output below. Values always have positive sign since absolute values.
            double doubleMaxValueShear = 0d;
            double doubleMaxValueMoment = 0d;
            double doubleMaxValueDeflection = 0d;
            Dictionary<double, double> dictionary;
            //
            CommonItems.stringOutputResults += "\n\n****** Shear results ******\n";
            dictionary = PostProcessor.GetDictionaryShear(intIntervals);
            // Output shear plot results and save maximum shear value found.
            foreach (KeyValuePair<double, double> keyValuePair in dictionary)
            {
                if (Math.Abs(keyValuePair.Value) > doubleMaxValueShear)
                {
                    doubleMaxValueShear = Math.Abs(keyValuePair.Value);
                }
                CommonItems.stringOutputResults += $"\n{keyValuePair.Key,CommonItems.intPadOutput:0.0000} {keyValuePair.Value,CommonItems.intPadOutput:0.0}";
            }
            //
            CommonItems.stringOutputResults += "\n\n****** Moment results ******\n";
            dictionary = PostProcessor.GetDictionaryMoment(intIntervals);
            // Output moment plot results and save maximum moment value found.
            foreach (KeyValuePair<double, double> keyValuePair in dictionary)
            {
                if (Math.Abs(keyValuePair.Value) > doubleMaxValueMoment)
                {
                    doubleMaxValueMoment = Math.Abs(keyValuePair.Value);
                }
                CommonItems.stringOutputResults += $"\n{keyValuePair.Key,CommonItems.intPadOutput:0.0000} {keyValuePair.Value,CommonItems.intPadOutput:0.0}";
            }
            //
            CommonItems.stringOutputResults += "\n\n****** Deflection results ******\n";
            dictionary = PostProcessor.GetDictionaryDeflection(intIntervals);
            // Output deflection plot results and save maximum deflection value found.
            foreach (KeyValuePair<double, double> keyValuePair in dictionary)
            {
                if (Math.Abs(keyValuePair.Value) > doubleMaxValueDeflection)
                {
                    doubleMaxValueDeflection = Math.Abs(keyValuePair.Value);
                }
                // Note: 'keyValuePair.Value,CommonItems.intPadOutput:0.00000' in next line should have same number of trailing zeros as value of PostProcessor.intRoundDigits
                CommonItems.stringOutputResults += $"\n{keyValuePair.Key,CommonItems.intPadOutput:0.0000} {keyValuePair.Value,CommonItems.intPadOutput:0.00000}";
            }
            //
            dictionary.Clear();     // Done with dictionary so clear content.
            CommonItems.stringOutputResults += $"\n\n****** Beam results: ******\n";
            CommonItems.stringOutputResults += $"\n{CommonItems.stringConstLoadForce}  {CommonItems.stringConstLoadMoment}";
            double doubleSupportPosition;
            double doubleSupportForce;
            double doubleSupportMoment;
            int intIndex = 0;
            foreach (INode iNodeScheme in iScheme.ListINode)
            {
                //Debug.WriteLine($"intIndex={intIndex}");
                if (iNodeScheme.BoolNodeSupport)
                {
                    // Node is a support so retrieve an display support position, force, and moment.
                    doubleSupportPosition = iNodeScheme.DoubleNodePosition;
                    doubleSupportForce = iSolver.DoubleMatrixReactions[intIndex, 0];
                    doubleSupportMoment = iSolver.DoubleMatrixReactions[++intIndex, 0];
                    CommonItems.stringOutputResults += $"\n\nSupport location: {doubleSupportPosition,0:0.0000} {CommonItems.stringConstUnitsLength}";
                    // Round following values before display to eliminate roundoff error that can occur at a support when value should really be zero.
                    CommonItems.stringOutputResults += $"\n  Reaction force: {Math.Round(doubleSupportForce, CommonItems.intRoundDigits, MidpointRounding.AwayFromZero).ToString(LibNum.fpNumericFormatSeparator)} {CommonItems.stringConstUnitsForce}";
                    CommonItems.stringOutputResults += $"\n Reaction moment: {Math.Round(doubleSupportMoment, CommonItems.intRoundDigits, MidpointRounding.AwayFromZero).ToString(LibNum.fpNumericFormatSeparator)} {CommonItems.stringConstUnitsMoment}";
                }
                else
                {
                    // This node is not a support so skip it.
                    intIndex++;
                }
                intIndex++;
            }
            CommonItems.stringOutputResults += $"\n\nAbsolute maximum values of shear, moment, and deflection found.";
            // Round following values before display to eliminate roundoff error.
            CommonItems.stringOutputResults += $"\n           Shear: {Math.Round(doubleMaxValueShear, CommonItems.intRoundDigits, MidpointRounding.AwayFromZero).ToString(LibNum.fpNumericFormatSeparator)} {CommonItems.stringConstUnitsForce}";
            CommonItems.stringOutputResults += $"\n          Moment: {Math.Round(doubleMaxValueMoment, CommonItems.intRoundDigits, MidpointRounding.AwayFromZero).ToString(LibNum.fpNumericFormatSeparator)} {CommonItems.stringConstUnitsMoment}";
            CommonItems.stringOutputResults += $"\n      Deflection: {Math.Round(doubleMaxValueDeflection, CommonItems.intRoundDigits, MidpointRounding.AwayFromZero).ToString(LibNum.fpNumericFormatNone)} {CommonItems.stringConstUnitsLength}";
            //
            //
            // NOTE: This is a beam analysis application versus a beam design application.
            // Following link shows many deflection limits that could be checked in a beam design application. Save link for future reference.
            // AISC Steel Construction Manual, 14th Edition, Deflection Limits: http://www.bgstructuralengineering.com/BGSCM14/BGSCM008/Deflection/BGSCM0080402.htm
            //
            // Comment out following Debug lines after confirming specific FEM case matches AISC Steel Construction formula results.
            // To test, uncomment one sample case below and then enter matching beam and load configuration. Can only test one case at a time!
            // Conclusion: Application returns results that closely match AISC Steel Construction formula results.
            //
            //if (CommonItems.listLoadConcentratedValues.Count == 1)
            //    DebugSamples.CheckResultsSampleBeam01(CommonItems.listLoadConcentratedValues[0]);
            //
            //if (CommonItems.listLoadConcentratedValues.Count == 2)
            //    DebugSamples.CheckResultsSampleBeam02(CommonItems.listLoadConcentratedValues[0], CommonItems.listLoadConcentratedValues[1]);
            //
            //if (CommonItems.listLoadConcentratedValues.Count == 1)
            //    DebugSamples.CheckResultsSampleBeam03(CommonItems.listLoadConcentratedValues[0]);
            //
            //if (CommonItems.listLoadConcentratedValues.Count == 1)
            //    DebugSamples.CheckResultsSampleBeam04(CommonItems.listLoadConcentratedValues[0]);
            //
            //if (CommonItems.listLoadUniformValues.Count == 1)
            //   DebugSamples.CheckResultsSampleBeam05(CommonItems.listLoadUniformValues[0]);
            //
            //if (CommonItems.listLoadUniformValues.Count == 1)
            //    DebugSamples.CheckResultsSampleBeam06(CommonItems.listLoadUniformValues[0]);
            //
            //if (CommonItems.listLoadUniformValues.Count == 2)
            //    DebugSamples.CheckResultsSampleBeam07(CommonItems.listLoadUniformValues[0]);
            //
            //DebugSamples.CheckResultsDoubleFormatting();
        }

        /*** Private methods ***************************************************************************************************/

        /// <summary>
        /// Create string of beam properties from User entered input values.
        /// </summary>
        /// <returns></returns>
        private static string CreateBeamPropertiesString()
        {
            string stringBeamProperties = "\n****** Beam Properties ******";
            stringBeamProperties += $"\n\n{CommonItems.stringConstDescriptionMaterial}: {CommonItems.stringNameMaterial}";
            stringBeamProperties += $"\n{CommonItems.stringConstYoungsModulus}: {CommonItems.doubleYoungsModulus.ToString(LibNum.fpNumericFormatSeparator)} {CommonItems.stringConstUnitsYoungsModulus}";
            stringBeamProperties += $"\n{CommonItems.stringConstPoissonsRatio}: {CommonItems.doublePoissonsRatio.ToString(LibNum.fpNumericFormatNone)}";
            stringBeamProperties += $"\n\n{CommonItems.stringConstDescriptionCrossSection}: {CommonItems.stringNameCrossSection}";
            stringBeamProperties += $"\n{CommonItems.stringConstInertia}: {CommonItems.doubleInertia.ToString(LibNum.fpNumericFormatNone)} {CommonItems.stringConstUnitsInertia}";
            stringBeamProperties += $"\n{CommonItems.stringConstBeamLength}: {CommonItems.doubleBeamLength.ToString(LibNum.fpNumericFormatNone)} {CommonItems.stringConstUnitsLength}";
            stringBeamProperties += $"\n\n{CommonItems.stringConstOutputSegmentLength}: {CommonItems.doubleOutputSegmentLength.ToString(LibNum.fpNumericFormatNone)}.  Value used: {IntervalLength().ToString(LibNum.fpNumericFormatNone)}";
            stringBeamProperties += $"\n\n{CommonItems.ShowEnteredSupportsAndLoads()}";
            stringBeamProperties += $"\n{ShowProcessedLoadsConcentrated()}";
            return stringBeamProperties;
        }

        /// <summary>
        /// Return formatted string with listing of concentrated load(s). Downward forces are negative and clockwise moments are negative.
        /// Requires a fixed width font to align output vertically. Best choice is Consolas. Options are Consolas, Courier New, Lucida Console, Lucida Sans Typewriter.
        /// </summary>
        /// <returns></returns>
        private static string ShowProcessedLoadsConcentrated()
        {
            //Debug.WriteLine($"CalculateResults.ShowProcessedLoadsConcentrated(): boolFoundLoadConsolidated={CommonItems.boolFoundLoadConsolidated}, boolFoundLoadOnSupport={CommonItems.boolFoundLoadOnSupport}, boolFoundLoadSamePosition={CommonItems.boolFoundLoadSamePosition}, boolFoundLoadCloseTogether={CommonItems.boolFoundLoadCloseTogether}, CommonItems.listLoadConcentratedValuesConsolidated.Count={CommonItems.listLoadConcentratedValuesConsolidated.Count}");


            string stringBeamProperties = string.Empty;
            stringBeamProperties += $"\n****** Concentrated loads used to calculate results  ******";

            if (CommonItems.boolFoundLoadConsolidated)
            {
                stringBeamProperties += $"\nConsolidated concentrated loads and simulated loads of uniform loads.";
            }
            if (CommonItems.boolFoundLoadOnSupport)
            {
                stringBeamProperties += $"\nOffseted one or more loads slightly since was on a support.";
            }
            if (CommonItems.boolFoundLoadSamePosition)
            {
                stringBeamProperties += $"\nConsolidated one or more loads since at same position.";
            }
            if (CommonItems.boolFoundLoadCloseTogether)
            {
                stringBeamProperties += $"\nConsolidated one or more loads since close together.";
            }


            // Next block shows loads if any.
            if (CommonItems.listLoadConcentratedValuesConsolidated.Count > 0)
            {
                stringBeamProperties += $"\n {CommonItems.stringConstPosition,CommonItems.intPadOutput}      {CommonItems.stringConstForce,CommonItems.intPadOutput}      {CommonItems.stringConstMoment,CommonItems.intPadOutput}";
                foreach (LoadConcentratedValues loadConcentratedValues in CommonItems.listLoadConcentratedValuesConsolidated)
                {
                    stringBeamProperties += $"\n {loadConcentratedValues.DoubleLoadConcentratedPosition,CommonItems.intPadOutput:0.0000}      {loadConcentratedValues.DoubleLoadConcentratedForce,CommonItems.intPadOutput:0.0000}      {loadConcentratedValues.DoubleLoadConcentratedMoment,CommonItems.intPadOutput:0.0000}";
                }
            }
            else
            {
                stringBeamProperties += $"\nNo loads entered or they cancelled out after load check process.";
            }

            return stringBeamProperties;
        }

        /// <summary>
        /// Calculate and return interval length. Value calculated from CommonItems.doubleBeamLength and CommonItems.doubleOutputSegmentLength.
        /// </summary>
        /// <returns></returns>
        private static double IntervalLength()
        {
            int intIntervals = Convert.ToInt32(CommonItems.doubleBeamLength / CommonItems.doubleOutputSegmentLength);
            return CommonItems.doubleBeamLength / intIntervals;
        }

        /// <summary>
        /// Build list of nodes that contains all supports and all concentrated loads.
        /// Add nodes at beam endpoints if they do not exist. Then sort node list by DoubleNodePosition.
        /// Method does not check if concentrated loads are on a support since this was done by method Home.xaml.cs.CheckLoadValues().
        /// </summary>
        /// <returns></returns>
        private static List<INode> BuildNodeList()
        {
            List<INode> listINodeSupportsAndLoads = new List<INode>();
            Node node;
            // Add a node for each support in CommonItems.listSupportValues.
            // Do not check if a concentrated load is on a support. This check was done in Home.xaml.cs.CheckLoadValues().
            foreach (SupportValues supportvalues in CommonItems.listSupportValues)
            {
                node = new Node
                {
                    BoolNodeSupport = true,     // True since a support node.
                    DoubleNodePosition = supportvalues.DoubleSupportPosition,
                    DoubleNodeForce = 0d,       // Not used since support node.
                    DoubleNodeMoment = 0d       // Not used since support node.
                };
                if (supportvalues.BoolSupportDisplacement)
                    node.IntNodeDofDisplacement = 0;
                else
                    node.IntNodeDofDisplacement = 1;
                if (supportvalues.BoolSupportRotation)
                    node.IntNodeDofRotation = 0;
                else
                    node.IntNodeDofRotation = 1;
                listINodeSupportsAndLoads.Add(node);
            }
            // Add a node for each concentrated load in CommonItems.listLoadConcentratedValuesConsolidated.
            foreach (LoadConcentratedValues loadConcentratedValues in CommonItems.listLoadConcentratedValuesConsolidated)
            {
                node = new Node
                {
                    BoolNodeSupport = false,            // False since a load node,
                    DoubleNodePosition = loadConcentratedValues.DoubleLoadConcentratedPosition,
                    DoubleNodeForce = loadConcentratedValues.DoubleLoadConcentratedForce,
                    DoubleNodeMoment = loadConcentratedValues.DoubleLoadConcentratedMoment,
                    IntNodeDofDisplacement = 0,         // Not used since load node.
                    IntNodeDofRotation = 0              // Not used since load node.
                };
                listINodeSupportsAndLoads.Add(node);
                //Debug.WriteLine($"BuildNodeList(): DoubleNodePosition={node.DoubleNodePosition}, DoubleNodeForce={node.DoubleNodeForce}, DoubleNodeMoment={node.DoubleNodeMoment}");
            }
            // Add a node at beam endpoints if no supports or loads have been placed there.
            bool boolPositionBeamStart = false;
            bool boolPositionBeamEnd = false;
            double doubleNodePosition;
            foreach (INode iNode in listINodeSupportsAndLoads)
            {
                doubleNodePosition = iNode.DoubleNodePosition;
                if (doubleNodePosition == 0d)
                {
                    boolPositionBeamStart = true;   // A node has been entered at position 0d.
                }
                if (doubleNodePosition == CommonItems.doubleBeamLength)
                {
                    boolPositionBeamEnd = true;     // A node has been entered for position CommonItems.doubleBeamLength.
                }
            }
            if (!boolPositionBeamStart)
            {
                node = new Node
                {
                    BoolNodeSupport = false,            // False since a load node,
                    DoubleNodePosition = 0d,
                    DoubleNodeForce = 0d,
                    DoubleNodeMoment = 0d,
                    IntNodeDofDisplacement = 0,         // Not used since load node.
                    IntNodeDofRotation = 0              // Not used since load node.
                };
                listINodeSupportsAndLoads.Add(node);
                //Debug.WriteLine($"BuildNodeList(): Added node at beam start, DoubleNodePosition={node.DoubleNodePosition}");
            }
            if (!boolPositionBeamEnd)
            {
                node = new Node
                {
                    BoolNodeSupport = false,            // False since a load node,
                    DoubleNodePosition = CommonItems.doubleBeamLength,
                    DoubleNodeForce = 0d,
                    DoubleNodeMoment = 0d,
                    IntNodeDofDisplacement = 0,         // Not used since load node.
                    IntNodeDofRotation = 0              // Not used since load node.
                };
                listINodeSupportsAndLoads.Add(node);
            }
            // Following is debug code.
            // Sort listINodeSupportsAndLoads to place support nodes in proper position.
            //List<INode> listINodes = listINodeSupportsAndLoads.OrderBy(x => x.DoubleNodePosition).ToList();   // Sort using LINQ method syntax.
            //foreach (INode iNode in listINodes)
            //{
            //    Debug.WriteLine($"BuildNodeList(): DoubleNodePosition={iNode.DoubleNodePosition}, BoolNodeSupport={iNode.BoolNodeSupport}, DoubleNodeForce={iNode.DoubleNodeForce}, DoubleNodeMoment={iNode.DoubleNodeMoment}");
            //}
            return listINodeSupportsAndLoads.OrderBy(x => x.DoubleNodePosition).ToList();   // Sort using LINQ method syntax.
        }

    }
}
