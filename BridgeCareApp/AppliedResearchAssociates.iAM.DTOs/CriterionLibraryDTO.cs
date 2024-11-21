using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    /// <summary>
    /// Contains the logic used in decision making (i.e., query)
    /// </summary>
    public class CriterionLibraryDTO : BaseLibraryDTO
    {
        /// <summary>
        /// Text expression used to define the query
        /// </summary>
        public string MergedCriteriaExpression { get; set; }

        /// <summary>
        /// Is this criteria expression only meant to be used
        /// once?
        /// </summary>
        /// <remarks>
        /// At the moment, this is always true
        /// </remarks>
        public bool IsSingleUse { get; set; }
    }
}
