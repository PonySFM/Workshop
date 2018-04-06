using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class MockFileSystem : IFileSystem
    {
        private readonly List<MockFile> _files = new List<MockFile>();
        private readonly Random _random;

        public MockFileSystem()
        {
            _random = new Random();
        }

        /* TODO: should overwrite be default behaviour? */
        public void CopyFile(string src, string dest)
        {
            var file = GetEntryByPath(src);
            if (file != null)
            {
                var copy = new MockFile(dest, file.FileType, file.Data);
                DeleteFile(dest);
                AddFile(copy);
            }
        }

        public bool DirectoryExists(string path)
        {
            return EntryExists(path, MockFileType.Directory);
        }

        public long GetFileSize(string path)
        {
            var entry = GetEntryByPath(path);
            return entry?.Data?.Length ?? 0;
        }

        public bool FileExists(string path)
        {
            return EntryExists(path, MockFileType.File);
        }

        public void AddFile(MockFile file)
        {
            _files.Add(file);
        }

        private bool EntryExists(string path, MockFileType type)
        {
            foreach (var file in _files)
            {
                if (file.Path == path && file.FileType == type)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Verifies if an entry exists within the file list.
        /// </summary>
        /// <param name="file">Entry file.</param>
        /// <returns>Returns true if found.</returns>
        private bool EntryExists(MockFile file)
        {
            return _files.Contains(file);
        }

        private MockFile GetEntryByPath(string path)
        {
            foreach (var file in _files)
            {
                if (file.Path == path)
                    return file;
            }
            return null;
        }

        public IEnumerable<MockFile> GetEntries(string path)
        {
            return _files.Where(file => file.Path == path);
        }

        public void DeleteEntry(MockFile item)
        {
            _files.Remove(item);
        }

        private void DeleteEntry(string path)
        {
            foreach (var file in _files)
            {
                if (file.Path == path)
                {
                    _files.Remove(file);
                    return;
                }
            }
        }

        /* TODO: should overwrite be default behaviour? */
        public void CreateFile(string path, byte[] data = null)
        {
            if (EntryExists(path, MockFileType.File))
                DeleteEntry(path);
            AddFile(new MockFile(path, MockFileType.File, data));
        }

        public void CreateDirectory(string path)
        {
            AddFile(new MockFile(path, MockFileType.Directory, null));
        }

        public byte[] ReadFile(string path)
        {
            if (!EntryExists(path, MockFileType.File))
                return null;

            return GetEntryByPath(path).Data;
        }

        public XmlDocument OpenXml(string filepath)
        {
            var doc = new XmlDocument();
            var data = ReadFile(filepath);
            var str = Encoding.UTF8.GetString(data);
            doc.LoadXml(str);

            return doc;
        }

        public void SaveXml(XmlDocument doc, string filepath)
        {
            var memStream = new MemoryStream();
            doc.Save(memStream);

            memStream.Flush();
            memStream.Position = 0;

            CreateFile(filepath, memStream.ToArray());
        }


        public string GetChecksum(string filepath)
        {
            var data = ReadFile(filepath);

            return data == null ?
                string.Empty : FileUtil.GetChecksum(new MemoryStream(data));
        }

        public List<IFile> GetFiles(string dir)
        {
            var ret = new List<IFile>();

            if (!dir.EndsWith("\\"))
                dir = dir + "\\";

            foreach (var file in _files)
            {
                var s = file.Path.Replace(dir, "");
                if (file.IsFile() && file.Path.StartsWith(dir) && !s.Contains("\\"))
                {
                    ret.Add(file);
                }
            }

            return ret;
        }

        public List<IFile> GetDirectories(string dir, bool recursive = false)
        {
            var ret = new List<IFile>();

            if (!dir.EndsWith("\\"))
                dir = dir + "\\";

            foreach (var file in _files)
            {
                var cond = file.IsDirectory() && file.Path.StartsWith(dir);
                if (!recursive)
                    cond = cond && !file.Path.Trim(dir.ToCharArray()).Contains("\\");
                if (cond)
                {
                    ret.Add(file);
                }
            }

            return ret;
        }

        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of files within the specified directory.
        /// </summary>
        /// <param name="dir">Directory path.</param>
        /// <returns>A file enumeration.</returns>
        /// <remarks>
        /// Named GetFileEnumeration to not to break comptability with <see cref="GetFiles(string)"/>
        /// </remarks>
        public IEnumerable<IFile> GetFileEnumeration(string dir)
        {
            if (!dir.EndsWith("\\"))
                dir = dir + "\\";

            foreach (var file in _files)
            {
                var s = file.Path.Replace(dir, "");
                if (file.IsFile() && file.Path.StartsWith(dir) &&
                    !s.Contains("\\"))
                {
                    yield return file;
                }
            }
        }

        /// <summary>
        /// Get an <see cref="IEnumerable{T}"/> of directories within the specified directory.
        /// </summary>
        /// <param name="dir">Directory path.</param>
        /// <returns>A directory enumeration.</returns>
        public IEnumerable<IFile> GetDirectoryEnumeration(string dir)
        {
            if (!dir.EndsWith("\\"))
                dir = dir + "\\";

            foreach (var file in _files)
            {
                var s = file.Path.Replace(dir, "");
                if (file.IsDirectory() && file.Path.StartsWith(dir) &&
                    !s.Contains("\\"))
                {
                    yield return file;
                }
            }
        }

        public void DeleteFile(string filepath)
        {
            var file = _files.Find(f => f.Path == filepath && f.IsFile());
            if (file != null)
                _files.Remove(file);
        }

        public void DeleteDirectory(string filepath)
        {
            foreach (var file in GetFiles(filepath))
            {
                DeleteFile(file.Path);
            }

            foreach (var dir in GetDirectories(filepath))
            {
                DeleteDirectory(dir.Path);
            }

            var direntry = _files.Find(f => f.Path == filepath && f.IsDirectory());
            if (direntry != null)
                _files.Remove(direntry);
        }

        public Task CopyFileAsync(string src, string dest)
        {
            return Task.Factory.StartNew(() => {
                CopyFile(src, dest);
            });
        }

        public IZipFile OpenZip(string filepath)
        {
            return new MockZipFile(filepath, this);
        }

        public string GetTempPath()
        {
            return "C:\\TEMP" + _random.Next();
        }
    }
}
