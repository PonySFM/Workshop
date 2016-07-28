﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Diagnostics;

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
            if (!UriProtocolExists())
                InstallUriProtocol();

            if(e.Args.Length == 1)
            {
                string uri = e.Args[0];
                uri = uri.TrimStart("ponysfm://".ToCharArray());
                uri = uri.TrimEnd('/');
                new InstallationWindow(Convert.ToInt32(uri)).ShowDialog();
                return;
            }

            /*
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
            */

            var w = SetupWindow.Instance;
            w.SetPage(new SetupGreeting());
            w.Show();
        }
    }
}