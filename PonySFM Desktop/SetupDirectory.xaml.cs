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

namespace PonySFM_Desktop
{
    /// <summary>
    /// Interaction logic for SetupDirectory.xaml
    /// </summary>
    public partial class SetupDirectory : Page
    {
        public SetupDirectory()
        {
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

            if (!Directory.Exists(dir))
            {
                MessageBox.Show("The path chosen is not a valid directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!SFM.LikelyToBeSFMDir(dir))
                if (MessageBox.Show("The directory does not seem like the typical SFM installation. Continue?",
                    "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                    return;

            SFM.SetDirectory(dir);
        }

        private void DirectoryPathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnNext.IsEnabled = !string.IsNullOrWhiteSpace(DirectoryPathBox.Text);
        }
    }
}
