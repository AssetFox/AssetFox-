﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace AppliedResearchAssociates.iAM.Hubs
{
    public class BridgeCareHub : Hub
    {
        public async Task SendMessage(string status) => await Clients.All.SendAsync("BroadcastMessage", status);

        public async Task AssociateMessage(string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, username);
        }
    }

    public static class HubConstant
    {
        public const string BroadcastError = "BroadcastError";
        public const string BroadcastWarning = "BroadcastWarning";
        public const string BroadcastAssignDataStatus = "BroadcastAssignDataStatus";
        public const string BroadcastReportGenerationStatus = "BroadcastReportGenerationStatus";
        public const string BroadcastScenarioStatusUpdate = "BroadcastScenarioStatusUpdate";
        public const string BroadcastSimulationAnalysisDetail = "BroadcastSimulationAnalysisDetail";
        public const string BroadcastDataMigration = "BroadcastDataMigration";
    }
}
