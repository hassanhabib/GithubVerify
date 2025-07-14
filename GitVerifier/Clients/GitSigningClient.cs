// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Services.Orchestrations.GitSignings;

namespace GitHubCommitVerifier.Clients;

public class GitSigningClient
{
    private readonly IGitSigningCommandService commandService;

    public GitSigningClient(IGitSigningCommandService commandService)
    {
        this.commandService = commandService;
    }

    public async ValueTask ExecuteAsync(string[] args) =>
        await commandService.ProcessCommandAsync(args);
}
