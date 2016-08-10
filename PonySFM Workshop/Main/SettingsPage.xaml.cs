﻿using PonySFM_Workshop.Source;
using System.Windows;
using System.Windows.Controls;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        SettingsPresenter _presenter;

        public SettingsPage(ConfigHandler config)
        {
            _presenter = new SettingsPresenter(config);
            _presenter.View = this;
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void DirectoryBrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            var newPath = SFM.ShowDirectoryDialog(DirectoryTextBox.Text);
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

        private void GoBack()
        {
            MainWindow.Instance.SetPage("MainPage");
        }
    }
}
