using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DataSources
{
    public class DataSourceRepositoryTests
    {
        private UnitOfDataPersistenceWork _testRepo;
        private IQueryable<DataSourceEntity> _testDataSourceList;
        private IQueryable<AttributeEntity> _testAttributeSourceList;
        private Mock<IAMContext> _mockedContext;
        private Mock<DbSet<DataSourceEntity>> _mockedDataSourceSet;

        public DataSourceRepositoryTests()
        {
            _mockedContext = new Mock<IAMContext>();
            var encryptionKey = TestHelper.UnitOfWork.EncryptionKey;
            _testDataSourceList = TestEntitiesForDataSources.SimpleRepo(encryptionKey).AsQueryable();
            _testAttributeSourceList = TestEntitiesForDataSources.SimpleAttributeRepo(encryptionKey).AsQueryable();

            _mockedDataSourceSet = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.DataSource, _testDataSourceList);
            MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, _testAttributeSourceList);

            var mockedRepo = new UnitOfDataPersistenceWork(TestHelper.UnitOfWork.Config, _mockedContext.Object);
            _testRepo = mockedRepo;
        }

        [Fact]
        public void CanGetAllValidDataSources()
        {
            // Arrange
            var repo = new DataSourceRepository(_testRepo);

            // Act
            var result = repo.GetDataSources();
            var testSource = result.First(_ => _.Id == new Guid("72b3cca4-57f1-4e0d-ad13-37c2664f1299"));

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("SQL Server Data Source", testSource.Name);
            Assert.True(testSource is SQLDataSourceDTO);
        }

        [Fact]
        public void CanGetSpecificDataSource()
        {
            // Arrange
            var repo = new DataSourceRepository(_testRepo);

            // Act
            var result = repo.GetDataSource(new Guid("72b3cca4-57f1-4e0d-ad13-37c2664f1299"));

            // Assert
            Assert.Equal("SQL Server Data Source", result.Name);
            Assert.True(result is SQLDataSourceDTO);
        }

        [Fact]
        public void Get_DatasourceDoesNotExist_ReturnsNull()
        {
            // Arrange
            var repo = TestHelper.UnitOfWork.DataSourceRepo;

            // Act
            var result = repo.GetDataSource(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Delete_DatasourceExists_Deletes()
        {
            // Arrange
            var repo = new DataSourceRepository(_testRepo);

            // Act
            repo.DeleteDataSource(new Guid("147cb3e1-e9fc-4fd6-a265-105d546d9ddb"));

            // Assert
            _mockedDataSourceSet.Verify(_ => _.Remove(It.IsAny<DataSourceEntity>()), Times.Once());
            _mockedContext.Verify(_ => _.SaveChanges(), Times.Once());
        }


        [Fact]
        public void Delete_DatasourceDoesNotExist_Throws()
        {
            // Arrange
            var repo = TestHelper.UnitOfWork.DataSourceRepo;

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.DeleteDataSource(Guid.NewGuid()));
        }

        // We should test a successful addition here, but since it is an extension we cannot do that

        // We should test a successful update here, but since it is an extension we cannot do that

        [Fact]
        public void DataSourceWithDuplicateNameCannotBeAdded()
        {
            // Arrange
            var repo = new DataSourceRepository(_testRepo);
            var newSource = new ExcelDataSourceDTO
            {
                Id = Guid.NewGuid(),
                Name = "Some Excel File",
                DateColumn = "InspectionTime",
                LocationColumn = "AssetID"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => repo.UpsertDatasource(newSource));
            Assert.Equal("An existing data source with the same name already exists", exception.Message);
        }

        [Fact]
        public void AddHandlesFailedValidation()
        {
            // Arrange
            var repo = new DataSourceRepository(_testRepo);
            var newSource = new BadExcelDataSourceDTO
            {
                Id = Guid.NewGuid(),
                Name = "A bad Excel File",
                DateColumn = "InspectionTime",
                LocationColumn = "AssetID"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => repo.UpsertDatasource(newSource));
            Assert.Equal("The data source could not be validated", exception.Message);
        }
    }
}
