using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public string ProjectNameAndVersion =>
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " v" +
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public AboutPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        void Hyperlink_RequestPage(object sender, RequestNavigateEventArgs e)
        {
            MainWindow.Instance.SetPage("LicensesPage");
            e.Handled = true;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        void GoBack()
        {
            MainWindow.Instance.SetPage("MainPage");
        }
    }
}
