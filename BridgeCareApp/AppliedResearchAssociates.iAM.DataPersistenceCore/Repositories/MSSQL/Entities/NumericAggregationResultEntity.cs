using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class NumericAggregationResultEntity : IHaveAggregatedResult<double>
    {
        public double Value { get; set; }

    }
}
