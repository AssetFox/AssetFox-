using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Represents the costs of applying a specific treatment to an asset.
    /// A treatment may have multiple costs as the cost may depend on a
    /// factor defined by an asset's attributes.  For example, the cost of
    /// replacing an asset on an interstate may be higher than the cost of
    /// replacing an asset on a local road.
    /// </summary>
    public class TreatmentCostDTO : BaseDTO
    {
        /// <summary>
        /// The equation used to calculate the cost of a treatment.
        /// </summary>
        public EquationDTO Equation { get; set; }

        /// <summary>
        /// Defines the assets that will be affected by this cost
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
