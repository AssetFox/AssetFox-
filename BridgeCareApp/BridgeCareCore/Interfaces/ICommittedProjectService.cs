using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface ICommittedProjectService
    {
        FileInfoDTO ExportCommittedProjectsFile(Guid simulationId);

        FileInfoDTO CreateCommittedProjectTemplate();

        void ImportCommittedProjectFiles(Guid simulationId, ExcelPackage excelPackage, string filename, bool applyNoTreatment);

        public double GetTreatmentCost(Guid simulationId, string brkey, string treatment, int year);
        List<CommittedProjectConsequenceDTO> GetValidConsequences(Guid committedProjectId, Guid simulationId, string brkey, string treatment, int year);
    }
}
