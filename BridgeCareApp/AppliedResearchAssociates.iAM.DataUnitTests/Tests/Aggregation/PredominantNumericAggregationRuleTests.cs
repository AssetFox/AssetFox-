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
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Aggregation
{
    public class PredominantNumericAggregationRuleTests
    {
        private readonly Guid guId = Guid.Empty;
        private readonly NumericAttribute numericAttribute;
        List<IAttributeDatum> attributeData;
        private readonly SectionLocation sectionLocation;

        public PredominantNumericAggregationRuleTests()
        {
            attributeData = new List<IAttributeDatum>();
            sectionLocation = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier1);
            numericAttribute = new NumericAttribute(
                1.0,
                100.0,
                0.0,
                guId,
                AttributeTypeNames.Number,
                "Predominant",
                CommonTestParameterValues.TestCommand,
                ConnectionType.MSSQL,
                CommonTestParameterValues.ConnectionString,
                false,
                false,
                guId);
        }

        [Fact]
        public void ApplyWithSingleYearData()
        {
            // Arrange
            attributeData.Add(new AttributeDatum<double>(guId, numericAttribute, CommonTestParameterValues.DoubleValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            var predominantAggregationRule = new PredominantNumericAggregationRule();
            var expectedValue = CommonTestParameterValues.DoubleValue;

            //Act            
            var result = predominantAggregationRule.Apply(attributeData, numericAttribute);

            //Assert            
            var resultItems = ((IEnumerable<(Attribute attribute, (int year, double value))>)result)?.ToList();
            Assert.Single(resultItems);
            var resultItem = resultItems.FirstOrDefault();
            Assert.Equal(numericAttribute, resultItem.attribute);
            Assert.Equal(CommonTestParameterValues.TimeStamp.Year, resultItem.Item2.year);
            Assert.Equal(expectedValue, resultItem.Item2.value);
        }

        [Fact]
        public void ApplyWithSingleYearMultipleRecordsData()
        {
            // Arrange
            var expectedValue = CommonTestParameterValues.DoubleValue;
            var datumDate = new DateTime(CommonTestParameterValues.TimeStamp.Year, 2, 1);
            attributeData.Add(new AttributeDatum<double>(guId, numericAttribute, expectedValue, sectionLocation, datumDate));
            datumDate = new DateTime(CommonTestParameterValues.TimeStamp.Year, 3, 5);
            attributeData.Add(new AttributeDatum<double>(guId, numericAttribute, expectedValue, sectionLocation, datumDate));
            datumDate = new DateTime(CommonTestParameterValues.TimeStamp.Year, 4, 7);
            attributeData.Add(new AttributeDatum<double>(guId, numericAttribute, CommonTestParameterValues.DoubleValue2, sectionLocation, datumDate));
            var predominantAggregationRule = new PredominantNumericAggregationRule();

            // Act
            var result = predominantAggregationRule.Apply(attributeData, numericAttribute);

            // Assert
            var resultItems = ((IEnumerable<(Attribute attribute, (int year, double value))>)result)?.ToList();
            Assert.Single(resultItems);
            var resultItem = resultItems.FirstOrDefault();
            Assert.Equal(numericAttribute, resultItem.attribute);
            Assert.Equal(CommonTestParameterValues.TimeStamp.Year, resultItem.Item2.year);
            Assert.Equal(expectedValue, resultItem.Item2.value);
        }

        [Fact]
        public void ApplyWithDistinctYearsData()
        {
            // Arrange
            var expectedValue1 = CommonTestParameterValues.DoubleValue;
            var expectedValue2 = CommonTestParameterValues.DoubleValue2;
            attributeData.Add(new AttributeDatum<double>(guId, numericAttribute, expectedValue1, sectionLocation, CommonTestParameterValues.TimeStamp));
            attributeData.Add(new AttributeDatum<double>(guId, numericAttribute, expectedValue2, sectionLocation, CommonTestParameterValues.TimeStamp.AddYears(-1)));
            var predominantAggregationRule = new PredominantNumericAggregationRule();

            //Act            
            var result = predominantAggregationRule.Apply(attributeData, numericAttribute);

            //Assert            
            var resultItems = ((IEnumerable<(Attribute attribute, (int year, double value))>)result)?.ToList();
            Assert.True(resultItems != null && resultItems.Count == 2);
            Assert.Contains(resultItems, _ => _.attribute == numericAttribute);
            Assert.Contains(resultItems, _ => _.Item2 == (attributeData[0].TimeStamp.Year, expectedValue1));
            Assert.Contains(resultItems, _ => _.Item2 == (attributeData[0].TimeStamp.Year - 1, expectedValue2));
        }
    }
}
