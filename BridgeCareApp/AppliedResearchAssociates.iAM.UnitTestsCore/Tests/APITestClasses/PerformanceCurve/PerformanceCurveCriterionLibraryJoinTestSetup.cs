using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class PerformanceCurveCriterionLibraryJoinTestSetup
    {
        public static void JoinPerformanceCurveToCriterionLibrary(
            IUnitOfWork unitOfWork,
            Guid performanceCurveId,
            Guid criterionLibraryId)
        {
            var criterionCurveJoin = new CriterionLibraryPerformanceCurveEntity
            {
                PerformanceCurveId = performanceCurveId,
                CriterionLibraryId = criterionLibraryId
            };
            unitOfWork.Context.Add(criterionCurveJoin);
            unitOfWork.Context.SaveChanges();
        }
    }
}
