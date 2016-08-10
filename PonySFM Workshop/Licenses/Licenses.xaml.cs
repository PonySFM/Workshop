using PonySFM_Workshop.Source;
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

namespace PonySFM_Workshop.License
{
    /// <summary>
    /// Interaction logic for Licenses.xaml
    /// </summary>
    public partial class Licenses : Page
    {
        SettingsPresenter _presenter;

        public Licenses()
        {
            InitializeComponent();
        }

        public Licenses(ConfigHandler config)
        {
            _presenter = new SettingsPresenter(config);
            _presenter.View = this;
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
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
