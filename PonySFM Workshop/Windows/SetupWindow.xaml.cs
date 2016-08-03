using MahApps.Metro.Controls;
using PonySFM_Workshop.Source;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : MetroWindow
    {
        private static SetupWindow singleton;
        private bool isFinished = false;
        
        public static SetupWindow Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new SetupWindow();

                return singleton;
            }
        }

        private List<Page> pages = new List<Page>();
        private int pageIndex = 0;

        public SetupWindow()
        {
            InitializeComponent();
            /* Order matters here */
            pages.Add(new SetupGreeting());
            /* TODO: where to put config? Global var? */
            /// Settings could be a public static variable in <see cref="ConfigHandler"/> or another
            /// represented class, such as Settings.
            pages.Add(new SetupDirectory(new ConfigHandler(ModManager.ConfigFileLocation, WindowsFileSystem.Instance)));
        }

        public void SetPage(Page page)
        {
            ContentFrame.Navigate(page);
        }

        public void NextPage()
        {
            SetPage(pages[++pageIndex]);
        }

        public void PrevPage()
        {
            SetPage(pages[--pageIndex]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isFinished)
                e.Cancel = MessageBox.Show("Do you really want to cancel the setup?", "PonySFM Setup", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No;
            else
                e.Cancel = false;
        }

        public void GoToMainWindow()
        {
            isFinished = true;

            /* TODO: we really shouldn't have to redeclare this here */
            ConfigHandler config = new ConfigHandler(ModManager.ConfigFileLocation, WindowsFileSystem.Instance);
            RevisionManager revMgr = new RevisionManager(config.Read(), WindowsFileSystem.Instance);

            new MainWindow(revMgr).Show();
            Close();
        }
    }
}
