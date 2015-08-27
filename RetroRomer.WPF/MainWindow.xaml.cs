using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RetroRomer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
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
            var dlg = new Microsoft.Win32.OpenFileDialog
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
            // TODO: Start the download process
        }

        private void radioButtonRetroRoms_Click(object sender, RoutedEventArgs e)
        {
            textBoxCustomWebsite.IsEnabled = false;
        }
    }
}
