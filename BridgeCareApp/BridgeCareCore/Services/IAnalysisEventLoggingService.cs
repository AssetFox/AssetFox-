namespace BridgeCareCore.Services
{
    public interface IAnalysisEventLoggingService
    {
        void Log(AnalysisEventLogEntry logEntry);
    }
}
