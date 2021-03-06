﻿using System.Collections.Generic;
using System.Windows;
using CoreLib;
using MahApps.Metro.Controls;

namespace PonySFM_Workshop.Deinstallation
{
    /// <summary>
    /// Interaction logic for DeinstallationWindow.xaml
    /// </summary>
    public partial class DeinstallationWindow : MetroWindow
    {
        private readonly DeinstallationPresenter _presenter;
        private readonly bool _closeOnFinish;
        private bool _showDetails;

        public DeinstallationWindow(RevisionManager revisionManager, List<int> ids, bool closeOnFinish = false)
        {
            _presenter = new DeinstallationPresenter(revisionManager, ids) {View = this};
            _closeOnFinish = closeOnFinish;
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await _presenter.Execute();
            if (_closeOnFinish)
                Close();
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
