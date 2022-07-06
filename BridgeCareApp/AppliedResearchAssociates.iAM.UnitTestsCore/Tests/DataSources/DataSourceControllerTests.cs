using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using BridgeCareCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DataSources
{
    public class DataSourceControllerTests
    {
        private TestHelper _testHelper => TestHelper.Instance;
        private Mock<IUnitOfWork> _mockUOW;
        private Mock<IDataSourceRepository> _mockDataSource;
        private Guid _badSource = Guid.Parse("7ed4d236-7f8c-450a-ae05-9c1359ed0ce6");

        public DataSourceControllerTests()
        {
            _mockUOW = new Mock<IUnitOfWork>();

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(_ => _.UserExists(It.IsAny<string>())).Returns(true);
            _mockUOW.Setup(_ => _.UserRepo).Returns(mockUserRepo.Object);

            _mockDataSource = new Mock<IDataSourceRepository>();
            _mockDataSource.Setup(_ => _.GetDataSources()).Returns(TestDataForDataSources.SourceDTOs());
            _mockDataSource.Setup(_ => _.GetDataSource(It.IsAny<Guid>()))
                .Returns<Guid>(p => TestDataForDataSources.SourceDTOs().Single(ds => ds.Id == p));

            _mockUOW.Setup(_ => _.DataSourceRepo).Returns(_mockDataSource.Object);
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
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

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
            var hubService = _testHelper.MockHubService;
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                hubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

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
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

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
            var sourceToDelete = TestDataForDataSources.SimpleRepo().Single(_ => _.Name == "Some Excel File").Id;
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await controller.DeleteDataSource(sourceToDelete);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockDataSource.Verify(_ => _.DeleteDataSource(It.IsAny<Guid>()), Times.Once());        }

        [Fact]
        public async Task DeleteHandlesBadDataSourceId()
        {
            // Arrange
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);
            _mockDataSource.Setup(_ => _.DeleteDataSource(It.IsAny<Guid>())).Throws<RowNotInTableException>();

            // Act & Assert
            await Assert.ThrowsAsync<RowNotInTableException>(() => controller.DeleteDataSource(_badSource));
        }

        [Fact]
        public async Task GetAllDataSourcesReturnsValues()
        {
            // Arrange
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await controller.GetDataSources();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = result as OkObjectResult;
            Assert.IsType<List<BaseDataSourceDTO>>(objectResult.Value);
            var resultValue = objectResult.Value as List<BaseDataSourceDTO>;
            Assert.Equal(2, resultValue.Count);
            Assert.True(resultValue.Any(_ => _.Name == "SQL Server Data Source"));
        }

        [Fact]
        public async Task GetAllWorksWithNoDataSources()
        {
            // Arrange
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);
            _mockDataSource.Setup(_ => _.GetDataSources()).Returns(new List<BaseDataSourceDTO>());

            // Act
            var result = await controller.GetDataSources();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = result as OkObjectResult;
            Assert.IsType<List<BaseDataSourceDTO>>(objectResult.Value);
            var resultValue = objectResult.Value as List<BaseDataSourceDTO>;
            Assert.Equal(0, resultValue.Count);
        }

        [Fact]
        public async Task GetOneReturnsProperTypes()
        {
            // We can test BOTH Excel and SQL here
            // Arrange
            var sourceSQL = TestDataForDataSources.SimpleRepo().Single(_ => _.Name == "SQL Server Data Source").Id;
            var sourceExcel = TestDataForDataSources.SimpleRepo().Single(_ => _.Name == "Some Excel File").Id;
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

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
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);
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
            var controller = new DataSourceController(
                _testHelper.MockEsecSecurityAdmin.Object,
                _mockUOW.Object,
                _testHelper.MockHubService.Object,
                _testHelper.MockHttpContextAccessor.Object);

            // Act
            var result = await controller.GetDataSourceTypes();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var objectResult = result as OkObjectResult;
            Assert.IsType<List<string>>(objectResult.Value);
            var resultValue = objectResult.Value as List<string>;
            Assert.Equal(3, resultValue.Count);
        }
    }
}
