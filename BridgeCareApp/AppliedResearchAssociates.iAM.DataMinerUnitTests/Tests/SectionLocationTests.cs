using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataMiner;
using System;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests
{
    public class SectionLocationTests
    {
        private readonly Guid guId = Guid.Empty;
        private const string locationIdentifier = "TestUniqueId";
        private const string locationIdentifier2 = "TestUniqueId2";
        private Mock<Location> mockLocation;
        private Mock<SectionLocation> mockSectionLocation;

        public SectionLocationTests()
        {
            mockLocation = new Mock<Location>(guId, locationIdentifier);
            mockSectionLocation = new Mock<SectionLocation>(guId, locationIdentifier);
        }

        [Fact]
        public void MatchOnTrueTest()
        {
            var sectionLocation = new SectionLocation(guId, locationIdentifier);

            var result = sectionLocation.MatchOn(mockSectionLocation.Object);
            Assert.True(result);
        }

        [Fact]
        public void MatchOnFalseTest()
        {
            var sectionLocation = new SectionLocation(guId, locationIdentifier2);

            var result = sectionLocation.MatchOn(mockSectionLocation.Object);
            Assert.False(result);
        }

        [Fact]
        public void MatchOnNonSectionLocationParameterTest()
        {
            var sectionLocation = new SectionLocation(guId, locationIdentifier);

            var result = sectionLocation.MatchOn(mockLocation.Object);
            Assert.False(result);
        }        
    }
}
