// Copyright (c) The Standard Organization. All rights reserved.
namespace GitHubCommitVerifier.Services.GitSignings;

public interface IGitSigningOrchestrationService
{
    ValueTask CheckGitSigningStatusAsync();
    ValueTask SetupSSHSigningAsync(string userName, string userEmail);
    ValueTask VerifySigningSetupAsync();
    ValueTask ResetSSHSigningAsync();
}
