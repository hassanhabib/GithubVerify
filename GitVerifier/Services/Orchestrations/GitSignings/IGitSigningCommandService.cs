// Copyright (c) The Standard Organization. All rights reserved.
namespace GitHubCommitVerifier.Services.Orchestrations.GitSignings;

public interface IGitSigningCommandService
{
    ValueTask ProcessCommandAsync(string[] args);
}
