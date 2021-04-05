using BeamAnalysis.Common;
using LibraryCoder.MainPageCommon;
using LibraryCoder.Numerics;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace BeamAnalysis.Pages
{
    public sealed partial class EnterLoadsUniform : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        /// <summary>
        /// Left uniform load position from left end of beam. Minimum value is 0d. Maximum value is CommonItems.doubleBeamLength.
        /// </summary>
        private double doubleLoadUniformPositionLeft;

        /// <summary>
        /// Right uniform load position from left end of beam.  Value must be > doubleLoadUniformPositionLeft and <= CommonItems.doubleBeamLength.
        /// </summary>
        private double doubleLoadUniformPositionRight;

        /// <summary>
        /// Left uniform load force. Uniform forces are distributed via straight line from left to right. Downward forces are negative.
        /// </summary>
        private double doubleLoadUniformForceLeft;

        /// <summary>
        /// Right uniform load force. Uniform forces are distributed via straight line from left to right. Downward forces are negative.
        /// </summary>
        private double doubleLoadUniformForceRight;

        /// <summary>
        /// Limit left and right force values of uniform loads. Value can be positive or negative. Value is 100000000000.0.
        /// </summary>
        private const double doubleLoadUniformForceMaximum = 100000000000.0;

        /// <summary>
        /// True if User has entered valid doubleLoadUniformPositionLeft value, false otherwise.
        /// </summary>
        private bool boolEnteredLoadUniformPositionLeft;

        /// <summary>
        /// True if User has entered valid doubleLoadUniformPositionRight value, false otherwise.
        /// </summary>
        private bool boolEnteredLoadUniformPositionRight;

        /// <summary>
        /// True if User has entered valid doubleLoadUniformForceLeft value, false otherwise.
        /// </summary>
        private bool boolEnteredLoadUniformForceLeft;

        /// <summary>
        /// True if User has entered valid doubleLoadUniformForceRight value, false otherwise.
        /// </summary>
        private bool boolEnteredLoadUniformForceRight;

        /// <summary>
        /// Page that allows User to add uniform loads to beam.
        /// </summary>
        public EnterLoadsUniform()
        {
            InitializeComponent();
            // Set MainPage Button Visibility.
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, true);    // Show back button on this page since nothing done before returning to Home page.
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButSamples, false);
            // Overwrite XAML default Foreground colors.
            TblkLoadUniformNote.Foreground = LibMPC.colorBright;
            TblkLoadUniformBeamLength.Foreground = LibMPC.colorSuccess;
            TblkLoadUniformDisplay.Foreground = LibMPC.colorSuccess;
            TblkLoadUniformPositionLeft.Foreground = LibMPC.colorNormal;
            TblkLoadUniformForceLeft.Foreground = LibMPC.colorNormal;
            TblkLoadUniformPositionRight.Foreground = LibMPC.colorNormal;
            TblkLoadUniformForceRight.Foreground = LibMPC.colorNormal;
            ButLoadUniformReturn.Foreground = LibMPC.colorSuccess;      // Always can return to previous page.
            // Overwrite XAML default values.
            TblkLoadUniformNote.Text = $"Enter one or more uniform loads using consistent USC or SI units.  Enter left and right uniform load positions from left end of beam and enter left and right vertical force values per unit length of beam.  {CommonItems.stringConstLoadForce}  Uniform loads are simulated by distributing concentrated loads via straight line from left to right.";
            TblkLoadUniformBeamLength.Text = $"{CommonItems.stringConstBeamLength} = {CommonItems.doubleBeamLength} {CommonItems.stringConstUnitsLength}";
            TblkLoadUniformResult.Text = string.Empty;
            TblkLoadUniformPositionLeft.Text = $"{CommonItems.stringConstPositionLeft} {CommonItems.stringConstPositionReference} {CommonItems.stringConstUnitsLength}";
            TblkLoadUniformForceLeft.Text = $"{CommonItems.stringConstForceLeft} {CommonItems.stringConstUnitsUniform} {CommonItems.stringConstLoadForce}";
            TblkLoadUniformPositionRight.Text = $"{CommonItems.stringConstPositionRight} {CommonItems.stringConstPositionReference} {CommonItems.stringConstUnitsLength}";
            TblkLoadUniformForceRight.Text = $"{CommonItems.stringConstForceRight} {CommonItems.stringConstUnitsUniform} {CommonItems.stringConstLoadForce}";
            ButLoadUniformAdd.Content = "Add above uniform load";
            ButLoadUniformReturn.Content = $"{CommonItems.stringConstButtonReturn}";
            ButLoadUniformSimulatedLoads.Content = "Show simulated loads";
            ButLoadUniformClear.Content = "Clear uniform loads";
            // Set XAML PlaceholderText values.
            TboxLoadUniformPositionLeft.PlaceholderText = CommonItems.stringConstPositionLeft;
            TboxLoadUniformForceLeft.PlaceholderText = CommonItems.stringConstForceLeft;
            TboxLoadUniformPositionRight.PlaceholderText = CommonItems.stringConstPositionRight;
            TboxLoadUniformForceRight.PlaceholderText = CommonItems.stringConstForceRight;
            // Setup scrolling for this page.
            LibMPC.ScrollViewerOn(mainPage.mainPageScrollViewer, horz: ScrollMode.Disabled, vert: ScrollMode.Auto, horzVis: ScrollBarVisibility.Disabled, vertVis: ScrollBarVisibility.Auto, zoom: ZoomMode.Disabled);
        }

        /*** private methods follow ********************************************************************************************/

        /// <summary>
        /// Code that runs after page is loaded that will not execute properly until page is rendered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            List<TextBox> listTextBox = new List<TextBox>()
            {
                TboxLoadUniformPositionLeft,
                TboxLoadUniformForceLeft,
                TboxLoadUniformPositionRight,
                TboxLoadUniformForceRight
            };
            LibMPC.SizePageTextBoxes(listTextBox);
            List<Button> listButton = new List<Button>()
            {
                ButLoadUniformAdd,
                ButLoadUniformReturn,
                ButLoadUniformSimulatedLoads,
                ButLoadUniformClear
            };
            LibMPC.SizePageButtons(listButton);
            LoadUniformDefaultValues();
        }

        /// <summary>
        /// Set default values for uniform load.
        /// </summary>
        private void LoadUniformDefaultValues()
        {
            // Next 3 lines initialize left uniform load position values.
            TboxLoadUniformPositionLeft.Text = string.Empty;
            doubleLoadUniformPositionLeft = 0d;
            boolEnteredLoadUniformPositionLeft = false;
            // Next 3 lines initialize right uniform load position values.
            TboxLoadUniformPositionRight.Text = string.Empty;
            doubleLoadUniformPositionRight = 0d;
            boolEnteredLoadUniformPositionRight = false;
            // Next 3 lines initialize left uniform load force values.
            TboxLoadUniformForceLeft.Text = string.Empty;
            doubleLoadUniformForceLeft = 0d;
            boolEnteredLoadUniformForceLeft = false;
            // Next 3 lines initialize right uniform load force values.
            TboxLoadUniformForceRight.Text = string.Empty;
            doubleLoadUniformForceRight = 0d;
            boolEnteredLoadUniformForceRight = false;
            if (CommonItems.listLoadUniformValues.Count > 0)
                CommonItems.boolEnteredLoadUniform = true;
            else
                CommonItems.boolEnteredLoadUniform = false;
            TblkLoadUniformDisplay.Text = CommonItems.ShowEnteredSupportsAndLoads();
            TblkLoadUniformLostFocus.Text = string.Empty;
            CommonItems.ButtonForegroundColorSet(ButLoadUniformAdd, false);
            CommonItems.ButtonForegroundColorSet(ButLoadUniformClear, CommonItems.boolEnteredLoadUniform);
            CommonItems.ButtonForegroundColorSet(ButLoadUniformSimulatedLoads, CommonItems.boolEnteredLoadUniform);
        }

        /// <summary>
        /// Check if uniform load input values are valid and toggle foreground color of ButLoadUniformAdd accordingly.
        /// </summary>
        /// <returns></returns>
        private void CheckLoadUniformInputValues()
        {
            // Need to check if all input values valid each time method is called since they can be changed independently by User any time.
            TblkLoadUniformResult.Text = string.Empty;
            // Set boolLoadUniformValid to true as default. Value of following bools are set elsewhere so do not need to duplicate error checks here.
            bool boolLoadUniformValid = true;
            if (!boolEnteredLoadUniformPositionLeft)
                boolLoadUniformValid = false;
            if (!boolEnteredLoadUniformPositionRight)
                boolLoadUniformValid = false;
            if (!boolEnteredLoadUniformForceLeft)
                boolLoadUniformValid = false;
            if (!boolEnteredLoadUniformForceRight)
                boolLoadUniformValid = false;
            CommonItems.ButtonForegroundColorSet(ButLoadUniformAdd, boolLoadUniformValid);
        }

        /// <summary>
        /// Returns true if parameters have same sign or one or both parameters are zero. Returns false otherwise.
        /// </summary>
        /// <param name="doubleFirst">First double to compare.</param>
        /// <param name="doubleSecond">Second double to compare.</param>
        /// <returns></returns>
        private bool CheckSignSameDouble(double doubleFirst, double doubleSecond)
        {
            bool boolSameSign = false;
            if (doubleFirst == 0d || doubleSecond == 0d)
                boolSameSign = true;
            if (doubleFirst > 0d && doubleSecond > 0d)
                boolSameSign = true;
            if (doubleFirst < 0d && doubleSecond < 0d)
                boolSameSign = true;
            //Debug.WriteLine($" CheckSignSameDouble(): Returned {boolSameSign}, doubleFirst={doubleFirst}, doubleSecond={doubleSecond}");
            return boolSameSign;
        }

        /*** Uniform load events follow ****************************************************************************************/

        // Next 3 methods handle User entry in TboxLoadUniformPositionLeft.

        /// <summary>
        /// Do following when User presses Enter key in TboxLoadUniformPositionLeft.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformPositionLeft_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                //Debug.WriteLine($"TboxLoadUniformPositionLeft_KeyDown(): Event fired.");
                TboxLoadUniformPositionLeft_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxLoadUniformPositionLeft focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformPositionLeft_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxLoadUniformPositionLeft_LostFocus(): Event fired.");
            boolEnteredLoadUniformPositionLeft = false;
            if (TboxLoadUniformPositionLeft.Text.Length > 0)
            {
                doubleLoadUniformPositionLeft = LibNum.TextBoxGetDouble(TboxLoadUniformPositionLeft, EnumTextBoxUpdate.Yes);           // Get the input from the TextBox and convert to matching numeric.
                if (doubleLoadUniformPositionLeft >= 0d && doubleLoadUniformPositionLeft <= CommonItems.doubleBeamLength)    // Verify uniform load is on beam.
                {
                    //Debug.WriteLine($"TboxLoadUniformPositionLeft_LostFocus(): doubleLoadUniformPositionRight={doubleLoadUniformPositionRight}, TboxLoadUniformPositionRight.Text.Length={TboxLoadUniformPositionRight.Text.Length}");
                    // Skip error messages if right value not entered.
                    if (TboxLoadUniformPositionRight.Text.Length == 0 || doubleLoadUniformPositionLeft < doubleLoadUniformPositionRight)
                    {
                        // Require uniform load length to be at least 1 length unit.
                        if (TboxLoadUniformPositionRight.Text.Length == 0 || (doubleLoadUniformPositionRight - doubleLoadUniformPositionLeft) >= 1d)
                        {
                            boolEnteredLoadUniformPositionLeft = true;
                            LibMPC.OutputMsgSuccess(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionLeft} valid.");
                        }
                        else
                        {
                            LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionLeft} not valid since uniform load length must be greater than or equal to 1.0.  Model load as a concentrated load.");
                        }
                    }
                    else
                    {
                        LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionLeft} not valid since must be less than {CommonItems.stringConstPositionRight} value.");
                    }
                }
                else
                {
                    LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionLeft} not valid since not on beam.");
                }
            }
            else
            {
                LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionLeft} not valid since no value entered.");
            }
            CheckLoadUniformInputValues();
        }

        /// <summary>
        /// Verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformPositionLeft_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxLoadUniformPositionLeft, EnumNumericType._double);
            //Debug.WriteLine($"TboxLoadUniformPositionLeft_TextChanged(): Text changed, verified that input string is valid double");
        }

        // Next 3 methods handle User entry in TboxLoadUniformForceLeft.

        /// <summary>
        /// Do following when User presses Enter key in TboxLoadUniformForceLeft.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformForceLeft_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                //Debug.WriteLine($"TboxLoadUniformForceLeft_KeyDown(): Event fired.");
                TboxLoadUniformForceLeft_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxLoadUniformForceLeft focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformForceLeft_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxLoadUniformForceLeft_LostFocus(): Event fired.");
            boolEnteredLoadUniformForceLeft = false;
            if (TboxLoadUniformForceLeft.Text.Length > 0)
            {
                doubleLoadUniformForceLeft = LibNum.TextBoxGetDouble(TboxLoadUniformForceLeft, EnumTextBoxUpdate.Yes);           // Get the input from the TextBox and convert to matching numeric.
                // Limit maximum force to resonable value. Value can be positive or negative.
                if (doubleLoadUniformForceLeft >= -doubleLoadUniformForceMaximum && doubleLoadUniformForceLeft <= doubleLoadUniformForceMaximum)
                {
                    // Skip error messages if right value not entered.
                    if (TboxLoadUniformForceRight.Text.Length == 0 || CheckSignSameDouble(doubleLoadUniformForceLeft, doubleLoadUniformForceRight))
                    {
                        boolEnteredLoadUniformForceLeft = true;
                        LibMPC.OutputMsgSuccess(TblkLoadUniformLostFocus, $"{CommonItems.stringConstForceLeft} valid");
                    }
                    else
                    {
                        LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstForceLeft} not valid since sign not same as {CommonItems.stringConstForceRight}.");
                    }
                }
                else
                {
                    LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstForceLeft} not valid since not in range of +/- {doubleLoadUniformForceMaximum:G}.");
                }
            }
            else
            {
                LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstForceLeft} not valid since no value entered.");
            }
            CheckLoadUniformInputValues();
        }

        /// <summary>
        /// Verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformForceLeft_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxLoadUniformForceLeft, EnumNumericType._double);
            //Debug.WriteLine($"TboxLoadUniformForceLeft_TextChanged(): Text changed, verified that input string is valid double");
        }

        // Next 3 methods handle User entry in TboxLoadUniformPositionRight.

        /// <summary>
        /// Do following when User presses Enter key in TboxLoadUniformPositionRight.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformPositionRight_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                //Debug.WriteLine($"TboxLoadUniformPositionRight_KeyDown(): Event fired.");
                TboxLoadUniformPositionRight_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxLoadUniformPositionRight focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformPositionRight_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxLoadUniformPositionRight_LostFocus(): Event fired.");
            boolEnteredLoadUniformPositionRight = false;
            if (TboxLoadUniformPositionRight.Text.Length > 0)
            {
                doubleLoadUniformPositionRight = LibNum.TextBoxGetDouble(TboxLoadUniformPositionRight, EnumTextBoxUpdate.Yes);           // Get the input from the TextBox and convert to matching numeric.
                if (doubleLoadUniformPositionRight >= 0d && doubleLoadUniformPositionRight <= CommonItems.doubleBeamLength)    // Verify uniform load is on beam.
                {
                    //Debug.WriteLine($"TboxLoadUniformPositionRight_LostFocus(): doubleLoadUniformPositionLeft={doubleLoadUniformPositionLeft}, TboxLoadUniformPositionLeft.Text.Length={TboxLoadUniformPositionLeft.Text.Length}");
                    // Skip error messages if left value not entered.
                    if (TboxLoadUniformPositionLeft.Text.Length == 0 || doubleLoadUniformPositionLeft < doubleLoadUniformPositionRight)
                    {
                        // Require uniform load length to be at least 1 length unit.
                        if (TboxLoadUniformPositionLeft.Text.Length == 0 || (doubleLoadUniformPositionRight - doubleLoadUniformPositionLeft) >= 1d)
                        {
                            boolEnteredLoadUniformPositionRight = true;
                            LibMPC.OutputMsgSuccess(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionRight} valid.");
                        }
                        else
                        {
                            LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionRight} not valid since uniform load length must be greater than or equal to 1.0.  Model load as a concentrated load.");
                        }
                    }
                    else
                    {
                        LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionRight} not valid since must be greater than {CommonItems.stringConstPositionLeft} value.");
                    }
                }
                else
                {
                    LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionRight} not valid since not on beam.");
                }
            }
            else
            {
                LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstPositionRight} not valid since no value entered.");
            }
            CheckLoadUniformInputValues();
        }

        /// <summary>
        /// Verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformPositionRight_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxLoadUniformPositionRight, EnumNumericType._double);
            //Debug.WriteLine($"TboxLoadUniformPositionRight_TextChanged(): Text changed, verified that input string is valid double");
        }

        // Next 3 methods handle User entry in TboxLoadUniformForceRight.

        /// <summary>
        /// Do following when User presses Enter key in TboxLoadUniformForceRight.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformForceRight_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                //Debug.WriteLine($"TboxLoadUniformForceRight_KeyDown(): Event fired.");
                TboxLoadUniformForceRight_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxLoadUniformForceRight focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformForceRight_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxLoadUniformForceRight_LostFocus(): Event fired.");
            boolEnteredLoadUniformForceRight = false;
            if (TboxLoadUniformForceRight.Text.Length > 0)
            {
                doubleLoadUniformForceRight = LibNum.TextBoxGetDouble(TboxLoadUniformForceRight, EnumTextBoxUpdate.Yes);           // Get the input from the TextBox and convert to matching numeric.
                // Limit maximum force to resonable value. Value can be positive or negative.
                if (doubleLoadUniformForceRight >= -doubleLoadUniformForceMaximum && doubleLoadUniformForceRight <= doubleLoadUniformForceMaximum)
                {
                    // Skip error messages if right value not entered.
                    if (TboxLoadUniformForceRight.Text.Length == 0 || CheckSignSameDouble(doubleLoadUniformForceLeft, doubleLoadUniformForceRight))
                    {
                        boolEnteredLoadUniformForceRight = true;
                        LibMPC.OutputMsgSuccess(TblkLoadUniformLostFocus, $"{CommonItems.stringConstForceRight} valid.");
                    }
                    else
                    {
                        LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstForceRight} not valid since sign not same as {CommonItems.stringConstForceLeft}.");
                    }
                }
                else
                {
                    LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstForceRight} not valid since not in range of +/- {doubleLoadUniformForceMaximum:G}.");
                }
            }
            else
            {
                LibMPC.OutputMsgError(TblkLoadUniformLostFocus, $"{CommonItems.stringConstForceRight} not valid since no value entered.");
            }
            CheckLoadUniformInputValues();
        }

        /// <summary>
        /// Verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadUniformForceRight_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxLoadUniformForceRight, EnumNumericType._double);
            //Debug.WriteLine($"TboxLoadUniformForceRight_TextChanged(): Text changed, verified that input string is valid double");
        }

        /// <summary>
        /// Add uniform load if User has entered valid position and force values for load. Otherwise show error message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLoadUniformAdd_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"ButLoaduniformAdd_Click() event fired.");
            TblkLoadUniformLostFocus.Text = string.Empty;
            if (boolEnteredLoadUniformPositionLeft && boolEnteredLoadUniformPositionRight && boolEnteredLoadUniformForceLeft && boolEnteredLoadUniformForceRight)
            {
                CommonItems.AddLoadUniformToList(doubleLoadUniformPositionLeft, doubleLoadUniformPositionRight, doubleLoadUniformForceLeft, doubleLoadUniformForceRight);
                TblkLoadUniformDisplay.Text = CommonItems.ShowEnteredSupportsAndLoads();
                LibMPC.OutputMsgSuccess(TblkLoadUniformResult, $"Uniform load added.");
                LoadUniformDefaultValues();
            }
            else
            {
                //Debug.WriteLine($"ButLoaduniformAdd_Click() One or more input values not valid.");
                // Build error message to show User if position or force values not entered.
                string stringErrorMsg = "Uniform load not added since one or more following values not entered or not valid:\n";
                bool boolErrorPositionLeft = false;
                bool boolErrorForceLeft = false;
                bool boolErrorPositionRight = false;

                if (!boolEnteredLoadUniformPositionLeft)
                {
                    boolErrorPositionLeft = true;
                    stringErrorMsg += $"{CommonItems.stringConstPositionLeft}";
                }
                if (!boolEnteredLoadUniformForceLeft)
                {
                    boolErrorForceLeft = true;
                    if (boolErrorPositionLeft)
                        stringErrorMsg += $", {CommonItems.stringConstForceLeft}";
                    else
                        stringErrorMsg += $"{CommonItems.stringConstForceLeft}";
                }
                if (!boolEnteredLoadUniformPositionRight)
                {
                    boolErrorPositionRight = true;
                    if (boolErrorPositionLeft || boolErrorForceLeft)
                        stringErrorMsg += $", {CommonItems.stringConstPositionRight}";
                    else
                        stringErrorMsg += $"{CommonItems.stringConstPositionRight}";
                }
                if (!boolEnteredLoadUniformForceRight)
                {
                    if (boolErrorPositionLeft || boolErrorForceLeft || boolErrorPositionRight)
                        stringErrorMsg += $", {CommonItems.stringConstForceRight}";
                    else
                        stringErrorMsg += $"{CommonItems.stringConstForceRight}";
                }
                LibMPC.OutputMsgError(TblkLoadUniformResult, stringErrorMsg);
            }

            CommonItems.ButtonForegroundColorSet(ButLoadUniformClear, CommonItems.boolEnteredLoadUniform);
            CommonItems.ButtonForegroundColorSet(ButLoadUniformSimulatedLoads, CommonItems.boolEnteredLoadUniform);
        }

        /// <summary>
        /// Return to Home page if uniform load values valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLoadUniformReturn_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            mainPage.ShowPageHome();
        }

        /// <summary>
        /// Clear uniform load list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLoadUniformClear_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButLoadUniformClear_Click() event fired.");
            if (CommonItems.ClearLoadsUniform())
            {
                LibMPC.OutputMsgSuccess(TblkLoadUniformResult, "Uniform loads cleared.");
            }
            else
            {
                LibMPC.OutputMsgError(TblkLoadUniformResult, "Uniform loads not cleared since no loads entered.");
            }
            LoadUniformDefaultValues();     // Always reset default load uniform values.
            CommonItems.ButtonForegroundColorSet(ButLoadUniformAdd, false);
            CommonItems.ButtonForegroundColorSet(ButLoadUniformClear, false);
            CommonItems.ButtonForegroundColorSet(ButLoadUniformSimulatedLoads, false);

        }

        /// <summary>
        /// Calculate beam results if all required entry items entered and valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLoadUniformSimulatedLoads_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButLoadUniformSimulatedLoads_Click(): Event fired.");
            TblkLoadUniformLostFocus.Text = string.Empty;
            if (CommonItems.boolEnteredLoadUniform)
            {
                CommonItems.stringOutputResults = CommonItems.ShowUniformLoadsSimulated();
                mainPage.ShowPageDisplaySimulatedLoads();
            }
            else
            {
                LibMPC.OutputMsgError(TblkLoadUniformResult, "No results to display since no uniform loads entered.");
            }
        }

        /// <summary>
        /// Common LostFocus event for buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLoadUniform_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButLoadUniform_LostFocus() event fired.");
            TblkLoadUniformLostFocus.Text = string.Empty;
        }

    }
}
