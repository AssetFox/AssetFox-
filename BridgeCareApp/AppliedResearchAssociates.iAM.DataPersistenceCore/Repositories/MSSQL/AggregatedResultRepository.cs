using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository<T> : MSSQLRepository<IEnumerable<(DataMinerAttribute attribute, (int year, T value))>>, IAggregatedResultDataRepository
    {
        public AggregatedResultRepository(IAMContext context) : base(context) { }

        public void AddAggregatedResults<T>(
            List<IEnumerable<(DataMinerAttribute attribute, (int year, T value))>> domains, Guid maintainableAssetId) =>
            context.AggregatedResults.AddRange(domains.SelectMany(d => d.ToEntity(maintainableAssetId)));
    }
}
