using GitHubCommitVerifier.Brokers.FileSystems;
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Brokers.Processes;
using GitHubCommitVerifier.Clients;
using GitHubCommitVerifier.Services.GitSignings;

namespace GitHubCommitVerifier;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        var loggingBroker = new LoggingBroker();
        var processBroker = new ProcessBroker();
        var fileSystemBroker = new FileSystemBroker();
        var gitSigningService = new GitSigningService(processBroker, fileSystemBroker, loggingBroker);
        var client = new GitSigningClient(gitSigningService, loggingBroker);

        return await client.ExecuteAsync(args);
    }
}
