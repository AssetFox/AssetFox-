using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Models;
using Microsoft.SqlServer.Dac.Model;
using System.Threading;
using System;
using AppliedResearchAssociates.iAM.WorkQueue;
using Microsoft.Extensions.DependencyInjection;
using AppliedResearchAssociates.iAM.Reporting.Logging;

namespace BridgeCareCore.Services.General_Work_Queue.WorkItems
{
    public record DeleteSimulationWorkitem(Guid SimulationId, string userId, string scenarioName) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => SimulationId.ToString();

        public DateTime StartTime { get; set; }

        public string UserId => userId;

        public string WorkDescription => "Delete Simulation";

        public WorkQueueMetadata Metadata =>
            new WorkQueueMetadata() { WorkType = WorkType.DeleteSimulation, DomainType = DomainType.Network };

        public string WorkName => scenarioName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _queueLogger = new GeneralWorkQueLogger(_hubService, UserId, updateStatusOnHandle);
            _unitOfWork.SimulationRepo.DeleteSimulation(SimulationId, cancellationToken, _queueLogger);
        }
    }
}
