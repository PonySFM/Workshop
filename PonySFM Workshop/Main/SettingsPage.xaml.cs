using System.Windows;
using System.Windows.Controls;
using CoreLib;
using PonySFM_Workshop.DialogBox;

namespace PonySFM_Workshop.Main
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private readonly SettingsPresenter _presenter;

        public SettingsPage(ConfigHandler config)
        {
            _presenter = new SettingsPresenter(config) {View = this};
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void DirectoryBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            var newPath = DialogSystem.ShowDirectoryDialog(DirectoryTextBox.Text);
            if (!string.IsNullOrEmpty(newPath))
                _presenter.SFMDirectory = newPath;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!_presenter.Save())
            {
                MessageBox.Show(_presenter.SaveError, "Error while saving", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GoBack();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _presenter.Reset();
            GoBack();
        }

        private static void GoBack()
        {
            MainWindow.Instance.SetPage("MainPage");
        }
    }
}
