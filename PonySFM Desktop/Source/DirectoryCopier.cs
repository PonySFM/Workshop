using System.IO;
using System.Collections.Generic;

namespace PonySFM_Desktop
{
    public class DirectoryCopier
    {
        IFileSystem _fs;
        string _source;
        string _dest;
        bool _copySubDirs;

        public delegate void FileExistsHandler(object sender, DirectoryCopierFileExistsEventArgs e);
        public event FileExistsHandler OnFileExists;

        public DirectoryCopier(IFileSystem fs, string source, string dest, bool copySubDirs)
        {
            _fs = fs;
            _source = source;
            _dest = dest;
            _copySubDirs = copySubDirs;
        }

        public void Execute()
        {
            CopyDirectory(_source, _dest, _copySubDirs);
        }

        private void CopyDirectory(string source, string dest, bool copySubDirs)
        {
            List<IFile> sourceFiles = _fs.GetFiles(source);

            if (!_fs.DirectoryExists(dest))
                _fs.CreateDirectory(dest);

            foreach (var file in sourceFiles)
            {
                string newPath = Path.Combine(dest, file.Name);

                if (!_fs.FileExists(newPath))
                    _fs.CopyFile(file.Path, newPath);
                else
                    if (FireFileExistsEvent(file.Path, newPath))
                        _fs.CopyFile(file.Path, newPath);
            }

            if (copySubDirs)
            {
                var dirs = _fs.GetDirectories(source);
                foreach (var dir in dirs)
                {
                    string newDirPath = Path.Combine(dest, dir.Name);
                    CopyDirectory(dir.Path, newDirPath, true);
                }
            }
        }

        private bool FireFileExistsEvent(string file1, string file2)
        {
            var eventArgs = new DirectoryCopierFileExistsEventArgs(file1, file2);
            OnFileExists(this, eventArgs);

            return eventArgs.ShouldCopy;
        }
    }

    public class DirectoryCopierFileExistsEventArgs
    {
        public string File1 { get; private set; }
        public string File2 { get; private set; }

        public bool ShouldCopy { get; set; }

        public DirectoryCopierFileExistsEventArgs(string file1, string file2)
        {
            File1 = file1;
            File2 = file2;
        }
    }
}
