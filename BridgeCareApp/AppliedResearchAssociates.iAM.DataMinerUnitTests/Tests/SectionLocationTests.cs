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
            // Arrange
            var sectionLocation = new SectionLocation(guId, locationIdentifier);

            // Act
            var result = sectionLocation.MatchOn(mockSectionLocation.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MatchOnFalseTest()
        {
            // Arrange
            var sectionLocation = new SectionLocation(guId, locationIdentifier2);

            // Act
            var result = sectionLocation.MatchOn(mockSectionLocation.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MatchOnNonSectionLocationParameterTest()
        {
            // Arrange
            var sectionLocation = new SectionLocation(guId, locationIdentifier);

            // Act
            var result = sectionLocation.MatchOn(mockLocation.Object);

            // Assert
            Assert.False(result);
        }        
    }
}
