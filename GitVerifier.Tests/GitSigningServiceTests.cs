// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.FileSystems;
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Brokers.Processes;
using GitHubCommitVerifier.Services.GitSignings;
using Moq;

namespace GitVerifier.Tests;

public class GitSigningServiceTests
{
    private readonly Mock<IProcessBroker> processBrokerMock;
    private readonly Mock<IFileSystemBroker> fileSystemBrokerMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly GitSigningService service;

    public GitSigningServiceTests()
    {
        processBrokerMock = new Mock<IProcessBroker>();
        fileSystemBrokerMock = new Mock<IFileSystemBroker>();
        loggingBrokerMock = new Mock<ILoggingBroker>();

        service = new GitSigningService(
            processBrokerMock.Object,
            fileSystemBrokerMock.Object,
            loggingBrokerMock.Object);
    }

    [Fact]
    public async Task ShouldCheckGitSigningStatusAsync()
    {
        // given
        processBrokerMock.Setup(broker =>
            broker.ExecuteGitCommandAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

        // when
        await service.CheckGitSigningStatusAsync();

        // then
        processBrokerMock.Verify(broker =>
            broker.ExecuteGitCommandAsync(
                "config --global --get commit.gpgsign"),
        Times.Once);

        processBrokerMock.Verify(broker =>
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

        fileSystemBrokerMock.Setup(broker =>
            broker.FileExists(It.IsAny<string>()))
                .Returns(false);

        processBrokerMock.Setup(broker =>
            broker.ExecuteCommandAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

        fileSystemBrokerMock.Setup(broker =>
            broker.ReadFileAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

        fileSystemBrokerMock.Setup(broker =>
            broker.WriteFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ValueTask.CompletedTask);

        // when
        await service.SetupSSHSigningAsync("user", "email");

        // then
        processBrokerMock.Verify(broker =>
            broker.ExecuteCommandAsync(
                "ssh-keygen",
                It.Is<string>(args => args.Contains(sshPath))),
        Times.Once);
    }
}
