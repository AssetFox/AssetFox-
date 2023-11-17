using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// SupersedeRule export dto for a Treatment
    /// </summary>
    public class TreatmentSupersedeRuleExportDTO : BaseDTO
    {
        /// <summary>
        /// Treatment name for which rule getting added
        /// </summary>
        public string TreatmentName { get; set; }

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
