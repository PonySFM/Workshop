namespace PonySFM_Workshop
{
    public interface IFile
    {
        string Path { get; }
        string Name { get; }
        bool IsDirectory();
        bool IsFile();
    }
}
