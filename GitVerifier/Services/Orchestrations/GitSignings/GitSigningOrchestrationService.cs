// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Services.GitSignings;

namespace GitHubCommitVerifier.Services.Orchestrations.GitSignings;

public class GitSigningOrchestrationService : IGitSigningOrchestrationService
{
    private readonly IGitSigningService gitSigningService;
    private readonly ILoggingBroker loggingBroker;

    public GitSigningOrchestrationService(
        IGitSigningService gitSigningService,
        ILoggingBroker loggingBroker)
    {
        this.gitSigningService = gitSigningService;
        this.loggingBroker = loggingBroker;
    }

    public async ValueTask ProcessCommandAsync(string[] args)
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
                await gitSigningService.CheckGitSigningStatusAsync();
                break;
            case "setup":
                await gitSigningService.SetupSSHSigningAsync(userName, userEmail);
                break;
            case "verify":
                await gitSigningService.VerifySigningSetupAsync();
                break;
            case "reset":
                await gitSigningService.ResetSSHSigningAsync();
                break;
            default:

                loggingBroker.Log(
                    "Invalid command. Use 'check', 'setup', 'verify', or 'reset'.");

                break;
        }
    }
}
