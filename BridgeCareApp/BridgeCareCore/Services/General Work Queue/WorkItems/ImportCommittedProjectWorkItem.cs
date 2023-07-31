using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Models;
using System.Threading;
using System;
using OfficeOpenXml;
using Microsoft.Extensions.DependencyInjection;
using BridgeCareCore.Interfaces;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using BridgeCareCore.Controllers;

namespace BridgeCareCore.Services.General_Work_Queue.WorkItems
{
    public record ImportCommittedProjectWorkItem(Guid SimulationId, ExcelPackage ExcelPackage, string Filename, bool ApplyNoTreatment, string UserId, string SimulationName) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => SimulationId.ToString();

        public DateTime StartTime { get; set; }

        public string WorkDescription => "Import Committed Project";

        public WorkQueueMetadata Metadata =>
            new WorkQueueMetadata() { WorkType = WorkType.ImportCommittedProject, DomainType = DomainType.CommittedProject };

        public string WorkName => SimulationName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _committedProjectService = scope.ServiceProvider.GetRequiredService<ICommittedProjectService>();
            var _queueLogger = new FastWorkQueueLogger(_hubService, UserId, updateStatusOnHandle, SimulationId);
            _committedProjectService.ImportCommittedProjectFiles(SimulationId, ExcelPackage, Filename, ApplyNoTreatment, cancellationToken, _queueLogger);       
        }

        public void OnFault(IServiceProvider serviceProvider, string errorMessage)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();

            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastError, $"{CommittedProjectController.CommittedProjectError}::ImportCommittedProjects - {errorMessage}");
        }

        public void OnCompletion(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastTaskCompleted, $"Successfully imported committed projects into simulation {WorkName}");
        }

        public void OnUpdate(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastFastWorkQueueUpdate, WorkId);
        }
    }
}
