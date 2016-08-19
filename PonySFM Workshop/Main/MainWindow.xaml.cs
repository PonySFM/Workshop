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
using System.Runtime.InteropServices;
using System;
using System.Windows.Interop;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private SFMDirectoryParser _sfmDirParser;
        private ConfigFile _configFile;

        public static MainWindow Instance;

        private Dictionary<string, Page> _pages = new Dictionary<string, Page>();

        static bool MenuIsOpen;

        public string StatusBarText
        {
            get
            {
                return "SFM Directory: \"" + _configFile.SFMDirectoryPath + "\"";
            }
        }

        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);

        private MainWindow()
        {
            InitializeComponent();

            Instance = this;
            StatusBarTextBlock.DataContext = this;
        }

        public static void Initiate()
        {
            new MainWindow();
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

        void CloseMenu()
        {
            MenuBar.BeginStoryboard(FindResource("MenuBarClose") as Storyboard);

            MenuIsOpen = false;
        }

        void OpenMenu()
        {
            MenuBar.BeginStoryboard(FindResource("MenuBarOpen") as Storyboard);

            MenuIsOpen = true;
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

            if (!MenuIsOpen)
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

            if (MenuIsOpen)
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

        private void MetroWindow_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MenuIsOpen && e.GetPosition(sender as IInputElement).X > MenuBar.Width)
                CloseMenu();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetWindowText(new WindowInteropHelper(this).Handle, "PonySFM Workshop");
        }
    }
}
