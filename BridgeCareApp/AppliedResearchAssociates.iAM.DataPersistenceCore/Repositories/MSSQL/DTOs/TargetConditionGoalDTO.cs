using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class TargetConditionGoalDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Attribute { get; set; }

        public double Target { get; set; }

        public int? Year { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
