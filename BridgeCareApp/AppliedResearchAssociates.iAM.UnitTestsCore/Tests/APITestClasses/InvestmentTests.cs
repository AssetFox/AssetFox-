using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services;
using BridgeCareCore.Services.SummaryReport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MoreLinq;
using OfficeOpenXml;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class InvestmentTests
    {
        private readonly TestHelper _testHelper;
        private readonly InvestmentBudgetsService _service;
        private InvestmentController _controller;

        private BudgetLibraryEntity _testBudgetLibrary;
        private BudgetEntity _testBudget;
        private InvestmentPlanEntity _testInvestmentPlan;
        private ScenarioBudgetEntity _testScenarioBudget;

        public InvestmentTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _service = new InvestmentBudgetsService(_testHelper.UnitOfWork);
        }

        private void CreateAuthorizedController() =>
            _controller = new InvestmentController(_service, _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

        private void CreateUnauthorizedController() =>
            _controller = new InvestmentController(_service, _testHelper.MockEsecSecurityNotAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);

        private void CreateLibraryTestData()
        {
            _testBudgetLibrary = new BudgetLibraryEntity {Id = Guid.NewGuid(), Name = "Test Name"};
            _testHelper.UnitOfWork.Context.AddEntity(_testBudgetLibrary);


            _testBudget = new BudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = "Budget",
                BudgetLibraryId = _testBudgetLibrary.Id,
                BudgetAmounts =
                    new List<BudgetAmountEntity>
                    {
                        new BudgetAmountEntity {Id = Guid.NewGuid(), Year = DateTime.Now.Year, Value = 500000}
                    },
                CriterionLibraryBudgetJoin = new CriterionLibraryBudgetEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(), MergedCriteriaExpression = "expression", Name = "Criterion"
                    }
                }
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testBudget);


            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateScenarioTestData()
        {
            _testInvestmentPlan = new InvestmentPlanEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = _testHelper.TestSimulation.Id,
                FirstYearOfAnalysisPeriod = DateTime.Now.Year,
                NumberOfYearsInAnalysisPeriod = 1,
                MinimumProjectCostLimit = 500000,
                InflationRatePercentage = 3
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testInvestmentPlan);


            _testScenarioBudget = new ScenarioBudgetEntity
            {
                Id = Guid.NewGuid(),
                Name = "Budget",
                SimulationId = _testHelper.TestSimulation.Id,
                ScenarioBudgetAmounts =
                    new List<ScenarioBudgetAmountEntity>
                    {
                        new ScenarioBudgetAmountEntity
                        {
                            Id = Guid.NewGuid(), Year = DateTime.Now.Year, Value = 500000
                        }
                    },
                CriterionLibraryScenarioBudgetJoin = new CriterionLibraryScenarioBudgetEntity
                {
                    CriterionLibrary = new CriterionLibraryEntity
                    {
                        Id = Guid.NewGuid(), MergedCriteriaExpression = "expression", Name = "Criterion"
                    }
                }
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testScenarioBudget);


            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateRequestWithLibraryFormData(bool overwriteBudgets = false)
        {
            var httpContext = new DefaultHttpContext();
            _testHelper.AddAuthorizationHeader(httpContext);
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestInvestmentBudgets.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var formFile = new FormFile(memStream, 0, memStream.Length, null, "TestInvestmentBudgets.xlsx");

            var formData = new Dictionary<string, StringValues>()
            {
                {"overwriteBudgets", overwriteBudgets ? new StringValues("1") : new StringValues("0")},
                {"libraryId", new StringValues(_testBudgetLibrary.Id.ToString())},
            };

            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection {formFile});
            _testHelper.MockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
        }

        private void CreateRequestWithScenarioFormData(bool overwriteBudgets = false)
        {
            var httpContext = new DefaultHttpContext();
            _testHelper.AddAuthorizationHeader(httpContext);
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestInvestmentBudgets.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var formFile = new FormFile(memStream, 0, memStream.Length, null, "TestInvestmentBudgets.xlsx");

            var formData = new Dictionary<string, StringValues>()
            {
                {"overwriteBudgets", overwriteBudgets ? new StringValues("1") : new StringValues("0")},
                {"simulationId", new StringValues(_testHelper.TestSimulation.Id.ToString())}
            };

            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection {formFile});
            _testHelper.MockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
        }

        private void CreateRequestForExceptionTesting(FormFile file = null)
        {
            var httpContext = new DefaultHttpContext();

            FormFileCollection formFileCollection;
            if (file != null)
            {
                formFileCollection = new FormFileCollection {file};
            }
            else
            {
                formFileCollection = new FormFileCollection();
            }

            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);
            _testHelper.MockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
        }

        [Fact]
        public async void ShouldReturnOkResultOnLibraryGet()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();

                // Act
                var result = await _controller.GetBudgetLibraries();

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
        public async void ShouldReturnOkResultOnScenarioGet()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();

                // Act
                var result = await _controller.GetInvestment(_testHelper.TestSimulation.Id);

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
        public async void ShouldReturnOkResultOnLibraryPost()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                var dto = new BudgetLibraryDTO {Id = Guid.NewGuid(), Name = "", Budgets = new List<BudgetDTO>()};

                // Act
                var result = await _controller.UpsertBudgetLibrary(dto);

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
        public async void ShouldReturnOkResultOnScenarioPost()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                var dto = new InvestmentDTO
                {
                    ScenarioBudgets = new List<BudgetDTO>(), InvestmentPlan = new InvestmentPlanDTO()
                };

                // Act
                var result = await _controller.UpsertInvestment(_testHelper.TestSimulation.Id, dto);

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
                CreateAuthorizedController();

                // Act
                var result = await _controller.DeleteBudgetLibrary(Guid.Empty);

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
        public async void ShouldGetLibraryData()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateLibraryTestData();

                // Act
                var result = await _controller.GetBudgetLibraries();

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dtos = (List<BudgetLibraryDTO>)Convert.ChangeType(okObjResult.Value,
                    typeof(List<BudgetLibraryDTO>));
                Assert.Single(dtos);
                Assert.Equal(_testBudgetLibrary.Id, dtos[0].Id);

                Assert.Single(dtos[0].Budgets);
                Assert.Equal(_testBudget.Id, dtos[0].Budgets[0].Id);
                Assert.Single(dtos[0].Budgets[0].BudgetAmounts);

                var budgetAmount = _testBudget.BudgetAmounts.ToList()[0];
                var dtoBudgetAmount = dtos[0].Budgets[0].BudgetAmounts[0];
                Assert.Equal(budgetAmount.Id, dtoBudgetAmount.Id);
                Assert.Equal(budgetAmount.Year, dtoBudgetAmount.Year);
                Assert.Equal(budgetAmount.Value, dtoBudgetAmount.Value);

                Assert.Equal(_testBudget.CriterionLibraryBudgetJoin.CriterionLibraryId,
                    dtos[0].Budgets[0].CriterionLibrary.Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetInvestmentData()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateScenarioTestData();

                // Act
                var result = await _controller.GetInvestment(_testHelper.TestSimulation.Id);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dto = (InvestmentDTO)Convert.ChangeType(okObjResult.Value, typeof(InvestmentDTO));
                Assert.Single(dto.ScenarioBudgets);
                Assert.Equal(_testInvestmentPlan.Id, dto.InvestmentPlan.Id);
                Assert.Equal(_testInvestmentPlan.FirstYearOfAnalysisPeriod,
                    dto.InvestmentPlan.FirstYearOfAnalysisPeriod);
                Assert.Equal(_testInvestmentPlan.MinimumProjectCostLimit, dto.InvestmentPlan.MinimumProjectCostLimit);
                Assert.Equal(_testInvestmentPlan.NumberOfYearsInAnalysisPeriod,
                    dto.InvestmentPlan.NumberOfYearsInAnalysisPeriod);

                Assert.Single(dto.ScenarioBudgets[0].BudgetAmounts);
                var budgetAmount = _testScenarioBudget.ScenarioBudgetAmounts.ToList()[0];
                var dtoBudgetAmount = dto.ScenarioBudgets[0].BudgetAmounts[0];
                Assert.Equal(budgetAmount.Id, dtoBudgetAmount.Id);
                Assert.Equal(budgetAmount.Year, dtoBudgetAmount.Year);
                Assert.Equal(budgetAmount.Value, dtoBudgetAmount.Value);

                Assert.Equal(_testScenarioBudget.CriterionLibraryScenarioBudgetJoin.CriterionLibraryId,
                    dto.ScenarioBudgets[0].CriterionLibrary.Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyLibraryData()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateLibraryTestData();

                _testBudgetLibrary.Budgets = new List<BudgetEntity> {_testBudget};
                var dto = _testBudgetLibrary.ToDto();
                dto.Description = "Updated Description";
                dto.Budgets[0].Name = "Updated Name";
                dto.Budgets[0].BudgetAmounts[0].Value = 1000000;
                dto.Budgets[0].CriterionLibrary = new CriterionLibraryDTO();

                // Act
                await _controller.UpsertBudgetLibrary(dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedDto = _testHelper.UnitOfWork.BudgetRepo.GetBudgetLibraries()[0];

                    Assert.Equal(dto.Description, modifiedDto.Description);

                    Assert.Equal(dto.Budgets[0].Name, modifiedDto.Budgets[0].Name);
                    Assert.Equal(dto.Budgets[0].CriterionLibrary.Id,
                        modifiedDto.Budgets[0].CriterionLibrary.Id);

                    Assert.Single(modifiedDto.Budgets[0].BudgetAmounts);
                    Assert.Equal(dto.Budgets[0].BudgetAmounts[0].Value,
                        modifiedDto.Budgets[0].BudgetAmounts[0].Value);
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldModifyInvestmentData()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateScenarioTestData();

                var dto = new InvestmentDTO
                {
                    ScenarioBudgets = new List<BudgetDTO> {_testScenarioBudget.ToDto()},
                    InvestmentPlan = _testInvestmentPlan.ToDto()
                };
                dto.ScenarioBudgets[0].Name = "Updated Name";
                dto.ScenarioBudgets[0].BudgetAmounts[0].Value = 1000000;
                dto.ScenarioBudgets[0].CriterionLibrary = new CriterionLibraryDTO();
                dto.InvestmentPlan.MinimumProjectCostLimit = 1000000;

                // Act
                await _controller.UpsertInvestment(_testHelper.TestSimulation.Id, dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedBudgetDto =
                        _testHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(_testHelper.TestSimulation.Id)[0];
                    var modifiedInvestmentPlanDto =
                        _testHelper.UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(_testHelper.TestSimulation.Id);

                    Assert.Equal(dto.ScenarioBudgets[0].Name, modifiedBudgetDto.Name);
                    Assert.Equal(dto.ScenarioBudgets[0].CriterionLibrary.Id,
                        modifiedBudgetDto.CriterionLibrary.Id);

                    Assert.Single(modifiedBudgetDto.BudgetAmounts);
                    Assert.Equal(dto.ScenarioBudgets[0].BudgetAmounts[0].Value,
                        modifiedBudgetDto.BudgetAmounts[0].Value);

                    Assert.Equal(dto.InvestmentPlan.MinimumProjectCostLimit,
                        modifiedInvestmentPlanDto.MinimumProjectCostLimit);
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
                CreateAuthorizedController();
                CreateLibraryTestData();

                // Act
                var result = await _controller.DeleteBudgetLibrary(_testBudgetLibrary.Id);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(!_testHelper.UnitOfWork.Context.BudgetLibrary.Any(_ => _.Id == _testBudgetLibrary.Id));
                Assert.True(!_testHelper.UnitOfWork.Context.Budget.Any(_ => _.Id == _testBudget.Id));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryBudget.Any(_ =>
                        _.BudgetId == _testBudget.Id));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.BudgetAmount.Any(_ =>
                        _.Id == _testBudget.BudgetAmounts.ToList()[0].Id));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowUnauthorizedOnInvestmentPost()
        {
            try
            {
                // Arrange
                CreateUnauthorizedController();
                CreateScenarioTestData();
                var dto = new InvestmentDTO
                {
                    ScenarioBudgets = new List<BudgetDTO> {_testScenarioBudget.ToDto()},
                    InvestmentPlan = _testInvestmentPlan.ToDto()
                };

                // Act
                var result = await _controller.UpsertInvestment(_testHelper.TestSimulation.Id, dto);

                // Assert
                Assert.IsType<UnauthorizedResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        /**************************INVESTMENT BUDGETS EXCEL FILE IMPORT/EXPORT TESTS***********************************/
        [Fact]
        public async void ShouldReturnUnauthorizedOnScenarioImport()
        {
            try
            {
                // Arrange
                CreateUnauthorizedController();
                CreateScenarioTestData();
                CreateRequestWithScenarioFormData();

                // Act
                var result = await _controller.ImportScenarioInvestmentBudgetsExcelFile();

                // Assert
                Assert.IsType<UnauthorizedResult>(result);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldImportLibraryBudgetsFromFile()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateLibraryTestData();
                CreateRequestWithLibraryFormData();

                // Act
                await _controller.ImportLibraryInvestmentBudgetsExcelFile();

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var budgetAmounts = _testHelper.UnitOfWork.BudgetAmountRepo
                        .GetLibraryBudgetAmounts(_testBudgetLibrary.Id)
                        .Where(_ => _.Budget.Name.IndexOf("Sample") != -1)
                        .ToList();

                    Assert.Equal(2, budgetAmounts.Count);
                    Assert.True(budgetAmounts.All(_ => _.Year == 2021));
                    Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

                    var budgets = _testHelper.UnitOfWork.BudgetRepo.GetLibraryBudgets(_testBudgetLibrary.Id);
                    Assert.Equal(3, budgets.Count);
                    Assert.True(budgets.Any(_ => _.Name == "Sample Budget 1"));
                    Assert.True(budgets.Any(_ => _.Name == "Sample Budget 2"));
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldOverwriteExistingLibraryBudgetWithBudgetFromImportedInvestmentBudgetsFile()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateLibraryTestData();
                CreateRequestWithLibraryFormData();

                _testBudget.Name = "Sample Budget 1";
                _testHelper.UnitOfWork.Context.UpdateEntity(_testBudget, _testBudget.Id);

                var budgetAmount = _testBudget.BudgetAmounts.ToList()[0];
                budgetAmount.Year = 2021;
                budgetAmount.Value = 4000000;
                _testHelper.UnitOfWork.Context.UpdateEntity(budgetAmount, budgetAmount.Id);

                // Act
                await _controller.ImportLibraryInvestmentBudgetsExcelFile();

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var budgetAmounts =
                        _testHelper.UnitOfWork.BudgetAmountRepo.GetLibraryBudgetAmounts(_testBudgetLibrary.Id);
                    Assert.Equal(2, budgetAmounts.Count);
                    Assert.True(budgetAmounts.All(_ => _.Year == 2021));
                    Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

                    var budgets = _testHelper.UnitOfWork.BudgetRepo.GetLibraryBudgets(_testBudgetLibrary.Id);
                    Assert.Equal(2, budgets.Count);
                    Assert.True(budgets.Any(_ => _.Name == "Sample Budget 1"));
                    Assert.True(budgets.Any(_ => _.Name == "Sample Budget 2"));
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldExportSampleLibraryBudgetsFile()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateLibraryTestData();
                CreateRequestWithLibraryFormData();
                _testHelper.UnitOfWork.Context.DeleteAll<BudgetAmountEntity>(_ => _.BudgetId == _testBudget.Id);

                // Act
                var result =
                    await _controller.ExportLibraryInvestmentBudgetsExcelFile(_testBudgetLibrary.Id);

                // Assert
                Assert.IsType<OkObjectResult>(result);

                var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
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

                var currentYear = DateTime.Now.Year;
                worksheet.Cells[2, 1, worksheet.Dimension.End.Row, 1]
                    .Select(cell => cell.GetValue<int>()).ToList().ForEach(year =>
                    {
                        Assert.Equal(currentYear, year);
                        currentYear++;
                    });

                var budgetAmounts = worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column]
                    .Select(cell => cell.GetValue<decimal>()).ToList();
                Assert.True(budgetAmounts.All(amount => amount == decimal.Parse("5000000")));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldExportLibraryBudgetsFile()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateLibraryTestData();
                CreateRequestWithLibraryFormData();

                // Act
                var result =
                    await _controller.ExportLibraryInvestmentBudgetsExcelFile(_testBudgetLibrary.Id);

                // Assert
                Assert.IsType<OkObjectResult>(result);

                var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
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
                Assert.Equal(1, worksheetBudgetNames.Count);
                Assert.Equal(_testBudget.Name, worksheetBudgetNames[0]);

                var worksheetBudgetYearAndAmount = worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]
                    .Select(cell => cell.GetValue<string>()).ToList();
                Assert.Equal(DateTime.Now.Year.ToString(), worksheetBudgetYearAndAmount[0]);
                Assert.Equal(_testBudget.BudgetAmounts.ToList()[0].Value.ToString(), worksheetBudgetYearAndAmount[1]);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowConstraintWhenNoMimeTypeForLibraryImport()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportLibraryInvestmentBudgetsExcelFile());
                Assert.Equal("Request MIME type is invalid.", exception.Message);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowConstraintWhenNoFilesForLibraryImport()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateRequestForExceptionTesting();

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportLibraryInvestmentBudgetsExcelFile());
                Assert.Equal("Investment budgets file not found.", exception.Message);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowConstraintWhenNoBudgetLibraryIdFoundForImport()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                    "dummy.txt");
                CreateRequestForExceptionTesting(file);

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportLibraryInvestmentBudgetsExcelFile());
                Assert.Equal("Request contained no budget library id.", exception.Message);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldImportScenarioBudgetsFromFile()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateScenarioTestData();
                CreateRequestWithScenarioFormData();

                // Act
                await _controller.ImportScenarioInvestmentBudgetsExcelFile();

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var budgetAmounts = _testHelper.UnitOfWork.BudgetAmountRepo
                        .GetScenarioBudgetAmounts(_testHelper.TestSimulation.Id)
                        .Where(_ => _.ScenarioBudget.Name.IndexOf("Sample") != -1)
                        .ToList();

                    Assert.Equal(2, budgetAmounts.Count);
                    Assert.True(budgetAmounts.All(_ => _.Year == 2021));
                    Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

                    var budgets = _testHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(_testHelper.TestSimulation.Id);
                    Assert.Equal(3, budgets.Count);
                    Assert.True(budgets.Any(_ => _.Name == "Sample Budget 1"));
                    Assert.True(budgets.Any(_ => _.Name == "Sample Budget 2"));
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldOverwriteExistingScenarioBudgetWithBudgetFromImportedInvestmentBudgetsFile()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateScenarioTestData();
                CreateRequestWithScenarioFormData();

                _testScenarioBudget.Name = "Sample Budget 1";
                _testHelper.UnitOfWork.Context.UpdateEntity(_testScenarioBudget, _testScenarioBudget.Id);

                var budgetAmount = _testScenarioBudget.ScenarioBudgetAmounts.ToList()[0];
                budgetAmount.Year = 2021;
                budgetAmount.Value = 4000000;
                _testHelper.UnitOfWork.Context.UpdateEntity(budgetAmount, budgetAmount.Id);

                // Act
                await _controller.ImportScenarioInvestmentBudgetsExcelFile();

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var budgetAmounts =
                        _testHelper.UnitOfWork.BudgetAmountRepo.GetScenarioBudgetAmounts(_testHelper.TestSimulation.Id);
                    Assert.Equal(2, budgetAmounts.Count);
                    Assert.True(budgetAmounts.All(_ => _.Year == 2021));
                    Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

                    var budgets = _testHelper.UnitOfWork.BudgetRepo.GetScenarioBudgets(_testHelper.TestSimulation.Id);
                    Assert.Equal(2, budgets.Count);
                    Assert.True(budgets.Any(_ => _.Name == "Sample Budget 1"));
                    Assert.True(budgets.Any(_ => _.Name == "Sample Budget 2"));
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldExportSampleScenarioBudgetsFile()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateScenarioTestData();
                CreateRequestWithScenarioFormData();
                _testHelper.UnitOfWork.Context.DeleteAll<ScenarioBudgetAmountEntity>(_ => _.ScenarioBudgetId == _testScenarioBudget.Id);

                // Act
                var result =
                    await _controller.ExportScenarioInvestmentBudgetsExcelFile(_testHelper.TestSimulation.Id);

                // Assert
                Assert.IsType<OkObjectResult>(result);

                var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
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

                var currentYear = DateTime.Now.Year;
                worksheet.Cells[2, 1, worksheet.Dimension.End.Row, 1]
                    .Select(cell => cell.GetValue<int>()).ToList().ForEach(year =>
                    {
                        Assert.Equal(currentYear, year);
                        currentYear++;
                    });

                var budgetAmounts = worksheet.Cells[2, 2, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column]
                    .Select(cell => cell.GetValue<decimal>()).ToList();
                Assert.True(budgetAmounts.All(amount => amount == decimal.Parse("5000000")));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldExportScenarioBudgetsFile()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateScenarioTestData();
                CreateRequestWithScenarioFormData();

                // Act
                var result =
                    await _controller.ExportScenarioInvestmentBudgetsExcelFile(_testHelper.TestSimulation.Id);

                // Assert
                Assert.IsType<OkObjectResult>(result);

                var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
                Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
                Assert.Equal("Test_Simulation_investment_budgets.xlsx", fileInfo.FileName);

                var file = Convert.FromBase64String(fileInfo.FileData);
                var memStream = new MemoryStream();
                memStream.Write(file, 0, file.Length);
                memStream.Seek(0, SeekOrigin.Begin);

                var excelPackage = new ExcelPackage(memStream);
                var worksheet = excelPackage.Workbook.Worksheets[0];

                var worksheetBudgetNames = worksheet.Cells[1, 2, 1, worksheet.Dimension.End.Column]
                    .Select(cell => cell.GetValue<string>()).ToList();
                Assert.Equal(1, worksheetBudgetNames.Count);
                Assert.Equal(_testScenarioBudget.Name, worksheetBudgetNames[0]);

                var worksheetBudgetYearAndAmount = worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]
                    .Select(cell => cell.GetValue<string>()).ToList();
                Assert.Equal(DateTime.Now.Year.ToString(), worksheetBudgetYearAndAmount[0]);
                Assert.Equal(_testScenarioBudget.ScenarioBudgetAmounts.ToList()[0].Value.ToString(), worksheetBudgetYearAndAmount[1]);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowConstraintWhenNoMimeTypeForScenarioImport()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportScenarioInvestmentBudgetsExcelFile());
                Assert.Equal("Request MIME type is invalid.", exception.Message);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowConstraintWhenNoFilesForScenarioImport()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                CreateRequestForExceptionTesting();

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportScenarioInvestmentBudgetsExcelFile());
                Assert.Equal("Investment budgets file not found.", exception.Message);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowConstraintWhenNoBudgetSimulationIdFoundForImport()
        {
            try
            {
                // Arrange
                CreateAuthorizedController();
                var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                    "dummy.txt");
                CreateRequestForExceptionTesting(file);

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportScenarioInvestmentBudgetsExcelFile());
                Assert.Equal("Request contained no simulation id.", exception.Message);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
