using Xunit;
using Moq;
using System;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using AppliedResearchAssociates.iAM.DataMinerUnitTests.TestUtils;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests
{
    public class AggregationRuleFactoryTests
    {
        private Mock<Attribute> mockAttribute;
        private readonly Guid guid = Guid.Empty;

        [Fact]
        public void CreateNumericRuleTest()
        {
            // Arrange
            Init(AggregationRuleTypeNames.Average);

            // Act
            var result = AggregationRuleFactory.CreateNumericRule(mockAttribute.Object);

            // Assert
            Assert.IsType<AverageAggregationRule>(result);
        }

        [Fact]
        public void CreateNumericRuleExceptionTest()
        {
            // Arrange
            Init(AggregationRuleTypeNames.Other);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => AggregationRuleFactory.CreateNumericRule(mockAttribute.Object));
        }

        [Fact]
        public void CreateTextRuleTest()
        {
            // Arrange
            Init(AggregationRuleTypeNames.Predominant);

            // Act
            var result = AggregationRuleFactory.CreateTextRule(mockAttribute.Object);

            // Assert
            Assert.IsType<PredominantAggregationRule>(result);
        }

        [Fact]
        public void CreateTextRuleExceptionTest()
        {
            // Arrange
            Init(AggregationRuleTypeNames.Other);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => AggregationRuleFactory.CreateTextRule(mockAttribute.Object));
        }

        public void Init(string aggregationRuleType)
        {
            mockAttribute = new Mock<Attribute>(guid, null, null, aggregationRuleType, null, DataMiner.ConnectionType.MSSQL, null, false, false);
        }
    }
}
