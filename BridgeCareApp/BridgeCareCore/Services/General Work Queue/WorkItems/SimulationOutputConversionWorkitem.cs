using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.WorkQueue;
using Microsoft.SqlServer.Dac.Model;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using BridgeCareCore.Models;

namespace BridgeCareCore.Services
{
    public record SimulationOutputConversionWorkitem(Guid ScenarioId, string UserId, string ScenarioName) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => ScenarioId.ToString();

        public DateTime StartTime { get; set; }

        public string WorkDescription => "Convert scenario output to relational from json";

        public WorkQueueMetadata Metadata => new WorkQueueMetadata() { DomainType = DomainType.Simulation, WorkType = WorkType.SimulationOutputConversion};

        public string WorkName => ScenarioName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _queueLogger = new GeneralWorkQueueLogger(_hubService, UserId, updateStatusOnHandle);
            _unitOfWork.SimulationOutputRepo.ConvertSimulationOutpuFromJsonTorelational(ScenarioId, cancellationToken, _queueLogger);
        }
    }
}
