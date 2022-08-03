using Xunit;
using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Networking
{
    public class NetworkFactoryTests
    {
        private readonly Guid sectionLocationId = Guid.Empty;
        private readonly SectionLocation sectionLocation;
        List<IAttributeDatum> attributeData;

        public NetworkFactoryTests()
        {
            sectionLocation = new SectionLocation(sectionLocationId, CommonTestParameterValues.LocationIdentifier1);
            attributeData = new List<IAttributeDatum>();            
        }

        [Fact]
        public void CreateNetworkFromAttributeDataRecords_SectionLocationInDb_CreatesWithMaintainableAsset()
        {
            // Arrange
            attributeData.Add(new AttributeDatum<string>(sectionLocationId, null, CommonTestParameterValues.StringValue, sectionLocation, CommonTestParameterValues.TimeStamp));

            // Act
            var result = NetworkFactory.CreateNetworkFromAttributeDataRecords(attributeData, CommonTestParameterValues.DefaultEquation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MaintainableAssets.Count);
        }
    }
}
