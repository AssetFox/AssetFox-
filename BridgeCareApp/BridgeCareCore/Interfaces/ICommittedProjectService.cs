using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface ICommittedProjectService
    {
        FileInfoDTO ExportCommittedProjectsFile(Guid simulationId);

        void ImportCommittedProjectFiles(Guid simulationId, List<ExcelPackage> excelPackages, bool applyNoTreatment);
    }
}
