using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class DeficientConditionGoalDTO : BaseDTO
    {
        public string Name { get; set; }
        public string Attribute { get; set; }
        public double AllowedDeficientPercentage { get; set; }
        public double DeficientLimit { get; set; }
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
