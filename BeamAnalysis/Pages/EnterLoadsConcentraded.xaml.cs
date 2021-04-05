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
    public sealed partial class EnterLoadsConcentrated : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        /// <summary>
        /// Concentrated load position from left end of beam. Minimum value is 0d. Maximum value is CommonItems.doubleBeamLength.
        /// Each load can have a vertical force and/or a moment.
        /// </summary>
        private double doubleLoadConcentratedPosition;

        /// <summary>
        /// Vertical force at concentrated load position. Downward forces are negative.
        /// </summary>
        private double doubleLoadConcentratedForce;

        /// <summary>
        /// Moment at concentrated load position. Clockwise moments are negative.
        /// </summary>
        private double doubleLoadConcentratedMoment;

        /// <summary>
        /// Limit maximum force and moment values to this value. Value can be positive or negative. Current value is 100000000000.0.
        /// </summary>
        private readonly double doubleLoadConcentratedValueMax = 100000000000.0;

        /// <summary>
        /// True if User has entered valid doubleLoadConcentratedPosition value, false otherwise.
        /// </summary>
        private bool boolEnteredLoadConcentratedPosition;

        /// <summary>
        /// True if User has entered valid doubleLoadConcentratedForce value, false otherwise.
        /// </summary>
        private bool boolEnteredLoadConcentratedForce;

        /// <summary>
        /// True if User has entered valid doubleLoadConcentratedMoment value, false otherwise.
        /// </summary>
        private bool boolEnteredLoadConcentratedMoment;

        /// <summary>
        /// Page that allows User to add concentrated loads to beam.
        /// </summary>
        public EnterLoadsConcentrated()
        {
            InitializeComponent();
            // Set MainPage Button Visibility.
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, true);    // Show back button on this page since nothing done before returning to Home page.
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButSamples, false);
            // Overwrite XAML default Foreground colors.
            TblkLoadConcentratedNote.Foreground = LibMPC.colorBright;
            TblkLoadConcentratedBeamLength.Foreground = LibMPC.colorSuccess;
            TblkLoadConcentratedDisplay.Foreground = LibMPC.colorSuccess;
            TblkLoadConcentratedPosition.Foreground = LibMPC.colorNormal;
            TblkLoadConcentratedForce.Foreground = LibMPC.colorNormal;
            TblkLoadConcentratedMoment.Foreground = LibMPC.colorNormal;
            ButLoadConcentratedReturn.Foreground = LibMPC.colorSuccess;
            // Overwrite XAML default values.
            TblkLoadConcentratedNote.Text = $"Enter one or more concentrated loads using consistent USC or SI units.  Enter concentrated load position from left end of beam and enter vertical force and/or moment values at position.  {CommonItems.stringConstLoadForce}  {CommonItems.stringConstLoadMoment}";
            TblkLoadConcentratedBeamLength.Text = $"{CommonItems.stringConstBeamLength} = {CommonItems.doubleBeamLength} {CommonItems.stringConstUnitsLength}";
            TblkLoadConcentratedResult.Text = string.Empty;
            TblkLoadConcentratedPosition.Text = $"{CommonItems.stringConstPosition} {CommonItems.stringConstPositionReference} {CommonItems.stringConstUnitsLength}";
            TblkLoadConcentratedForce.Text = $"{CommonItems.stringConstForce} {CommonItems.stringConstUnitsForce} {CommonItems.stringConstLoadForce}";
            TblkLoadConcentratedMoment.Text = $"{CommonItems.stringConstMoment} {CommonItems.stringConstUnitsMoment} {CommonItems.stringConstLoadMoment}";
            ButLoadConcentratedAdd.Content = "Add above concentrated load";
            ButLoadConcentratedReturn.Content = $"{CommonItems.stringConstButtonReturn}";
            ButLoadConcentratedClear.Content = "Clear concentrated loads";
            // Set XAML PlaceholderText values.
            TboxLoadConcentratedPosition.PlaceholderText = CommonItems.stringConstPosition;
            TboxLoadConcentratedForce.PlaceholderText = CommonItems.stringConstForce;
            TboxLoadConcentratedMoment.PlaceholderText = CommonItems.stringConstMoment;
            // Setup scrolling for this page.
            LibMPC.ScrollViewerOn(mainPage.mainPageScrollViewer, horz: ScrollMode.Disabled, vert: ScrollMode.Auto, horzVis: ScrollBarVisibility.Disabled, vertVis: ScrollBarVisibility.Auto, zoom: ZoomMode.Disabled);
        }

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
                TboxLoadConcentratedPosition,
                TboxLoadConcentratedForce,
                TboxLoadConcentratedMoment
            };
            LibMPC.SizePageTextBoxes(listTextBox);
            List<Button> listButton = new List<Button>()
            {
                ButLoadConcentratedAdd,
                ButLoadConcentratedReturn,
                ButLoadConcentratedClear
            };
            LibMPC.SizePageButtons(listButton);
            // Special case for this page: Ensure MinWidth of TextBoxes >= MinWidth of Buttons.
            if (TboxLoadConcentratedMoment.MinWidth < ButLoadConcentratedClear.MinWidth)
            {
                double doubleMinWidth = ButLoadConcentratedClear.MinWidth;
                TboxLoadConcentratedPosition.MinWidth = doubleMinWidth;
                TboxLoadConcentratedForce.MinWidth = doubleMinWidth;
                TboxLoadConcentratedMoment.MinWidth = doubleMinWidth;
            }
            LoadConcentratedDefaultValues();
        }

        /// <summary>
        /// Set default values for concentrated load.
        /// </summary>
        private void LoadConcentratedDefaultValues()
        {
            // Next 3 lines initialize concentrated load position values.
            TboxLoadConcentratedPosition.Text = string.Empty;
            doubleLoadConcentratedPosition = 0d;
            boolEnteredLoadConcentratedPosition = false;
            // Next 3 lines initialize concentrated load force values.
            TboxLoadConcentratedForce.Text = string.Empty;
            doubleLoadConcentratedForce = 0d;
            boolEnteredLoadConcentratedForce = false;
            // Next 3 lines initialize concentrated load moment values.
            TboxLoadConcentratedMoment.Text = string.Empty;
            doubleLoadConcentratedMoment = 0d;
            boolEnteredLoadConcentratedMoment = false;
            TblkLoadConcentratedDisplay.Text = CommonItems.ShowEnteredSupportsAndLoads();
            TblkLoadConcentratedLostFocus.Text = string.Empty;
            CommonItems.ButtonForegroundColorSet(ButLoadConcentratedAdd, false);
            if (CommonItems.listLoadConcentratedValues.Count > 0)
                CommonItems.boolEnteredLoadConcentrated = true;
            else
                CommonItems.boolEnteredLoadConcentrated = false;
            CommonItems.ButtonForegroundColorSet(ButLoadConcentratedClear, CommonItems.boolEnteredLoadConcentrated);
        }

        /// <summary>
        /// Check if concentrated load input values are valid and toggle foreground color of ButLoadConcentratedAdd accordingly.
        /// </summary>
        /// <returns></returns>
        private void CheckLoadConcentratedInputValues()
        {
            // Need to check if all input values valid each time method is called since they can be changed independently by User any time.
            TblkLoadConcentratedResult.Text = string.Empty;
            bool boolLoadConcentratedInputValuesValid = true;

            if (TboxLoadConcentratedPosition.Text.Length == 0)
                boolLoadConcentratedInputValuesValid = false;

            if (TboxLoadConcentratedForce.Text.Length == 0)
                boolLoadConcentratedInputValuesValid = false;

            if (TboxLoadConcentratedMoment.Text.Length == 0)
                boolLoadConcentratedInputValuesValid = false;

            // Check if concentrated load position is on beam.
            if (doubleLoadConcentratedPosition < 0d || doubleLoadConcentratedPosition > CommonItems.doubleBeamLength)
                boolLoadConcentratedInputValuesValid = false;

            // Check if concentrated load force value in valid range.
            if (doubleLoadConcentratedForce < -doubleLoadConcentratedValueMax || doubleLoadConcentratedForce > doubleLoadConcentratedValueMax)
                boolLoadConcentratedInputValuesValid = false;

            // Check if concentrated load moment value in valid range.
            if (doubleLoadConcentratedMoment < -doubleLoadConcentratedValueMax || doubleLoadConcentratedMoment > doubleLoadConcentratedValueMax)
                boolLoadConcentratedInputValuesValid = false;

            CommonItems.ButtonForegroundColorSet(ButLoadConcentratedAdd, boolLoadConcentratedInputValuesValid);
        }

        /// <summary>
        /// Add concentrated load to list CommonItems.listLoadConcentratedValues.
        /// </summary>
        private void AddLoadConcentratedToList()
        {
            //Debug.WriteLine($"AddLoadConcentratedToList(): doubleLoadConcentratedPosition={CommonItems.doubleLoadConcentratedPosition}, doubleLoadConcentratedForce={CommonItems.doubleLoadConcentratedForce}, doubleLoadConcentratedMoment={CommonItems.doubleLoadConcentratedMoment}");
            LoadConcentratedValues loadConcentratedValues = new LoadConcentratedValues
            {
                DoubleLoadConcentratedPosition = doubleLoadConcentratedPosition,
                DoubleLoadConcentratedForce = doubleLoadConcentratedForce,
                DoubleLoadConcentratedMoment = doubleLoadConcentratedMoment
            };
            CommonItems.listLoadConcentratedValues.Add(loadConcentratedValues);      // Add concentrated load values to concentrated load list.
            if (CommonItems.listLoadConcentratedValues.Count > 0)
            {
                // More than one concentrated load entered so sort by DoubleLoadConcentratedPosition.
                CommonItems.listLoadConcentratedValues.Sort((x, y) => x.DoubleLoadConcentratedPosition.CompareTo(y.DoubleLoadConcentratedPosition));
            }
            CommonItems.boolEnteredLoadConcentrated = true;
        }

        /*** Concentrated load entry events follow *****************************************************************************/

        // Next 3 methods handle User entry in TboxLoadConcentratedPosition.

        /// <summary>
        /// Do following when User presses Enter key in TboxLoadConcentratedPosition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadConcentratedPosition_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                //Debug.WriteLine($"TboxLoadConcentratedPosition_KeyDown(): Event fired.");
                TboxLoadConcentratedPosition_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxLoadConcentratedPosition focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadConcentratedPosition_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TTboxLoadConcentratedPosition_LostFocus(): Event fired.");
            boolEnteredLoadConcentratedPosition = false;
            if (TboxLoadConcentratedPosition.Text.Length > 0)
            {
                doubleLoadConcentratedPosition = LibNum.TextBoxGetDouble(TboxLoadConcentratedPosition, EnumTextBoxUpdate.Yes);           // Get the input from the TextBox and convert to matching numeric.
                if (doubleLoadConcentratedPosition >= 0d && doubleLoadConcentratedPosition <= CommonItems.doubleBeamLength)    // Verify concentrated load is on beam.
                {
                    boolEnteredLoadConcentratedPosition = true;
                    LibMPC.OutputMsgSuccess(TblkLoadConcentratedLostFocus, $"{CommonItems.stringConstPosition} valid.");
                }
                else
                {
                    LibMPC.OutputMsgError(TblkLoadConcentratedLostFocus, $"{CommonItems.stringConstPosition} not valid since not on beam.");
                }
            }
            else
            {
                LibMPC.OutputMsgError(TblkLoadConcentratedLostFocus, $"{CommonItems.stringConstPosition} not valid since no value entered.");
            }
            CheckLoadConcentratedInputValues();
        }

        /// <summary>
        /// Verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadConcentratedPosition_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxLoadConcentratedPosition, EnumNumericType._double);
            //Debug.WriteLine($"TboxLoadConcentratedPosition_TextChanged(): Text changed, verified that input string is valid double");
        }

        // Next 3 methods handle User entry in TboxLoadConcentratedForce.

        /// <summary>
        /// Do following when User presses Enter key in TboxLoadConcentratedForce.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadConcentratedForce_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                //Debug.WriteLine($"TboxLoadConcentratedForce_KeyDown(): Event fired.");
                TboxLoadConcentratedForce_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxLoadConcentratedForce focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadConcentratedForce_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxLoadConcentratedForce_LostFocus(): Event fired.");
            boolEnteredLoadConcentratedForce = false;
            if (TboxLoadConcentratedForce.Text.Length > 0)
            {
                doubleLoadConcentratedForce = LibNum.TextBoxGetDouble(TboxLoadConcentratedForce, EnumTextBoxUpdate.Yes);           // Get the input from the TextBox and convert to matching numeric.
                // Limit maximum force to resonable value. Value can be positive or negative.
                if (doubleLoadConcentratedForce >= -doubleLoadConcentratedValueMax && doubleLoadConcentratedForce <= doubleLoadConcentratedValueMax)
                {
                    boolEnteredLoadConcentratedForce = true;
                    LibMPC.OutputMsgSuccess(TblkLoadConcentratedLostFocus, $"{CommonItems.stringConstForce} valid.");
                }
                else
                {
                    LibMPC.OutputMsgError(TblkLoadConcentratedLostFocus, $"{CommonItems.stringConstForce} not valid since not in range of +/- {doubleLoadConcentratedValueMax:G}.");
                }
            }
            else
            {
                LibMPC.OutputMsgError(TblkLoadConcentratedLostFocus, $"{CommonItems.stringConstForce} not valid since no value entered.");
            }
            CheckLoadConcentratedInputValues();
        }

        /// <summary>
        /// On TboxLoadConcentratedForce_TextChanged() event, verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadConcentratedForce_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxLoadConcentratedForce, EnumNumericType._double);
            //Debug.WriteLine($"TboxLoadConcentratedForce_TextChanged(): Text changed, verified that input string is valid double");
        }

        // Next 3 methods handle User entry in TboxLoadConcentratedMoment.

        /// <summary>
        /// Do following when User presses Enter key in TboxLoadConcentratedMoment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadConcentratedMoment_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                //Debug.WriteLine($"TboxLoadConcentratedMoment_KeyDown(): Event fired!");
                TboxLoadConcentratedMoment_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxLoadConcentratedMoment focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadConcentratedMoment_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxLoadConcentratedMoment_LostFocus(): Event fired!");
            boolEnteredLoadConcentratedMoment = false;
            if (TboxLoadConcentratedMoment.Text.Length > 0)
            {
                doubleLoadConcentratedMoment = LibNum.TextBoxGetDouble(TboxLoadConcentratedMoment, EnumTextBoxUpdate.Yes);   // Get the input from the TextBox and convert to matching numeric.
                if (doubleLoadConcentratedMoment >= -doubleLoadConcentratedValueMax && doubleLoadConcentratedMoment <= doubleLoadConcentratedValueMax)
                {
                    boolEnteredLoadConcentratedMoment = true;
                    LibMPC.OutputMsgSuccess(TblkLoadConcentratedLostFocus, $"{CommonItems.stringConstMoment} valid.");
                }
                else
                {
                    LibMPC.OutputMsgError(TblkLoadConcentratedLostFocus, $"{CommonItems.stringConstMoment} not valid since not in range of +/- {doubleLoadConcentratedValueMax:G}.");
                }
            }
            else
            {
                LibMPC.OutputMsgError(TblkLoadConcentratedLostFocus, $"{CommonItems.stringConstMoment} not valid since no value entered.");
            }
            CheckLoadConcentratedInputValues();
        }

        /// <summary>
        /// On TboxLoadConcentratedMoment_TextChanged() event, verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxLoadConcentratedMoment_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxLoadConcentratedMoment, EnumNumericType._double);
            //Debug.WriteLine($"TboxLoadConcentratedMoment_TextChanged(): Text changed, verified that input string is valid double");
        }

        /// <summary>
        /// Add load if User has entered position, force, and moment values for load. Otherwise show error message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLoadConcentratedAdd_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"ButLoadConcentratedAdd_Click() event fired.");
            TblkLoadConcentratedLostFocus.Text = string.Empty;
            if (boolEnteredLoadConcentratedPosition && boolEnteredLoadConcentratedForce && boolEnteredLoadConcentratedMoment)
            {
                AddLoadConcentratedToList();
                TblkLoadConcentratedDisplay.Text = CommonItems.ShowEnteredSupportsAndLoads();
                LibMPC.OutputMsgSuccess(TblkLoadConcentratedResult, $"Concentrated load added.");
                LoadConcentratedDefaultValues();
            }
            else
            {
                // Build error message to show User if position, force, or moment values not entered.
                string stringErrorMsg = "Concentrated load not added since one or more following values not entered or not valid:\n";
                bool boolErrorPosition = false;
                bool boolErrorForce = false;
                if (!boolEnteredLoadConcentratedPosition)
                {
                    boolErrorPosition = true;
                    stringErrorMsg += $"{CommonItems.stringConstPosition}";
                }
                if (!boolEnteredLoadConcentratedForce)
                {
                    boolErrorForce = true;
                    if (boolErrorPosition)
                        stringErrorMsg += $", {CommonItems.stringConstForce}";
                    else
                        stringErrorMsg += $"{CommonItems.stringConstForce}";
                }
                if (!boolEnteredLoadConcentratedMoment)
                {
                    if (boolErrorPosition || boolErrorForce)
                        stringErrorMsg += $", {CommonItems.stringConstMoment}";
                    else
                        stringErrorMsg += $"{CommonItems.stringConstMoment}";
                }
                LibMPC.OutputMsgError(TblkLoadConcentratedResult, stringErrorMsg);
            }
            CommonItems.ButtonForegroundColorSet(ButLoadConcentratedClear, CommonItems.boolEnteredLoadConcentrated);
        }

        /// <summary>
        ///  Return to Home page if concentrated load values valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLoadConcentratedReturn_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            mainPage.ShowPageHome();
        }

        /// <summary>
        /// Clear concentrated load list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLoadConcentratedClear_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButLoadConcentratedClear_Click() event fired.");
            if (CommonItems.ClearLoadsConcentrated())
            {
                TblkLoadConcentratedDisplay.Text = string.Empty;
                LibMPC.OutputMsgSuccess(TblkLoadConcentratedResult, "Concentrated loads cleared.");
            }
            else
            {
                TblkLoadConcentratedLostFocus.Text = string.Empty;
                LibMPC.OutputMsgError(TblkLoadConcentratedResult, "Concentrated loads not cleared since no loads entered.");
            }
            LoadConcentratedDefaultValues();     // Always reset default load concentrated values.
            CommonItems.ButtonForegroundColorSet(ButLoadConcentratedAdd, false);
            CommonItems.ButtonForegroundColorSet(ButLoadConcentratedClear, false);
        }

        /// <summary>
        /// Common LostFocus event for buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButLoadConcentrated_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButLoadConcentrated_LostFocus() event fired.");
            TblkLoadConcentratedLostFocus.Text = string.Empty;
        }

    }
}
