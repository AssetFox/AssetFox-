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

namespace BridgeCareCore.Services.General_Work_Queue.WorkItems
{
    public record ImportCommittedProjectWorkItem(Guid simulationId, ExcelPackage excelPackage, string filename, bool applyNoTreatment, string userId, string simulationName) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => simulationId.ToString();

        public DateTime StartTime { get; set; }

        public string UserId => userId;

        public string WorkDescription => "Import Committed Project";

        public WorkQueueMetadata Metadata =>
            new WorkQueueMetadata() { WorkType = WorkType.ImportCommittedProject, DomainType = DomainType.CommittedProject };

        public string WorkName => simulationName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _committedProjectService = scope.ServiceProvider.GetRequiredService<ICommittedProjectService>();
            var _queueLogger = new GeneralWorkQueueLogger(_hubService, UserId, updateStatusOnHandle);
            _committedProjectService.ImportCommittedProjectFiles(simulationId, excelPackage, filename, applyNoTreatment);
        }
    }
}
