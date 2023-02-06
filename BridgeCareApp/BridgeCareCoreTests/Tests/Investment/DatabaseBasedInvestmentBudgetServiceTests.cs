using System.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Models.DefaultData;
using BridgeCareCore.Services;
using BridgeCareCore.Services.DefaultData;
using BridgeCareCore.Utils.Interfaces;
using BridgeCareCoreTests.Helpers;
using BridgeCareCoreTests.Tests.Investment;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using OfficeOpenXml;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class DatabaseBasedInvestmentBudgetServiceTests
    {
        // The code under test in these tests accesses the db context, without
        // going through a repo. Removing real db access from the tests is therefore
        // difficult.
        private BudgetEntity _testBudget;
        private BudgetLibraryEntity _testBudgetLibrary;
        private InvestmentPlanEntity _testInvestmentPlan;
        private ScenarioBudgetEntity _testScenarioBudget;
        private const string BudgetEntityName = "Budget";
        private readonly Mock<IInvestmentDefaultDataService> _mockInvestmentDefaultDataService = new Mock<IInvestmentDefaultDataService>();
        private readonly Mock<IClaimHelper> _mockClaimHelper = new();

        private InvestmentBudgetsService CreateService(Mock<IUnitOfWork> unitOfWork)
        {
            var investmentDefaultDataService = new InvestmentDefaultDataService();
            var expressionValidationService = ExpressionValidationServiceMocks.EverythingIsValid();
            var hubService = HubServiceMocks.DefaultMock();
            var service = new InvestmentBudgetsService(
                unitOfWork.Object,
                expressionValidationService.Object,
                hubService.Object,
                investmentDefaultDataService
                );
            return service;
        }
        private Dictionary<string, string> GetCriteriaPerBudgetName()
        {
            var criteriaPerBudgetName = new Dictionary<string, string>();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestInvestmentBudgets.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var excelPackage = new ExcelPackage(memStream);
            var criteriaWorksheet = excelPackage.Workbook.Worksheets[1];
            var budgetCol = 1;
            var criteriaCol = 2;
            for (var row = 2; row <= criteriaWorksheet.Dimension.End.Row; row++)
            {
                var budgetName = criteriaWorksheet.GetValue<string>(row, budgetCol);
                var criteria = criteriaWorksheet.GetValue<string>(row, criteriaCol);
                if (!criteriaPerBudgetName.ContainsKey(budgetName))
                {
                    criteriaPerBudgetName.Add(budgetName, string.Empty);
                }

                criteriaPerBudgetName[budgetName] = criteria;
            }

            return criteriaPerBudgetName;
        }

        private void AddTestDataToDatabase()
        {
            var year = DateTime.Now.Year;
            _testBudgetLibrary = new BudgetLibraryEntity { Id = Guid.NewGuid(), Name = "Test Name" };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudgetLibrary);
            TestHelper.UnitOfWork.Context.SaveChanges();

            _testBudget = new BudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = BudgetEntityName,
                BudgetLibraryId = _testBudgetLibrary.Id,
                BudgetAmounts =
                new List<BudgetAmountEntity>
                {
                   new BudgetAmountEntity {Id = Guid.NewGuid(), Year = year, Value = 500000}
                },
                CriterionLibraryBudgetJoin = new CriterionLibraryBudgetEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        MergedCriteriaExpression = "expression",
                        Name = "Criterion"
                    }
                }
            };
            TestHelper.UnitOfWork.Context.AddEntity(_testBudget);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private void AddScenarioDataToDatabase(Guid simulationId)
        {
            var year = DateTime.Now.Year;
            _testInvestmentPlan = new InvestmentPlanEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                FirstYearOfAnalysisPeriod = year,
                NumberOfYearsInAnalysisPeriod = 1,
                MinimumProjectCostLimit = 500000,
                InflationRatePercentage = 3
            };
            var investmentPlan = TestHelper.UnitOfWork.Context.InvestmentPlan.FirstOrDefault(ip => ip.SimulationId == simulationId);
            if (investmentPlan != null)
            {
                TestHelper.UnitOfWork.Context.Remove(investmentPlan);
            }
            TestHelper.UnitOfWork.Context.AddEntity(_testInvestmentPlan);

            _testScenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = BudgetEntityName,
                SimulationId = simulationId,
                ScenarioBudgetAmounts =
                        new List<ScenarioBudgetAmountEntity>
                        {
                        new ScenarioBudgetAmountEntity
                        {
                            Id = Guid.NewGuid(), Year = year, Value = 5000000
                        }
                        },
                CriterionLibraryScenarioBudgetJoin = new CriterionLibraryScenarioBudgetEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(),
                        MergedCriteriaExpression = "expression",
                        Name = "Criterion"
                    }
                }
            };
            var scenarioBudget = TestHelper.UnitOfWork.Context.ScenarioBudget.FirstOrDefault(b => b.Name == BudgetEntityName);
            if (scenarioBudget != null)
            {
                TestHelper.UnitOfWork.Context.Remove(scenarioBudget);
            }
            TestHelper.UnitOfWork.Context.AddEntity(_testScenarioBudget);
            TestHelper.UnitOfWork.Context.SaveChanges();
        }

        private ExcelPackage CreateRequestWithLibraryFormData(bool overwriteBudgets = false)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestInvestmentBudgets.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var excelPackage = new ExcelPackage(memStream);
            return excelPackage;
        }

        private ExcelPackage CreateRequestWithScenarioFormData(Guid simulationId, bool overwriteBudgets = false)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestInvestmentBudgets.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var excelPackage = new ExcelPackage(memStream);
            return excelPackage;
        }

        [Fact]
        public void ImportLibraryInvestmentBudgetsFile_Does()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var budgetAmountRepo = BudgetAmountRepositoryMocks.New(unitOfWork);
            var criterionLibraryRepo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var service = CreateService(unitOfWork);
            var excelPackage = CreateRequestWithLibraryFormData();
            var year = 2022;
            var budgetId = Guid.NewGuid();
            var sampleBudget1Id = Guid.NewGuid();
            var sampleBudget2Id = Guid.NewGuid();
            decimal fiveMillion = 5000000;
            var budgetDto = BudgetDtos.WithSingleAmount(budgetId, "Budget", year, 1234);
            var sampleBudget1Dto = BudgetDtos.WithSingleAmount(sampleBudget1Id, "Sample Budget 1", year, fiveMillion);
            var sampleBudget2Dto = BudgetDtos.WithSingleAmount(sampleBudget1Id, "Sample Budget 2", year, fiveMillion);
            budgetRepo.Setup(b => b.GetLibraryBudgets(libraryId)).ReturnsList(budgetDto);
            var currentUserCriteriaFilter = new UserCriteriaDTO
            {
                HasCriteria = false
            };
            var expectedBudgetLibraryAfter = new BudgetLibraryDTO
            {
                Name = "Test Name",
                Id = libraryId,
                Budgets = new List<BudgetDTO>
                {
                    budgetDto,
                    sampleBudget1Dto,
                    sampleBudget2Dto,
                },
            };
            budgetRepo.Setup(b => b.GetBudgetLibrary(libraryId)).Returns(expectedBudgetLibraryAfter);

            var result = service.ImportLibraryInvestmentBudgetsFile(libraryId, excelPackage, currentUserCriteriaFilter, false);

            // Assert
            ObjectAssertions.Equivalent(expectedBudgetLibraryAfter, result.BudgetLibrary);
            var invocations = budgetRepo.Invocations.ToList();
            var addInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.AddBudgets));
            var addedBudgets = addInvocation.Arguments[0] as List<BudgetDTOWithLibraryId>;
            Assert.Equal(2, addedBudgets.Count);
            ObjectAssertions.EquivalentExcluding(sampleBudget1Dto, addedBudgets[0].Budget, b => b.Id, b=> b.BudgetAmounts);
            ObjectAssertions.EquivalentExcluding(sampleBudget2Dto, addedBudgets[1].Budget, b => b.Id, b=> b.BudgetAmounts);
            Assert.Equal(libraryId, addedBudgets[0].BudgetLibraryId);
            Assert.Equal(libraryId, addedBudgets[1].BudgetLibraryId);
            var amountInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.AddLibraryBudgetAmounts));
            var addedAmounts = amountInvocation.Arguments[0] as List<BudgetAmountDTOWithBudgetId>;
            Assert.Equal(2, addedAmounts.Count);
            var expectedAmountZero = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = addedBudgets[0].Budget.Id,
                BudgetAmount = new BudgetAmountDTO
                {
                    Value = 5000000,
                    Year = 2022,
                }
            };
            var expectedAmountOne = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = addedBudgets[1].Budget.Id,
                BudgetAmount = new BudgetAmountDTO
                {
                    Value = 5000000,
                    Year = 2022,
                }
            };
            ObjectAssertions.EquivalentExcluding(expectedAmountZero, addedAmounts[0], a => a.BudgetAmount.Id);
            ObjectAssertions.EquivalentExcluding(expectedAmountOne, addedAmounts[1], a => a.BudgetAmount.Id);
            var updateInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpdateLibraryBudgetAmounts));
            var updatedAmounts = updateInvocation.Arguments[0] as List<BudgetAmountDTOWithBudgetId>;
            var expectedUpdateAmount = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = budgetId,
                BudgetAmount = new BudgetAmountDTO
                {
                    Id = budgetDto.BudgetAmounts[0].Id,
                    Year = 2022,
                    Value = 1234,
                    BudgetName = "Budget",
                }
            };
            var updatedAmount = updatedAmounts.Single();
            ObjectAssertions.Equivalent(expectedUpdateAmount, updatedAmount);
        }

        [Fact (Skip ="WJ needs working front end to investigate")]
        public void ImportLibraryInvestmentBudgetsFile_BudgetExists_Overwrites()
        {
            // Arrange
            var year = DateTime.Now.Year;
            var service = DatabaseBasedInvestmentBudgetServiceTestSetup.SetupDatabaseBasedService(TestHelper.UnitOfWork);
            AddTestDataToDatabase();
            var excelPackage = CreateRequestWithLibraryFormData();

            _testBudget.Name = "Sample Budget 1";
            TestHelper.UnitOfWork.Context.UpdateEntity(_testBudget, _testBudget.Id);

            var budgetAmount = _testBudget.BudgetAmounts.ToList()[0];
            budgetAmount.Year = year;
            budgetAmount.Value = 4000000;
            TestHelper.UnitOfWork.Context.UpdateEntity(budgetAmount, budgetAmount.Id);
            var currentUserCriteriaFilter = new UserCriteriaDTO
            {
                HasCriteria = false
            };
            // need to construct the ExcelPackage in order to call the service.

            service.ImportLibraryInvestmentBudgetsFile(_testBudgetLibrary.Id, excelPackage, currentUserCriteriaFilter, false);

            // Assert
            var budgetAmounts =
                TestHelper.UnitOfWork.BudgetAmountRepo.GetLibraryBudgetAmounts(_testBudgetLibrary.Id);
            Assert.Equal(2, budgetAmounts.Count);
            Assert.True(budgetAmounts.All(_ => _.Year == year));
            Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

            var budgets = TestHelper.UnitOfWork.BudgetRepo.GetLibraryBudgets(_testBudgetLibrary.Id);
            Assert.Equal(2, budgets.Count);
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 1");
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 2");

            var criteriaPerBudgetName = GetCriteriaPerBudgetName();
            var budgetNames = budgets.Where(_ => _.Name.Contains("Sample Budget")).Select(_ => _.Name).ToList();
            var criteria = TestHelper.UnitOfWork.Context.CriterionLibrary.AsNoTracking().AsSplitQuery()
                .Where(_ => _.IsSingleUse &&
                            _.CriterionLibraryScenarioBudgetJoins.Any(join =>
                                budgetNames.Contains(join.ScenarioBudget.Name)))
                .Include(_ => _.CriterionLibraryScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                .ToList();
        }

        [Fact]
        public void ExportLibraryInvestmentBudgetsFile_NoBudgetAmountsReturnedFromRepo_CreatesSampleFile()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetAmountRepo = BudgetAmountRepositoryMocks.New(unitOfWork);
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            budgetAmountRepo.Setup(b => b.GetLibraryBudgetAmounts(libraryId)).ReturnsEmptyList();
            var meaninglessDictionary = new Dictionary<string, string> { { "Budget", "expression" } };
            budgetRepo.Setup(b => b.GetCriteriaPerBudgetNameForBudgetLibrary(libraryId)).Returns(meaninglessDictionary);
            var service2 = CreateService(unitOfWork);
            var year = 2023;

            // Act
            var fileInfo = service2.ExportLibraryInvestmentBudgetsFile(libraryId);

            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
            Assert.Equal("sample_investment_budgets_import_export_file.xlsx", fileInfo.FileName);

            var file = Convert.FromBase64String(fileInfo.FileData);
            var memStream = new MemoryStream();
            memStream.Write(file, 0, file.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            var excelPackage = new ExcelPackage(memStream);
            var worksheet = excelPackage.Workbook.Worksheets[0];

            var worksheetBudgetNames = worksheet.Cells[1, 2, 1, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.Equal(4, worksheetBudgetNames.Count);
            Assert.True(worksheetBudgetNames.All(name => name.Contains("Sample Budget")));

            var expectedYear = year;
            worksheet.Cells[2, 1, worksheet.Dimension.End.Row, 1]
                .Select(cell => cell.GetValue<int>()).ToList().ForEach(year =>
                {
                    Assert.Equal(expectedYear, year);
                    expectedYear++;
                });

            var budgetAmounts = worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<decimal>()).ToList();
            Assert.True(budgetAmounts.All(amount => amount == decimal.Parse("5000000")));
        }

        [Fact]
        public void ExportLibraryInvestmentBudgetsFile_Does()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var budgetAmountRepo = BudgetAmountRepositoryMocks.New(unitOfWork);
            var criterionLibraryRepo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            var budgetAmount = BudgetAmountDtos.ForBudgetAndYear(budget, 2022, 500000);
            var budgetAmounts = new List<BudgetAmountDTO> { budgetAmount };
            budgetAmountRepo.Setup(b => b.GetLibraryBudgetAmounts(libraryId)).Returns(budgetAmounts);
            var criteria = new Dictionary<string, string> { { "Budget", "expression" } };
            budgetRepo.Setup(b => b.GetCriteriaPerBudgetNameForBudgetLibrary(libraryId)).Returns(criteria);
            budgetRepo.Setup(b => b.GetBudgetLibraryName(libraryId)).Returns("Test Name");
            var service = DatabaseBasedInvestmentBudgetServiceTestSetup.SetupDatabaseBasedService(TestHelper.UnitOfWork);
            var service2 = CreateService(unitOfWork);

            // Act
            var fileInfo = service2.ExportLibraryInvestmentBudgetsFile(libraryId);

            // Assert
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
            Assert.Equal("Test_Name_investment_budgets.xlsx", fileInfo.FileName);

            var file = Convert.FromBase64String(fileInfo.FileData);
            var memStream = new MemoryStream();
            memStream.Write(file, 0, file.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            var excelPackage = new ExcelPackage(memStream);
            var worksheet = excelPackage.Workbook.Worksheets[0];

            var worksheetBudgetNames = worksheet.Cells[1, 2, 1, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.Single(worksheetBudgetNames);
            Assert.Equal("Budget", worksheetBudgetNames[0]);

            var worksheetBudgetYearAndAmount = worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.Equal(2022.ToString(), worksheetBudgetYearAndAmount[0]);
            Assert.Equal("500000", worksheetBudgetYearAndAmount[1]);
        }


        [Fact]
        public void ImportScenarioInvestmentBudgetsFile_Does()
        {
            // Arrange
            var year = 2022;
            var service = DatabaseBasedInvestmentBudgetServiceTestSetup.SetupDatabaseBasedService(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            var excelPackage = CreateRequestWithScenarioFormData(simulation.Id);
            AddScenarioDataToDatabase(simulation.Id);
            var currentUserCriteriaFilter = new UserCriteriaDTO
            {
                HasCriteria = false
            };

            // Act
            service.ImportScenarioInvestmentBudgetsFile(simulation.Id, excelPackage, currentUserCriteriaFilter, false);
            // Assert

            var budgetAmounts = TestHelper.UnitOfWork.BudgetAmountRepo
                .GetScenarioBudgetAmounts(simulation.Id)
                .Where(_ => _.BudgetName.IndexOf("Sample") != -1)
                .ToList();

            Assert.Equal(2, budgetAmounts.Count);
            Assert.True(budgetAmounts.All(_ => _.Year == year));
            Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

            var budgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            Assert.Equal(3, budgets.Count);
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 1");
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 2");

            var criteriaPerBudgetName = GetCriteriaPerBudgetName();
            var budgetNames = budgets.Where(_ => _.Name.Contains("Sample Budget")).Select(_ => _.Name).ToList();
        }

        [Fact(Skip = "WJ needs working front end to investigate")]
        public void ImportScenarioInvestmentBudgetsFile_BudgetExists_Overwrites()
        {
            // Arrange
            var year = DateTime.Now.Year;
            var service = DatabaseBasedInvestmentBudgetServiceTestSetup.SetupDatabaseBasedService(TestHelper.UnitOfWork);
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork);
            AddScenarioDataToDatabase(simulation.Id);
            var excelPackage = CreateRequestWithScenarioFormData(simulation.Id);

            _testScenarioBudget.Name = "Sample Budget 1";
            TestHelper.UnitOfWork.Context.UpdateEntity(_testScenarioBudget, _testScenarioBudget.Id);

            var budgetAmount = _testScenarioBudget.ScenarioBudgetAmounts.ToList()[0];
            budgetAmount.Year = year;
            budgetAmount.Value = 4000000;
            TestHelper.UnitOfWork.Context.UpdateEntity(budgetAmount, budgetAmount.Id);
            var currentUserCriteriaFilter = new UserCriteriaDTO
            {
                HasCriteria = false
            };

            // Act
            service.ImportScenarioInvestmentBudgetsFile(simulation.Id, excelPackage, currentUserCriteriaFilter, false);

            // Assert
            var budgetAmounts =
                TestHelper.UnitOfWork.BudgetAmountRepo.GetScenarioBudgetAmounts(simulation.Id);
            Assert.Equal(2, budgetAmounts.Count);
            Assert.True(budgetAmounts.All(_ => _.Year == year));
            Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

            var budgets = TestHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(simulation.Id);
            Assert.Equal(2, budgets.Count);
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 1");
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 2");
            var criteriaPerBudgetName = GetCriteriaPerBudgetName();
            var budgetNames = budgets.Where(_ => _.Name.Contains("Sample Budget")).Select(_ => _.Name).ToList();
            var criteria = TestHelper.UnitOfWork.Context.CriterionLibrary.AsNoTracking().AsSplitQuery()
                .Where(_ => _.IsSingleUse &&
                            _.CriterionLibraryScenarioBudgetJoins.Any(join =>
                                budgetNames.Contains(join.ScenarioBudget.Name)))
                .Include(_ => _.CriterionLibraryScenarioBudgetJoins)
                .ThenInclude(_ => _.ScenarioBudget)
                .ToList();
        }

        [Fact]
        public void ExportScenarioInvestmentBudgetsFile_Does()
        {
            // Arrange
            var service = DatabaseBasedInvestmentBudgetServiceTestSetup.SetupDatabaseBasedService(TestHelper.UnitOfWork);
            var simulationName = RandomStrings.Length11();
            var simulation = SimulationTestSetup.CreateSimulation(TestHelper.UnitOfWork, null, simulationName);
            AddScenarioDataToDatabase(simulation.Id);
            var accessor = CreateRequestWithScenarioFormData(simulation.Id);
            // Act
            var fileInfo = service.ExportScenarioInvestmentBudgetsFile(simulation.Id);

            // Assert
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
            Assert.Equal($"{simulationName}_investment_budgets.xlsx", fileInfo.FileName);

            var file = Convert.FromBase64String(fileInfo.FileData);
            var memStream = new MemoryStream();
            memStream.Write(file, 0, file.Length);
            memStream.Seek(0, SeekOrigin.Begin);

            var excelPackage = new ExcelPackage(memStream);
            var worksheet = excelPackage.Workbook.Worksheets[0];

            var worksheetBudgetNames = worksheet.Cells[1, 2, 1, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.True(worksheetBudgetNames.Any());
            Assert.Equal(_testScenarioBudget.Name, worksheetBudgetNames[0]);

            var worksheetBudgetYearAndAmount = worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            //  Assert.Equal(expectedYear.ToString(), worksheetBudgetYearAndAmount[0]);
            Assert.Equal(_testScenarioBudget.ScenarioBudgetAmounts.ToList()[0].Value.ToString(), worksheetBudgetYearAndAmount[1]);
        }
    }
}
