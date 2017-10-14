using System.Windows;
using System.Windows.Forms;
using MahApps.Metro.Controls;

namespace PonySFM_Workshop.DialogBox
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

    public static class DialogSystem
    {
        public static DialogResult Show(string title, string entry)
        {
            var dialogWindow = new DialogWindow(title, entry);
            dialogWindow.ShowDialog();
            return dialogWindow.Result;
        }

        public static string ShowDirectoryDialog(string currentDir = null)
        {
            var dialog = new FolderBrowserDialog();
            if (!string.IsNullOrEmpty(currentDir))
                dialog.SelectedPath = currentDir;
            return dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    }
}
