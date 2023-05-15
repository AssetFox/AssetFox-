using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class CalculatedAttributeEquationCriteriaPairDTO : BaseDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public CriterionLibraryDTO  CriteriaLibrary { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public EquationDTO Equation { get; set; }
    }
}
