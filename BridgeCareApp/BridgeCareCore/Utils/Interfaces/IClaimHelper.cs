using System;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IClaimHelper
    {
        public void CheckUserSimulationReadAuthorization(Guid simulationId, Guid userId);

        public void CheckUserSimulationModifyAuthorization(Guid simulationId, Guid userId);

        public bool RequirePermittedCheck();

        public void CheckUserLibraryModifyAuthorization(Guid owner, Guid userId);        
    }
}
