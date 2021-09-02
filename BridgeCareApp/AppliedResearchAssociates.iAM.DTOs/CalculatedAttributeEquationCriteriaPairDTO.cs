using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CalculatedAttributeEquationCriteriaPairDTO : BaseDTO
    {
        public CriterionLibraryDTO  CriteriaLibrary { get; set; }

        public EquationDTO Equation { get; set; }
    }
}
