using LibraryCoder.MainPageCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using static System.Math;

// CommonItems.cs: This file contains common items that are accessible and used in multiple pages.

namespace BeamAnalysis.Common
{
    /// <summary>
    /// Emum used to select type of output to display to User.
    /// </summary>
    public enum EnumOutputResultType { Approximate, Deflection, Moment, Shear };

    /// <summary>
    /// Class used to save User entered supports.
    /// </summary>
    public class SupportValues
    {
        /// <summary>
        /// Support position from left end of beam.
        /// </summary>
        public double DoubleSupportPosition { get; set; }

        /// <summary>
        /// Enable displacement at support if true, otherwise disable displacement.
        /// True corresponds to restraint value of 0. False corresponds to restraint value of 1.
        /// </summary>
        public bool BoolSupportDisplacement { get; set; }

        /// <summary>
        /// Enable rotation at support if true, otherwise disable rotation.
        /// True corresponds to restraint value of 0. False corresponds to restraint value of 1.
        /// </summary>
        public bool BoolSupportRotation { get; set; }
    }

    /// <summary>
    /// Class used to save User entered concentrated loads.
    /// </summary>
    public class LoadConcentratedValues
    {
        /// <summary>
        /// Concentratrated load position from left end of beam.
        /// </summary>
        public double DoubleLoadConcentratedPosition { get; set; }

        /// <summary>
        /// Concentrated load force. Downward forces are negative.
        /// </summary>
        public double DoubleLoadConcentratedForce { get; set; }

        /// <summary>
        /// Concentrated load moment. Clockwise moments are negative.
        /// </summary>
        public double DoubleLoadConcentratedMoment { get; set; }
    }

    /// <summary>
    /// Class used to save User entered uniform loads. App simulates uniform loads with series of concentrated loads.
    /// Uniform loads are split into segments of equal length. Then area and centroid for each segment is calculated.
    /// Then an equivalent concentrated load is applied to the centroid position of the segment.
    /// </summary>
    public class LoadUniformValues
    {
        /// <summary>
        /// Left uniform load position from left end of beam.
        /// </summary>
        public double DoublePositionLeft { get; set; }

        /// <summary>
        /// Right uniform load position from left end of beam. Value must be greater than DoublePositionLeft.
        /// </summary>
        public double DoublePositionRight { get; set; }

        /// <summary>
        /// Left uniform load force. Uniform forces are distributed by area via straight line from left to right. Downward forces are negative.
        /// </summary>
        public double DoubleForceLeft { get; set; }

        /// <summary>
        /// Right uniform load force. Uniform forces are distributed by area via straight line from left to right. Downward forces are negative.
        /// </summary>
        public double DoubleForceRight { get; set; }

        /// <summary>
        /// Length of uniform load. Value is DoublePositionRight less DoublePositionLeft.
        /// </summary>
        public double DoubleLoadLength { get; set; }

        /// <summary>
        /// Length of each segment used to simulate uniform load. This value is DoubleLoadLength divided by IntNumberOfSegments.
        /// </summary>
        public double DoubleSegmentLength { get; set; }

        /// <summary>
        /// Number of segments used to simulate uniform load. This value is derived from CommonItems.doubleLoadUniformSegmentLength.
        /// </summary>
        public int IntNumberOfSegments { get; set; }

        /// <summary>
        /// List that contains simulated concentrated loads created from uniform load. Downward forces are negative.
        /// </summary>
        public List<LoadConcentratedValues> ListSimulatedLoads { get; set; }
    }

    public static class CommonItems
    {
        // All User input data store 'ds' strings (keys) declared here. These are (key, value) pairs. Each key has a matching value.
        public static readonly string ds_StringNameMaterial = "StringNameMaterial";
        public static readonly string ds_DoubleYoungsModulus = "DoubleYoungsModulus";
        public static readonly string ds_DoublePoissonsRatio = "DoublePoissonsRatio";
        public static readonly string ds_StringNameCrossSection = "StringNameCrossSection";
        public static readonly string ds_DoubleInertia = "DoubleInertia";
        public static readonly string ds_DoubleBeamLength = "DoubleBeamLength";
        public static readonly string ds_DoubleOutputSegmentLength = "DoubleOutputSegmentLength";
        public static readonly string ds_DoubleCombineLoadDistance = "DoubleCombineLoadDistance";
        public static readonly string ds_BoolMatricesShow = "BoolMatricesShow";
        public static readonly string ds_BoolAppPurchased = "BoolAppPurchased";
        public static readonly string ds_BoolAppRated = "BoolAppRated";
        public static readonly string ds_IntAppRatedCounter = "IntAppRatedCounter";

        /// <summary>
        /// True if application has been purchased, false otherwise.
        /// </summary>
        public static bool boolAppPurchased = false;

        /// <summary>
        /// True if application has been rated, false otherwise.
        /// </summary>
        public static bool boolAppRated = false;

        /// <summary>
        /// True if application purchase check has been competed, false otherwise.
        /// </summary>
        public static bool boolPurchaseCheckCompleted = false;

        /// <summary>
        /// Save purchase check output string here for display on page Start if User comes back to page.
        /// </summary>
        public static string stringPurchaseCheckOutput;

        /// <summary>
        /// Show User ButRateApp button if this number of page loads since last reset.  Current value is 20.
        /// </summary>
        public static readonly int intShowButRateApp = 20;

        /// <summary>
        /// List that contains User entered support values. List is initialized in MainPage.Page_Loaded() event.
        /// </summary>
        public static List<SupportValues> listSupportValues;

        /// <summary>
        /// List that contains concentrated load values that User has entered. List is initialized in MainPage.Page_Loaded() event.
        /// </summary>
        public static List<LoadConcentratedValues> listLoadConcentratedValues;

        /// <summary>
        /// List that contains concentrated load values used to calculate beam results. List is constructed by method Home.CheckLoadValues().
        /// </summary>
        public static List<LoadConcentratedValues> listLoadConcentratedValuesConsolidated;

        /// <summary>
        /// List that contains User entered uniform load values and associated list of simulated concentrated loads. List is initialized in MainPage.Page_Loaded() event.
        /// </summary>
        public static List<LoadUniformValues> listLoadUniformValues;

        /// <summary>
        /// Name of beam material. Value not used in calculations.
        /// </summary>
        public static string stringNameMaterial;

        /// <summary>
        /// Name of beam cross section. Value not used in calculations.
        /// </summary>
        public static string stringNameCrossSection;

        /// <summary>
        /// Young's modulus for material, also known as Elastic Modulus, Ey.
        /// </summary>
        public static double doubleYoungsModulus;
            
        /// <summary>
        /// Value of Poisson's ratio for material.
        /// </summary>
        public static double doublePoissonsRatio;       // More at: https://en.wikipedia.org/wiki/Poisson%27s_ratio

        /// <summary>
        /// Moment of inertia for cross section, Iy.
        /// </summary>
        public static double doubleInertia;             // More at: https://en.wikipedia.org/wiki/List_of_second_moments_of_area

        /// <summary>
        /// Beam length.
        /// </summary>
        public static double doubleBeamLength;

        /// <summary>
        /// Segment length in beam length units to show an output result. Value used to adjust interval of output results.
        /// Value of 1.0 will output results at beam length unit intervals. Value will be adjusted by app to ensure all intervals
        /// have equal length.
        /// </summary>
        public static double doubleOutputSegmentLength;

        /// <summary>
        /// True if User has entered valid doubleYoungsModulus value, false otherwise.
        /// </summary>
        public static bool boolEnteredYoungsModulus = false;

        /// <summary>
        /// True if User has entered valid doublePoissonsRatio value, false otherwise.
        /// </summary>
        public static bool boolEnteredPoissonsRatio = false;

        /// <summary>
        /// True if User has entered valid doubleInertia value, false otherwise.
        /// </summary>
        public static bool boolEnteredInertia = false;

        /// <summary>
        /// True if User has entered valid doubleBeamLength value, false otherwise.
        /// </summary>
        public static bool boolEnteredBeamLength = false;

        /// <summary>
        /// True if User has entered valid output segment length value, false otherwise.
        /// </summary>
        public static bool boolEnteredOutputSegmentLength = false;

        /// <summary>
        /// True if User has entered at least one valid support, false otherwise.
        /// </summary>
        public static bool boolEnteredSupport = false;

        /// <summary>
        /// True if User has entered at least one valid concentrated load, false otherwise.
        /// </summary>
        public static bool boolEnteredLoadConcentrated = false;

        /// <summary>
        /// True if User has entered at least one valid uniform load, false otherwise.
        /// </summary>
        public static bool boolEnteredLoadUniform = false;

        /// <summary>
        /// Show various matrices in beam output if true.
        /// </summary>
        public static bool boolMatricesShow = false;

        /// <summary>
        /// True if concentrated loads and simulated loads of uniform loads consolidated, false otherwise.
        /// </summary>
        public static bool boolFoundLoadConsolidated;

        /// <summary>
        /// True if load found on support, false otherwise.
        /// </summary>
        public static bool boolFoundLoadOnSupport;

        /// <summary>
        /// True if load found at same position, false otherwise.
        /// </summary>
        public static bool boolFoundLoadSamePosition;

        /// <summary>
        /// True if load found close to another load, false otherwise.
        /// </summary>
        public static bool boolFoundLoadCloseTogether;

        /// <summary>
        /// Number of digits to round too before equality comparison. Value used as last parameter of LibNum.EqualByRounding().
        /// Value of 5 works well since output is generally limited to 4 digits.
        /// More digits displayed doesn't mean result is more correct. Current value 5.
        /// </summary>
        public const int intRoundDigits = 5;

        /// <summary>
        /// Pad output values by this amount. Value is 12.
        /// </summary>
        public const int intPadOutput = 12;

        /// <summary>
        /// Number of digits to round doubles too before equality comparison. Doubles have 15 digits of precision. Value is 12.
        /// </summary>
        public const int intDoubleEqual = 12;

        /// <summary>
        /// String of output results displayed on pages DisplayResults and DisplaySimulatedLoads.
        /// </summary>
        public static string stringOutputResults;

        // Following strings values are used in mutiple locations of application. Set here once and use everywhere to avoid mutiple edits.
        public const string stringConstApplicationNote = "Application calculates 2D beam results using User entered beam properties, supports, and loads.  Application does not convert any units.  It is responsibility of User to enter consistent USC or SI values throughout application.  Following unit combinations return consistent results for an identical beam.";
        public const string stringConstOutputSegmentLength = "Output segment length";
        public const string stringConstCombineLoadDistance = "Combine load distance";
        public const string stringConstDescriptionMaterial = "Material description";
        public const string stringConstYoungsModulus = "Young's modulus";
        public const string stringConstPoissonsRatio = "Poisson's ratio";
        public const string stringConstDescriptionCrossSection = "Cross section description";
        public const string stringConstInertia = "Cross section inertia";
        public const string stringConstBeamLength = "Beam length";
        public const string stringConstPosition = "Position";
        public const string stringConstDisplacement = "Displacement";
        public const string stringConstRotation = "Rotation";
        public const string stringConstForce = "Force";
        public const string stringConstMoment = "Moment";
        public const string stringConstPositionLeft = "Position left";
        public const string stringConstForceLeft = "Force left";
        public const string stringConstPositionRight = "Position right";
        public const string stringConstForceRight = "Force right";
        public const string stringConstPositionReference = "from left end of beam";
        public const string stringConstLoadForce = "Downward forces are negative.";
        public const string stringConstLoadMoment = "Clockwise moments are negative.";
        public const string stringConstButtonReturn = "Return to previous page";
        public const string stringConstUnitsUSCInch = "USC inch units are: lb/in², in⁴, in, lb, lb.in, lb/in.";
        public const string stringConstUnitsUSCFoot = "USC foot units are: lb/ft², ft⁴, ft, lb, lb.ft, lb/ft.";
        public const string stringConstUnitsSI = "SI meter units are: Pa, m⁴, m, N, N.m, N/m.";
        public const string stringConstUnitsLength = "(in, ft, m).";
        public const string stringConstUnitsYoungsModulus = "(lb/in², lb/ft², Pa).";
        public const string stringConstUnitsInertia = "(in⁴, ft⁴, m⁴).";
        public const string stringConstUnitsForce = "(lb, N).";
        public const string stringConstUnitsMoment = "(lb.in, lb.ft, N.m).";
        public const string stringConstUnitsUniform = "(lb/in, lb/ft, N/m).";

        /*** public static methods follow **************************************************************************************/

        /// <summary>
        /// If true then set button foreground color to success color, otherwise set button foreground color to error color.
        /// </summary>
        /// <param name="buttonToSet">Set foreground color of this button.</param>
        /// <param name="boolSuccess">If true show success foreground color in button, otherwise show error foreground color in button.</param>
        public static void ButtonForegroundColorSet(Button buttonToSet, bool boolSuccess)
        {
            if (boolSuccess)
                buttonToSet.Foreground = LibMPC.colorSuccess;
            else
                buttonToSet.Foreground = LibMPC.colorError;
            //Debug.WriteLine($"CommonItems.ButtonForegroundColorSet(): boolSuccess={boolSuccess}");
        }

        /// <summary>
        /// Return formatted string with listing of support(s).
        /// Output requires a fixed width font to align output vertically. Best choice is Consolas.
        /// Options are Consolas, Courier New, Lucida Console, Lucida Sans Typewriter.
        /// </summary>
        /// <returns></returns>
        public static string ShowEnteredSupports()
        {
            string stringEnteredSupports = string.Empty;
            if (boolEnteredSupport)
            {
                stringEnteredSupports = $"Support(s):\n {stringConstPosition,intPadOutput}      {stringConstDisplacement,intPadOutput}      {stringConstRotation,intPadOutput}";
                string stringSupportDisplacement;
                string stringSupportRotation;
                foreach (SupportValues supportValues in listSupportValues)
                {
                    if (supportValues.BoolSupportDisplacement)
                        stringSupportDisplacement = "On";
                    else
                        stringSupportDisplacement = "Off";

                    if (supportValues.BoolSupportRotation)
                        stringSupportRotation = "On";
                    else
                        stringSupportRotation = "Off";
                    stringEnteredSupports += $"\n {supportValues.DoubleSupportPosition,intPadOutput:0.0000}      {stringSupportDisplacement,intPadOutput:0.0000}      {stringSupportRotation,intPadOutput:0.0000}";
                }
            }
            return stringEnteredSupports;
        }

        /// <summary>
        /// Return formatted string with listing of concentrated load(s).
        /// Downward forces are negative and clockwise moments are negative.
        /// Output requires a fixed width font to align output vertically. Best choice is Consolas.
        /// Options are Consolas, Courier New, Lucida Console, Lucida Sans Typewriter.
        /// </summary>
        /// <returns></returns>
        public static string ShowEnteredLoadsConcentrated()
        {
            string stringEnteredLoadsConcentrated = string.Empty;
            if (boolEnteredLoadConcentrated)
            {
                stringEnteredLoadsConcentrated += $"Concentrated load(s):\n{stringConstLoadForce} {stringConstLoadMoment}\n {stringConstPosition,intPadOutput}      {stringConstForce,intPadOutput}      {stringConstMoment,intPadOutput}";
                foreach (LoadConcentratedValues loadConcentratedValues in listLoadConcentratedValues)
                {
                    stringEnteredLoadsConcentrated += $"\n {loadConcentratedValues.DoubleLoadConcentratedPosition,intPadOutput:0.0000}      {loadConcentratedValues.DoubleLoadConcentratedForce,intPadOutput:0.0000}      {loadConcentratedValues.DoubleLoadConcentratedMoment,intPadOutput:0.0000}";
                }
            }
            return stringEnteredLoadsConcentrated;
        }

        /// <summary>
        /// Return formatted string with listing of uniform load(s). Downward forces are negative.
        /// Output requires a fixed width font to align output vertically. Best choice is Consolas.
        /// Options are Consolas, Courier New, Lucida Console, Lucida Sans Typewriter.
        /// </summary>
        /// <returns></returns>
        public static string ShowEnteredLoadsUniform()
        {
            string stringEnteredLoadsUniform = string.Empty;
            if (boolEnteredLoadUniform)
            {
                stringEnteredLoadsUniform += $"Uniform load(s):\n{stringConstLoadForce}\n{stringConstPositionLeft,intPadOutput}   {stringConstForceLeft,intPadOutput} {stringConstPositionRight,intPadOutput}   {stringConstForceRight,intPadOutput}";
                foreach (LoadUniformValues loadUniformValues in listLoadUniformValues)
                {
                    stringEnteredLoadsUniform += $"\n {loadUniformValues.DoublePositionLeft,intPadOutput:0.0000}   {loadUniformValues.DoubleForceLeft,intPadOutput:0.0000}   {loadUniformValues.DoublePositionRight,intPadOutput:0.0000}   {loadUniformValues.DoubleForceRight,intPadOutput:0.0000}";
                }
            }
            return stringEnteredLoadsUniform;
        }

        /// <summary>
        /// Return formatted string with listing of concentrated load(s) simulated from a uniform load.
        /// Downward forces are negative and clockwise moments are negative.
        /// Output requires a fixed width font to align output vertically. Best choice is Consolas.
        /// Options are Consolas, Courier New, Lucida Console, Lucida Sans Typewriter.
        /// </summary>
        /// <returns></returns>
        public static string ShowUniformLoadsSimulated()
        {
            string stringUniformLoadsSimulated = string.Empty;
            if (listLoadUniformValues.Count > 0)
            {
                int intCounter = 0;
                List<LoadConcentratedValues> loadSimulatedValues;
                foreach (LoadUniformValues loadUniformValues in listLoadUniformValues)
                {
                    intCounter++;
                    stringUniformLoadsSimulated += $"\n\nUniform load #{intCounter}:\n{stringConstPositionLeft,intPadOutput}   {stringConstForceLeft,intPadOutput} {stringConstPositionRight,intPadOutput}   {stringConstForceRight,intPadOutput}";
                    stringUniformLoadsSimulated += $"\n {loadUniformValues.DoublePositionLeft,intPadOutput:0.0000}   {loadUniformValues.DoubleForceLeft,intPadOutput:0.0000}   {loadUniformValues.DoublePositionRight,intPadOutput:0.0000}   {loadUniformValues.DoubleForceRight,intPadOutput:0.0000}";
                    stringUniformLoadsSimulated += $"\nLoad length={loadUniformValues.DoubleLoadLength}, Segments={loadUniformValues.IntNumberOfSegments}, Segment Length={loadUniformValues.DoubleSegmentLength}";
                    stringUniformLoadsSimulated += $"\nConcentrated loads that simulate above uniform load:\n {stringConstPosition,intPadOutput}   {stringConstForce,intPadOutput}";
                    loadSimulatedValues = loadUniformValues.ListSimulatedLoads;
                    foreach (LoadConcentratedValues loadConcentratedValues in loadSimulatedValues)
                    {
                        stringUniformLoadsSimulated += $"\n {loadConcentratedValues.DoubleLoadConcentratedPosition,intPadOutput:0.0000}   {loadConcentratedValues.DoubleLoadConcentratedForce,intPadOutput:0.0000}";
                    }
                }
            }
            return stringUniformLoadsSimulated;
        }

        /// <summary>
        /// Return formatted string that shows User entered supports, concentrated loads, and uniform loads, if any.
        /// Output requires a fixed width font to align output vertically. Best choice is Consolas.
        /// Options are Consolas, Courier New, Lucida Console, Lucida Sans Typewriter.
        /// </summary>
        /// <returns></returns>
        public static string ShowEnteredSupportsAndLoads()
        {
            // Show loads even if supports not entered.
            string stringEnteredValues = string.Empty;
            if (boolEnteredSupport)
            {
                stringEnteredValues = ShowEnteredSupports();
            }
            if (boolEnteredLoadConcentrated)
            {
                if (boolEnteredSupport)
                    stringEnteredValues += $"\n\n{ShowEnteredLoadsConcentrated()}";
                else
                    stringEnteredValues += $"{ShowEnteredLoadsConcentrated()}";
            }
            if (boolEnteredLoadUniform)
            {
                if (boolEnteredSupport || boolEnteredLoadConcentrated)
                    stringEnteredValues += $"\n\n{ShowEnteredLoadsUniform()}";
                else
                    stringEnteredValues += $"{ShowEnteredLoadsUniform()}";
            }
            return stringEnteredValues;
        }

        /// <summary>
        /// Clear list of support values. Returns true if supports found, false otherwise.
        /// </summary>
        public static bool ClearSupports()
        {
            bool boolReturn = false;
            if (boolEnteredSupport)
            {
                listSupportValues.Clear();
                boolReturn = true;
            }
            boolEnteredSupport = false;
            return boolReturn;
        }

        /// <summary>
        /// Clear list of concentrated load values. Returns true if concentrated loads found, false otherwise.
        /// </summary>
        public static bool ClearLoadsConcentrated()
        {
            bool boolReturn = false;
            if (boolEnteredLoadConcentrated)
            {
                listLoadConcentratedValues.Clear();
                boolReturn = true;
            }
            boolEnteredLoadConcentrated = false;
            return boolReturn;
        }

        /// <summary>
        /// Clear list of uniform load values. Returns true if uniform loads found, false otherwise.
        /// </summary>
        public static bool ClearLoadsUniform()
        {
            bool boolReturn = false;
            if (boolEnteredLoadUniform)
            {
                listLoadUniformValues.Clear();
                //Debug.WriteLine("ClearLoadsUniform(): Uniform loads cleared so returned true.");
                boolReturn = true;
            }
            boolEnteredLoadUniform = false;
            return boolReturn;
        }

        /// <summary>
        /// Recalculate simulated loads using current value of doubleOutputSegmentLength.
        /// </summary>
        public static void RecalculateSimulatedLoads()
        {
            //Debug.WriteLine($"CommonItems.RecalculateSimulatedLoads(): listLoadUniformValues.Count={listLoadUniformValues.Count}, doubleOutputSegmentLength={doubleOutputSegmentLength}");
            double doubleLoadUniformPositionLeft;
            double doubleLoadUniformPositionRight;
            double doubleLoadUniformForceLeft;
            double doubleLoadUniformForceRight;
            if (listLoadUniformValues.Count > 0)
            {
                // Create new uniform load list and copy any existing uniform loads to it.
                List<LoadUniformValues> listLoadUniformValuesCopy = new List<LoadUniformValues> { };
                foreach (LoadUniformValues loadUniformValues in listLoadUniformValues)
                {
                    listLoadUniformValuesCopy.Add(loadUniformValues);
                }
                // Clear all uniform loads from existing list.
                listLoadUniformValues.Clear();
                // Rebuild uniform load list using current CommonItems values.
                foreach (LoadUniformValues loadUniformValues in listLoadUniformValuesCopy)
                {
                    doubleLoadUniformPositionLeft = loadUniformValues.DoublePositionLeft;
                    doubleLoadUniformPositionRight = loadUniformValues.DoublePositionRight;
                    doubleLoadUniformForceLeft = loadUniformValues.DoubleForceLeft;
                    doubleLoadUniformForceRight = loadUniformValues.DoubleForceRight;
                    AddLoadUniformToList(doubleLoadUniformPositionLeft, doubleLoadUniformPositionRight, doubleLoadUniformForceLeft, doubleLoadUniformForceRight);
                }
            }
        }

        /// <summary>
        /// Add uniform load to list listLoadUniformValues after generating simulated load list for uniform load.
        /// </summary>
        /// <param name="doubleLoadUniformPositionLeft">Left uniform load position from left end of beam. Minimum value is 0d. Maximum value is doubleBeamLength.</param>
        /// <param name="doubleLoadUniformPositionRight">Right uniform load position from left end of beam.  Value must be > DoublePositionLeft and <= doubleBeamLength.</param>
        /// <param name="doubleLoadUniformForceLeft">Left uniform load force. Uniform forces are distributed via straight line from left to right. Downward forces are negative.</param>
        /// <param name="doubleLoadUniformForceRight">Right uniform load force. Uniform forces are distributed via straight line from left to right. Downward forces are negative.</param>
        public static void AddLoadUniformToList(double doubleLoadUniformPositionLeft, double doubleLoadUniformPositionRight, double doubleLoadUniformForceLeft, double doubleLoadUniformForceRight)
        {
            //Debug.WriteLine($"CommonItems.AddLoadUniformToList(): Method entered.");
            LoadUniformValues loadUniformValues = new LoadUniformValues { };
            UniformLoadSimulate(ref loadUniformValues, doubleLoadUniformPositionLeft, doubleLoadUniformPositionRight, doubleLoadUniformForceLeft, doubleLoadUniformForceRight);
            listLoadUniformValues.Add(loadUniformValues);
            if (listLoadUniformValues.Count > 1)
            {
                // More than one uniform load entered so sort by DoublePositionLeft.
                listLoadUniformValues.Sort((x, y) => x.DoublePositionLeft.CompareTo(y.DoublePositionLeft));
            }
            boolEnteredLoadUniform = true;
        }

        /*** private static methods follow *************************************************************************************/

        /// <summary>
        /// Calculate list of concentrated loads that simulate a uniform load. Concentrated loads are derived from segments of uniform load.
        /// The area of each segment is calculated and an equivalent concentrated load is placed at centroid of each segment.
        /// Parameters used to keep method independent from equivalent CommonItems values.
        /// </summary>
        /// <param name="loadUniformValues">Class used to save User entered uniform loads.</param>
        /// <param name="doubleLoadUniformPositionLeft">Left uniform load position from left end of beam. Minimum value is 0d. Maximum value is doubleBeamLength.</param>
        /// <param name="doubleLoadUniformPositionRight">Right uniform load position from left end of beam.  Value must be > DoublePositionLeft and <= doubleBeamLength.</param>
        /// <param name="doubleLoadUniformForceLeft">Left uniform load force. Uniform forces are distributed via straight line from left to right. Downward forces are negative.</param>
        /// <param name="doubleLoadUniformForceRight">Right uniform load force. Uniform forces are distributed via straight line from left to right. Downward forces are negative.</param>
        private static void UniformLoadSimulate(ref LoadUniformValues loadUniformValues, double doubleLoadUniformPositionLeft, double doubleLoadUniformPositionRight, double doubleLoadUniformForceLeft, double doubleLoadUniformForceRight)
        {
            loadUniformValues.DoublePositionLeft = doubleLoadUniformPositionLeft;
            loadUniformValues.DoublePositionRight = doubleLoadUniformPositionRight;
            loadUniformValues.DoubleForceLeft = doubleLoadUniformForceLeft;
            loadUniformValues.DoubleForceRight = doubleLoadUniformForceRight;
            loadUniformValues.DoubleLoadLength = doubleLoadUniformPositionRight - doubleLoadUniformPositionLeft;
            if (doubleOutputSegmentLength == 0d)
            {
                // Case if doubleOutputSegmentLength was not entered or set to 0.
                loadUniformValues.IntNumberOfSegments = 1;
                loadUniformValues.DoubleSegmentLength = loadUniformValues.DoubleLoadLength;
            }
            else
            {
                // Calculate number of beam segments to spread uniform load across beam length.
                int intBeamSegments = (int)Round(doubleBeamLength / doubleOutputSegmentLength, MidpointRounding.AwayFromZero);
                double doubleBeamSegmentLength = doubleBeamLength / intBeamSegments;
                //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): doubleBeamLength={doubleBeamLength}, intBeamSegments={intBeamSegments}, doubleBeamSegmentLength={doubleBeamSegmentLength}");
                // Use beam segment length as starting value to calculate load segment length.
                if (doubleBeamSegmentLength >= loadUniformValues.DoubleLoadLength)
                {
                    loadUniformValues.IntNumberOfSegments = 1;
                    loadUniformValues.DoubleSegmentLength = loadUniformValues.DoubleLoadLength;
                }
                else
                {
                    loadUniformValues.IntNumberOfSegments = (int)Round(loadUniformValues.DoubleLoadLength / doubleBeamSegmentLength, MidpointRounding.AwayFromZero);
                    loadUniformValues.DoubleSegmentLength = loadUniformValues.DoubleLoadLength / loadUniformValues.IntNumberOfSegments;
                }
            }
            //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): loadUniformValues.IntNumberOfSegments={loadUniformValues.IntNumberOfSegments}, loadUniformValues.DoubleSegmentLength={loadUniformValues.DoubleSegmentLength}");
            // Total uniform load force is area of uniform load. Absolute values need to be considered since forces can be positive or negative.
            // Next two variables are used as 'ref' variables if uniform load is triangle or trapazoid. Therefore they need to be initialized to 0d.
            double doubleArea = 0d;         // Calculated segment area (force) of rectangle, triangle, or trapazoid.
            double doubleCentroid = 0d;     // Calculated segment centroid of rectangle, triangle, or trapazoid.
            loadUniformValues.ListSimulatedLoads = new List<LoadConcentratedValues> { };
            // Case #1: Uniform load is a rectangle.
            bool boolLoadIsRectangle = false;
            if (loadUniformValues.DoubleForceLeft == loadUniformValues.DoubleForceRight)
            {
                //Debug.WriteLine($"EnterLoadsUniform.UniformLoadSimulate(): Uniform load is a rectangle.");
                boolLoadIsRectangle = true;
                doubleCentroid = loadUniformValues.DoublePositionLeft + loadUniformValues.DoubleSegmentLength / 2d;
                doubleArea = loadUniformValues.DoubleForceLeft * loadUniformValues.DoubleLoadLength / loadUniformValues.IntNumberOfSegments;
                if (loadUniformValues.IntNumberOfSegments == 1)   // Applied single rectangle load.
                {
                    //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): Applied single rectangle load, doubleCentroid={doubleCentroid}, doubleArea={doubleArea}");
                    LoadConcentratedValues loadConcentratedValues = new LoadConcentratedValues
                    {
                        DoubleLoadConcentratedPosition = doubleCentroid,
                        DoubleLoadConcentratedForce = doubleArea,
                        DoubleLoadConcentratedMoment = 0d       // Moment always zero if uniform load.
                    };
                    loadUniformValues.ListSimulatedLoads.Add(loadConcentratedValues);
                }
                else
                {
                    doubleCentroid -= loadUniformValues.DoubleSegmentLength;      // Initialize doubleCentroid value for first pass in next loop.
                    for (int i = 0; i < loadUniformValues.IntNumberOfSegments; i++)
                    {
                        doubleCentroid += loadUniformValues.DoubleSegmentLength;
                        //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): Applied multiple rectangle loads, doubleCentroid={doubleCentroid}, doubleArea={doubleArea}");
                        LoadConcentratedValues loadConcentratedValues = new LoadConcentratedValues
                        {
                            DoubleLoadConcentratedPosition = doubleCentroid,
                            DoubleLoadConcentratedForce = doubleArea,
                            DoubleLoadConcentratedMoment = 0d       // Moment always zero if uniform load.
                        };
                        loadUniformValues.ListSimulatedLoads.Add(loadConcentratedValues);
                    }
                }
            }
            // Case #2: Uniform load is a triangle or trapazoid.
            double doubleTriangleHeight;    // Difference of ForceLeft and ForceRight is triangle height.
            if (!boolLoadIsRectangle)
            {
                //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): Uniform load is a triangle or trapazoid.");
                if (loadUniformValues.IntNumberOfSegments == 1)   // Applied single triangle or trapazoid uniform load.
                {
                    doubleTriangleHeight = CalcTriangleHeight(loadUniformValues.DoubleForceLeft, loadUniformValues.DoubleForceRight);
                    if (Abs(loadUniformValues.DoubleForceLeft) > Abs(loadUniformValues.DoubleForceRight))
                    {
                        CalcCentroidValues(ref doubleArea, ref doubleCentroid, loadUniformValues.DoubleLoadLength, loadUniformValues.DoubleForceRight, doubleTriangleHeight, true);
                        //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): Case: Abs(DoubleForceLeft) > Abs(DoubleForceRight), doubleArea={doubleArea}, doubleCentroid={doubleCentroid}");
                    }
                    else
                    {
                        CalcCentroidValues(ref doubleArea, ref doubleCentroid, loadUniformValues.DoubleLoadLength, loadUniformValues.DoubleForceLeft, doubleTriangleHeight, false);
                        //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): Case: Abs(DoubleForceLeft) < Abs(DoubleForceRight), doubleArea={doubleArea}, doubleCentroid={doubleCentroid}");
                    }
                    doubleCentroid = loadUniformValues.DoublePositionLeft + doubleCentroid;
                    //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): Case: Applied single triangle or trapazoid uniform load, doubleCentroid={doubleCentroid}, doubleArea={doubleArea}");
                    LoadConcentratedValues loadConcentratedValues = new LoadConcentratedValues
                    {
                        DoubleLoadConcentratedPosition = doubleCentroid,
                        DoubleLoadConcentratedForce = doubleArea,
                        DoubleLoadConcentratedMoment = 0d       // Moment always zero if uniform load.
                    };
                    loadUniformValues.ListSimulatedLoads.Add(loadConcentratedValues);
                }
                else
                {
                    // Equation of slope is m=(y2-y1)/(x2-x1) where m is the slope. More at: https://cls.syr.edu/mathtuneup/grapha/Unit4/Unit4a.html
                    double doubleSlope = (loadUniformValues.DoubleForceRight - loadUniformValues.DoubleForceLeft) / loadUniformValues.DoubleLoadLength;
                    // Equation of a line is y=mx+b where m is the slope and b is the y-intercept.
                    double doubleInterceptForce = loadUniformValues.DoubleForceLeft;            // Save value for use below.
                    double doubleInterceptPosition = loadUniformValues.DoublePositionLeft;      // Save value for use below.
                    double doubleLoadSegmentPositionLeft = loadUniformValues.DoublePositionLeft - loadUniformValues.DoubleSegmentLength;    // Initialize value for first pass in next loop.
                    double doubleSegmentPositionLeft;      // Initialize value for first pass in next loop.
                    double doubleSegmentForceLeft;         // Initialize value for first pass in next loop.
                    double doubleSegmentPositionRight = loadUniformValues.DoublePositionLeft;  // Initialize value for first pass in next loop.
                    double doubleSegmentForceRight = loadUniformValues.DoubleForceLeft;        // Initialize value for first pass in next loop.
                    //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): doubleSlope={doubleSlope}, doubleInterceptPosition={doubleInterceptPosition}, doubleInterceptForce={doubleInterceptForce}, loadUniformValues.DoubleSegmentLength={loadUniformValues.DoubleSegmentLength}");
                    for (int i = 0; i < loadUniformValues.IntNumberOfSegments; i++)
                    {
                        doubleSegmentPositionLeft = doubleSegmentPositionRight;
                        doubleSegmentPositionRight = doubleSegmentPositionLeft + loadUniformValues.DoubleSegmentLength;
                        doubleSegmentForceLeft = doubleSegmentForceRight;
                        doubleSegmentForceRight = doubleSlope * (doubleSegmentPositionRight - doubleInterceptPosition) + doubleInterceptForce;
                        //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): Segment {i + 1} of {loadUniformValues.IntNumberOfSegments}:  Line points are (doubleSegmentPositionLeft={doubleSegmentPositionLeft}, doubleSegmentForceLeft={doubleSegmentForceLeft}), (doubleSegmentPositionRight={doubleSegmentPositionRight}, doubleSegmentForceRight={doubleSegmentForceRight})");
                        doubleTriangleHeight = CalcTriangleHeight(doubleSegmentForceLeft, doubleSegmentForceRight);
                        if (Abs(doubleSegmentForceLeft) > Abs(doubleSegmentForceRight))
                        {
                            CalcCentroidValues(ref doubleArea, ref doubleCentroid, loadUniformValues.DoubleSegmentLength, doubleSegmentForceRight, doubleTriangleHeight, true);
                            //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): Abs(doubleSegmentForceLeft) > Abs(doubleSegmentForceRight), doubleArea={doubleArea}, doubleCentroid={doubleCentroid}");
                        }
                        else
                        {
                            CalcCentroidValues(ref doubleArea, ref doubleCentroid, loadUniformValues.DoubleSegmentLength, doubleSegmentForceLeft, doubleTriangleHeight, false);
                            //Debug.WriteLine($"CommonItems.UniformLoadSimulate(): Abs(doubleSegmentForceLeft) < Abs(doubleSegmentForceRight), doubleArea={doubleArea}, doubleCentroid={doubleCentroid}");
                        }
                        doubleLoadSegmentPositionLeft += loadUniformValues.DoubleSegmentLength;
                        doubleCentroid = doubleLoadSegmentPositionLeft + doubleCentroid;
                        //Debug.WriteLine($"CommonItems.UniformLoadSimulate():  Applied sloping uniform loads {i + 1} of {loadUniformValues.IntNumberOfSegments}, doubleCentroid={doubleCentroid}, doubleArea={doubleArea}");
                        LoadConcentratedValues loadConcentratedValues = new LoadConcentratedValues
                        {
                            DoubleLoadConcentratedPosition = doubleCentroid,
                            DoubleLoadConcentratedForce = doubleArea,
                            DoubleLoadConcentratedMoment = 0d       // Moment always zero if uniform load.
                        };
                        loadUniformValues.ListSimulatedLoads.Add(loadConcentratedValues);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate area and horizontal centroid position of a rectangle that has a right triangle above or below it.
        /// Triangle and rectangle have same width. Rectangle height may be zero, if so, calulate results for a triangle.
        /// </summary>
        /// <param name="doubleArea">Ref variable that returns calculated area of combined rectangle and right triangle.</param>
        /// <param name="doubleCentroid">Ref variable that returns calculated centroid of combined rectangle and right triangle from left side.</param>
        /// <param name="doubleRectangleLength">Length of rectangle and right triangle.</param>
        /// <param name="doubleRectangleHeight">Height of rectangle.</param>
        /// <param name="doubleTriangleHeight">Height of right triangle.</param>
        /// <param name="boolTriangleLeft">True if vertical leg of right triangle is on left side, false otherwise.</param>
        private static void CalcCentroidValues(ref double doubleArea, ref double doubleCentroid, double doubleRectangleLength, double doubleRectangleHeight, double doubleTriangleHeight, bool boolTriangleLeft)
        {
            //Debug.WriteLine($"CommonItems.CalcCentroidValues(): Parameters are doubleRectangleLength={doubleRectangleLength}, doubleRectangleHeight={doubleRectangleHeight}, doubleTriangleHeight={doubleTriangleHeight}, boolTriangleLeft={boolTriangleLeft}");
            if (doubleRectangleHeight == 0d)
            {
                // Only calculate triangle values (considerably quicker).
                doubleArea = doubleRectangleLength * doubleTriangleHeight / 2d;
                if (boolTriangleLeft)
                    doubleCentroid = doubleRectangleLength / 3.0;
                else
                    doubleCentroid = doubleRectangleLength * 2.0 / 3.0;
            }
            else
            {
                // Calculate combined rectangle and triangle values.
                double doubleAreaRect = doubleRectangleLength * doubleRectangleHeight;
                double doubleAreaTriangle = doubleRectangleLength * doubleTriangleHeight / 2d;
                doubleArea = doubleAreaRect + doubleAreaTriangle;
                if (boolTriangleLeft)
                    doubleCentroid = (doubleAreaRect * doubleRectangleLength / 2.0 + doubleAreaTriangle * doubleRectangleLength / 3.0) / doubleArea;
                else
                    doubleCentroid = (doubleAreaRect * doubleRectangleLength / 2.0 + doubleAreaTriangle * doubleRectangleLength * 2.0 / 3.0) / doubleArea;
            }
            //Debug.WriteLine($"CommonItems.CalcCentroidValues(): Calculated Ref variables are doubleArea={doubleArea}, doubleCentroid={doubleCentroid}");
        }

        /// <summary>
        /// Calculate triangle height and set proper sign (+/-) for it through all triangle or trapazoid uniform load configurations.
        /// Method does no error checking so input methods need to assure parameters have same sign or one is zero.
        /// </summary>
        /// <param name="doubleForceLeft">Left uniform force per beam length unit.</param>
        /// <param name="doubleForceRight">Right uniform force per beam length unit.</param>
        /// <returns></returns>
        private static double CalcTriangleHeight(double doubleForceLeft, double doubleForceRight)
        {
            double doubleTriangleHeight = doubleForceLeft - doubleForceRight;
            // Change sign (+/-) of doubleTriangleHeight to match sign of doubleForceLeft if needed.
            if ((doubleForceLeft <= 0d && doubleTriangleHeight >= 0d) || (doubleForceLeft >= 0d && doubleTriangleHeight <= 0d))
            {
                doubleTriangleHeight *= -1.0;
            }
            //Debug.WriteLine($"\nCommonItems.CalcTriangleHeight(): doubleForceLeft={doubleForceLeft}, doubleForceRight={doubleForceRight}, doubleTriangleHeight={doubleTriangleHeight}\n");
            return doubleTriangleHeight;
        }

    }
}
