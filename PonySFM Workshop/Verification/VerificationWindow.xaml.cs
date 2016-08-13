using System.Collections.Generic;
using System.Windows;
using CoreLib;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for VerificationWindow.xaml
    /// </summary>
    public partial class VerificationWindow : Window
    {
        RevisionManager _revisionManager;
        VerificationPresenter _presenter;
        public VerificationWindow(List<int> ids, RevisionManager revisionManager)
        {
            _presenter = new VerificationPresenter(revisionManager, ids);
            _presenter.View = this;
            _revisionManager = revisionManager;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> failedIDS = await _presenter.Execute();
            if (failedIDS.Count > 0)
            {
                var answer = MessageBox.Show(string.Format("{0} revisions failed to validate. Reinstall them?", failedIDS.Count), "Validation failed", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);
                if(answer == MessageBoxResult.Yes)
                {
                    var deinstallationWindow = new DeinstallationWindow(_revisionManager, failedIDS, true);
                    deinstallationWindow.ShowDialog();

                    var installationWindow = new InstallationWindow(failedIDS, _revisionManager, true);
                    installationWindow.ShowDialog();

                    Close();
                }
            }
        }
    }
}
