// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Diagnostics;

namespace GitHubCommitVerifier
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet GithubVerify [check|setup|verify|reset] [username] [email]");
                return 1;
            }

            string command = args[0].ToLower();
            string userName = args.Length > 1 ? args[1] : Environment.UserName;
            string userEmail = args.Length > 2 ? args[2] : "your_email@example.com";

            switch (command)
            {
                case "check":
                    await CheckGitSigningStatus();
                    break;
                case "setup":
                    await SetupSSHSigning(userName, userEmail);
                    break;
                case "verify":
                    await VerifySigningSetup();
                    break;
                case "reset":
                    await ResetSSHSigning();
                    break;
                default:
                    Console.WriteLine("Invalid command. Use 'check', 'setup', 'verify', or 'reset'.");
                    return 1;
            }

            return 0;
        }

        private static async Task CheckGitSigningStatus()
        {
            Console.WriteLine("Checking commit signing configuration...");
            string config = await RunGitCommand("config --global --get commit.gpgsign");
            string format = await RunGitCommand("config --global --get gpg.format");

            if (config.Trim() == "true" && format.Trim() == "ssh")
                Console.WriteLine("✅ Verified commit signing is enabled with SSH.");
            else
                Console.WriteLine("❌ Verified commit signing is not configured properly.");
        }

        private static async Task SetupSSHSigning(string userName, string userEmail)
        {
            Console.WriteLine("Setting up SSH signing...");

            string targetDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh");

            if (Directory.Exists(targetDirectory) is not true)
            {
                Directory.CreateDirectory(targetDirectory);
            }

            string sshPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh", "id_ed25519");

            if (!File.Exists(sshPath))
            {
                Console.WriteLine("Generating SSH key...");
                await RunCommand("ssh-keygen", $"-t ed25519 -C \"{userEmail}\" -f \"{sshPath}\" -N \"\"");
            }
            else
            {
                Console.WriteLine("SSH key already exists at " + sshPath);
            }

            string allowedSignersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".gnupg", "allowed_signers");
            Directory.CreateDirectory(Path.GetDirectoryName(allowedSignersPath)!);
            string sshKey = await File.ReadAllTextAsync(sshPath + ".pub");
            await File.WriteAllTextAsync(allowedSignersPath, $"{userName} {sshKey}");

            Console.WriteLine("Configuring Git...");
            await RunGitCommand("config --global gpg.format ssh");
            await RunGitCommand($"config --global user.signingkey {sshPath}.pub");
            await RunGitCommand("config --global commit.gpgsign true");
            await RunGitCommand($"config --global gpg.ssh.allowedSignersFile {allowedSignersPath}");

            Console.WriteLine("✅ SSH key and Git config setup completed.");
            Console.WriteLine("🔑 Upload your public key to GitHub: https://github.com/settings/ssh");
            Console.WriteLine($"📋 Your public key is here: {sshPath}.pub");
        }

        private static async Task VerifySigningSetup()
        {
            Console.WriteLine("Verifying commit signing...");
            string output = await RunCommand("git", "log --show-signature -1");
            if (output.Contains("Good \"ssh-ed25519\" signature"))
            {
                Console.WriteLine("✅ Last commit is properly signed with SSH.");
            }
            else if (output.Contains("gpg.ssh.allowedSignersFile needs to be configured"))
            {
                Console.WriteLine("❌ Missing allowedSignersFile config. Run `setup` again or manually fix Git config.");
            }
            else if (output.Contains("No signature"))
            {
                Console.WriteLine("❌ Last commit is not signed.");
            }
            else
            {
                Console.WriteLine("⚠️ Unknown verification state:");
                Console.WriteLine(output);
            }
        }

        private static async Task ResetSSHSigning()
        {
            Console.WriteLine("⚠️ Resetting SSH signing configuration...");

            string sshPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh", "id_ed25519");
            string sshPubPath = sshPath + ".pub";
            string allowedSignersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".gnupg", "allowed_signers");

            // Remove Git config settings
            await RunGitCommand("config --global --unset gpg.format");
            await RunGitCommand("config --global --unset user.signingkey");
            await RunGitCommand("config --global --unset commit.gpgsign");
            await RunGitCommand("config --global --unset gpg.ssh.allowedSignersFile");

            // Delete SSH key files
            if (File.Exists(sshPath))
            {
                File.Delete(sshPath);
                Console.WriteLine("🗑️ Deleted private SSH key.");
            }

            if (File.Exists(sshPubPath))
            {
                File.Delete(sshPubPath);
                Console.WriteLine("🗑️ Deleted public SSH key.");
            }

            // Delete allowed signers file
            if (File.Exists(allowedSignersPath))
            {
                File.Delete(allowedSignersPath);
                Console.WriteLine("🗑️ Deleted allowed signers file.");
            }

            Console.WriteLine("✅ SSH signing configuration has been reset.");
        }

        private static async Task<string> RunGitCommand(string args)
        {
            return await RunCommand("git", args);
        }

        private static async Task<string> RunCommand(string command, string args)
        {
            var tcs = new TaskCompletionSource<string>();
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            string output = "";
            process.OutputDataReceived += (s, e) => { if (e.Data != null) output += e.Data + "\n"; };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) output += e.Data + "\n"; };
            process.Exited += (s, e) => tcs.TrySetResult(output);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();

            return output;
        }
    }
}
