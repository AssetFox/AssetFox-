using System;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class PerformanceCurveDTO : BaseDTO
    {
        public string Attribute { get; set; }

        public string Name { get; set; }

        public Guid LibraryId { get; set; }

        public bool IsModified { get; set; }

        public bool Shift { get; set; }

        public CriterionLibraryDTO CriterionLibrary { get; set; }

        public EquationDTO Equation { get; set; }
    }
}
