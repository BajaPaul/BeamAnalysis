using BeamAnalysis.Common;
using LibraryCoder.MainPageCommon;
using LibraryCoder.Numerics;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// Sample beam properties using inch units that can be used are: ASTM-A36 Steel, 29,000,000, 0.26, W10x30, 170, 120.

namespace BeamAnalysis.Pages
{
    public sealed partial class EnterBeamProperties : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        // Following website has list of materials with Young's modulus values. https://en.wikipedia.org/wiki/Young%27s_modulus

        /// <summary>
        /// Limit doubleYoungsModulus minimum value. Value is 1000000.0.
        /// </summary>
        private const double doubleYoungsModulusMinimum = 1000000.0;

        /// <summary>
        /// Limit doubleYoungsModulus maximum value. Value is 50000000000000.0.
        /// </summary>
        private const double doubleYoungsModulusMaximum = 50000000000000.0;

        /// <summary>
        /// Limit doubleInertia minimum value. Value is 0.00000001.
        /// </summary>
        private const double doubleInertiaMinimum = 0.00000001;

        /// <summary>
        /// Limit doubleInertia maximum value. Value is 100000000000.0.
        /// </summary>
        private const double doubleInertiaMaximum = 100000000000.0;

        /// <summary>
        /// Limit doubleBeamLength minimum value. Value is 0.1.
        /// </summary>
        private const double doubleBeamLengthMinimum = 0.1;

        /// <summary>
        /// Limit doubleBeamLength maximum value. Value is 100000.0.
        /// </summary>
        private const double doubleBeamLengthMaximum = 100000.0;

        /// <summary>
        /// Page that allows User to enter physical properties of beam.
        /// </summary>
        public EnterBeamProperties()
        {
            InitializeComponent();
            // Set MainPage Button Visibility.
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, true);    // Show back button on this page since nothing done before returning to Home page.
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButSamples, false);
            // Overwrite XAML TextBlock Foreground colors.
            TblkBeamPropertiesNote.Foreground = LibMPC.colorBright;
            TblkBeamPropertiesNameMaterial.Foreground = LibMPC.colorNormal;
            TblkBeamPropertiesYoungModulus.Foreground = LibMPC.colorNormal;
            TblkBeamPropertiesPoissonsRatio.Foreground = LibMPC.colorNormal;
            TblkBeamPropertiesNameCrossSection.Foreground = LibMPC.colorNormal;
            TblkBeamPropertiesInertia.Foreground = LibMPC.colorNormal;
            TblkBeamPropertiesLength.Foreground = LibMPC.colorNormal;
            // Overwrite XAML default values.
            TblkBeamPropertiesNote.Text = "Enter beam properties using consistent USC or SI units.  If beam length is changed, then any existing supports and loads will be cleared since they are dependent on beam length.";
            TblkBeamPropertiesDisplay.Text = string.Empty;
            TblkBeamPropertiesLostFocus.Text = string.Empty;
            TblkBeamPropertiesNameMaterial.Text = $"{CommonItems.stringConstDescriptionMaterial} (optional).";
            TblkBeamPropertiesYoungModulus.Text = $"{CommonItems.stringConstYoungsModulus} {CommonItems.stringConstUnitsYoungsModulus}";
            TblkBeamPropertiesPoissonsRatio.Text = $"{CommonItems.stringConstPoissonsRatio}.";
            TblkBeamPropertiesNameCrossSection.Text = $"{CommonItems.stringConstDescriptionCrossSection} (optional).";
            TblkBeamPropertiesInertia.Text = $"{CommonItems.stringConstInertia} {CommonItems.stringConstUnitsInertia}";
            TblkBeamPropertiesLength.Text = $"{CommonItems.stringConstBeamLength} {CommonItems.stringConstUnitsLength}";
            ButBeamPropertiesReturn.Content = CommonItems.stringConstButtonReturn;
            ButBeamPropertiesReturn.Foreground = LibMPC.colorSuccess;
            ButBeamPropertiesClear.Content = "Clear beam properties";
            // Set XAML PlaceholderText values.
            TboxBeamPropertiesNameMaterial.PlaceholderText = CommonItems.stringConstDescriptionMaterial;
            TboxBeamPropertiesYoungsModulus.PlaceholderText = CommonItems.stringConstYoungsModulus;
            TboxBeamPropertiesPoissonsRatio.PlaceholderText = CommonItems.stringConstPoissonsRatio;
            TboxBeamPropertiesNameCrossSection.PlaceholderText = CommonItems.stringConstDescriptionCrossSection;
            TboxBeamPropertiesInertia.PlaceholderText = CommonItems.stringConstInertia;
            TboxBeamPropertiesLength.PlaceholderText = CommonItems.stringConstBeamLength;
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
                TboxBeamPropertiesNameMaterial,
                TboxBeamPropertiesYoungsModulus,
                TboxBeamPropertiesPoissonsRatio,
                TboxBeamPropertiesNameCrossSection,
                TboxBeamPropertiesInertia,
                TboxBeamPropertiesLength
            };
            LibMPC.SizePageTextBoxes(listTextBox);
            List<Button> listButton = new List<Button>()
            {
                ButBeamPropertiesReturn,
                ButBeamPropertiesClear
            };
            LibMPC.SizePageButtons(listButton);
            SetPageVariables();
        }

        /// <summary>
        /// Set page variables. Use last enter values as defaults. Otherwise use default values.
        /// </summary>
        private void SetPageVariables()
        {
            // CommonItems.stringNameMaterial not used to calculate results.
            CommonItems.stringNameMaterial = LibMPC.DataStoreStringToString(mainPage.applicationDataContainer, CommonItems.ds_StringNameMaterial);
            if (CommonItems.stringNameMaterial.Length.Equals(0))
                TboxBeamPropertiesNameMaterial.Text = string.Empty;   // Set default value.
            else
                TboxBeamPropertiesNameMaterial.Text = CommonItems.stringNameMaterial;      // Use last value entered.
            CommonItems.doubleYoungsModulus = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoubleYoungsModulus);
            if (CommonItems.doubleYoungsModulus.Equals(0d))
            {
                CommonItems.boolEnteredYoungsModulus = false;
                TboxBeamPropertiesYoungsModulus.Text = string.Empty;
            }
            else
            {
                CommonItems.boolEnteredYoungsModulus = true;
                TboxBeamPropertiesYoungsModulus.Text = CommonItems.doubleYoungsModulus.ToString(LibNum.fpNumericFormatSeparator);     // Use last value entered.
            }
            CommonItems.doublePoissonsRatio = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoublePoissonsRatio);
            if (CommonItems.doublePoissonsRatio.Equals(0d))
            {
                CommonItems.boolEnteredPoissonsRatio = false;
                TboxBeamPropertiesPoissonsRatio.Text = string.Empty;
            }
            else
            {
                CommonItems.boolEnteredPoissonsRatio = true;
                TboxBeamPropertiesPoissonsRatio.Text = CommonItems.doublePoissonsRatio.ToString(LibNum.fpNumericFormatNone);     // Use last value entered.
            }
            // CommonItems.stringNameCrossSection not used to calculate results.
            CommonItems.stringNameCrossSection = LibMPC.DataStoreStringToString(mainPage.applicationDataContainer, CommonItems.ds_StringNameCrossSection);
            if (CommonItems.stringNameCrossSection.Length.Equals(0))
                TboxBeamPropertiesNameCrossSection.Text = string.Empty;   // Set default value.
            else
                TboxBeamPropertiesNameCrossSection.Text = CommonItems.stringNameCrossSection;      // Use last value entered.
            CommonItems.doubleInertia = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoubleInertia);
            if (CommonItems.doubleInertia.Equals(0d))
            {
                CommonItems.boolEnteredInertia = false;
                TboxBeamPropertiesInertia.Text = string.Empty;     // Set default value.
            }
            else
            {
                CommonItems.boolEnteredInertia = true;
                TboxBeamPropertiesInertia.Text = CommonItems.doubleInertia.ToString(LibNum.fpNumericFormatNone);   // Use last value entered.
            }
            CommonItems.doubleBeamLength = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoubleBeamLength);
            if (CommonItems.doubleBeamLength.Equals(0d))
                TboxBeamPropertiesLength.Text = string.Empty;      // Set default value.
            else
                TboxBeamPropertiesLength.Text = $"{CommonItems.doubleBeamLength:G}";   // Use last value entered.
            CheckBeamPropertyInputValues();
        }

        /// <summary>
        /// Check and show status of required input items on BeamProperties page. Return true if all items valid, false otherwise.
        /// </summary>
        private bool CheckBeamPropertyInputValues()
        {
            bool boolInputValid = false;
            if (CommonItems.boolEnteredYoungsModulus && CommonItems.boolEnteredPoissonsRatio && CommonItems.boolEnteredInertia && CommonItems.boolEnteredBeamLength)
            {
                boolInputValid = true;
                LibMPC.OutputMsgSuccess(TblkBeamPropertiesDisplay, $"Beam length = {CommonItems.doubleBeamLength.ToString(LibNum.fpNumericFormatNone)} {CommonItems.stringConstUnitsLength}\nAll items on page are valid.");
            }
            else
            {
                // Do not overwrite existing textBlockItemStatus if here!
                string stringErrorMsg = "Following item(s) not entered or not valid:\n";
                if (!CommonItems.boolEnteredYoungsModulus)
                    stringErrorMsg += $"{CommonItems.stringConstYoungsModulus}, ";
                if (!CommonItems.boolEnteredPoissonsRatio)
                    stringErrorMsg += $"{CommonItems.stringConstPoissonsRatio}, ";
                if (!CommonItems.boolEnteredInertia)
                    stringErrorMsg += $"{CommonItems.stringConstInertia}, ";
                if (!CommonItems.boolEnteredBeamLength)
                    stringErrorMsg += CommonItems.stringConstBeamLength;
                stringErrorMsg += "\nCorrect above item(s) to calculate beam results.";
                LibMPC.OutputMsgError(TblkBeamPropertiesDisplay, stringErrorMsg);
            }
            if ((CommonItems.stringNameMaterial.Length > 0) || (CommonItems.stringNameCrossSection.Length > 0) || CommonItems.boolEnteredYoungsModulus || CommonItems.boolEnteredPoissonsRatio || CommonItems.boolEnteredInertia || CommonItems.boolEnteredBeamLength)
                CommonItems.ButtonForegroundColorSet(ButBeamPropertiesClear, true);
            else
                CommonItems.ButtonForegroundColorSet(ButBeamPropertiesClear, false);
            return boolInputValid;
        }

        /*** Entry Methods follow **********************************************************************************************/

        // Next 2 methods handle User entry in TboxBeamPropertiesNameMaterial.

        /// <summary>
        /// Invoked when user presses Enter key while in TboxBeamPropertiesNameMaterial. Value not required since not used to calculate beam results.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesNameMaterial_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)    // Check if Enter key was pressed. Ignore everything else.
            {
                TboxBeamPropertiesNameMaterial_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when focus moves from TboxBeamPropertiesNameMaterial. Value not required since not used to calculate beam results.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesNameMaterial_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("TboxBeamPropertiesNameMaterial_LostFocus(): Event fired.");
            CommonItems.stringNameMaterial = TboxBeamPropertiesNameMaterial.Text;
            mainPage.applicationDataContainer.Values[CommonItems.ds_StringNameMaterial] = TboxBeamPropertiesNameMaterial.Text;
            CheckBeamPropertyInputValues();
        }

        // Next 3 methods handle User entry in TboxBeamPropertiesYoungsModulus.

        /// <summary>
        /// Do following when User presses Enter key in TboxBeamPropertiesYoungsModulus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesYoungsModulus_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                TboxBeamPropertiesYoungsModulus_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxBeamPropertiesYoungsModulus focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesYoungsModulus_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("TboxBeamPropertiesYoungsModulus_LostFocus(): Event fired.");
            CommonItems.doubleYoungsModulus = LibNum.TextBoxGetDouble(TboxBeamPropertiesYoungsModulus, EnumTextBoxUpdate.Yes);      // Get the input from the TextBox and convert to matching numeric.
            if (CommonItems.doubleYoungsModulus >= doubleYoungsModulusMinimum && CommonItems.doubleYoungsModulus <= doubleYoungsModulusMaximum)
            {
                CommonItems.boolEnteredYoungsModulus = true;
                LibMPC.OutputMsgSuccess(TblkBeamPropertiesLostFocus, $"{CommonItems.stringConstYoungsModulus} valid.");
            }
            else
            {
                CommonItems.boolEnteredYoungsModulus = false;
                CommonItems.doubleYoungsModulus = 0d;
                LibMPC.OutputMsgError(TblkBeamPropertiesLostFocus, $"{CommonItems.stringConstYoungsModulus} not valid.  Value must be in range of {doubleYoungsModulusMinimum.ToString(LibNum.fpNumericFormatSeparator)} and {doubleYoungsModulusMaximum.ToString(LibNum.fpNumericFormatSeparator)}.");
            }
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoubleYoungsModulus] = CommonItems.doubleYoungsModulus;
            if (CommonItems.doubleYoungsModulus.Equals(0d))
                TboxBeamPropertiesYoungsModulus.Text = string.Empty;
            else
                TboxBeamPropertiesYoungsModulus.Text = CommonItems.doubleYoungsModulus.ToString(LibNum.fpNumericFormatSeparator);
            CheckBeamPropertyInputValues();
        }

        /// <summary>
        /// On TboxBeamPropertiesYoungsModulus_TextChanged() event, verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesYoungsModulus_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("TboxBeamPropertiesYoungsModulus_TextChanged(): Event fired");
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxBeamPropertiesYoungsModulus, EnumNumericType._double);
        }

        // Next 3 methods handle User entry in TboxBeamPropertiesPoissonsRatio.

        /// <summary>
        /// Do following when User presses Enter key in TboxBeamPropertiesPoissonsRatio.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesPoissonsRatio_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                TboxBeamPropertiesPoissonsRatio_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxBeamPropertiesPoissonsRatio focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesPoissonsRatio_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxBeamPropertiesPoissonsRatio_LostFocus(): Event fired.");
            CommonItems.doublePoissonsRatio = LibNum.TextBoxGetDouble(TboxBeamPropertiesPoissonsRatio, EnumTextBoxUpdate.Yes);      // Get the input from the TextBox and convert to matching numeric.
            if (CommonItems.doublePoissonsRatio > 0d && CommonItems.doublePoissonsRatio < 1d)   // Poisson is a ratio so value must be between 0 and 1.
            {
                CommonItems.boolEnteredPoissonsRatio = true;
                LibMPC.OutputMsgSuccess(TblkBeamPropertiesLostFocus, $"{CommonItems.stringConstPoissonsRatio} valid.");
            }
            else
            {
                CommonItems.boolEnteredPoissonsRatio = false;
                CommonItems.doublePoissonsRatio = 0d;
                LibMPC.OutputMsgError(TblkBeamPropertiesLostFocus, $"{CommonItems.stringConstPoissonsRatio} not valid.  Value must be between 0 and 1.");
            }
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoublePoissonsRatio] = CommonItems.doublePoissonsRatio;
            if (CommonItems.doublePoissonsRatio.Equals(0d))
                TboxBeamPropertiesPoissonsRatio.Text = string.Empty;
            else
                TboxBeamPropertiesPoissonsRatio.Text = CommonItems.doublePoissonsRatio.ToString(LibNum.fpNumericFormatNone);
            CheckBeamPropertyInputValues();
        }

        /// <summary>
        /// On TboxBeamPropertiesPoissonsRatio_TextChanged() event, verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesPoissonsRatio_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxBeamPropertiesPoissonsRatio_TextChanged(): Event fired.");
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxBeamPropertiesPoissonsRatio, EnumNumericType._double);
        }

        // Next 2 methods handle User entry in TboxBeamPropertiesNameCrossSection.

        /// <summary>
        /// Invoked when user presses Enter key while in TboxBeamPropertiesNameCrossSection. Value not required since not used to calculate beam results.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesNameCrossSection_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)    // Check if Enter key was pressed. Ignore everything else.
            {
                TboxBeamPropertiesNameCrossSection_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when focus moves from TboxBeamPropertiesNameCrossSection. Value not required since not used to calculate beam results.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesNameCrossSection_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            // TboxDescriptionCrossSection not needed to complete calculations so can be empty.
            CommonItems.stringNameCrossSection = TboxBeamPropertiesNameCrossSection.Text;
            mainPage.applicationDataContainer.Values[CommonItems.ds_StringNameCrossSection] = TboxBeamPropertiesNameCrossSection.Text;
            CheckBeamPropertyInputValues();
        }

        // Next 3 methods handle User entry in TboxBeamPropertiesInertia.

        /// <summary>
        /// Do following when User presses Enter key in TboxBeamPropertiesInertia.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesInertia_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                TboxBeamPropertiesInertia_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxBeamPropertiesInertia focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesInertia_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxBeamPropertiesInertia_LostFocus(): Event fired.");
            CommonItems.doubleInertia = LibNum.TextBoxGetDouble(TboxBeamPropertiesInertia, EnumTextBoxUpdate.Yes);      // Get the input from the TextBox and convert to matching numeric.
            if (CommonItems.doubleInertia >= doubleInertiaMinimum && CommonItems.doubleInertia <= doubleInertiaMaximum)
            {
                CommonItems.boolEnteredInertia = true;
                LibMPC.OutputMsgSuccess(TblkBeamPropertiesLostFocus, $"{CommonItems.stringConstInertia} valid.");
            }
            else
            {
                CommonItems.boolEnteredInertia = false;
                CommonItems.doubleInertia = 0d;
                LibMPC.OutputMsgError(TblkBeamPropertiesLostFocus, $"{CommonItems.stringConstInertia} not valid.  Value must be in range of {doubleInertiaMinimum.ToString(LibNum.fpNumericFormatNone)} to {doubleInertiaMaximum.ToString(LibNum.fpNumericFormatNone)}.");
            }
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoubleInertia] = CommonItems.doubleInertia;
            if (CommonItems.doubleInertia.Equals(0d))
                TboxBeamPropertiesInertia.Text = string.Empty;
            else
                TboxBeamPropertiesInertia.Text = CommonItems.doubleInertia.ToString(LibNum.fpNumericFormatNone);
            CheckBeamPropertyInputValues();
        }

        /// <summary>
        /// On TboxInertia_TextChanged() event, verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesInertia_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxBeamPropertiesInertia_TextChanged(): Event fired!");
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxBeamPropertiesInertia, EnumNumericType._double);
        }

        // Next 3 methods handle User entry in TboxBeamPropertiesLength.

        /// <summary>
        /// Do following when User presses Enter key in TboxBeamPropertiesLength.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesLength_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                //Debug.WriteLine($"TboxBeamPropertiesLength_KeyDown(): Event fired.");
                TboxBeamPropertiesLength_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxBeamPropertiesLength focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesLength_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxBeamPropertiesLength_LostFocus(): Event fired.");
            // Save input to local beam length value, then check if valid, if valid then compare to global variable to check if value has changed.
            double doubleBeamLength = LibNum.TextBoxGetDouble(TboxBeamPropertiesLength, EnumTextBoxUpdate.Yes);   // Get the input from the TextBox and convert to matching numeric.
            if (doubleBeamLength >= doubleBeamLengthMinimum && doubleBeamLength <= doubleBeamLengthMaximum)
            {
                string stringSuccessMessage = $"{CommonItems.stringConstBeamLength} valid.";     // Default success string.
                CommonItems.boolEnteredBeamLength = true;
                if (!doubleBeamLength.Equals(CommonItems.doubleBeamLength))
                {
                    // Beam length changed.
                    CommonItems.doubleBeamLength = doubleBeamLength;
                    if (CommonItems.ClearLoadsConcentrated() || CommonItems.ClearLoadsUniform())
                    {
                        // Loads cannot be entered unless supports entered, so also clear supports.
                        CommonItems.ClearSupports();
                        stringSuccessMessage = $"{CommonItems.stringConstBeamLength} valid but has changed so cleared existing supports and loads.";

                    }
                    else if (CommonItems.ClearSupports())
                    {
                        stringSuccessMessage = $"{CommonItems.stringConstBeamLength} valid but has changed so cleared existing supports.";
                    }
                }
                LibMPC.OutputMsgSuccess(TblkBeamPropertiesLostFocus, stringSuccessMessage);
            }
            else
            {
                CommonItems.boolEnteredBeamLength = false;
                CommonItems.doubleBeamLength = 0d;
                LibMPC.OutputMsgError(TblkBeamPropertiesLostFocus, $"{CommonItems.stringConstBeamLength} not valid.  Value must be in range of {doubleBeamLengthMinimum.ToString(LibNum.fpNumericFormatNone)} to {doubleBeamLengthMaximum.ToString(LibNum.fpNumericFormatNone)}.");
            }
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoubleBeamLength] = CommonItems.doubleBeamLength;
            if (CommonItems.doubleBeamLength.Equals(0d))
                TboxBeamPropertiesLength.Text = string.Empty;
            else
                TboxBeamPropertiesLength.Text = CommonItems.doubleBeamLength.ToString(LibNum.fpNumericFormatNone);
            CheckBeamPropertyInputValues();
        }

        /// <summary>
        /// On TboxBeamPropertiesLength_TextChanged() event, verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxBeamPropertiesLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxBeamPropertiesLength_TextChanged(): Event fired.");
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxBeamPropertiesLength, EnumNumericType._double);
        }

        /// <summary>
        /// Return to Home page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButBeamPropertiesReturn_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"ButBeamPropertiesReturn_Click(): Event fired.");
            mainPage.ShowPageHome();
        }

        /// <summary>
        /// Clear beam properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButBeamPropertiesClear_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButBeamPropertiesClear_Click() event fired.");
            CommonItems.stringNameMaterial = TboxBeamPropertiesNameMaterial.Text = string.Empty;
            mainPage.applicationDataContainer.Values[CommonItems.ds_StringNameMaterial] = TboxBeamPropertiesNameMaterial.Text;
            TboxBeamPropertiesYoungsModulus.Text = string.Empty;
            CommonItems.boolEnteredYoungsModulus = false;
            CommonItems.doubleYoungsModulus = 0d;
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoubleYoungsModulus] = CommonItems.doubleYoungsModulus;
            TboxBeamPropertiesPoissonsRatio.Text = string.Empty;
            CommonItems.boolEnteredPoissonsRatio = false;
            CommonItems.doublePoissonsRatio = 0d;
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoublePoissonsRatio] = CommonItems.doublePoissonsRatio;
            CommonItems.stringNameCrossSection = TboxBeamPropertiesNameCrossSection.Text = string.Empty;
            mainPage.applicationDataContainer.Values[CommonItems.ds_StringNameCrossSection] = TboxBeamPropertiesNameCrossSection.Text;
            TboxBeamPropertiesInertia.Text = string.Empty;
            CommonItems.boolEnteredInertia = false;
            CommonItems.doubleInertia = 0d;
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoubleInertia] = CommonItems.doubleInertia;
            TboxBeamPropertiesLength.Text = string.Empty;
            CommonItems.boolEnteredBeamLength = false;
            CommonItems.doubleBeamLength = 0d;
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoubleBeamLength] = CommonItems.doubleBeamLength;
            // Also clear supports and loads.
            CommonItems.ClearSupports();
            CommonItems.ClearLoadsConcentrated();
            CommonItems.ClearLoadsUniform();
            CheckBeamPropertyInputValues();
            LibMPC.OutputMsgSuccess(TblkBeamPropertiesLostFocus, "Cleared all items in this page.  Also cleared any existing supports and loads.");
        }

        /// <summary>
        /// Common LostFocus event for buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButBeamProperties_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButBeamProperties_LostFocus() event fired.");
            TblkBeamPropertiesLostFocus.Text = string.Empty;
        }

    }
}
