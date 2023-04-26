﻿using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Logging;
using AppliedResearchAssociates.iAM.WorkQueue;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using OfficeOpenXml;
using System.Threading;
using System;
using Microsoft.Extensions.DependencyInjection;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Hubs;
using GreenDonut;

namespace BridgeCareCore.Services.General_Work_Queue.WorkItems
{
    public record ImportLibraryInvestmentWorkitem(Guid BudgetLibraryId, ExcelPackage ExcelPackage, UserCriteriaDTO CurrentUserCriteriaFilter,bool OverwriteBudgets, string UserId, string NetworkName) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => BudgetLibraryId.ToString();

        public DateTime StartTime { get; set; }

        public string WorkDescription => "Import Library Investment";

        public WorkQueueMetadata Metadata =>
            new WorkQueueMetadata() { WorkType = WorkType.ImportLibraryInvestment, DomainType = DomainType.Investment };

        public string WorkName => NetworkName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _committedProjectService = scope.ServiceProvider.GetRequiredService<IInvestmentBudgetsService>();
            var _queueLogger = new GeneralWorkQueueLogger(_hubService, UserId, updateStatusOnHandle);
            _queueLogger.UpdateWorkQueueStatus(Guid.Parse(WorkId), "Starting Import");
            var importResult = _committedProjectService.ImportLibraryInvestmentBudgetsFile(BudgetLibraryId, ExcelPackage, CurrentUserCriteriaFilter, OverwriteBudgets, cancellationToken, _queueLogger);
            if (importResult.WarningMessage != null)
            {
                _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastWarning, importResult.WarningMessage);
            }
        }
    }
}