// Copyright (c) The Standard Organization. All rights reserved.
using GitHubCommitVerifier.Brokers.Loggings;
using GitHubCommitVerifier.Services.Foundations.FileSystems;
using GitHubCommitVerifier.Services.Foundations.Processes;

namespace GitHubCommitVerifier.Services.GitSignings;

public class GitSigningOrchestrationService : IGitSigningOrchestrationService
{
    private readonly IProcessService processService;
    private readonly IFileSystemService fileSystemService;
    private readonly ILoggingBroker loggingBroker;

    public GitSigningOrchestrationService(
        IProcessService processService,
        IFileSystemService fileSystemService,
        ILoggingBroker loggingBroker)
    {
        this.processService = processService;
        this.fileSystemService = fileSystemService;
        this.loggingBroker = loggingBroker;
    }

    public async ValueTask CheckGitSigningStatusAsync()
    {
        loggingBroker.Log("Checking commit signing configuration...");
        string config = await processService.ExecuteGitCommandAsync("config --global --get commit.gpgsign");
        string format = await processService.ExecuteGitCommandAsync("config --global --get gpg.format");

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

        if (!fileSystemService.FileExists(sshPath))
        {
            loggingBroker.Log("Generating SSH key...");
            await processService.ExecuteCommandAsync(
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

        fileSystemService.CreateDirectory(allowedSignersPath);
        string sshKey = await fileSystemService.ReadFileAsync(sshPath + ".pub");
        await fileSystemService.WriteFileAsync(allowedSignersPath, $"{userName} {sshKey}");

        loggingBroker.Log("Configuring Git...");
        await processService.ExecuteGitCommandAsync("config --global gpg.format ssh");

        await processService.ExecuteGitCommandAsync(
            $"config --global user.signingkey {sshPath}.pub");

        await processService.ExecuteGitCommandAsync(
            "config --global commit.gpgsign true");

        await processService.ExecuteGitCommandAsync(
            $"config --global gpg.ssh.allowedSignersFile {allowedSignersPath}");

        loggingBroker.Log("✅ SSH key and Git config setup completed.");
        loggingBroker.Log("🔑 Upload your public key to GitHub: https://github.com/settings/ssh");
        loggingBroker.Log($"📋 Your public key is here: {sshPath}.pub");
    }

    public async ValueTask VerifySigningSetupAsync()
    {
        loggingBroker.Log("Verifying commit signing...");
        string output = await processService.ExecuteCommandAsync("git", "log --show-signature -1");
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

        await processService.ExecuteGitCommandAsync("config --global --unset gpg.format");
        await processService.ExecuteGitCommandAsync("config --global --unset user.signingkey");
        await processService.ExecuteGitCommandAsync("config --global --unset commit.gpgsign");
        await processService.ExecuteGitCommandAsync("config --global --unset gpg.ssh.allowedSignersFile");

        fileSystemService.DeleteFile(sshPath);
        fileSystemService.DeleteFile(sshPubPath);
        fileSystemService.DeleteFile(allowedSignersPath);

        loggingBroker.Log("✅ SSH signing configuration has been reset.");
    }
}
