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
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Reporting.Logging
{
    public class FastWorkQueueLogger : IWorkQueueLog
    {
        private readonly IHubService _hubService;
        private readonly string _username;
        private Action<string> _updateAction;
        private string _workId;

        public FastWorkQueueLogger(IHubService hubService, string userName, Action<string> updateAction, string workId)
        {
            _hubService = hubService;
            _username = userName;
            _updateAction = updateAction;
            _workId = workId;
        }

        public void UpdateWorkQueueStatus(string statusMessage)
        {
            var queueStatusUpdate = new QueuedWorkStatusUpdateModel() { Id = _workId, Status = statusMessage };
            _updateAction.Invoke(statusMessage);
            _hubService.SendRealTimeMessage(_username, HubConstant.BroadcastFastWorkQueueStatusUpdate, queueStatusUpdate);
        }
    }
}
