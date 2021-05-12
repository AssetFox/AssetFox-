using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class TreatmentTests
    {
        private readonly TestHelper _testHelper;
        private readonly TreatmentController _controller;

        private static readonly Guid TreatmentLibraryId = Guid.Parse("39eaafd8-9056-4581-b34c-7a580b395cf2");
        private static readonly Guid TreatmentId = Guid.Parse("27c69c4b-afac-49ca-b28e-4c4be3fbe17f");
        private static readonly Guid CostId = Guid.Parse("32c20689-bc51-4bee-ab64-60817a4d190f");
        private static readonly Guid ConsequenceId = Guid.Parse("8c392b47-b3c8-410a-a713-be53cd55be2b");
        private static readonly Guid BudgetLibraryId = Guid.Parse("6c9de21f-b7f7-40e5-989d-4857105dcccd");
        private static readonly Guid BudgetId = Guid.Parse("4948cc8a-a93a-44ad-915c-b04ac37cd68f");
        private static readonly Guid CostEquationId = Guid.Parse("cd9a030c-dff2-4704-bdf2-1ff65f14f3c0");
        private static readonly Guid ConsequenceEquationId = Guid.Parse("273bb70f-c586-4176-a43d-ecf1f0cfc5d1");

        public TreatmentTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new TreatmentController(_testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        public TreatmentLibraryEntity TestTreatmentLibrary { get; } = new TreatmentLibraryEntity
        {
            Id = TreatmentLibraryId,
            Name = "Test Name"
        };

        public SelectableTreatmentEntity TestTreatment { get; } = new SelectableTreatmentEntity
        {
            Id = TreatmentId,
            TreatmentLibraryId = TreatmentLibraryId,
            Name = "Test Name",
            ShadowForAnyTreatment = 1,
            ShadowForSameTreatment = 1
        };

        public TreatmentCostEntity TestTreatmentCost { get; } = new TreatmentCostEntity
        {
            Id = CostId,
            TreatmentId = TreatmentId
        };

        public ConditionalTreatmentConsequenceEntity TestTeatmentConsequence { get; } =
            new ConditionalTreatmentConsequenceEntity
            {
                Id = ConsequenceId,
                SelectableTreatmentId = TreatmentId,
                ChangeValue = "1"
            };

        public EquationEntity TestCostEquation = new EquationEntity
        {
            Id = CostEquationId,
            Expression = "Test Expression"
        };

        public EquationEntity TestConsequenceEquation = new EquationEntity
        {
            Id = ConsequenceEquationId,
            Expression = "Test Expression"
        };

        public BudgetLibraryEntity TestBudgetLibrary { get; } = new BudgetLibraryEntity
        {
            Id = BudgetLibraryId,
            Name = "Test Name"
        };

        public BudgetEntity TestBudget { get; } = new BudgetEntity
        {
            Id = BudgetId,
            BudgetLibraryId = BudgetLibraryId,
            Name = "Test Name"
        };

        public SelectableTreatmentBudgetEntity TestTreatmentBudget = new SelectableTreatmentBudgetEntity();

        private void SetupForGet()
        {
            _testHelper.UnitOfWork.Context.TreatmentLibrary.Add(TestTreatmentLibrary);
            _testHelper.UnitOfWork.Context.SelectableTreatment.Add(TestTreatment);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGet();
            //_testHelper.UnitOfDataPersistenceWork.Context.TreatmentCost.Add(TestTreatmentCost);
            var attribute = _testHelper.UnitOfWork.Context.Attribute.First();
            TestTeatmentConsequence.AttributeId = attribute.Id;
            //_testHelper.UnitOfDataPersistenceWork.Context.TreatmentConsequence.Add(TestTeatmentConsequence);
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.BudgetLibrary.Add(TestBudgetLibrary);
            _testHelper.UnitOfWork.Context.Budget.Add(TestBudget);
            TestTreatmentBudget.SelectableTreatmentId = TreatmentId;
            TestTreatmentBudget.BudgetId = BudgetId;
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.TreatmentLibraries();

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
                var result = await _controller.UpsertTreatmentLibrary(Guid.Empty, TestTreatmentLibrary.ToDto());

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
                var result = await _controller.DeleteTreatmentLibrary(Guid.Empty);

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
        public async void ShouldGetTreatmentData()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.TreatmentLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<TreatmentLibraryDTO>)Convert.ChangeType(okObjResult.Value, typeof(List<TreatmentLibraryDTO>));
                Assert.Single(dtos);

                Assert.Equal(TreatmentLibraryId, dtos[0].Id);
                Assert.Single(dtos[0].Treatments);

                Assert.Equal(TreatmentId, dtos[0].Treatments[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyTreatmentData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.TreatmentLibraries();
                var dtos = (List<TreatmentLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<TreatmentLibraryDTO>));

                var criterionLibraryDTO = _testHelper.TestCriterionLibrary.ToDto();

                var dto = dtos[0];
                dto.Description = "Updated Description";
                dto.Treatments[0].Name = "Updated Name";
                dto.Treatments[0].CriterionLibrary = criterionLibraryDTO;
                var costDTO = TestTreatmentCost.ToDto();
                costDTO.CriterionLibrary = criterionLibraryDTO;
                costDTO.Equation = TestCostEquation.ToDto();
                dto.Treatments[0].Costs.Add(costDTO);
                var consequenceDTO = TestTeatmentConsequence.ToDto();
                consequenceDTO.CriterionLibrary = criterionLibraryDTO;
                consequenceDTO.Equation = TestConsequenceEquation.ToDto();
                var attribute =
                    _testHelper.UnitOfWork.Context.Attribute.Single(_ =>
                        _.Id == TestTeatmentConsequence.AttributeId);
                consequenceDTO.Attribute = attribute.Name;
                dto.Treatments[0].Consequences.Add(consequenceDTO);
                dto.Treatments[0].BudgetIds.Add(BudgetId);

                // Act
                await _controller.UpsertTreatmentLibrary(_testHelper.TestSimulation.Id, dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto =
                        _testHelper.UnitOfWork.SelectableTreatmentRepo.TreatmentLibrariesWithTreatments()[0];
                    Assert.Equal(dto.Description, modifiedDto.Description);
                    Assert.Single(modifiedDto.AppliedScenarioIds);
                    Assert.Equal(_testHelper.TestSimulation.Id, modifiedDto.AppliedScenarioIds[0]);

                    Assert.Equal(dto.Treatments[0].Name, modifiedDto.Treatments[0].Name);
                    Assert.Equal(dto.Treatments[0].CriterionLibrary.Id,
                        modifiedDto.Treatments[0].CriterionLibrary.Id);
                    Assert.True(modifiedDto.Treatments[0].Costs.Any());

                    Assert.Equal(dto.Treatments[0].Costs[0].CriterionLibrary.Id,
                        modifiedDto.Treatments[0].Costs[0].CriterionLibrary.Id);
                    Assert.Equal(dto.Treatments[0].Costs[0].Equation.Id,
                        modifiedDto.Treatments[0].Costs[0].Equation.Id);

                    Assert.Equal(dto.Treatments[0].Costs[0].CriterionLibrary.Id,
                        modifiedDto.Treatments[0].Consequences[0].CriterionLibrary.Id);
                    Assert.Equal(dto.Treatments[0].Consequences[0].Equation.Id,
                        modifiedDto.Treatments[0].Consequences[0].Equation.Id);

                    Assert.Equal(dto.Treatments[0].BudgetIds[0],
                        modifiedDto.Treatments[0].BudgetIds[0]);
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteBudgetData()
        {
            try
            {
                // Arrange
                SetupForUpsertOrDelete();
                var getResult = await _controller.TreatmentLibraries();
                var dtos = (List<TreatmentLibraryDTO>)Convert.ChangeType((getResult as OkObjectResult).Value,
                    typeof(List<TreatmentLibraryDTO>));

                var criterionLibraryDTO = _testHelper.TestCriterionLibrary.ToDto();

                var treatmentLibraryDTO = dtos[0];
                treatmentLibraryDTO.Treatments[0].CriterionLibrary = criterionLibraryDTO;
                var costDTO = TestTreatmentCost.ToDto();
                costDTO.CriterionLibrary = criterionLibraryDTO;
                costDTO.Equation = TestCostEquation.ToDto();
                treatmentLibraryDTO.Treatments[0].Costs.Add(costDTO);
                var consequenceDTO = TestTeatmentConsequence.ToDto();
                consequenceDTO.CriterionLibrary = criterionLibraryDTO;
                consequenceDTO.Equation = TestConsequenceEquation.ToDto();
                var attribute =
                    _testHelper.UnitOfWork.Context.Attribute.Single(_ =>
                        _.Id == TestTeatmentConsequence.AttributeId);
                consequenceDTO.Attribute = attribute.Name;
                treatmentLibraryDTO.Treatments[0].Consequences.Add(consequenceDTO);
                treatmentLibraryDTO.Treatments[0].BudgetIds.Add(BudgetId);

                await _controller.UpsertTreatmentLibrary(_testHelper.TestSimulation.Id, treatmentLibraryDTO);

                // Act
                var result = await _controller.DeleteTreatmentLibrary(TreatmentLibraryId);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(!_testHelper.UnitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == TreatmentLibraryId));
                Assert.True(!_testHelper.UnitOfWork.Context.SelectableTreatment.Any(_ => _.Id == TreatmentId));
                Assert.True(!_testHelper.UnitOfWork.Context.TreatmentLibrarySimulation.Any(_ =>
                    _.TreatmentLibraryId == TreatmentLibraryId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibrarySelectableTreatment.Any(_ =>
                        _.SelectableTreatmentId == TreatmentId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryTreatmentCost.Any(_ =>
                        _.TreatmentCostId == CostId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryTreatmentConsequence.Any(_ =>
                        _.ConditionalTreatmentConsequenceId == ConsequenceId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.TreatmentCost.Any(_ => _.Id == CostId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.TreatmentConsequence.Any(_ => _.Id == ConsequenceId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.TreatmentCostEquation.Any(_ => _.TreatmentCostId == CostId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.TreatmentConsequenceEquation.Any(_ => _.ConditionalTreatmentConsequenceId == ConsequenceId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.Equation.Any(_ => _.Id == CostEquationId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.Equation.Any(_ => _.Id == ConsequenceEquationId));
                Assert.True(!_testHelper.UnitOfWork.Context.TreatmentBudget.Any());
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
