using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Common;

using AppliedResearchAssociates.iAM.Common.PerformanceMeasurement;

using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.WorkQueue;

using AppliedResearchAssociates.Validation;
using BridgeCareCore.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeCareCore.Services
{
    public record DeleteNetworkWorkitem(Guid NetworkId, string UserId) : IWorkSpecification

    {
        public string WorkId => NetworkId.ToString();


        public DateTime StartTime { get; set; }

        public string UserId => UserId;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            updateStatusOnHandle.Invoke("Deleting Network");
            _unitOfWork.NetworkRepo.DeleteNetwork(NetworkId);
            updateStatusOnHandle.Invoke("Network Deleted");
        }
    }
}
