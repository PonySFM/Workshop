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
    public enum DialogBoxResult
    {
        Yes, YesAll, Ok, No, Cancel
    }

    [Flags]
    public enum EnableButtons
    {
        Yes, YesAll, Ok, No, Cancel
    }

    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : MetroWindow
    {
        public DialogBox(string text)
        {
            Text = text;
            Result = DialogBoxResult.Cancel;
            InitializeComponent();
        }

        public DialogBoxResult Result { get; private set; }
        public string Text { get; private set; }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(DialogBoxResult.Yes);
        }

        private void YesAllButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(DialogBoxResult.YesAll);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(DialogBoxResult.No);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(DialogBoxResult.Cancel);
        }

        private void SetDialogResult(DialogBoxResult result)
        {
            Result = result;
            Close();
        }
    }
}
