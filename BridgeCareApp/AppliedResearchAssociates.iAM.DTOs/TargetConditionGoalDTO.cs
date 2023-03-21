using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TargetConditionGoalDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Attribute { get; set; }

        public double Target { get; set; }

        public int? Year { get; set; }

        public Guid LibraryId { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
