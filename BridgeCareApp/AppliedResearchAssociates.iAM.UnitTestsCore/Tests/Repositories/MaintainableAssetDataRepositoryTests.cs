using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Repositories
{
    public class MaintainableAssetDataRepositoryTests
    {
        private TestDataForMaintainableAssetRepo _testData;
        private TestDataForMaintainableAssetRepo _failedTestData;
        private UnitOfDataPersistenceWork _testRepo;
        private UnitOfDataPersistenceWork _testRawRepo;
        private UnitOfDataPersistenceWork _failedRepo;
        private Mock<IAMContext> _mockedContext;
        private Mock<IAMContext> _failedMockedContext;
        private Mock<DbSet<MaintainableAssetEntity>> _mockedMaintainableAssetEntitySet;
        private Mock<DbSet<AggregatedResultEntity>> _mockedAggregatedResultsEntitySet;
        private Mock<DbSet<MaintainableAssetLocationEntity>> _mockedMaintainableAssetLocationEntitySet;
        private Mock<DbSet<AttributeEntity>> _mockedAttributeSet;
        private Mock<DbSet<AdminSettingsEntity>> _mockedAdminSettings;

        private void Setup()
        {
            _testData = new TestDataForMaintainableAssetRepo();
            _failedMockedContext = new Mock<IAMContext>();
            _mockedContext = new Mock<IAMContext>();

            _mockedMaintainableAssetEntitySet = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.MaintainableAsset, _testData.MaintainableAssetsLibrary);
            _mockedAggregatedResultsEntitySet = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.AggregatedResult, _testData.AggregatedResultsLibrary);
            _mockedAdminSettings = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.AdminSettings, _testData.AdminSettingsLibrary);
            _mockedMaintainableAssetLocationEntitySet = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.MaintainableAssetLocation, _testData.MaintainableAssetLocationLibrary);
            _mockedAttributeSet = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, _testData.AttributeLibrary);
            var mockedConfiguration = new Mock<IConfiguration>();

            var mockedRepo = new Mock<UnitOfDataPersistenceWork>(mockedConfiguration.Object, _mockedContext.Object);
            mockedRepo.Setup(_ => _.NetworkRepo.GetMainNetwork()).Returns(_testData.TestNetwork);
            _testRepo = mockedRepo.Object;

            var mockedRawRepo = new Mock<UnitOfDataPersistenceWork>(mockedConfiguration.Object, _mockedContext.Object);
            mockedRepo.Setup(_ => _.NetworkRepo.GetRawNetwork()).Returns(_testData.TestNetwork);
            _testRawRepo = mockedRepo.Object;

            _mockedMaintainableAssetEntitySet = MockedContextBuilder.AddDataSet(_failedMockedContext, _ => _.MaintainableAsset, _testData.MaintainableAssetsLibrary);
            _mockedAggregatedResultsEntitySet = MockedContextBuilder.AddDataSet(_failedMockedContext, _ => _.AggregatedResult, _testData.AggregatedResultsLibrary);
            _mockedAdminSettings = MockedContextBuilder.AddDataSet(_failedMockedContext, _ => _.AdminSettings, _testData.FailedAdminSettingsLibrary);
            _mockedMaintainableAssetLocationEntitySet = MockedContextBuilder.AddDataSet(_failedMockedContext, _ => _.MaintainableAssetLocation, _testData.MaintainableAssetLocationLibrary);
            _mockedAttributeSet = MockedContextBuilder.AddDataSet(_failedMockedContext, _ => _.Attribute, _testData.AttributeLibrary);
            var failedMockedConfiguration = new Mock<IConfiguration>();

            var mockedFailedRepo = new Mock<UnitOfDataPersistenceWork>(failedMockedConfiguration.Object, _failedMockedContext.Object);
            mockedFailedRepo.Setup(_ => _.NetworkRepo.GetMainNetwork()).Returns(_testData.TestNetwork);
            mockedFailedRepo.Setup(_ => _.NetworkRepo.GetRawNetwork()).Returns(_testData.TestNetwork);
            _failedRepo = mockedFailedRepo.Object;
        }

        [Fact]
        public void GeneratesKeyPropertiesDictionaryWithNumericKey()
        {
            // Arrange
            Setup();
            var checkGuid = new Guid("8f80c690-3088-4084-b0e5-a8e070000a06");

            // Act
            var repo = new MaintainableAssetDataRepository(_testRepo);
            var rawRepo = new MaintainableAssetDataRepository(_testRawRepo);

            // Assert
            Assert.Equal(2, repo.KeyProperties.Count());
            Assert.Equal(5, repo.KeyProperties["BRKEY_"].Count());
            var brKeyDatum = repo.KeyProperties["BRKEY_"].FirstOrDefault(_ => _.KeyValue.Value == "13401256");
            var bmsIdDatum = repo.KeyProperties["BMSID"].FirstOrDefault(_ => _.KeyValue.Value == "13401256");
            Assert.Equal(brKeyDatum.AssetId, checkGuid);
            Assert.Equal(bmsIdDatum.AssetId, checkGuid);

            Assert.Equal(2, rawRepo.KeyProperties.Count());
            Assert.Equal(5, rawRepo.KeyProperties["BRKEY_"].Count());
            var brKeyRawDatum = rawRepo.KeyProperties["BRKEY_"].FirstOrDefault(_ => _.KeyValue.Value == "13401256");
            var bmsIdRawDatum = rawRepo.KeyProperties["BMSID"].FirstOrDefault(_ => _.KeyValue.Value == "13401256");
            Assert.Equal(brKeyRawDatum.AssetId, checkGuid);
            Assert.Equal(bmsIdRawDatum.AssetId, checkGuid);
        }

        [Fact]
        public void failedReport()
        {
            Setup();
            var failedRepo = new MaintainableAssetDataRepository(_failedRepo);

            Assert.Equal(0, failedRepo.KeyProperties.Count());
        }

        [Fact]
        public void ReturnsSegmentDataWithBRKey()
        {
            Setup();
            var repo = new MaintainableAssetDataRepository(_testRepo);

            // Act
            var testSegment = repo.GetAssetAttributes("BRKEY_", "13401256");

            // Assert
            var brKeyAsset = testSegment.Single(_ => _.Name == "BRKEY_");
            Assert.Equal("13401256", brKeyAsset.TextValue);
            var lengthAsset = testSegment.First(_ => _.Name == "Length");
            var nameAsset = testSegment.First(_ => _.Name == "Name");
            Assert.Equal("15.4", lengthAsset.TextValue);
            Assert.Equal("First B", nameAsset.TextValue);
        }

        [Fact]
        public void ReturnsSegmeentDataWithBMSID()
        {
            // Arrange
            Setup();
            var repo = new MaintainableAssetDataRepository(_testRepo);

            // Act
            var testSegment = repo.GetAssetAttributes("BMSID", "13401256");

            // Assert
            Assert.Equal(1, testSegment.Where(_ => _.Name == "BRKEY_").Count());            
            Assert.Equal("13401256", testSegment.First(_ => _.Name == "BRKEY_").Value);
            Assert.Equal("15.4", testSegment.First(_ => _.Name == "Length").TextValue);
            Assert.Equal("First B", testSegment.First(_ => _.Name == "Name").TextValue);
        }

        [Fact]
        public void HandlesUnmatchedKey()
        {
            // Arrange
            Setup();
            var repo = new MaintainableAssetDataRepository(_testRepo);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => repo.GetAssetAttributes("Dummy", "0"));
        }

        [Fact]
        public void HandlesNoSegmentFound()
        {
            // Should the system also remove the asset from KeyProperties if not found?  I think so.

            // Arrange
            Setup();
            var repo = new MaintainableAssetDataRepository(_testRepo);

            // Act
            var testSegment = repo.GetAssetAttributes("BRKEY_", "100");

            // Assert
            Assert.Equal(0, testSegment.Count());
        }
    }
}
