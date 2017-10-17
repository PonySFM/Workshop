namespace CoreLib
{
    public class DirectoryCopierFileExistsEventArgs : DirectoryCopierCopyEventArgs
    {
        public DirectoryCopierFileCopyMode FileCopyMode { get; set; }

        public DirectoryCopierFileExistsEventArgs(string file1, string file2) : base(file1, file2)
        {
            /* This default value will only be used if no one registers to the OnFileCopy event */
            FileCopyMode = DirectoryCopierFileCopyMode.Copy;
        }
    }
}