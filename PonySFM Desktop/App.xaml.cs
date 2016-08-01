using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using PonySFM_Desktop.Source;

namespace PonySFM_Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
         static void InstallUriProtocol()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();

            processStartInfo.FileName = "PostInstall.exe";

            processStartInfo.Verb = "runas";
            processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            processStartInfo.UseShellExecute = true;

            Console.WriteLine("Need to register custom application protocol.");
            Console.WriteLine("Please confirm UAC, we won't bother you again.\n");

            Logger.Log("Prompting to install URI protocol...\n");

            try
            {
                Process.Start(processStartInfo);
            }
            catch(Exception e)
            {
                Logger.Log("Failed to call PostInstall.exe: " + e.Message);
                return;
            }
            finally
            {
                Logger.Log("Done!\n");
            }
        }

        bool UriProtocolExists()
        {
            var regValue = Registry.GetValue(@"HKEY_CLASSES_ROOT\ponysfm", "URL protocol", null);
            if (regValue == null)
                return false;

            var cmd = Registry.GetValue(@"HKEY_CLASSES_ROOT\ponysfm\Shell\open\command", null, null);
            var cmdStr = (String)cmd;
            Logger.Log(@"HKEY_CLASS_ROOT\ponysfm\Shell\open\command seems to be " + cmdStr + "\n");

            var loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return cmdStr.ToLower().Contains(System.Reflection.Assembly.GetExecutingAssembly().Location.ToLower());
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
#if !DEBUG
            if (!UriProtocolExists())
                InstallUriProtocol();
#endif

            ConfigHandler config = new ConfigHandler(ModManager.ConfigFileLocation, WindowsFileSystem.Instance);
            RevisionManager revMgr = new RevisionManager(config.Read(), WindowsFileSystem.Instance);

            if(e.Args.Length == 1)
            {
                string uri = e.Args[0];
                uri = uri.TrimStart("ponysfm://".ToCharArray());
                uri = uri.TrimEnd('/');
                int id = Convert.ToInt32(uri);

                if(revMgr.IsInstalled(id))
                {
                    var msg = MessageBox.Show("This revision is already installed. Do you want to uninstall?", "PonySFM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(msg == MessageBoxResult.Yes)
                    {
                        var list = new List<int>() { id };
                        new DeinstallationWindow(revMgr, list).ShowDialog();
                    }
                }
                else
                    new InstallationWindow(id, revMgr).ShowDialog();

                return;
            }

            if(config.Exists())
            {
                new MainWindow(revMgr).Show();
                return;
            }
            else
            {
                var w = SetupWindow.Instance;
                w.SetPage(new SetupGreeting());
                w.Show();
            }
        }
    }
}
