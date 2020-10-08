using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository<T> : MSSQLRepository<IEnumerable<(DataMinerAttribute attribute, (int year, T value))>>
    {
        public AggregatedResultRepository(IAMContext context) : base(context) { }

        public override async void AddAll(
            IEnumerable<IEnumerable<(DataMinerAttribute attribute, (int year, T value))>> domains,
            params object[] args)
        {
            if (!args.Any())
            {
                throw new NullReferenceException("No arguments found for aggregated result query");
            }

            var maintainableAssetId = (Guid)args[0];

            await context.AggregatedResults.AddRangeAsync(domains.SelectMany(d => d.ToEntity(maintainableAssetId)));
        }
    }
}
