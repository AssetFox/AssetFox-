using System;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public interface INetworkDataRepository
    {
        void AddNetworkWithoutAnyData(Network network);

        Network GetNetworkWithNoAttributeData(Guid id);

        Network GetNetworkWithAllData(Guid id);
    }
}
