using System.Collections.Generic;
using System.Windows;
using System.Threading;
using CoreLib;
using CoreLib.Impl;
using MahApps.Metro.Controls;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for InstallationWindow.xaml
    /// </summary>
    public partial class InstallationWindow : MetroWindow
    {
        InstallationPresenter _presenter;
        bool _closeOnFinish;
        bool _showDetails;

        public InstallationWindow(List<int> ids, RevisionManager revisionMgr, bool closeOnFinish = false)
        {
            _presenter = new InstallationPresenter(PonySFMAPIConnector.Instance, WindowsFileSystem.Instance, revisionMgr, ids);
            _presenter.View = this;
            _closeOnFinish = closeOnFinish;

            _presenter.OnFileExists += OnFileExistsTriggered;

            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _presenter.Execute();
            if (_closeOnFinish)
                Close();
        }

        public void OnFileExistsTriggered(object sender, DirectoryCopierFileExistsEventArgs e)
        {
            e.FileCopyMode = OnFileExists(e.File1, e.File2);
        }

        public DirectoryCopierFileCopyMode OnFileExists(string file1, string file2)
        {
            var ret = DirectoryCopierFileCopyMode.DoNotCopy;
            bool b = false;
            /* FIXME: This should block. It doesn't block. Why doesn't it block? */
            ret = Dispatcher.Invoke(() =>
            {
                switch (DialogSystem.Show("Conflict", string.Format("The file {0} already exists. Overwrite?", file2)))
                {
                    case PonySFM_Workshop.DialogResult.Ok:
                        ret = DirectoryCopierFileCopyMode.Copy;
                        break;
                    case PonySFM_Workshop.DialogResult.No:
                        ret = DirectoryCopierFileCopyMode.DoNotCopy;
                        break;
                    case PonySFM_Workshop.DialogResult.YesAll:
                        ret = DirectoryCopierFileCopyMode.CopyAll;
                        break;
                    case PonySFM_Workshop.DialogResult.Cancel:
                        ret = DirectoryCopierFileCopyMode.Cancel;
                        /* TODO: delete already installed files */
                        break;
                }

                b = true;
                return ret;
            });

            while (!b) Thread.Sleep(100);
            return ret;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /* TODO: prompt? */
            _presenter.CancellationSource.Cancel();
            e.Cancel = false;
        }

        private void ToggleDetailsBtn_Click(object sender, RoutedEventArgs e)
        {
            _showDetails = !_showDetails;

            if(_showDetails)
            {
                installationLog.Visibility = Visibility.Visible;
                Height = 500;
            }
            else
            {
                installationLog.Visibility = Visibility.Hidden;
                Height = 200;
            }
        }
    }
}
