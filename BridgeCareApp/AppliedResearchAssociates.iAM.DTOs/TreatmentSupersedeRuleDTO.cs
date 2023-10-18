using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TreatmentSupersedeRuleDTO: BaseDTO
    {
        public TreatmentDTO treatment { get; set; } // Prevent treatment

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
