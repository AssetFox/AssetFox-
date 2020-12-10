using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICommittedProjectRepository
    {
        void CreateCommittedProjects(List<CommittedProject> committedProjects, Guid simulationId);
        void GetSimulationCommittedProjects(Simulation simulation);
    }
}
