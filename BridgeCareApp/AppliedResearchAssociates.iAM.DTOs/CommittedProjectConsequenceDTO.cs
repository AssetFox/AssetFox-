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

        public Guid CommittedProjectId { get; set; }

        public string Attribute { get; set; }

        public string ChangeValue { get; set; }

        public float PerformanceFactor { get; set; }
    }
}
