using System.Windows.Forms;

namespace PonySFM_Workshop.DialogBox
{
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