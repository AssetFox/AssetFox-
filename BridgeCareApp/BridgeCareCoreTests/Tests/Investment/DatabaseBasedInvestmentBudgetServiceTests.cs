using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Xunit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using Microsoft.Extensions.Primitives;
using Moq;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models.DefaultData;
using BridgeCareCore.Services;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Utils.Interfaces;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCoreTests.Tests.Investment;

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

        private InvestmentController CreateDatabaseAuthorizedController(InvestmentBudgetsService service, IHttpContextAccessor accessor = null)
        {
            _mockInvestmentDefaultDataService.Setup(m => m.GetInvestmentDefaultData()).ReturnsAsync(new InvestmentDefaultData());
            accessor ??= HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var pagingService = InvestmentPagingServiceMocks.DefaultMock();
            var controller = new InvestmentController(service,
                pagingService.Object,
                EsecSecurityMocks.Admin,
                TestHelper.UnitOfWork,
                hubService,
                accessor,
                _mockInvestmentDefaultDataService.Object,
                _mockClaimHelper.Object);
            return controller;
        }

        [Fact]
        public void ImportLibraryInvestmentBudgetsFile_Does()
        {
            // Arrange
            var service = DatabaseBasedInvestmentBudgetServiceTestSetup.SetupDatabaseBasedService(TestHelper.UnitOfWork);
            AddTestDataToDatabase();
            var excelPackage = CreateRequestWithLibraryFormData();
            var year = 2022;

            var currentUserCriteriaFilter = new UserCriteriaDTO
            {
                HasCriteria = false
            };
            // need to construct the ExcelPackage in order to call the service.
            service.ImportLibraryInvestmentBudgetsFile(_testBudgetLibrary.Id, excelPackage, currentUserCriteriaFilter, false);

            // Assert
            var budgetAmounts = TestHelper.UnitOfWork.BudgetAmountRepo
                .GetLibraryBudgetAmounts(_testBudgetLibrary.Id)
                .Where(_ => _.Budget.Name.IndexOf("Sample") != -1)
                .ToList();

            Assert.Equal(2, budgetAmounts.Count);
            Assert.True(budgetAmounts.All(_ => _.Year == year));
            Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

            var budgets = TestHelper.UnitOfWork.BudgetRepo.GetLibraryBudgets(_testBudgetLibrary.Id);
            Assert.Equal(3, budgets.Count);
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 1");
            Assert.Contains(budgets, _ => _.Name == "Sample Budget 2");

            var criteriaPerBudgetName = GetCriteriaPerBudgetName();
            var budgetNames = budgets.Where(_ => _.Name.Contains("Sample Budget")).Select(_ => _.Name).ToList();
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
        public void ExportLibraryInvestmentBudgetsFile_NoBudgetAmountsInDatabase_CreatesSampleFile()
        {
            // Arrange
            var year = DateTime.Now.Year;
            var service = DatabaseBasedInvestmentBudgetServiceTestSetup.SetupDatabaseBasedService(TestHelper.UnitOfWork);
            AddTestDataToDatabase();
            TestHelper.UnitOfWork.Context.DeleteAll<BudgetAmountEntity>(_ => _.BudgetId == _testBudget.Id);

            // Act
            var fileInfo = service.ExportLibraryInvestmentBudgetsFile(_testBudgetLibrary.Id);

            // Assert
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
            var service = DatabaseBasedInvestmentBudgetServiceTestSetup.SetupDatabaseBasedService(TestHelper.UnitOfWork);
            AddTestDataToDatabase();
            var year = DateTime.Now.Year;

            // Act
            var fileInfo = service.ExportLibraryInvestmentBudgetsFile(_testBudgetLibrary.Id);

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
            Assert.Equal(_testBudget.Name, worksheetBudgetNames[0]);

            var worksheetBudgetYearAndAmount = worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            Assert.Equal(year.ToString(), worksheetBudgetYearAndAmount[0]);
            Assert.Equal(_testBudget.BudgetAmounts.ToList()[0].Value.ToString(), worksheetBudgetYearAndAmount[1]);
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
