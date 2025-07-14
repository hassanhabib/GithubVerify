using GitHubCommitVerifier.Brokers.FileSystems;
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Brokers.Processes;
using GitHubCommitVerifier.Clients;
using GitHubCommitVerifier.Services.GitSignings;
using GitHubCommitVerifier.Services.Orchestrations.GitSignings;

namespace GitHubCommitVerifier;

internal class Program
{
    static async Task Main(string[] args)
    {
        var loggingBroker = new LoggingBroker();
        var processBroker = new ProcessBroker();
        var fileSystemBroker = new FileSystemBroker();
        var gitSigningService = new GitSigningService(
            processBroker,
            fileSystemBroker,
            loggingBroker);
        var orchestrationService = new GitSigningOrchestrationService(
            gitSigningService,
            loggingBroker);
        var client = new GitSigningClient(orchestrationService);

        await client.ExecuteAsync(args);
    }
}
