using MahApps.Metro.Controls;
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
    public enum DialogResult
    {
        Yes, YesAll, Ok, No, Cancel
    }

    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogWindow : MetroWindow
    {
        public DialogWindow(string entry = "Are you sure?")
        {
            InitializeComponent();

            DialogLabel.Text = entry;
        }

        public DialogResult Result { get; private set; }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(PonySFM_Workshop.DialogResult.Yes);
        }

        private void YesAllButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(PonySFM_Workshop.DialogResult.YesAll);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(PonySFM_Workshop.DialogResult.No);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(PonySFM_Workshop.DialogResult.Cancel);
        }

        private void SetDialogResult(DialogResult result)
        {
            Result = result;
            Close();
        }
    }

    public static class DialogSystem
    {
        public static DialogResult Show()
        {
            return new DialogWindow().Result;
        }

        public static DialogResult Show(string entry)
        {
            return new DialogWindow(entry).Result;
        }
    }
}
