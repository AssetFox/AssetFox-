using System;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IClaimHelper
    {
        public void CheckUserSimulationReadAuthorization(Guid simulationId);

        public void CheckUserSimulationModifyAuthorization(Guid simulationId);

        public bool RequirePermittedCheck();

        public void CheckUserLibraryModifyAuthorization(Guid owner);

        public bool HasAdminClaim();
    }
}
