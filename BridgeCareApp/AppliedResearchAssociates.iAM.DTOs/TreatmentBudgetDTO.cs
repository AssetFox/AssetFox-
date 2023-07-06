using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Represents the budget used in a simulation
    /// </summary>
    public class TreatmentBudgetDTO : BaseDTO
    {
        /// <summary>
        /// Name of the budget
        /// </summary>
        public string Name { get; set; }
    }
}
