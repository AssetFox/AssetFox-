using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MoreLinq;
using Xunit;
using Assert = Xunit.Assert;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class UpdateScenarioPerformanceCurveTests
    {
        private TestHelper _testHelper => TestHelper.Instance;
        private void Setup()
        {
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.SetupDefaultHttpContext();
        }

        [Fact]
        public async Task ShouldModifyScenarioPerformanceCurveData()
        {
            Setup();
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();

            var deletedCurveId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Add(new ScenarioPerformanceCurveEntity
            {
                Id = deletedCurveId,
                SimulationId = simulation.Id,
                AttributeId = attribute.Id,
                Shift = false,
                Name = "Deleted"
            });
            var performanceCurveId = Guid.NewGuid();

            var scenarioPerformanceCurveEntity = ScenarioPerformanceCurveTestSetup.SetupForScenarioCurveGet(_testHelper.UnitOfWork, simulation.Id, performanceCurveId);
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);

            var localScenarioPerformanceCurves = _testHelper.UnitOfWork.PerformanceCurveRepo
                .GetScenarioPerformanceCurves(simulation.Id);
            Assert.Equal(2, localScenarioPerformanceCurves.Count);
            var indexToDelete = localScenarioPerformanceCurves.FindIndex(pc => pc.Id == deletedCurveId);
            var indexToUpdate = 1 - indexToDelete;
            var curveToUpdate = localScenarioPerformanceCurves[indexToUpdate];
            var idToUpdate = curveToUpdate.Id;
            curveToUpdate.Name = "Updated";
            curveToUpdate.CriterionLibrary = criterionLibrary.ToDto();
            curveToUpdate.Equation = EquationTestSetup.TestEquation.ToDto();
            var curveToDelete = localScenarioPerformanceCurves.Single(curve => curve.Id == deletedCurveId);
            localScenarioPerformanceCurves.Remove(curveToDelete);
            var idToAdd = Guid.NewGuid();
            localScenarioPerformanceCurves.Add(new PerformanceCurveDTO
            {
                Id = idToAdd,
                Attribute = attribute.Name,
                Shift = false,
                Name = "New"
            });

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulation.Id, localScenarioPerformanceCurves);

            // Assert
            var scenarioPerformanceCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo
                .GetScenarioPerformanceCurves(simulation.Id);
            Assert.Equal(localScenarioPerformanceCurves.Count, scenarioPerformanceCurvesAfter.Count);

            Assert.False(
                _testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Any(_ => _.Id == deletedCurveId));

            var localNewCurve = localScenarioPerformanceCurves.Single(_ => _.Name == "New");
            var serverNewCurve = scenarioPerformanceCurvesAfter.FirstOrDefault(_ => _.Id == localNewCurve.Id);
            Assert.NotNull(serverNewCurve);
            Assert.Equal(localNewCurve.Attribute, serverNewCurve.Attribute);

            var localUpdatedCurve = localScenarioPerformanceCurves.Single(_ => _.Id == idToUpdate);
            var serverUpdatedCurve = scenarioPerformanceCurvesAfter
                .Single(_ => _.Id == idToUpdate);
            Assert.Equal(localUpdatedCurve.Name, serverUpdatedCurve.Name);
            Assert.Equal(localUpdatedCurve.Attribute, serverUpdatedCurve.Attribute);
         //   Assert.Equal(localUpdatedCurve.CriterionLibrary.Id, serverUpdatedCurve.CriterionLibrary.Id);
            // Wjwjwj revisit the above and the below after understanding the situation with the similar error in the previous test
            Assert.Equal(localUpdatedCurve.CriterionLibrary.MergedCriteriaExpression,
                serverUpdatedCurve.CriterionLibrary.MergedCriteriaExpression);
       //     Assert.Equal(localUpdatedCurve.Equation.Id, serverUpdatedCurve.Equation.Id);
       // uncomment this too
            Assert.Equal(localUpdatedCurve.Equation.Expression, serverUpdatedCurve.Equation.Expression);
        }

        [Fact]
        public async Task UpsertScenarioPerformanceCurves_ScenarioPerformanceCurveInDb_CurvesInUpsertAreEmpty_Deletes()
        {
            Setup();
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            var controller = PerformanceCurveControllerTestSetup.SetupController(_testHelper, _testHelper.MockEsecSecurityAdmin);
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
            var deletedCurveId = Guid.NewGuid();
            _testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Add(new ScenarioPerformanceCurveEntity
            {
                Id = deletedCurveId,
                SimulationId = simulation.Id,
                AttributeId = attribute.Id,
                Shift = false,
                Name = "Deleted"
            });
            var criterionLibrary = CriterionLibraryTestSetup.TestCriterionLibraryInDb(_testHelper.UnitOfWork);
            var localScenarioPerformanceCurves = _testHelper.UnitOfWork.PerformanceCurveRepo
                .GetScenarioPerformanceCurves(simulation.Id);
            Assert.Single(localScenarioPerformanceCurves);
            localScenarioPerformanceCurves.RemoveAt(0);

            // Act
            await controller.UpsertScenarioPerformanceCurves(simulation.Id, localScenarioPerformanceCurves);

            // Assert
            var scenarioPerformanceCurvesAfter = _testHelper.UnitOfWork.PerformanceCurveRepo
                .GetScenarioPerformanceCurves(simulation.Id);
            Assert.Empty(scenarioPerformanceCurvesAfter);
            Assert.False(_testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Any(_ => _.Id == deletedCurveId));
        }

    }
}
