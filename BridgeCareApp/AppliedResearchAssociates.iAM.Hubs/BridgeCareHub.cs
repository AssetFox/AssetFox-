using System.Threading.Tasks;
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
        public const string BroadcastInfo = "BroadcastInfo";
        public const string BroadcastWarning = "BroadcastWarning";
        public const string BroadcastTaskCompleted = "BroadcastTaskCompleted";
        public const string BroadcastAssignDataStatus = "BroadcastAssignDataStatus";
        public const string BroadcastReportGenerationStatus = "BroadcastReportGenerationStatus";
        public const string BroadcastScenarioStatusUpdate = "BroadcastScenarioStatusUpdate";
        public const string BroadcastSimulationAnalysisDetail = "BroadcastSimulationAnalysisDetail";
        public const string BroadcastWorkQueueStatusUpdate = "BroadcastWorkQueueStatusUpdate";
        public const string BroadcastWorkQueueUpdate = "BroadcastWorkQueueUpdate";
        public const string BroadcastFastWorkQueueStatusUpdate = "BroadcastFastWorkQueueStatusUpdate";
        public const string BroadcastFastWorkQueueUpdate = "BroadcastFastWorkQueueUpdate";
        public const string BroadcastDataMigration = "BroadcastDataMigration";
        public const string BroadcastImportCompletion = "BroadcastImportCompletion";
        public const string BroadcastSimulationDeletionCompletion = "BroadcastSimulationDeletionCompletion";
    }
}
