using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ICommittedProjectRepository
    {
        void CreateCommittedProjects(List<CommittedProject> committedProjects, string simulationName);
        void GetSimulationCommittedProjects(Simulation simulation);
    }
}
