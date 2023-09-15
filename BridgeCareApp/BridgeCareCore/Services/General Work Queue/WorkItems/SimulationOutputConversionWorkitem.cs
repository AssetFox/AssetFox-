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
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Services;
using Microsoft.Graph.Models;

namespace BridgeCareCore.Services
{
    public record SimulationOutputConversionWorkitem(Guid ScenarioId, string UserId, string ScenarioName) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => WorkQueueWorkIdFactory.CreateId(ScenarioId, WorkType.SimulationOutputConversion);

        public DateTime StartTime { get; set; }

        public string WorkDescription => "Convert scenario output to relational from json";

        public WorkQueueMetadata Metadata => new WorkQueueMetadata() { DomainType = DomainType.Simulation, WorkType = WorkType.SimulationOutputConversion, DomainId = ScenarioId};

        public string WorkName => ScenarioName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _queueLogger = new GeneralWorkQueueLogger(_hubService, UserId, updateStatusOnHandle, WorkId);
            _unitOfWork.SimulationOutputRepo.ConvertSimulationOutpuFromJsonTorelational(ScenarioId, cancellationToken, _queueLogger);
        }

        public void OnFault(IServiceProvider serviceProvider, string errorMessage)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();

            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastError, $"Error Converting Simulation Output from Json to Relational::{errorMessage}");
        }

        public void OnCompletion(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastTaskCompleted, $"Successfully converted output to relational on scenario: {WorkName}");
        }

        public void OnUpdate(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastWorkQueueUpdate, WorkId);
        }
    }
}
