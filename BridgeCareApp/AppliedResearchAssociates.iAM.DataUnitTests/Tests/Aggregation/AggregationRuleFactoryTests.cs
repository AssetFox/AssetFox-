using Xunit;
using Moq;
using System;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using AppliedResearchAssociates.iAM.Data.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignmentUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests.Tests.Aggregation
{
    public class AggregationRuleFactoryTests
    {
        private Mock<Attribute> mockAttribute;
        private readonly Guid guId = Guid.Empty;
        private const string InvalidAggregationRuleType = "OTHER";

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
            Init(InvalidAggregationRuleType);

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
            Init(InvalidAggregationRuleType);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => AggregationRuleFactory.CreateTextRule(mockAttribute.Object));
        }

        public void Init(string aggregationRuleType)
        {
            mockAttribute = new Mock<Attribute>(guId, null, null, aggregationRuleType, null, Data.ConnectionType.MSSQL, null, Guid.Empty, false, false);
        }
    }
}
