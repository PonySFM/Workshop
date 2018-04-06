using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using CoreLib;
using CoreLib.Impl;
using MahApps.Metro.Controls;

namespace PonySFM_Workshop.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private SFMDirectoryParser _sfmDirParser;
        private ConfigFile _configFile;

        private static MainWindow _instance;

        public static MainWindow Instance => _instance ?? (_instance = new MainWindow());

        private readonly Dictionary<string, Page> _pages = new Dictionary<string, Page>();

        static bool _menuIsOpen;

        public string StatusBarText => "SFM Directory: \"" + _configFile?.SfmDirectoryPath + "\"";

        private MainWindow()
        {
            InitializeComponent();
            StatusBarTextBlock.DataContext = this;
        }

        public void InitialisePages()
        {
            var config = new ConfigHandler(ModManager.ConfigFileLocation, WindowsFileSystem.Instance);
            var configFile = config.Read();
            var revMgr = new RevisionManager(configFile, WindowsFileSystem.Instance);
            var sfmDirParser = new SFMDirectoryParser(configFile.SfmDirectoryPath, WindowsFileSystem.Instance);

            _sfmDirParser = sfmDirParser;
            _configFile = configFile;

            _pages["MainPage"] = new MainPage(Instance, configFile, revMgr);
            _pages["SettingsPage"] = new SettingsPage(config);
            _pages["LicensesPage"] = new Licenses.LicensesPage();
            _pages["AboutPage"] = new About.AboutPage(revMgr.Database);

            Instance.SetPage("MainPage");
        }

        public void SetPage(string page)
        {
            ContentFrame.Navigate(_pages[page]);
        }

        public void SetPage(Page page)
        {
            ContentFrame.Navigate(page);
        }

        public void RefreshListData()
        {
            ((MainPage) _pages["MainPage"]).RefreshListData();
        }

        private void CloseMenu()
        {
            MenuBar.BeginStoryboard(FindResource("MenuBarClose") as Storyboard);

            _menuIsOpen = false;
        }

        private void OpenMenu()
        {
            MenuBar.BeginStoryboard(FindResource("MenuBarOpen") as Storyboard);

            _menuIsOpen = true;
        }
        // Menu buttons

        // Online Guide
        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {


            //CloseMenu();
        }

        private void MenuItemOpenSFMDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_configFile.SfmDirectoryPath);

            CloseMenu();
        }

        private void MenuItemOpenSFM_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Path.Combine(_sfmDirParser.Path, "sfm.exe"));

            if (_menuIsOpen)
                CloseMenu();
        }

        private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
        {
            SetPage("SettingsPage");

            CloseMenu();
        }

        private void MenuItemOpenPonySFM_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ModManager.PonySfmurl);

            if (_menuIsOpen)
                CloseMenu();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            SetPage("AboutPage");

            CloseMenu();
        }

        // Window control events
        
        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            OpenMenu();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            CloseMenu();
        }

        // Other events

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_menuIsOpen && e.GetPosition(sender as IInputElement).X > MenuBar.Width)
                CloseMenu();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NativeMethods.SetWindowText(new WindowInteropHelper(this).Handle, "PonySFM Workshop");
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.patreon.com/PonySFM");
        }
    }
}
