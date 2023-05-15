using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// .
    /// </summary>
    public class CriterionLibraryDTO : BaseLibraryDTO
    {
        /// <summary>
        /// .
        /// </summary>
        public string MergedCriteriaExpression { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public bool IsSingleUse { get; set; }
    }
}
