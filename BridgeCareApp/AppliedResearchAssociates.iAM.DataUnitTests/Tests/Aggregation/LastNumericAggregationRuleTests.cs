using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;
using Xunit;
using Moq;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Aggregation
{
    public class LastNumericAggregationRuleTests
    {
        private readonly Mock<Attribute> mockAttribute;
        private readonly SectionLocation sectionLocation;

        public LastNumericAggregationRuleTests()
        {
            mockAttribute = new Mock<Attribute>(
                Guid.Empty,
                CommonTestParameterValues.Name,
                AttributeTypeNames.String,
                CommonTestParameterValues.RuleType,
                CommonTestParameterValues.TestCommand,
                ConnectionType.MSSQL,
                CommonTestParameterValues.ConnectionString,
                Guid.Empty,
                false,
                false
            );
            sectionLocation = new SectionLocation(Guid.Empty, CommonTestParameterValues.LocationIdentifier1);
        }

        [Fact]
        public void ApplyWithSingleDatum()
        {
            // Arrange
            var expectedValue = CommonTestParameterValues.DoubleValue;
            var data = new List<AttributeDatum<double>>();
            data.Add(new AttributeDatum<double>(Guid.Empty, null, expectedValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            var lastRule = new LastNumericAggregationRule();

            // Act
            var result = lastRule.Apply(data, mockAttribute.Object).ToList();

            // Assert
            Assert.Single(result);
            var resultItem = result.FirstOrDefault();
            Assert.Equal(mockAttribute.Object, resultItem.attribute);
            Assert.Equal(CommonTestParameterValues.TimeStamp.Year, resultItem.Item2.year);
            Assert.Equal(expectedValue, resultItem.Item2.value);
        }

        [Fact]
        public void ApplyWithSingleYearMultipleData()
        {
            // Arrange
            var FebTimestamp = new DateTime(CommonTestParameterValues.TimeStamp.Year, 2, 1);
            var JunTimestamp = new DateTime(CommonTestParameterValues.TimeStamp.Year, 6, 1);
            var expectedValue = CommonTestParameterValues.DoubleValue;
            var data = new List<AttributeDatum<double>>();
            data.Add(new AttributeDatum<double>(Guid.Empty, null, CommonTestParameterValues.DoubleValue2, sectionLocation, FebTimestamp));
            data.Add(new AttributeDatum<double>(Guid.Empty, null, expectedValue, sectionLocation, JunTimestamp));
            var lastRule = new LastNumericAggregationRule();

            // Act
            var result = lastRule.Apply(data, mockAttribute.Object).ToList();

            // Assert
            Assert.Single(result);
            var resultItem = result.FirstOrDefault();
            Assert.Equal(mockAttribute.Object, resultItem.attribute);
            Assert.Equal(CommonTestParameterValues.TimeStamp.Year, resultItem.Item2.year);
            Assert.Equal(expectedValue, resultItem.Item2.value);
        }

        [Fact]
        public void ApplyWithMultipleYearsData()
        {
            // Arrange
            var priorYearTime = CommonTestParameterValues.TimeStamp.AddYears(-1);
            var data = new List<AttributeDatum<double>>();
            data.Add(new AttributeDatum<double>(Guid.Empty, null, CommonTestParameterValues.DoubleValue2, sectionLocation, priorYearTime));
            data.Add(new AttributeDatum<double>(Guid.Empty, null, CommonTestParameterValues.DoubleValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            var lastRule = new LastNumericAggregationRule();

            // Act
            var result = lastRule.Apply(data, mockAttribute.Object).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            foreach (var datum in result)
            {
                Assert.Equal(mockAttribute.Object, datum.attribute);
            }
            Assert.Equal(CommonTestParameterValues.DoubleValue2, result.Single(_ => _.Item2.year == priorYearTime.Year).Item2.value);
            Assert.Equal(CommonTestParameterValues.DoubleValue, result.Single(_ => _.Item2.year == CommonTestParameterValues.TimeStamp.Year).Item2.value);
        }
    }
}
