using PonySFM_Workshop.Source;
using System;
using System.Collections.Generic;
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

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        SettingsPresenter _presenter;

        public SettingsPage(ConfigHandler config)
        {
            _presenter = new SettingsPresenter(config);
            _presenter.View = this;
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.SetPage("MainPage");
        }

        private void DirectoryBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            var newPath = SFM.ShowDirectoryDialog(DirectoryTextBox.Text);
            if (!string.IsNullOrEmpty(newPath))
                DirectoryTextBox.Text = newPath;
        }
    }
}
