using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Assert = Xunit.Assert;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Library_API_Test_Classes
{
    public class PerformanceCurveTests
    {
        private readonly TestHelper _testHelper;
        private readonly PerformanceCurveController _controller;

        private static readonly Guid PerformanceCurveLibraryId = Guid.Parse("1bcee741-02a5-4375-ac61-2323d45752b4");
        private static readonly Guid PerformanceCurveId = Guid.Parse("ce1c926b-4df7-4c3c-987f-9146756111b8");
        private static readonly Guid EquationId = Guid.Parse("a6c65132-e45c-4a48-a0b2-72cd274c9cc2");

        public PerformanceCurveTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _controller = new PerformanceCurveController(_testHelper.UnitOfDataPersistenceWork, _testHelper.MockEsecSecurity);
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

        public EquationEntity TestEquation { get; } = new EquationEntity
        {
            Id = EquationId,
            Expression = "Test Expression"
        };

        private void SetupForGet()
        {
            _testHelper.UnitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Add(TestPerformanceCurveLibrary);
            var attribute = _testHelper.UnitOfDataPersistenceWork.Context.Attribute.First();
            TestPerformanceCurve.AttributeId = attribute.Id;
            _testHelper.UnitOfDataPersistenceWork.Context.PerformanceCurve.Add(TestPerformanceCurve);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            _testHelper.UnitOfDataPersistenceWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfDataPersistenceWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.PerformanceCurveLibraries();

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
                // Act
                var result = await _controller
                    .UpsertPerformanceCurveLibrary(Guid.Empty, TestPerformanceCurveLibrary.ToDto());

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
        public async void ShouldGetAllPerformanceCurveLibrariesWithPerformanceCurves()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.PerformanceCurveLibraries();

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
                SetupForUpsertOrDelete();
                var getResult = await _controller.PerformanceCurveLibraries();
                var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<PerformanceCurveLibraryDTO>));

                var performanceCurveLibraryDTO = dtos[0];
                performanceCurveLibraryDTO.Description = "Updated Description";
                performanceCurveLibraryDTO.PerformanceCurves[0].Shift = true;
                performanceCurveLibraryDTO.PerformanceCurves[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();
                performanceCurveLibraryDTO.PerformanceCurves[0].Equation = TestEquation.ToDto();

                // Act
                var result =
                    await _controller.UpsertPerformanceCurveLibrary(_testHelper.TestSimulation.Id,
                        performanceCurveLibraryDTO);

                // Assert
                Assert.IsType<OkResult>(result);

                var performanceCurveLibraryEntity = _testHelper.UnitOfDataPersistenceWork.Context.PerformanceCurveLibrary
                    .Include(_ => _.PerformanceCurves)
                    .ThenInclude(_ => _.CriterionLibraryPerformanceCurveJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.PerformanceCurves)
                    .ThenInclude(_ => _.PerformanceCurveEquationJoin)
                    .ThenInclude(_ => _.Equation)
                    .Include(_ => _.PerformanceCurves)
                    .ThenInclude(_ => _.Attribute)
                    .Include(_ => _.PerformanceCurveLibrarySimulationJoins)
                    .Single(_ => _.Id == PerformanceCurveLibraryId);

                Assert.Equal(performanceCurveLibraryDTO.Description, performanceCurveLibraryEntity.Description);
                Assert.Single(performanceCurveLibraryEntity.PerformanceCurveLibrarySimulationJoins);
                var performanceCurveLibrarySimulationJoin =
                    performanceCurveLibraryEntity.PerformanceCurveLibrarySimulationJoins.ToList()[0];
                Assert.Equal(_testHelper.TestSimulation.Id, performanceCurveLibrarySimulationJoin.SimulationId);
                var performanceCurveEntity = performanceCurveLibraryEntity.PerformanceCurves.ToList()[0];
                Assert.Equal(performanceCurveLibraryDTO.PerformanceCurves[0].Shift, performanceCurveEntity.Shift);
                Assert.NotNull(performanceCurveEntity.CriterionLibraryPerformanceCurveJoin);
                Assert.Equal(performanceCurveLibraryDTO.PerformanceCurves[0].CriterionLibrary.Id,
                    performanceCurveEntity.CriterionLibraryPerformanceCurveJoin.CriterionLibrary.Id);
                Assert.NotNull(performanceCurveEntity.PerformanceCurveEquationJoin);
                Assert.Equal(performanceCurveLibraryDTO.PerformanceCurves[0].Equation.Id,
                    performanceCurveEntity.PerformanceCurveEquationJoin.Equation.Id);
                Assert.NotNull(performanceCurveEntity.Attribute);
                Assert.Equal(performanceCurveLibraryDTO.PerformanceCurves[0].Attribute,
                    performanceCurveEntity.Attribute.Name);
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
                SetupForUpsertOrDelete();
                var getResult = await _controller.PerformanceCurveLibraries();
                var dtos = (List<PerformanceCurveLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<PerformanceCurveLibraryDTO>));

                var performanceCurveLibraryDTO = dtos[0];
                performanceCurveLibraryDTO.PerformanceCurves[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.UpsertPerformanceCurveLibrary(_testHelper.TestSimulation.Id,
                    performanceCurveLibraryDTO);

                // Act
                var result = _controller.DeletePerformanceCurveLibrary(PerformanceCurveLibraryId);

                // Assert
                Assert.IsType<OkResult>(result.Result);

                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.PerformanceCurveLibrary.Any(_ => _.Id == PerformanceCurveLibraryId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.PerformanceCurve.Any(_ => _.Id == PerformanceCurveId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.PerformanceCurveLibrarySimulation.Any(_ =>
                    _.PerformanceCurveLibraryId == PerformanceCurveLibraryId));
                Assert.True(
                    !_testHelper.UnitOfDataPersistenceWork.Context.CriterionLibraryPerformanceCurve.Any(_ =>
                        _.PerformanceCurveId == PerformanceCurveId));
                Assert.True(
                    !_testHelper.UnitOfDataPersistenceWork.Context.PerformanceCurveEquation.Any(_ =>
                        _.PerformanceCurveId == PerformanceCurveId));
                Assert.True(!_testHelper.UnitOfDataPersistenceWork.Context.Equation.Any(_ => _.Id == EquationId));
                Assert.True(
                    !_testHelper.UnitOfDataPersistenceWork.Context.Attribute.Any(_ => _.PerformanceCurves.Any()));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
