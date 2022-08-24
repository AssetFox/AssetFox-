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
        public static CriterionLibraryEntity TestCriterionLibrary(Guid? id = null, string? name = null)
        {
            var resolvedId = id ?? Guid.NewGuid();
            var resolvedName = name ?? "Test Criterion " + RandomStrings.Length11();
            var returnValue = new CriterionLibraryEntity
            {
                Id = resolvedId,
                Name = resolvedName,
                MergedCriteriaExpression = "Test Expression"
            };
            return returnValue;
        }


        public static CriterionLibraryEntity TestCriterionLibraryInDb(IUnitOfWork unitOfWork)
        {
            var criterionLibrary = TestCriterionLibrary();
            unitOfWork.Context.CriterionLibrary.Add(criterionLibrary);
            unitOfWork.Context.SaveChanges();
            return criterionLibrary;
        }
    }
}
