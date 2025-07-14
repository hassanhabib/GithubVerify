// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Brokers.Processes;

namespace GitHubCommitVerifier.Services.Foundations.Processes;

public class ProcessService : IProcessService
{
    private readonly IProcessBroker processBroker;
    private readonly ILoggingBroker loggingBroker;

    public ProcessService(
        IProcessBroker processBroker,
        ILoggingBroker loggingBroker)
    {
        this.processBroker = processBroker;
        this.loggingBroker = loggingBroker;
    }

    public async ValueTask<string> ExecuteCommandAsync(string command, string arguments)
    {
        return await processBroker.ExecuteCommandAsync(command, arguments);
    }

    public async ValueTask<string> ExecuteGitCommandAsync(string arguments)
    {
        return await processBroker.ExecuteGitCommandAsync(arguments);
    }
}
