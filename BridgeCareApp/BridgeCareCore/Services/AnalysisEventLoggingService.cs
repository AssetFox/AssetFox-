using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace BridgeCareCore.Services;

public sealed class AnalysisEventLoggingService : IAnalysisEventLoggingService
{
    public AnalysisEventLoggingService(IConfiguration configuration)
    {
        var logFolderPath = configuration["AnalysisEventLogging:LogFolderPath"];
        if (logFolderPath != null)
        {
            _ = Directory.CreateDirectory(logFolderPath);
            var logTimestamp = Regex.Replace(DateTimeOffset.UtcNow.ToString("O"), @"[:.]", "_");
            var logFileName = $"{nameof(AnalysisEventLoggingService)}_{logTimestamp}.tsv";
            LogFilePath = Path.Combine(logFolderPath, logFileName);
            File.AppendAllText(LogFilePath, string.Join('\t', LogColumnHeaders) + Environment.NewLine);
        }
    }

    public void Log(AnalysisEventLogEntry logEntry)
    {
        if (LogFilePath is not null)
        {
            lock (LogLock)
            {
                var formattedLogEntry = logEntry.FormatForLog();
                File.AppendAllText(LogFilePath, formattedLogEntry + Environment.NewLine);
            }
        }
    }

    private static readonly string[] LogColumnHeaders =
    {
        "Timestamp",
        "Simulation ID",
        "Scenario Name",
        "Message",
    };

    private readonly string LogFilePath;
    private readonly object LogLock = new();
}
