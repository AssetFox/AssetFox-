using System.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Services;
using BridgeCareCore.Services.DefaultData;
using BridgeCareCoreTests.Helpers;
using Moq;
using OfficeOpenXml;
using Xunit;

namespace BridgeCareCoreTests.Tests
{
    public class DatabaseBasedInvestmentBudgetServiceTests
    {
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

        [Fact] // done
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
            ObjectAssertions.EquivalentExcluding(sampleBudget1Dto, addedBudgets[0].Budget, b => b.Id, b => b.BudgetAmounts);
            ObjectAssertions.EquivalentExcluding(sampleBudget2Dto, addedBudgets[1].Budget, b => b.Id, b => b.BudgetAmounts);
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

        [Fact] // done
        public void ImportLibraryInvestmentBudgetsFile_BudgetExists_Overwrites()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var criterionLibraryRepo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var libraryId = Guid.NewGuid();
            var service2 = CreateService(unitOfWork);
            var excelPackage = CreateRequestWithLibraryFormData();
            var year = 2022;
            var budgetId = Guid.NewGuid();
            var sampleBudget1Id = Guid.NewGuid();
            var sampleBudget2Id = Guid.NewGuid();
            decimal fiveMillion = 5000000;
            decimal fourMillion = 4000000;
            var budgetDto = BudgetDtos.WithSingleAmount(budgetId, "Budget", year, 1234);
            var budgetLibraryDto = new BudgetLibraryDTO
            {
                Budgets = new List<BudgetDTO> { budgetDto },
                Id = libraryId,
            };
            var sampleBudget1Dto = BudgetDtos.WithSingleAmount(sampleBudget1Id, "Sample Budget 1", year, fourMillion);
            var sampleBudget2Dto = BudgetDtos.WithSingleAmount(sampleBudget1Id, "Sample Budget 2", year, fiveMillion);
            budgetRepo.Setup(b => b.GetLibraryBudgets(libraryId)).ReturnsList(budgetDto);
            budgetRepo.Setup(b => b.GetBudgetLibrary(libraryId)).Returns(budgetLibraryDto);
            var currentUserCriteriaFilter = new UserCriteriaDTO
            {
                HasCriteria = false,
            };

            var importResult = service2.ImportLibraryInvestmentBudgetsFile(libraryId, excelPackage, currentUserCriteriaFilter, true);

            // Assert
            var expectedImportResult = new BudgetImportResultDTO
            {
                BudgetLibrary = budgetLibraryDto,
            };
            ObjectAssertions.Equivalent(expectedImportResult, importResult);

            var deleteBudgetsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.DeleteAllBudgetsForLibrary));
            var getLibraryBudgetsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.GetLibraryBudgets));
            Assert.Equal(libraryId, deleteBudgetsInvocation.Arguments[0]);
            Assert.Equal(libraryId, getLibraryBudgetsInvocation.Arguments[0]);
            var addBudgetsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.AddBudgets));
            var addBudgetAmountsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.AddLibraryBudgetAmounts));
            var updateBudgetAmountsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpdateLibraryBudgetAmounts));
            var deleteCriterionInvocations = criterionLibraryRepo.InvocationsWithName(nameof(ICriterionLibraryRepository.DeleteAllSingleUseCriterionLibrariesWithBudgetNamesForBudgetLibrary));
            foreach (var deleteInvocation in deleteCriterionInvocations)
            {
                Assert.Equal(libraryId, deleteInvocation.Arguments[0]);
                ObjectAssertions.CheckEnumerable(deleteInvocation.Arguments[1], "Sample Budget 1", "Sample Budget 2");
            }

            var budgetAmountsWithBudgetIds = addBudgetAmountsInvocation.Arguments[0] as List<BudgetAmountDTOWithBudgetId>;
            var budgetsWithLibraryIds = addBudgetsInvocation.Arguments[0] as List<BudgetDTOWithLibraryId>;
            var expectedBudgetWithLibraryId0 = new BudgetDTOWithLibraryId
            {
                Budget = new BudgetDTO
                {
                    Name = "Sample Budget 1",
                },
                BudgetLibraryId = libraryId,
            };
            var expectedBudgetWithLibraryId1 = new BudgetDTOWithLibraryId
            {
                Budget = new BudgetDTO
                {
                    Name = "Sample Budget 2",
                },
                BudgetLibraryId = libraryId,
            };
            ObjectAssertions.EquivalentExcluding(expectedBudgetWithLibraryId0, budgetsWithLibraryIds[0], b => b.Budget.Id);
            ObjectAssertions.EquivalentExcluding(expectedBudgetWithLibraryId1, budgetsWithLibraryIds[1], b => b.Budget.Id);
            var expectedBudgetAmountWithBudgetId0 = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = budgetsWithLibraryIds[0].Budget.Id,
                BudgetAmount = new BudgetAmountDTO
                {
                    Year = 2022,
                    Value = 5000000,
                }
            };
            var expectedBudgetAmountWithBudgetId1 = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = budgetsWithLibraryIds[1].Budget.Id,
                BudgetAmount = new BudgetAmountDTO
                {
                    Year = 2022,
                    Value = 5000000,
                }
            };
            ObjectAssertions.EquivalentExcluding(expectedBudgetAmountWithBudgetId0, budgetAmountsWithBudgetIds[0], x => x.BudgetAmount.Id);
            ObjectAssertions.EquivalentExcluding(expectedBudgetAmountWithBudgetId1, budgetAmountsWithBudgetIds[1], x => x.BudgetAmount.Id);
            var criterionInvocations = criterionLibraryRepo.Invocations.ToList();
            var addCriterionInvocation = criterionLibraryRepo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.AddLibraries));
            var addedCriterionLibraries = addCriterionInvocation.Arguments[0] as List<CriterionLibraryDTO>;
            var expectedAddedCriterionLibrary1 = new CriterionLibraryDTO
            {
                Name = "Sample Budget 1 Criterion Library",
                IsSingleUse = true,
                MergedCriteriaExpression = "[INTERSTATE]=|Y| AND [INTERNET_REPORT]=|State|",
            };
            ObjectAssertions.EquivalentExcluding(expectedAddedCriterionLibrary1, addedCriterionLibraries[0], x => x.Id);
            var expectedAddedCriterionLibrary2 = new CriterionLibraryDTO
            {
                Name = "Sample Budget 2 Criterion Library",
                IsSingleUse = true,
                MergedCriteriaExpression = "[INTERSTATE]='Y' AND [INTERNET_REPORT]='State'",
            };
            ObjectAssertions.EquivalentExcluding(expectedAddedCriterionLibrary2, addedCriterionLibraries[1], x => x.Id);
            var addLibraryBudgetJoinInvocation = criterionLibraryRepo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.AddLibraryBudgetJoins));
            var addedJoins = addLibraryBudgetJoinInvocation.Arguments[0] as List<CriterionLibraryBudgetDTO>;
            var expectedAddedJoins = new List<CriterionLibraryBudgetDTO>
            {
                new CriterionLibraryBudgetDTO
                {
                    CriterionLibraryId = addedCriterionLibraries[0].Id,
                    BudgetId = budgetsWithLibraryIds[0].Budget.Id,
                },
                new CriterionLibraryBudgetDTO
                {
                    CriterionLibraryId = addedCriterionLibraries[1].Id,
                    BudgetId = budgetsWithLibraryIds[1].Budget.Id,
                }
            };
            ObjectAssertions.Equivalent(expectedAddedJoins, addedJoins);
        }

        [Fact] // done
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

        [Fact] // done
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
            var service = CreateService(unitOfWork);

            // Act
            var fileInfo = service.ExportLibraryInvestmentBudgetsFile(libraryId);

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


        [Fact] // done
        public void ImportScenarioInvestmentBudgetsFile_Does()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var criterionLibraryRepo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var service = CreateService(unitOfWork);
            var excelPackage = CreateRequestWithLibraryFormData();
            var year = 2022;
            var budgetId = Guid.NewGuid();
            var sampleBudget1Id = Guid.NewGuid();
            var sampleBudget2Id = Guid.NewGuid();
            decimal fiveMillion = 5000000;
            var budgetDto1 = BudgetDtos.WithSingleAmount(budgetId, "Budget", year, 1234);
            var budgetDtoList1 = new List<BudgetDTO> { budgetDto1 };

            var budgetLibraryDto = new BudgetLibraryDTO
            {
                Budgets = new List<BudgetDTO> { budgetDto1 },
                Id = simulationId,
            };
            var sampleBudget1Dto = BudgetDtos.WithSingleAmount(sampleBudget1Id, "Sample Budget 1", year, fiveMillion);
            var sampleBudget2Dto = BudgetDtos.WithSingleAmount(sampleBudget1Id, "Sample Budget 2", year, fiveMillion);
            var budgetDtoList2 = new List<BudgetDTO> { budgetDto1, sampleBudget1Dto, sampleBudget2Dto };
            budgetRepo.SetupSequence(b => b.GetScenarioBudgets(simulationId)).Returns(budgetDtoList1).Returns(budgetDtoList2);
            budgetRepo.Setup(b => b.GetBudgetLibrary(simulationId)).Returns(budgetLibraryDto);
            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock(unitOfWork);
            var investmentPlanId = Guid.NewGuid();
            var investmentPlan = new InvestmentPlanDTO
            {
                FirstYearOfAnalysisPeriod = 2023,
                Id = investmentPlanId,
                InflationRatePercentage = 3,
                NumberOfYearsInAnalysisPeriod = 1,
                MinimumProjectCostLimit = 500000,
            };
            investmentPlanRepo.Setup(i => i.GetInvestmentPlan(simulationId)).Returns(investmentPlan);
            var currentUserCriteriaFilter = new UserCriteriaDTO
            {
                HasCriteria = false
            };

            // Act
            var result = service.ImportScenarioInvestmentBudgetsFile(simulationId, excelPackage, currentUserCriteriaFilter, false);

            // Assert
            var budgetInvocations = budgetRepo.Invocations.ToList();
            var libraryInvocations = criterionLibraryRepo.Invocations.ToList();
            var addScenarioBudgetsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.AddScenarioBudgets));
            Assert.Equal(simulationId, addScenarioBudgetsInvocation.Arguments[0]);
            var addedScenarioBudgets = addScenarioBudgetsInvocation.Arguments[1] as List<BudgetDTO>;
            Assert.Equal("Sample Budget 1", addedScenarioBudgets[0].Name);
            Assert.Equal("Sample Budget 2", addedScenarioBudgets[1].Name);
            var addScenarioBudgetAmountsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.AddScenarioBudgetAmounts));
            var addedScenarioBudgetAmounts = addScenarioBudgetAmountsInvocation.Arguments[0] as List<BudgetAmountDTOWithBudgetId>;
            var expectedAddedScenarioBudgetAmount0 = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = addedScenarioBudgets[0].Id,
                BudgetAmount = new BudgetAmountDTO
                {
                    Year = 2022,
                    Value = 5000000,
                }
            };
            var expectedAddedScenarioBudgetAmount1 = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = addedScenarioBudgets[1].Id,
                BudgetAmount = new BudgetAmountDTO
                {
                    Year = 2022,
                    Value = 5000000,
                }
            };
            ObjectAssertions.EquivalentExcluding(expectedAddedScenarioBudgetAmount0, addedScenarioBudgetAmounts[0], x => x.BudgetAmount.Id);
            ObjectAssertions.EquivalentExcluding(expectedAddedScenarioBudgetAmount1, addedScenarioBudgetAmounts[1], x => x.BudgetAmount.Id);
            var updateScenarioBudgetAmountsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpdateScenarioBudgetAmounts));
            Assert.Equal(simulationId, updateScenarioBudgetAmountsInvocation.Arguments[0]);
            var expectedUpdatedAmount = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = budgetDto1.Id,
                BudgetAmount = new BudgetAmountDTO
                {
                    BudgetName = "Budget",
                    Value = 1234m,
                    Year = 2022,
                    Id = budgetDto1.BudgetAmounts[0].Id,
                }
            };
            ObjectAssertions.EquivalentSingleton(expectedUpdatedAmount, updateScenarioBudgetAmountsInvocation.Arguments[1]);
            var addLibrariesInvocation = criterionLibraryRepo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.AddLibraries));
            var addedCriterionLibraries = addLibrariesInvocation.Arguments[0] as List<CriterionLibraryDTO>;
            var expectedAddedCriterionLibrary1 = new CriterionLibraryDTO
            {
                Name = "Sample Budget 1 Criterion Library",
                IsSingleUse = true,
                MergedCriteriaExpression = "[INTERSTATE]=|Y| AND [INTERNET_REPORT]=|State|",
            };
            ObjectAssertions.EquivalentExcluding(expectedAddedCriterionLibrary1, addedCriterionLibraries[0], x => x.Id);
            var expectedAddedCriterionLibrary2 = new CriterionLibraryDTO
            {
                Name = "Sample Budget 2 Criterion Library",
                IsSingleUse = true,
                MergedCriteriaExpression = "[INTERSTATE]='Y' AND [INTERNET_REPORT]='State'",
            };
            ObjectAssertions.EquivalentExcluding(expectedAddedCriterionLibrary2, addedCriterionLibraries[1], x => x.Id);
            var addJoinsInvocation = criterionLibraryRepo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.AddLibraryScenarioBudgetJoins));
            var addedJoins = addJoinsInvocation.Arguments[0] as List<CriterionLibraryScenarioBudgetDTO>;
            var expectedJoins = new List<CriterionLibraryScenarioBudgetDTO>
            {
                new CriterionLibraryScenarioBudgetDTO
                {
                    CriterionLibraryId = addedCriterionLibraries[0].Id,
                    ScenarioBudgetId = addedScenarioBudgets[0].Id,
                },
                new CriterionLibraryScenarioBudgetDTO
                {
                    CriterionLibraryId= addedCriterionLibraries[1].Id,
                    ScenarioBudgetId= addedScenarioBudgets[1].Id,
                }
            };
            ObjectAssertions.Equivalent(expectedJoins, addedJoins);
        }

        [Fact] // done
        public void ImportScenarioInvestmentBudgetsFile_BudgetExists_Overwrites()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var criterionLibraryRepo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var simulationId = Guid.NewGuid();
            var service = CreateService(unitOfWork);
            var excelPackage = CreateRequestWithLibraryFormData();
            var year = 2022;
            var sampleBudget1Id = Guid.NewGuid();
            var sampleBudget2Id = Guid.NewGuid();
            decimal fiveMillion = 5000000;
            decimal fourMillion = 4000000;
            var sampleBudget1Dto4Million = BudgetDtos.WithSingleAmount(sampleBudget1Id, "Sample Budget 1", year, fourMillion);
            var budgetDtoList1 = new List<BudgetDTO> { sampleBudget1Dto4Million };
            var sampleBudget1Dto5Million = BudgetDtos.WithSingleAmount(sampleBudget1Id, "Sample Budget 1", year, fiveMillion);
            var sampleBudget2Dto = BudgetDtos.WithSingleAmount(sampleBudget1Id, "Sample Budget 2", year, fiveMillion);
            var budgetDtoList2 = new List<BudgetDTO> { sampleBudget1Dto5Million, sampleBudget2Dto };
            var budgetLibraryDto = new BudgetLibraryDTO
            {
                Budgets = new List<BudgetDTO> { sampleBudget1Dto4Million, sampleBudget2Dto },
                Id = simulationId,
            };
            budgetRepo.SetupSequence(b => b.GetScenarioBudgets(simulationId)).Returns(budgetDtoList1).Returns(budgetDtoList2);
            budgetRepo.Setup(b => b.GetBudgetLibrary(simulationId)).Returns(budgetLibraryDto);
            var investmentPlanRepo = InvestmentPlanRepositoryMocks.NewMock(unitOfWork);
            var investmentPlanId = Guid.NewGuid();
            var investmentPlan = new InvestmentPlanDTO
            {
                FirstYearOfAnalysisPeriod = 2023,
                Id = investmentPlanId,
                InflationRatePercentage = 3,
                NumberOfYearsInAnalysisPeriod = 1,
                MinimumProjectCostLimit = 500000,
            };
            investmentPlanRepo.Setup(i => i.GetInvestmentPlan(simulationId)).Returns(investmentPlan);
            var currentUserCriteriaFilter = new UserCriteriaDTO
            {
                HasCriteria = false
            };

            // Act
            var result = service.ImportScenarioInvestmentBudgetsFile(simulationId, excelPackage, currentUserCriteriaFilter, false);

            // Assert
            var budgetInvocations = budgetRepo.Invocations.ToList();
            var libraryInvocations = criterionLibraryRepo.Invocations.ToList();
            var addScenarioBudgetsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.AddScenarioBudgets));
            Assert.Equal(simulationId, addScenarioBudgetsInvocation.Arguments[0]);
            var addedScenarioBudgets = addScenarioBudgetsInvocation.Arguments[1] as List<BudgetDTO>;
            Assert.Equal("Sample Budget 2", addedScenarioBudgets[0].Name);
            Assert.Single(addedScenarioBudgets);
            var addScenarioBudgetAmountsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.AddScenarioBudgetAmounts));
            var addedScenarioBudgetAmounts = addScenarioBudgetAmountsInvocation.Arguments[0] as List<BudgetAmountDTOWithBudgetId>;
            var expectedAddedScenarioBudgetAmount0 = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = addedScenarioBudgets[0].Id,
                BudgetAmount = new BudgetAmountDTO
                {
                    Year = 2022,
                    Value = 5000000,
                }
            };
            ObjectAssertions.EquivalentExcluding(expectedAddedScenarioBudgetAmount0, addedScenarioBudgetAmounts[0], x => x.BudgetAmount.Id);
            var updateScenarioBudgetAmountsInvocation = budgetRepo.SingleInvocationWithName(nameof(IBudgetRepository.UpdateScenarioBudgetAmounts));
            Assert.Equal(simulationId, updateScenarioBudgetAmountsInvocation.Arguments[0]);
            var expectedUpdatedAmount = new BudgetAmountDTOWithBudgetId
            {
                BudgetId = sampleBudget1Dto4Million.Id,
                BudgetAmount = new BudgetAmountDTO
                {
                    BudgetName = "Sample Budget 1",
                    Value = fiveMillion,
                    Year = 2022,
                    Id = sampleBudget1Dto4Million.BudgetAmounts[0].Id,
                }
            };
            ObjectAssertions.EquivalentSingleton(expectedUpdatedAmount, updateScenarioBudgetAmountsInvocation.Arguments[1]);
            var addLibrariesInvocation = criterionLibraryRepo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.AddLibraries));
            var addedCriterionLibraries = addLibrariesInvocation.Arguments[0] as List<CriterionLibraryDTO>;
            var expectedAddedCriterionLibrary1 = new CriterionLibraryDTO
            {
                Name = "Sample Budget 1 Criterion Library",
                IsSingleUse = true,
                MergedCriteriaExpression = "[INTERSTATE]=|Y| AND [INTERNET_REPORT]=|State|",
            };
            ObjectAssertions.EquivalentExcluding(expectedAddedCriterionLibrary1, addedCriterionLibraries[0], x => x.Id);
            var expectedAddedCriterionLibrary2 = new CriterionLibraryDTO
            {
                Name = "Sample Budget 2 Criterion Library",
                IsSingleUse = true,
                MergedCriteriaExpression = "[INTERSTATE]='Y' AND [INTERNET_REPORT]='State'",
            };
            ObjectAssertions.EquivalentExcluding(expectedAddedCriterionLibrary2, addedCriterionLibraries[1], x => x.Id);
            var addJoinsInvocation = criterionLibraryRepo.SingleInvocationWithName(nameof(ICriterionLibraryRepository.AddLibraryScenarioBudgetJoins));
            var addedJoins = addJoinsInvocation.Arguments[0] as List<CriterionLibraryScenarioBudgetDTO>;
            var expectedJoins = new List<CriterionLibraryScenarioBudgetDTO>
            {
                new CriterionLibraryScenarioBudgetDTO
                {
                    CriterionLibraryId = addedCriterionLibraries[0].Id,
                    ScenarioBudgetId = sampleBudget1Id,
                },
                new CriterionLibraryScenarioBudgetDTO
                {
                    CriterionLibraryId= addedCriterionLibraries[1].Id,
                }
            };
            ObjectAssertions.EquivalentExcluding(expectedJoins, addedJoins, x => x[1].ScenarioBudgetId);
        }

        [Fact]
        public void ExportScenarioInvestmentBudgetsFile_Does()
        {
            // Arrange
            var unitOfWork = UnitOfWorkMocks.New();
            var budgetRepo = BudgetRepositoryMocks.New(unitOfWork);
            var budgetAmountRepo = BudgetAmountRepositoryMocks.New(unitOfWork);
            var criterionLibraryRepo = CriterionLibraryRepositoryMocks.New(unitOfWork);
            var simulationRepo = SimulationRepositoryMocks.DefaultMock(unitOfWork);
            var simulationId = Guid.NewGuid();
            var budgetId = Guid.NewGuid();
            var budget = BudgetDtos.New(budgetId);
            var budgetAmount = BudgetAmountDtos.ForBudgetAndYear(budget, 2022, 500000);
            var budgetAmounts = new List<BudgetAmountDTO> { budgetAmount };
            budgetAmountRepo.Setup(b => b.GetScenarioBudgetAmounts(simulationId)).Returns(budgetAmounts);
            var simulationName = "Simulation";
            simulationRepo.Setup(r => r.GetSimulationName(simulationId)).Returns(simulationName);
            var criteria = new Dictionary<string, string> { { "Budget", "expression" } };
            budgetRepo.Setup(b => b.GetCriteriaPerBudgetNameForSimulation(simulationId)).Returns(criteria);
            var service = CreateService(unitOfWork);

            // Act
            var fileInfo = service.ExportScenarioInvestmentBudgetsFile(simulationId);

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
            var worksheetBudgetName = worksheetBudgetNames.Single();
            Assert.Equal("Budget", worksheetBudgetName);

            var worksheetBudgetYearAndAmount = worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]
                .Select(cell => cell.GetValue<string>()).ToList();
            ObjectAssertions.Equivalent(new List<string> { "2022", "500000" }, worksheetBudgetYearAndAmount );
        }
    }
}
