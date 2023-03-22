using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class DeficientConditionGoalDTO : BaseDTO
    {
        public string Name { get; set; }

        public string Attribute { get; set; }

        public double AllowedDeficientPercentage { get; set; }

        public double DeficientLimit { get; set; }

        public Guid LibraryId { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
