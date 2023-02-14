using System.Data;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using BridgeCareCore.Utils;
using Microsoft.AspNetCore.Authorization;
using BridgeCareCoreTests.Helpers;

namespace BridgeCareCoreTests.Tests
{
    public class DataSourceControllerTests
    {
        private Mock<IUnitOfWork> _mockUOW;
        private Mock<IDataSourceRepository> _mockDataSource;
        private Guid _badSource = Guid.Parse("7ed4d236-7f8c-450a-ae05-9c1359ed0ce6");

        public DataSourceControllerTests()
        {
            _mockUOW = new Mock<IUnitOfWork>();

            var mockUserRepo = UserRepositoryMocks.EveryoneExists();
            _mockUOW.Setup(_ => _.UserRepo).Returns(mockUserRepo.Object);

            _mockDataSource = new Mock<IDataSourceRepository>();
            _mockDataSource.Setup(_ => _.GetDataSources()).Returns(TestDataForDataSources.SourceDTOs());
            _mockDataSource.Setup(_ => _.GetDataSource(It.IsAny<Guid>()))
                .Returns<Guid>(p => TestDataForDataSources.SourceDTOs().Single(ds => ds.Id == p));

            _mockUOW.Setup(_ => _.DataSourceRepo).Returns(_mockDataSource.Object);
        }

        public DataSourceController CreateTestController(List<string> userClaims)
        {
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var testUser = ClaimsPrincipals.WithNameClaims(userClaims);
            var controller = new DataSourceController(
                EsecSecurityMocks.AdminMock.Object,
                _mockUOW.Object,
                hubService,
                accessor);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = testUser }
            };
            return controller;
        }
        [Fact]
        public async Task UpsertSQLWorksWithValidData()
        {
            // Arrange
            var newValue = new SQLDataSourceDTO
            {
                Id = Guid.NewGuid(),
                ConnectionString = "connection",
                Name = "Test"
            };
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.UpsertSqlDataSource(newValue);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockDataSource.Verify(_ => _.UpsertDatasource(It.IsAny<SQLDataSourceDTO>()), Times.Once());
        }

        [Fact]
        public async Task UpsertHandlesRepoFailure()
        {
            // Arrange
            string errorMessage = "Error message thrown by UpsertDatasource";
            var newValue = new SQLDataSourceDTO
            {
                Id = Guid.NewGuid(),
                ConnectionString = "connection",
                Name = "Test"
            };
            _mockDataSource.Setup(_ => _.UpsertDatasource(It.IsAny<BaseDataSourceDTO>()))
                .Throws(new ArgumentException(errorMessage));
            var hubService = HubServiceMocks.Default();
            var accessor = HttpContextAccessorMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentException>(() => controller.UpsertSqlDataSource(newValue));
            Assert.Equal(errorMessage, result.Message);
        }

        [Fact]
        public async Task UpsertExcelWorksWithValidData()
        {
            // Arrange
            var newValue = new ExcelDataSourceDTO
            {
                Id = Guid.NewGuid(),
                Name = "Test", 
                DateColumn = "Inspection Date",
                LocationColumn = "BRKEY"
            };
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.UpsertExcelDataSource(newValue);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockDataSource.Verify(_ => _.UpsertDatasource(It.IsAny<ExcelDataSourceDTO>()), Times.Once());
        }

        [Fact]
        public async Task DeleteWorksWithExistingDataSource()
        {
            // Arrange
            var sourceToDelete = TestDataForDataSources.ExcelDatasourceId;
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.DeleteDataSource(sourceToDelete);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockDataSource.Verify(_ => _.DeleteDataSource(It.IsAny<Guid>()), Times.Once());        }

        [Fact]
        public async Task DeleteHandlesBadDataSourceId()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);
            _mockDataSource.Setup(_ => _.DeleteDataSource(It.IsAny<Guid>())).Throws<RowNotInTableException>();

            // Act & Assert
            await Assert.ThrowsAsync<RowNotInTableException>(() => controller.DeleteDataSource(_badSource));
        }

        [Fact]
        public async Task GetAllDataSourcesReturnsValues()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.GetDataSources();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = result as OkObjectResult;
            Assert.IsType<List<BaseDataSourceDTO>>(objectResult.Value);
            var resultValue = objectResult.Value as List<BaseDataSourceDTO>;
            Assert.Equal(2, resultValue.Count);
            Assert.Contains(resultValue, _ => _.Name == "SQL Server Data Source");
        }

        [Fact]
        public async Task GetAllWorksWithNoDataSources()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);
            _mockDataSource.Setup(_ => _.GetDataSources()).Returns(new List<BaseDataSourceDTO>());

            // Act
            var result = await controller.GetDataSources();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = result as OkObjectResult;
            Assert.IsType<List<BaseDataSourceDTO>>(objectResult.Value);
            var resultValue = objectResult.Value as List<BaseDataSourceDTO>;
            Assert.Empty(resultValue);
        }

        [Fact]
        public async Task GetOneReturnsProperTypes()
        {
            // We can test BOTH Excel and SQL here
            // Arrange
            var sourceSQL = TestDataForDataSources.SqlDatasourceId;
            var sourceExcel = TestDataForDataSources.ExcelDatasourceId;
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var resultSQL = await controller.GetDataSource(sourceSQL);
            var resultExcel = await controller.GetDataSource(sourceExcel);

            // Assert
            Assert.IsType<OkObjectResult>(resultSQL);
            var objectResult = resultSQL as OkObjectResult;
            Assert.IsType<SQLDataSourceDTO>(objectResult.Value);
            var resultValueSQL = objectResult.Value as SQLDataSourceDTO;
            Assert.Equal(sourceSQL, resultValueSQL.Id);

            Assert.IsType<OkObjectResult>(resultExcel);
            objectResult = resultExcel as OkObjectResult;
            Assert.IsType<ExcelDataSourceDTO>(objectResult.Value);
            var resultValueExcel = objectResult.Value as ExcelDataSourceDTO;
            Assert.Equal(sourceExcel, resultValueExcel.Id);
        }

        [Fact]
        public async Task GetOneHandlesBadId()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);
            _mockDataSource.Setup(_ => _.GetDataSource(It.IsAny<Guid>())).Returns<BaseDataSourceDTO>(null);

            // Act
            var result = await controller.GetDataSource(_badSource);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetTypesReturnsValid()
        {
            // Arrange
            var accessor = HttpContextAccessorMocks.Default();
            var hubService = HubServiceMocks.Default();
            var controller = new DataSourceController(
                EsecSecurityMocks.Admin,
                _mockUOW.Object,
                hubService,
                accessor);

            // Act
            var result = await controller.GetDataSourceTypes();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = result as OkObjectResult;
            Assert.IsType<List<string>>(objectResult.Value);
            var resultValue = objectResult.Value as List<string>;
            Assert.Equal(2, resultValue.Count);
        }
        [Fact]
        public async Task UserIsViewDataSourceAuthorized()
        {
            // Non-admin authorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ViewDataSourceClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.DataSourceViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "ViewDataSourceClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsDeleteDataSourceAuthorized()
        {
            // Non-admin unauthorized
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("DeleteDataSourceClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.DataSourceModifyAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.Esec, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.ReadOnly }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "DeleteDataSourceClaim");
            // Assert
            Assert.False(allowed.Succeeded);
        }
        [Fact]
        public async Task UserIsViewDataSourceAuthorized_B2C()
        {
            // Arrange
            var authorizationService = BuildAuthorizationServiceMocks.BuildAuthorizationService(services =>
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("ViewDataSourceClaim",
                        policy => policy.RequireClaim(ClaimTypes.Name,
                                                      BridgeCareCore.Security.SecurityConstants.Claim.DataSourceViewAccess));
                });
            });
            var roleClaimsMapper = new RoleClaimsMapper();
            var controller = CreateTestController(roleClaimsMapper.GetClaims(BridgeCareCore.Security.SecurityConstants.SecurityTypes.B2C, new List<string> { BridgeCareCore.Security.SecurityConstants.Role.Administrator }));
            // Act
            var allowed = await authorizationService.AuthorizeAsync(controller.User, "ViewDataSourceClaim");
            // Assert
            Assert.True(allowed.Succeeded);
        }

    }
}
