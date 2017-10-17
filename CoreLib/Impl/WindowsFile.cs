using System;
using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class WindowsFile : IFile
    {
        public string Name => System.IO.Path.GetFileName(Path);
        public string Path { get; }

        public WindowsFile(string path)
        {
            Path = path;
        }

        public bool IsDirectory()
        {
            throw new NotImplementedException();
        }

        public bool IsFile()
        {
            throw new NotImplementedException();
        }
    }
}