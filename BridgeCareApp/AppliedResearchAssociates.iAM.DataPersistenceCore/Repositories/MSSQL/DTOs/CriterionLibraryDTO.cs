using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class CriterionLibraryDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string MergedCriteriaExpression { get; set; }
    }
}
