using Xunit;
using System;
using AppliedResearchAssociates.iAM.DataAssignmentUnitTests.TestUtils;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataMiner;
using Moq;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAssignment.Networking;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests.Tests.Networking
{
    public class MaintainableAssetTests
    {
        private readonly Guid guId = Guid.Empty;
        private readonly Guid networkId = Guid.NewGuid();
        private readonly Mock<Attribute> mockAttribute;
        List<IAttributeDatum> attributeData;
        private readonly SectionLocation sectionLocation;
        private readonly SectionLocation sectionLocationUnmatched;
        private readonly Mock<AverageAggregationRule> mockAggregationRule;        

        public MaintainableAssetTests()
        {
            attributeData = new List<IAttributeDatum>();
            sectionLocation = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier1);
            sectionLocationUnmatched = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier2);
            mockAttribute = new Mock<Attribute>(guId, CommonTestParameterValues.Name, AttributeTypeNames.StringType, CommonTestParameterValues.RuleType, CommonTestParameterValues.TestCommand, DataMiner.ConnectionType.MSSQL, CommonTestParameterValues.ConnectionString, false, false);
            mockAggregationRule = new Mock<AverageAggregationRule>();
            
        }

        [Fact]
        public void GetAggregatedValuesByYearTest()
        {
            // Assign
            attributeData.Add(new AttributeDatum<double>(guId, mockAttribute.Object, CommonTestParameterValues.DoubleValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            (Attribute attribute, (int year, double value)) attributeYearValueTupple = (mockAttribute.Object, (CommonTestParameterValues.TimeStamp.Year, CommonTestParameterValues.DoubleValue));

            mockAggregationRule.Setup(m => m.Apply(attributeData, mockAttribute.Object)).Returns(new List<(Attribute attribute, (int year, double value))> { attributeYearValueTupple });

            var maintainableAsset = new MaintainableAsset(guId, networkId, sectionLocation, CommonTestParameterValues.DefaultEquation);
            maintainableAsset.AssignAttributeData(attributeData);
            
            // Act
            var result = maintainableAsset.GetAggregatedValuesByYear<double>(mockAttribute.Object, mockAggregationRule.Object);

            // Assert
            Assert.IsType<AggregatedResult<double>>(result);
            Assert.Equal(result.MaintainableAsset, maintainableAsset);
            Assert.Single(result.AggregatedData);
            Assert.Equal(result.AggregatedData.FirstOrDefault(), attributeYearValueTupple);
        }

        [Fact]
        public void AssignAttributeDataMatchedLocationTest()
        {
            // Arrange
            attributeData.Add(new AttributeDatum<double>(guId, mockAttribute.Object, CommonTestParameterValues.DoubleValue, sectionLocation, CommonTestParameterValues.TimeStamp));
                        
            var maintainableAsset = new MaintainableAsset(guId, networkId, sectionLocation, CommonTestParameterValues.DefaultEquation);

            // Act
            maintainableAsset.AssignAttributeData(attributeData);

            // Assert
            Assert.Equal(maintainableAsset.AssignedData, attributeData);
        }

        [Fact]
        public void AssignAttributeDataUnMatchedLocationTest()
        {
            // Arrange
            attributeData.Add(new AttributeDatum<double>(guId, mockAttribute.Object, CommonTestParameterValues.DoubleValue, sectionLocation, CommonTestParameterValues.TimeStamp));

            var maintainableAsset = new MaintainableAsset(guId, networkId, sectionLocationUnmatched, CommonTestParameterValues.DefaultEquation);

            // Act
            maintainableAsset.AssignAttributeData(attributeData);

            // Assert
            Assert.True(maintainableAsset.AssignedData.Count == 0);
        }

        [Fact]
        public void AssignAttributeDataFromDataSourceTest()
        {
            // Assign
            attributeData.Add(new AttributeDatum<double>(guId, mockAttribute.Object, CommonTestParameterValues.DoubleValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            
            var maintainableAsset = new MaintainableAsset(guId, networkId, sectionLocation, CommonTestParameterValues.DefaultEquation);

            // Act
            maintainableAsset.AssignAttributeDataFromDataSource(attributeData);

            // Assert
            Assert.Equal(maintainableAsset.AssignedData, attributeData);
        }
    }
}
