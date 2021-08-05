using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
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
    public class PerformanceCurveTests
    {
        private readonly TestHelper _testHelper;
        private PerformanceCurveController _controller;

        private static readonly Guid PerformanceCurveLibraryId = Guid.Parse("1bcee741-02a5-4375-ac61-2323d45752b4");
        private static readonly Guid PerformanceCurveId = Guid.Parse("ce1c926b-4df7-4c3c-987f-9146756111b8");
        private static readonly Guid ScenarioPerformanceCurveId = Guid.Parse("5451c55f-05a7-4dda-8fc0-e925b52d7af1");
        private static readonly Guid EquationId = Guid.Parse("a6c65132-e45c-4a48-a0b2-72cd274c9cc2");

        public PerformanceCurveTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            /*_controller = new PerformanceCurveController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);*/
        }

        public PerformanceCurveLibraryEntity TestPerformanceCurveLibrary { get; } = new PerformanceCurveLibraryEntity
        {
            Id = PerformanceCurveLibraryId,
            Name = "Test Name"
        };

        public PerformanceCurveEntity TestPerformanceCurve { get; } = new PerformanceCurveEntity
        {
            Id = PerformanceCurveId,
            PerformanceCurveLibraryId = PerformanceCurveLibraryId,
            Name = "Test Name",
            Shift = false
        };

        public ScenarioPerformanceCurveEntity TestScenarioPerformanceCurve { get; } = new ScenarioPerformanceCurveEntity
        {
            Id = ScenarioPerformanceCurveId,
            Name = "Test Name",
            Shift = false
        };

        public EquationEntity TestEquation { get; } = new EquationEntity
        {
            Id = EquationId,
            Expression = "Test Expression"
        };

        private void SetupController(Moq.Mock<IEsecSecurity> mockedEsecSecurity) =>
            _controller = new PerformanceCurveController(
                mockedEsecSecurity.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

        private void SetupForGet()
        {
            _testHelper.UnitOfWork.Context.PerformanceCurveLibrary.Add(TestPerformanceCurveLibrary);
            TestPerformanceCurve.AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id;
            _testHelper.UnitOfWork.Context.PerformanceCurve.Add(TestPerformanceCurve);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void AddTestCriterionLibrary()
        {
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            AddTestCriterionLibrary();
        }

        private void SetupForScenarioCurveGet()
        {
            TestScenarioPerformanceCurve.SimulationId = _testHelper.TestSimulation.Id;
            TestScenarioPerformanceCurve.AttributeId = _testHelper.UnitOfWork.Context.Attribute.First().Id;
            _testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Add(TestScenarioPerformanceCurve);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForScenarioCurveUpsertOrDelete()
        {
            SetupForScenarioCurveGet();
            AddTestCriterionLibrary();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);

                // Act
                var result = await _controller.GetPerformanceCurveLibraries();

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnPost()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);

                // Act
                var result = await _controller
                    .UpsertPerformanceCurveLibrary(TestPerformanceCurveLibrary.ToDto());

                // Assert
                Assert.IsType<OkResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnDelete()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);

                // Act
                var result = await _controller.DeletePerformanceCurveLibrary(Guid.Empty);

                // Assert
                Assert.IsType<OkResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnScenarioCurveGet()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);

                // Act
                var result = await _controller.GetScenarioPerformanceCurves(_testHelper.TestSimulation.Id);

                // Assert
                Assert.IsType<OkObjectResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnOkResultOnScenarioCurvePost()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);
                TestScenarioPerformanceCurve.Attribute = _testHelper.UnitOfWork.Context.Attribute.First();

                // Act
                var result = await _controller
                    .UpsertScenarioPerformanceCurves(_testHelper.TestSimulation.Id,
                        new List<PerformanceCurveDTO> {TestScenarioPerformanceCurve.ToDto()});

                // Assert
                Assert.IsType<OkResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetAllPerformanceCurveLibrariesWithPerformanceCurves()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);
                SetupForGet();

                // Act
                var result = await _controller.GetPerformanceCurveLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<PerformanceCurveLibraryDTO>));
                Assert.Single(dtos);

                Assert.Equal(PerformanceCurveLibraryId, dtos[0].Id);
                Assert.Single(dtos[0].PerformanceCurves);

                Assert.Equal(PerformanceCurveId, dtos[0].PerformanceCurves[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyPerformanceCurveData()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);
                SetupForUpsertOrDelete();
                var getResult = await _controller.GetPerformanceCurveLibraries();
                var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<PerformanceCurveLibraryDTO>));

                var dto = dtos[0];
                dto.Description = "Updated Description";
                dto.PerformanceCurves[0].Shift = true;
                dto.PerformanceCurves[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();
                dto.PerformanceCurves[0].Equation = TestEquation.ToDto();

                // Act
                await _controller.UpsertPerformanceCurveLibrary(dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.PerformanceCurveRepo
                        .GetPerformanceCurveLibrariesWithPerformanceCurves()[0];
                    Assert.Equal(dto.Description, modifiedDto.Description);

                    Assert.Equal(dto.PerformanceCurves[0].Shift, modifiedDto.PerformanceCurves[0].Shift);
                    Assert.Equal(dto.PerformanceCurves[0].CriterionLibrary.Id,
                        modifiedDto.PerformanceCurves[0].CriterionLibrary.Id);
                    Assert.Equal(dto.PerformanceCurves[0].Equation.Id,
                        modifiedDto.PerformanceCurves[0].Equation.Id);
                    Assert.Equal(dto.PerformanceCurves[0].Attribute,
                        modifiedDto.PerformanceCurves[0].Attribute);
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeletePerformanceCurveData()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);
                SetupForUpsertOrDelete();
                var getResult = await _controller.GetPerformanceCurveLibraries();
                var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<PerformanceCurveLibraryDTO>));

                var performanceCurveLibraryDTO = dtos[0];
                performanceCurveLibraryDTO.PerformanceCurves[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.UpsertPerformanceCurveLibrary(performanceCurveLibraryDTO);

                // Act
                var result = _controller.DeletePerformanceCurveLibrary(PerformanceCurveLibraryId);

                // Assert
                Assert.IsType<OkResult>(result.Result);

                Assert.True(!_testHelper.UnitOfWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == PerformanceCurveLibraryId));
                Assert.True(!_testHelper.UnitOfWork.Context.PerformanceCurve.Any(_ => _.Id == PerformanceCurveId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryPerformanceCurve.Any(_ =>
                        _.PerformanceCurveId == PerformanceCurveId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.PerformanceCurveEquation.Any(_ =>
                        _.PerformanceCurveId == PerformanceCurveId));
                Assert.True(!_testHelper.UnitOfWork.Context.Equation.Any(_ => _.Id == EquationId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.Attribute.Any(_ => _.PerformanceCurves.Any()));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetAllScenarioPerformanceCurveData()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);
                SetupForScenarioCurveGet();

                // Act
                var result = await _controller.GetScenarioPerformanceCurves(_testHelper.TestSimulation.Id);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<PerformanceCurveDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<PerformanceCurveDTO>));
                Assert.Single(dtos);
                Assert.Equal(TestScenarioPerformanceCurve.Id, dtos[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyScenarioPerformanceCurveData()
        {
            try
            {
                // Arrange
                SetupController(_testHelper.MockEsecSecurityAuthorized);
                var attribute = _testHelper.UnitOfWork.Context.Attribute.First();

                var deletedCurveId = Guid.NewGuid();
                _testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Add(new ScenarioPerformanceCurveEntity
                {
                    Id = deletedCurveId,
                    SimulationId = _testHelper.TestSimulation.Id,
                    AttributeId = attribute.Id,
                    Shift = false,
                    Name = "Deleted"
                });

                SetupForScenarioCurveUpsertOrDelete();

                var localScenarioPerformanceCurves = _testHelper.UnitOfWork.PerformanceCurveRepo
                    .GetScenarioPerformanceCurves(_testHelper.TestSimulation.Id);
                localScenarioPerformanceCurves[0].Name = "Updated";
                localScenarioPerformanceCurves[0].CriterionLibrary = _testHelper.TestCriterionLibrary.ToDto();
                localScenarioPerformanceCurves[0].Equation = TestEquation.ToDto();
                localScenarioPerformanceCurves.Add(new PerformanceCurveDTO
                {
                    Id = Guid.NewGuid(),
                    Attribute= attribute.Name,
                    Shift = false,
                    Name = "New"
                });

                // Act
                await _controller.UpsertScenarioPerformanceCurves(_testHelper.TestSimulation.Id, localScenarioPerformanceCurves);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var serverScenarioPerformanceCurves = _testHelper.UnitOfWork.PerformanceCurveRepo
                        .GetScenarioPerformanceCurves(_testHelper.TestSimulation.Id);
                    Assert.Equal(localScenarioPerformanceCurves.Count, serverScenarioPerformanceCurves.Count);

                    Assert.True(
                        !_testHelper.UnitOfWork.Context.ScenarioPerformanceCurve.Any(_ => _.Id == deletedCurveId));

                    var localNewCurve = localScenarioPerformanceCurves.Single(_ => _.Name == "New");
                    var serverNewCurve = serverScenarioPerformanceCurves.FirstOrDefault(_ => _.Id == localNewCurve.Id);
                    Assert.NotNull(serverNewCurve);
                    Assert.Equal(localNewCurve.Attribute, serverNewCurve.Attribute);

                    var localUpdatedCurve = localScenarioPerformanceCurves.Single(_ => _.Id == ScenarioPerformanceCurveId);
                    var serverUpdatedCurve = serverScenarioPerformanceCurves
                        .FirstOrDefault(_ => _.Id == ScenarioPerformanceCurveId);
                    Assert.Equal(localUpdatedCurve.Name, serverUpdatedCurve.Name);
                    Assert.Equal(localUpdatedCurve.Attribute, serverUpdatedCurve.Attribute);
                    Assert.Equal(localUpdatedCurve.CriterionLibrary.Id, serverUpdatedCurve.CriterionLibrary.Id);
                    Assert.Equal(localUpdatedCurve.CriterionLibrary.MergedCriteriaExpression,
                        serverUpdatedCurve.CriterionLibrary.MergedCriteriaExpression);
                    Assert.Equal(localUpdatedCurve.Equation.Id, serverUpdatedCurve.Equation.Id);
                    Assert.Equal(localUpdatedCurve.Equation.Expression, serverUpdatedCurve.Equation.Expression);
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldReturnUnauthorizedOnPost()
        {
            try
            {
                // Arrange
                _testHelper.SetupDefaultHttpContext();
                var mockedUnauthorized = new Mock<IEsecSecurity>();
                mockedUnauthorized.Setup(_ => _.GetUserInformation(It.IsAny<HttpRequest>()))
                    .Returns(new UserInfo
                    {
                        Name = "districtEngineer", Role = Role.DistrictEngineer, Email = "fake@pa.gov"
                    });
                SetupController(mockedUnauthorized);
                TestScenarioPerformanceCurve.Attribute = _testHelper.UnitOfWork.Context.Attribute.First();

                // Act
                var upsertScenarioPerformanceCurveLibraryResult = await _controller
                    .UpsertScenarioPerformanceCurves(_testHelper.TestSimulation.Id,
                        new List<PerformanceCurveDTO> {TestScenarioPerformanceCurve.ToDto()});

                // Assert
                Assert.IsType<UnauthorizedResult>(upsertScenarioPerformanceCurveLibraryResult);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
