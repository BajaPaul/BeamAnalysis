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
    public sealed partial class EnterSupports : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        /// <summary>
        /// Support position from left end of beam. Minimum value is 0d. Maximum value is CommonItems.doubleBeamLength.
        /// </summary>
        private double doubleSupportPosition;

        /// <summary>
        /// Enable displacement at support if true, otherwise disable displacement at support.
        /// True corresponds to restraint value of 0. False corresponds to restraint value of 1.
        /// </summary>
        private bool boolSupportDisplacement;

        /// <summary>
        /// Enable rotation at support if true, otherwise disable rotation at support.
        /// True corresponds to restraint value of 0. False corresponds to restraint value of 1.
        /// </summary>
        private bool boolSupportRotation;

        /// <summary>
        /// True if User has entered valid doubleSupportPosition value, false otherwise.
        /// </summary>
        private bool boolEnteredSupportPosition;

        /// <summary>
        /// Toggle support output message in method AddSupport() depending if DefinedSupport entered or support entered via ButSupportAdd_Click().
        /// </summary>
        private bool boolDefinedSupportEntered;

        /// <summary>
        /// Bool is true if all DefinedSupport values entered without error, false otherwise. 
        /// </summary>
        private bool boolDefinedSupportEnteredAll;

        /// <summary>
        /// Page that allows User to add supports to beam.
        /// </summary>
        public EnterSupports()
        {
            InitializeComponent();
            // Set MainPage Button Visibility.
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, true);
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButSamples, false);
            // Overwrite XAML TextBlock Foreground colors.
            TblkSupportNote.Foreground = LibMPC.colorBright;
            TblkSupportBeamLength.Foreground = LibMPC.colorSuccess;
            TblkSupportDisplay.Foreground = LibMPC.colorSuccess;
            TblkSupportPosition.Foreground = LibMPC.colorNormal;
            TblkSupportDisplacement.Foreground = LibMPC.colorNormal;
            TblkSupportRotation.Foreground = LibMPC.colorNormal;
            // Overwrite XAML default values.
            TblkSupportNote.Text = "Enter one or more supports using consistent USC or SI units.  Enter support position from left end of beam and set displacement and rotation toggles on or off.  Entered supports should create a stable beam.";
            TblkSupportBeamLength.Text = $"{CommonItems.stringConstBeamLength} = {CommonItems.doubleBeamLength} {CommonItems.stringConstUnitsLength}";
            TblkSupportResult.Text = string.Empty;
            TblkSupportPosition.Text = $"{CommonItems.stringConstPosition} {CommonItems.stringConstPositionReference} {CommonItems.stringConstUnitsLength}";
            TblkSupportDisplacement.Text = $"{CommonItems.stringConstDisplacement}";
            TblkSupportRotation.Text = $"{CommonItems.stringConstRotation}";
            ButSupportAdd.Content = "Add above support";
            ButSupportReturn.Content = $"{CommonItems.stringConstButtonReturn}";
            ButSupportDefined1.Content = "Add simple supports at each end of beam";
            ButSupportDefined2.Content = "Add cantilever support at left end of beam";
            // Set XAML PlaceholderText values.
            TboxSupportPosition.PlaceholderText = CommonItems.stringConstPosition;
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
            List<ToggleSwitch> listToggleSwitchesThisPage = new List<ToggleSwitch>()
            {
                TogSupportDisplacement,
                TogSupportRotation
            };
            LibMPC.SizePageToggleSwitches(listToggleSwitchesThisPage);
            List<Button> listButton = new List<Button>()
            {
                ButSupportAdd,
                ButSupportReturn,
                ButSupportClear
            };
            LibMPC.SizePageButtons(listButton);
            // Only one TextBox this pages so set MinWidth to above button widths.
            TboxSupportPosition.MinWidth = ButSupportClear.MinWidth;
            // Resize following defined support buttons separately since description is much longer.
            listButton = new List<Button>()
            {
                ButSupportDefined1,
                ButSupportDefined2
            };
            LibMPC.SizePageButtons(listButton);
            SetDefaultValuesSupport();
            CommonItems.ButtonForegroundColorSet(ButSupportReturn, true);
            CommonItems.ButtonForegroundColorSet(ButSupportClear, CommonItems.boolEnteredSupport);
        }

        /// <summary>
        /// Set default values for support.
        /// </summary>
        private void SetDefaultValuesSupport()
        {
            // Next 3 lines initialize support position values.
            TboxSupportPosition.Text = string.Empty;
            doubleSupportPosition = 0d;
            boolEnteredSupportPosition = false;
            // Next 2 lines initialize support displacement values.
            TogSupportDisplacement.IsOn = false;
            boolSupportDisplacement = false;
            // Next 2 lines initialize support rotation values.
            TogSupportRotation.IsOn = true;
            boolSupportRotation = true;

            TblkSupportDisplay.Text = CommonItems.ShowEnteredSupportsAndLoads();
            TblkSupportLostFocus.Text = string.Empty;
            CommonItems.ButtonForegroundColorSet(ButSupportAdd, false);
        }

        /// <summary>
        /// Check if support input values are valid and toggle foreground color of ButSupportAdd accordingly.
        /// </summary>
        /// <returns></returns>
        private void CheckSupportInputValues()
        {
            // Need to check if all input values valid each time method is called since they values can be changed by User any time.
            bool boolSupportInputValuesValid = true;
            if (TboxSupportPosition.Text.Length == 0)
                boolSupportInputValuesValid = false;
            // Check if support position is on beam.
            if (doubleSupportPosition < 0d || doubleSupportPosition > CommonItems.doubleBeamLength)
                boolSupportInputValuesValid = false;
            // Check if displacement and rotation bools are both true. Support is not valid if so since not really a support in this case.
            if (boolSupportDisplacement == true && boolSupportRotation == true)
                boolSupportInputValuesValid = false;
            CommonItems.ButtonForegroundColorSet(ButSupportAdd, boolSupportInputValuesValid);
        }

        /// <summary>
        /// Add support to beam.
        /// </summary>
        private void AddSupport()
        {
            //Debug.WriteLine($"AddSupport(): doubleSupportPosition={doubleSupportPosition}, boolSupportDisplacement={boolSupportDisplacement}, boolSupportRotation={boolSupportRotation}");
            // Check displacement and rotation bools are not both true since not a support if so.
            if (!(boolSupportDisplacement == true && boolSupportRotation == true))
            {
                bool boolDuplicateSupportFound = false;
                foreach (SupportValues supportValues in CommonItems.listSupportValues)
                {
                    if (supportValues.DoubleSupportPosition.Equals(doubleSupportPosition))
                    {
                        boolDuplicateSupportFound = true;
                        break;
                    }
                }
                if (boolDuplicateSupportFound)
                {
                    if (boolDefinedSupportEntered)
                    {
                        boolDefinedSupportEnteredAll = false;
                        LibMPC.OutputMsgError(TblkSupportResult, "One or more supports not added since support already entered at position.");
                    }
                    else
                        LibMPC.OutputMsgError(TblkSupportResult, "Support not added since support already entered at position.");
                }
                else
                {
                    SupportValues supportValuesAdd = new SupportValues
                    {
                        DoubleSupportPosition = doubleSupportPosition,
                        BoolSupportDisplacement = boolSupportDisplacement,
                        BoolSupportRotation = boolSupportRotation
                    };
                    CommonItems.listSupportValues.Add(supportValuesAdd);     // Add support values to support list.


                    if (boolDefinedSupportEntered)
                    {
                        if (boolDefinedSupportEnteredAll)           // Do not overwrite error message if false.
                            LibMPC.OutputMsgSuccess(TblkSupportResult, "Support(s) added.");
                    }
                    else
                    {
                        LibMPC.OutputMsgSuccess(TblkSupportResult, "Support added.");
                    }
                    if (CommonItems.listSupportValues.Count > 0)
                    {
                        // More than one support entered so sort by DoubleSupportPosition.
                        CommonItems.listSupportValues.Sort((x, y) => x.DoubleSupportPosition.CompareTo(y.DoubleSupportPosition));
                    }
                    CommonItems.boolEnteredSupport = true;
                }
            }
            else
            {
                LibMPC.OutputMsgError(TblkSupportResult, "Support not added since displacement and rotation at support were both on.");
            }
            CommonItems.ButtonForegroundColorSet(ButSupportClear, CommonItems.boolEnteredSupport);
            SetDefaultValuesSupport();
        }

        /// <summary>
        /// Set defined support values.
        /// </summary>
        /// <param name="listSupportValues"></param>
        private void SetSupportDefinedValues(List<SupportValues> listSupportValues)
        {
            //Debug.WriteLine("SetSupportDefinedValues(): Method fired.");
            boolDefinedSupportEntered = true;
            boolDefinedSupportEnteredAll = true;
            foreach (SupportValues supportValues in listSupportValues)
            {
                doubleSupportPosition = supportValues.DoubleSupportPosition;
                boolSupportDisplacement = supportValues.BoolSupportDisplacement;
                boolSupportRotation = supportValues.BoolSupportRotation;
                AddSupport();
            }
            SetDefaultValuesSupport();
            boolDefinedSupportEntered = false;
            boolDefinedSupportEnteredAll = true;
        }

        /*** Support entry events follow ****************************************************************************************/

        // Next 3 methods handle User entry in TboxSupportPosition.

        /// <summary>
        /// Do following when User presses Enter key in TboxSupportPosition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxSupportPosition_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed.  Ignore everything else.
            {
                //Debug.WriteLine($"TboxSupportPosition_KeyDown(): Event fired!");
                TboxSupportPosition_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxSupportPosition focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxSupportPosition_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxSupportPosition_LostFocus(): Event fired.");
            boolEnteredSupportPosition = false;
            if (TboxSupportPosition.Text.Length > 0)
            {
                doubleSupportPosition = LibNum.TextBoxGetDouble(TboxSupportPosition, EnumTextBoxUpdate.Yes);         // Get the input from the TextBox and convert to matching numeric.
                if (doubleSupportPosition >= 0d && doubleSupportPosition <= CommonItems.doubleBeamLength)  // Verify support is on beam.
                {
                    boolEnteredSupportPosition = true;
                    LibMPC.OutputMsgSuccess(TblkSupportLostFocus, "Valid support position entered.");
                }
                else
                {
                    LibMPC.OutputMsgError(TblkSupportLostFocus, "Invalid support position entered.  Support must be on beam.");
                }
            }
            else
            {
                LibMPC.OutputMsgError(TblkSupportLostFocus, "Invalid support position since no value entered.");
            }
            CheckSupportInputValues();
        }

        /// <summary>
        /// Verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxSupportPosition_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxSupportPosition_TextChanged(): Event fired!");
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxSupportPosition, EnumNumericType._double);
        }

        /// <summary>
        /// Toggle support displacement restraint.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogSupportDisplacement_Toggled(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            TogSupportDisplacement_LostFocus(null, null);
        }

        /// <summary>
        /// Support displacement LostFocus event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogSupportDisplacement_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TogSupportDisplacement_LostFocus(): Event fired.");
            if (TogSupportDisplacement.IsOn == true)
            {
                boolSupportDisplacement = true;
                LibMPC.OutputMsgSuccess(TblkSupportLostFocus, "Displacement at support is On.");
            }
            else
            {
                boolSupportDisplacement = false;
                LibMPC.OutputMsgSuccess(TblkSupportLostFocus, "Displacement at support is Off.");
            }
            CheckSupportInputValues();
        }

        /// <summary>
        /// Toggle support rotation restraint.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogSupportRotation_Toggled(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            TogSupportRotation_LostFocus(null, null);
        }

        /// <summary>
        /// Support ratation LostFocus event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogSupportRotation_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TogSupportRotation_LostFocus(): Event fired.");
            if (TogSupportRotation.IsOn == true)
            {
                boolSupportRotation = true;
                LibMPC.OutputMsgSuccess(TblkSupportLostFocus, "Rotation at support is On.");
            }
            else
            {
                boolSupportRotation = false;
                LibMPC.OutputMsgSuccess(TblkSupportLostFocus, "Rotation at support is Off.");
            }
            CheckSupportInputValues();
        }

        /// <summary>
        /// Add a support.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButSupportAdd_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"ButSupportAdd_Click(): Event fired.");
            TblkSupportLostFocus.Text = string.Empty;
            if (boolEnteredSupportPosition)
            {
                boolDefinedSupportEntered = false;
                boolDefinedSupportEnteredAll = true;
                AddSupport();
            }
            else
            {
                CommonItems.ButtonForegroundColorSet(ButSupportAdd, boolEnteredSupportPosition);
                LibMPC.OutputMsgError(TblkSupportResult, "Support not added since support position not entered or not valid.");
            }
            CommonItems.ButtonForegroundColorSet(ButSupportClear, CommonItems.boolEnteredSupport);
        }

        /// <summary>
        /// Check if support values valid. Return to Home page if so.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButSupportReturn_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"ButSupportReturn_Click(): Event fired.");
            mainPage.ShowPageHome();
        }

        /// <summary>
        /// Clear support list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButSupportClear_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButSupportClear_Click() event fired.");
            if (CommonItems.ClearSupports())
            {
                TblkSupportDisplay.Text = string.Empty;
                LibMPC.OutputMsgSuccess(TblkSupportResult, "Supports were cleared.");
            }
            else
            {
                TblkSupportLostFocus.Text = string.Empty;
                LibMPC.OutputMsgError(TblkSupportResult, "Supports were not cleared since no supports entered.");
            }
            SetDefaultValuesSupport();  // Always reset default support values.
            CommonItems.ButtonForegroundColorSet(ButSupportAdd, false);
            CommonItems.ButtonForegroundColorSet(ButSupportClear, false);
        }

        /// <summary>
        /// Add defined support #1 to beam.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButSupportDefined1_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"ButSupportDefined1_Click(): Event fired.");
            List<SupportValues> listSupportValues = new List<SupportValues>
            {
                new SupportValues() { DoubleSupportPosition = 0d, BoolSupportDisplacement = false, BoolSupportRotation = true},
                new SupportValues() { DoubleSupportPosition = CommonItems.doubleBeamLength, BoolSupportDisplacement = false, BoolSupportRotation = true}
            };
            SetSupportDefinedValues(listSupportValues);
        }

        /// <summary>
        /// Add defined support #2 to beam.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButSupportDefined2_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"ButSupportDefined2_Click(): Event fired.");
            List<SupportValues> listSupportValues = new List<SupportValues>
            {
                new SupportValues() { DoubleSupportPosition = 0d, BoolSupportDisplacement = false, BoolSupportRotation = false}
            };
            SetSupportDefinedValues(listSupportValues);
        }

        /// <summary>
        /// Common LostFocus event for buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButSupport_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButSupport_LostFocus() event fired.");
            TblkSupportLostFocus.Text = string.Empty;
        }

    }
}
