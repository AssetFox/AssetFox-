using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.Data;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests
{
    public class DirectionalRouteTests
    {
        private readonly DirectionalRoute directionalRoute;
        private Mock<DirectionalRoute> mockDirectionalRoute;
        private const string locationIdentifier = "TestUniqueId";
        private const string locationIdentifier2 = "TestUniqueId2";

        public DirectionalRouteTests()
        {
            directionalRoute = new DirectionalRoute(locationIdentifier, Direction.N);
            Init(locationIdentifier, Direction.N);
        }        

        [Fact]
        public void MatchOnTest()
        {
            // Act
            var doesMatch = directionalRoute.MatchOn(mockDirectionalRoute.Object);

            // Assert
            Assert.True(doesMatch);
        }

        [Fact]
        public void MatchOnOnlyLocationIdentifierTest()
        {
            // Arrange
            Init(locationIdentifier, Direction.S);

            // Act
            var doesMatch = directionalRoute.MatchOn(mockDirectionalRoute.Object);

            // Assert
            Assert.True(doesMatch);
        }

        [Fact]
        public void MatchOnFalseTest()
        {
            // Arrange
            Init(locationIdentifier2, Direction.N);

            // Act
            var doesMatch = directionalRoute.MatchOn(mockDirectionalRoute.Object);

            // Assert
            Assert.False(doesMatch);
        }

        private void Init(string locationIdentifier, Direction direction)
        {
            mockDirectionalRoute = new Mock<DirectionalRoute>(locationIdentifier, direction);
        }
    }
}
