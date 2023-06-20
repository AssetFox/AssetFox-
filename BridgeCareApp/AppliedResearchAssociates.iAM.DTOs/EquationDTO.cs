using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Defines an equation
    /// </summary>
    public class EquationDTO : BaseDTO
    {
        /// <summary>
        /// Text that defines the equation
        /// </summary>
        public string Expression { get; set; }
    }
}
