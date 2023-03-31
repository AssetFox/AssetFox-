﻿using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.WorkQueue;
using Microsoft.SqlServer.Dac.Model;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Reporting.Logging;

namespace BridgeCareCore.Services
{
    public record SimulationOutputConversionWorkitem(Guid scenarioId, string userId, string scenarioName) : IWorkSpecification

    {
        public string WorkId => scenarioId.ToString();

        public DateTime StartTime { get; set; }

        public string UserId => userId;

        public string WorkDescription => "Convert scenario output to relational from json";

        public WorkType WorkType => WorkType.DeleteNetwork;

        public string WorkName => scenarioName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _queueLogger = new GeneralWorkQueLogger(_hubService, userId, updateStatusOnHandle);
            _unitOfWork.SimulationOutputRepo.ConvertSimulationOutpuFromJsonTorelational(scenarioId, cancellationToken, _queueLogger);
        }
    }
}
