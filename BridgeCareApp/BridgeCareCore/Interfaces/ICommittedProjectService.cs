using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface ICommittedProjectService
    {
        FileInfoDTO ExportCommittedProjectsFile(Guid simulationId);

        void ImportCommittedProjectFiles(Guid simulationId, ExcelPackage excelPackage, bool applyNoTreatment);
    }
}
