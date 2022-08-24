using System;

namespace BridgeCareCore.Utils.Interfaces
{
    public interface IClaimHelper
    {
        public void CheckUserSimulationReadAuthorization(Guid simulationId);

        public void CheckUserSimulationModifyAuthorization(Guid simulationId);
    }
}
