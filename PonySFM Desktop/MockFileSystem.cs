using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public enum MockFileType
    {
        FILE,
        DIRECTORY,
    }

    public class MockFile
    {
        private string _path;
        private MockFileType _fileType;

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

        public MockFile(string path, MockFileType fileType)
        {
            _path = path;
            _fileType = fileType;
        }
    }

    public class MockFileSystem : IFileSystem
    {
        private List<MockFile> files = new List<MockFile>();

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
    }
}
