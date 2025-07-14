// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.Processes;

namespace GitHubCommitVerifier.Services.Foundations.Processes;

public interface IProcessService
{
    ValueTask<string> ExecuteCommandAsync(string command, string arguments);
    ValueTask<string> ExecuteGitCommandAsync(string arguments);
}
