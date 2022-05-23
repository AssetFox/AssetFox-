using Xunit;
using System;
using AppliedResearchAssociates.iAM.DataAssignmentUnitTests.TestUtils;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests.Tests.Networking
{
    public class NetworkFactoryTests
    {
        private readonly Guid guId = Guid.Empty;
        private readonly SectionLocation sectionLocation;
        List<IAttributeDatum> attributeData;

        public NetworkFactoryTests()
        {
            sectionLocation = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier1);
            attributeData = new List<IAttributeDatum>();            
        }

        [Fact]
        public void CreateNetworkFromAttributeDataRecordsTest()
        {
            // Arrange
            attributeData.Add(new AttributeDatum<string>(guId, null, CommonTestParameterValues.StringValue, sectionLocation, CommonTestParameterValues.TimeStamp));

            // Act
            var result = NetworkFactory.CreateNetworkFromAttributeDataRecords(attributeData, CommonTestParameterValues.DefaultEquation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.MaintainableAssets.Count);
        }
    }
}
