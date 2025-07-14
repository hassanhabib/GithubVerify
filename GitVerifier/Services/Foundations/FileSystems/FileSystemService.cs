// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.FileSystems;
using GitHubCommitVerifier.Brokers.Loggings;

namespace GitHubCommitVerifier.Services.Foundations.FileSystems;

public class FileSystemService : IFileSystemService
{
    private readonly IFileSystemBroker fileSystemBroker;
    private readonly ILoggingBroker loggingBroker;

    public FileSystemService(
        IFileSystemBroker fileSystemBroker,
        ILoggingBroker loggingBroker)
    {
        this.fileSystemBroker = fileSystemBroker;
        this.loggingBroker = loggingBroker;
    }

    public bool FileExists(string path) =>
        fileSystemBroker.FileExists(path);

    public async ValueTask<string> ReadFileAsync(string path) =>
        await fileSystemBroker.ReadFileAsync(path);

    public async ValueTask WriteFileAsync(string path, string content) =>
        await fileSystemBroker.WriteFileAsync(path, content);

    public void DeleteFile(string path) =>
        fileSystemBroker.DeleteFile(path);

    public void CreateDirectory(string? path) =>
        fileSystemBroker.CreateDirectory(path);
}
