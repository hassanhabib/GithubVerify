// Copyright (c) The Standard Organization. All rights reserved.
namespace GitHubCommitVerifier.Services.Orchestrations.GitSignings;

public interface IGitSigningOrchestrationService
{
    ValueTask ProcessCommandAsync(string[] args);
}
