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
    /// Interaction logic for DeinstallationWindow.xaml
    /// </summary>
    public partial class DeinstallationWindow : Window
    {
        DeinstallationPresenter _presenter;
        bool _closeOnFinish;

        public DeinstallationWindow(RevisionManager revisionManager, List<int> ids, bool closeOnFinish = false)
        {
            _presenter = new DeinstallationPresenter(revisionManager, ids);
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
    }
}
