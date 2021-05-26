using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class CriterionLibraryDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string MergedCriteriaExpression { get; set; }

        public bool ForScenario { get; set; }
    }
}
