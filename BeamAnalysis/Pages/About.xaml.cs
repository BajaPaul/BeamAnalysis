using LibraryCoder.MainPageCommon;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BeamAnalysis.Pages
{
    public sealed partial class About : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        private const string stringAboutPayment = "Unlimited trial period for evaluation.  Buy Application once and use on all your Windows 10 devices.  Please rate and review Application.";
        private const string stringAboutApp = "Application uses the Finite Element Method (FEM) to analyze 2D beams.  Beams can be configured with multiple support and load configurations.\n\nOne or more supports can be entered.  Displacement and rotation at each support can be restrained.\n\nOne or more concentrated loads can be entered.  Each concentrated load can contain a vertical force and/or moment value.\n\nOne or more uniform loads can be entered.  Rectangular, triangular, and trapezoidal uniform loads are supported by Application.  Each uniform load can be applied to whole beam or portion of beam.Uniform loads can overlap each other.\n\nUniform loads are simulated with a series of concentrated loads.  The spread distance of simulated loads can be adjusted.  Smaller spread distances will produce more simulated loads and more accurate results but will require more time to calculate results.";
        private const string stringAboutCredit = "CREDIT: Underlying FEM C# code used by this application was developed by Staskolukasz.  He placed his code on GitHub.com via ‘fob2d’, ‘Simple implementation of the Finite Element Method in C#.  Single span steel I beam under concentrated loads.’  Staskolukasz’s underlying code has been updated considerably by programmer to run within the UWP environment using latest Visual Studio releases.  His code has also been customized considerably to allow input and output for custom beam support and load configurations entered by User.";
        private const string stringAboutDisclaim = "DISCLAIMER: User assumes all risk of using calculated results from Application.  Developer has done considerable research and testing to validate results from Application are valid.\n\nApplication performs minimal error checking of User entered values to catch obvious errors.  It is beyond scope of this application to check that every possible User entered value for every possible beam, support, and load configuration returns valid results.  Garbage in, garbage out.";
        private const string stringAboutLinks = "Explore following links for more information about Application.";

        /// <summary>
        /// Page that shows information about application.
        /// </summary>
        public About()
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
            TblkAboutDeveloper.Foreground = LibMPC.colorBright;
            TblkAboutPayment.Foreground = LibMPC.colorSuccess;
            TblkAboutApp.Foreground = LibMPC.colorNormal;
            TblkAboutCredit.Foreground = LibMPC.colorSuccess;
            TblkAboutDisclaim.Foreground = LibMPC.colorError;
            TblkAboutLinks.Foreground = LibMPC.colorSuccess;
            // Overwrite XAML default values.
            TblkAboutDeveloper.Text = $"Installed application version is {mainPage.stringAppVersion}\nContact developer using following Email link if you encounter issues with application.";
            TblkAboutPayment.Text = stringAboutPayment;
            TblkAboutApp.Text = stringAboutApp;
            TblkAboutCredit.Text = stringAboutCredit;
            TblkAboutDisclaim.Text = stringAboutDisclaim;
            TblkAboutLinks.Text = stringAboutLinks;
            ButAboutEmail.Content = "Email Support";
            ButAboutRateApp.Content = "Rate and review app";
            ButAboutCredit.Content = "Staskolukasz fob2d app";
            ButAboutFEM.Content = "Finite element method";
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
                ButAboutEmail,
                ButAboutRateApp,
                ButAboutCredit,
                ButAboutFEM
            };
            LibMPC.SizePageButtons(listButton);
            LibMPC.ButtonEmailXboxDisable(ButAboutEmail);
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

        /// <summary>
        /// Show User MS Store App rating popup box. Popup box will lock all access to App until closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButAboutRateApp_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.

            await mainPage.RateAppInW10StoreAsync();
        }

        private void TblkAboutApp_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
