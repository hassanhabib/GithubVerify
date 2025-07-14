namespace GitHubCommitVerifier.Brokers.Processes;

public interface IProcessBroker
{
    ValueTask<string> ExecuteCommandAsync(string command, string arguments);
    ValueTask<string> ExecuteGitCommandAsync(string arguments);
}
