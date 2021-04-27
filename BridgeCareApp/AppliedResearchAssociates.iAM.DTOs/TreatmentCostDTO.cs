using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TreatmentCostDTO : BaseDTO
    {
        public EquationDTO Equation { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
