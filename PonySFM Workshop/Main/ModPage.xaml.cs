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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CoreLib;

namespace PonySFM_Workshop.Main
{
    /// <summary>
    /// Interaction logic for ModPage.xaml
    /// </summary>
    public partial class ModPage : Page
    {
        private readonly ModPagePresenter _presenter;
        private readonly RevisionManager _revisionManager;
        private readonly Revision _revision;

        public ModPage(RevisionManager revisionManager, Revision revision)
        {
            _revisionManager = revisionManager;
            _revision = revision;
            _presenter = new ModPagePresenter(revision);
            _presenter.View = this;
            InitializeComponent();
        }

        private void BackBtn_OnClick(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void UninstallBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var w = new Deinstallation.DeinstallationWindow(_revisionManager, new List<int>{ _revision.ID }, true);
            w.ShowDialog();
            MainWindow.Instance.RefreshListData();
            GoBack();
        }

        private void VerifyBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var w = new Verification.VerificationWindow(new List<int>{ _revision.ID }, _revisionManager);
            w.ShowDialog();
            MainWindow.Instance.RefreshListData();
        }

        private void GoBack()
        {
            MainWindow.Instance.SetPage("MainPage");
        }
    }
}
