using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TextAggregationResultEntity : IHaveAggregatedResult<string>
    {
        public string Value { get; set; }
    }
}
