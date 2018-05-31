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
using System.Reflection;
using System.Security.Principal;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static void RegisterUriProtocol(string protocolName, string applicationPath, string description)
        {
            // Create new key for desired URL protocol
            var key = Registry.ClassesRoot.CreateSubKey(protocolName);

            // Assign protocol
            if (key != null)
            {
                key.SetValue(null, description);
                key.SetValue("URL Protocol", string.Empty);

                // Register Shell values
                Registry.ClassesRoot.CreateSubKey(protocolName + "\\Shell");
                Registry.ClassesRoot.CreateSubKey(protocolName + "\\Shell\\open");
            }

            key = Registry.ClassesRoot.CreateSubKey(protocolName + "\\Shell\\open\\command");

            // Specify application handling the URL protocol
            key?.SetValue(null, "\"" + applicationPath + "\" %1");
        }

        private static void InstallUriProtocol()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = Assembly.GetExecutingAssembly().Location,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = true,
                Arguments = "--register-uri-protocol"
            };

            Logger.Log("Prompting to install URI protocol...\n");

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception e)
            {
                Logger.Log("Failed to install URI protocol: " + e.Message);
                return;
            }
            finally
            {
                Logger.Log("Done!\n");
            }
        }

        private static bool UriProtocolExists()
        {
            var regValue = Registry.GetValue(@"HKEY_CLASSES_ROOT\ponysfm", "URL protocol", null);
            if (regValue == null)
                return false;

            var cmd = Registry.GetValue(@"HKEY_CLASSES_ROOT\ponysfm\Shell\open\command", null, null);
            var cmdStr = (string)cmd;
            Logger.Log(@"HKEY_CLASS_ROOT\ponysfm\Shell\open\command seems to be " + cmdStr + "\n");

            var loc = System.Reflection.Assembly.GetExecutingAssembly().Location;
            return cmdStr.ToLower().Contains(System.Reflection.Assembly.GetExecutingAssembly().Location.ToLower());
        }

        private const string PipeName = "PSFM_WS";

        private void StartPipeThread()
        {
            Task.Run(() =>
            {
                var ps = new PipeSecurity();
                var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                var par = new PipeAccessRule(everyone, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
                ps.AddAccessRule(par);
                using (var server = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 512, 512, ps, HandleInheritability.None))
                {
                    while (true)
                    {
                        server.WaitForConnection();
                        var reader = new StreamReader(server);
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            break;
                        if(line == "BringToFront")
                        {
                            Dispatcher.Invoke(() => PonySFM_Workshop.Main.MainWindow.Instance.Activate());
                        }
                        else
                        {
                            Dispatcher.Invoke(() => PonySFM_Workshop.Main.MainWindow.Instance.Activate());
                            var id = Convert.ToInt32(line);
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

        private static void SendPipeString(string str)
        {
            var client = new NamedPipeClientStream(".", PipeName);
            client.Connect(1000);

            if (client.IsConnected)
            {
                var writer = new StreamWriter(client);
                writer.WriteLine(str);
                writer.Flush();
                client.WaitForPipeDrain();
            }
        }

        private ConfigHandler _config;
        private RevisionManager _revMgr;

        private bool StartInstallation(int id)
        {
            /* force refresh */
            _revMgr = new RevisionManager(_config.Read(), WindowsFileSystem.Instance);

            if (_revMgr.IsInstalled(id))
            {
                var msg = MessageBox.Show("This revision is already installed. Do you want to uninstall?",
                    "PonySFM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msg == MessageBoxResult.Yes)
                {
                    var list = new List<int>() { id };
                    new Deinstallation.DeinstallationWindow(_revMgr, list, true).ShowDialog();
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var ids = new List<int>() { id };
                new Installation.InstallationWindow(ids, _revMgr, true).ShowDialog();
            }

            PonySFM_Workshop.Main.MainWindow.Instance.RefreshListData();

            return true;
        } 

        private static Mutex _appMutex;
        private const string MainMutexName = "{DD4066A3-069D-4EC1-BDB8-FA1CCE1C52C4}";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool installUriProtocol = e.Args.Length > 0 && e.Args[0] == "--register-uri-protocol";
            if (installUriProtocol)
            {
                RegisterUriProtocol("ponysfm", Assembly.GetExecutingAssembly().Location, "PonySFM Installer Client");
                return;
            }

            ModManager.CreateFolders();

#if !DEBUG
            if (!UriProtocolExists()) {
                InstallUriProtocol();
            }
#endif

            bool mutexCreated;
            // The string can be anything, but re-producing the same GUID would be quite a thing.
            _appMutex = new Mutex(true, MainMutexName, out mutexCreated);

            if (mutexCreated)
            {
                Logger.Open();
                StartPipeThread();
            }

            _config = new ConfigHandler(ModManager.ConfigFileLocation, WindowsFileSystem.Instance);
            if (_config.Exists())
            {
                _revMgr = new RevisionManager(_config.Read(), WindowsFileSystem.Instance);

                if (e.Args.Length == 1)
                {
                    var uri = e.Args[0];
                    uri = uri.TrimStart("ponysfm://".ToCharArray());
                    uri = uri.TrimEnd('/');
                    var id = Convert.ToInt32(uri);

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
                    PonySFM_Workshop.Main.MainWindow.Instance.InitialisePages();
                    PonySFM_Workshop.Main.MainWindow.Instance.Show();
                }
                else
                {
                    SendPipeString("BringToFront");
                }
            }
            else // Installation mode
            {
                var w = Setup.SetupWindow.Instance;
                w.SetPage(new Setup.Greeting.SetupGreeting());
                w.Show();
            }

            /* Immediately shut down if in client mode */
            if (!mutexCreated)
            {
                Shutdown();
            }
        }
    }
}

