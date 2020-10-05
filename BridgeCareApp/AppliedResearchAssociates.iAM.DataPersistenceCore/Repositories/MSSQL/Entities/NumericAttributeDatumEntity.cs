using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class NumericAttributeDatumEntity : AttributeDatumEntity, IHaveAttributeDatumValue<double>
    {
        public double Value { get; set; }
    }
}
