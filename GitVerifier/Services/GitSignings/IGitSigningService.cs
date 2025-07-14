// Copyright (c) The Standard Organization. All rights reserved.
namespace GitHubCommitVerifier.Services.GitSignings;

public interface IGitSigningService
{
    ValueTask CheckGitSigningStatusAsync();
    ValueTask SetupSSHSigningAsync(string userName, string userEmail);
    ValueTask VerifySigningSetupAsync();
    ValueTask ResetSSHSigningAsync();
}
