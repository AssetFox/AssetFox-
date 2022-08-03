using Xunit;
using System;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data;
using Moq;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using System.Linq;
using AppliedResearchAssociates.iAM.DataUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Aggregation
{
    public class PredominantAggregationRuleTests
    {
        private readonly Guid guId = Guid.Empty;
        private readonly TextAttribute textAttribute;
        List<IAttributeDatum> attributeData;
        private readonly SectionLocation sectionLocation;

        public PredominantAggregationRuleTests()
        {
            attributeData = new List<IAttributeDatum>();
            sectionLocation = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier1);
            textAttribute = new TextAttribute(
                "defaultValue",
                Guid.Empty,
                AttributeTypeNames.String,
                CommonTestParameterValues.RuleType,
                CommonTestParameterValues.TestCommand,
                ConnectionType.MSSQL,
                CommonTestParameterValues.ConnectionString,
                false,
                false,
                Guid.Empty
                );
        }

        [Fact]
        public void ApplyWithSingleYearData()
        {
            // Arrange
            attributeData.Add(new AttributeDatum<string>(guId, null, CommonTestParameterValues.StringValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            var predominantAggregationRule = new PredominantAggregationRule();
            var expectedValue = CommonTestParameterValues.StringValue;

            //Act            
            var result = predominantAggregationRule.Apply(attributeData, textAttribute);

            //Assert            
            var resultItems = ((IEnumerable<(Attribute attribute, (int year, string value))>)result)?.ToList();
            Assert.True(resultItems != null && resultItems.Count == 1);
            var resultItem = resultItems.FirstOrDefault();
            Assert.Equal(resultItem.attribute, textAttribute);
            Assert.Equal(resultItem.Item2.year, attributeData[0].TimeStamp.Year);
            Assert.Equal(resultItem.Item2.value, expectedValue);
        }

        [Fact]
        public void ApplyWithSignleYearMultipleRecordsData()
        {
            // Arrange            
            attributeData.Add(new AttributeDatum<string>(guId, null, CommonTestParameterValues.StringValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            attributeData.Add(new AttributeDatum<string>(guId, null, CommonTestParameterValues.StringValue2, sectionLocation, CommonTestParameterValues.TimeStamp));
            var predominantAggregationRule = new PredominantAggregationRule();
            var expectedValue = CommonTestParameterValues.StringValue;

            //Act            
            var result = predominantAggregationRule.Apply(attributeData, textAttribute);

            //Assert            
            var resultItems = ((IEnumerable<(Attribute attribute, (int year, string value))>)result)?.ToList();
            Assert.True(resultItems != null && resultItems.Count == 1);
            var resultItem = resultItems.FirstOrDefault();
            Assert.Equal(resultItem.attribute, textAttribute);
            Assert.Equal(resultItem.Item2.year, attributeData[0].TimeStamp.Year);
            Assert.Equal(resultItem.Item2.value, expectedValue);
        }

        [Fact]
        public void ApplyWithDistinctYearsData()
        {
            // Arrange            
            attributeData.Add(new AttributeDatum<string>(guId, null, CommonTestParameterValues.StringValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            attributeData.Add(new AttributeDatum<string>(guId, null, CommonTestParameterValues.StringValue2, sectionLocation, CommonTestParameterValues.TimeStamp.AddYears(-1)));
            var predominantAggregationRule = new PredominantAggregationRule();
            var expectedValue1 = CommonTestParameterValues.StringValue;
            var expectedValue2 = CommonTestParameterValues.StringValue2;

            //Act            
            var result = predominantAggregationRule.Apply(attributeData, textAttribute);

            //Assert            
            var resultItems = ((IEnumerable<(Attribute attribute, (int year, string value))>)result)?.ToList();
            Assert.True(resultItems != null && resultItems.Count == 2);
            Assert.Contains(resultItems, _ => _.attribute == textAttribute);
            Assert.Contains(resultItems, _ => _.Item2 == (attributeData[0].TimeStamp.Year, expectedValue1));
            Assert.Contains(resultItems, _ => _.Item2 == (attributeData[0].TimeStamp.Year - 1, expectedValue2));
        }
    }
}
