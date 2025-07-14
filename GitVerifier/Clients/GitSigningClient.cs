// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Services.Orchestrations.GitSignings;

namespace GitHubCommitVerifier.Clients;

public class GitSigningClient
{
    private readonly IGitSigningOrchestrationService orchestrationService;

    public GitSigningClient(IGitSigningOrchestrationService orchestrationService)
    {
        this.orchestrationService = orchestrationService;
    }

    public async ValueTask ExecuteAsync(string[] args) =>
        await orchestrationService.ProcessCommandAsync(args);
}
