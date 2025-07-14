// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.FileSystems;
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Brokers.Processes;
using GitHubCommitVerifier.Clients;
using GitHubCommitVerifier.Services.GitSignings;
using GitHubCommitVerifier.Services.Foundations.Processes;
using GitHubCommitVerifier.Services.Foundations.FileSystems;

namespace GitHubCommitVerifier;

internal class Program
{
    static async Task Main(string[] args)
    {
        var loggingBroker = new LoggingBroker();
        var processBroker = new ProcessBroker();
        var fileSystemBroker = new FileSystemBroker();

        var processService = new ProcessService(
            processBroker,
            loggingBroker);

        var fileSystemService = new FileSystemService(
            fileSystemBroker,
            loggingBroker);

        var gitSigningOrchestrationService = new GitSigningOrchestrationService(
            processService,
            fileSystemService,
            loggingBroker);

        var client = new GitSigningClient(
            gitSigningOrchestrationService,
            loggingBroker);

        await client.ExecuteAsync(args);
    }
}
