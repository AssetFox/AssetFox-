using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataPersistenceCore.MSSQL.Mappings;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class AggregatedResultRepository<T> : MSSQLRepository<AggregatedResult<T>>, IAggregatedResultRepository
    {
        public AggregatedResultRepository(IAMContext context) : base(context) { }

        public int AddAggregatedResults<U>(IEnumerable<AggregatedResult<U>> domainAggregatedResults)
        {
            context.AggregatedResults.AddRange(domainAggregatedResults.SelectMany(_ => _.ToEntity()));
            return domainAggregatedResults.Count();
        }

        public int DeleteAggregatedResults(Guid networkId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAggregatedResult> GetAggregatedResults(Guid networkId)
        {
            throw new NotImplementedException();
        }
    }
}
