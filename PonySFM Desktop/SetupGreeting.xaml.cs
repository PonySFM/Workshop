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
    /// Interaction logic for SetupGreeting.xaml
    /// </summary>
    public partial class SetupGreeting : Page
    {
        public SetupGreeting()
        {
            InitializeComponent();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            SetupWindow.Instance.NextPage();
        }
    }
}
