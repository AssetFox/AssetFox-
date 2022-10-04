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
            IUnitOfWork unitOfWork,
            Guid performanceCurveId,
            Guid criterionLibraryId,
            string simulationName)
        {
            var dictionary = new Dictionary<string, List<Guid>>();
            var guids = new List<Guid> { criterionLibraryId };
            dictionary["Hello"] = guids;
            unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(dictionary, "PerformanceCurveEntity", simulationName);
            //var criterionCurveJoin = new CriterionLibraryPerformanceCurveEntity
            //{
            //    PerformanceCurveId = performanceCurveId,
            //    CriterionLibraryId = criterionLibraryId
            //};
            //unitOfWork.Context.Add(criterionCurveJoin);
            //unitOfWork.Context.SaveChanges();
        }
    }
}
