using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace RetroRomer.WPF
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> _logResults;
        private string _filename;
        private string _destinationPath;
        private string _username;
        private string _password;

        public MainWindow()
        {
            _logResults = new ObservableCollection<string>();
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
            listBoxLog.ItemsSource = _logResults;
            _filename = textBoxFilename.Text;
            _destinationPath = textBoxDestination.Text;
            _username = textBoxUsername.Text;
            _password = pBoxPassword.Password;

            PrepareAndDownloadFiles();

            MessageBox.Show("Download finished!", "Operation completed", MessageBoxButton.OK);
        }

        private void PrepareAndDownloadFiles()
        {
            var fileReader = new FileReader();
            var fileContents = fileReader.ReadFile(_filename);
            _logResults.Add($"Opened and read file {_filename}");

            var processedContents = fileReader.AddFilenameExtensionToEntries(fileContents);
            _logResults.Add($"Processed contents of file.");

            var downloader = new Downloader
            {
                DestinationPath = _destinationPath,
                Username = _username,
                Password = _password
            };
            _logResults.Add($"Initialized downloader");
            foreach (var file in processedContents)
            {
                _logResults.Add($"Downloading file {file}");
                var response = downloader.GetFile(file);
                _logResults.Add(response ? $"Successfully downloaded {file}" : $"Failed to download {file}");
            }
        }

        private void radioButtonRetroRoms_Click(object sender, RoutedEventArgs e)
        {
            textBoxCustomWebsite.IsEnabled = false;
        }
    }
}