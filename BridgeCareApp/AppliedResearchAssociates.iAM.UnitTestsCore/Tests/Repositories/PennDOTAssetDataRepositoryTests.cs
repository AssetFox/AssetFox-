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
    public class PennDOTAssetDataRepositoryTests
    {
        private TestDataForPennDOTAssetDataRepo _testData;
        private UnitOfDataPersistenceWork _testRepo;
        private Mock<IAMContext> _mockedContext;
        private Mock<DbSet<AttributeEntity>> _mockedAttributeEntitySet;
        private Mock<DbSet<SectionEntity>> _mockedSectionEntitySet;
        private Mock<ILogger<PennDOTAssetDataRepository>> _mockedLogger;

        public PennDOTAssetDataRepositoryTests()
        {
            _testData = new TestDataForPennDOTAssetDataRepo();
            _mockedContext = new Mock<IAMContext>();

            // From https://docs.microsoft.com/en-us/ef/ef6/fundamentals/testing/mocking?redirectedfrom=MSDN
            _mockedAttributeEntitySet = new Mock<DbSet<AttributeEntity>>();
            _mockedAttributeEntitySet.As<IQueryable<AttributeEntity>>().Setup(_ => _.Provider).Returns(_testData.AttributeLibrary.Provider);
            _mockedAttributeEntitySet.As<IQueryable<AttributeEntity>>().Setup(_ => _.Expression).Returns(_testData.AttributeLibrary.Expression);
            _mockedAttributeEntitySet.As<IQueryable<AttributeEntity>>().Setup(_ => _.ElementType).Returns(_testData.AttributeLibrary.ElementType);
            _mockedAttributeEntitySet.As<IQueryable<AttributeEntity>>().Setup(_ => _.GetEnumerator()).Returns(_testData.AttributeLibrary.GetEnumerator());

            _mockedSectionEntitySet = new Mock<DbSet<SectionEntity>>();
            _mockedSectionEntitySet.As<IQueryable<SectionEntity>>().Setup(_ => _.Provider).Returns(_testData.SectionLibrary.Provider);
            _mockedSectionEntitySet.As<IQueryable<SectionEntity>>().Setup(_ => _.Expression).Returns(_testData.SectionLibrary.Expression);
            _mockedSectionEntitySet.As<IQueryable<SectionEntity>>().Setup(_ => _.ElementType).Returns(_testData.SectionLibrary.ElementType);
            _mockedSectionEntitySet.As<IQueryable<SectionEntity>>().Setup(_ => _.GetEnumerator()).Returns(_testData.SectionLibrary.GetEnumerator());

            _mockedContext.Setup(_ => _.Attribute).Returns(_mockedAttributeEntitySet.Object);
            _mockedContext.Setup(_ => _.Section).Returns(_mockedSectionEntitySet.Object);
            var mockedRepo = new Mock<UnitOfDataPersistenceWork>((new Mock<IConfiguration>()).Object, _mockedContext.Object);
            //mockedRepo.Setup(_ => _.Context).Returns(_mockedContext.Object);
            mockedRepo.Setup(_ => _.NetworkRepo.GetPennDotNetwork()).Returns(_testData.TestNetwork);
            _testRepo = mockedRepo.Object;

            _mockedLogger = new Mock<ILogger<PennDOTAssetDataRepository>>();
        }

        [Fact]
        public void GeneratesKeyPropertiesDictionaryWithNumericKey()
        {
            // Arrange
            var keyFields = new List<string>() { "BRKey", "BMSID" };
            var checkGuid = new Guid("8f80c690-3088-4084-b0e5-a8e070000a06");

            // Act
            var repo = new PennDOTAssetDataRepository(keyFields, _testRepo, _mockedLogger.Object);

            // Assert
            Assert.Equal(2, repo.KeyProperties.Count());
            Assert.Equal(5, repo.KeyProperties["BRKey"].Count());
            Assert.NotNull(repo.KeyProperties["BRKey"].FirstOrDefault(_ => _.KeyValue.Value == "2").SegmentId == checkGuid);
            Assert.NotNull(repo.KeyProperties["BMSID"].FirstOrDefault(_ => _.KeyValue.Value == "13401256").SegmentId == checkGuid);
        }

        [Fact]
        public void HandlesMissingKeyInAttributeRepository()
        {
            // Arrange
            var keyFields = new List<string>() { "Dummy" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new PennDOTAssetDataRepository(keyFields, _testRepo, _mockedLogger.Object));

        }

        [Fact]
        public void HandlesNoDataForExistingAttribute()
        {
            // Arrange
            var keyFields = new List<string>() { "NoData" };

            // Act
            var repo = new PennDOTAssetDataRepository(keyFields, _testRepo, _mockedLogger.Object);

            // Assert
            Assert.Equal(1, repo.KeyProperties.Count());
            Assert.Equal(0, repo.KeyProperties["NoData"].Count());
        }

        [Fact]
        public void ReturnsSegmeentData()
        {
            // Arrange
            var keyFields = new List<string>() { "BRKey", "BMSID" };
            var repo = new PennDOTAssetDataRepository(keyFields, _testRepo, _mockedLogger.Object);

            // Act
            var testSegment = repo.GetAssetAttributes("BRKey", "2");

            // Assert
            Assert.Equal(1, testSegment.Where(_ => _.Name == "BRKey").Count());
            Assert.Equal(2, testSegment.First(_ => _.Name == "BRKey").NumericValue);
            Assert.Equal("2", testSegment.First(_ => _.Name == "BRKey").Value);
            Assert.Equal(15.4, testSegment.First(_ => _.Name == "Length").NumericValue);
            Assert.Equal("13401256", testSegment.First(_ => _.Name == "BMSID").Value);
            Assert.Equal("First B", testSegment.First(_ => _.Name == "Name").TextValue);
        }

        [Fact]
        public void HandlesUnmatchedKey()
        {
            // Arrange
            var keyFields = new List<string>() { "BRKey", "BMSID" };
            var repo = new PennDOTAssetDataRepository(keyFields, _testRepo, _mockedLogger.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => repo.GetAssetAttributes("Dummy", "0"));
        }

        [Fact]
        public void HandlesNoSegmentFound()
        {
            // Should the system also remove the asset from KeyProperties if not found?  I think so.

            // Arrange
            var keyFields = new List<string>() { "BRKey", "BMSID" };
            var repo = new PennDOTAssetDataRepository(keyFields, _testRepo, _mockedLogger.Object);

            // Act
            var testSegment = repo.GetAssetAttributes("BRKey", "100");

            // Assert
            Assert.Equal(0, testSegment.Count());
        }
    }
}
