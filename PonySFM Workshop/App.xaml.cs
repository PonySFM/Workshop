using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using System.IO.Pipes;
using CoreLib;
using CoreLib.Impl;
using System.Threading.Tasks;
using System.IO;

namespace PonySFM_Workshop
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
            catch (Exception e)
            {
                Logger.Log("Failed to call PostInstall.exe: " + e.Message);
                return;
            }
            finally
            {
                Logger.Log("Done!\n");
            }
        }

        static string PipeName = "PSFM_WS";
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

        private void StartPipeThread()
        {
            Task.Run(() =>
            {
                var ps = new PipeSecurity();
                var par = new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
                ps.AddAccessRule(par);
                using (var server = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 512, 512, ps, HandleInheritability.None))
                {
                    while (true)
                    {
                        server.WaitForConnection();
                        var reader = new StreamReader(server);
                        string line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            break;
                        if(line == "BringToFront")
                        {
                            Dispatcher.Invoke(() => PonySFM_Workshop.MainWindow.Instance.Activate());
                        }
                        else
                        {
                            Dispatcher.Invoke(() => PonySFM_Workshop.MainWindow.Instance.Activate());
                            int id = Convert.ToInt32(line);
                            if(id != 0)
                            {
                                Dispatcher.Invoke(() => StartInstallation(id));
                            }
                        }
                        server.Disconnect();
                    }
                }
            });
        }

        static void SendPipeString(string str)
        {
            using (var client = new NamedPipeClientStream(".", PipeName))
            {
                client.Connect(1000);
                if (!client.IsConnected)
                    return;
                using (var writer = new StreamWriter(client))
                {
                    writer.WriteLine(str);
                    writer.Flush();
                    client.WaitForPipeDrain();
                }
            }
        }

        ConfigHandler config;
        RevisionManager revMgr;

        private bool StartInstallation(int id)
        {
            /* force refresh */
            revMgr = new RevisionManager(config.Read(), WindowsFileSystem.Instance);

            if (revMgr.IsInstalled(id))
            {
                var msg = MessageBox.Show("This revision is already installed. Do you want to uninstall?",
                    "PonySFM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msg == MessageBoxResult.Yes)
                {
                    var list = new List<int>() { id };
                    new DeinstallationWindow(revMgr, list, true).ShowDialog();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                List<int> ids = new List<int>() { id };
                new InstallationWindow(ids, revMgr, true).ShowDialog();
            }

            PonySFM_Workshop.MainWindow.Instance.RefreshListData();

            return true;
        } 

        static Mutex AppMutex;
        static string MainMutexName = "{DD4066A3-069D-4EC1-BDB8-FA1CCE1C52C4}";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ModManager.CreateFolders();

#if !DEBUG
            if (!UriProtocolExists())
                InstallUriProtocol();
#endif
            bool mutexCreated;
            // The string can be anything, but re-producing the same GUID would be quite a thing.
            AppMutex = new Mutex(true, MainMutexName, out mutexCreated);

            if (mutexCreated)
            {
                Logger.Open();
                StartPipeThread();
            }

            config = new ConfigHandler(ModManager.ConfigFileLocation, WindowsFileSystem.Instance);
            if (config.Exists())
            {
                revMgr = new RevisionManager(config.Read(), WindowsFileSystem.Instance);

                if (e.Args.Length == 1)
                {
                    string uri = e.Args[0];
                    uri = uri.TrimStart("ponysfm://".ToCharArray());
                    uri = uri.TrimEnd('/');
                    int id = Convert.ToInt32(uri);

                    if (mutexCreated)
                    {
                        StartInstallation(id);
                        return;
                    }
                    else
                    {
                        SendPipeString(id.ToString());
                    }
                }

                if (mutexCreated)
                {
                    PonySFM_Workshop.MainWindow.Instance.InitialisePages();
                    PonySFM_Workshop.MainWindow.Instance.Show();
                }
                else
                {
                    SendPipeString("BringToFront");
                }
            }
            else
            {
                var w = SetupWindow.Instance;
                w.SetPage(new SetupGreeting());
                w.Show();
            }

            /* Immediately shut down if in client mode */
            if(!mutexCreated)
            {
                Shutdown();
            }
        }
    }
}

