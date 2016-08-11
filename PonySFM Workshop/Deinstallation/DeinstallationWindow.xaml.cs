using System.Collections.Generic;
using System.Windows;

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
