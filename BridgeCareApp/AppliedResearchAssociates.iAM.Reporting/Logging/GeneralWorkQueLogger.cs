using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs;
using NLog;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Common.Logging;

namespace AppliedResearchAssociates.iAM.Reporting.Logging
{
    public class GeneralWorkQueLogger : IWorkQueueLog
    {
        private readonly IHubService _hubService;
        private readonly string _username;
        private Action<string> _updateAction;

        public GeneralWorkQueLogger(IHubService hubService, string userName, Action<string> updateAction)
        {
            _hubService = hubService;
            _username = userName;
            _updateAction = updateAction;
        }

        public void UpdateWorkQueueStatus(Guid workId, string statusMessage)
        {
            var queueStatusUpdate = new QueuedWorkStatusUpdateModel() { Id = workId, Status = statusMessage };
            _updateAction.Invoke(statusMessage);
            _hubService.SendRealTimeMessage(_username, HubConstant.BroadcastWorkQueueStatusUpdate, queueStatusUpdate);
        }
    }
}
