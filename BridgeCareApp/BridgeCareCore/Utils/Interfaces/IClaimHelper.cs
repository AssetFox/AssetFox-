using System;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IClaimHelper
    {
        public void CheckUserSimulationReadAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess = false);

        public void CheckUserSimulationModifyAuthorization(Guid simulationId, Guid userId, bool checkSimulationAccess = false);

        public void CheckUserSimulationCancelAnalysisAuthorization(Guid simulationId, string userName, bool checkSimulationAccess);

        public bool RequirePermittedCheck();

        public void CheckUserLibraryModifyAuthorization(Guid owner, Guid userId);        
    }
}
