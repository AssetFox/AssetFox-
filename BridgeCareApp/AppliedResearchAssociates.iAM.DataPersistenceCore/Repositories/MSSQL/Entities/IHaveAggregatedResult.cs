using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    interface IHaveAggregatedResult<T>
    {
        public T Value { get; set; }
    }
}
