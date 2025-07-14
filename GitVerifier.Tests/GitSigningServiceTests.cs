using GitHubCommitVerifier.Brokers.FileSystems;
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Brokers.Processes;
using GitHubCommitVerifier.Services.GitSignings;
using Moq;

namespace GitVerifier.Tests;

public class GitSigningServiceTests
{
    [Fact]
    public async Task ShouldCheckGitSigningStatusAsync()
    {
        // given
        var processBrokerMock = new Mock<IProcessBroker>();
        var fileBrokerMock = new Mock<IFileSystemBroker>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();

        processBrokerMock.Setup(broker =>
            broker.ExecuteGitCommandAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);

        var service = new GitSigningService(processBrokerMock.Object, fileBrokerMock.Object, loggingBrokerMock.Object);

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
        var processBrokerMock = new Mock<IProcessBroker>();
        var fileBrokerMock = new Mock<IFileSystemBroker>();
        var loggingBrokerMock = new Mock<ILoggingBroker>();
        string sshPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".ssh",
            "id_ed25519");

        fileBrokerMock.Setup(broker =>
            broker.FileExists(It.IsAny<string>()))
                .Returns(false);
        processBrokerMock.Setup(broker =>
            broker.ExecuteCommandAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(string.Empty);
        fileBrokerMock.Setup(broker =>
            broker.ReadFileAsync(It.IsAny<string>()))
                .ReturnsAsync(string.Empty);
        fileBrokerMock.Setup(broker =>
            broker.WriteFileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ValueTask.CompletedTask);

        var service = new GitSigningService(processBrokerMock.Object, fileBrokerMock.Object, loggingBrokerMock.Object);

        // when
        await service.SetupSSHSigningAsync("user","email");

        // then
        processBrokerMock.Verify(broker =>
            broker.ExecuteCommandAsync(
                "ssh-keygen",
                It.Is<string>(args => args.Contains(sshPath))),
            Times.Once);
    }
}
