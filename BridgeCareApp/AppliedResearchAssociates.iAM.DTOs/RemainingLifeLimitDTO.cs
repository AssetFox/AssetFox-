using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class RemainingLifeLimitDTO : BaseDTO
    {
        public string Attribute { get; set; }

        public double Value { get; set; }

        public Guid LibraryId { get; set; }

        public bool IsModified { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }
    }
}
