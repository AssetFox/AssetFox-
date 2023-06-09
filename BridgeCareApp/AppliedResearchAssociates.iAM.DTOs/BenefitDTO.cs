using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Describes the attribute used to determine the benefit based
    /// on the improvement to conditions of this specific attribute
    /// </summary>
    public class BenefitDTO : BaseDTO
    {
        /// <summary>
        /// The attribute used to determine benefit
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Improvements to the benefit attribute below this limit
        /// do not affect the benefit calculation
        /// </summary>
        /// <example>
        /// Since a PCI of 20 and a PCI of 0 on a pavement will result
        /// in a replacement and no additional returns from other factors
        /// such as safety are seen, there is no difference in the actual
        /// benefit of applying a treatment to the pavement with a PCI
        /// of 20 versus applying the treatment to the pavement with a
        /// PCI of 0
        /// </example>
        /// <remarks>
        /// Set to 0 if this feature is not used in this simulation
        /// </remarks>
        public double Limit { get; set; }
    }
}
