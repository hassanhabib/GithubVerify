// Copyright (c) The Standard Organization. All rights reserved.
namespace GitHubCommitVerifier.Brokers.Loggings;

public class LoggingBroker : ILoggingBroker
{
    public void Log(string message) => Console.WriteLine(message);
}
