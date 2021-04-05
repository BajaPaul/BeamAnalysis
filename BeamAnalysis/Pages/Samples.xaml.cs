using BeamAnalysis.Common;
using LibraryCoder.MainPageCommon;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BeamAnalysis.Pages
{
    public sealed partial class Samples : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        private const string stringProperties01 = "Following samples confirm input values using above USC or SI unit combinations output same deflection value.  Sample loads output deflection slightly less than L/200.";
        private const string stringProperties02 = "\n\nBeam properties for following samples are:";
        private const string stringProperties03 = "\nMaterial description: ASTM-A36 Steel.";
        private const string stringProperties04 = "\nYoung's modulus: 29,000,000 lb/in² (psi) = 4,176,000,000 lb/ft² (psf) = 199,947,961,501.883 Pa";
        private const string stringProperties05 = "\nPoisson's ratio: 0.26";
        private const string stringProperties06 = "\nCross section description: W10x30.";
        private const string stringProperties07 = "\nVertical cross section inertia: 170 in⁴ = 0.0081983024691358 ft⁴ = 0.000070759342352 m⁴";
        private const string stringProperties08 = "\nBeam length: 120 in = 10 ft = 3.048 m, equivalent beam midpoints are 60 in = 5 ft = 1.524 m. 120 inches/200 = 0.6 inches";

        private const string stringSample1_1 = "\nSample #1: Beam with simple supports at each end and single concentrated load at midpoint.\nDisplacement at supports = Off, Rotation at supports = On.\nConcentrated Load: -80,000 lb = -355,857.72922084 N.";
        private const string stringSample1_2 = "\n\nUSC inch results using above values: Output segment length = 1.\nReaction left and right 40,000 lb.  Maximum moment and deflection occurs at 60.0 inches.  Moment = 2,399,960 lb.in.  Deflection = -0.584178 inches.";
        private const string stringSample1_3 = "\n\nUSC foot results using above values: Output segment length = 0.083333333333333.\nReaction left and right 40,000 lb.  Maximum moment and deflection occurs at 5.0 feet.  Moment = 199,960 lb.ft = 2,399,520 lb.in.  Deflection = -0.048682 feet = -0.584184 inches.";
        private const string stringSample1_4 = "\n\nSI meter results using above values: Output segment length = 0.0254.\nReaction left and right 177928.9 N = 40,000 lb.  Maximum moment and deflection occurs at 1.524 meters.  Moment = 270,985.7 N.m = 2,398,425.54378485 lb.in.  Deflection = -0.014838 meters = -0.584173228346457 inches.";
        private const string stringSample1_5 = "\n\nAISC formula results for USC inch units:  Reaction left and right 40,000 lb.  Moment = 2,400,000 lb.in.  Deflection = -0.584178498985801 inches.";

        private const string stringSample2_1 = "\nSample #2: Beam with simple supports at each end and single concentrated moment at midpoint.\nDisplacement at supports = Off, Rotation at supports = On.\nConcentrated Moment: -25,000,000 lb.in = -2,083,333.33333333 ft.lb = -2,824,620.72569042 N.m";
        private const string stringSample2_2 = "\n\nUSC inch results using above values:  Reaction left and right -/+ 208,333.3 lb.  Maximum moment occurs at 60.0 inches and is 12,500,000.0 lb.in.  Approximate maximum deflection occurs at 34.6563 inches and is 0.585548 inches.";
        private const string stringSample2_3 = "\n\nUSC foot results using above values:  Reaction left and right -/+ 208,333.3 lb.  Maximum moment occurs at 5.0 feet and is 1,041,666.66666667 lb.ft = 12,500,000 lb.in.  Approximate maximum deflection occurs at 2.8750 feet = 34.5 inches and is 0.048794 feet = 0.585528 inches.";
        private const string stringSample2_4 = "\n\nSI meter results using above values:  Reaction left and right -/+ 926,712.8 N = 208,333.3 lb.  Maximum moment occurs at 1.524 m and is 1,412,310.36284521 N.m = 12,500,000 lb.in.  Approximate maximum deflection occurs at 0.8709 m = 34.29 inches and is 0.014871 meters = 0.585472 inches.";

        private const string stringSample3_1 = "\nSample #3: Beam with simple supports at each end and rectangular uniform load across beam length.\nDisplacement at supports = Off, Rotation at supports = On.\nUniform Load: -1,000 lb/in = -12,000 lb/ft = -175,126.835246476 N/m.";
        private const string stringSample3_2 = "\n\nUSC inch results using above values: Uniform load segment length = 1.\nReaction left and right 60,000 lb.  Maximum moment and deflection occurs at 60.0 inches.  Moment = 1,800,000 lb.in.  Deflection = -0.547683 inches.";
        private const string stringSample3_3 = "\n\nUSC foot results using above values: Uniform load segment length = 0.083333333333333.\nReaction left and right 60,000 lb.  Maximum moment and deflection occurs at 5.0 feet.  Moment = 150,000 lb.ft = 1,800,000 lb.in.  Deflection = -0.045629 feet = -0.547548 inches.";
        private const string stringSample3_4 = "\n\nSI meter results using above values: Uniform load segment length = 0.0254.\nReaction left and right 266893.3 N = 60,000 lb.  Maximum moment and deflection occurs at 1.524 meters.  Moment = 203372.7 N.m = 1,800,000 lb.in.  Deflection = -0.013910 meters = -0.547637795275591 inches.";
        private const string stringSample3_5 = "\n\nAISC formula results for USC inch units:  Reaction left and right 60,000 lb.  Moment = 1,800,000 lb.in.  Deflection = -0.547667342799189 inches.";

        private const string stringTitlePUC = "Precision Unit Converter";
        private readonly string stringInfoPUC = $"Click following button to install '{stringTitlePUC}' application created by developer to assist with conversions used in this application.";

        /// <summary>
        /// Page that shows various samples and results using consistent USC or SI unit combinations.
        /// </summary>
        public Samples()
        {
            InitializeComponent();
            // Hide XAML layout rectangles by setting their color to RelativePanel Background color.
            RectLayoutCenter.Fill = Rpanel.Background;
            RectLayoutLeft.Fill = Rpanel.Background;
            RectLayoutRight.Fill = Rpanel.Background;
            // Set MainPage Button Visibility.
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButSamples, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, true);
            // Overwrite XAML TextBlock Foreground colors.
            TblkSamplesNotes.Foreground = LibMPC.colorBright;
            TblkSamplesProperties.Foreground = LibMPC.colorNormal;
            TblkSamples1.Foreground = LibMPC.colorSuccess;
            TblkSamples2.Foreground = LibMPC.colorError;
            TblkSamples3.Foreground = LibMPC.colorSuccess;
            TblkInfoPUC.Foreground = LibMPC.colorNormal;
            // Overwrite XAML default values.
            TblkSamplesNotes.Text = $"{CommonItems.stringConstApplicationNote}\n\n{CommonItems.stringConstUnitsUSCInch}\n{CommonItems.stringConstUnitsUSCFoot}\n{CommonItems.stringConstUnitsSI}";
            TblkSamplesProperties.Text = stringProperties01 + stringProperties02 + stringProperties03 + stringProperties04 + stringProperties05 + stringProperties06 + stringProperties07 + stringProperties08;

            TblkSamples1.Text = stringSample1_1 + stringSample1_2 + stringSample1_3 + stringSample1_4 + stringSample1_5;
            TblkSamples2.Text = stringSample2_1 + stringSample2_2 + stringSample2_3 + stringSample2_4;
            TblkSamples3.Text = stringSample3_1 + stringSample3_2 + stringSample3_3 + stringSample3_4 + stringSample3_5;

            ButSamplesA36Steel.Content = "ASTM-A36 Steel Properties";
            ButSamplesBeamsWF.Content = "Wide Flange Beams";
            TblkInfoPUC.Text = stringInfoPUC;
            ButSamplesPUC.Content = stringTitlePUC;
            ButSamplesA36Steel.Tag = "https://en.wikipedia.org/wiki/A36_steel";
            ButSamplesBeamsWF.Tag = "https://www.structural-drafting-net-expert.com/steel-sections-i-beam-w-shape.html";
            ButSamplesPUC.Tag = "https://www.microsoft.com/store/apps/9NBLGGH438MK";
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
            List<Button> listButton = new List<Button>()
            {
                ButSamplesA36Steel,
                ButSamplesBeamsWF,
                ButSamplesPUC
            };
            LibMPC.SizePageButtons(listButton);
        }

        /// <summary>
        /// Invoked when user clicks a button requesting link to more information.
        /// </summary>
        /// <param name="sender">A button with a Tag that contains hyperlink string.</param>
        /// <param name="e"></param>
        private async void ButAboutHyperlink_Click(object sender, RoutedEventArgs e)
        {
            _ = e;          // Discard unused parameter.
            await LibMPC.ButtonHyperlinkLaunchAsync((Button)sender);
        }

    }
}
