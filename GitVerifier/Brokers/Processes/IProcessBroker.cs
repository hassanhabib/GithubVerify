// Copyright (c) The Standard Organization. All rights reserved.
namespace GitHubCommitVerifier.Brokers.Processes;

public interface IProcessBroker
{
    ValueTask<string> ExecuteCommandAsync(string command, string arguments);
    ValueTask<string> ExecuteGitCommandAsync(string arguments);
}
