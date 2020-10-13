using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using RetroRommer.Domain;
using Serilog;
using Serilog.Core;

namespace RetroRommer.Core
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RetroRommerService _service;
        private string _destinationPath;
        private string _filename;
        private readonly Logger _logger;
        private string _password;
        private string _username;

        public MainWindow()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            InitializeComponent();

            _service = new RetroRommerService(_logger);

            TextBoxCustomWebsite.Text = configuration.GetValue("Website", string.Empty);
            TextBoxFilename.Text = configuration.GetValue("MissFile", string.Empty);
            TextBoxDestination.Text = configuration.GetValue("Destination", string.Empty);
            TextBoxUsername.Text = configuration.GetValue("Username", string.Empty);
            PBoxPassword.Password = configuration.GetValue("Password", string.Empty);
        }

        private void ButtonSelectFile_Click(object sender, RoutedEventArgs e)
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
            TextBoxFilename.Text = filename;
        }

        private void ButtonSelectDestination_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true) TextBoxDestination.Text = dialog.SelectedPath;
        }

        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            _filename = TextBoxFilename.Text;
            _destinationPath = TextBoxDestination.Text;
            _username = TextBoxUsername.Text;
            _password = PBoxPassword.Password;

            PrepareAndDownloadFiles();

            MessageBox.Show("Download finished!", "Operation completed", MessageBoxButton.OK);
        }

        private async void PrepareAndDownloadFiles()
        {
            _logger.Information("Beginning to download files...");
            var fileContents = _service.ReadFile(_filename);
            var processedContents = _service.AddFilenameExtensionToEntries(fileContents);

            foreach (var file in processedContents)
                await _service.GetFile(file, _username, _password, _destinationPath);
        }
    }
}