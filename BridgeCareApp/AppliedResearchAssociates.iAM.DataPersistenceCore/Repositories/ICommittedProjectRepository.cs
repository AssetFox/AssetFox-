using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;
using System.IO;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICommittedProjectRepository
    {
        void GetSimulationCommittedProjects(Simulation simulation);

        List<SectionCommittedProjectDTO> GetSectionCommittedProjectDTOs(Guid simulationId);

        Guid GetSimulationId(Guid projectId);

        List<BaseCommittedProjectDTO> GetCommittedProjectsForExport(Guid simulationId);

        void UpsertCommittedProjects(List<SectionCommittedProjectDTO> projects);

        void SetCommittedProjectTemplate(FileStream name);

        void DeleteSimulationCommittedProjects(Guid simulationId);

        void DeleteSpecificCommittedProjects(List<Guid> projectIds);

        string GetNetworkKeyAttribute(Guid simulationId);
    }
}
