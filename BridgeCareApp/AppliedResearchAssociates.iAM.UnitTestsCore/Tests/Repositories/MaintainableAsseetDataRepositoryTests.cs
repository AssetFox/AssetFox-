﻿using System;
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

        public void Setup()
        {
            _testData = new TestDataForMaintainableAssetRepo();
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

            _mockedMaintainableAssetLocationEntitySet = new Mock<DbSet<MaintainableAssetLocationEntity>>();
            _mockedMaintainableAssetLocationEntitySet.As<IQueryable<MaintainableAssetLocationEntity>>().Setup(_ => _.Provider).Returns(_testData.MaintainableAssetLocationLibrary.Provider);
            _mockedMaintainableAssetLocationEntitySet.As<IQueryable<MaintainableAssetLocationEntity>>().Setup(_ => _.Expression).Returns(_testData.MaintainableAssetLocationLibrary.Expression);
            _mockedMaintainableAssetLocationEntitySet.As<IQueryable<MaintainableAssetLocationEntity>>().Setup(_ => _.ElementType).Returns(_testData.MaintainableAssetLocationLibrary.ElementType);
            _mockedMaintainableAssetLocationEntitySet.As<IQueryable<MaintainableAssetLocationEntity>>().Setup(_ => _.GetEnumerator()).Returns(_testData.MaintainableAssetLocationLibrary.GetEnumerator());

            _mockedAttributeSet = new Mock<DbSet<AttributeEntity>>();
            _mockedAttributeSet.As<IQueryable<AttributeEntity>>().Setup(_ => _.Provider).Returns(_testData.AttributeLibrary.Provider);
            _mockedAttributeSet.As<IQueryable<AttributeEntity>>().Setup(_ => _.Expression).Returns(_testData.AttributeLibrary.Expression);
            _mockedAttributeSet.As<IQueryable<AttributeEntity>>().Setup(_ => _.ElementType).Returns(_testData.AttributeLibrary.ElementType);
            _mockedAttributeSet.As<IQueryable<AttributeEntity>>().Setup(_ => _.GetEnumerator()).Returns(_testData.AttributeLibrary.GetEnumerator());

            _mockedContext.Setup(_ => _.MaintainableAsset).Returns(_mockedMaintainableAssetEntitySet.Object);
            _mockedContext.Setup(_ => _.AggregatedResult).Returns(_mockedAggregatedResultsEntitySet.Object);
            _mockedContext.Setup(_ => _.MaintainableAssetLocation).Returns(_mockedMaintainableAssetLocationEntitySet.Object);
            _mockedContext.Setup(_ => _.Attribute).Returns(_mockedAttributeSet.Object);

            var mockedConfiguration = new Mock<IConfiguration>();
            var mockedKeySection = new Mock<IConfigurationSection>();
            var mockedKeySectionList = new List<IConfigurationSection>()
            {
                CreateSpecificIConfigurationSection("BRKEY_"),
                CreateSpecificIConfigurationSection("BMSID")
            };
            mockedKeySection.Setup(_ => _.GetChildren()).Returns(mockedKeySectionList.AsEnumerable());
            mockedConfiguration.Setup(_ => _.GetSection("InventoryData:KeyProperties")).Returns(mockedKeySection.Object);

            var mockedRepo = new Mock<UnitOfDataPersistenceWork>(mockedConfiguration.Object, _mockedContext.Object);
            mockedRepo.Setup(_ => _.NetworkRepo.GetMainNetwork()).Returns(_testData.TestNetwork);
            //mockedRepo.Setup(_ => _.AttributeRepo.GetAttributes()).Returns(_testData.AttributeLibrary.Select(_ => _.ToDto()).ToList());
            _testRepo = mockedRepo.Object;
        }

        private IConfigurationSection CreateSpecificIConfigurationSection(string returnValue)
        {
            var section = new Mock<IConfigurationSection>();
            section.Setup(_ => _.Value).Returns(returnValue);
            return section.Object;
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
            Assert.NotNull(repo.KeyProperties["BRKEY_"].FirstOrDefault(_ => _.KeyValue.Value == "2").AssetId == checkGuid);
            Assert.NotNull(repo.KeyProperties["BMSID"].FirstOrDefault(_ => _.KeyValue.Value == "13401256").AssetId == checkGuid);
        }

        [Fact]
        public void ReturnsSegmeentDataWithBRKey()
        {
            // Arrange
            Setup();
            var repo = new MaintainableAssetDataRepository(_testRepo);

            // Act
            var testSegment = repo.GetAssetAttributes("BRKEY_", "2");

            // Assert
            Assert.Equal(1, testSegment.Where(_ => _.Name == "BRKEY_").Count());            
            Assert.Equal("2", testSegment.First(_ => _.Name == "BRKEY_").Value);
            Assert.Equal("15.4", testSegment.First(_ => _.Name == "Length").TextValue);
            Assert.Equal("First B", testSegment.First(_ => _.Name == "Name").TextValue);
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
            Assert.Equal("2", testSegment.First(_ => _.Name == "BRKEY_").Value);
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
