using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.TestHelpers;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.TestHelpers.Assertions;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment
{
    public class SelectableTreatmentRepositoryTests
    {
        private TreatmentLibraryEntity _testTreatmentLibrary;
        private SelectableTreatmentEntity _testTreatment;
        private TreatmentCostEntity _testTreatmentCost;
        private ConditionalTreatmentConsequenceEntity _testTreatmentConsequence;
        private ScenarioSelectableTreatmentEntity _testScenarioTreatment;
        private ScenarioTreatmentCostEntity _testScenarioTreatmentCost;
        private ScenarioConditionalTreatmentConsequenceEntity _testScenarioTreatmentConsequence;


        private void SetupAttributesAndNetwork()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
        }


        private void CreateLibraryTestData()
        {
            _testTreatmentLibrary = new TreatmentLibraryEntity { Id = Guid.NewGuid(), Name = "Test Name" };
            TestHelper.UnitOfWork.Context.TreatmentLibrary.Add(_testTreatmentLibrary);

            _testTreatment = new SelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                TreatmentLibraryId = _testTreatmentLibrary.Id,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatment);

            _testTreatmentCost = new TreatmentCostEntity { Id = Guid.NewGuid(), TreatmentId = _testTreatment.Id };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatmentCost);

            _testTreatmentConsequence = new ConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                SelectableTreatmentId = _testTreatment.Id,
                ChangeValue = "1",
                AttributeId = TestHelper.UnitOfWork.Context.Attribute.First().Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testTreatmentConsequence);

            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private ScenarioBudgetEntity CreateScenarioTestData(Guid simulationId)
        {
            var budget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name"
            };
            TestHelper.UnitOfWork.Context.AddEntity(budget);


            _testScenarioTreatment = new ScenarioSelectableTreatmentEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                Name = "Test Name",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatment);
            TestHelper.UnitOfWork.Context.AddEntity(new ScenarioSelectableTreatmentScenarioBudgetEntity
            {
                ScenarioBudgetId = budget.Id,
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            });

            _testScenarioTreatmentCost = new ScenarioTreatmentCostEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentCost);


            _testScenarioTreatmentConsequence = new ScenarioConditionalTreatmentConsequenceEntity
            {
                Id = Guid.NewGuid(),
                ScenarioSelectableTreatmentId = _testScenarioTreatment.Id,
                ChangeValue = "1",
                AttributeId = TestHelper.UnitOfWork.Context.Attribute.First().Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioTreatmentConsequence);


            TestHelper.UnitOfWork.Context.SaveChanges();
            return budget;
        }

        [Fact]
        public void TreatmentInDbWithCostEquation_DeleteLibrary_EquationIsDeleted()
        {
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var library = TreatmentLibraryDtos.Empty(libraryId);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(treatmentId);
            var costId = Guid.NewGuid();
            var costLibraryId = Guid.NewGuid();
            var insertCostEquationId = Guid.NewGuid();
            var cost = TreatmentCostDtos.WithEquationAndCriterionLibrary(costId, insertCostEquationId, costLibraryId, "equation", "mergedCriteriaExpression");
            treatment.Costs.Add(cost);
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(library);
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(treatments, libraryId);
            var costInDb = TestHelper.UnitOfWork.Context.TreatmentCost
                .Include(tc => tc.TreatmentCostEquationJoin)
                .SingleOrDefault(x => x.Id == costId);
            Assert.NotNull(costInDb);
            var equationIdInDb = costInDb.TreatmentCostEquationJoin.EquationId;
            var equationInDb = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationIdInDb);
            Assert.NotNull(equationInDb);

            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(libraryId);

            var equationInDbAfter = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationIdInDb);
            Assert.Null(equationInDbAfter);
            var costInDbAfter = TestHelper.UnitOfWork.Context.TreatmentCost
                .SingleOrDefault(x => x.Id == costId);
            Assert.Null(costInDbAfter);
        }

        [Fact]
        public void TreatmentInDbWithConsequenceEquation_DeleteLibrary_EquationIsDeleted()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var attributeName = TestAttributeNames.CulvDurationN;
            var library = TreatmentLibraryDtos.Empty(libraryId);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(treatmentId);
            var consequenceId = Guid.NewGuid();
            var consequenceLibraryId = Guid.NewGuid();
            var insertConsequenceEquationId = Guid.NewGuid();
            var consequence = TreatmentConsequenceDtos.WithEquationAndCriterionLibrary(consequenceId, attributeName, insertConsequenceEquationId, consequenceLibraryId);
            treatment.Consequences.Add(consequence);
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(library);
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(treatments, libraryId);
            var consequenceInDb = TestHelper.UnitOfWork.Context.TreatmentConsequence
                .Include(tc => tc.ConditionalTreatmentConsequenceEquationJoin)
                .SingleOrDefault(x => x.Id == consequenceId);
            Assert.NotNull(consequenceInDb);
            var equationIdInDb = consequenceInDb.ConditionalTreatmentConsequenceEquationJoin.EquationId;
            var equationInDb = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationIdInDb);
            Assert.NotNull(equationInDb);

            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(libraryId);

            var equationInDbAfter = TestHelper.UnitOfWork.Context.Equation.SingleOrDefault(e => e.Id == equationIdInDb);
            Assert.Null(equationInDbAfter);
            var costInDbAfter = TestHelper.UnitOfWork.Context.TreatmentCost
                .SingleOrDefault(x => x.Id == consequenceId);
            Assert.Null(costInDbAfter);
        }

        [Fact]
        public void GetSimpleTreatmentsByLibraryId_EntitiesInDatabase_Gets()
        {
            // Arrange
            SetupAttributesAndNetwork();
            CreateLibraryTestData();

            // Act
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSimpleTreatmentsByLibraryId(_testTreatmentLibrary.Id);
            Assert.Single(dtos);

            Assert.Equal(_testTreatment.Id, dtos[0].Id);
            Assert.Equal(_testTreatment.Name, dtos[0].Name);
        }


        [Fact]
        public void GetSimpleTreatmentsBySimulationId_EntitiesInDb_Gets()
        {
            // Arrange
            SetupAttributesAndNetwork();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var budget = CreateScenarioTestData(simulation.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSimpleTreatmentsBySimulationId(simulation.Id);

            Assert.Single(dtos);

            Assert.Equal(_testScenarioTreatment.Id, dtos[0].Id);
        }

        [Fact]
        public void GetAllTreatmentLibrariesNoChildren_AtLeastOneLibraryInDb_Gets()
        {
            SetupAttributesAndNetwork();
            // Act
            var result = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibrariesNoChildren();

            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetScenarioSelectableTreatments_SimulationInDb_DoesNotThrow()
        {
            SetupAttributesAndNetwork();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            // Act
            var result = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation.Id);
        }

        [Fact]
        public void GetTreatmentLibraryWithSingleTreatmentByTreatmentId_TreatmentInDb_Expected()
        {
            // Arrange
            SetupAttributesAndNetwork();
            CreateLibraryTestData();

            // Act
            var libraryDTO = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetTreatmentLibraryWithSingleTreatmentByTreatmentId(_testTreatment.Id);

            // Assert
            Assert.Equal(_testTreatmentLibrary.Id, libraryDTO.Id);
            var dto = libraryDTO.Treatments[0];

            Assert.Equal(_testTreatment.Id, dto.Id);
            Assert.Single(dto.Consequences);
            Assert.Single(dto.Costs);

            Assert.Equal(_testTreatmentConsequence.Id, dto.Consequences[0].Id);
            Assert.Equal(_testTreatmentCost.Id, dto.Costs[0].Id);
        }

        [Fact]
        public void GetScenarioSelectableTreatmentById_TreatmentInDb_Expected()
        {
            SetupAttributesAndNetwork();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var budget = CreateScenarioTestData(simulation.Id);

            // Act
            var treatmentDtoWithSimulationId = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatmentById(_testScenarioTreatment.Id);

            // Assert
            Assert.Equal(simulation.Id, treatmentDtoWithSimulationId.SimulationId);
            var treatmentDto = treatmentDtoWithSimulationId.Treatment;
            Assert.Equal(_testScenarioTreatment.Id, treatmentDto.Id);
            Assert.Single(treatmentDto.Consequences);
            Assert.Single(treatmentDto.Costs);
            Assert.Single(treatmentDto.BudgetIds);

            Assert.Equal(_testScenarioTreatmentConsequence.Id, treatmentDto.Consequences[0].Id);
            Assert.Equal(_testScenarioTreatmentCost.Id, treatmentDto.Costs[0].Id);
            Assert.Contains(budget.Id, treatmentDto.BudgetIds);
        }

        [Fact]
        public void UpsertOrDeleteLibrary_ThenTreatments_DoesNotThrow()
        {
            // Arrange
            SetupAttributesAndNetwork();
            var dto = new TreatmentLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                Treatments = new List<TreatmentDTO>()
            };

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertTreatmentLibrary(dto);
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dto.Treatments, dto.Id);
        }



        [Fact]
        public void UpsertOrDeleteScenarioTreatments_DoesNotThrow()
        {
            // Arrange
            SetupAttributesAndNetwork();
            var dtos = new List<TreatmentDTO>();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulation.Id);
        }

        [Fact]
        public void DeleteTreatmentLibrary_NoSuchLibraryInDb_DoesNotThrow()
        {
            // we pass if this does not throw.
            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(Guid.NewGuid());
        }


        [Fact]
        public void GetAllTreatmentLibrariesNoChildren_EntitiesInDatabase_Deletes()
        {
            //Arrange
            SetupAttributesAndNetwork();
            CreateLibraryTestData();

            // Act
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibrariesNoChildren();

            Assert.Contains(dtos, t => t.Id == _testTreatmentLibrary.Id);
        }

        [Fact]
        public void GetScenarioSelectableTreatments_EntitiesInDatabase_Gets()
        {
            // Arrange
            SetupAttributesAndNetwork();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var budget = CreateScenarioTestData(simulation.Id);

            // Act
            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation.Id);

            Assert.Single(dtos);

            Assert.Equal(_testScenarioTreatment.Id, dtos[0].Id);
            Assert.Single(dtos[0].Consequences);
            Assert.Single(dtos[0].Costs);
            Assert.Single(dtos[0].BudgetIds);

            Assert.Equal(_testScenarioTreatmentConsequence.Id, dtos[0].Consequences[0].Id);
            Assert.Equal(_testScenarioTreatmentCost.Id, dtos[0].Costs[0].Id);
            Assert.Contains(budget.Id, dtos[0].BudgetIds);
        }

        [Fact]
        public void UpsertOrDeleteTreatmentLibraryTreatmentsAndPossiblyUsers_LibraryAndTreatmentsInDb_Updates()
        {
            // WJWJWJ could be a good test to modify for the new repo method?
            SetupAttributesAndNetwork();
            CreateLibraryTestData();

            var dto = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibrariesNoChildren();
            var dtoLibrary = dto.Where(t => t.Name == "Test Name").FirstOrDefault();
            var treatments = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(dtoLibrary.Id);
            dtoLibrary.Description = "Updated Description";
            treatments[0].Name = "Updated Name";
            treatments[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            treatments[0].Costs[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            treatments[0].Costs[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            treatments[0].Consequences[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            treatments[0].Consequences[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatmentLibraryTreatmentsAndPossiblyUsers(dtoLibrary, false, Guid.Empty);
            // Assert
            var modifiedDto =
                TestHelper.UnitOfWork.SelectableTreatmentRepo.GetAllTreatmentLibraries().Single(lib => lib.Id == dtoLibrary.Id);
            Assert.Equal(dtoLibrary.Description, modifiedDto.Description);
        }

        [Fact]
        public void UpsertOrDeleteScenarioSelectableTreatment_ValidInput_Succeeds()
        {
            // Arrange
            SetupAttributesAndNetwork();
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);
            var scenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = "",
                SimulationId = simulation.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(scenarioBudget);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var dtos = TestHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);

            dtos[0].Description = "Updated Description";
            dtos[0].Name = "Updated Name";
            dtos[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dtos[0].Costs[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dtos[0].Costs[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dtos[0].Consequences[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dtos[0].Consequences[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dtos[0].BudgetIds.Add(scenarioBudget.Id);

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulation.Id);

            // Assert
            var modifiedDto = TestHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);
            Assert.Equal(dtos[0].Description, modifiedDto[0].Description);
            Assert.Equal(dtos[0].Name, modifiedDto[0].Name);
            Assert.Equal(dtos[0].BudgetIds.Count, modifiedDto[0].BudgetIds.Count);
            Assert.Contains(scenarioBudget.Id, modifiedDto[0].BudgetIds);
        }

        [Fact]
        public void UpsertOrDeleteSelectableTreatments_TwoConsequencesCollide_DbUnchanged()
        {
            // Arrange
            SetupAttributesAndNetwork();
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            CreateScenarioTestData(simulation.Id);

            var scenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = "",
                SimulationId = simulation.Id
            };
            TestHelper.UnitOfWork.Context.AddEntity(scenarioBudget);
            TestHelper.UnitOfWork.Context.SaveChanges();

            var dto = TestHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);

            dto[0].Description = "Updated Description";
            dto[0].Name = "Updated Name";
            dto[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dto[0].Costs[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dto[0].Costs[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dto[0].Consequences[0].CriterionLibrary = new CriterionLibraryDTO
            {
                Id = Guid.NewGuid(),
                Name = "",
                MergedCriteriaExpression = "",
                IsSingleUse = true
            };
            dto[0].Consequences[0].Equation = new EquationDTO { Id = Guid.NewGuid(), Expression = "" };
            dto[0].BudgetIds.Add(scenarioBudget.Id);
            var consequenceToCollideWith = dto[0].Consequences[0];
            var collidingConsequence = new TreatmentConsequenceDTO
            {
                Id = consequenceToCollideWith.Id,
                Equation = new EquationDTO
                {
                    Id = Guid.NewGuid(),
                    Expression = consequenceToCollideWith.Equation.Expression,
                }
            };
            dto[0].Consequences.Add(collidingConsequence);
            var treatmentsBefore = TestHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);

            // Act
            var exception = Assert.ThrowsAny<Exception>(() => TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dto, simulation.Id));

            // Assert
            var treatmentsAfter = TestHelper.UnitOfWork.SelectableTreatmentRepo
                .GetScenarioSelectableTreatments(simulation.Id);
            ObjectAssertions.Equivalent(treatmentsBefore, treatmentsAfter);
        }

        [Fact]
        public void DeleteTreatmentLibrary_EntitiesInDb_Deletes()
        {
            // Arrange
            SetupAttributesAndNetwork();
            CreateLibraryTestData();

            // Act
            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteTreatmentLibrary(_testTreatmentLibrary.Id);

            Assert.False(
                TestHelper.UnitOfWork.Context.TreatmentLibrary.Any(_ => _.Id == _testTreatmentLibrary.Id));
            Assert.False(TestHelper.UnitOfWork.Context.SelectableTreatment.Any(_ => _.Id == _testTreatment.Id));
            Assert.False(TestHelper.UnitOfWork.Context.TreatmentCost.Any(_ => _.Id == _testTreatmentCost.Id));
            Assert.False(
                TestHelper.UnitOfWork.Context.TreatmentConsequence.Any(_ =>
                    _.Id == _testTreatmentConsequence.Id));
        }

        [Fact]
        public void GetSelectableTreatmentByLibraryIdAndName_Does()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var treatmentName = RandomStrings.WithPrefix("treatment");
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, treatmentName);

            var result = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatmentByLibraryIdAndName(treatmentLibraryId, treatmentName);

            treatment.BudgetIds = new List<Guid>();
            ObjectAssertions.EquivalentExcluding(treatment, result, x => x.CriterionLibrary, _ => _.SupersedeRules);

        }

        [Fact]
        public void GetSelectableTreatmentByLibraryIdAndName_TreatmentInDbWithDifferentName_DoesNotFind()
        {
            var networkId = Guid.NewGuid();
            var treatmentLibraryId = Guid.NewGuid();
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatmentId = Guid.NewGuid();
            var treatmentName = RandomStrings.WithPrefix("treatment");
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(
                TestHelper.UnitOfWork, treatmentLibraryId, treatmentId, treatmentName);

            var result = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatmentByLibraryIdAndName(treatmentLibraryId, "wrongName");

            Assert.Null(result);
        }

        [Fact]
        public void GetLibraryModifiedDate_Does()
        {
            var libraryId = Guid.NewGuid();
            var before = DateTime.Now;

            var library = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryId);

            var after = DateTime.Now;
            var modifiedDate = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetLibraryModifiedDate(libraryId);
            DateTimeAssertions.Between(before, after, modifiedDate, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void AddLibraryTreatments_Does()
        {
            var treatmentLibraryId = Guid.NewGuid();
            var library = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists();
            var treatments = new List<TreatmentDTO> { treatment };

            TestHelper.UnitOfWork.SelectableTreatmentRepo.AddLibraryTreatments(
                treatments, treatmentLibraryId);

            var treatmentsAfter = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(treatmentLibraryId);
            var treatmentAfter = treatmentsAfter.Single();
            ObjectAssertions.EquivalentExcluding(treatment, treatmentAfter, t => t.CriterionLibrary);
        }

        [Fact]
        public void AddScenarioSelectableTreatment_Does()
        {
            SetupAttributesAndNetwork();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, NetworkTestSetup.NetworkId);

            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists();
            var treatments = new List<TreatmentDTO> { treatment };

            TestHelper.UnitOfWork.SelectableTreatmentRepo.AddScenarioSelectableTreatment(treatments, simulation.Id);

            var treatmentsAfter = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulation.Id);
            var treatmentAfter = treatmentsAfter.Single();
            ObjectAssertions.EquivalentExcluding(treatment, treatmentAfter, t => t.CriterionLibrary, t => t.Budgets);
        }

        [Fact]
        public void GetSingleTreatmentLibrary_LibraryInDb_Gets()
        {
            var treatmentLibraryId = Guid.NewGuid();
            var library = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists();
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.AddLibraryTreatments(
                treatments, treatmentLibraryId);

            var libraryAfter = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(treatmentLibraryId);
            ObjectAssertions.EquivalentExcluding(library, libraryAfter, l => l.Owner, l => l.Treatments);
            var treatmentAfter = libraryAfter.Treatments.Single();
            ObjectAssertions.EquivalentExcluding(treatment, treatmentAfter, t => t.Budgets, t => t.CriterionLibrary);
        }

        [Fact]
        public void ReplaceTreatmentLibrary_LibraryInDbWithTreatment_DeletesTreatment()
        {
            var treatmentLibraryId = Guid.NewGuid();
            var library = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists();
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.AddLibraryTreatments(
                treatments, treatmentLibraryId);
            var replaceTreatmentList = new List<TreatmentDTO> { };

            TestHelper.UnitOfWork.SelectableTreatmentRepo.ReplaceTreatmentLibrary(treatmentLibraryId, replaceTreatmentList);

            var libraryAfter = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(treatmentLibraryId);
            Assert.Empty(libraryAfter.Treatments);
        }

        [Fact]
        public void DeleteTreatment_TreatmentInDb_Deletes()
        {
            var treatmentLibraryId = Guid.NewGuid();
            var library = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentLibraryId);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists();
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.AddLibraryTreatments(
                treatments, treatmentLibraryId);
            var libraryBefore = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(treatmentLibraryId);

            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteTreatment(treatment, treatmentLibraryId);

            var libraryAfter = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSingleTreatmentLibary(treatmentLibraryId);
            Assert.Single(libraryBefore.Treatments);
            Assert.Empty(libraryAfter.Treatments);
        }

        [Fact]
        public void DeleteScenarioSelectableTreatment_SimulationInDbWithTreatment_Deletes()
        {
            SetupAttributesAndNetwork();
            var simulationId = Guid.NewGuid();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, networkId: NetworkTestSetup.NetworkId);
            var treatmentDto = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists();
            var treatmentDtos = new List<TreatmentDTO> { treatmentDto };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.AddScenarioSelectableTreatment(treatmentDtos, simulationId);
            var simulationTreatmentsBefore = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);

            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteScenarioSelectableTreatment(treatmentDto, simulationId);

            var simulationTreatmentsAfter = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            Assert.Single(simulationTreatmentsBefore);
            Assert.Empty(simulationTreatmentsAfter);
        }
    }
}
