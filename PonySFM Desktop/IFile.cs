namespace PonySFM_Desktop
{
    public interface IFile
    {
        string Path { get; }
        string Name { get; }
        bool IsDirectory();
        bool IsFile();
    }
}
