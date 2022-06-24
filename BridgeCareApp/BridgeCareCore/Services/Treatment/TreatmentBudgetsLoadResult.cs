using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.Treatment
{
    public class TreatmentBudgetsLoadResult
    {
        public List<Guid> budgetIds { get; set; }

        public List<string> ValidationMessages { get; set; }
    }
}
