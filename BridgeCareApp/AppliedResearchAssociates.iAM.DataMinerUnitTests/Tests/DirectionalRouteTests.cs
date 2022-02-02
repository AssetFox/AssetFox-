using Xunit;
using Moq;
using AppliedResearchAssociates.iAM.DataMiner;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests
{
    public class DirectionalRouteTests
    {
        private readonly Route directionalRoute;
        private Mock<DirectionalRoute> mockDirectionalRoute;
        private const string locationIdentifier = "TestUniqueId";

        public DirectionalRouteTests()
        {
            directionalRoute = new DirectionalRoute(locationIdentifier, Direction.N);
            Init(locationIdentifier, Direction.N);
        }        

        [Fact]
        public void MatchOnTest()
        {
            var doesMatch = directionalRoute.MatchOn(mockDirectionalRoute.Object);

            Assert.True(doesMatch);
        }

        [Fact]
        public void MatchOnOnlyLocationIdentifierTest()
        {
            Init(locationIdentifier, Direction.S);
            var doesMatch = directionalRoute.MatchOn(mockDirectionalRoute.Object);

            Assert.True(doesMatch);
        }

        [Fact]
        public void MatchOnFalseTest()
        {
            Init("TestUniqueId_2", Direction.N);
            var doesMatch = directionalRoute.MatchOn(mockDirectionalRoute.Object);

            Assert.False(doesMatch);
        }

        private void Init(string locationIdentifier, Direction direction)
        {
            mockDirectionalRoute = new Mock<DirectionalRoute>(locationIdentifier, direction);
        }
    }
}
