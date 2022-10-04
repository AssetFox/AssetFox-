using System;
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
        public static CriterionLibraryDTO TestCriterionLibrary(string? namePrefix = null)
        {
            var resolvedNamePrefix = namePrefix ?? "Test Criterion Library ";
            var id = Guid.NewGuid();
            var resolvedName = resolvedNamePrefix + RandomStrings.Length11();
            var returnValue = new CriterionLibraryDTO
            {
                Id = id,
                Name = resolvedName,
                MergedCriteriaExpression = "Test Expression"
            };
            return returnValue;
        }


        public static CriterionLibraryDTO TestCriterionLibraryInDb(IUnitOfWork unitOfWork, string namePrefix = null)
        {
            var resolvedNamePrefix = namePrefix ?? "TestCriterionLibrary";
            var criterionLibrary = TestCriterionLibrary(resolvedNamePrefix);
            unitOfWork.CriterionLibraryRepo.UpsertCriterionLibrary(criterionLibrary);
            return criterionLibrary;
        }
    }
}
