using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace PonySFM_Desktop
{
    public enum MockFileType
    {
        File, Directory,
    }

    //TODO: Consider using FileInfo, since it covers both directories and files.
    public class MockFile : IFile
    {
        private string _path;
        private MockFileType _fileType;
        private byte[] _data;

        public string Path
        {
            get
            {
                return _path;
            }

            set
            {
                _path = value;
            }
        }

        public string Name
        {
            get
            {
                return System.IO.Path.GetFileName(_path);
            }
        }

        public MockFileType FileType
        {
            get
            {
                return _fileType;
            }

            set
            {
                _fileType = value;
            }
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        public MockFile(string path, MockFileType fileType, byte[] data = null)
        {
            _path = path;
            _fileType = fileType;
            _data = data;
        }

        public bool IsDirectory()
        {
            return _fileType == MockFileType.Directory;
        }

        public bool IsFile()
        {
            return _fileType == MockFileType.File;
        }
    }

    //TODO: Optimize MockFileSystem.
    // Simply because I think quite a bit could be optimized.
    public class MockFileSystem : IFileSystem
    {
        private List<MockFile> files = new List<MockFile>();

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

        public bool FileExists(string path)
        {
            return EntryExists(path, MockFileType.File);
        }

        public void AddFile(MockFile file)
        {
            files.Add(file);
        }

        private bool EntryExists(string path, MockFileType type)
        {
            foreach (var file in files)
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
            return files.Contains(file);
        }

        public MockFile GetEntryByPath(string path)
        {
            foreach (var file in files)
            {
                if (file.Path == path)
                    return file;
            }
            return null;
        }

        public IEnumerable<MockFile> GetEntries(string path)
        {
            foreach (var file in files)
            {
                if (file.Path == path)
                    yield return file;
            }
        }

        public void DeleteEntry(MockFile item)
        {
            files.Remove(item);
        }

        public void DeleteEntry(string path)
        {
            foreach (var file in files)
            {
                if (file.Path == path)
                {
                    files.Remove(file);
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

        public XmlDocument OpenXML(string filepath)
        {
            var doc = new XmlDocument();
            var data = ReadFile(filepath);
            string str = Encoding.UTF8.GetString(data);
            doc.LoadXml(str);

            return doc;
        }

        public void SaveXML(XmlDocument doc, string filepath)
        {
            MemoryStream memStream = new MemoryStream();
            doc.Save(memStream);

            string str = Encoding.UTF8.GetString(memStream.ToArray());
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
            List<IFile> ret = new List<IFile>();

            if (!dir.EndsWith("\\"))
                dir = dir + "\\";

            foreach (var file in files)
            {
                if (file.IsFile() && file.Path.StartsWith(dir) && !file.Path.Trim(dir.ToCharArray()).Contains("\\"))
                {
                    ret.Add(file);
                }
            }

            return ret;
        }

        public List<IFile> GetDirectories(string dir)
        {
            List<IFile> ret = new List<IFile>();

            if (!dir.EndsWith("\\"))
                dir = dir + "\\";

            foreach (var file in files)
            {
                if (file.IsDirectory() && file.Path.StartsWith(dir) && !file.Path.Trim(dir.ToCharArray()).Contains("\\"))
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

            foreach (var file in files)
            {
                if (file.IsFile() && file.Path.StartsWith(dir) &&
                    !file.Path.Trim(dir.ToCharArray()).Contains("\\"))
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

            foreach (var file in files)
            {
                if (file.IsDirectory() && file.Path.StartsWith(dir) &&
                    !file.Path.Trim(dir.ToCharArray()).Contains("\\"))
                {
                    yield return file;
                }
            }
        }

        public void DeleteFile(string filepath)
        {
            var file = files.Find(f => f.Path == filepath && f.IsFile());
            if (file != null)
                files.Remove(file);
        }

        public void DeleteDirectory(string filepath)
        {
            var file = files.Find(f => f.Path == filepath && f.IsDirectory());
            if (file != null)
                files.Remove(file);
        }
    }
}
