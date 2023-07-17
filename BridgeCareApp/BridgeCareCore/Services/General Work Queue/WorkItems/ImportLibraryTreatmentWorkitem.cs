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
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Controllers;

namespace BridgeCareCore.Services.General_Work_Queue.WorkItems
{
    public record ImportLibraryTreatmentWorkitem(Guid TreatmentLibraryId, ExcelPackage ExcelPackage, string UserId, string treatmentName) : IWorkSpecification<WorkQueueMetadata>

    {
        public string WorkId => TreatmentLibraryId.ToString();

        public DateTime StartTime { get; set; }

        public string WorkDescription => "Import Library Treatment";

        public WorkQueueMetadata Metadata =>
            new WorkQueueMetadata() { WorkType = WorkType.ImportLibraryTreatment, DomainType = DomainType.Treatment };

        public string WorkName => treatmentName;

        public void DoWork(IServiceProvider serviceProvider, Action<string> updateStatusOnHandle, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            var _treatmentService = scope.ServiceProvider.GetRequiredService<ITreatmentService>();
            var _queueLogger = new GeneralWorkQueueLogger(_hubService, UserId, updateStatusOnHandle, TreatmentLibraryId);
            var importResult = _treatmentService.ImportLibraryTreatmentsFile(TreatmentLibraryId, ExcelPackage, cancellationToken, _queueLogger);
            if (importResult.WarningMessage != null && importResult.WarningMessage.Trim() != "")
            {
                _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastWarning, importResult.WarningMessage);
            }
        }

        public void OnFault(IServiceProvider serviceProvider, string errorMessage)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();

            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastError, $"{TreatmentController.TreatmentError}::ImportLibraryTreatmentsFile - {errorMessage}");
        }

        public void OnCompletion(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _hubService = scope.ServiceProvider.GetRequiredService<IHubService>();
            _hubService.SendRealTimeMessage(UserId, HubConstant.BroadcastTaskCompleted, $"successfully imported treatment library: {WorkName}");
        }
    }
}
