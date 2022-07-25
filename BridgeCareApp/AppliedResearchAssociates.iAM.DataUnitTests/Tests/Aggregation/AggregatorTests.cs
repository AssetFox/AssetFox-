using Xunit;
using System;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Aggregation
{
    public class AggregatorTests
    {
        List<IAttributeDatum> attributeData;
        List<MaintainableAsset> maintainableAssets = new List<MaintainableAsset>();
        private readonly Guid guId = Guid.Empty;
        private readonly SectionLocation sectionLocation1;
        private readonly SectionLocation sectionLocation2;

        public AggregatorTests()
        {
            attributeData = new List<IAttributeDatum>();
            sectionLocation1 = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier1);
            sectionLocation2 = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier2);
        }

        [Fact]
        public void AssignNoMatchingAttributeDataToMaintainableAssetTest()
        {
            // Arrange
            attributeData.Add(new AttributeDatum<string>(guId, null, CommonTestParameterValues.StringValue, sectionLocation1, CommonTestParameterValues.TimeStamp));            
            maintainableAssets.Add(new MaintainableAsset(guId, guId, sectionLocation2, string.Empty));

            //Act
            Aggregator.AssignAttributeDataToMaintainableAsset(attributeData, maintainableAssets);

            //Assert
            Assert.True(maintainableAssets[0].AssignedData.Count == 0);
        }

        [Fact]
        public void AssignMatchingAttributeDataToMaintainableAssetTest()
        {
            // Arrange
            attributeData.Add(new AttributeDatum<string>(guId, null, CommonTestParameterValues.StringValue, sectionLocation1, CommonTestParameterValues.TimeStamp));
            maintainableAssets.Add(new MaintainableAsset(guId, guId, sectionLocation1, string.Empty));

            //Act
            Aggregator.AssignAttributeDataToMaintainableAsset(attributeData, maintainableAssets);

            //Assert
            Assert.True(maintainableAssets[0].AssignedData.Count == 1);
            Assert.Equal(attributeData, maintainableAssets[0].AssignedData);
        }
    }
}
