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
using System.Windows.Shapes;

namespace PonySFM_Desktop
{
    /// <summary>
    /// Interaction logic for InstallationWindow.xaml
    /// </summary>
    public partial class InstallationWindow : Window
    {
        int id;

        public InstallationWindow(int id)
        {
            this.id = id;
            Title = "Installing Revision " + id;
            InitializeComponent();

            Start();
        }

        private async void Start()
        {
            try
            {
                await Task.Run(() =>
                {
                    LogInstallation("Installing Revision " + id);
                    LogInstallation("Done!");
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //Close();
        }

        private void LogInstallation(string msg)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                installationLog.Text += msg;
            }));
        }
    }
}
