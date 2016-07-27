using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PonySFM_Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            /* TODO: conditions on when to launch setup or main */
            if(!SFM.CheckConfigExists())
            {
                var w = SetupWindow.Instance;
                w.SetPage(new SetupGreeting());
                w.Show();
            }
            else
            {
                var w = global::PonySFM_Desktop.MainWindow.Instance;
                w.Show();
            }
        }
    }
}
