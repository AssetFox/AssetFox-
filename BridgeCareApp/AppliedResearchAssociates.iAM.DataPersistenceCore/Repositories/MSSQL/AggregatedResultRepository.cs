using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Interfaces;
using DataMinerAttribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository<T> : MSSQLRepository<AggregatedResult<T>>, IAggregatedResultDataRepository
    {
        public AggregatedResultRepository(IAMContext context) : base(context) { }

        public void AddAggregatedResults<T>(IEnumerable<AggregatedResult<T>> domainAggregatedResults) =>
            context.AggregatedResults.AddRange((IEnumerable<Entities.AggregatedResultEntity>)domainAggregatedResults);
    }
}
