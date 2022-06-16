using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICommittedProjectRepository
    {
        void GetSimulationCommittedProjects(Simulation simulation);

        List<BaseCommittedProjectDTO> GetCommittedProjectsForExport(Guid simulationId);

        void CreateCommittedProjects(List<BaseCommittedProjectDTO> committedProjectEntities);

        void DeleteCommittedProjects(Guid simulationId);
    }
}
