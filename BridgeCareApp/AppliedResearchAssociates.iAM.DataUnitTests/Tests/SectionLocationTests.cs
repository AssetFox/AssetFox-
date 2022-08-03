using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.Data;
using System;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests
{
    internal class DummyLocationClass : Location
    {
        public DummyLocationClass(Guid id, string locationIdentifier) : base(id, locationIdentifier)
        {
        }

        public override bool MatchOn(Location location) => throw new NotImplementedException();
    }
    public class SectionLocationTests
    {
        private readonly Guid guId = Guid.Empty;
        private const string locationIdentifier = "TestUniqueId";
        private const string locationIdentifier2 = "TestUniqueId2";
        private SectionLocation sectionLocation;

        public SectionLocationTests()
        {
            sectionLocation = new SectionLocation(guId, locationIdentifier);
        }

        [Fact]
        public void MatchOnTrueTest()
        {
            // Arrange
            var sectionLocation2 = new SectionLocation(guId, locationIdentifier);

            // Act
            var result = sectionLocation2.MatchOn(sectionLocation);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MatchOnFalseTest()
        {
            // Arrange
            var sectionLocation2 = new SectionLocation(guId, locationIdentifier2);

            // Act
            var result = sectionLocation2.MatchOn(sectionLocation);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void MatchOnNonSectionLocationParameterTest()
        {
            // Arrange
            var otherLocation = new Mock<Location>(guId, locationIdentifier);
            Assert.Equal(otherLocation.Object.LocationIdentifier, sectionLocation.LocationIdentifier);

            // Act
            var result = sectionLocation.MatchOn(otherLocation.Object);

            // Assert
            Assert.False(result);
        }        
    }
}
