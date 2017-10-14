using System.Collections.Generic;
using System.Windows;
using CoreLib;
using MahApps.Metro.Controls;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for VerificationWindow.xaml
    /// </summary>
    public partial class VerificationWindow : MetroWindow
    {
        private readonly RevisionManager _revisionManager;
        private readonly VerificationPresenter _presenter;
        private bool _showDetails;

        public VerificationWindow(List<int> ids, RevisionManager revisionManager)
        {
            _presenter = new VerificationPresenter(revisionManager, ids) {View = this};
            _revisionManager = revisionManager;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var failedIDs = await _presenter.Execute();
            if (failedIDs.Count == 0) return;

            var answer = MessageBox.Show($"{failedIDs.Count} revisions failed to validate. Reinstall them?", "Validation failed", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

            if(answer == MessageBoxResult.Yes)
            {
                var deinstallationWindow = new DeinstallationWindow(_revisionManager, failedIDs, true);
                deinstallationWindow.ShowDialog();

                var installationWindow = new InstallationWindow(failedIDs, _revisionManager, true);
                installationWindow.ShowDialog();

                Close();
            }
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
