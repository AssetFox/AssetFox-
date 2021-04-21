using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Mocks;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using BridgeCareCore.Controllers;
using BridgeCareCore.Models;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.API_Test_Classes
{
    public class CommittedProjectTests
    {
        private readonly TestHelper _testHelper;
        private readonly CommittedProjectService _service;
        private CommittedProjectController _controller;

        private static readonly Guid InvestmentPlanId = Guid.Parse("f6af0c20-da73-4bec-8318-e904c53b4fec");
        private static readonly Guid BudgetLibraryId = Guid.Parse("6e99c853-a881-4953-bc9c-38632949f70c");
        private static readonly Guid BudgetId = Guid.Parse("62cad814-a475-4ee0-8810-09f28bd282a8");
        private static readonly Guid MaintainableAssetId = Guid.Parse("0bd797b0-a104-4fc3-9b16-479f87e89b4a");
        private static readonly Guid MaintainableAssetLocationId = Guid.Parse("74ff59a1-7450-4f61-9930-d687feb13ec7");
        private static readonly Guid CommittedProjectId = Guid.Parse("06d74235-0970-4d7c-82a4-843ff73ad34a");

        private static readonly List<string> ConsequenceAttributeNames = new List<string>
        {
            "DECK_SEEDED",
            "SUB_SEEDED",
            "SUP_SEEDED",
            "CULV_SEEDED",
            "DECK_DURATION_N",
            "SUB_DURATION_N",
            "SUP_DURATION_N",
            "CULV_DURATION_N"
        };

        public CommittedProjectTests()
        {
            _testHelper = new TestHelper();
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _testHelper.CreateSimulation();
            _service = new CommittedProjectService(_testHelper.UnitOfWork);
        }

        private InvestmentPlanEntity TestInvestmentPlan { get; } = new InvestmentPlanEntity
        {
            Id = InvestmentPlanId,
            FirstYearOfAnalysisPeriod = 2020,
            InflationRatePercentage = 0,
            MinimumProjectCostLimit = 0,
            NumberOfYearsInAnalysisPeriod = 1
        };

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

        private MaintainableAssetEntity TestAsset { get; } = new MaintainableAssetEntity
        {
            Id = MaintainableAssetId, SpatialWeighting = "[DECK_AREA]"
        };

        private MaintainableAssetLocationEntity TestAssetLocation { get; } = new MaintainableAssetLocationEntity
        {
            Id = MaintainableAssetLocationId,
            LocationIdentifier = "1-2",
            MaintainableAssetId = MaintainableAssetId,
            Discriminator = "SectionLocation"
        };

        private CommittedProjectEntity TestProject { get; } = new CommittedProjectEntity
        {
            Id = CommittedProjectId,
            BudgetId = BudgetId,
            MaintainableAssetId = MaintainableAssetId,
            Name = "Rehabilitation",
            Year = 2021,
            ShadowForAnyTreatment = 1,
            ShadowForSameTreatment = 2,
            Cost = 250000
        };

        private void AddTestData()
        {
            TestInvestmentPlan.SimulationId = _testHelper.TestSimulation.Id;
            _testHelper.UnitOfWork.Context.AddEntity(TestInvestmentPlan);
            _testHelper.UnitOfWork.Context.AddEntity(TestBudgetLibrary);
            _testHelper.UnitOfWork.Context.AddEntity(TestBudget);
            TestAsset.NetworkId = _testHelper.TestNetwork.Id;
            _testHelper.UnitOfWork.Context.AddEntity(TestAsset);
            _testHelper.UnitOfWork.Context.AddEntity(TestAssetLocation);
        }

        private void SetupForImport()
        {
            AddTestData();
            _testHelper.UnitOfWork.Context.AddEntity(new BudgetLibrarySimulationEntity
            {
                SimulationId = _testHelper.TestSimulation.Id, BudgetLibraryId = BudgetLibraryId
            });
            _controller.ControllerContext = CreateRequestWithFormData();
        }

        private void SetupForExport()
        {
            AddTestData();
            TestProject.SimulationId = _testHelper.TestSimulation.Id;
            _testHelper.UnitOfWork.Context.AddEntity(TestProject);
            var attributeIdsPerAttributeName =
                _testHelper.UnitOfWork.Context.Attribute.Where(_ => ConsequenceAttributeNames.Contains(_.Name))
                    .ToDictionary(_ => _.Name, _ => _.Id);
            var consequences = ConsequenceAttributeNames.Select(attributeName => new CommittedProjectConsequenceEntity
            {
                Id = Guid.NewGuid(),
                CommittedProjectId = CommittedProjectId,
                AttributeId = attributeIdsPerAttributeName[attributeName],
                ChangeValue = "1"
            }).ToList();
            _testHelper.UnitOfWork.Context.AddAll(consequences);
        }

        private ControllerContext CreateDefaultControllerContext()
        {
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());
            return new ControllerContext(actionContext);
        }

        private ControllerContext CreateRequestWithFormData()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestData\\Files",
                "TestCommittedProjects.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var formFile = new FormFile(memStream, 0, memStream.Length, null, "TestCommittedProjects.xlsx");

            var formData = new Dictionary<string, StringValues>()
            {
                {"applyNoTreatment", new StringValues("0")},
                {"simulationId", new StringValues(_testHelper.TestSimulation.Id.ToString())}
            };

            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection {formFile});

            var actionContext = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());

            return new ControllerContext(actionContext);
        }

        private ControllerContext CreateRequestWithFormData(FileInfoDTO fileInfo)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var memStream = new MemoryStream(Convert.FromBase64String(fileInfo.FileData));
            var formFile = new FormFile(memStream, 0, memStream.Length, null, fileInfo.FileName);

            var formData = new Dictionary<string, StringValues>()
            {
                {"applyNoTreatment", new StringValues("0")},
                {"simulationId", new StringValues(_testHelper.TestSimulation.Id.ToString())}
            };

            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection {formFile});

            var actionContext = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());

            return new ControllerContext(actionContext);
        }

        private void AssertCommittedProjectsData()
        {
            var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var committedProjects = _testHelper.UnitOfWork.Context.CommittedProject
                        .Select(project => new CommittedProjectEntity
                        {
                            Name = project.Name,
                            SimulationId = project.SimulationId,
                            BudgetId = project.BudgetId,
                            MaintainableAssetId = project.MaintainableAssetId,
                            Cost = project.Cost,
                            Year = project.Year,
                            ShadowForAnyTreatment = project.ShadowForAnyTreatment,
                            ShadowForSameTreatment = project.ShadowForSameTreatment,
                            CommittedProjectConsequences = project.CommittedProjectConsequences
                                .Select(consequence => new CommittedProjectConsequenceEntity
                                {
                                    Attribute = new AttributeEntity {Name = consequence.Attribute.Name},
                                    ChangeValue = consequence.ChangeValue
                                }).ToList()
                        }).ToList();
                    Assert.Single(committedProjects);
                    Assert.Equal("Rehabilitation", committedProjects[0].Name);
                    Assert.Equal(250000, committedProjects[0].Cost);
                    Assert.Equal(2021, committedProjects[0].Year);
                    Assert.Equal(1, committedProjects[0].ShadowForAnyTreatment);
                    Assert.Equal(2, committedProjects[0].ShadowForSameTreatment);
                    Assert.Equal(_testHelper.TestSimulation.Id, committedProjects[0].SimulationId);
                    Assert.Equal(BudgetId, committedProjects[0].BudgetId);
                    Assert.Equal(MaintainableAssetId, committedProjects[0].MaintainableAssetId);

                    var consequences = committedProjects[0].CommittedProjectConsequences.ToList();
                    Assert.Equal(8, consequences.Count);
                    ConsequenceAttributeNames.ForEach(attributeName =>
                    {
                        var consequence = consequences.SingleOrDefault(_ => _.Attribute.Name == attributeName);
                        Assert.NotNull(consequence);
                        Assert.Equal("1", consequence.ChangeValue);
                    });
                };
        }

        [Fact]
        public async void ShouldReturnOkResultOnGet()
        {
            try
            {
                // Arrange
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                _controller.ControllerContext = CreateDefaultControllerContext();

                // Act
                var result = await _controller.ExportCommittedProjects(_testHelper.TestSimulation.Id);

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
                // Arrange
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                SetupForImport();

                // Act
                var result = await _controller.ImportCommittedProjects();

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
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                _controller.ControllerContext = CreateDefaultControllerContext();

                // Act
                var result = await _controller.DeleteCommittedProjects(_testHelper.TestSimulation.Id);

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
        public async void ShouldThrowNotAuthorizedOnGet()
        {
            try
            {
                // Arrange
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityNotAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                _controller.ControllerContext = CreateDefaultControllerContext();

                // Act + Assert
                await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                    await _controller.ExportCommittedProjects(_testHelper.TestSimulation.Id));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowNotAuthorizedOnPost()
        {
            try
            {
                // Arrange
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityNotAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                SetupForImport();

                // Act + Assert
                await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                    await _controller.ImportCommittedProjects());
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowNotAuthorizedOnDelete()
        {
            try
            {
                // Arrange
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityNotAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                _controller.ControllerContext = CreateDefaultControllerContext();

                // Act + Assert
                await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                    await _controller.DeleteCommittedProjects(_testHelper.TestSimulation.Id));
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldImportCommittedProjectsFromFile()
        {
            try
            {
                // Arrange
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                SetupForImport();

                // Act
                await _controller.ImportCommittedProjects();

                // Assert
                AssertCommittedProjectsData();
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldExportCommittedProjectsToFile()
        {
            try
            {
                // Arrange
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                _controller.ControllerContext = CreateDefaultControllerContext();
                SetupForExport();

                // Act
                var result = await _controller.ExportCommittedProjects(_testHelper.TestSimulation.Id);

                // Assert
                Assert.IsType<OkObjectResult>(result);

                var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
                Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
                Assert.Equal("CommittedProjects_Test_Simulation.xlsx", fileInfo.FileName);

                var timer = new Timer {Interval = 5000};
                timer.Elapsed += async delegate
                {
                    _controller.ControllerContext = CreateRequestWithFormData(fileInfo);
                    await _controller.ImportCommittedProjects();
                };

                AssertCommittedProjectsData();
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldDeleteCommittedProjectData()
        {
            try
            {
                // Arrange
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                _controller.ControllerContext = CreateDefaultControllerContext();
                SetupForExport();

                // Act
                await _controller.DeleteCommittedProjects(_testHelper.TestSimulation.Id);

                // Assert
                var timer = new Timer {Interval = 5000};
                timer.Elapsed += delegate
                {
                    var committedProjects = _testHelper.UnitOfWork.Context.CommittedProject
                        .Where(_ => _.SimulationId == _testHelper.TestSimulation.Id)
                        .ToList();
                    Assert.Empty(committedProjects);

                    var consequences = _testHelper.UnitOfWork.Context.CommittedProjectConsequence
                        .Where(_ => _.CommittedProjectId == CommittedProjectId)
                        .ToList();
                    Assert.Empty(consequences);
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
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                _controller.ControllerContext = CreateDefaultControllerContext();

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportCommittedProjects());
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
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                _controller.ControllerContext = CreateDefaultControllerContext();
                _controller.ControllerContext.HttpContext.Request.Form =
                    new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection());

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportCommittedProjects());
                Assert.Equal("Committed project files were not found.", exception.Message);
            }
            finally
            {
                // Cleanup
                _testHelper.CleanUp();
            }
        }

        [Fact]
        public async void ShouldThrowConstraintWhenNoSimulationIdForImport()
        {
            try
            {
                // Arrange
                _controller = new CommittedProjectController(_service,
                    _testHelper.MockEsecSecurityAuthorized.Object,
                    _testHelper.UnitOfWork,
                    _testHelper.MockHubService.Object);
                _controller.ControllerContext = CreateDefaultControllerContext();
                var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                    "dummy.txt");
                _controller.ControllerContext.HttpContext.Request.Form =
                    new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection{file});

                // Act + Asset
                var exception = await Assert.ThrowsAsync<ConstraintException>(async () =>
                    await _controller.ImportCommittedProjects());
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
