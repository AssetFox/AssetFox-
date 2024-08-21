using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Common.Logging;
using System.Threading;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface ICommittedProjectService
    {
        FileInfoDTO ExportCommittedProjectsFile(Guid simulationId);

        FileInfoDTO CreateCommittedProjectTemplate(Guid networkId);

        void ImportCommittedProjectFiles(Guid simulationId, ExcelPackage excelPackage, string filename, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null);

        double GetTreatmentCost(string assetKeyData, Guid treatmentId, Guid networkId);

        List<CommittedProjectConsequenceDTO> GetValidConsequences(Guid committedProjectId, Guid treatmentId, string assetKeyData, Guid networkId);
    }
}
