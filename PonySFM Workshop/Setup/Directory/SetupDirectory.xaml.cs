using System.Windows;
using System.Windows.Controls;
using CoreLib;
using CoreLib.Impl;

namespace PonySFM_Workshop
{
    /// <summary>
    /// Interaction logic for SetupDirectory.xaml
    /// </summary>
    public partial class SetupDirectory : Page
    {
        private readonly ConfigHandler _config;

        public SetupDirectory(ConfigHandler config)
        {
            _config = config;
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var newPath = DialogSystem.ShowDirectoryDialog(DirectoryPathBox.Text);
            if (!string.IsNullOrEmpty(newPath))
                DirectoryPathBox.Text = newPath;
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            SetupWindow.Instance.PrevPage();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            var dir = DirectoryPathBox.Text;
            var parser = new SFMDirectoryParser(dir, WindowsFileSystem.Instance);
            var error = parser.Validate();

            if (error == SFMDirectoryParserError.NotExists)
            {
                MessageBox.Show("The path chosen is not a valid directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (error == SFMDirectoryParserError.NotLikely)
                if (MessageBox.Show("The directory does not seem like the typical SFM installation. Continue?",
                    "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                    return;

            _config.Write(new ConfigFile(parser.Path));

            var gameinfoHandler = new GameinfoHandler(parser.GameinfoPath, WindowsFileSystem.Instance);
            var ret = gameinfoHandler.Execute();

            /* TODO: maybe display a bit more useful error message including troubleshooting issues */
            if (ret == GameinfoHandlerError.FileInvalid)
                MessageBox.Show("Failed to add entry to gameinfo.txt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            SetupWindow.Instance.GoToMainWindow();
        }

        private void DirectoryPathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnNext.IsEnabled = !string.IsNullOrWhiteSpace(DirectoryPathBox.Text);
        }
    }
}
