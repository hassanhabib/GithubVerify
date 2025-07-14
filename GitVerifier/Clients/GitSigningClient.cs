// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Services.GitSignings;

namespace GitHubCommitVerifier.Clients;

public class GitSigningClient
{
    private readonly IGitSigningOrchestrationService orchestrationService;
    private readonly ILoggingBroker loggingBroker;

    public GitSigningClient(
        IGitSigningOrchestrationService orchestrationService,
        ILoggingBroker loggingBroker)
    {
        this.orchestrationService = orchestrationService;
        this.loggingBroker = loggingBroker;
    }

    public async ValueTask ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            loggingBroker.Log(
                "Usage: dotnet GithubVerify [check|setup|verify|reset] [username] [email]");

            return;
        }

        string command = args[0].ToLower();
        string userName = args.Length > 1 ? args[1] : Environment.UserName;
        string userEmail = args.Length > 2 ? args[2] : "your_email@example.com";

        switch (command)
        {
            case "check":
                await orchestrationService.CheckGitSigningStatusAsync();
                break;
            case "setup":
                await orchestrationService.SetupSSHSigningAsync(userName, userEmail);
                break;
            case "verify":
                await orchestrationService.VerifySigningSetupAsync();
                break;
            case "reset":
                await orchestrationService.ResetSSHSigningAsync();
                break;
            default:
                loggingBroker.Log(
                    "Invalid command. Use 'check', 'setup', 'verify', or 'reset'.");
                break;
        }
    }
}
