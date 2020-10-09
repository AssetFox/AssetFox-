using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces
{
    public interface IAggregatedResultDataRepository
    {
        void AddAggregatedResults<T>(List<IEnumerable<(DataMinerAttribute attribute, (int year, T value))>> domain);
    }
}
