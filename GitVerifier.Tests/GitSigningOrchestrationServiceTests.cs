// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Services.GitSignings;
using GitHubCommitVerifier.Services.Foundations.FileSystems;
using GitHubCommitVerifier.Services.Foundations.Processes;
using Moq;

namespace GitVerifier.Tests;

public class GitSigningOrchestrationServiceTests
{
    private readonly Mock<IProcessService> processServiceMock;
    private readonly Mock<IFileSystemService> fileSystemServiceMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly GitSigningOrchestrationService service;

    public GitSigningOrchestrationServiceTests()
    {
        this.processServiceMock = new Mock<IProcessService>();
        this.fileSystemServiceMock = new Mock<IFileSystemService>();
        this.loggingBrokerMock = new Mock<ILoggingBroker>();

        this.service = new GitSigningOrchestrationService(
            this.processServiceMock.Object,
            this.fileSystemServiceMock.Object,
            this.loggingBrokerMock.Object);
    }

    [Fact]
    public async Task ShouldCheckGitSigningStatusAsync()
    {
        // given
        this.processServiceMock.Setup(broker =>
            broker.ExecuteGitCommandAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

        // when
        await this.service.CheckGitSigningStatusAsync();

        // then
        this.processServiceMock.Verify(broker =>
            broker.ExecuteGitCommandAsync(
                "config --global --get commit.gpgsign"),
        Times.Once);

        this.processServiceMock.Verify(broker =>
            broker.ExecuteGitCommandAsync(
                "config --global --get gpg.format"),
        Times.Once);
    }

    [Fact]
    public async Task ShouldGenerateKeyIfMissingOnSetupAsync()
    {
        // given
        string sshPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".ssh",
            "id_ed25519");

        this.fileSystemServiceMock.Setup(broker =>
            broker.FileExists(It.IsAny<string>()))
                .Returns(false);

        this.processServiceMock.Setup(broker =>
            broker.ExecuteCommandAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

        this.fileSystemServiceMock.Setup(broker =>
            broker.ReadFileAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

        this.fileSystemServiceMock.Setup(broker =>
            broker.WriteFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ValueTask.CompletedTask);

        // when
        await this.service.SetupSSHSigningAsync("user", "email");

        // then
        this.processServiceMock.Verify(broker =>
            broker.ExecuteCommandAsync(
                "ssh-keygen",
                It.Is<string>(args => args.Contains(sshPath))),
        Times.Once);
    }
}
