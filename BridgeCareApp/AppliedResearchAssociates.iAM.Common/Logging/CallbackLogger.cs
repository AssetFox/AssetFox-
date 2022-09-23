using System;
using AppliedResearchAssociates.iAM.Common;

namespace BridgeCareCore.Services
{
    public class CallbackLogger : ILog
    {
        private readonly Action<string> _callback;

        public CallbackLogger(Action<string> callback)
        {
            _callback = callback;
        }

        public void Log(string message) => _callback.Invoke(message);
        public void Debug(string message) => Log(message);
        public void Error(string message) => Log(message);
        public void Information(string message) => Log(message);
        public void Warning(string message) => Log(message);
    }
}
