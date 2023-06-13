using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Represents the consequence of applying a specific treatment to an asset.
    /// A treatment may have multiple consequences as a treatment may affect
    /// multiple conditions (e.g., any reconstruction)
    /// </summary>
    public class TreatmentConsequenceDTO : BaseDTO
    {
        /// <summary>
        /// Name of the attribute representing the condition to be changed
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// The change in the value.  This can be a relative change (indicated by + or - before the value
        /// without a % at the end) related to the current value of the attribute, a percentage change
        /// indicated by a % in the string, or an absoulte value (a change that ignores the current value
        /// of the attribute) represented by no +, -, or % in the string.  This should not be used when
        /// an equation is specified.
        /// </summary>
        public string ChangeValue { get; set; }

        /// <summary>
        /// The equation used to calculate the change in value.  This should
        /// not be used when a change value is specified.
        /// </summary>
        public EquationDTO Equation { get; set; }

        /// <summary>
        /// Defines the assets that will be affected by this consequence
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
