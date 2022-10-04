﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class CriterionLibraryTestSetup // WjTestSetupDto
    {
        public static CriterionLibraryDTO TestCriterionLibrary(string? namePrefix = null, string mergedCriteriaExpression = null)
        {
            var resolvedNamePrefix = namePrefix ?? "Test Criterion Library ";
            var resolvedCriteriaExpression = mergedCriteriaExpression ?? "Test Expression";
            var id = Guid.NewGuid();
            var resolvedName = resolvedNamePrefix + RandomStrings.Length11();
            var returnValue = new CriterionLibraryDTO
            {
                Id = id,
                Name = resolvedName,
                MergedCriteriaExpression = resolvedCriteriaExpression,
            };
            return returnValue;
        }


        public static CriterionLibraryDTO TestCriterionLibraryInDb(IUnitOfWork unitOfWork, string namePrefix = null, string mergedCreteriaExpression = null)
        {
            var resolvedNamePrefix = namePrefix ?? "TestCriterionLibrary";
            var resolvedExpression = mergedCreteriaExpression ?? "Test Expression";
            var criterionLibrary = TestCriterionLibrary(resolvedNamePrefix, resolvedExpression);
            unitOfWork.CriterionLibraryRepo.UpsertCriterionLibrary(criterionLibrary);
            return criterionLibrary;
        }
    }
}
