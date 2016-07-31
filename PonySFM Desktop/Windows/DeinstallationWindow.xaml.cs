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
    /// Interaction logic for DeinstallationWindow.xaml
    /// </summary>
    public partial class DeinstallationWindow : Window
    {
        DeinstallationPresenter _presenter;

        public DeinstallationWindow(RevisionManager revisionManager, List<int> ids)
        {
            _presenter = new DeinstallationPresenter(revisionManager, ids);
            _presenter.View = this;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _presenter.Execute();
        }
    }
}
