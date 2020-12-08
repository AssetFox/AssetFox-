using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TextAttributeValueHistoryMostRecentValueEntity : AttributeValueHistoryEntity
    {
        public string MostRecentValue { get; set; }
    }
}
