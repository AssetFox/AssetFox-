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
        private UnitOfDataPersistenceWork _testRepo;
        private Mock<IAMContext> _mockedContext;
        private Mock<DbSet<MaintainableAssetEntity>> _mockedMaintainableAssetEntitySet;
        private Mock<DbSet<AggregatedResultEntity>> _mockedAggregatedResultsEntitySet;
        private Mock<DbSet<MaintainableAssetLocationEntity>> _mockedMaintainableAssetLocationEntitySet;
        private Mock<DbSet<AttributeEntity>> _mockedAttributeSet;

        private void Setup()
        {
            _testData = new TestDataForMaintainableAssetRepo();
            _mockedContext = new Mock<IAMContext>();

            _mockedMaintainableAssetEntitySet = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.MaintainableAsset, _testData.MaintainableAssetsLibrary);
            _mockedAggregatedResultsEntitySet = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.AggregatedResult, _testData.AggregatedResultsLibrary);
            _mockedMaintainableAssetLocationEntitySet = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.MaintainableAssetLocation, _testData.MaintainableAssetLocationLibrary);
            _mockedAttributeSet = MockedContextBuilder.AddDataSet(_mockedContext, _ => _.Attribute, _testData.AttributeLibrary);

            var mockedConfiguration = new Mock<IConfiguration>();
            MockedContextBuilder.AddConfigurationKeys(mockedConfiguration, _testRepo.AdminDataRepo.GetKeyFields(), new List<string> { "BRKEY_", "BMSID" });

            var mockedRepo = new Mock<UnitOfDataPersistenceWork>(mockedConfiguration.Object, _mockedContext.Object);
            mockedRepo.Setup(_ => _.NetworkRepo.GetMainNetwork()).Returns(_testData.TestNetwork);
            _testRepo = mockedRepo.Object;
        }

        [Fact]
        public void GeneratesKeyPropertiesDictionaryWithNumericKey()
        {
            // Arrange
            Setup();
            var checkGuid = new Guid("8f80c690-3088-4084-b0e5-a8e070000a06");

            // Act
            var repo = new MaintainableAssetDataRepository(_testRepo);

            // Assert
            Assert.Equal(2, repo.KeyProperties.Count());
            Assert.Equal(5, repo.KeyProperties["BRKEY_"].Count());
            var brKeyDatum = repo.KeyProperties["BRKEY_"].FirstOrDefault(_ => _.KeyValue.Value == "13401256");
            var bmsIdDatum = repo.KeyProperties["BMSID"].FirstOrDefault(_ => _.KeyValue.Value == "13401256");
            Assert.Equal(brKeyDatum.AssetId, checkGuid);
            Assert.Equal(bmsIdDatum.AssetId, checkGuid);
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
