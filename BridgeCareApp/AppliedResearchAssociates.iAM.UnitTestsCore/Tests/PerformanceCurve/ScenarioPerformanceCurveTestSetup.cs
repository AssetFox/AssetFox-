using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class ScenarioPerformanceCurveTestSetup
    {
        public static ScenarioPerformanceCurveEntity ScenarioEntity(Guid simulationId, Guid attributeId, Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var resolveName = RandomStrings.WithPrefix("Curve name");
            return new ScenarioPerformanceCurveEntity
            {
                Id = resolveId,
                SimulationId = simulationId,
                Name = resolveName,
                Shift = false,
                AttributeId = attributeId,
            };
        }

        /// <summary>
        /// If a criterionLibrary or an equation is passed in, it is expected to NOT yet
        /// be in the db. This setup will add it, but with a different id.
        /// </summary>
        public static PerformanceCurveDTO DtoForEntityInDb(UnitOfDataPersistenceWork unitOfWork, Guid simulationId, Guid curveId, CriterionLibraryDTO criterionLibraryDto = null, string equation = null, string attribute = TestAttributeNames.ActionType)
        {
            var equationDto = equation == null ? null : EquationTestSetup.Dto(equation);
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = attribute,
                Id = curveId,
                Name = "Curve",
                CriterionLibrary = criterionLibraryDto,
                Equation = equationDto,
            };
            var performanceCurves = new List<PerformanceCurveDTO> { performanceCurveDto };
            unitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurvesNonAtomic(performanceCurves, simulationId);
            unitOfWork.Context.SaveChanges();
            var scenarioPerformanceCurves = unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var returnDto = scenarioPerformanceCurves.Single(curve => curve.Id == curveId);
            return returnDto;
        }
    }
}
