using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class TextAttributeValueHistoryEntity : AttributeValueHistoryEntity
    {
        public int Year { get; set; }

        public string Value { get; set; }
    }
}
