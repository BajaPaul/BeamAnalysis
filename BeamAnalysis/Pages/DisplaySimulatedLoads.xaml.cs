using BeamAnalysis.Common;
using LibraryCoder.MainPageCommon;
using Windows.UI.Xaml.Controls;

namespace BeamAnalysis.Pages
{
    public sealed partial class DisplaySimulatedLoads : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        /// <summary>
        /// Page that displays User entered uniform loads and resulting concentrated loads used to simulate the uniform load. Output is formatted text string.
        /// </summary>
        public DisplaySimulatedLoads()
        {
            InitializeComponent();
            // Set MainPage Button Visibility.
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, true);    // Show back button on this page since nothing done before returning to previous page.
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButSamples, false);
            // Overwrite XAML TextBlock Foreground colors.
            TblkDisplaySimulatedLoadsNote.Foreground = LibMPC.colorBright;
            TblkDisplaySimulatedLoadsOutput.Foreground = LibMPC.colorNormal;
            // Overwrite XAML default values.
            TblkDisplaySimulatedLoadsNote.Text = $"Display of User entered uniform loads and resulting concentrated loads used to simulate the uniform load.  App simulates uniform loads with series of concentrated loads.  Uniform loads are split into segments of equal length.  Then area and centroid for each segment is calculated.  Then an equivalent concentrated load is applied to the centroid position of the segment.  Output results can be refined by using smaller segments at expense of computer processing time and more output to view.  {CommonItems.stringConstLoadForce}.";
            TblkDisplaySimulatedLoadsOutput.Text = CommonItems.stringOutputResults;
            // Clear CommonItems.stringOutputResults since no longer needed.
            CommonItems.stringOutputResults = string.Empty;
            // Setup scrolling for this page.
            LibMPC.ScrollViewerOn(mainPage.mainPageScrollViewer, horz: ScrollMode.Disabled, vert: ScrollMode.Auto, horzVis: ScrollBarVisibility.Disabled, vertVis: ScrollBarVisibility.Auto, zoom: ZoomMode.Disabled);
        }

    }
}
