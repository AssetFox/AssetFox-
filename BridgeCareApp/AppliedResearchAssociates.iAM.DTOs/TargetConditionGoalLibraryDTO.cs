using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class TargetConditionGoalLibraryDTO : BaseLibraryDTO
    {
        public List<TargetConditionGoalDTO> TargetConditionGoals { get; set; }
    }
}
