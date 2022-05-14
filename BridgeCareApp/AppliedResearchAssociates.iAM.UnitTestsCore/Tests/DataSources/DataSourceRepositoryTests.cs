using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;


namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.DataSources
{
    public class DataSourceRepositoryTests
    {
        private UnitOfDataPersistenceWork _testRepo;
        private IQueryable<DataSourceEntity> _testDataSourceList;
        private IQueryable<AttributeEntity> _testAttributeSourceList;
        private Mock<IAMContext> _mockedContext;
        private Mock<DbSet<DataSourceEntity>> _mockedDataSourceSet;
        private Mock<DbSet<AttributeEntity>> _mockedAttributeSet;

        public DataSourceRepositoryTests()
        {
            _mockedContext = new Mock<IAMContext>();
            _testDataSourceList = TestDataForDataSources.SimpleRepo().AsQueryable();
            _testAttributeSourceList = TestDataForDataSources.SimpleAttributeRepo().AsQueryable();

            // From https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN
            _mockedDataSourceSet = new Mock<DbSet<DataSourceEntity>>();
            _mockedDataSourceSet.As<IQueryable<DataSourceEntity>>().Setup(_ => _.Provider).Returns(_testDataSourceList.Provider);
            _mockedDataSourceSet.As<IQueryable<DataSourceEntity>>().Setup(_ => _.Expression).Returns(_testDataSourceList.Expression);
            _mockedDataSourceSet.As<IQueryable<DataSourceEntity>>().Setup(_ => _.ElementType).Returns(_testDataSourceList.ElementType);
            _mockedDataSourceSet.As<IQueryable<DataSourceEntity>>().Setup(_ => _.GetEnumerator()).Returns(_testDataSourceList.GetEnumerator());

            _mockedAttributeSet = new Mock<DbSet<AttributeEntity>>();
            _mockedAttributeSet.As<IQueryable<AttributeEntity>>().Setup(_ => _.Provider).Returns(_testAttributeSourceList.Provider);
            _mockedAttributeSet.As<IQueryable<AttributeEntity>>().Setup(_ => _.Expression).Returns(_testAttributeSourceList.Expression);
            _mockedAttributeSet.As<IQueryable<AttributeEntity>>().Setup(_ => _.ElementType).Returns(_testAttributeSourceList.ElementType);
            _mockedAttributeSet.As<IQueryable<AttributeEntity>>().Setup(_ => _.GetEnumerator()).Returns(_testAttributeSourceList.GetEnumerator());

            _mockedContext.Setup(_ => _.DataSource).Returns(_mockedDataSourceSet.Object);
            _mockedContext.Setup(_ => _.Set<DataSourceEntity>()).Returns(_mockedDataSourceSet.Object);
            _mockedContext.Setup(_ => _.Attribute).Returns(_mockedAttributeSet.Object);
            _mockedContext.Setup(_ => _.Set<AttributeEntity>()).Returns(_mockedAttributeSet.Object);

            var mockedRepo = new UnitOfDataPersistenceWork((new Mock<IConfiguration>()).Object, _mockedContext.Object);
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
        public void GetReturnsNullWhenIdDoesNotExist()
        {
            // Arrange
            var repo = new DataSourceRepository(_testRepo);

            // Act
            var result = repo.GetDataSource(new Guid("5bd3dcb8-c8a4-409e-915f-b5bf8875f652"));

            // Assert
            Assert.Equal(null, result);
        }

        // We should test a successful delete here, but since it is an extension we cannot do that

        [Fact]
        public void DoesNotDeleteDataSourcesWithAttributes()
        {
            // Arrange
            var repo = new DataSourceRepository(_testRepo);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => repo.DeleteDataSource(new Guid("72b3cca4-57f1-4e0d-ad13-37c2664f1299")));
        }

        [Fact]
        public void DeleteHandlesIdDoesNotExist()
        {
            // Arrange
            var repo = new DataSourceRepository(_testRepo);

            // Act & Assert
            Assert.Throws<RowNotInTableException>(() => repo.DeleteDataSource(new Guid("5bd3dcb8-c8a4-409e-915f-b5bf8875f652")));
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
