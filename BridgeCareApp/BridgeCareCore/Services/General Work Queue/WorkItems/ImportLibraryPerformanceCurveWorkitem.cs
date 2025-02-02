using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
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
using AppliedResearchAssociates.iAM.Hubs;
using BridgeCareCore.Controllers;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace BridgeCareCore.Services.General_Work_Queue.WorkItems
{
    public record ImportLibraryPerformanceCurveWorkitem(Guid PerformanceCurveLibraryId, ExcelPackage ExcelPackage, UserCriteriaDTO CurrentUserCriteriaFilter, string UserId, string PerformanceCurveName) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => WorkQueueWorkIdFactory.CreateId(PerformanceCurveLibraryId, WorkType.ImportLibraryPerformanceCurve);

        public DateTime StartTime { get; set; }

        public string WorkDescription => "Import Library Performance Curve";

        public WorkQueueMetadata Metadata =>
            new WorkQueueMetadata() { WorkType = WorkType.ImportLibraryPerformanceCurve, DomainType = DomainType.PerformanceCurve, DomainId = PerformanceCurveLibraryId};

        public string WorkName => PerformanceCurveName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _performanceCurvesService = scope.ServiceProvider.GetRequiredService<IPerformanceCurvesService>();
            var _queueLogger = new FastWorkQueueLogger(_hubService, UserId, updateStatusOnHandle, WorkId);
            var importResult = _performanceCurvesService.ImportLibraryPerformanceCurvesFile(PerformanceCurveLibraryId, ExcelPackage, CurrentUserCriteriaFilter, cancellationToken, _queueLogger);
            if (importResult.WarningMessage != null)
            {
                _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastWarning, importResult.WarningMessage);
            }
        }

        public void OnFault(IServiceProvider serviceProvider, string errorMessage)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();

            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastError, $"{PerformanceCurveController.DeteriorationModelError}::ImportLibraryPerformanceCurvesExcelFile - {errorMessage}");
        }

        public void OnCompletion(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastTaskCompleted, $"Successfully imported performance curve library: {WorkName}");
            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastImportCompletion, new ImportCompletionDTO()
            {
                Id = Metadata.DomainId,
                WorkType = Metadata.WorkType
            });
        }

        public void OnUpdate(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastFastWorkQueueUpdate, WorkId);
        }
    }
}
