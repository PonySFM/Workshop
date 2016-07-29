using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.Windows.Shapes;

namespace PonySFM_Desktop
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        private static SetupWindow singleton;

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
            pages.Add(new SetupDirectory());
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
            e.Cancel = MessageBox.Show("Do you really want to cancel the setup?", "PonySFM Setup", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No;
        }
    }
}
