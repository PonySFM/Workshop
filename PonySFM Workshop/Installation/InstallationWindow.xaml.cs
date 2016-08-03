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

        public bool OnFileExists(string file1, string file2)
        {
            var msg = MessageBox.Show(string.Format("The file {0} already exists. Overwrite?", file2), "Installation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return msg == MessageBoxResult.Yes;
        }
    }
}
