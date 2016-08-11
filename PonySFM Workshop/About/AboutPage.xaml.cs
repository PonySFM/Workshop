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
        string ProjectVersion =>
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public AboutPage()
        {
            InitializeComponent();

            ProjectNameTextBlock.Text += ProjectVersion;
        }

        void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            e.Handled =
                Process.Start(e.Uri.AbsoluteUri).ExitCode > 0;
        }

        void Hyperlink_RequestPage(object sender, RequestNavigateEventArgs e)
        {
            MainWindow.Instance.SetPage(e.Uri.ToString());
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        void GoBack()
        {
            MainWindow.Instance.SetPage("MainPage");
        }
    }
}
