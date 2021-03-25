using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Library_API_Test_Classes
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
            _controller = new TreatmentController(_testHelper.UnitOfWork, _testHelper.MockEsecSecurity);
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

                var treatmentLibraryDTO = dtos[0];
                treatmentLibraryDTO.Description = "Updated Description";
                treatmentLibraryDTO.Treatments[0].Name = "Updated Name";
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

                // Act
                var result =
                    await _controller.UpsertTreatmentLibrary(_testHelper.TestSimulation.Id, treatmentLibraryDTO);

                // Assert
                Assert.IsType<OkResult>(result);

                var treatmentLibraryEntity = _testHelper.UnitOfWork.Context.TreatmentLibrary
                    .Include(_ => _.Treatments)
                    .ThenInclude(_ => _.TreatmentCosts)
                    .ThenInclude(_ => _.TreatmentCostEquationJoin)
                    .ThenInclude(_ => _.Equation)
                    .Include(_ => _.Treatments)
                    .ThenInclude(_ => _.TreatmentCosts)
                    .ThenInclude(_ => _.CriterionLibraryTreatmentCostJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.Treatments)
                    .ThenInclude(_ => _.TreatmentConsequences)
                    .ThenInclude(_ => _.Attribute)
                    .Include(_ => _.Treatments)
                    .ThenInclude(_ => _.TreatmentConsequences)
                    .ThenInclude(_ => _.ConditionalTreatmentConsequenceEquationJoin)
                    .ThenInclude(_ => _.Equation)
                    .Include(_ => _.Treatments)
                    .ThenInclude(_ => _.TreatmentConsequences)
                    .ThenInclude(_ => _.CriterionLibraryConditionalTreatmentConsequenceJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.Treatments)
                    .ThenInclude(_ => _.TreatmentBudgetJoins)
                    .ThenInclude(_ => _.Budget)
                    .Include(_ => _.Treatments)
                    .ThenInclude(_ => _.CriterionLibrarySelectableTreatmentJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.TreatmentLibrarySimulationJoins)
                    .Single(_ => _.Id == TreatmentLibraryId);

                Assert.Equal(treatmentLibraryDTO.Description, treatmentLibraryEntity.Description);
                Assert.Single(treatmentLibraryEntity.TreatmentLibrarySimulationJoins);
                var treatmentLibrarySimulationJoin = treatmentLibraryEntity.TreatmentLibrarySimulationJoins.ToList()[0];
                Assert.Equal(_testHelper.TestSimulation.Id, treatmentLibrarySimulationJoin.SimulationId);
                var treatmentEntity = treatmentLibraryEntity.Treatments.ToList()[0];
                Assert.Equal(treatmentLibraryDTO.Treatments[0].Name, treatmentEntity.Name);
                Assert.NotNull(treatmentEntity.CriterionLibrarySelectableTreatmentJoin);
                Assert.Equal(treatmentLibraryDTO.Treatments[0].CriterionLibrary.Id,
                    treatmentEntity.CriterionLibrarySelectableTreatmentJoin.CriterionLibrary.Id);
                Assert.True(treatmentEntity.TreatmentCosts.Any());

                var costEntity = treatmentEntity.TreatmentCosts.ToList()[0];
                Assert.NotNull(costEntity.CriterionLibraryTreatmentCostJoin);
                Assert.NotNull(costEntity.TreatmentCostEquationJoin);
                Assert.Equal(treatmentLibraryDTO.Treatments[0].Costs[0].CriterionLibrary.Id,
                    costEntity.CriterionLibraryTreatmentCostJoin.CriterionLibrary.Id);
                Assert.Equal(treatmentLibraryDTO.Treatments[0].Costs[0].Equation.Id,
                    costEntity.TreatmentCostEquationJoin.Equation.Id);

                var consequenceEntity = treatmentEntity.TreatmentConsequences.ToList()[0];
                Assert.NotNull(consequenceEntity.CriterionLibraryConditionalTreatmentConsequenceJoin);
                Assert.NotNull(consequenceEntity.ConditionalTreatmentConsequenceEquationJoin);
                Assert.Equal(treatmentLibraryDTO.Treatments[0].Costs[0].CriterionLibrary.Id,
                    consequenceEntity.CriterionLibraryConditionalTreatmentConsequenceJoin.CriterionLibrary.Id);
                Assert.Equal(treatmentLibraryDTO.Treatments[0].Consequences[0].Equation.Id,
                    consequenceEntity.ConditionalTreatmentConsequenceEquationJoin.Equation.Id);

                Assert.Single(treatmentEntity.TreatmentBudgetJoins);
                Assert.Equal(treatmentLibraryDTO.Treatments[0].BudgetIds[0],
                    treatmentEntity.TreatmentBudgetJoins.ToList()[0].BudgetId);
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
