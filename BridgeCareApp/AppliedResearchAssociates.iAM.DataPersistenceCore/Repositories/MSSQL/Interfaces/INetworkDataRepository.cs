using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataAssignment.Segmentation;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public interface INetworkDataRepository
    {
        void AddNetworkWithoutAnyData(Network network);
        Network GetNetworkWithNoAttributeData(Guid id);

        Network GetNetworkWillAllData(Guid id);
    }
}
