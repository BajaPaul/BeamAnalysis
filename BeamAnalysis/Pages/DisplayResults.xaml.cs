using BeamAnalysis.Common;
using LibraryCoder.MainPageCommon;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace BeamAnalysis.Pages
{
    public sealed partial class DisplayResults : Page
    {
        /// <summary>
        /// Pointer to MainPage used to call public methods or variables in MainPage.
        /// </summary>
        private readonly MainPage mainPage = MainPage.mainPagePointer;

        /// <summary>
        /// Page that displays calculated beam results from User entered input. Output is formatted text string.
        /// </summary>
        public DisplayResults()
        {
            InitializeComponent();
            // Set MainPage Button Visibility.
            LibMPC.ButtonVisibility(mainPage.mainPageButAbout, false);
            LibMPC.ButtonVisibility(mainPage.mainPageButBack, true);
            LibMPC.ButtonVisibility(mainPage.mainPageButSamples, false);
            // Overwrite XAML TextBlock Foreground colors.
            TblkDisplayResultsNote.Foreground = LibMPC.colorBright;
            TblkDisplayResultsOutput.Foreground = LibMPC.colorNormal;
            TblkDisplayResultsSaveMsgs.Foreground = LibMPC.colorSuccess;
            ButDisplayResultsSave.Foreground = LibMPC.colorSuccess;
            // Overwrite XAML default values.
            TblkDisplayResultsNote.Text = "Calculated beam results";
            TblkDisplayResultsOutput.Text = CommonItems.stringOutputResults;
            TblkDisplayResultsSaveMsgs.Text = "Click following button to save these results to a file. Then select folder you want to save results too.  Then select existing file or right-click to create a New > Text Document.  If new file, then rename Text Document to something else.  Make sure the file is selected before clicking Open.";
            ButDisplayResultsSave.Content = "Save results";
            // Clear CommonItems.stringOutputResults since no longer needed.
            CommonItems.stringOutputResults = string.Empty;
            // Setup scrolling for this page.
            LibMPC.ScrollViewerOn(mainPage.mainPageScrollViewer, horz: ScrollMode.Disabled, vert: ScrollMode.Auto, horzVis: ScrollBarVisibility.Disabled, vertVis: ScrollBarVisibility.Auto, zoom: ZoomMode.Disabled);
        }

        /// <summary>
        /// Save results to a file.
        /// </summary>
        private async void AsyncSaveResultsToFile()
        {
            //Debug.WriteLine($"AsyncSaveResultsToFile(): Method start.");
            FileOpenPicker fileOpenPicker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                ViewMode = PickerViewMode.List
            };
            // Need at least one filter to prevent exception.
            fileOpenPicker.FileTypeFilter.Add("*");
            StorageFile storageFilePicked = await fileOpenPicker.PickSingleFileAsync();
            if (storageFilePicked != null)
            {
                string stringOutputTitle = mainPage.stringAppTitle;
                if (!CommonItems.boolAppPurchased)
                {
                    // App not purchased.
                    stringOutputTitle += " - Application in trial mode since not purchased.";
                }
                //Debug.WriteLine($"AsyncSaveResultsToFile(): stringOutputTitle={stringOutputTitle}");
                DateTime dateTime = DateTime.Now;
                string stringOutputNote = $"{TblkDisplayResultsNote.Text}: {dateTime:d} @ {dateTime:t}";
                //Debug.WriteLine($"AsyncSaveResultsToFile(): Day: {dateTime:d} Time: {dateTime:t}");
                string stringOutput = $"{stringOutputTitle}\n\n{stringOutputNote}\n{TblkDisplayResultsOutput.Text}";
                await FileIO.WriteTextAsync(storageFilePicked, stringOutput);
                LibMPC.OutputMsgSuccess(TblkDisplayResultsSaveMsgs, $"Saved results to {storageFilePicked.Path}.");
                //Debug.WriteLine($"AsyncSaveResultsToFile(): Saved results to { storageFilePicked.Path}.");
            }
            else   // User did not select a file.
            {
                LibMPC.OutputMsgError(TblkDisplayResultsSaveMsgs, "Did not save results since no file selected.");
                //Debug.WriteLine($"AsyncSaveResultsToFile(): Did not save results since no file selected.");
            }
        }

        /// <summary>
        /// Event that saves results to a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButDisplayResultsSave_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _ = sender;     // Discard unused parameter.
            _ = e;          // Discard unused parameter.
            //Debug.WriteLine($"ButDisplayResultsSave_Click(): Saving output results to a file.");
            AsyncSaveResultsToFile();
        }

    }
}
