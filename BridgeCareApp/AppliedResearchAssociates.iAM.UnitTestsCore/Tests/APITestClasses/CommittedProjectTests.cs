using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Controllers;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.APITestClasses
{
    public class CommittedProjectTests
    {
        private readonly TestHelper _testHelper;
        private readonly CommittedProjectService _service;
        private CommittedProjectController _controller;

        private CommittedProjectEntity _testProject;

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
            _testHelper = TestHelper.Instance;
            _testHelper.CreateAttributes();
            _testHelper.CreateNetwork();
            _service = new CommittedProjectService(_testHelper.UnitOfWork);
        }

        private InvestmentPlanEntity CreateCommittedProjectTestData(Guid simulationId)
        {
            var entity = new InvestmentPlanEntity
            {
                Id = Guid.NewGuid(),
                FirstYearOfAnalysisPeriod = 2020,
                InflationRatePercentage = 0,
                MinimumProjectCostLimit = 0,
                NumberOfYearsInAnalysisPeriod = 1,
                SimulationId = simulationId
            };
            _testHelper.UnitOfWork.Context.AddEntity(entity);



            var attributeIdsPerAttributeName =
                _testHelper.UnitOfWork.Context.Attribute.Where(_ => ConsequenceAttributeNames.Contains(_.Name))
                    .ToDictionary(_ => _.Name, _ => _.Id);

            _testProject = new CommittedProjectEntity
            {
                Id = Guid.NewGuid(),
                ScenarioBudget =
                    new ScenarioBudgetEntity
                    {
                        Id = Guid.NewGuid(),
                        SimulationId = simulationId,
                        Name = "Test Name"
                    },
                MaintainableAsset =
                    new MaintainableAssetEntity
                    {
                        Id = Guid.NewGuid(),
                        NetworkId = _testHelper.TestNetwork.Id,
                        SpatialWeighting = "[DECK_AREA]",
                        MaintainableAssetLocation = new MaintainableAssetLocationEntity
                        {
                            Id = Guid.NewGuid(),
                            LocationIdentifier = "1-2",
                            Discriminator = "SectionLocation"
                        }
                    },
                Name = "Rehabilitation",
                Year = 2021,
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 2,
                Cost = 250000,
                SimulationId = simulationId,
                CommittedProjectConsequences = ConsequenceAttributeNames.Select(attributeName =>
                    new CommittedProjectConsequenceEntity
                    {
                        Id = Guid.NewGuid(),
                        AttributeId = attributeIdsPerAttributeName[attributeName],
                        ChangeValue = "1"
                    }).ToList()
            };
            _testHelper.UnitOfWork.Context.AddEntity(_testProject);


            _testHelper.UnitOfWork.Context.SaveChanges();
            return entity;
        }

        private void CreateRequestWithFormData(Guid simulationId)
        {
            var httpContext = new DefaultHttpContext();
            _testHelper.AddAuthorizationHeader(httpContext);
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestUtils\\Files",
                "TestCommittedProjects.xlsx");
            using var stream = File.OpenRead(filePath);
            var memStream = new MemoryStream();
            stream.CopyTo(memStream);
            var formFile = new FormFile(memStream, 0, memStream.Length, null, "TestCommittedProjects.xlsx");

            var formData = new Dictionary<string, StringValues>()
            {
                {"applyNoTreatment", new StringValues("0")},
                {"simulationId", new StringValues(simulationId.ToString())}
            };

            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection { formFile });
            _testHelper.MockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
        }

        private void CreateRequestWithFormData(Guid simulationId, FileInfoDTO fileInfo)
        {
            var httpContext = new DefaultHttpContext();
            _testHelper.AddAuthorizationHeader(httpContext);
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");

            var memStream = new MemoryStream(Convert.FromBase64String(fileInfo.FileData));
            var formFile = new FormFile(memStream, 0, memStream.Length, null, fileInfo.FileName);

            var formData = new Dictionary<string, StringValues>()
            {
                {"applyNoTreatment", new StringValues("0")},
                {"simulationId", new StringValues(simulationId.ToString())}
            };

            httpContext.Request.Form = new FormCollection(formData, new FormFileCollection { formFile });
            _testHelper.MockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
        }

        private void CreateRequestForExceptionTesting(FormFile file = null)
        {
            var httpContext = new DefaultHttpContext();

            FormFileCollection formFileCollection;
            if (file != null)
            {
                formFileCollection = new FormFileCollection { file };
            }
            else
            {
                formFileCollection = new FormFileCollection();
            }

            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), formFileCollection);
            _testHelper.MockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);
        }


        private async Task AssertCommittedProjectsData(Guid simulationId)
        {
            var committedProjects = _testHelper.UnitOfWork.Context.CommittedProject
                .Select(project => new CommittedProjectEntity
                {
                    Name = project.Name,
                    SimulationId = project.SimulationId,
                    ScenarioBudgetId = project.ScenarioBudgetId,
                    MaintainableAssetId = project.MaintainableAssetId,
                    Cost = project.Cost,
                    Year = project.Year,
                    ShadowForAnyTreatment = project.ShadowForAnyTreatment,
                    ShadowForSameTreatment = project.ShadowForSameTreatment,
                    CommittedProjectConsequences = project.CommittedProjectConsequences
                        .Select(consequence => new CommittedProjectConsequenceEntity
                        {
                            Attribute = new AttributeEntity { Name = consequence.Attribute.Name },
                            ChangeValue = consequence.ChangeValue
                        }).ToList()
                }).ToList();
            Assert.Single(committedProjects);
            Assert.Equal("Rehabilitation", committedProjects[0].Name);
            Assert.Equal(250000, committedProjects[0].Cost);
            Assert.Equal(2021, committedProjects[0].Year);
            Assert.Equal(1, committedProjects[0].ShadowForAnyTreatment);
            Assert.Equal(2, committedProjects[0].ShadowForSameTreatment);
            Assert.Equal(simulationId, committedProjects[0].SimulationId);

            var consequences = committedProjects[0].CommittedProjectConsequences.ToList();
            Assert.Equal(8, consequences.Count);
            ConsequenceAttributeNames.ForEach(attributeName =>
            {
                var consequence = consequences.SingleOrDefault(_ => _.Attribute.Name == attributeName);
                Assert.NotNull(consequence);
                Assert.Equal("1", consequence.ChangeValue);
            });
        }

        [Fact]
        public async Task ShouldReturnOkResultOnGet()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await _controller.ExportCommittedProjects(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ShouldReturnOkResultOnPost()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            CreateCommittedProjectTestData(simulation.Id);
            CreateRequestWithFormData(simulation.Id);
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await _controller.ImportCommittedProjects();

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact(Skip = "Wj broke this. He needs to fix it. WjTodo.")]
        public async Task ShouldReturnOkResultOnDelete()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await _controller.DeleteCommittedProjects(simulation.Id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedOnGet()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityNotAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await _controller.ExportCommittedProjects(simulation.Id);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact(Skip = "WjTodo WJ broke this. He needs to fix it.")]
        public async Task ShouldReturnUnauthorizedOnPost()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            CreateRequestWithFormData(simulation.Id);
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityNotAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await _controller.ImportCommittedProjects();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact(Skip = "Wjtodo -- Wj broke this. He needs to fix it.")]
        public async Task ShouldReturnUnauthorizedOnDelete()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityNotAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await _controller.DeleteCommittedProjects(simulation.Id);

            // assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ShouldImportCommittedProjectsFromFile()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            CreateRequestWithFormData(simulation.Id);
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);


            // Act
            await _controller.ImportCommittedProjects();

            // Assert
            AssertCommittedProjectsData(simulation.Id);
        }

        public async Task ShouldExportCommittedProjectsToFile()
        {
            var simulation = _testHelper.CreateSimulation();
            // Arrange
            _testHelper.SetupDefaultHttpContext();
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await _controller.ExportCommittedProjects(simulation.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var fileInfo = (FileInfoDTO)Convert.ChangeType((result as OkObjectResult).Value, typeof(FileInfoDTO));
            Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileInfo.MimeType);
            Assert.Equal($"CommittedProjects_{simulation.Name}.xlsx", fileInfo.FileName);

            CreateRequestWithFormData(simulation.Id, fileInfo);
            await _controller.ImportCommittedProjects();

            await AssertCommittedProjectsData(simulation.Id);
        }

        public async Task ShouldDeleteCommittedProjectData()
        {
            // Arrange
            var simulation = _testHelper.CreateSimulation();
            _testHelper.SetupDefaultHttpContext();
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            await _controller.DeleteCommittedProjects(simulation.Id);

            // Assert
            await Task.Delay(5000);
            var committedProjects = _testHelper.UnitOfWork.Context.CommittedProject
                .Where(_ => _.SimulationId == simulation.Id)
                .ToList();
            Assert.Empty(committedProjects);

            var consequences = _testHelper.UnitOfWork.Context.CommittedProjectConsequence
                .Where(_ => _.CommittedProjectId == _testProject.Id)
                .ToList();
            Assert.Empty(consequences);
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoMimeTypeWithBadRequestForImport()
        {
            // Arrange
            _testHelper.SetupDefaultHttpContext();
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act + Asset
            var result = await _controller.ImportCommittedProjects();
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(((BadRequestObjectResult)result).Value, "Committed Project error::Request MIME type is invalid.");
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoFilesWithBadRequestForImport()
        {
            // Arrange
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);
            CreateRequestForExceptionTesting();

            // Act + Asset
            var result = await _controller.ImportCommittedProjects();
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(((BadRequestObjectResult)result).Value, "Committed Project error::Committed project file not found.");
        }

        [Fact]
        public async Task ShouldThrowConstraintWhenNoSimulationIdWithBadRequestForImport()
        {
            // Arrange
            _controller = new CommittedProjectController(_service,
                _testHelper.MockEsecSecurityAuthorized.Object,
                _testHelper.UnitOfWork,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data",
                "dummy.txt");
            CreateRequestForExceptionTesting(file);

            // Act + Asset
            var result = await _controller.ImportCommittedProjects();
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(((BadRequestObjectResult)result).Value, "Committed Project error::Request contained no simulation id.");
        }
    }
}
