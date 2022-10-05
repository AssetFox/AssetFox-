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
    public static class ScenarioPerformanceCurveTestSetup // WjTestSetupDto
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


        public static PerformanceCurveDTO DtoForEntityInDb(IUnitOfWork unitOfWork, Guid simulationId, Guid curveId)
        {
            var performanceCurveDto = new PerformanceCurveDTO
            {
                Attribute = TestAttributeNames.ActionType,
                Id = curveId,
                Name = "Curve",
            };
            var performanceCurves = new List<PerformanceCurveDTO> { performanceCurveDto };
            unitOfWork.PerformanceCurveRepo.UpsertOrDeleteScenarioPerformanceCurves(performanceCurves, simulationId);
            unitOfWork.Context.SaveChanges();
            var scenarioPerformanceCurves = unitOfWork.PerformanceCurveRepo.GetScenarioPerformanceCurves(simulationId);
            var returnDto = scenarioPerformanceCurves.Single(curve => curve.Id == curveId);
            return returnDto;
        }
    }
}
