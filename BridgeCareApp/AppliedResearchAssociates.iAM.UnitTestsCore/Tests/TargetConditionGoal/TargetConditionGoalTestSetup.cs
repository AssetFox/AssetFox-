using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TargetConditionGoal
{
    public static class TargetConditionGoalTestSetup
    {
        public static TargetConditionGoalDTO ModelForLibraryGoalInDb(Guid targetConditionGoalLibraryId, string attributeName)
        {
            var targetConditionGoal = TargetConditionGoalDtos.Dto(attributeName);
            var targetConditionGoals = new List<TargetConditionGoalDTO> { targetConditionGoal };
            TestHelper.UnitOfWork.TargetConditionGoalRepo.UpsertOrDeleteTargetConditionGoals(targetConditionGoals, targetConditionGoalLibraryId);
            return targetConditionGoal;
        }
    }
}
