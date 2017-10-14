using System.Windows;
using System.Windows.Controls;

namespace PonySFM_Workshop.Setup.Greeting
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
