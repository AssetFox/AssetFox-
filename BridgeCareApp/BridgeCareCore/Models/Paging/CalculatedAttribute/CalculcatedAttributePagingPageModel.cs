using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Models
{
    public class CalculcatedAttributePagingPageModel : PagingPageModel<CalculatedAttributeEquationCriteriaPairDTO>
    {
        public int CalculationTiming { get; set; }

        public Guid LibraryId { get; set; }

        public bool IsModified { get; set; }

        public CalculatedAttributeEquationCriteriaPairDTO DefaultEquation { get; set; }
    }
}
