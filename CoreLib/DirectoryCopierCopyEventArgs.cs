using System;

namespace CoreLib
{
    public class DirectoryCopierCopyEventArgs : EventArgs
    {
        public string File1 { get; }
        public string File2 { get; }

        public DirectoryCopierCopyEventArgs(string file1, string file2)
        {
            File1 = file1;
            File2 = file2;
        }
    }
}