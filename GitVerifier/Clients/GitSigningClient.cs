using GitHubCommitVerifier.Services.GitSignings;
using GitHubCommitVerifier.Brokers.Loggings;

namespace GitHubCommitVerifier.Clients;

public class GitSigningClient
{
    private readonly IGitSigningService gitSigningService;
    private readonly ILoggingBroker loggingBroker;

    public GitSigningClient(IGitSigningService gitSigningService, ILoggingBroker loggingBroker)
    {
        this.gitSigningService = gitSigningService;
        this.loggingBroker = loggingBroker;
    }

    public async ValueTask<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            loggingBroker.Log("Usage: dotnet GithubVerify [check|setup|verify|reset] [username] [email]");
            return 1;
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
                loggingBroker.Log("Invalid command. Use 'check', 'setup', 'verify', or 'reset'.");
                return 1;
        }

        return 0;
    }
}
