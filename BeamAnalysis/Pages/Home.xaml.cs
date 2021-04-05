using BeamAnalysis.Common;
using BeamAnalysis.CommonFEM;
using LibraryCoder.MainPageCommon;
using LibraryCoder.Numerics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace BeamAnalysis.Pages
{
    public sealed partial class Home : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        /// <summary>
        /// Limit doubleOutputSegmentLength minimum value.  Value is 0.0254. This value allows a meter to be divided into 1 inch segments.
        /// </summary>
        private const double doubleOutputSegmentLengthMinimum = 0.0254;

        /// <summary>
        /// Combine two concentrated loads at vertical force centroid if they are within this distance of each other.
        /// </summary>
        private double doubleCombineLoadDistance;

        /// <summary>
        /// Limit doubleCombineLoadDistance minimum value. Value is 0.01.
        /// </summary>
        private const double doubleCombineLoadDistanceMinimum = 0.01;

        /// <summary>
        /// Limit doubleCombineLoadDistance maximum value. Value is 1.0.
        /// </summary>
        private const double doubleCombineLoadDistanceMaximum = 1.0;

        /// <summary>
        /// True if User has entered valid combine load distance value, false otherwise.
        /// </summary>
        private bool boolEnteredCombineLoadDistance = false;

        /// <summary>
        /// Home page that loads after MainPage that references other pages so User can construct beam model and output results.
        /// </summary>
        public Home()
        {
            InitializeComponent();
            // Hide XAML layout rectangles by setting their color to RelativePanel Background color.
            RectLayoutCenter.Fill = Rpanel.Background;
            RectLayoutLeft.Fill = Rpanel.Background;
            RectLayoutRight.Fill = Rpanel.Background;
            // Set MainPage Button Visibility.
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, true);
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButSamples, true);
            // Overwrite XAML TextBlock Foreground colors.
            TblkHomeNote.Foreground = LibMPC.colorBright;
            TblkHomeOutputSegmentLength.Foreground = LibMPC.colorNormal;
            TblkHomeCombineLoadDistance.Foreground = LibMPC.colorNormal;
            TblkHomeMatricesShow.Foreground = LibMPC.colorNormal;
            TblkHomeCalculate.Foreground = LibMPC.colorError;
            // Overwrite XAML default values.
            TblkHomeNote.Text = $"{CommonItems.stringConstApplicationNote}\n\n{CommonItems.stringConstUnitsUSCInch}\n{CommonItems.stringConstUnitsUSCFoot}\n{CommonItems.stringConstUnitsSI}";
            TblkHomeStatus.Text = string.Empty;
            TblkHomeDisplay.Text = string.Empty;
            TblkHomeOutputSegmentLength.Text = $"{CommonItems.stringConstOutputSegmentLength} per unit length of beam.\nDisplay results at these intervals if no supports or loads entered at position.\nValue will be adjusted to so all segments have equal length.\nSmaller values output more accurate results but take longer to calculate.";
            TblkHomeCombineLoadDistance.Text = $"{CommonItems.stringConstCombineLoadDistance}.  Value must be less than {CommonItems.stringConstOutputSegmentLength}.\nCombine loads if they are within this distance of each other.\nThis value can greatly simplify model with little loss of accuracy if multiple uniform loads entered.";
            TblkHomeMatricesShow.Text = "Show matrices in output results.";
            TblkHomeCalculate.Text = string.Empty;
            ButHomeProperties.Content = "Enter beam properties";
            ButHomeSupports.Content = "Enter supports";
            ButHomeConcentratedLoads.Content = "Enter concentrated loads";
            ButHomeUniformLoads.Content = "Enter uniform loads";
            ButHomeCalculate.Content = "Calculate beam results";
            // Set XAML PlaceholderText values.
            TboxHomeOutputSegmentLength.PlaceholderText = CommonItems.stringConstOutputSegmentLength;
            TboxHomeCombineLoadDistance.PlaceholderText = CommonItems.stringConstCombineLoadDistance;
            PBarHomeStatus.Foreground = LibMPC.colorSuccess;
            ButHomePurchaseApp.Foreground = LibMPC.colorSuccess;
            // Setup scrolling for this page.
            LibMPC.ScrollViewerOn(mainPage.mainPageScrollViewer, horz: ScrollMode.Disabled, vert: ScrollMode.Auto, horzVis: ScrollBarVisibility.Disabled, vertVis: ScrollBarVisibility.Auto, zoom: ZoomMode.Disabled);
        }

        /*** Private methods follow ********************************************************************************************/

        /// <summary>
        /// Code that runs after page is loaded that will not execute properly until page is rendered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            List<TextBox> listTextBox = new List<TextBox>()
            {
                TboxHomeOutputSegmentLength,
                TboxHomeCombineLoadDistance
            };
            LibMPC.SizePageTextBoxes(listTextBox);
            List<Button> listButton = new List<Button>()
            {
                ButHomeProperties,
                ButHomeSupports,
                ButHomeConcentratedLoads,
                ButHomeUniformLoads,
                ButHomeCalculate,
                ButHomePurchaseApp
            };
            LibMPC.SizePageButtons(listButton);
            LoadBeamValues();
            SetButtonForegroundColors();
            TblkHomeLostFocus.Text = string.Empty;
            await AppPurchaseCheck();
            AppRatedCheck();
        }

        /// <summary>
        /// Load beam values.
        /// </summary>
        private void LoadBeamValues()
        {
            // CommonItems.stringNameMaterial not used to calculate results.
            CommonItems.stringNameMaterial = LibMPC.DataStoreStringToString(mainPage.applicationDataContainer, CommonItems.ds_StringNameMaterial);
            CommonItems.doubleYoungsModulus = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoubleYoungsModulus);
            if (CommonItems.doubleYoungsModulus == 0d)
            {
                CommonItems.boolEnteredYoungsModulus = false;
            }
            else
            {
                CommonItems.boolEnteredYoungsModulus = true;
            }
            CommonItems.doublePoissonsRatio = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoublePoissonsRatio);
            if (CommonItems.doublePoissonsRatio == 0d)
            {
                CommonItems.boolEnteredPoissonsRatio = false;
            }
            else
            {
                CommonItems.boolEnteredPoissonsRatio = true;
            }
            // CommonItems.stringNameCrossSection not used to calculate results.
            CommonItems.stringNameCrossSection = LibMPC.DataStoreStringToString(mainPage.applicationDataContainer, CommonItems.ds_StringNameCrossSection);
            CommonItems.doubleInertia = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoubleInertia);
            if (CommonItems.doubleInertia == 0d)
            {
                CommonItems.boolEnteredInertia = false;
            }
            else
            {
                CommonItems.boolEnteredInertia = true;
            }
            CommonItems.doubleBeamLength = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoubleBeamLength);
            if (CommonItems.doubleBeamLength == 0d)
            {
                CommonItems.boolEnteredBeamLength = false;
            }
            else
            {
                CommonItems.boolEnteredBeamLength = true;
            }
            CommonItems.doubleOutputSegmentLength = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoubleOutputSegmentLength);
            if (CommonItems.doubleOutputSegmentLength == 0d)
            {
                CommonItems.boolEnteredOutputSegmentLength = false;
                TboxHomeOutputSegmentLength.Text = string.Empty;     // Set default value.
            }
            else
            {
                CommonItems.boolEnteredOutputSegmentLength = true;
                TboxHomeOutputSegmentLength.Text = CommonItems.doubleOutputSegmentLength.ToString(LibNum.fpNumericFormatNone);     // Use last value entered.
            }
            doubleCombineLoadDistance = LibMPC.DataStoreStringToDouble(mainPage.applicationDataContainer, CommonItems.ds_DoubleCombineLoadDistance);
            if (doubleCombineLoadDistance == 0d)
            {
                boolEnteredCombineLoadDistance = false;
                TboxHomeCombineLoadDistance.Text = string.Empty;     // Set default value.
            }
            else
            {
                boolEnteredCombineLoadDistance = true;
                TboxHomeCombineLoadDistance.Text = doubleCombineLoadDistance.ToString(LibNum.fpNumericFormatNone);     // Use last value entered.
            }
            CommonItems.boolMatricesShow = LibMPC.DataStoreStringToBool(mainPage.applicationDataContainer, CommonItems.ds_BoolMatricesShow);

            if (CommonItems.boolMatricesShow)
                TogHomeMatricesShow.IsOn = true;
            else
                TogHomeMatricesShow.IsOn = false;

            CheckHomeInputValues();
        }

        /// <summary>
        /// Set foreground color of buttons in this page depending on related bool values.
        /// </summary>
        private void SetButtonForegroundColors()
        {
            CommonItems.ButtonForegroundColorSet(ButHomeProperties, true);

            if (CommonItems.boolEnteredBeamLength)
            {
                CommonItems.ButtonForegroundColorSet(ButHomeSupports, true);
            }
            else
            {
                CommonItems.ButtonForegroundColorSet(ButHomeSupports, false);
            }
            if (CommonItems.boolEnteredBeamLength && CommonItems.boolEnteredSupport && CommonItems.boolEnteredOutputSegmentLength && boolEnteredCombineLoadDistance)
            {
                CommonItems.ButtonForegroundColorSet(ButHomeConcentratedLoads, true);
                CommonItems.ButtonForegroundColorSet(ButHomeUniformLoads, true);
            }
            else
            {
                CommonItems.ButtonForegroundColorSet(ButHomeConcentratedLoads, false);
                CommonItems.ButtonForegroundColorSet(ButHomeUniformLoads, false);
            }
        }

        /// <summary>
        /// Consolidate all concentrated loads and uniform load simulated loads to listLoadConcentratedValues.
        /// </summary>
        /// <param name="listLoadConcentratedValues">Ref list to update that is defined by calling method.</param>
        private void ConsolidateConcentratedAndUniformLoads(ref List<LoadConcentratedValues> listLoadConcentratedValues)
        {
            CommonItems.boolFoundLoadConsolidated = false;
            bool boolLoadConcentrated = false;
            bool boolLoadSimulated = false;
            if (CommonItems.listLoadConcentratedValues.Count > 0)
            {
                foreach (LoadConcentratedValues loadConcentratedValues in CommonItems.listLoadConcentratedValues)
                {
                    listLoadConcentratedValues.Add(loadConcentratedValues);
                    boolLoadConcentrated = true;
                }
            }
            if (CommonItems.listLoadUniformValues.Count > 0)
            {
                foreach (LoadUniformValues loadUniformValues in CommonItems.listLoadUniformValues)
                {
                    if (loadUniformValues.ListSimulatedLoads.Count > 0)
                    {
                        foreach (LoadConcentratedValues loadConcentrateValueSimulated in loadUniformValues.ListSimulatedLoads)
                        {
                            listLoadConcentratedValues.Add(loadConcentrateValueSimulated);
                            boolLoadSimulated = true;
                        }
                    }
                }
            }
            // Sort by DoubleLoadConcentratedPosition if more than one load found.
            if (listLoadConcentratedValues.Count > 1)
            {
                if (boolLoadConcentrated && boolLoadSimulated)
                {
                    // Concentrated loads and simulated loads consolidated.
                    CommonItems.boolFoundLoadConsolidated = true;
                }
                listLoadConcentratedValues.Sort((x, y) => x.DoubleLoadConcentratedPosition.CompareTo(y.DoubleLoadConcentratedPosition));
            }
            //
            ////Debug.WriteLine($"\nConsolidateConcentratedAndUniformLoads(): List of loads:");
            ////foreach (LoadConcentratedValues loadConcentratedValues in listLoadConcentratedValues)
            ////{
            ////    Debug.WriteLine($"  Position={loadConcentratedValues.DoubleLoadConcentratedPosition}, Force={loadConcentratedValues.DoubleLoadConcentratedForce}, Moment={loadConcentratedValues.DoubleLoadConcentratedMoment}");
            ////}
        }

        /// <summary>
        /// Check if any concentrated loads in listLoadConcentratedValues on are on a support and offset accordingly.
        /// </summary>
        /// <param name="listLoadConcentratedValues">Ref list to update that is defined by calling method.</param>
        private void CheckIfLoadsOnSupport(ref List<LoadConcentratedValues> listLoadConcentratedValues)
        {
            CommonItems.boolFoundLoadOnSupport = false;
            double doubleOffsetPositionRight;
            double doubleOffsetPositionLeft;
            double doubleOffsetForce;
            double doubleOffsetMoment;
            bool boolLoadAdded = false;     // Sort listLoadConcentratedValues if concentrated load is on a support between endpoints since a load will be added to right side of support.
            LoadConcentratedValues loadConcentratedValuesOffset;
            int intLoadsConcentratedCount = listLoadConcentratedValues.Count;
            for (int i = 0; i < intLoadsConcentratedCount; i++)
            {
                foreach (SupportValues supportValues in CommonItems.listSupportValues)
                {
                    // LibNum.EqualByRounding() method to check equality in next if statement. Should this method be used elsewhere?
                    if (LibNum.EqualByRounding(supportValues.DoubleSupportPosition, listLoadConcentratedValues[i].DoubleLoadConcentratedPosition, CommonItems.intDoubleEqual))
                    {
                        CommonItems.boolFoundLoadOnSupport = true;
                        if (supportValues.DoubleSupportPosition == 0d)
                        {
                            //Debug.WriteLine($"Home.CheckIfLoadsOnSupport(): i={i}, Found load on support at left end of beam so offset load position to right.");
                            doubleOffsetPositionRight = listLoadConcentratedValues[i].DoubleLoadConcentratedPosition + PostProcessor.doubleOffsetDistanceSupport;
                            doubleOffsetForce = listLoadConcentratedValues[i].DoubleLoadConcentratedForce;
                            doubleOffsetMoment = listLoadConcentratedValues[i].DoubleLoadConcentratedMoment;
                            // Remove load that is on right support.
                            listLoadConcentratedValues.RemoveAt(i);
                            // Create new load to add to right of support.
                            loadConcentratedValuesOffset = new LoadConcentratedValues
                            {
                                DoubleLoadConcentratedPosition = doubleOffsetPositionRight,
                                DoubleLoadConcentratedForce = doubleOffsetForce,
                                DoubleLoadConcentratedMoment = doubleOffsetMoment
                            };
                            //Debug.WriteLine($"Home.CheckIfLoadsOnSupport(): Found load on support at left end of beam, new load values are: i={i}, Position={loadConcentratedValuesOffset.DoubleLoadConcentratedPosition}, Force={loadConcentratedValuesOffset.DoubleLoadConcentratedForce}, Moment={loadConcentratedValuesOffset.DoubleLoadConcentratedMoment}");
                            listLoadConcentratedValues.Insert(i, loadConcentratedValuesOffset);
                            break;      // Exit support foreach loop and move on to next load.
                        }
                        else if (supportValues.DoubleSupportPosition == CommonItems.doubleBeamLength)
                        {
                            //Debug.WriteLine($"Home.CheckIfLoadsOnSupport(): i={i}, Found load on support at right end of beam so offset load position to left.");
                            doubleOffsetPositionLeft = listLoadConcentratedValues[i].DoubleLoadConcentratedPosition - PostProcessor.doubleOffsetDistanceSupport;
                            doubleOffsetForce = listLoadConcentratedValues[i].DoubleLoadConcentratedForce;
                            doubleOffsetMoment = listLoadConcentratedValues[i].DoubleLoadConcentratedMoment;
                            // Remove load that is on left support.
                            listLoadConcentratedValues.RemoveAt(i);
                            // Create new load to add to left of support.
                            loadConcentratedValuesOffset = new LoadConcentratedValues
                            {
                                DoubleLoadConcentratedPosition = doubleOffsetPositionLeft,
                                DoubleLoadConcentratedForce = doubleOffsetForce,
                                DoubleLoadConcentratedMoment = doubleOffsetMoment
                            };
                            //Debug.WriteLine($"Home.CheckIfLoadsOnSupport(): Found load on support at right end of beam, new load values are: i={i}, Position={loadConcentratedValuesOffset.DoubleLoadConcentratedPosition}, Force={loadConcentratedValuesOffset.DoubleLoadConcentratedForce}, Moment={loadConcentratedValuesOffset.DoubleLoadConcentratedMoment}");
                            listLoadConcentratedValues.Insert(i, loadConcentratedValuesOffset);
                            break;      // Exit support foreach loop and move on to next load.
                        }
                        else
                        {
                            //Debug.WriteLine($"Home.CheckIfLoadsOnSupport(): i={i}, Found load on support between beam endpoints so split load force and moment values to positions left and right of support position.");
                            doubleOffsetPositionLeft = listLoadConcentratedValues[i].DoubleLoadConcentratedPosition - PostProcessor.doubleOffsetDistanceSupport;
                            doubleOffsetPositionRight = listLoadConcentratedValues[i].DoubleLoadConcentratedPosition + PostProcessor.doubleOffsetDistanceSupport;
                            doubleOffsetForce = listLoadConcentratedValues[i].DoubleLoadConcentratedForce / 2d;
                            doubleOffsetMoment = listLoadConcentratedValues[i].DoubleLoadConcentratedMoment / 2d;
                            // Remove load that is on support between beam endpoints.
                            listLoadConcentratedValues.RemoveAt(i);
                            // Create new load to add to left of support.
                            loadConcentratedValuesOffset = new LoadConcentratedValues
                            {
                                DoubleLoadConcentratedPosition = doubleOffsetPositionLeft,
                                DoubleLoadConcentratedForce = doubleOffsetForce,
                                DoubleLoadConcentratedMoment = doubleOffsetMoment
                            };
                            //Debug.WriteLine($"Home.CheckIfLoadsOnSupport(): loadConcentratedValuesOffset values for left load are: i={i}, Position={loadConcentratedValuesOffset.DoubleLoadConcentratedPosition}, Force={loadConcentratedValuesOffset.DoubleLoadConcentratedForce}, Moment={loadConcentratedValuesOffset.DoubleLoadConcentratedMoment}");
                            listLoadConcentratedValues.Insert(i, loadConcentratedValuesOffset);
                            // Create new load to add to right of support.
                            loadConcentratedValuesOffset = new LoadConcentratedValues
                            {
                                DoubleLoadConcentratedPosition = doubleOffsetPositionRight,
                                DoubleLoadConcentratedForce = doubleOffsetForce,
                                DoubleLoadConcentratedMoment = doubleOffsetMoment
                            };
                            //Debug.WriteLine($"Home.CheckIfLoadsOnSupport(): loadConcentratedValuesOffsetRight values for right load are: i+1={i+1}, Position={loadConcentratedValuesOffset.DoubleLoadConcentratedPosition}, Force={loadConcentratedValuesOffset.DoubleLoadConcentratedForce}, Moment={loadConcentratedValuesOffset.DoubleLoadConcentratedMoment}");
                            listLoadConcentratedValues.Insert(i + 1, loadConcentratedValuesOffset);
                            boolLoadAdded = true;
                            i++;    // Skip inserted load to right of support.
                            intLoadsConcentratedCount++;    // Increase count by one since inserted load to right of support.
                            break;      // Exit support foreach loop and move on to next load.
                        }
                    }
                    else
                    {
                        //Debug.WriteLine($"Home.CheckIfLoadsOnSupport(): Load at {listLoadConcentratedValues[i].DoubleLoadConcentratedPosition} is not on support at {supportValues.DoubleSupportPosition}.");
                    }
                }
            }
            // Sort listLoadConcentratedValuesCombined by DoubleLoadConcentratedPosition again since at least one load was added by above code block.
            if (boolLoadAdded)
            {
                listLoadConcentratedValues.Sort((x, y) => x.DoubleLoadConcentratedPosition.CompareTo(y.DoubleLoadConcentratedPosition));
            }
            //
            //Debug.WriteLine("\nHome.CheckIfLoadsOnSupport(): List of loads:");
            //foreach (LoadConcentratedValues loadConcentratedValues in listLoadConcentratedValues)
            //{
            //    Debug.WriteLine($"  Position={loadConcentratedValues.DoubleLoadConcentratedPosition}, Force={loadConcentratedValues.DoubleLoadConcentratedForce}, Moment={loadConcentratedValues.DoubleLoadConcentratedMoment}");
            //}
        }

        /// <summary>
        /// Check if any concentrated loads in listLoadConcentratedValues are at same position and combine them if so.
        /// </summary>
        /// <param name="listLoadConcentratedValues">Ref list to update that is defined by calling method.</param>
        private void CheckIfLoadsSamePosition(ref List<LoadConcentratedValues> listLoadConcentratedValues)
        {
            CommonItems.boolFoundLoadSamePosition = false;
            if (listLoadConcentratedValues.Count > 1)
            {
                //Debug.WriteLine("Home.CheckIfLoadsSamePosition(): Combine concentrated loads if at same position.");
                int intCountLoadsConcentrated = listLoadConcentratedValues.Count;
                int intNextItem;
                LoadConcentratedValues loadConcentratedValuesCombined;
                if (listLoadConcentratedValues.Count > 1)
                {
                    for (int i = 0; i < intCountLoadsConcentrated; i++)
                    {
                        intNextItem = i + 1;
                        if (intNextItem < intCountLoadsConcentrated)    // Check if intNextItem is not last item in listLoadConcentratedValuesCombined.
                        {
                            if (listLoadConcentratedValues[i].DoubleLoadConcentratedPosition == listLoadConcentratedValues[i + 1].DoubleLoadConcentratedPosition)
                            {
                                CommonItems.boolFoundLoadSamePosition = true;
                                //Debug.WriteLine($"Home.CheckIfLoadsSamePosition(): Found two concentrated loads with same position so combined them into single load.");
                                loadConcentratedValuesCombined = new LoadConcentratedValues
                                {
                                    DoubleLoadConcentratedPosition = listLoadConcentratedValues[i].DoubleLoadConcentratedPosition,
                                    DoubleLoadConcentratedForce = listLoadConcentratedValues[i].DoubleLoadConcentratedForce + listLoadConcentratedValues[intNextItem].DoubleLoadConcentratedForce,
                                    DoubleLoadConcentratedMoment = listLoadConcentratedValues[i].DoubleLoadConcentratedMoment + listLoadConcentratedValues[intNextItem].DoubleLoadConcentratedMoment
                                };
                                listLoadConcentratedValues.RemoveAt(intNextItem);
                                listLoadConcentratedValues.RemoveAt(i);
                                // Do not insert combined load if force and moment are both zero.
                                if (loadConcentratedValuesCombined.DoubleLoadConcentratedForce == 0d && loadConcentratedValuesCombined.DoubleLoadConcentratedMoment == 0d)
                                {
                                    // Reduce intCountLoadsConcentrated by two since two loads removed from list.
                                    intCountLoadsConcentrated--;
                                    intCountLoadsConcentrated--;
                                }
                                else
                                {
                                    listLoadConcentratedValues.Insert(i, loadConcentratedValuesCombined);    // Insert combined load into list since force or moment not zero.
                                    intCountLoadsConcentrated--;    // Reduce intCountLoadsConcentrated by one since two loads removed from list but one load added back.
                                }
                                i--;    // Reduce index by one since need to check if next load has same position.
                            }
                        }
                    }
                }
            }
            //
            //Debug.WriteLine($"\nHome.CheckIfLoadsSamePosition(): List of loads:");
            //foreach (LoadConcentratedValues loadConcentratedValues in listLoadConcentratedValues)
            //{
            //    Debug.WriteLine($"  Position={loadConcentratedValues.DoubleLoadConcentratedPosition}, Force={loadConcentratedValues.DoubleLoadConcentratedForce}, Moment={loadConcentratedValues.DoubleLoadConcentratedMoment}");
            //}
        }

        /// <summary>
        /// Check if any concentrated loads in listLoadConcentratedValues are close together and combine them if so.
        /// </summary>
        /// <param name="listLoadConcentratedValues">Ref list to update that is defined by calling method.</param>
        /// <returns></returns>
        private void CheckIfLoadsCloseTogether(ref List<LoadConcentratedValues> listLoadConcentratedValues)
        {
            CommonItems.boolFoundLoadCloseTogether = false;
            bool boolLoadsCloseTogether;
            int intCountLoadsConcentrated;
            do
            {
                boolLoadsCloseTogether = false;
                intCountLoadsConcentrated = listLoadConcentratedValues.Count;
                if (intCountLoadsConcentrated > 1)
                {
                    //Debug.WriteLine("Home.CheckIfLoadsCloseTogether(): Found more than one load.");
                    double doublePositionLeft;
                    double doublePositionRight;
                    double doublePositionDifference;
                    double doubleCombinedForce;
                    double doubleCombinedMoment;
                    double doubleCentroid;
                    int intNextItem;
                    for (int i = 0; i < intCountLoadsConcentrated; i++)
                    {
                        intNextItem = i + 1;
                        if (intNextItem < intCountLoadsConcentrated)    // Check if intNextItem is not last item in listLoadConcentratedValuesCombined.
                        {
                            doublePositionLeft = listLoadConcentratedValues[i].DoubleLoadConcentratedPosition;
                            doublePositionRight = listLoadConcentratedValues[intNextItem].DoubleLoadConcentratedPosition;
                            doublePositionDifference = doublePositionRight - doublePositionLeft;
                            // Need to check if load was on a support and then split by CheckIfLoadsOnSupport() called previously. If so then skip following code block.
                            //Debug.WriteLine($"Home.CheckIfLoadsCloseTogether(): doublePositionLeft={doublePositionLeft}, doublePositionRight={doublePositionRight}, doublePositionDifference={doublePositionDifference}, PostProcessor.doubleOffsetDistanceSupport*2d={PostProcessor.doubleOffsetDistanceSupport * 2d}");
                            if (!LibNum.EqualByRounding(doublePositionDifference, PostProcessor.doubleOffsetDistanceSupport * 2d, CommonItems.intRoundDigits))
                            {
                                if (doublePositionDifference < doubleCombineLoadDistance || LibNum.EqualByRounding(doublePositionDifference, doubleCombineLoadDistance, CommonItems.intRoundDigits))
                                {
                                    //Debug.WriteLine($"Home.CheckIfLoadsCloseTogether(): Found two concentrated loads close together so combined them into single load, doublePositionDifference={doublePositionDifference}, doubleCombineLoadDistance={doubleCombineLoadDistance}");
                                    CommonItems.boolFoundLoadCloseTogether = true;
                                    doubleCombinedForce = listLoadConcentratedValues[i].DoubleLoadConcentratedForce + listLoadConcentratedValues[intNextItem].DoubleLoadConcentratedForce;
                                    doubleCombinedMoment = listLoadConcentratedValues[i].DoubleLoadConcentratedMoment + listLoadConcentratedValues[intNextItem].DoubleLoadConcentratedMoment;
                                    // Test uniform loads. All values should cancele out.
                                    //Downward forces are negative.
                                    //Position left     Force left Position right    Force right
                                    //       0.0000      -200.0000       120.0000      -200.0000
                                    //       0.0000       200.0000       120.0000         0.0000
                                    //       0.0000         0.0000       120.0000       200.0000
                                    // Do not insert combined load if force and moment are both zero.
                                    if (LibNum.EqualByRounding(doubleCombinedForce, 0d, CommonItems.intRoundDigits) && LibNum.EqualByRounding(doubleCombinedMoment, 0d, CommonItems.intRoundDigits))
                                    {
                                        // Remove loads since combined force and moment are both zero.
                                        listLoadConcentratedValues.RemoveAt(intNextItem);
                                        listLoadConcentratedValues.RemoveAt(i);
                                        // Reduce intCountLoadsConcentrated by two since two loads removed from list.
                                        intCountLoadsConcentrated--;
                                        intCountLoadsConcentrated--;
                                        //Debug.WriteLine($"Home.CheckIfLoadsCloseTogether(): CASE=true: Removed two loads since combined force and moment are both zero.");
                                    }
                                    else
                                    {
                                        //Debug.WriteLine($"Home.CheckIfLoadsCloseTogether(): CASE=False: Combined force and moment are NOT both zero so continue.");
                                        // Calculate centroid position of loads to combine. NOTE: Next line does not consider moment values to calculate centroid position.
                                        // Calculated centroid position can be outside of load left and right position values versus in between them as expected.
                                        doubleCentroid = (doublePositionLeft * listLoadConcentratedValues[i].DoubleLoadConcentratedForce + doublePositionRight * listLoadConcentratedValues[intNextItem].DoubleLoadConcentratedForce) / doubleCombinedForce;
                                        //Debug.WriteLine($"Home.CheckIfLoadsCloseTogether(): Calculated doubleCentroid={doubleCentroid}, doublePositionLeft={doublePositionLeft}, doublePositionRight={doublePositionRight}, ForceRight={listLoadConcentratedValues[i].DoubleLoadConcentratedForce}, ForceLeft={listLoadConcentratedValues[intNextItem].DoubleLoadConcentratedForce}");
                                        LoadConcentratedValues loadConcentratedValues = new LoadConcentratedValues
                                        {
                                            DoubleLoadConcentratedPosition = doubleCentroid,
                                            DoubleLoadConcentratedForce = doubleCombinedForce,
                                            DoubleLoadConcentratedMoment = doubleCombinedMoment
                                        };
                                        listLoadConcentratedValues.RemoveAt(intNextItem);
                                        listLoadConcentratedValues.RemoveAt(i);
                                        listLoadConcentratedValues.Insert(i, loadConcentratedValues);    // Insert combined load into list.
                                        // Reduce intCountLoadsConcentrated by one since two loads removed from list but one load added back.
                                        intCountLoadsConcentrated--;    
                                        boolLoadsCloseTogether = true;  // Do loop one more time to verify inserted load not close to next load.
                                        //Debug.WriteLine($"Home.CheckIfLoadsCloseTogether(): Found two loads closed together so combined them.\ni={i}, boolLoadsCloseTogether={boolLoadsCloseTogether}, Position={loadConcentratedValues.DoubleLoadConcentratedPosition}, Force={loadConcentratedValues.DoubleLoadConcentratedForce}, Moment={loadConcentratedValues.DoubleLoadConcentratedMoment}");
                                    }
                                    i--;    // Reduce index by one since need to check if next load has same position.
                                }
                            }
                        }
                    }
                }
            } while (boolLoadsCloseTogether);
            //
            //Debug.WriteLine($"\nHome.CheckIfLoadsCloseTogether(): List of loads:");
            //foreach (LoadConcentratedValues loadConcentratedValues in listLoadConcentratedValues)
            //{
            //    Debug.WriteLine($"  Position={loadConcentratedValues.DoubleLoadConcentratedPosition}, Force={loadConcentratedValues.DoubleLoadConcentratedForce}, Moment={loadConcentratedValues.DoubleLoadConcentratedMoment}");
            //}
        }

        /// <summary>
        /// Consolidate concentrated loads and simulated loads created from uniform loads. 
        /// Offset loads slightly if they are on a support. Combine loads if at same position or are close together.
        /// </summary>
        private void ConsolidateLoadValues()
        {
            //Debug.WriteLine($"Home.ConsolidateLoadValues(): Method start.");
            List<LoadConcentratedValues> listLoadConcentratedValuesCombined = new List<LoadConcentratedValues> { };
            // Must process following four methods in sequence shown.
            ConsolidateConcentratedAndUniformLoads(ref listLoadConcentratedValuesCombined);
            CheckIfLoadsOnSupport(ref listLoadConcentratedValuesCombined);
            CheckIfLoadsSamePosition(ref listLoadConcentratedValuesCombined);
            CheckIfLoadsCloseTogether(ref listLoadConcentratedValuesCombined);
            // Clear CommonItems.listLoadConcentratedValuesConsolidated and copy new values to it for output.
            CommonItems.listLoadConcentratedValuesConsolidated.Clear();
            //Debug.WriteLine($"\nHome.ConsolidateLoadValues(): Added following loads to CommonItems.listLoadConcentratedValuesConsolidated:");
            foreach (LoadConcentratedValues loadConcentratedValues in listLoadConcentratedValuesCombined)
            {
                CommonItems.listLoadConcentratedValuesConsolidated.Add(loadConcentratedValues);
                //Debug.WriteLine($"  Position={loadConcentratedValues.DoubleLoadConcentratedPosition}, Force={loadConcentratedValues.DoubleLoadConcentratedForce}, Moment={loadConcentratedValues.DoubleLoadConcentratedMoment}");
            }
        }

        /// <summary>
        /// Check support values. Return empty string if supports are valid, otherwise return error message.
        /// If single support found, check displacement and rotation at support are both false, otherwise unstable beam.
        /// </summary>
        private string CheckSupportValues()
        {
            //Debug.WriteLine($"CheckSupportValues(): CommonItems.boolEnteredSupport={CommonItems.boolEnteredSupport}, CommonItems.listSupportValues.Count={CommonItems.listSupportValues.Count}");
            string stringErrorMsg = string.Empty;
            if (CommonItems.boolEnteredSupport)
            {
                if (CommonItems.listSupportValues.Count == 1)
                {
                    SupportValues supportValues = CommonItems.listSupportValues[0];
                    if (supportValues.BoolSupportDisplacement || supportValues.BoolSupportRotation)
                    {
                        // Do not change value of CommonItems.boolEnteredSupport to false here.
                        stringErrorMsg = "Single support entered but is invalid since displacement and/or rotation enabled, ";
                        //Debug.WriteLine($"CheckSupportValues(): {stringErrorMsg}");
                    }
                }
            }
            else 
            {
                stringErrorMsg = "Supports, ";
            }
            return stringErrorMsg;
        }

        /// <summary>
        /// Check if required items to calculate beam results have been entered and are valid. Return true if so, false othwerwise.
        /// This method also toggles color of ButHomeCalculate.
        /// </summary>
        /// <returns></returns>
        private bool CalculateCheck()
        {
            bool boolCalculate = true;      // Bool is true unless invalid input value found below.

            if (!CommonItems.boolEnteredYoungsModulus)
                boolCalculate = false;
            else if (!CommonItems.boolEnteredPoissonsRatio)
                boolCalculate = false;
            else if (!CommonItems.boolEnteredInertia)
                boolCalculate = false;
            else if (!CommonItems.boolEnteredBeamLength)
                boolCalculate = false;
            else if (!CommonItems.boolEnteredOutputSegmentLength)
                boolCalculate = false;
            else if (!boolEnteredCombineLoadDistance)
                boolCalculate = false;
            else if (!(CommonItems.boolEnteredLoadConcentrated || CommonItems.boolEnteredLoadUniform))
                boolCalculate = false;
            else
            {
                // Check support values.
                string stringErrorMsg = CheckSupportValues();
                if (stringErrorMsg.Length > 0)
                    boolCalculate = false;  // Error found.
            }
            //Debug.WriteLine($"Home.CalculateCheck(): boolCalculate={boolCalculate}");
            if (boolCalculate)
            {
                LibMPC.OutputMsgSuccess(TblkHomeCalculate, "All required items entered so can now calculate results.\nTime required to calculate results is proportional to values entered.");
                CommonItems.ButtonForegroundColorSet(ButHomeCalculate, true);
            }
            else
            {
                TblkHomeCalculate.Text = string.Empty;
                CommonItems.ButtonForegroundColorSet(ButHomeCalculate, false);
            }
            return boolCalculate;
        }

        /// <summary>
        /// Check and show status of required input items on Home page.
        /// </summary>
        private void CheckHomeInputValues()
        {
            if (CalculateCheck())
            {
                LibMPC.OutputMsgSuccess(TblkHomeStatus, $"Beam length = {CommonItems.doubleBeamLength.ToString(LibNum.fpNumericFormatNone)} {CommonItems.stringConstUnitsLength}.\nRequired items to calculate beam results have been entered and are valid:");
            }
            else
            {
                string stringHeader = "Following items not entered or not valid:\n";
                string stringErrorMsg;
                if (CommonItems.boolEnteredBeamLength)
                    stringErrorMsg = $"{CommonItems.stringConstBeamLength} = {CommonItems.doubleBeamLength} {CommonItems.stringConstUnitsLength}\n{stringHeader}";
                else
                    stringErrorMsg = stringHeader;
                if (!CommonItems.boolEnteredYoungsModulus)
                    stringErrorMsg += $"{CommonItems.stringConstYoungsModulus}, ";
                if (!CommonItems.boolEnteredPoissonsRatio)
                    stringErrorMsg += $"{CommonItems.stringConstPoissonsRatio}, ";
                if (!CommonItems.boolEnteredInertia)
                    stringErrorMsg += $"{CommonItems.stringConstInertia}, ";
                if (!CommonItems.boolEnteredBeamLength)
                    stringErrorMsg += $"{CommonItems.stringConstBeamLength}, ";
                // Check support values.
                stringErrorMsg += CheckSupportValues();
                if (!CommonItems.boolEnteredOutputSegmentLength)
                    stringErrorMsg += $"{CommonItems.stringConstOutputSegmentLength}, ";
                if (!boolEnteredCombineLoadDistance)
                    stringErrorMsg += $"{CommonItems.stringConstCombineLoadDistance}, ";
                if (!(CommonItems.boolEnteredLoadConcentrated || CommonItems.boolEnteredLoadUniform))
                    stringErrorMsg += "Loads, ";
                stringErrorMsg += "\nEnter or correct above items to calculate beam results.";
                LibMPC.OutputMsgError(TblkHomeStatus, stringErrorMsg);
            }
            // Display supports, concentrated loads, and uniform loads, if any.
            LibMPC.OutputMsgSuccess(TblkHomeDisplay, CommonItems.ShowEnteredSupportsAndLoads());
        }

        /// <summary>
        /// Enable or disable page items depending on value of parameter boolEnable. Method used when calculating beam results.
        /// </summary>
        /// <param name="boolEnable">Enable page items if true, otherwise disable page items.</param>
        private void EnablePageItems(bool boolEnable)
        {
            if (boolEnable)
            {
                mainPage.mainPageButAbout.IsEnabled = true;
                mainPage.mainPageButSamples.IsEnabled = true;
                ButHomeProperties.IsEnabled = true;
                ButHomeSupports.IsEnabled = true;
                TboxHomeOutputSegmentLength.IsEnabled = true;
                TboxHomeCombineLoadDistance.IsEnabled = true;
                ButHomeConcentratedLoads.IsEnabled = true;
                ButHomeUniformLoads.IsEnabled = true;
                TogHomeMatricesShow.IsEnabled = true;
                ButHomeCalculate.IsEnabled = true;
                ButHomePurchaseApp.IsEnabled = true;
                ButHomeRateApp.IsEnabled = true;
            }
            else
            {
                mainPage.mainPageButAbout.IsEnabled = false;
                mainPage.mainPageButSamples.IsEnabled = false;
                ButHomeProperties.IsEnabled = false;
                ButHomeSupports.IsEnabled = false;
                TboxHomeOutputSegmentLength.IsEnabled = false;
                TboxHomeCombineLoadDistance.IsEnabled = false;
                ButHomeConcentratedLoads.IsEnabled = false;
                ButHomeUniformLoads.IsEnabled = false;
                TogHomeMatricesShow.IsEnabled = false;
                ButHomeCalculate.IsEnabled = false;
                ButHomePurchaseApp.IsEnabled = false;
                ButHomeRateApp.IsEnabled = false;
            }
        }

        /// <summary>
        /// Get purchase status of application. Method controls visibility/Enable of PBarStatus, TblkPurchaseApp, and ButPurchaseApp.
        /// </summary>
        private async Task AppPurchaseCheck()
        {
            if (CommonItems.boolAppPurchased)
            {
                // App has been purchased so hide following values and return.
                PBarHomeStatus.Visibility = Visibility.Collapsed;
                TblkHomePurchaseApp.Visibility = Visibility.Collapsed;
                LibMPC.ButtonVisibility(ButHomePurchaseApp, false);
            }
            else
            {
                if (CommonItems.boolPurchaseCheckCompleted)
                {
                    // App has not been purchased but purchase check done so show previous message. This occurs if User returning from another page.
                    PBarHomeStatus.Visibility = Visibility.Collapsed;
                    LibMPC.OutputMsgError(TblkHomePurchaseApp, CommonItems.stringPurchaseCheckOutput);
                    TblkHomePurchaseApp.Visibility = Visibility.Visible;
                    LibMPC.ButtonVisibility(ButHomePurchaseApp, true);
                }
                else
                {
                    // App has not been purchased so do purchase check.
                    LibMPC.OutputMsgBright(TblkHomePurchaseApp, "Application purchase check in progress...");
                    PBarHomeStatus.Foreground = LibMPC.colorError;          // Set color PBarStatus from default.
                    PBarHomeStatus.Visibility = Visibility.Visible;
                    PBarHomeStatus.IsIndeterminate = true;
                    EnablePageItems(false);
                    CommonItems.boolAppPurchased = await LibMPC.AppPurchaseStatusAsync(mainPage.applicationDataContainer, CommonItems.ds_BoolAppPurchased);
                    if (CommonItems.boolAppPurchased)
                    {
                        // App purchased.
                        LibMPC.OutputMsgSuccess(TblkHomePurchaseApp, LibMPC.stringAppPurchaseResult);
                        LibMPC.ButtonVisibility(ButHomePurchaseApp, false);
                    }
                    else
                    {
                        // App not purchased.
                        LibMPC.OutputMsgError(TblkHomePurchaseApp, LibMPC.stringAppPurchaseResult);
                        LibMPC.ButtonVisibility(ButHomePurchaseApp, true);
                    }
                    PBarHomeStatus.IsIndeterminate = false;
                    PBarHomeStatus.Visibility = Visibility.Collapsed;
                    CommonItems.boolPurchaseCheckCompleted = true;
                    CommonItems.stringPurchaseCheckOutput = TblkHomePurchaseApp.Text;
                    EnablePageItems(true);
                }
            }
        }

        /// <summary>
        /// Attempt to buy application. Method controls visibility/Enable of PBarHomeStatus, TblkHomePurchaseApp, and ButHomePurchaseApp.
        /// </summary>
        private async Task AppPurchaseBuy()
        {
            LibMPC.OutputMsgNormal(TblkHomePurchaseApp, "Attempting to purchase application...");
            EnablePageItems(false);
            PBarHomeStatus.Foreground = LibMPC.colorError;          // Set color PBarStatus from default.
            PBarHomeStatus.Visibility = Visibility.Visible;
            PBarHomeStatus.IsIndeterminate = true;
            CommonItems.boolAppPurchased = await LibMPC.AppPurchaseBuyAsync(mainPage.applicationDataContainer, CommonItems.ds_BoolAppPurchased);
            if (CommonItems.boolAppPurchased)
            {
                // App purchased.
                LibMPC.OutputMsgSuccess(TblkHomePurchaseApp, LibMPC.stringAppPurchaseResult);
                LibMPC.ButtonVisibility(ButHomePurchaseApp, false);
            }
            else
            {
                // App not purchased.
                LibMPC.OutputMsgError(TblkHomePurchaseApp, LibMPC.stringAppPurchaseResult);
                LibMPC.ButtonVisibility(ButHomePurchaseApp, true);
            }
            PBarHomeStatus.IsIndeterminate = false;
            PBarHomeStatus.Visibility = Visibility.Collapsed;
            EnablePageItems(true);
        }

        /// <summary>
        /// If application has not been rated then show ButRateApp occasionally.
        /// </summary>
        private void AppRatedCheck()
        {
            LibMPC.ButtonVisibility(ButHomeRateApp, false);
            if (!CommonItems.boolAppRated)
            {
                if (mainPage.applicationDataContainer.Values.ContainsKey(CommonItems.ds_IntAppRatedCounter))
                {
                    int intAppRatedCounter = (int)mainPage.applicationDataContainer.Values[CommonItems.ds_IntAppRatedCounter];
                    intAppRatedCounter++;
                    if (intAppRatedCounter >= CommonItems.intShowButRateApp)
                    {
                        mainPage.applicationDataContainer.Values[CommonItems.ds_IntAppRatedCounter] = 0;     // Reset data store setting to 0.
                        ButHomeRateApp.Foreground = LibMPC.colorSuccess;
                        LibMPC.ButtonVisibility(ButHomeRateApp, true);
                        //Debug.WriteLine($"AppRatedCheck(): CommonItems.ds_IntAppRatedCounter set to {(int)mainPage.applicationDataContainer.Values[CommonItems.ds_IntAppRatedCounter]}");
                    }
                    else
                    {
                        mainPage.applicationDataContainer.Values[CommonItems.ds_IntAppRatedCounter] = intAppRatedCounter;     // Update data store setting to intAppRatedCounter.
                        //Debug.WriteLine($"AppRatedCheck(): CommonItems.ds_IntAppRatedCounter updated to {(int)mainPage.applicationDataContainer.Values[CommonItems.ds_IntAppRatedCounter]}");
                    }
                }
                else
                {
                    mainPage.applicationDataContainer.Values[CommonItems.ds_IntAppRatedCounter] = 1;     // Initialize data store setting to 1.
                    //Debug.WriteLine($"AppRatedCheck(): CommonItems.ds_IntAppRatedCounter not found so set value to {(int)mainPage.applicationDataContainer.Values[CommonItems.ds_IntAppRatedCounter]}");
                }
            }
        }

        /*** Home entry events follow ******************************************************************************************/

        // Next 3 methods handle User entry in TboxHomeOutputSegmentLength.

        /// <summary>
        /// Do following when User presses Enter key in TboxHomeOutputSegmentLength.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxHomeOutputSegmentLength_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed. Ignore everything else.
            {
                TboxHomeOutputSegmentLength_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxHomeOutputSegmentLength focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxHomeOutputSegmentLength_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxHomeOutputSegmentLength_LostFocus(): Event fired.");
            double doubleOutputSegmentLengthEntered = LibNum.TextBoxGetDouble(TboxHomeOutputSegmentLength, EnumTextBoxUpdate.Yes);      // Get the input from the TextBox and convert to matching numeric.
            if (doubleOutputSegmentLengthEntered >= doubleOutputSegmentLengthMinimum && doubleOutputSegmentLengthEntered <= CommonItems.doubleBeamLength)
            {
                string stringMessage = $"{CommonItems.stringConstOutputSegmentLength} valid.";  // Default message.
                //Debug.WriteLine($"TboxHomeOutputSegmentLength_LostFocus(): Entered valid doubleOutputSegmentLengthEntered={doubleOutputSegmentLengthEntered}");
                if (doubleOutputSegmentLengthEntered != CommonItems.doubleOutputSegmentLength)
                {
                    // New value of CommonItems.doubleOutputSegmentLength is valid and has changed.
                    CommonItems.doubleOutputSegmentLength = doubleOutputSegmentLengthEntered;
                    //Debug.WriteLine($"TboxHomeOutputSegmentLength_LostFocus(): Entered new value for doubleOutputSegmentLength={CommonItems.doubleOutputSegmentLength}");
                    // Update simulated loads if any unform loads have been entered.
                    if (CommonItems.listLoadUniformValues.Count > 0)
                    {
                        CommonItems.RecalculateSimulatedLoads();
                        //Debug.WriteLine($"TboxHomeOutputSegmentLength_LostFocus(): Recalculated simulated loads of uniform loads.");
                        stringMessage = $"{CommonItems.stringConstOutputSegmentLength} valid. Recalculated simulated loads of uniform loads.";  // Overwrite default message.
                    }
                }
                CommonItems.boolEnteredOutputSegmentLength = true;
                LibMPC.OutputMsgSuccess(TblkHomeLostFocus, stringMessage);
            }
            else
            {
                CommonItems.doubleOutputSegmentLength = 0d;
                CommonItems.boolEnteredOutputSegmentLength = false;
                LibMPC.OutputMsgError(TblkHomeLostFocus, $"{CommonItems.stringConstOutputSegmentLength} not valid. Value must be in range of {doubleOutputSegmentLengthMinimum.ToString(LibNum.fpNumericFormatNone)} to {CommonItems.doubleBeamLength.ToString(LibNum.fpNumericFormatNone)}.");
            }
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoubleOutputSegmentLength] = CommonItems.doubleOutputSegmentLength;
            if (CommonItems.doubleOutputSegmentLength == 0d)
                TboxHomeOutputSegmentLength.Text = string.Empty;
            else
                TboxHomeOutputSegmentLength.Text = CommonItems.doubleOutputSegmentLength.ToString(LibNum.fpNumericFormatNone);
            // Check doubleCombineLoadDistance is less than CommonItems.doubleOutputSegmentLength.
            if (doubleCombineLoadDistance >= CommonItems.doubleOutputSegmentLength)
            {
                doubleCombineLoadDistance = 0d;
                boolEnteredCombineLoadDistance = false;
                TboxHomeCombineLoadDistance.Text = string.Empty;     // Set default value.
                mainPage.applicationDataContainer.Values[CommonItems.ds_DoubleCombineLoadDistance] = doubleCombineLoadDistance;
                Debug.WriteLine($"TboxHomeOutputSegmentLength_LostFocus(): doubleCombineLoadDistance >= CommonItems.doubleOutputSegmentLength, so set to zero");
            }
            CheckHomeInputValues();
            SetButtonForegroundColors();
        }

        /// <summary>
        /// On TboxHomeOutputSegmentLength_TextChanged() event, verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxHomeOutputSegmentLength_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxHomeOutputSegmentLength_TextChanged(): Event fired.");
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxHomeOutputSegmentLength, EnumNumericType._double);
        }

        // Next 3 methods handle User entry in TboxHomeCombineLoadDistance.

        /// <summary>
        /// Do following when User presses Enter key in TboxHomeCombineLoadDistance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxHomeCombineLoadDistance_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            if (e.Key == Windows.System.VirtualKey.Enter)           // Check if 'Enter' key was pressed. Ignore everything else.
            {
                TboxHomeCombineLoadDistance_LostFocus(null, null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Do following when TboxHomeCombineLoadDistance focus changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxHomeCombineLoadDistance_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxHomeCombineLoadDistance_LostFocus(): Event fired.");
            doubleCombineLoadDistance = LibNum.TextBoxGetDouble(TboxHomeCombineLoadDistance, EnumTextBoxUpdate.Yes);      // Get the input from the TextBox and convert to matching numeric.
            if (doubleCombineLoadDistance >= doubleCombineLoadDistanceMinimum && doubleCombineLoadDistance <= doubleCombineLoadDistanceMaximum && doubleCombineLoadDistance < CommonItems.doubleOutputSegmentLength)
            {
                boolEnteredCombineLoadDistance = true;
                LibMPC.OutputMsgSuccess(TblkHomeLostFocus, $"{CommonItems.stringConstCombineLoadDistance} valid.");
            }
            else
            {
                if (doubleCombineLoadDistance >= CommonItems.doubleOutputSegmentLength)
                    LibMPC.OutputMsgError(TblkHomeLostFocus, $"{CommonItems.stringConstCombineLoadDistance} not valid since equal or greater than {CommonItems.stringConstOutputSegmentLength}.");
                else
                    LibMPC.OutputMsgError(TblkHomeLostFocus, $"{CommonItems.stringConstCombineLoadDistance} not valid. Value must be in range of {doubleCombineLoadDistanceMinimum.ToString(LibNum.fpNumericFormatNone)} to {doubleCombineLoadDistanceMaximum.ToString(LibNum.fpNumericFormatNone)}.");
                doubleCombineLoadDistance = 0d;
                boolEnteredCombineLoadDistance = false;
            }
            mainPage.applicationDataContainer.Values[CommonItems.ds_DoubleCombineLoadDistance] = doubleCombineLoadDistance;
            if (doubleCombineLoadDistance == 0d)
                TboxHomeCombineLoadDistance.Text = string.Empty;
            else
                TboxHomeCombineLoadDistance.Text = doubleCombineLoadDistance.ToString(LibNum.fpNumericFormatNone);
            //Debug.WriteLine($"TboxHomeCombineLoadDistance_LostFocus(): doubleCombineLoadDistance={doubleCombineLoadDistance}, TboxHomeCombineLoadDistance.Text={TboxHomeCombineLoadDistance.Text}.");
            CheckHomeInputValues();
            SetButtonForegroundColors();
        }

        /// <summary>
        /// On TboxHomeCombineLoadDistance_TextChanged() event, verify character entered still results in a valid numeric, otherwise discard last character entered.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TboxHomeCombineLoadDistance_TextChanged(object sender, TextChangedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"TboxHomeCombineLoadDistance_TextChanged(): Event fired.");
            // Verify character entered still results in a valid double, otherwise discard last character entered.
            LibNum.NumericTextBoxTextChanged(TboxHomeCombineLoadDistance, EnumNumericType._double);
        }

        // Next 2 methods handle User entry in TogHomeMatricesShow.

        /// <summary>
        /// Toggle Display Matrices mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogHomeMatricesShow_Toggled(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            TogHomeMatricesShow_LostFocus(null, null);
        }

        /// <summary>
        /// Toggle display of matrices in output.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogHomeMatricesShow_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            if (TogHomeMatricesShow.IsOn == true)
            {
                CommonItems.boolMatricesShow = true;
                LibMPC.OutputMsgSuccess(TblkHomeLostFocus, "Show matrices in output.");
            }
            else
            {
                CommonItems.boolMatricesShow = false;
                LibMPC.OutputMsgSuccess(TblkHomeLostFocus, "Do not show matrices in output.");
            }
            mainPage.applicationDataContainer.Values[CommonItems.ds_BoolMatricesShow] = CommonItems.boolMatricesShow; // Write setting to data store.
            CheckHomeInputValues();
            SetButtonForegroundColors();
        }

        /// <summary>
        /// Navigate to page that allows User to enter various beam properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButHomeEnterBeamProperties_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButHomeEnterBeamProperties_Click(): Event fired.");
            // Beam properties page is always available to User.
            mainPage.ShowPageEnterBeamProperties();
        }

        /// <summary>
        /// Navigate to page that allows User to enter various supports on beam.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButHomeEnterSupports_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButHomeEnterSupports_Click():  Event fired!");
            bool boolInvalidItemFound = false;      // Bool is false unless changed below.
            List<string> listStringErrors = new List<string>();
            if (!CommonItems.boolEnteredBeamLength)
            {
                boolInvalidItemFound = true;
                listStringErrors.Add($"{CommonItems.stringConstBeamLength} not entered or not valid.");
            }
            if (boolInvalidItemFound)
            {
                listStringErrors.Add("Cannot add supports.");
                LibMPC.OutputMsgError(TblkHomeLostFocus, LibMPC.JoinListString(listStringErrors, EnumStringSeparator.TwoSpaces));
            }
            else
            {
                //Debug.WriteLine($"ButHomeEnterSupports_Click(): DebugMsg02: boolInvalidItemFound={boolInvalidItemFound}, Did not find errors so add supports");
                mainPage.ShowPageEnterSupports();
            }
        }

        /// <summary>
        /// Navigate to page that allows User to enter various concentrated loads on beam.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButHomeEnterLoadsConcentrated_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButHomeEnterLoadsConcentrated_Click(): Event fired.");
            bool boolInvalidItemFound = false;      // Bool is false unless changed below.
            List<string> listStringErrors = new List<string>();
            if (!CommonItems.boolEnteredBeamLength)
            {
                boolInvalidItemFound = true;
                listStringErrors.Add($"{CommonItems.stringConstBeamLength} not entered or not valid.");
            }
            if (!CommonItems.boolEnteredSupport)
            {
                boolInvalidItemFound = true;
                listStringErrors.Add("Supports not entered.");
            }
            if (boolInvalidItemFound)
            {
                listStringErrors.Add("Cannot add concentrated loads.");
                LibMPC.OutputMsgError(TblkHomeLostFocus, LibMPC.JoinListString(listStringErrors, EnumStringSeparator.TwoSpaces));
                //Debug.WriteLine($"ButHomeEnterLoadsConcentrated_Click(): {LibMPC.JoinListString(listStringErrors, EnumStringSeparator.TwoSpaces)}");
            }
            else
            {
                //Debug.WriteLine($"ButHomeEnterLoadsConcentrated_Click(): Did not find any input errors so navigate to page EnterLoadsConcentrated.");
                mainPage.ShowPageEnterLoadsConcentrated();
            }
        }

        /// <summary>
        /// Navigate to page that allows User to enter various uniform loads on beam.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButHomeEnterLoadsUniform_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButHomeEnterLoadsUniform_Click(): Event fired.");
            bool boolInvalidItemFound = false;      // Bool is false unless changed below.
            List<string> listStringErrors = new List<string>();
            if (!CommonItems.boolEnteredBeamLength)
            {
                boolInvalidItemFound = true;
                listStringErrors.Add($"{CommonItems.stringConstBeamLength} not entered or not valid.");
            }
            if (!CommonItems.boolEnteredSupport)
            {
                boolInvalidItemFound = true;
                listStringErrors.Add("Supports not entered.");
            }
            if (boolInvalidItemFound)
            {
                listStringErrors.Add("Cannot add uniform loads.");
                LibMPC.OutputMsgError(TblkHomeLostFocus, LibMPC.JoinListString(listStringErrors, EnumStringSeparator.TwoSpaces));
                //Debug.WriteLine($"ButHomeEnterLoadsUniform_Click(): {LibMPC.JoinListString(listStringErrors, EnumStringSeparator.TwoSpaces)}");
            }
            else
            {
                //Debug.WriteLine($"ButHomeEnterLoadsUniform_Click(): Did not find any input errors so navigated to page EnterLoadsUniform.");
                mainPage.ShowPageEnterLoadsUniform();
            }
        }

        /// <summary>
        /// Calculate beam results if all required entry items entered and valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButHomeCalculate_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButHomeCalculate_Click(): Event fired.");
            if (CalculateCheck())
            {
                LibMPC.OutputMsgSuccess(TblkHomeCalculate, "Calculating beam results...");
                EnablePageItems(false);
                ConsolidateLoadValues();
                PBarHomeStatus.Visibility = Visibility.Visible;
                PBarHomeStatus.IsIndeterminate = true;
                await Task.Run(() => CalculateResults.CalculateBeamResults());
                PBarHomeStatus.IsIndeterminate = false;
                PBarHomeStatus.Visibility = Visibility.Collapsed;
                mainPage.ShowPageDisplayResults();
                EnablePageItems(true);
            } 
            else
            {
                LibMPC.OutputMsgError(TblkHomeLostFocus, "Cannot calculate results since required items not entered or not valid.");
            }
        }

        /// <summary>
        /// Common LostFocus event for buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButHome_LostFocus(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine("ButSupport_LostFocus() event fired.");
            TblkHomeLostFocus.Text = string.Empty;
        }

        /// <summary>
        /// Purchase application button. Button visible if application has not been purchased, collapsed otherwise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButHomePurchaseApp_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            await AppPurchaseBuy();
        }

        /// <summary>
        /// Invoked when user clicks hyperlink button ButRateApp. MS Store popup box will lock out all access to App.
        /// Therefore no reason to hide buttons on page. Do not show ButRateApp link again after User rates App.
        /// Goal is to get more App ratings in Microsoft Store without hassling User too much.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButHomeRateApp_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            if (await mainPage.RateAppInW10StoreAsync())
                LibMPC.ButtonVisibility(ButHomeRateApp, false);
        }

    }
}
