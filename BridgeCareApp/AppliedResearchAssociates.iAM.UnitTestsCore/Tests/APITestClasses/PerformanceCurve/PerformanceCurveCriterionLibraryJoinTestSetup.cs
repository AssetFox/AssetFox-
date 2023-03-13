using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveCriterionLibraryJoinTestSetup
    {
        public static void JoinPerformanceCurveToCriterionLibrary(
            UnitOfDataPersistenceWork unitOfWork,
            Guid performanceCurveId,
            string simulationName,
            string mergedCriteriaExpression)
        {
            var dictionary = new Dictionary<string, List<Guid>>();
            var guids = new List<Guid> { performanceCurveId };
            dictionary[mergedCriteriaExpression] = guids;
            unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(dictionary, "PerformanceCurveEntity", simulationName);
            unitOfWork.Context.SaveChanges();
        }
    }
}
