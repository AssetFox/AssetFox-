using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Defines a criteria-equation pair used with calculated attributes
    /// </summary>
    public class CalculatedAttributeEquationCriteriaPairDTO : BaseDTO
    {
        /// <summary>
        /// Defines the assets that can use this equation
        /// </summary>
        public CriterionLibraryDTO  CriteriaLibrary { get; set; }

        /// <summary>
        /// Defines the equation
        /// </summary>
        public EquationDTO Equation { get; set; }
    }
}
