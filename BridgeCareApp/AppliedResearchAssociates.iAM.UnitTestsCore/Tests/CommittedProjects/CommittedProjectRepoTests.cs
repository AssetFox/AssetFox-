using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.Analysis;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Humanizer;
using System.Xml.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.Data.Networking;
using Org.BouncyCastle.Asn1.Cms;
using AppliedResearchAssociates.iAM.TestHelpers;
using System.Runtime.InteropServices;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.CommittedProjects
{
    public class CommittedProjectRepoTests
    {
        private UnitOfDataPersistenceWork _testUOW;
        private Mock<IAMContext> _mockedContext;
        private Guid _badScenario = Guid.Parse("0c66674c-8fcb-462b-8765-69d6815e0958");

        public CommittedProjectRepoTests()
        {
            var mockedTestUOW = new Mock<IUnitOfWork>();
            _mockedContext = new Mock<IAMContext>();

            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Simulation, TestEntitiesForCommittedProjects.Simulations.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.MaintainableAsset, TestEntitiesForCommittedProjects.MaintainableAssetEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.CommittedProject, TestEntitiesForCommittedProjects.CommittedProjectEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, TestEntitiesForCommittedProjects.AttribureEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.InvestmentPlan, TestEntitiesForCommittedProjects.InvestmentPlanEntities().AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioBudget, TestEntitiesForCommittedProjects.ScenarioBudgetEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.SelectableTreatment, TestEntitiesForCommittedProjects.SelectableTreatmentEntities.AsQueryable());
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.ScenarioSelectableTreatment, TestEntitiesForCommittedProjects.FourYearScenarioNoTreatmentEntities().AsQueryable());

            _testUOW = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _mockedContext.Object);
        }

        [Fact]
        public void GetForSimulationWorksWithCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(TestHelper.UnitOfWork);
            //var simulationDomain = CreateSimulation(TestDataForCommittedProjects.SimulationId);

            //var simulationEntity = _testUOW.Context.Simulation.Single(s => s.Id == simulationDomain.Id);
            //simulationEntity.NoTreatmentBeforeCommittedProjects = true;
            //_testUOW.Context.Simulation.Update(simulationEntity);
            //_testUOW.Context.SaveChanges();

            Guid id = Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee");
            var name = "";
            Guid owner = new Guid();
            var dto = SimulationDtos.Dto(id, name, owner);
            var network = NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var resolveNetworkId = network.Id;
            network.KeyAttributeId = Guid.Parse("35934403-FEA2-4A67-9F1D-FA8D0132F5AE");
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee"));

            var bud = new TreatmentBudgetDTO
            {
                Id = new Guid(),
                Name = "Budget Test 1"
            };

            var libraryId = Guid.NewGuid();
            var treatmentId = Guid.NewGuid();
            var library = TreatmentLibraryDtos.Empty(libraryId);
            var treatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(treatmentId);
            var costId = Guid.NewGuid();
            var costLibraryId = Guid.NewGuid();
            var insertCostEquationId = Guid.NewGuid();
            var cost = TreatmentCostDtos.WithEquationAndCriterionLibrary(costId, insertCostEquationId, costLibraryId, "equation", "mergedCriteriaExpression");
            treatment.Costs.Add(cost);
            treatment.Budgets = new List<TreatmentBudgetDTO>();
            treatment.Budgets.Add(bud);
            treatment.BudgetIds = new List<Guid> { libraryId, treatmentId };
            var treatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulation.Id);
            //_testUOW.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(treatments, simulationDomain.Id);

            //TestHelper.UnitOfWork.BudgetRepo.AddScenarioBudgets
            var committedProject = new SectionCommittedProjectDTO
            {
                Id = Guid.Parse("091001e2-c1f0-4af6-90e7-e998bbea5d00"),
                Year = 2023,
                Treatment = "Simple",
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 3,
                Cost = 200000,
                SimulationId = simulation.Id,
                //ScenarioBudgetId = ScenarioBudgetDTOs().Single(_ => _.Name == "Interstate").Id,
                LocationKeys = new Dictionary<string, string>()
                {
                    { "ID", "46f5da89-5e65-4b8a-9b36-03d9af0302f7" },
                    { "BRKEY_", "2" },
                    { "BMSID", "9876543" }
                },
                Consequences = new List<CommittedProjectConsequenceDTO>()
                {
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_SEEDED",
                        ChangeValue = "9"
                    },
                    new CommittedProjectConsequenceDTO()
                    {
                        Id = Guid.NewGuid(),
                        Attribute = "DECK_DURATION_N",
                        ChangeValue = "1"
                    }
                }
            };
            
            //var keyAttributeId = Guid.NewGuid();
            //var assetKeyData = "key";
            //var maintainableAssets = new List<Data.Networking.MaintainableAsset>();
            //var assetId = Guid.NewGuid();
            //var locationIdentifier = RandomStrings.WithPrefix("Location");
            //var location = Locations.Section(locationIdentifier);
            //var maintainableAsset = new Data.Networking.MaintainableAsset(assetId, resolveNetworkId, location, "[Deck_Area]");
            //var attributeName = RandomStrings.WithPrefix("attribute");
            //var attribute = AttributeTestSetup.Text(keyAttributeId, attributeName);
            //maintainableAssets.Add(maintainableAsset);
            //var attributes = new List<Data.Attributes.Attribute> { attribute };
            //AggregatedResultTestSetup.SetTextAggregatedResultsInDb(TestHelper.UnitOfWork,
            //    maintainableAssets, attributes, assetKeyData);


            TestHelper.UnitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgets(TestDataForCommittedProjects.ScenarioBudgets, simulation.Id);
            //TestHelper.UnitOfWork.NetworkRepo.
            List<SectionCommittedProjectDTO> sectionCommittedProjects = new List<SectionCommittedProjectDTO>();
            sectionCommittedProjects.Add(committedProject);//   TestDataForCommittedProjects.SimpleSectionCommittedProjectDTO(Guid.Parse("091001e2-c1f0-4af6-90e7-e998bbea5d00"), simulation.Id, 2023));
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.SimulationId = Guid.Parse("dcdacfde-02da-4109-b8aa-add932756dee"));
            TestHelper.UnitOfWork.CommittedProjectRepo.UpsertCommittedProjects(newProjects);
            //repo.UpsertCommittedProjects(newProjects);
            // call upsertordeletetreatment to create a treatment
            // and attach to a simulation
            // Act
            var simulationD = CreateSimulation(simulation.Id, false);
            repo.GetSimulationCommittedProjects(simulationD);

            // Assert
            Assert.Equal(2, simulationD.CommittedProjects.Count);
            Assert.Equal(210000, simulationD.CommittedProjects.Sum(_ => _.Cost));
        }


        [Fact(Skip = "Test is not dependent on the No Treatment Before Committed Project Flag")]
        public void NoTreatmentBeforeCommittedProjects_GetSimulationCommittedProjects_Expected()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var inputSimulationEntity = TestEntitiesForCommittedProjects.Simulations.Single(_ => _.Name == "FourYearTest");
            var simulationDomain = CreateSimulation(inputSimulationEntity.Id);
            var simulationEntity = _testUOW.Context.Simulation.Single(s => s.Id == simulationDomain.Id);
            simulationEntity.NoTreatmentBeforeCommittedProjects = true;
            _testUOW.Context.Simulation.Update(simulationEntity);
            _testUOW.Context.SaveChanges();

            // Act
            repo.GetSimulationCommittedProjects(simulationDomain);

            // Assert
            var committedProjectNames = simulationDomain.CommittedProjects.Select(cp => cp.Name).ToList();
            Assert.Equal(4, simulationDomain.CommittedProjects.Count);
            Assert.Equal(10000, simulationDomain.CommittedProjects.Sum(_ => _.Cost));
            Assert.Equal(3, simulationDomain.CommittedProjects.Count(_ => _.Name != "Something"));
        }

        [Fact]
        public void GetForSimulationWorksWithoutCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var simulationDomain = CreateSimulation(TestDataForCommittedProjects.NoCommitSimulationId);

            // Act
            repo.GetSimulationCommittedProjects(simulationDomain);

            // Assert
            Assert.Equal(0, simulationDomain.CommittedProjects.Count);
        }

        [Fact]
        public void GetForSimulationHandlesBadScenarioId()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var simulationDomain = CreateSimulation(_badScenario, false);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.GetSimulationCommittedProjects(simulationDomain));
        }

        [Fact]
        public void GetForExportWorksWithCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetCommittedProjectsForExport(TestDataForCommittedProjects.SimulationId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(210000, result.Sum(_ => _.Cost));
            Assert.True(result.First() is SectionCommittedProjectDTO);
            Assert.Equal(2, result.First().Consequences.Count);
            Assert.Equal(2, result.First().LocationKeys.Count);
        }

        [Fact]
        public void GetForExportWorksWithoutCommittedProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetCommittedProjectsForExport(TestDataForCommittedProjects.NoCommitSimulationId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetForExportHandlesBadScenarioId()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.GetCommittedProjectsForExport(_badScenario));
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void UpsertWorksForValidCommittedProjectData()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.SimulationId = TestDataForCommittedProjects.NoCommitSimulationId);

            // Act
            repo.UpsertCommittedProjects(newProjects);
        }

        [Fact]
        public void UpsertHandlesBadSimulationId()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.SimulationId = _badScenario);

            // Act & Assert
            var exception = Assert.Throws<RowNotInTableException>(() => repo.UpsertCommittedProjects(newProjects));
            Assert.Contains("simulation ID", exception.Message);
        }

        [Fact]
        public void UpsertHandlesNonExistingBudgets()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.ScenarioBudgetId = Guid.Parse("0d91f67d-d5f4-4c1b-861c-3a5a24aab100"));

            // Act & Assert
            var exception = Assert.Throws<RowNotInTableException>(() => repo.UpsertCommittedProjects(newProjects));
            Assert.Contains("budget IDs", exception.Message);
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void UpsertWorksWithNullBudgets()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var newProjects = TestDataForCommittedProjects.ValidCommittedProjects;
            newProjects.ForEach(_ => _.ScenarioBudgetId = null);

            // Act
            repo.UpsertCommittedProjects(newProjects);
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void DeleteSimulationWorksWithValidSimulation()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            repo.DeleteSimulationCommittedProjects(TestDataForCommittedProjects.SimulationId);
        }

        [Fact]
        public void DeleteSimulationHandlesInvalidSimulation()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.DeleteSimulationCommittedProjects(_badScenario));
        }

        [Fact]
        public void DeleteSimulationHandlesSimulationWithNoCommitts()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            repo.DeleteSimulationCommittedProjects(TestDataForCommittedProjects.NoCommitSimulationId);

            // No assert required as long as it works
        }

        [Fact(Skip = "Unable to run with BulkExtensions")]
        public void DeleteSpecificWorksWithValidProject()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var projectsToDelete = TestDataForCommittedProjects.ValidCommittedProjects.Select(_ => _.Id).ToList();

            // Act
            repo.DeleteSpecificCommittedProjects(projectsToDelete);
        }

        [Fact]
        public void DeleteSpecificHandlesInvalidProject()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);
            var projectsToDelete = new List<Guid>() { Guid.Parse("ba5645ae-4f13-4a9f-94fd-2c03d26de500") };

            // Act & Assert
            // No assert here.  If a specific project does not exist but others do, we do not want to throw an error
        }

        [Fact]
        public void CanGetSectionCommittedProjectsForSimulationWithProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetSectionCommittedProjectDTOs(TestDataForCommittedProjects.SimulationId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(210000, result.Sum(_ => _.Cost));
            Assert.Equal(2, result.First().Consequences.Count);
            Assert.Equal(2, result.First().LocationKeys.Count);
        }

        [Fact]
        public void GetSectionCommittedProjectsHandlesSimulationWithoutProjects()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetSectionCommittedProjectDTOs(TestDataForCommittedProjects.NoCommitSimulationId);

            // Assert
            Assert.Equal(0, result.Count);
            Assert.IsType<List<SectionCommittedProjectDTO>>(result);
        }

        [Fact]
        public void GetSectionCommittedProjectsHandlesBadSimulations()
        { 
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.GetSectionCommittedProjectDTOs(_badScenario));
        }

        [Fact]
        public void GetSimulationIdReturnsValidValue()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act
            var result = repo.GetSimulationId(Guid.Parse("2e9e66df-4436-49b1-ae68-9f5c10656b1b"));

            // Assert
            Assert.Equal(TestDataForCommittedProjects.SimulationId, result);
        }

        [Fact]
        public void GetSimulationIdHandlesBadProject()
        {
            // Arrange
            var repo = new CommittedProjectRepository(_testUOW);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.GetSimulationId(Guid.Parse("aa84643a-24c0-4722-820c-6a1fed01ccac")));
        }

        #region Helpers
        private Simulation CreateSimulation(Guid simulationId, bool populateInvestments = true)
        {
            var exp = _testUOW.AttributeRepo.GetExplorer();
            var testNetwork = exp.AddNetwork();
            testNetwork.Id = TestDataForCommittedProjects.NetworkId;
            SectionMapper mapper = new(testNetwork);
            foreach (var asset in TestEntitiesForCommittedProjects.MaintainableAssetEntities)
            {
                mapper.CreateMaintainableAsset(asset);
            }
            var simulation = testNetwork.AddSimulation();
            simulation.Id = simulationId;
            // This has to be ignored to create a bad scenario object
            if (populateInvestments) _testUOW.InvestmentPlanRepo.GetSimulationInvestmentPlan(simulation);
            return simulation;
        }

        #endregion
    }
}
