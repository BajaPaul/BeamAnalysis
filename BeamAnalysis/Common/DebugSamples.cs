using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Math;

// TODO: Comment out calls to following Debug methods before App publish.
// Use following Debug methods to validate that FEM results from this application are accurate.
// These method calculate results using beam formulas from AISC Manual of Steel Construction.
// These methods are quick and dirty and intended to be used with a specific FEM load to confirm results.

namespace BeamAnalysis.Common
{
    /// <summary>
    /// Page that contains various debug code samples that can be used to validate FEM results in various cases.
    /// </summary>
    class DebugSamples
    {
        /// <summary>
        /// Debug code that sends contents of generic dictionary out to Debug.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary">Dictionary to output contents.</param>
        /// <param name="stringMethodName">Method name, sample = "GetDictionaryMoment".</param>
        /// <param name="stringMessage">Message, sample = "Values before final sort".</param>
        public static void DebugShowLibraryContents<TKey, TValue>(Dictionary<TKey, TValue> dictionary, string stringMethodName, string stringMessage)
        {
            if (dictionary.Count > 0)
            {
                foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
                {
                    Debug.WriteLine($"DebugShowLibraryContents(): {stringMethodName}(): {stringMessage}: ({keyValuePair.Key}, {keyValuePair.Value})");
                }
            }
            else
            {
                Debug.WriteLine($"DebugShowLibraryContents(): {stringMethodName}(): No values to output since count was 0");
            }
        }

        /// <summary>
        /// Debug Code: Beam with simple supports at left and right ends that has single concentrated load at any point.
        /// Calculate results from User entered values and send output to Debug.
        /// </summary>
        /// <param name="loadConcentratedValues">Concentrated load values.</param>
        public static void CheckResultsSampleBeam01(LoadConcentratedValues loadConcentratedValues)
        {
            Debug.WriteLine("\nBeam with simple supports at left and right ends that has single concentrated load at any point:");
            double E = CommonItems.doubleYoungsModulus;
            double I = CommonItems.doubleInertia;
            double l = CommonItems.doubleBeamLength;
            double P = loadConcentratedValues.DoubleLoadConcentratedForce;
            double a = loadConcentratedValues.DoubleLoadConcentratedPosition;
            double b = l - a;
            Debug.WriteLine($"Inputs: E={E}, I={I}, l={l}, P={P}, a={a}");
            // Reverse signs, upward forces are positive, downward forces are negative.
            double Rleft = -(P * b / l);
            double Rright = -(P * a / l);
            double Mmax = -(P * a * b / l);                     // Mmax occurs at point of load, 'a'.
            double Da = P * a * a * b * b / (3d * E * I * l);   // Deflection at 'a'
            Debug.WriteLine($"Reaction left={Rleft}, Reaction right={Rright}");
            Debug.WriteLine($"Shear left={-Rleft}, Shear right={Rright}");
            Debug.WriteLine($"a={a}, Maximum moment is {Mmax}");
            Debug.WriteLine($"a={a}, Maximum deflection is {Da}");
            if (a > b)  // Next values calculated and shown only if a > b.
            {
                double XDmax = Sqrt(a * (a + 2d * b) / 3d);
                double Dmax = P * a * b * (a + 2d * b) * Sqrt(3d * a * (a + 2d * b)) / (27d * E * I * l);
                Debug.WriteLine($"Max deflection occurs @ XDmax={XDmax}, Deflection @ XDmax={Dmax}");
            }
        }

        /// <summary>
        /// Debug Code: Beam with simple supports at left and right ends that has two equal concentrated loads symmetrically placed.
        /// Calculate results from User entered values and send output to Debug.
        /// </summary>
        /// <param name="loadConcentratedValuesLeft">Left concentrated load values.</param>
        /// <param name="loadConcentratedValuesRight">Right concentrated load values.</param>
        public static void CheckResultsSampleBeam02(LoadConcentratedValues loadConcentratedValuesLeft, LoadConcentratedValues loadConcentratedValuesRight)
        {
            Debug.WriteLine("\nBeam with simple supports at left and right ends that has two equal concentrated loads symmetrically placed:");
            double E = CommonItems.doubleYoungsModulus;
            double I = CommonItems.doubleInertia;
            double l = CommonItems.doubleBeamLength;
            double Pl = loadConcentratedValuesLeft.DoubleLoadConcentratedForce;
            double a = loadConcentratedValuesLeft.DoubleLoadConcentratedPosition;
            double Pr = loadConcentratedValuesRight.DoubleLoadConcentratedForce;
            double b = l - loadConcentratedValuesRight.DoubleLoadConcentratedPosition;
            Debug.WriteLine($"Inputs: E={E}, I={I}, l={l}, Pl={Pl}, a={a}, Pr={Pr}, b={b}");
            if (Pl == Pr && a == b)     //Check input.
            {
                // Reverse signs, upward forces are positive, downward forces are negative.
                double Rleft = -(Pl);
                double Rright = Rleft;
                double Mmax = -(Pl * a);     // Mmax occurs between loads.
                double Dmax = Pl * a * (3d * l * l - 4d * a * a) / (24 * E * I);
                double Da = Pl * a * (3d * l * a - 3d * a * a - a * a) / (6d * E * I);
                Debug.WriteLine($"Reaction left={Rleft}, Reaction right={Rright}");
                Debug.WriteLine($"Shear left={-Rleft}, Shear right={Rright}");
                Debug.WriteLine($"Maximum moment occurs between loads is {Mmax}");
                Debug.WriteLine($"Deflection @ a={a} is {Da}");
                Debug.WriteLine($"Max deflection @ midpoint={l / 2d} is {Dmax}");
            }
            else
                Debug.WriteLine($"Skipped results since entered load values not equal or not placed symmetrically.");
        }

        /// <summary>
        /// Debug Code: Beam with simple support at left end and fixed support at right end that has single concentrated load at any point.
        /// Calculate results from User entered values and send output to Debug.
        /// </summary>
        /// <param name="loadConcentratedValues">Concentrated load values.</param>
        public static void CheckResultsSampleBeam03(LoadConcentratedValues loadConcentratedValues)
        {
            Debug.WriteLine("\nBeam with simple support at left end and fixed support at right end that has single concentrated load at any point:");
            double E = CommonItems.doubleYoungsModulus;
            double I = CommonItems.doubleInertia;
            double l = CommonItems.doubleBeamLength;
            double P = loadConcentratedValues.DoubleLoadConcentratedForce;
            double a = loadConcentratedValues.DoubleLoadConcentratedPosition;
            double b = l - a;
            Debug.WriteLine($"Inputs: E={E}, I={I}, l={l}, P={P}, a={a}");
            // Reverse signs, upward forces are positive, downward forces are negative.
            double Rleft = -(P * b * b * (a + 2d * l) / (2d * l * l * l));
            double Rright = -(P * a * (3d * l * l - a * a) / (2d * l * l * l));
            double Ma = -(Rleft * a);     // Moment at 'a'.
            double Da = (P * a * a * b * b * b) * (3d * l + a) / (12d * E * I * l * l * l);  // Deflection at 'a'
            double Ml = P * a * b * (a + l) / (2d * l * l);      // Moment at left (fixed) end of beam.

            // Not checking all available formula results since formulas are quite complex.

            Debug.WriteLine($"Reaction left={Rleft}, Reaction right={Rright}");
            Debug.WriteLine($"Shear left={-Rleft}, Shear right={Rright}");
            Debug.WriteLine($"a={a}, Ma={Ma}");
            Debug.WriteLine($"a={a}, Da={Da}");
            Debug.WriteLine($"l={l}, Ml={Ml}");
        }

        /// <summary>
        /// Debug Code: Cantilever beam with no support at left end and fixed support at right end that has single concentrated load at any point.
        /// Calculate results from User entered values and send output to Debug.
        /// </summary>
        /// <param name="loadConcentratedValues">Concentrated load values.</param>
        public static void CheckResultsSampleBeam04(LoadConcentratedValues loadConcentratedValues)
        {
            Debug.WriteLine("\nCantilever beam with no support at left end and fixed support at right end that has single concentrated load at any point:");
            double E = CommonItems.doubleYoungsModulus;
            double I = CommonItems.doubleInertia;
            double l = CommonItems.doubleBeamLength;
            double P = loadConcentratedValues.DoubleLoadConcentratedForce;
            double a = loadConcentratedValues.DoubleLoadConcentratedPosition;
            double b = l - a;
            Debug.WriteLine($"Inputs: E={E}, I={I}, l={l}, P={P}, a={a}");
            // Reverse signs, upward forces are positive, downward forces are negative.
            double Rright = -(P);
            double Ml = P * b;      // Moment at left (fixed) end of beam.
            double D0 = P * b * b * (3d * l - b) / (6d * E * I);
            double Da = P * b * b * b / (3d * E * I);

            Debug.WriteLine($"Reaction left={0d}, Reaction right={Rright}");
            Debug.WriteLine($"Shear left={0d}, Shear right={Rright}");
            Debug.WriteLine($"l={l}, Ml={Ml}");
            Debug.WriteLine($"a={a}, Ma={0d}");
            Debug.WriteLine($"a={a}, Da={Da}");
            Debug.WriteLine($"At left end of beam, Dmax={D0}");
        }

        /// <summary>
        /// Debug Code: Beam with simple supports at left and right ends that has rectangular uniform load across beam length.
        /// Calculate results from User entered values and send output to Debug.
        /// </summary>
        /// <param name="loadUniformValue">Uniform load values.</param>
        public static void CheckResultsSampleBeam05(LoadUniformValues loadUniformValue)
        {
            Debug.WriteLine("\nBeam with simple supports at left and right ends that has rectangular uniform load across beam length:");
            double E = CommonItems.doubleYoungsModulus;
            double I = CommonItems.doubleInertia;
            double l = CommonItems.doubleBeamLength;
            double wL = loadUniformValue.DoubleForceLeft;
            double wR = loadUniformValue.DoubleForceRight;
            double xL = loadUniformValue.DoublePositionLeft;
            double xR = loadUniformValue.DoublePositionRight;
            if ((wL == wR) && (xL == 0d) && (xR == l))    //Check input.
            {
                Debug.WriteLine($"Inputs: E={E}, I={I}, l={l}, w={wL}");
                // Reverse signs, upward forces are positive, downward forces are negative.
                double R = -(wL * l / 2d);
                double Mmax = -(wL * l * l / 8d);                   // Mmax occurs at midpoint of beam.
                double Dmax = (wL * l * l * l * l * 5d / (384d * E * I));  // Dmax occurs at midpoint of beam.
                Debug.WriteLine($"Reactions left and right={R}");
                Debug.WriteLine($"Shear left={-R}, Shear right={R}");
                Debug.WriteLine($"Maximum moment={Mmax} at x={l / 2d}");
                Debug.WriteLine($"Maximum deflection={Dmax} at x={l / 2d}");
            }
            else
                Debug.WriteLine($"Skipped results since entered uniform load values not equal or not across beam length.");
        }

        /// <summary>
        /// Debug Code: Beam with simple supports at left and right ends that has triangular uniform load across beam length.
        /// Load is 0 at left end and increases towards right end.
        /// Calculate results from User entered values and send output to Debug.
        /// </summary>
        /// <param name="loadUniformValue">Uniform load values.</param>
        public static void CheckResultsSampleBeam06(LoadUniformValues loadUniformValue)
        {
            Debug.WriteLine("\nBeam with simple supports at left and right ends that has triangular uniform load across beam length.  Load is 0 at left end and increases towards right end:");
            double E = CommonItems.doubleYoungsModulus;
            double I = CommonItems.doubleInertia;
            double l = CommonItems.doubleBeamLength;
            double wL = loadUniformValue.DoubleForceLeft;
            double wR = loadUniformValue.DoubleForceRight;
            double xL = loadUniformValue.DoublePositionLeft;
            double xR = loadUniformValue.DoublePositionRight;

            Debug.WriteLine($"Inputs: wL={wL}, wR={wR}, xL={xL}, xRL={xR}");

            if ((wL == 0d) && (Abs(wR) > 0d) && (xL == 0d) && (xR == l))    //Check input.
            {
                Debug.WriteLine($"Inputs: E={E}, I={I}, l={l}, wL={wL}, wR={wR}");
                // Reverse signs, upward forces are positive, downward forces are negative.
                double W = wR * l / 2d;     // Total load.
                double Rleft = -(W / 3d);
                double Rright = -(W * 2d / 3d);
                double Mmax = -(W * l * 2d / (9d * Sqrt(3d)));
                double Dmax = W * l * l * l * 0.01304 / (E * I);
                Debug.WriteLine($"Reaction left={Rleft}, reaction right={Rright}");
                Debug.WriteLine($"Shear left={-Rleft}, Shear right={Rright}");
                Debug.WriteLine($"Maximum moment={Mmax} at x={l * 0.5774}");
                Debug.WriteLine($"Maximum deflection={Dmax} at x={l * 0.5193}");
            }
            else
                Debug.WriteLine($"Skipped results since entered uniform load values not proper or not across beam length.");
        }

        /// <summary>
        /// Debug Code: Beam with simple supports at left and right ends that has triangular uniform load across beam length. Peak is at beam center.
        /// Calculate results from User entered values and send output to Debug.
        /// </summary>
        /// <param name="loadUniformValue">Uniform load values.</param>
        public static void CheckResultsSampleBeam07(LoadUniformValues loadUniformValue)
        {
            Debug.WriteLine("\nBeam with simple supports at left and right ends that has triangular uniform load across beam length. Peak is at beam center:");
            double E = CommonItems.doubleYoungsModulus;
            double I = CommonItems.doubleInertia;
            double l = CommonItems.doubleBeamLength;
            // Following input is for left trianglular load.
            double wL = loadUniformValue.DoubleForceLeft;
            double wR = loadUniformValue.DoubleForceRight;
            double xL = loadUniformValue.DoublePositionLeft;
            double xR = loadUniformValue.DoublePositionRight;

            Debug.WriteLine($"Inputs: wL={wL}, wR={wR}, xL={xL}, xR={xR}");

            double center = l / 2d;     // Beam midpoint.
            if ((wL == 0d) && (Abs(wR) > 0d) && (xL == 0d) && (xR == center))    //Check input.
            {
                Debug.WriteLine($"Inputs: E={E}, I={I}, l={l}, wL={wL}, wR={wR}, xR={xR}");
                // Reverse signs, upward forces are positive, downward forces are negative.
                double W = wR * center;     // Total load.
                double Rleft = -(W / 2d);
                double Rright = Rleft;
                double Mmax = -(W * l / 6d);
                double Dmax = W * l * l * l / (E * I * 60d);
                Debug.WriteLine($"Reaction left={Rleft}, reaction right={Rright}");
                Debug.WriteLine($"Shear left={-Rleft}, Shear right={Rright}, Shear center=0.0");
                Debug.WriteLine($"Maximum moment={Mmax} at x={center}");
                Debug.WriteLine($"Maximum deflection={Dmax} at x={center}");
            }
            else
                Debug.WriteLine($"Skipped results since entered uniform load values not proper.");
        }

        /// <summary>
        /// This method is debug code to show contents of a two dimension array/matrix.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        public static void Show2DArrayValues<T>(T[,] matrix)
        {
            int intUpperBoundI = matrix.GetUpperBound(0);
            int intUpperBoundJ = matrix.GetUpperBound(1);
            Debug.WriteLine($"Show2DArrayValues(): Index values of matrix: intUpperBoundI={intUpperBoundI}, intUpperBoundJ={intUpperBoundJ}");
            for (int i = 0; i <= intUpperBoundI; i++)
            {
                for (int j = 0; j <= intUpperBoundJ; j++)
                {
                    Debug.Write(matrix[i, j] + "\t");
                }
                Debug.WriteLine("");
            }
        }

        /// <summary>
        /// Debug sample that shows output from various double format options.
        /// </summary>
        public static void CheckResultsDoubleFormatting()
        {
            // More at: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#the-general-g-format-specifier
            // When used with a Double, "G17" format specifier ensures original Double value successfully round-trips. 
            double doublePI = Math.PI;
            double doublePI10000 = Math.PI * 10000d;     // Shift the deciaml point to see what happens.
            Debug.WriteLine($"");
            Debug.WriteLine($"{doublePI}");
            Debug.WriteLine($"{doublePI:G}");
            Debug.WriteLine($"{doublePI:G2}");
            Debug.WriteLine($"{doublePI:G4}");
            Debug.WriteLine($"{doublePI:G6}");
            Debug.WriteLine($"{doublePI:G8}");
            Debug.WriteLine($"{doublePI:G10}");
            Debug.WriteLine($"{doublePI:G12}");
            Debug.WriteLine($"{doublePI:G15}");         // G15 is default for doubles.
            Debug.WriteLine($"{doublePI:G17}\n");       // G17 will round-trip for doubles.
            Debug.WriteLine($"{doublePI10000}");
            Debug.WriteLine($"{doublePI10000:G}");
            Debug.WriteLine($"{doublePI10000:G2}");
            Debug.WriteLine($"{doublePI10000:G4}");
            Debug.WriteLine($"{doublePI10000:G6}");
            Debug.WriteLine($"{doublePI10000:G8}");
            Debug.WriteLine($"{doublePI10000:G10}");
            Debug.WriteLine($"{doublePI10000:G12}");
            Debug.WriteLine($"{doublePI10000:G15}");
            Debug.WriteLine($"{doublePI10000:G17}\n");
            // Conclusion: Use "G" for output to TextBoxes. "G17" will cause rounding of TextBox value so it will round-trip, not good!
        }

    }
}
