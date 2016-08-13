using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CoreLib;
using CoreLib.Impl;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private static MainWindow singleton;
        private SFMDirectoryParser _sfmDirParser;
        private ConfigFile _configFile;

        public static MainWindow Instance =>
            singleton == null ?
                singleton = new MainWindow() :
                singleton;

        private Dictionary<string, Page> _pages = new Dictionary<string, Page>();

        private MainWindow()
        {
            InitializeComponent();
        }

        public void InitialisePages()
        {
            var config = new ConfigHandler(ModManager.ConfigFileLocation, WindowsFileSystem.Instance);
            var configFile = config.Read();
            var revMgr = new RevisionManager(configFile, WindowsFileSystem.Instance);
            var sfmDirParser = new SFMDirectoryParser(configFile.SFMDirectoryPath, WindowsFileSystem.Instance);

            _sfmDirParser = sfmDirParser;
            _configFile = configFile;

            _pages["MainPage"] = new MainPage(Instance, configFile, revMgr);
            _pages["SettingsPage"] = new SettingsPage(config);
            _pages["LicensesPage"] = new LicensesPage();
            _pages["AboutPage"] = new AboutPage();

            Instance.SetPage("MainPage");
        }

        public void SetPage(string page)
        {
            ContentFrame.Navigate(_pages[page]);
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            SetPage("AboutPage");
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemOpenSFMDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_configFile.SFMDirectoryPath);
        }

        private void MenuItemOpenSFM_Click(object sender, RoutedEventArgs e)
        {
            var path = System.IO.Path.Combine(_sfmDirParser.Path, "sfm.exe");
            Process.Start(path);
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            SetPage("SettingsPage");
        }

        private void MenuItemOpenPonySFM_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ModManager.PonySFMURL);
        }
    }
}
