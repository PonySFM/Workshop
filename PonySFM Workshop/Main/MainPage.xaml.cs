using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CoreLib;
using CoreLib.Impl;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        ConfigFile _configFile;
        MainWindow _window;
        MainWindowPresenter _presenter;
        SFMDirectoryParser _sfmDirParser;

        public MainPage(MainWindow window, ConfigFile configFile, RevisionManager revisionManager)
        {
            _window = window;
            _configFile = configFile;
            _sfmDirParser = new SFMDirectoryParser(_configFile.SFMDirectoryPath, WindowsFileSystem.Instance);
            _presenter = new MainWindowPresenter(revisionManager);
            _presenter.View = this;
            InitializeComponent();
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
