using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class PerformanceCurveDTO : BaseDTO
    {
        public string Attribute { get; set; }

        public string Name { get; set; }

        public bool Shift { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }

        public EquationDTO Equation { get; set; }
    }
}
