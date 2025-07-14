// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.FileSystems;

namespace GitHubCommitVerifier.Services.Foundations.FileSystems;

public interface IFileSystemService
{
    bool FileExists(string path);
    ValueTask<string> ReadFileAsync(string path);
    ValueTask WriteFileAsync(string path, string content);
    void DeleteFile(string path);
    void CreateDirectory(string? path);
}
