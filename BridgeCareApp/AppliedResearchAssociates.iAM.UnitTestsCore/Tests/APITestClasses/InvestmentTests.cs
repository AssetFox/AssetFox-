using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
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

        private static readonly Guid BudgetLibraryId = Guid.Parse("a7035a0c-5436-4b16-ada2-063590d94994");
        private static readonly Guid BudgetId = Guid.Parse("0d93abe9-1fd8-4304-badb-e982f8c376da");
        private static readonly Guid BudgetAmountId = Guid.Parse("40f7215a-4024-4ef2-91f9-23e583cf640b");
        private static readonly Guid InvestmentPlanId = Guid.Parse("0f9ed186-e62b-48b2-880d-85618740096b");

        public InvestmentTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _service = new InvestmentBudgetsService(_testHelper.UnitOfWork);
            _controller = new InvestmentController(_service, _testHelper.MockEsecSecurityAuthorized.Object, _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object, _testHelper.MockHttpContextAccessor.Object);
        }

        private BudgetLibraryEntity TestBudgetLibrary { get; } = new BudgetLibraryEntity
        {
            Id = BudgetLibraryId,
            Name = "Test Name"
        };

        private BudgetEntity TestBudget { get; } = new BudgetEntity
        {
            Id = BudgetId,
            BudgetLibraryId = BudgetLibraryId,
            Name = "Test Name"
        };

        private BudgetAmountEntity TestBudgetAmount { get; } = new BudgetAmountEntity
        {
            Id = BudgetAmountId,
            BudgetId = BudgetId,
            Year = DateTime.Now.Year,
            Value = 500000
        };

        private InvestmentPlanEntity TestInvestmentPlan { get; } = new InvestmentPlanEntity
        {
            Id = InvestmentPlanId,
            FirstYearOfAnalysisPeriod = DateTime.Now.Year,
            InflationRatePercentage = 1,
            MinimumProjectCostLimit = 500000,
            NumberOfYearsInAnalysisPeriod = 1
        };

        private void SetupForGet()
        {
            _testHelper.UnitOfWork.Context.BudgetLibrary.Add(TestBudgetLibrary);
            _testHelper.UnitOfWork.Context.Budget.Add(TestBudget);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForGetAll()
        {
            SetupForGet();
            TestInvestmentPlan.SimulationId = _testHelper.TestSimulation.Id;
            _testHelper.UnitOfWork.Context.InvestmentPlan.Add(TestInvestmentPlan);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void SetupForUpsertOrDelete()
        {
            SetupForGetAll();
            _testHelper.UnitOfWork.Context.CriterionLibrary.Add(_testHelper.TestCriterionLibrary);
            _testHelper.UnitOfWork.Context.SaveChanges();
        }

        private void CreateRequestWithFormData(bool overwriteBudgets = false)
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
                {"libraryId", new StringValues(BudgetLibraryId.ToString())},
                {"simulationId", new StringValues(_testHelper.TestSimulation.Id.ToString())}
            };

            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection {formFile});
            _testHelper.MockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
        }

        private void SetupForImport(bool overwriteBudgets = false)
        {
            SetupForGet();
            _testHelper.UnitOfWork.Context.BudgetAmount.Add(TestBudgetAmount);
            _testHelper.UnitOfWork.Context.BudgetLibrarySimulation.Add(new BudgetLibrarySimulationEntity
            {
                SimulationId = _testHelper.TestSimulation.Id, BudgetLibraryId = BudgetLibraryId
            });
            _testHelper.UnitOfWork.Context.SaveChanges();
            CreateRequestWithFormData(overwriteBudgets);
        }

        private void SetupForExport()
        {
            SetupForGet();
            _testHelper.UnitOfWork.Context.BudgetAmount.Add(TestBudgetAmount);
            _testHelper.UnitOfWork.Context.SaveChanges();
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
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Act
                var result = await _controller.GetInvestment(Guid.Empty);

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
                    .UpsertInvestment(Guid.Empty,
                        new UpsertInvestmentDataDTO
                        {
                            BudgetLibrary = TestBudgetLibrary.ToDto(),
                            InvestmentPlan = new InvestmentPlanDTO()
                        });

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
        public async void ShouldGetInvestmentData()
        {
            try
            {
                // Arrange
                SetupForGet();

                // Act
                var result = await _controller.GetInvestment(Guid.Empty);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dto = (InvestmentDTO)Convert.ChangeType(okObjResult.Value, typeof(InvestmentDTO));
                Assert.Single(dto.BudgetLibraries);
                Assert.Equal(Guid.Empty, dto.InvestmentPlan.Id);

                Assert.Equal(BudgetLibraryId, dto.BudgetLibraries[0].Id);
                Assert.Single(dto.BudgetLibraries[0].Budgets);

                Assert.Equal(BudgetId, dto.BudgetLibraries[0].Budgets[0].Id);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldGetAllInvestmentData()
        {
            try
            {
                // Arrange
                SetupForGetAll();

                // Act
                var result = await _controller.GetInvestment(_testHelper.TestSimulation.Id);

                // Assert
                var okObjResult = result as OkObjectResult;
                Assert.NotNull(okObjResult.Value);

                var dto = (InvestmentDTO)Convert.ChangeType(okObjResult.Value, typeof(InvestmentDTO));
                Assert.Single(dto.BudgetLibraries);
                Assert.Equal(InvestmentPlanId, dto.InvestmentPlan.Id);

                Assert.Equal(BudgetLibraryId, dto.BudgetLibraries[0].Id);
                Assert.Single(dto.BudgetLibraries[0].Budgets);

                Assert.Equal(BudgetId, dto.BudgetLibraries[0].Budgets[0].Id);
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
                SetupForUpsertOrDelete();
                var getResult = await _controller.GetInvestment(_testHelper.TestSimulation.Id);
                var investmentDto = (InvestmentDTO)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(InvestmentDTO));

                var dto = new UpsertInvestmentDataDTO
                {
                    BudgetLibrary = investmentDto.BudgetLibraries[0],
                    InvestmentPlan = investmentDto.InvestmentPlan
                };
                dto.BudgetLibrary.Description = "Updated Description";
                dto.BudgetLibrary.Budgets[0].Name = "Updated Name";
                dto.BudgetLibrary.Budgets[0].BudgetAmounts
                    .Add(TestBudgetAmount.ToDto(dto.BudgetLibrary.Budgets[0].Name));
                dto.BudgetLibrary.Budgets[0].BudgetAmounts[0].Value = 1000000;
                dto.BudgetLibrary.Budgets[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();
                dto.InvestmentPlan.MinimumProjectCostLimit = 1000000;

                // Act
                await _controller.UpsertInvestment(_testHelper.TestSimulation.Id, dto);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var modifiedBudgetLibraryDto = _testHelper.UnitOfWork.BudgetRepo.BudgetLibrariesWithBudgets()[0];
                    var modifiedInvestmentPlanDto =
                        _testHelper.UnitOfWork.InvestmentPlanRepo.ScenarioInvestmentPlan(_testHelper.TestSimulation.Id);

                    Assert.Equal(dto.BudgetLibrary.Description, modifiedBudgetLibraryDto.Description);
                    Assert.Single(modifiedBudgetLibraryDto.AppliedScenarioIds);
                    Assert.Equal(_testHelper.TestSimulation.Id, modifiedBudgetLibraryDto.AppliedScenarioIds[0]);

                    Assert.Equal(dto.BudgetLibrary.Budgets[0].Name, modifiedBudgetLibraryDto.Budgets[0].Name);
                    Assert.Equal(dto.BudgetLibrary.Budgets[0].CriterionLibrary.Id,
                        modifiedBudgetLibraryDto.Budgets[0].CriterionLibrary.Id);

                    Assert.True(modifiedBudgetLibraryDto.Budgets[0].BudgetAmounts.Any());
                    Assert.Equal(dto.BudgetLibrary.Budgets[0].BudgetAmounts[0].Value,
                        modifiedBudgetLibraryDto.Budgets[0].BudgetAmounts[0].Value);

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
                SetupForUpsertOrDelete();
                var getResult = await _controller.GetInvestment(_testHelper.TestSimulation.Id);
                var dto = (InvestmentDTO)Convert.ChangeType((getResult as OkObjectResult).Value, typeof(InvestmentDTO));

                var addOrUpdateInvestmentDTO = new UpsertInvestmentDataDTO
                {
                    BudgetLibrary = dto.BudgetLibraries[0],
                    InvestmentPlan = dto.InvestmentPlan
                };
                addOrUpdateInvestmentDTO.BudgetLibrary.Budgets[0].BudgetAmounts
                    .Add(TestBudgetAmount.ToDto("Test Name"));
                addOrUpdateInvestmentDTO.BudgetLibrary.Budgets[0].CriterionLibrary =
                    _testHelper.TestCriterionLibrary.ToDto();

                await _controller.UpsertInvestment(_testHelper.TestSimulation.Id, addOrUpdateInvestmentDTO);

                // Act
                var result = await _controller.DeleteBudgetLibrary(BudgetLibraryId);

                // Assert
                Assert.IsType<OkResult>(result);

                Assert.True(!_testHelper.UnitOfWork.Context.BudgetLibrary.Any(_ => _.Id == BudgetLibraryId));
                Assert.True(!_testHelper.UnitOfWork.Context.Budget.Any(_ => _.Id == BudgetId));
                Assert.True(!_testHelper.UnitOfWork.Context.BudgetLibrarySimulation.Any(_ =>
                    _.BudgetLibraryId == BudgetLibraryId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.CriterionLibraryBudget.Any(_ =>
                        _.BudgetId == BudgetId));
                Assert.True(
                    !_testHelper.UnitOfWork.Context.BudgetAmount.Any(_ => _.Id == BudgetAmountId));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        /**************************INVESTMENT BUDGETS EXCEL FILE IMPORT/EXPORT TESTS***********************************/
        [Fact]
        public async void ShouldReturnUnauthorizedOnGet()
        {
            try
            {
                // Arrange
                _testHelper.SetupDefaultHttpContext();
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityNotAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);

                // Act
                var result = await _controller.ExportInvestmentBudgetsExcelFile(BudgetLibraryId, _testHelper.TestSimulation.Id);

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
        public async void ShouldReturnUnauthorizedOnPost()
        {
            try
            {
                // Arrange
                SetupForImport();
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityNotAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);

                // Act
                var result = await _controller.ImportInvestmentBudgetsExcelFile();

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
        public async void ShouldImportInvestmentBudgetsFromFile()
        {
            try
            {
                // Arrange
                SetupForGet();
                CreateRequestWithFormData();
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);


                // Act
                await _controller.ImportInvestmentBudgetsExcelFile();

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var budgetAmounts =
                        _testHelper.UnitOfWork.BudgetAmountRepo.GetBudgetAmountsByBudgetLibraryId(BudgetLibraryId);
                    Assert.Equal(2, budgetAmounts.Count);
                    Assert.True(budgetAmounts.All(_ => _.Year == 2021));
                    Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

                    var budgets = _testHelper.UnitOfWork.BudgetRepo.GetBudgetsWithBudgetAmounts(BudgetLibraryId);
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
        public async void ShouldOverwriteExistingBudgetWithBudgetFromImportedInvestmentBudgetsFile()
        {
            try
            {
                // Arrange
                SetupForGet();
                CreateRequestWithFormData();
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);

                var existingBudget = new BudgetEntity
                {
                    Id = Guid.NewGuid(), Name = "Sample Budget 1", BudgetLibraryId = BudgetLibraryId
                };
                _testHelper.UnitOfWork.Context.Budget.Add(existingBudget);
                _testHelper.UnitOfWork.Context.SaveChanges();
                var existingBudgetAmount = new BudgetAmountEntity
                {
                    Id = Guid.NewGuid(), Year = 2021, Value = 4000000, BudgetId = existingBudget.Id
                };
                _testHelper.UnitOfWork.Context.BudgetAmount.Add(existingBudgetAmount);
                _testHelper.UnitOfWork.Context.SaveChanges();

                // Act
                await _controller.ImportInvestmentBudgetsExcelFile();

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var budgetAmounts =
                        _testHelper.UnitOfWork.BudgetAmountRepo.GetBudgetAmountsByBudgetLibraryId(BudgetLibraryId);
                    Assert.Equal(2, budgetAmounts.Count);
                    Assert.True(budgetAmounts.All(_ => _.Year == 2021));
                    Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

                    var budgets = _testHelper.UnitOfWork.BudgetRepo.GetBudgetsWithBudgetAmounts(BudgetLibraryId);
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
        public async void ShouldExportSampleInvestmentBudgetsFile()
        {
            try
            {
                // Arrange
                SetupForGet();
                _testHelper.SetupDefaultHttpContext();
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);

                // Act
                var result = await _controller.ExportInvestmentBudgetsExcelFile(BudgetLibraryId, _testHelper.TestSimulation.Id);

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
        public async void ShouldExportInvestmentBudgetsFile()
        {
            try
            {
                // Arrange
                SetupForExport();
                _testHelper.SetupDefaultHttpContext();
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);

                // Act
                var result = await _controller.ExportInvestmentBudgetsExcelFile(BudgetLibraryId, _testHelper.TestSimulation.Id);

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
                Assert.Equal("Test Name", worksheetBudgetNames[0]);

                var worksheetBudgetYearAndAmount = worksheet.Cells[2, 1, 2, worksheet.Dimension.End.Column]
                    .Select(cell => cell.GetValue<string>()).ToList();
                Assert.Equal(DateTime.Now.Year.ToString(), worksheetBudgetYearAndAmount[0]);
                Assert.Equal("500000", worksheetBudgetYearAndAmount[1]);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldOverwriteExistingInvestmentBudgets()
        {
            try
            {
                // Arrange
                SetupForImport(true);
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);

                // Act
                await _controller.ImportInvestmentBudgetsExcelFile();

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    Assert.True(_testHelper.UnitOfWork.Context.Budget.Any(_ => _.Id == BudgetId));
                    Assert.Empty(_testHelper.UnitOfWork.Context.BudgetAmount.Where(_ => _.BudgetId == BudgetId).ToList());

                    var budgetAmounts =
                        _testHelper.UnitOfWork.BudgetAmountRepo.GetBudgetAmountsByBudgetLibraryId(BudgetLibraryId);
                    Assert.True(budgetAmounts.All(_ => _.Year == 2021));
                    Assert.True(budgetAmounts.All(_ => _.Value == decimal.Parse("5000000")));

                    var budgets = budgetAmounts.Select(_ => _.Budget.Name).ToList();
                    Assert.Equal(2, budgets.Count);
                    Assert.True(budgets.All(name => name.Contains("Sample Budget")));
                };
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowConstraintWhenNoMimeTypeForImport()
        {
            try
            {
                // Arrange
                _testHelper.SetupDefaultHttpContext();
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportInvestmentBudgetsExcelFile());
                Assert.Equal("Request MIME type is invalid.", exception.Message);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowConstraintWhenNoFilesForImport()
        {
            try
            {
                // Arrange
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);
                CreateRequestForExceptionTesting();

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportInvestmentBudgetsExcelFile());
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
                _controller = new InvestmentController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object,
                    _testHelper.MockHttpContextAccessor.Object);
                var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                    "dummy.txt");
                CreateRequestForExceptionTesting(file);
                /*_controller.ControllerContext.HttpContext.Request.Form =
                    new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection{file});*/

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportInvestmentBudgetsExcelFile());
                Assert.Equal("Request contained no budget library id.", exception.Message);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }
    }
}
