using Xunit;
using AppliedResearchAssociates.iAM.Data;
using System;
using AppliedResearchAssociates.iAM.Data.Attributes;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests
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
            // Act
            var locationResult = LocationBuilder.CreateLocation(locationIdentifier, start, end);

            // Assert
            Assert.NotNull(locationResult);
            Assert.IsType<LinearLocation>(locationResult);
            Assert.IsType<SimpleRoute>(((LinearLocation)locationResult).Route);
        }

        [Fact]
        public void CreateLocationLinearWithDirectionalRoute()
        {
            // Act
            var locationResult = LocationBuilder.CreateLocation(locationIdentifier, start, end, direction);

            // Assert
            Assert.NotNull(locationResult);
            Assert.IsType<LinearLocation>(locationResult);
            Assert.IsType<DirectionalRoute>(((LinearLocation)locationResult).Route);
        }

        [Fact]
        public void CreateLocationGis()
        {
            // Act
            var locationResult = LocationBuilder.CreateLocation(locationIdentifier, wellKnownText: wellKnownText);

            // Assert
            Assert.NotNull(locationResult);
            Assert.IsType<GisLocation>(locationResult);
        }

        [Fact]
        public void CreateLocationSection()
        {
            // Act
            var locationResult = LocationBuilder.CreateLocation(locationIdentifier);

            // Assert
            Assert.NotNull(locationResult);
            Assert.IsType<SectionLocation>(locationResult);
        }

        [Fact]
        public void CreateLocationInvalidOperationException()
        {
            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => LocationBuilder.CreateLocation(null));
        }
    }
}
