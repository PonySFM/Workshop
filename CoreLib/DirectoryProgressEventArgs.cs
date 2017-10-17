using System;

namespace CoreLib
{
    public class DirectoryProgressEventArgs : EventArgs
    {
        public int Progress { get; }

        public DirectoryProgressEventArgs(int progress)
        {
            Progress = progress;
        }
    }
}