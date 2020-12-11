using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class NumericAttributeValueHistoryMostRecentValueEntity : AttributeValueHistoryEntity
    {
        public double MostRecentValue { get; set; }
    }
}
