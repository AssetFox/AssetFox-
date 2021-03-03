using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class TreatmentConsequenceDTO : BaseDTO
    {
        public string Attribute { get; set; }

        public string ChangeValue { get; set; }

        public EquationDTO Equation { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
