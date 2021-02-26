using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs
{
    public class TreatmentCostDTO : BaseDTO
    {
        public EquationDTO Equation { get; set; }
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
