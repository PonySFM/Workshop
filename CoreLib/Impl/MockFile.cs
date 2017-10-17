using CoreLib.Interface;

namespace CoreLib.Impl
{
    public class MockFile : IFile
    {
        public string Name => System.IO.Path.GetFileName(Path);

        public string Path { get; set; }
        public MockFileType FileType { get; set; }
        public byte[] Data { get; set; }

        public MockFile(string path, MockFileType fileType, byte[] data = null)
        {
            Path = path;
            FileType = fileType;
            Data = data;
        }

        public bool IsDirectory() => FileType == MockFileType.Directory;
        public bool IsFile() => FileType == MockFileType.File;
    }
}