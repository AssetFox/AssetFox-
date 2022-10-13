using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class CriterionLibraryTestSetup
    {
        public static CriterionLibraryEntity TestCriterionLibrary(string? name = null)
        {
            var resolvedId = Guid.NewGuid();
            var resolvedName = name ?? "Test Criterion " + RandomStrings.Length11();
            var mergedCriteriaExpression = RandomStrings.WithPrefix("Test Expression");
            var returnValue = new CriterionLibraryEntity
            {
                Id = resolvedId,
                Name = resolvedName,
                MergedCriteriaExpression = mergedCriteriaExpression,
            };
            return returnValue;
        }


        public static CriterionLibraryEntity TestCriterionLibraryInDb(IUnitOfWork unitOfWork, string name = null)
        {
            var criterionLibrary = TestCriterionLibrary(name);
            unitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            unitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }
    }
}
