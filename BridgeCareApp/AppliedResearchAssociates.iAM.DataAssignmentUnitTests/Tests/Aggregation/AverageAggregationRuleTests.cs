﻿using Xunit;
using System;
using AppliedResearchAssociates.iAM.DataAssignmentUnitTests.TestUtils;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataMiner;
using Moq;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using System.Linq;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests.Tests.Aggregation
{
    public class AverageAggregationRuleTests
    {
        private readonly Guid guId = Guid.Empty;
        private readonly Mock<Attribute> mockAttribute;
        List<IAttributeDatum> attributeData;
        private readonly SectionLocation sectionLocation;

        public AverageAggregationRuleTests()
        {
            attributeData = new List<IAttributeDatum>();
            sectionLocation = new SectionLocation(guId, CommonTestParameterValues.LocationIdentifier1);
            mockAttribute = new Mock<Attribute>(guId, CommonTestParameterValues.Name, AttributeTypeNames.StringType, CommonTestParameterValues.RuleType, CommonTestParameterValues.TestCommand, DataMiner.ConnectionType.MSSQL, CommonTestParameterValues.ConnectionString, false, false);
        }

        [Fact]
        public void ApplyWithSingleYearData()
        {
            // Arrange
            attributeData.Add(new AttributeDatum<double>(guId, null, CommonTestParameterValues.DoubleValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            var averageAggregationRule = new AverageAggregationRule();
            var expectedValue = CommonTestParameterValues.DoubleValue;

            //Act            
            var result = averageAggregationRule.Apply(attributeData, mockAttribute.Object);

            //Assert            
            var resultItems = ((IEnumerable<(Attribute attribute, (int year, double value))>)result)?.ToList();
            Assert.True(resultItems != null && resultItems.Count == 1);
            var resultItem = resultItems.FirstOrDefault();
            Assert.Equal(resultItem.attribute, mockAttribute.Object);
            Assert.Equal(resultItem.Item2.year, attributeData[0].TimeStamp.Year);
            Assert.Equal(resultItem.Item2.value, expectedValue);
        }

        [Fact]
        public void ApplyWithSignleYearMultipleRecordsData()
        {
            // Arrange            
            attributeData.Add(new AttributeDatum<double>(guId, null, CommonTestParameterValues.DoubleValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            attributeData.Add(new AttributeDatum<double>(guId, null, CommonTestParameterValues.DoubleValue2, sectionLocation, CommonTestParameterValues.TimeStamp));
            var averageAggregationRule = new AverageAggregationRule();
            var expectedValue = attributeData.Select(_ => ((AttributeDatum<double>)_).Value).Average();

            //Act            
            var result = averageAggregationRule.Apply(attributeData, mockAttribute.Object);

            //Assert            
            var resultItems = ((IEnumerable<(Attribute attribute, (int year, double value))>)result)?.ToList();
            Assert.True(resultItems != null && resultItems.Count == 1);
            var resultItem = resultItems.FirstOrDefault();
            Assert.Equal(resultItem.attribute, mockAttribute.Object);
            Assert.Equal(resultItem.Item2.year, attributeData[0].TimeStamp.Year);
            Assert.Equal(resultItem.Item2.value, expectedValue);
        }

        [Fact]
        public void ApplyWithDistinctYearsData()
        {
            // Arrange            
            attributeData.Add(new AttributeDatum<double>(guId, null, CommonTestParameterValues.DoubleValue, sectionLocation, CommonTestParameterValues.TimeStamp));
            attributeData.Add(new AttributeDatum<double>(guId, null, CommonTestParameterValues.DoubleValue2, sectionLocation, CommonTestParameterValues.TimeStamp.AddYears(-1)));
            var averageAggregationRule = new AverageAggregationRule();
            var expectedValue1 = CommonTestParameterValues.DoubleValue;
            var expectedValue2 = CommonTestParameterValues.DoubleValue2;

            //Act            
            var result = averageAggregationRule.Apply(attributeData, mockAttribute.Object);

            //Assert            
            var resultItems = ((IEnumerable<(Attribute attribute, (int year, double value))>)result)?.ToList();
            Assert.True(resultItems != null && resultItems.Count == 2);
            Assert.Contains(resultItems, _ => _.attribute == mockAttribute.Object);            
            Assert.Contains(resultItems, _ => _.Item2 == (attributeData[0].TimeStamp.Year, expectedValue1));
            Assert.Contains(resultItems, _ => _.Item2 == (attributeData[0].TimeStamp.Year - 1, expectedValue2));
        }
    }
}