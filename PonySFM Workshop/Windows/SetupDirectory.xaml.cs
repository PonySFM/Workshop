using PonySFM_Workshop.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for SetupDirectory.xaml
    /// </summary>
    public partial class SetupDirectory : Page
    {
        private ConfigHandler _config;

        public SetupDirectory(ConfigHandler config)
        {
            _config = config;
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var newPath = SFM.ShowDirectoryDialog(DirectoryPathBox.Text);
            if (!string.IsNullOrEmpty(newPath))
                DirectoryPathBox.Text = newPath;
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            SetupWindow.Instance.PrevPage();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            string dir = DirectoryPathBox.Text;
            var parser = new SFMDirectoryParser(dir, WindowsFileSystem.Instance);
            var error = parser.Validate();

            if (error == SFMDirectoryParserError.NotExists)
            {
                MessageBox.Show("The path chosen is not a valid directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (error == SFMDirectoryParserError.NotLikely)
                if (MessageBox.Show("The directory does not seem like the typical SFM installation. Continue?",
                    "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                    return;

            _config.Write(new ConfigFile(dir));

            SetupWindow.Instance.GoToMainWindow();
        }

        private void DirectoryPathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnNext.IsEnabled = !string.IsNullOrWhiteSpace(DirectoryPathBox.Text);
        }
    }
}
