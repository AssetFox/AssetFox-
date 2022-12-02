using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces
{
    public interface ICommittedProjectService
    {
        FileInfoDTO ExportCommittedProjectsFile(Guid simulationId);

        FileInfoDTO CreateCommittedProjectTemplate();

        void ImportCommittedProjectFiles(Guid simulationId, ExcelPackage excelPackage, string filename, bool applyNoTreatment);

        double GetTreatmentCost(Guid simulationId, string brkey, string treatment, int year, Guid networkId);

        List<CommittedProjectConsequenceDTO> GetValidConsequences(Guid committedProjectId, Guid simulationId, string brkey, string treatment, int year, Guid networkId);

        PagingPageModel<SectionCommittedProjectDTO> GetCommittedProjectPage(List<SectionCommittedProjectDTO> committedProjects, PagingRequestModel<SectionCommittedProjectDTO> request);

        List<SectionCommittedProjectDTO> GetSyncedDataset(Guid simulationId, PagingSyncModel<SectionCommittedProjectDTO> request);
    }
}
