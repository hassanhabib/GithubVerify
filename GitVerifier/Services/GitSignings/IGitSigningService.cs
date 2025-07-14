namespace GitHubCommitVerifier.Services.GitSignings;

public interface IGitSigningService
{
    ValueTask CheckGitSigningStatusAsync();
    ValueTask SetupSSHSigningAsync(string userName, string userEmail);
    ValueTask VerifySigningSetupAsync();
    ValueTask ResetSSHSigningAsync();
}
