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

namespace PonySFM_Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static MainWindow singleton;

        private MainWindow()
        {
            InitializeComponent();

            _installedRevisions.Add(new Revision(2, null));
            dataGrid.DataContext = this;
        }

        public List<Revision> _installedRevisions = new List<Revision>();

        public List<Revision> InstalledRevisions
        {
            get
            {
                return _installedRevisions;
            }
        }

        public static MainWindow Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new MainWindow();
                return singleton;
            }
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            new AboutDialog().ShowDialog();
        }
    }
}
