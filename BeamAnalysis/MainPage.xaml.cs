using BeamAnalysis.Common;
using BeamAnalysis.Pages;
using LibraryCoder.MainPageCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Following Enum is generally unique for each App so place here.
/// <summary>
/// Enum used to reset App setup values via method AppReset().
/// </summary>
public enum EnumResetApp { DoNothing, ResetApp, ResetPurchaseHistory, ResetRateHistory, ShowDataStoreValues };

namespace BeamAnalysis
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Pointer to MainPage.  Other pages can use this pointer to access public methods and public variables created in MainPage.
        /// </summary>
        public static MainPage mainPagePointer;

        // TODO: Update version number in next string before publishing application to Microsoft Store.
        /// <summary>
        /// String containing version of application as set in Package.appxmanifest file.
        /// </summary>
        public readonly string stringAppVersion = "2021.4.3";

        /// <summary>
        /// String containing name of application.
        /// </summary>
        public readonly string stringAppTitle = "Beam Analysis";

        // Make following MainPage XAML items public so values can be changed elsewhere in application.

        /// <summary>
        /// Set public value of MainPage XAML ScrollViewerMP.
        /// </summary>
        public ScrollViewer mainPageScrollViewer;

        /// <summary>
        /// Set public value of MainPage XAML ButBack.
        /// </summary>
        public Button mainPageButBack;

        /// <summary>
        /// Set public value of MainPage XAML ButAbout.
        /// </summary>
        public Button mainPageButAbout;

        /// <summary>
        /// Set public value of MainPage XAML ButSample.
        /// </summary>
        public Button mainPageButSamples;

        /// <summary>
        /// Location App uses to read or write various App settings. Save set value here for use in other pages as needed.
        /// </summary>
        public ApplicationDataContainer applicationDataContainer;

        /// <summary>
        /// MainPage of application that sets up base page of application and allows navigation between various pages.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            mainPagePointer = this;     // Set pointer to this page at this location since required by various pages, methods, and libraries.

            // Overwrite XAML default values.
            ButBack.Content = "\uE72B";     // Show back arrow char versus "Back".
            ButAbout.Content = "About";
            ButSamples.Content = "Samples";
            TblkAppTitle.Text = stringAppTitle;

            // Set MainPage public values XAML variables so can be called from library LibMPC.
            mainPageScrollViewer = ScrollViewerMP;
            mainPageButBack = ButBack;
            mainPageButAbout = ButAbout;
            mainPageButSamples = ButSamples;
            // Back-a-page navigation event handler. Invoked when software or hardware back button is selected, 
            // or Windows key + Backspace is entered, or say, "Hey Cortana, go back".
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequestedPage;

            // Get App data store location.
            // https://msdn.microsoft.com/windows/uwp/app-settings/store-and-retrieve-app-data#local-app-data
            applicationDataContainer = ApplicationData.Current.LocalSettings;
            LibMPC.CustomizeAppTitleBar();

            // Initialize lists that will contain User entered supports, concentrated loads, uniforms loads, consolidated loads.
            CommonItems.listSupportValues = new List<SupportValues>();
            CommonItems.listLoadConcentratedValues = new List<LoadConcentratedValues>();
            CommonItems.listLoadUniformValues = new List<LoadUniformValues>();
            CommonItems.listLoadConcentratedValuesConsolidated = new List<LoadConcentratedValues>();

            // TODO: set next line to EnumResetApp.DoNothing before store publish.
            AppReset(EnumResetApp.DoNothing);   // Reset App to various states.

            // TODO: Comment out next 3 code lines before App publish.
            //StorageFolder storageFolderApp = ApplicationData.Current.LocalFolder;
            //Debug.WriteLine($"MainPage().Page_Loaded(): storageFolderApp.Path={storageFolderApp.Path}");
            //AppReset(EnumResetApp.ShowDataStoreValues);   // Show data store values.
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
                ButBack,
                ButAbout,
                ButSamples
            };
            LibMPC.SizePageButtons(listButton);
            LibMPC.ScrollViewerOff(mainPageScrollViewer);  // Turn ScrollViewerMP off for now.  Individual pages will set it as required.
            ShowPageHome();
        }

        /*** Public Methods ****************************************************************************************************/

        /// <summary>
        /// Open Windows 10 Store App so User can rate and review this App.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RateAppInW10StoreAsync()
        {
            if (await LibMPC.ShowRatingReviewDialogAsync())
            {
                CommonItems.boolAppRated = true;
                applicationDataContainer.Values[CommonItems.ds_BoolAppRated] = true;        // Write setting to data store. 
                applicationDataContainer.Values.Remove(CommonItems.ds_IntAppRatedCounter);  // Remove ds_IntAppRatedCounter since no longer used.
                Debug.WriteLine("RateAppInW10StoreAsync(): App was rated.");
                return true;
            }
            Debug.WriteLine("RateAppInW10StoreAsync(): App not rated.");
            return false;
        }

        /// <summary>
        /// Navigate to page Home.
        /// </summary>
        public void ShowPageHome()
        {
            FrameMP.Navigate(typeof(Home));
            FrameMP.BackStack.Clear();      // Clear navigation history after arriving at page Home.
        }

        /// <summary>
        /// Navigate to page EnterBeamProperties.
        /// </summary>
        public void ShowPageEnterBeamProperties()
        {
            FrameMP.Navigate(typeof(EnterBeamProperties));
        }

        /// <summary>
        /// Navigate to page EnterSupports.
        /// </summary>
        public void ShowPageEnterSupports()
        {
            FrameMP.Navigate(typeof(EnterSupports));
        }

        /// <summary>
        /// Navigate to page EnterLoadsConcentrated.
        /// </summary>
        public void ShowPageEnterLoadsConcentrated()
        {
            FrameMP.Navigate(typeof(EnterLoadsConcentrated));
        }

        /// <summary>
        /// Navigate to page EnterLoadsUniform.
        /// </summary>
        public void ShowPageEnterLoadsUniform()
        {
            FrameMP.Navigate(typeof(EnterLoadsUniform));
        }

        /// <summary>
        /// Navigate to page DisplayResults.
        /// </summary>
        public void ShowPageDisplayResults()
        {
            FrameMP.Navigate(typeof(DisplayResults));
        }

        /// <summary>
        /// Navigate to page DisplaySimulatedLoads.
        /// </summary>
        public void ShowPageDisplaySimulatedLoads()
        {
            FrameMP.Navigate(typeof(DisplaySimulatedLoads));
        }

        /// <summary>
        /// Navigate to page previous.
        /// </summary>
        public void ShowPagePrevious()
        {
            PageGoBack();
        }

        /*** Private Methods ***************************************************************************************************/

        /// <summary>
        /// Reset App to various states using parameter enumResetApp.
        /// </summary>
        /// <param name="enumResetApp">Enum used to reset App setup values.</param>
        private void AppReset(EnumResetApp enumResetApp)
        {
            switch (enumResetApp)
            {
                case EnumResetApp.DoNothing:                // Do nothing. Most common so exit quick.
                    break;
                case EnumResetApp.ResetApp:                 // Clear all data store settings.
                    applicationDataContainer.Values.Clear();
                    break;
                case EnumResetApp.ResetPurchaseHistory:     // Clear App purchase history.
                    applicationDataContainer.Values.Remove(CommonItems.ds_BoolAppPurchased);
                    CommonItems.boolAppPurchased = false;
                    break;
                case EnumResetApp.ResetRateHistory:         // Clear App rate history.
                    applicationDataContainer.Values.Remove(CommonItems.ds_BoolAppRated);
                    CommonItems.boolAppRated = false;
                    break;
                case EnumResetApp.ShowDataStoreValues:      // Show data store values via Debug.
                    LibMPC.ListDataStoreItems(applicationDataContainer);
                    break;
                default:    // Throw exception so error can be discovered and corrected.
                    throw new NotSupportedException($"MainPage.AppReset(): enumResetApp={enumResetApp} not found in switch statement.");
            }
        }

        /// <summary>
        /// Back-a-page navigation event handler. Invoked when software or hardware back button is selected, 
        /// or Windows key + Backspace is entered, or say, "Hey Cortana, go back".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackRequestedPage(object sender, BackRequestedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            // If event has not already been handled then navigate back to previous page.
            // Next if statement required to prevent App from ending abruptly on a back event.
            if (FrameMP.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                PageGoBack();
            }
        }

        /// <summary>
        /// Navigate back a page.
        /// </summary>
        private void PageGoBack()
        {
            if (FrameMP.CanGoBack)
                FrameMP.GoBack();
        }

        /// <summary>
        /// Navigate to page About.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButAbout_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            FrameMP.Navigate(typeof(About));
        }

        /// <summary>
        /// Navigate to page Samples.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButSamples_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            FrameMP.Navigate(typeof(Samples));
        }

        /// <summary>
        /// Navigate back to previous page when back button in title bar is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButBack_Click(object sender, RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            PageGoBack();
        }

    }
}
