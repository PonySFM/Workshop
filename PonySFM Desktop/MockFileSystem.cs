using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PonySFM_Desktop
{
    public enum MockFileType
    {
        FILE,
        DIRECTORY,
    }

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
            return _fileType == MockFileType.DIRECTORY;
        }

        public bool IsFile()
        {
            return _fileType == MockFileType.FILE;
        }
    }

    public class MockFileSystem : IFileSystem
    {
        private List<MockFile> files = new List<MockFile>();

        /* TODO: should overwrite be default behaviour? */
        public void CopyFile(string src, string dest)
        {
            var file = GetEntry(src);
            if (file != null)
            {
                var copy = new MockFile(dest, file.FileType, file.Data);
                DeleteFile(dest);
                AddFile(copy);
            }
        }

        public bool DirectoryExists(string path)
        {
            return EntryExists(path, MockFileType.DIRECTORY);
        }

        public bool FileExists(string path)
        {
            return EntryExists(path, MockFileType.FILE);
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

        public MockFile GetEntry(string path)
        {
            foreach (var file in files)
            {
                if (file.Path == path)
                    return file;
            }
            return null;
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
            if (EntryExists(path, MockFileType.FILE))
                DeleteEntry(path);
            AddFile(new MockFile(path, MockFileType.FILE, data));
        }

        public void CreateDirectory(string path)
        {
            AddFile(new MockFile(path, MockFileType.DIRECTORY, null));
        }

        public byte[] ReadFile(string path)
        {
            if (!EntryExists(path, MockFileType.FILE))
                return null;

            return GetEntry(path).Data;
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
            if (data == null)
            {
                return string.Empty;
            }
            else
            {
                var stream = new MemoryStream(data);
                return FileUtil.GetChecksum(stream);
            }
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
