using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TargetConditionGoal
{
    public class TargetConditionGoalLibraryTestSetup
    {
        private static CriterionLibraryDTO CreateCriterionLibraryObject(string criteriaExpression = "", bool singleUse = false)
        {
            return new CriterionLibraryDTO()
            {
                MergedCriteriaExpression = criteriaExpression,
                IsSingleUse = singleUse
            };
        }
        private static TargetConditionGoalDTO CreateTargetConditionGoalDto(string targetConditionGoalName)
        {
            //create criterion library
            var criterionLibraryObject = CreateCriterionLibraryObject("0=0", true);

            return new TargetConditionGoalDTO()
            {
                Id = Guid.NewGuid(),
                Name = targetConditionGoalName,
                Attribute = "testAttribute",
                Year = 2020,
                Target = 0,
                CriterionLibrary = criterionLibraryObject
            };
        }
        public static TargetConditionGoalLibraryDTO CreateTargetConditionGoalLibraryDto(string name)
        {
            //setup
            var targetConditionGoalList = new List<TargetConditionGoalDTO>();
            targetConditionGoalList.Add(CreateTargetConditionGoalDto("Target Condition Goal 1"));

            //create budget library
            return new TargetConditionGoalLibraryDTO()
            {
                Id = Guid.NewGuid(),
                Name = name,
                TargetConditionGoals = targetConditionGoalList?.ToList(),
            };
        }

        public static TargetConditionGoalLibraryDTO ModelForEntityInDb(IUnitOfWork unitOfWork, string targetConditionGoalLibraryName = null)
        {
            var resolveTargetConditionGoalLibraryName = targetConditionGoalLibraryName ?? RandomStrings.WithPrefix("TargetConditionGoalLibrary");
            var dto = CreateTargetConditionGoalLibraryDto(resolveTargetConditionGoalLibraryName);
            unitOfWork.TargetConditionGoalRepo.UpsertTargetConditionGoalLibrary(dto);
            return dto;
        }
    }
}
