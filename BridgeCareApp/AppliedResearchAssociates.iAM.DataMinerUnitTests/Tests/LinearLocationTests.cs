using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataMiner;
using System;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests
{
    public class LinearLocationTests
    {
        private Mock<DirectionalRoute> mockDirectionalRoute;
        private Mock<Location> mockLocation;
        private Mock<LinearLocation> mockLinearLocation;
        private readonly Guid guId = Guid.Empty;
        private const double start = 0;
        private const double end = 10;
        private const string locationIdentifier = "TestUniqueId";

        public LinearLocationTests()
        {
            mockDirectionalRoute = new Mock<DirectionalRoute>(locationIdentifier, Direction.E);
            mockLinearLocation = new Mock<LinearLocation>(guId, mockDirectionalRoute.Object, locationIdentifier, start, end + 1);
            mockLocation = new Mock<Location>(guId, locationIdentifier);
        }

        [Fact]
        public void MatchOnTrueTest()
        {
            // Arrange
            mockDirectionalRoute.Setup(m => m.MatchOn(It.IsAny<Route>())).Returns(true);
            var linearLocation = new LinearLocation(guId, mockDirectionalRoute.Object, locationIdentifier, start, end);

            // Act
            var result = linearLocation.MatchOn(mockLinearLocation.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MatchOnFalseTest()
        {
            // Arrange
            var linearLocation = new LinearLocation(guId, mockDirectionalRoute.Object, locationIdentifier, start, end);

            // Act
            var result = linearLocation.MatchOn(mockLocation.Object);

            // Assert
            Assert.False(result);
        }
    }
}
