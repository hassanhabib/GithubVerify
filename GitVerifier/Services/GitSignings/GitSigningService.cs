using GitHubCommitVerifier.Brokers.FileSystems;
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Brokers.Processes;

namespace GitHubCommitVerifier.Services.GitSignings;

public class GitSigningService : IGitSigningService
{
    private readonly IProcessBroker processBroker;
    private readonly IFileSystemBroker fileSystemBroker;
    private readonly ILoggingBroker loggingBroker;

    public GitSigningService(
        IProcessBroker processBroker,
        IFileSystemBroker fileSystemBroker,
        ILoggingBroker loggingBroker)
    {
        this.processBroker = processBroker;
        this.fileSystemBroker = fileSystemBroker;
        this.loggingBroker = loggingBroker;
    }

    public async ValueTask CheckGitSigningStatusAsync()
    {
        loggingBroker.Log("Checking commit signing configuration...");
        string config = await processBroker.ExecuteGitCommandAsync("config --global --get commit.gpgsign");
        string format = await processBroker.ExecuteGitCommandAsync("config --global --get gpg.format");

        if (config.Trim() == "true" && format.Trim() == "ssh")
            loggingBroker.Log("✅ Verified commit signing is enabled with SSH.");
        else
            loggingBroker.Log("❌ Verified commit signing is not configured properly.");
    }

    public async ValueTask SetupSSHSigningAsync(string userName, string userEmail)
    {
        loggingBroker.Log("Setting up SSH signing...");
        string sshPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".ssh",
            "id_ed25519");

        if (!fileSystemBroker.FileExists(sshPath))
        {
            loggingBroker.Log("Generating SSH key...");
            await processBroker.ExecuteCommandAsync(
                "ssh-keygen",
                $"-t ed25519 -C \"{userEmail}\" -f \"{sshPath}\" -N \"\"");
        }
        else
        {
            loggingBroker.Log($"SSH key already exists at {sshPath}");
        }

        string allowedSignersPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".gnupg",
            "allowed_signers");
        fileSystemBroker.CreateDirectory(allowedSignersPath);
        string sshKey = await fileSystemBroker.ReadFileAsync(sshPath + ".pub");
        await fileSystemBroker.WriteFileAsync(allowedSignersPath, $"{userName} {sshKey}");

        loggingBroker.Log("Configuring Git...");
        await processBroker.ExecuteGitCommandAsync("config --global gpg.format ssh");
        await processBroker.ExecuteGitCommandAsync(
            $"config --global user.signingkey {sshPath}.pub");
        await processBroker.ExecuteGitCommandAsync(
            "config --global commit.gpgsign true");
        await processBroker.ExecuteGitCommandAsync(
            $"config --global gpg.ssh.allowedSignersFile {allowedSignersPath}");

        loggingBroker.Log("✅ SSH key and Git config setup completed.");
        loggingBroker.Log("🔑 Upload your public key to GitHub: https://github.com/settings/ssh");
        loggingBroker.Log($"📋 Your public key is here: {sshPath}.pub");
    }

    public async ValueTask VerifySigningSetupAsync()
    {
        loggingBroker.Log("Verifying commit signing...");
        string output = await processBroker.ExecuteCommandAsync("git", "log --show-signature -1");
        if (output.Contains("Good \"ssh-ed25519\" signature"))
        {
            loggingBroker.Log("✅ Last commit is properly signed with SSH.");
        }
        else if (output.Contains("gpg.ssh.allowedSignersFile needs to be configured"))
        {
            loggingBroker.Log("❌ Missing allowedSignersFile config. Run `setup` again or manually fix Git config.");
        }
        else if (output.Contains("No signature"))
        {
            loggingBroker.Log("❌ Last commit is not signed.");
        }
        else
        {
            loggingBroker.Log("⚠️ Unknown verification state:");
            loggingBroker.Log(output);
        }
    }

    public async ValueTask ResetSSHSigningAsync()
    {
        loggingBroker.Log("⚠️ Resetting SSH signing configuration...");

        string sshPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".ssh",
            "id_ed25519");
        string sshPubPath = sshPath + ".pub";
        string allowedSignersPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".gnupg",
            "allowed_signers");

        await processBroker.ExecuteGitCommandAsync("config --global --unset gpg.format");
        await processBroker.ExecuteGitCommandAsync("config --global --unset user.signingkey");
        await processBroker.ExecuteGitCommandAsync("config --global --unset commit.gpgsign");
        await processBroker.ExecuteGitCommandAsync("config --global --unset gpg.ssh.allowedSignersFile");

        fileSystemBroker.DeleteFile(sshPath);
        fileSystemBroker.DeleteFile(sshPubPath);
        fileSystemBroker.DeleteFile(allowedSignersPath);

        loggingBroker.Log("✅ SSH signing configuration has been reset.");
    }
}
