using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// SupersedeRule for a Treatment
    /// </summary>
    public class TreatmentSupersedeRuleDTO: BaseDTO
    {
        /// <summary>
        /// The treatment that is being superseded
        /// </summary>
        public TreatmentDTO treatment { get; set; }

        /// <summary>
        /// The criteria that is used to determine if the supersede rule should be applied
        /// </summary>
        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
