using System.Collections.Generic;
using BridgeCare.EntityClasses;
using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface ICommittedRepository
    {
        void SaveCommittedProjects(int simulationId, List<CommittedProjectModel> committedProjectModels, BridgeCareContext db);
        void SavePermittedCommittedProjects(int simulationId, List<CommittedProjectModel> committedProjectModels, BridgeCareContext db, string username);

        List<CommittedEntity> GetCommittedProjects(int simulationId, BridgeCareContext db);
        List<CommittedEntity> GetPermittedCommittedProjects(int simulationId, BridgeCareContext db, string username);

        void DeleteCommittedProjects(int simulationId, BridgeCareContext db);
        void DeletePermittedCommittedProjects(int simulationId, BridgeCareContext db, string username);
    }
}
