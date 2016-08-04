using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        ConfigFile _configFile;
        MainWindowPresenter _presenter;
        SFMDirectoryParser _sfmDirParser;

        public MainWindow(ConfigFile configFile, RevisionManager revisionManager)
        {
            _configFile = configFile;
            _sfmDirParser = new SFMDirectoryParser(_configFile.SFMDirectoryPath, WindowsFileSystem.Instance);
            _presenter = new MainWindowPresenter(revisionManager);
            _presenter.View = this;
            InitializeComponent();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutDialog().ShowDialog();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            _presenter.OnUninstall();
            dataGrid.Items.Refresh();
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            _presenter.OnVerify();
            dataGrid.Items.Refresh();
        }

        private void MenuItemOpenSFMDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_configFile.SFMDirectoryPath);
        }

        private void MenuViewOnSite_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGrid.CurrentItem;
            if (item == null)
                return;

            var url = PonySFMAPIConnector.Instance.GetRevisionURL((item as RevisionListItem).Revision);
            Process.Start(url);
        }
    }
}
