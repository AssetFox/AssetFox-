using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class DeficientConditionGoalLibraryDTO : BaseLibraryDTO
    {
        public List<DeficientConditionGoalDTO> DeficientConditionGoals { get; set; }
    }
}
