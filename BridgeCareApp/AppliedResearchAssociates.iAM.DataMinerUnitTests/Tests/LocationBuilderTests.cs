using Xunit;
using AppliedResearchAssociates.iAM.DataMiner;
using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests
{
    public class LocationBuilderTests
    {
        private const string locationIdentifier = "TestUniqueId";
        private const double start = 0;
        private const double end = 10;
        private const Direction direction = Direction.W;
        private const string wellKnownText = "TestWellKnownText";

        [Fact]
        public void CreateLocationLinearWithSimpleRoute()
        {
            var locationResult = LocationBuilder.CreateLocation(locationIdentifier, start, end);

            Assert.NotNull(locationResult);
            Assert.IsType<LinearLocation>(locationResult);
            Assert.IsType<SimpleRoute>(((LinearLocation)locationResult).Route);
        }

        [Fact]
        public void CreateLocationLinearWithDirectionalRoute()
        {
            var locationResult = LocationBuilder.CreateLocation(locationIdentifier, start, end, direction);

            Assert.NotNull(locationResult);
            Assert.IsType<LinearLocation>(locationResult);
            Assert.IsType<DirectionalRoute>(((LinearLocation)locationResult).Route);
        }

        [Fact]
        public void CreateLocationGis()
        {
            var locationResult = LocationBuilder.CreateLocation(locationIdentifier, wellKnownText: wellKnownText);

            Assert.NotNull(locationResult);
            Assert.IsType<GisLocation>(locationResult);
        }

        [Fact]
        public void CreateLocationSection()
        {
            var locationResult = LocationBuilder.CreateLocation(locationIdentifier);

            Assert.NotNull(locationResult);
            Assert.IsType<SectionLocation>(locationResult);
        }

        [Fact]
        public void CreateLocationInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => LocationBuilder.CreateLocation(null));
        }
    }
}
