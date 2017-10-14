using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CoreLib;
using CoreLib.Impl;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : MetroWindow
    {
        private static SetupWindow _singleton;
        private bool _isFinished = false;

        public static SetupWindow Instance => _singleton ?? (_singleton = new SetupWindow());

        private readonly List<Page> _pages = new List<Page>();
        private int _pageIndex = 0;

        public SetupWindow()
        {
            InitializeComponent();
            /* Order matters here */
            _pages.Add(new SetupGreeting());
            /* TODO: where to put config? Global var? */
            // Settings could be a public static variable in <see cref="ConfigHandler"/> or another
            // represented class, such as Settings.
            _pages.Add(new SetupDirectory(new ConfigHandler(ModManager.ConfigFileLocation, WindowsFileSystem.Instance)));
        }

        public void SetPage(Page page)
        {
            ContentFrame.Navigate(page);
        }

        public void NextPage()
        {
            SetPage(_pages[++_pageIndex]);
        }

        public void PrevPage()
        {
            SetPage(_pages[--_pageIndex]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isFinished)
                e.Cancel = MessageBox.Show("Do you really want to cancel the setup?", "PonySFM Setup", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No;
            else
                e.Cancel = false;
        }

        public void GoToMainWindow()
        {
            _isFinished = true;
            MainWindow.Instance.InitialisePages();
            MainWindow.Instance.Show();
            Close();
        }
    }
}
