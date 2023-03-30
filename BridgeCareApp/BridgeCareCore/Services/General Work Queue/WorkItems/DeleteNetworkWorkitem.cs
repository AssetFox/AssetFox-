using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Common;

using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;

using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.WorkQueue;

using AppliedResearchAssociates.Validation;
using BridgeCareCore.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.Services
{
    public record DeleteNetworkWorkitem(Guid NetworkId, string userId, string networkName) : IWorkSpecification

    {
        public string WorkId => NetworkId.ToString();

        public DateTime StartTime { get; set; }

        public string UserId => userId;

        public string WorkDescription => "Delete Network";

        public WorkType WorkType => WorkType.DeleteNetwork;

        public string WorkName => networkName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _queueLogger = new GeneralWorkQueLogger(_hubService, UserId, updateStatusOnHandle);
            _unitOfWork.NetworkRepo.DeleteNetwork(NetworkId, cancellationToken, _queueLogger);
        }
    }
}
