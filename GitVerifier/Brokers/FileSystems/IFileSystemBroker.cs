// Copyright (c) The Standard Organization. All rights reserved.
namespace GitHubCommitVerifier.Brokers.FileSystems;

public interface IFileSystemBroker
{
    bool FileExists(string path);
    ValueTask<string> ReadFileAsync(string path);
    ValueTask WriteFileAsync(string path, string content);
    void DeleteFile(string path);
    void CreateDirectory(string? path);
}
