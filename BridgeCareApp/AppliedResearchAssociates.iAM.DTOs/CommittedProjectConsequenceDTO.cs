using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Describes the consequences of a particular treatment applied to a specific committed project
    /// </summary>
    /// <remarks>
    /// TreatmentConsequenceDTO is not used here as the user should ONLY be able to provide ChangeValues
    /// and not consequences.  That may be changed in the future.  A constructor that creates this
    /// based on a TreatmentConsequenceDTO is provided
    /// </remarks>
    public class CommittedProjectConsequenceDTO : BaseDTO
    {
        public CommittedProjectConsequenceDTO() { }

        public CommittedProjectConsequenceDTO(TreatmentConsequenceDTO fullConsequence)
        {
            Attribute = fullConsequence.Attribute;
            ChangeValue = fullConsequence.ChangeValue;
        }

        /// <summary>
        /// ID of the associated committed project
        /// </summary>
        public Guid CommittedProjectId { get; set; }

        /// <summary>
        /// Name of the attribute to be changed
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// The change in the value.  This can be a relative change (indicated by + or - before the value
        /// without a % at the end) related to the current value of the attribute, a percentage change
        /// indicated by a % in the string, or an absoulte value (a change that ignores the current value
        /// of the attribute) represented by no +, -, or % in the string.
        /// </summary>
        public string ChangeValue { get; set; }

        /// <summary>
        /// A factor by which the preformance curve is extended.  The default value is 1 which indicates
        /// no change while larger numbers indicate an increase in the performance window and a number
        /// less than 1 indicates a compression of the performance window.  Negative values are invalid
        /// </summary>
        public float PerformanceFactor { get; set; }
    }
}
