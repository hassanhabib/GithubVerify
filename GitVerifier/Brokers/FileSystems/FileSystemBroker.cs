namespace GitHubCommitVerifier.Brokers.FileSystems;

public class FileSystemBroker : IFileSystemBroker
{
    public bool FileExists(string path) => File.Exists(path);

    public async ValueTask<string> ReadFileAsync(string path) =>
        await File.ReadAllTextAsync(path);

    public async ValueTask WriteFileAsync(string path, string content) =>
        await File.WriteAllTextAsync(path, content);

    public void DeleteFile(string path)
    {
        if (FileExists(path))
        {
            File.Delete(path);
        }
    }

    public void CreateDirectory(string? path)
    {
        if (path is not null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        }
    }
}
