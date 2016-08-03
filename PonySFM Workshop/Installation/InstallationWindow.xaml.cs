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
using System.Threading;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for InstallationWindow.xaml
    /// </summary>
    public partial class InstallationWindow : Window
    {
        InstallationPresenter _presenter;
        bool _closeOnFinish;

        public InstallationWindow(List<int> ids, RevisionManager revisionMgr, bool closeOnFinish = false)
        {
            _presenter = new InstallationPresenter(PonySFMAPIConnector.Instance, WindowsFileSystem.Instance, revisionMgr, ids);
            _presenter.View = this;
            _closeOnFinish = closeOnFinish;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _presenter.Execute();
            if (_closeOnFinish)
                Close();
        }

        public DirectoryCopierFileCopyMode OnFileExists(string file1, string file2)
        {
            var ret = DirectoryCopierFileCopyMode.DoNotCopy;
            bool b = false;
            /* FIXME: This should block. It doesn't block. Why doesn't it block? */
            ret = Dispatcher.Invoke(() =>
            {
                var window = new DialogBox(string.Format("The file {0} already exists. Overwrite?", file2));
                window.ShowDialog();
                var result = window.Result;
                switch (result)
                {
                    case DialogBoxResult.Ok:
                        ret = DirectoryCopierFileCopyMode.Copy;
                        break;
                    case DialogBoxResult.No:
                        ret = DirectoryCopierFileCopyMode.DoNotCopy;
                        break;
                    case DialogBoxResult.YesAll:
                        ret = DirectoryCopierFileCopyMode.CopyAll;
                        break;
                    case DialogBoxResult.Cancel:
                        ret = DirectoryCopierFileCopyMode.DoNotCopy;
                        break;
                }

                b = true;
                return ret;
            });

            while (!b) Thread.Sleep(100);
            return ret;
        }
    }
}
