﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class DeficientConditionGoalDtos
    {
        public static DeficientConditionGoalDTO CulvDurationN(Guid? id = null, Guid? criterionLibraryId = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var resolveCriterionLibraryId = criterionLibraryId ?? Guid.NewGuid();
            var dto = new DeficientConditionGoalDTO
            {
                AllowedDeficientPercentage= 100,
                Attribute = TestAttributeNames.CulvDurationN,
                DeficientLimit = 1,
                Id = resolveId,
                Name = "CulvDurationN",
                CriterionLibrary = new CriterionLibraryDTO
                {
                    Id = resolveCriterionLibraryId,
                }
            };
            return dto;
        }


        public static DeficientConditionGoalDTO DtoWithIdOnlyCriterionLibrary(Guid? id = null, string attribute = "Attribute", Guid? criterionLibraryId = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var resolveCriterionLibraryId = criterionLibraryId ?? Guid.NewGuid();
            var dto = new DeficientConditionGoalDTO
            {
                Id = resolveId,
                CriterionLibrary = new CriterionLibraryDTO
                {
                    Id = resolveCriterionLibraryId,
                },
                Name = "Deficient Condition Goal",
                AllowedDeficientPercentage = 1,
                DeficientLimit = 2,
                Attribute = attribute,
            };
            return dto;
        }

        public static DeficientConditionGoalDTO DtoWithCriterionLibrary(Guid? id = null, string attribute = "Attribute", Guid? criterionLibraryId = null)
        {
            var resolveCriterionLibraryId = criterionLibraryId ?? Guid.NewGuid();
            var criterionLibrary = CriterionLibraryDtos.Dto(resolveCriterionLibraryId);
            var resolveId = id ?? Guid.NewGuid();           
            var dto = new DeficientConditionGoalDTO
            {
                Id = resolveId,               
                
                Name = "Deficient Condition Goal",
                AllowedDeficientPercentage = 1,
                DeficientLimit = 2,
                Attribute = attribute,
                CriterionLibrary = criterionLibrary
            };
            return dto;
        }
    }
}
