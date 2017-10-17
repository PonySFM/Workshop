using System.Windows;
using MahApps.Metro.Controls;

namespace PonySFM_Workshop.DialogBox
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogWindow : MetroWindow
    {
        public DialogWindow(string title, string entry)
        {
            InitializeComponent();
            Result = DialogBox.DialogResult.Cancel;
            DialogLabel.Text = entry;
            Title = title;
        }

        public DialogResult Result { get; private set; }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(DialogBox.DialogResult.Yes);
        }

        private void YesAllButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(DialogBox.DialogResult.YesAll);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(DialogBox.DialogResult.No);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SetDialogResult(DialogBox.DialogResult.Cancel);
        }

        private void SetDialogResult(DialogResult result)
        {
            Result = result;
            Close();
        }
    }
}
