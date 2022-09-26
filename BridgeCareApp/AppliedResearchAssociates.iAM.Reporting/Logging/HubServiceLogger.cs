using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting.Logging
{
    public class HubServiceLogger : ILog
    {
        private readonly IHubService _hubService;
        private readonly string _hubConstant;
        private readonly string _username;

        public HubServiceLogger(IHubService hubService, string hubConstant, string userName)
        {
            _hubService = hubService;
            _hubConstant = hubConstant;
            _username = userName;
        }

        public void Debug(string message)
        {
            _hubService.SendRealTimeMessage(_username, _hubConstant, message);
        }

        public void Information(string message)
        {
            _hubService.SendRealTimeMessage(_username, _hubConstant, message);
        }

        public void Warning(string message)
        {
            _hubService.SendRealTimeMessage(_username, _hubConstant, message);
        }

        public void Error(string message)
        {
            _hubService.SendRealTimeMessage(_username, _hubConstant, message);
        }
    }
}
