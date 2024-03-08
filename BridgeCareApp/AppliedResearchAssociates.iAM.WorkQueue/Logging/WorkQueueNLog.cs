using AppliedResearchAssociates.iAM.Common;
using NLog;

namespace AppliedResearchAssociates.iAM.WorkQueue.Logging
{
    public class WorkQueueNLog : ILog
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public WorkQueueNLog()
        {
        }

        public void Information(string message) => Logger.Info(message);

        public void Warning(string message) => Logger.Warn(message);

        public void Debug(string message) => Logger.Debug(message);

        public void Error(string message) => Logger.Error(message);
    }
}
