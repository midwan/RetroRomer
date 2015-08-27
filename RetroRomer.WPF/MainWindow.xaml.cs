using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace RetroRomer.WPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void radioButtonCustom_Checked(object sender, RoutedEventArgs e)
        {
            textBoxCustomWebsite.IsEnabled = true;
        }

        private void buttonSelectFile_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter =
                    "TXT Files (*.txt)|*.txt"
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            var filename = dlg.FileName;
            textBoxFilename.Text = filename;
        }

        private void buttonSelectDestination_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBoxDestination.Text = dialog.SelectedPath;
            }
        }

        private void buttonDownload_Click(object sender, RoutedEventArgs e)
        {
            PrepareAndDownloadFiles();
        }

        private void PrepareAndDownloadFiles()
        {
            var logResults = new List<string>();
            listBoxLog.ItemsSource = logResults;

            var fileReader = new FileReader();
            var fileContents = fileReader.ReadFile(textBoxFilename.Text);
            logResults.Add($"Opened and read file {textBoxFilename.Text}");

            var processedContents = fileReader.AddFilenameExtensionToEntries(fileContents);
            logResults.Add($"Processed contents of file.");

            var downloader = new Downloader
            {
                DestinationPath = textBoxDestination.Text,
                Username = textBoxUsername.Text,
                Password = pBoxPassword.Password
            };
            logResults.Add($"Initialized downloader");

            foreach (var file in processedContents)
            {
                logResults.Add($"Downloading file {file}");
                var response = downloader.GetFile(file);
                logResults.Add(response ? $"Successfully downloaded {file}" : $"Failed to download {file}");
            }

            System.Windows.MessageBox.Show("Download finished!", "Operation completed", MessageBoxButton.OK);
        }

        private void radioButtonRetroRoms_Click(object sender, RoutedEventArgs e)
        {
            textBoxCustomWebsite.IsEnabled = false;
        }
    }
}