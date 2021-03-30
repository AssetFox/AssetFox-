using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestData;
using Microsoft.Extensions.Logging;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public class PennDOTMaintainableAsseetDataRepositoryTests
    {
        private TestDataForPennDOTMaintainableAssetRepo _testData;
        private UnitOfDataPersistenceWork _testRepo;
        private Mock<IAMContext> _mockedContext;
        private Mock<DbSet<MaintainableAssetEntity>> _mockedMaintainableAssetEntitySet;
        private Mock<DbSet<AggregatedResultEntity>> _mockedAggregatedResultsEntitySet;

        public PennDOTMaintainableAsseetDataRepositoryTests()
        {
            _testData = new TestDataForPennDOTMaintainableAssetRepo();
            _mockedContext = new Mock<IAMContext>();

            // From https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN
            _mockedMaintainableAssetEntitySet = new Mock<DbSet<MaintainableAssetEntity>>();
            _mockedMaintainableAssetEntitySet.As<IQueryable<MaintainableAssetEntity>>().Setup(_ => _.Provider).Returns(_testData.MaintainableAssetsLibrary.Provider);
            _mockedMaintainableAssetEntitySet.As<IQueryable<MaintainableAssetEntity>>().Setup(_ => _.Expression).Returns(_testData.MaintainableAssetsLibrary.Expression);
            _mockedMaintainableAssetEntitySet.As<IQueryable<MaintainableAssetEntity>>().Setup(_ => _.ElementType).Returns(_testData.MaintainableAssetsLibrary.ElementType);
            _mockedMaintainableAssetEntitySet.As<IQueryable<MaintainableAssetEntity>>().Setup(_ => _.GetEnumerator()).Returns(_testData.MaintainableAssetsLibrary.GetEnumerator());

            _mockedAggregatedResultsEntitySet = new Mock<DbSet<AggregatedResultEntity>>();
            _mockedAggregatedResultsEntitySet.As<IQueryable<AggregatedResultEntity>>().Setup(_ => _.Provider).Returns(_testData.AggregatedResultsLibrary.Provider);
            _mockedAggregatedResultsEntitySet.As<IQueryable<AggregatedResultEntity>>().Setup(_ => _.Expression).Returns(_testData.AggregatedResultsLibrary.Expression);
            _mockedAggregatedResultsEntitySet.As<IQueryable<AggregatedResultEntity>>().Setup(_ => _.ElementType).Returns(_testData.AggregatedResultsLibrary.ElementType);
            _mockedAggregatedResultsEntitySet.As<IQueryable<AggregatedResultEntity>>().Setup(_ => _.GetEnumerator()).Returns(_testData.AggregatedResultsLibrary.GetEnumerator());

            _mockedContext.Setup(_ => _.MaintainableAsset).Returns(_mockedMaintainableAssetEntitySet.Object);
            _mockedContext.Setup(_ => _.AggregatedResult).Returns(_mockedAggregatedResultsEntitySet.Object);
            var mockedRepo = new Mock<UnitOfDataPersistenceWork>((new Mock<IConfiguration>()).Object, _mockedContext.Object);
            mockedRepo.Setup(_ => _.NetworkRepo.GetPennDotNetwork()).Returns(_testData.TestNetwork);
            _testRepo = mockedRepo.Object;
        }

        [Fact]
        public void GeneratesKeyPropertiesDictionaryWithNumericKey()
        {
            // Arrange
            var checkGuid = new Guid("8f80c690-3088-4084-b0e5-a8e070000a06");

            // Act
            var repo = new PennDOTMaintainableAssetDataRepository(_testRepo);

            // Assert
            Assert.Equal(2, repo.KeyProperties.Count());
            Assert.Equal(5, repo.KeyProperties["BRKEY"].Count());
            Assert.NotNull(repo.KeyProperties["BRKEY"].FirstOrDefault(_ => _.KeyValue.Value == "2").SegmentId == checkGuid);
            Assert.NotNull(repo.KeyProperties["BMSID"].FirstOrDefault(_ => _.KeyValue.Value == "13401256").SegmentId == checkGuid);
        }

        [Fact]
        public void ReturnsSegmeentDataWithBRKey()
        {
            // Arrange
            var repo = new PennDOTMaintainableAssetDataRepository(_testRepo);

            // Act
            var testSegment = repo.GetAssetAttributes("BRKEY", "2");

            // Assert
            Assert.Equal(1, testSegment.Where(_ => _.Name == "BRKEY").Count());
            Assert.Equal(2, testSegment.First(_ => _.Name == "BRKEY").NumericValue);
            Assert.Equal("2", testSegment.First(_ => _.Name == "BRKEY").Value);
            Assert.Equal(15.4, testSegment.First(_ => _.Name == "Length").NumericValue);
            Assert.Equal("First B", testSegment.First(_ => _.Name == "Name").TextValue);
        }

        [Fact]
        public void ReturnsSegmeentDataWithBMSID()
        {
            // Arrange
            var repo = new PennDOTMaintainableAssetDataRepository(_testRepo);

            // Act
            var testSegment = repo.GetAssetAttributes("BMSID", "13401256");

            // Assert
            Assert.Equal(1, testSegment.Where(_ => _.Name == "BRKEY").Count());
            Assert.Equal(2, testSegment.First(_ => _.Name == "BRKEY").NumericValue);
            Assert.Equal("2", testSegment.First(_ => _.Name == "BRKEY").Value);
            Assert.Equal(15.4, testSegment.First(_ => _.Name == "Length").NumericValue);
            Assert.Equal("First B", testSegment.First(_ => _.Name == "Name").TextValue);
        }

        [Fact]
        public void HandlesUnmatchedKey()
        {
            // Arrange
            var repo = new PennDOTMaintainableAssetDataRepository(_testRepo);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => repo.GetAssetAttributes("Dummy", "0"));
        }

        [Fact]
        public void HandlesNoSegmentFound()
        {
            // Should the system also remove the asset from KeyProperties if not found?  I think so.

            // Arrange
            var repo = new PennDOTMaintainableAssetDataRepository(_testRepo);

            // Act
            var testSegment = repo.GetAssetAttributes("BRKEY", "100");

            // Assert
            Assert.Equal(0, testSegment.Count());
        }
    }
}
