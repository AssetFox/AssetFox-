using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CriterionLibraryDTO : BaseLibraryDTO
    {
        public string MergedCriteriaExpression { get; set; }

        public bool IsSingleUse { get; set; }
    }
}
