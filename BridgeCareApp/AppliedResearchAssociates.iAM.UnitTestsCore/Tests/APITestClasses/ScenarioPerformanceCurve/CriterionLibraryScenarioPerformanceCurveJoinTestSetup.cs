using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class CriterionLibraryScenarioPerformanceCurveJoinTestSetup
    {
        public static void JoinCurveToCriterionLibrary(
            IUnitOfWork unitOfWork,
            Guid performanceCurveId,
            Guid criterionLibraryId) {
            var criterionCurveJoin = new CriterionLibraryScenarioPerformanceCurveEntity
            {
                ScenarioPerformanceCurveId = performanceCurveId,
                CriterionLibraryId = criterionLibraryId
            };
            unitOfWork.Context.Add(criterionCurveJoin);
            unitOfWork.Context.SaveChanges();
        } 
    }
}
