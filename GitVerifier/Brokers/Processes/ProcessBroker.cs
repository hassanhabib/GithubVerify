// Copyright (c) The Standard Organization. All rights reserved.
using System.Diagnostics;

namespace GitHubCommitVerifier.Brokers.Processes;

public class ProcessBroker : IProcessBroker
{
    public async ValueTask<string> ExecuteCommandAsync(string command, string arguments)
    {
        var outputCompletion = new TaskCompletionSource<string>();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        string output = string.Empty;
        process.OutputDataReceived += (_, e) => { if (e.Data != null) output += e.Data + "\n"; };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) output += e.Data + "\n"; };
        process.Exited += (_, _) => outputCompletion.TrySetResult(output);

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        return output;
    }

    public async ValueTask<string> ExecuteGitCommandAsync(string arguments) =>
        await ExecuteCommandAsync("git", arguments);
}
