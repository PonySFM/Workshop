namespace CoreLib.Interface
{
    public interface IFile
    {
        string Path { get; }
        string Name { get; }
        bool IsDirectory();
        bool IsFile();
    }
}
