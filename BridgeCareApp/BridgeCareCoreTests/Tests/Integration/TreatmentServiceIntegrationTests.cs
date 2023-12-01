using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TreatmentCost;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using BridgeCareCore.Services.Treatment;
using BridgeCareCoreTests.Helpers;
using OfficeOpenXml;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class TreatmentServiceIntegrationTests
    {
        private TreatmentService CreateTreatmentService(UnitOfDataPersistenceWork unitOfWork)
        {
            var expressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid().Object;
            var treatmentLoader = new ExcelTreatmentLoader(expressionValidationService);
            var service = new TreatmentService(unitOfWork, treatmentLoader);
            return service;
        }

        [Fact]
        public void DownloadLibraryTreatmentSpreadsheet_ThenUpload_SameTreatments()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            var libraryId = Guid.NewGuid();
            var library = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryId);
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibraryInDb(TestHelper.UnitOfWork, libraryId, treatmentId);
            var cost = LibraryTreatmentCostTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, treatmentId, libraryId);
            var consequence = LibraryTreatmentConsequenceTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryId, treatmentId,
                attribute: "AGE");
            var treatments1 = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(libraryId);
            var service = CreateTreatmentService(TestHelper.UnitOfWork);
            var fileInfo = service.ExportLibraryTreatmentsExcelFile(libraryId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            File.WriteAllBytes("zzzzz.xlsx", bytes);
            var excelPackage = new ExcelPackage(stream);
            var userCriteria = new UserCriteriaDTO();
            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteTreatment(treatment, libraryId);
            var treatments2 = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(libraryId);
            Assert.Empty(treatments2);
            service.ImportLibraryTreatmentsFile(libraryId, excelPackage);
            var treatments3 = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetSelectableTreatments(libraryId);
            var treatment1 = treatments1.Single();
            var treatment3 = treatments3.Single();
            ObjectAssertions.EquivalentExcluding(treatment1, treatment3,
                t => t.Id,
                t => t.Costs[0].Id,
                t => t.Costs[0].Equation.Id,
                t => t.Costs[0].CriterionLibrary.Id,
                t => t.Consequences[0].Id,
                t => t.Consequences[0].Equation.Id,
                t => t.Consequences[0].CriterionLibrary.Id);
        }

        [Fact]
        public void DownloadScenarioTreatmentSpreadsheet_ThenUpload_SameTreatments()
        {
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName);
            var budget = BudgetDtos.New();
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);
            var treatmentId = Guid.NewGuid();
            var treatmentBudget = TreatmentBudgetDtos.Dto(budget.Name);
            var treatmentBudgets = new List<TreatmentBudgetDTO> { treatmentBudget };
            var budgetIds = new List<Guid> { budget.Id };
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfSimulationInDb(TestHelper.UnitOfWork, simulationId, treatmentId, criterionExpression: "treatment criterion", budgets: treatmentBudgets, budgetIds: budgetIds);
            var initialTreatments = new List<TreatmentDTO> { treatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(initialTreatments, simulationId);
            var cost = ScenarioTreatmentCostTestSetup.CostForTreatmentInDb(TestHelper.UnitOfWork, treatmentId, simulationId);

            var consequence = ScenarioTreatmentConsequenceTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, simulationId, treatmentId,
                attribute: "AGE", equation: "[AGE]", criterion: "[AGE] > 10");
            var performanceFactor = TreatmentPerformanceFactorDtos.Dto("AGE");
            var performanceFactors = new List<TreatmentPerformanceFactorDTO> { performanceFactor };
            var performanceFactorDictionary = new Dictionary<Guid, List<TreatmentPerformanceFactorDTO>>
                { { treatment.Id, performanceFactors } };
            TestHelper.UnitOfWork.TreatmentPerformanceFactorRepo.UpsertScenarioTreatmentPerformanceFactors(
                performanceFactorDictionary, simulationId);
            var treatments1 = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            var service = CreateTreatmentService(TestHelper.UnitOfWork);
            var fileInfo = service.ExportScenarioTreatmentsExcelFile(simulationId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            File.WriteAllBytes("zzzzz.xlsx", bytes);
            var excelPackage = new ExcelPackage(stream);
            var userCriteria = new UserCriteriaDTO();
            TestHelper.UnitOfWork.SelectableTreatmentRepo.DeleteScenarioSelectableTreatment(treatment, simulationId);
            var treatments2 = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            Assert.Empty(treatments2);
            service.ImportScenarioTreatmentsFile(simulationId, excelPackage);
            var treatments3 = TestHelper.UnitOfWork.SelectableTreatmentRepo.GetScenarioSelectableTreatments(simulationId);
            var treatment1 = treatments1.Single();
            var treatment3 = treatments3.Single();
            ObjectAssertions.EquivalentExcluding(treatment1, treatment3,
                t => t.Id,
                t => t.CriterionLibrary.Id,
                t => t.Costs[0].Id,
                t => t.Costs[0].Equation.Id,
                t => t.Costs[0].CriterionLibrary.Id,
                t => t.Consequences[0].Id,
                t => t.Consequences[0].Equation.Id,
                t => t.Consequences[0].CriterionLibrary.Id,
                t => t.PerformanceFactors[0].Id);
        }

        [Fact]
        public void DownloadScenarioTreatmentSupersedeRulesSpreadsheet_ThenUpload_Same()
        {
            // Create simulation
            AttributeTestSetup.CreateAttributes(TestHelper.UnitOfWork);
            NetworkTestSetup.CreateNetwork(TestHelper.UnitOfWork);
            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, simulationId, simulationName);

            // Add treatments
            var budget = BudgetDtos.New();
            var budgets = new List<BudgetDTO> { budget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, budgets, simulationId);

            var treatmentBudget = TreatmentBudgetDtos.Dto(budget.Name);
            var treatmentBudgets = new List<TreatmentBudgetDTO> { treatmentBudget };
            var treatmentBudgetIds = new List<Guid> { treatmentBudget.Id };
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfSimulation(treatmentId, name: "Bridge Replacement", criterionExpression: "treatment criterion", budgets: treatmentBudgets, budgetIds: treatmentBudgetIds);

            var preventTreatmentBudget = TreatmentBudgetDtos.Dto(budget.Name);
            var preventTreatmentBudgets = new List<TreatmentBudgetDTO> { preventTreatmentBudget };
            var preventTreatmentBudgetIds = new List<Guid> { preventTreatmentBudget.Id };
            var preventTreatmentId = Guid.NewGuid();
            var preventTreatment = TreatmentTestSetup.ModelForSingleTreatmentOfSimulation(preventTreatmentId, name: "Deck Replacement", criterionExpression: "prevent treatment criterion", budgets: preventTreatmentBudgets, budgetIds: preventTreatmentBudgetIds);

            var dtos = new List<TreatmentDTO> { treatment, preventTreatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);

            // Add supersede rule
            var supersedeRule = TreatmentDtos.SupersedeRule(preventTreatment, "Age > 20");
            var scenarioTreatmentSupersedeRulesPerTreatmentId = TreatmentTestSetup.ModelTreatmentSupersedeRules(treatmentId, new List<TreatmentSupersedeRuleDTO> { supersedeRule });
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteScenarioTreatmentSupersedeRules(scenarioTreatmentSupersedeRulesPerTreatmentId, simulationId);
                        
            // Get treatment supersede rules
            var supersedeRules = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetScenarioTreatmentSupersedeRules(treatmentId, simulationId);

            var service = CreateTreatmentService(TestHelper.UnitOfWork);
            // Export
            var fileInfo = service.ExportScenarioTreatmentSupersedeRuleExcelFile(simulationId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            File.WriteAllBytes("ScenarioTreatmentSupersedeRules.xlsx", bytes);
            var excelPackage = new ExcelPackage(stream);

            // This was attempting to delete via supersede object.            
            var supersedeDictionary = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> { { treatmentId, new List<TreatmentSupersedeRuleDTO>() } };
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteScenarioTreatmentSupersedeRules(supersedeDictionary, simulationId);

            // Get treatment supersede rules            
            var treatmentSupersedeRulesEmpty = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetScenarioTreatmentSupersedeRules(treatmentId, simulationId);
            Assert.Empty(treatmentSupersedeRulesEmpty);

            // Import and Get
            service.ImportScenarioTreatmentSupersedeRulesFile(simulationId, excelPackage);
            var supersedeRulesSame = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetScenarioTreatmentSupersedeRules(treatmentId, simulationId);

            var treatmentSupersedeRule = supersedeRules.Single();
            var treatmentSupersedeRuleSame = supersedeRulesSame.Single();
            ObjectAssertions.EquivalentExcluding(treatmentSupersedeRule, treatmentSupersedeRuleSame,
                _ => _.Id, _ => _.CriterionLibrary.Id, _ => _.CriterionLibrary.Name);
        }

        [Fact]
        public void DownloadLibraryTreatmentSupersedeRuleSpreadsheet_ThenUpload_Same()
        {
            // Create treatment library
            var libraryId = Guid.NewGuid();
            var libraryName = RandomStrings.WithPrefix("Library");            
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryId);

            // Add treatments            
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibrary(treatmentId, name: "Bridge Replacement", criterionExpression: "treatment criterion");

            var preventTreatmentId = Guid.NewGuid();
            var preventTreatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibrary(preventTreatmentId, name: "Deck Replacement", criterionExpression: "prevent treatment criterion");

            var dtos = new List<TreatmentDTO> { treatment, preventTreatment };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dtos, libraryId);

            // Add supersede rule
            var supersedeRule = TreatmentDtos.SupersedeRule(preventTreatment, "Age > 20");
            var libraryTeatmentSupersedeRulesPerTreatmentId = TreatmentTestSetup.ModelTreatmentSupersedeRules(treatmentId, new List<TreatmentSupersedeRuleDTO> { supersedeRule });
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteTreatmentSupersedeRules(libraryTeatmentSupersedeRulesPerTreatmentId, libraryId);

            // Get treatment supersede rules
            var supersedeRules = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRules(treatmentId, libraryId);

            var service = CreateTreatmentService(TestHelper.UnitOfWork);
            // Export
            var fileInfo = service.ExportLibraryTreatmentSupersedeRuleExcelFile(libraryId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            File.WriteAllBytes("LibraryTreatmentSupersedeRules.xlsx", bytes);
            var excelPackage = new ExcelPackage(stream);

            // This was attempting to delete via supersede object.            
            var supersedeDictionary = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> { { treatmentId, new List<TreatmentSupersedeRuleDTO>() } };
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteTreatmentSupersedeRules(supersedeDictionary, libraryId);

            // Get treatment supersede rules            
            var treatmentSupersedeRulesEmpty = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRules(treatmentId, libraryId);
            // Assert
            Assert.Empty(treatmentSupersedeRulesEmpty);

            // Import and Get
            service.ImportLibraryTreatmentSupersedeRulesFile(libraryId, excelPackage);
            var supersedeRulesSame = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRules(treatmentId, libraryId);

            // Assert
            var treatmentSupersedeRule = supersedeRules.Single();
            var treatmentSupersedeRuleSame = supersedeRulesSame.Single();
            ObjectAssertions.EquivalentExcluding(treatmentSupersedeRule, treatmentSupersedeRuleSame,
                _ => _.Id, _ => _.CriterionLibrary.Id, _ => _.CriterionLibrary.Name);
        }

        [Fact]
        public void DownloadLibraryTreatmentSupersedeRulesSpreadsheet_ThenUpload_Same()
        {
            // Create treatment library
            var libraryId = Guid.NewGuid();
            var libraryName = RandomStrings.WithPrefix("Library");
            var treatmentLibrary = TreatmentLibraryTestSetup.ModelForEntityInDb(TestHelper.UnitOfWork, libraryId);

            // Add treatments            
            var treatmentId = Guid.NewGuid();
            var treatment = TreatmentTestSetup.ModelForSingleTreatmentOfLibrary(treatmentId, name: "Bridge Replacement", criterionExpression: "treatment criterion");

            var preventTreatment1Id = Guid.NewGuid();
            var preventTreatment1 = TreatmentTestSetup.ModelForSingleTreatmentOfLibrary(preventTreatment1Id, name: "Deck Replacement", criterionExpression: "prevent treatment criterion");

            var preventTreatment2Id = Guid.NewGuid();
            var preventTreatment2 = TreatmentTestSetup.ModelForSingleTreatmentOfLibrary(preventTreatment2Id, name: "Test DeckReplacement2", criterionExpression: "prevent treatment2 criterion");

            var dtos = new List<TreatmentDTO> { treatment, preventTreatment1, preventTreatment2 };
            TestHelper.UnitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dtos, libraryId);

            // Add supersede rule
            var supersedeRule1 = TreatmentDtos.SupersedeRule(preventTreatment1, "Age > 20");
            var supersedeRule2 = TreatmentDtos.SupersedeRule(preventTreatment2, "Age < 5");
            var libraryTeatmentSupersedeRulesPerTreatmentId = TreatmentTestSetup.ModelTreatmentSupersedeRules(treatmentId, new List<TreatmentSupersedeRuleDTO> { supersedeRule1, supersedeRule2 });
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteTreatmentSupersedeRules(libraryTeatmentSupersedeRulesPerTreatmentId, libraryId);

            // Get treatment supersede rules
            var supersedeRules = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRules(treatmentId, libraryId);

            var service = CreateTreatmentService(TestHelper.UnitOfWork);
            // Export
            var fileInfo = service.ExportLibraryTreatmentSupersedeRuleExcelFile(libraryId);
            var dataAsString = fileInfo.FileData;
            var bytes = Convert.FromBase64String(dataAsString);
            var stream = new MemoryStream(bytes);
            File.WriteAllBytes("LibraryTreatmentSupersedeRules.xlsx", bytes);
            var excelPackage = new ExcelPackage(stream);

            // This was attempting to delete via supersede object.            
            var supersedeDictionary = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> { { treatmentId, new List<TreatmentSupersedeRuleDTO>() } };
            TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.UpsertOrDeleteTreatmentSupersedeRules(supersedeDictionary, libraryId);

            // Get treatment supersede rules            
            var treatmentSupersedeRulesEmpty = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRules(treatmentId, libraryId);
            // Assert
            Assert.Empty(treatmentSupersedeRulesEmpty);

            // Import and Get
            service.ImportLibraryTreatmentSupersedeRulesFile(libraryId, excelPackage);
            var supersedeRulesSame = TestHelper.UnitOfWork.TreatmentSupersedeRuleRepo.GetLibraryTreatmentSupersedeRules(treatmentId, libraryId);

            // Assert
            var treatmentSupersedeRule2 = supersedeRules.FirstOrDefault(_=>_.Id == preventTreatment2Id);
            var treatmentSupersedeRule2Same = supersedeRulesSame.FirstOrDefault(_ => _.Id == preventTreatment2Id);
            Assert.Equal(supersedeRules.Count, supersedeRulesSame.Count);
            ObjectAssertions.EquivalentExcluding(treatmentSupersedeRule2, treatmentSupersedeRule2Same,
                _ => _.Id, _ => _.CriterionLibrary.Id, _ => _.CriterionLibrary.Name);
        }
    }
}
