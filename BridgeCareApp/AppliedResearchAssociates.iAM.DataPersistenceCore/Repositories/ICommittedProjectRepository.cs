using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICommittedProjectRepository
    {
        void CreateCommittedProjects(List<CommittedProject> committedProjects, Guid simulationId);

        void GetSimulationCommittedProjects(Simulation simulation);

        List<CommittedProjectEntity> GetCommittedProjectsForExport(Guid simulationId);

        void CreateCommittedProjects(List<CommittedProjectEntity> committedProjectEntities);

        void DeleteCommittedProjects(Guid simulationId);
    }
}
