using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CoreLib;
using CoreLib.Impl;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Animation;

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

        static bool MenuOpened;

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

        public void RefreshListData()
        {
            (_pages["MainPage"] as MainPage).RefreshListData();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Menu buttons

        // Online Guide
        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {


            //CloseMenu();
        }

        private void MenuItemOpenSFMDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_configFile.SFMDirectoryPath);

            CloseMenu();
        }

        private void MenuItemOpenSFM_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Path.Combine(_sfmDirParser.Path, "sfm.exe"));

            CloseMenu();
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            SetPage("SettingsPage");

            CloseMenu();
        }

        private void MenuItemOpenPonySFM_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ModManager.PonySFMURL);

            CloseMenu();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            SetPage("AboutPage");

            CloseMenu();
        }

        // Other events

        private void MetroWindow_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MenuOpened && e.GetPosition(sender as IInputElement).X > MenuBar.Width)
                CloseMenu();
        }
        
        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            OpenMenu();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            CloseMenu();
        }

        void CloseMenu()
        {
            MenuBar.BeginStoryboard(FindResource("MenuBarClose") as Storyboard);

            MenuOpened = false;
        }

        void OpenMenu()
        {
            MenuBar.BeginStoryboard(FindResource("MenuBarOpen") as Storyboard);

            MenuOpened = true;
        }
    }
}
