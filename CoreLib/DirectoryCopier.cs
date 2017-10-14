using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using CoreLib.Interface;
using System;
using System.Linq;

namespace CoreLib
{
    public class DirectoryCopier
    {
        private readonly IFileSystem _fs;
        private readonly string _source;
        private readonly string _dest;
        private readonly bool _copySubDirs;
        private int _progress;
        private int _totalFiles;
        private bool _copyAll;
        private CancellationToken _cancel;

        public delegate void FileExistsHandler(object sender, DirectoryCopierFileExistsEventArgs e);
        public event FileExistsHandler OnFileExists;

        public delegate void FileOnCopyHandler(object sender, DirectoryCopierCopyEventArgs e);
        public event FileOnCopyHandler OnFileCopy;

        public delegate void OnProgressHandler(object sender, DirectoryProgressEventArgs e);
        public event OnProgressHandler OnProgress;

        public DirectoryCopier(IFileSystem fs, string source, string dest, bool copySubDirs)
        {
            _fs = fs;
            _source = source;
            _dest = dest;
            _copySubDirs = copySubDirs;
            _progress = 0;
            _totalFiles = 0;
            _copyAll = false;
        }

        public async Task Execute(CancellationToken cancel = default(CancellationToken))
        {
            _cancel = cancel;
            _totalFiles = CountFiles(_source, _copySubDirs);
            await CopyDirectory(_source, _dest, _copySubDirs);
        }

        private async Task CopyDirectory(string source, string dest, bool copySubDirs)
        {
            _cancel.ThrowIfCancellationRequested();

            var sourceFiles = _fs.GetFiles(source);

            if (!_fs.DirectoryExists(dest))
                _fs.CreateDirectory(dest);

            foreach (var file in sourceFiles)
            {
                var newPath = Path.Combine(dest, file.Name);

                FireFileCopyEvent(file.Path, newPath);

                if (!_fs.FileExists(newPath))
                {
                    await _fs.CopyFileAsync(file.Path, newPath);
                }
                else
                {
                    if (!_copyAll)
                    {
                        var mode = FireFileExistsEvent(file.Path, newPath);
                        switch (mode)
                        {
                            case DirectoryCopierFileCopyMode.DoNotCopy:
                                continue;
                            case DirectoryCopierFileCopyMode.CopyAll:
                                _copyAll = true;
                                break;
                            case DirectoryCopierFileCopyMode.Copy:
                                break;
                            case DirectoryCopierFileCopyMode.Cancel:
                                throw new System.OperationCanceledException();
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    await _fs.CopyFileAsync(file.Path, newPath);
                }

                _progress++;
                FireProgressEvent((int)(_progress / (double)_totalFiles * 100.0));
            }

            if (copySubDirs)
            {
                var dirs = _fs.GetDirectories(source);
                foreach (var dir in dirs)
                {
                    var newDirPath = Path.Combine(dest, dir.Name);
                    await CopyDirectory(dir.Path, newDirPath, true);
                }
            }
        }

        private int CountFiles(string source, bool subdirs)
        {
            var ret = 0;

            var sourceFiles = _fs.GetFiles(source);

            ret += sourceFiles.Count;

            var dirs = _fs.GetDirectories(source);
            if (!subdirs) return ret;

            ret += dirs.Sum(dir => CountFiles(dir.Path, true));

            return ret;
        }

        private DirectoryCopierFileCopyMode FireFileExistsEvent(string file1, string file2)
        {
            var eventArgs = new DirectoryCopierFileExistsEventArgs(file1, file2);
            OnFileExists?.Invoke(this, eventArgs);
            return eventArgs.FileCopyMode;
        }

        private void FireFileCopyEvent(string file1, string file2)
        {
            var eventArgs = new DirectoryCopierCopyEventArgs(file1, file2);
            OnFileCopy?.Invoke(this, eventArgs);
        }

        private void FireProgressEvent(int progress)
        {
            var eventArgs = new DirectoryProgressEventArgs(progress);
            OnProgress?.Invoke(this, eventArgs);
        }
    }

    public class DirectoryProgressEventArgs : EventArgs
    {
        public int Progress { get; private set; }

        public DirectoryProgressEventArgs(int progress)
        {
            Progress = progress;
        }
    }

    public class DirectoryCopierCopyEventArgs : EventArgs
    {
        public string File1 { get; private set; }
        public string File2 { get; private set; }

        public DirectoryCopierCopyEventArgs(string file1, string file2)
        {
            File1 = file1;
            File2 = file2;
        }
    }

    public enum DirectoryCopierFileCopyMode
    {
        DoNotCopy,
        Copy,
        CopyAll,
        Cancel
    }

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
