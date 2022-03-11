using Xunit;
using Moq;
using System;
using Attribute = AppliedResearchAssociates.iAM.DataMiner.Attributes.Attribute;
using AppliedResearchAssociates.iAM.DataAssignment.Aggregation;
using AppliedResearchAssociates.iAM.DataAssignmentUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataAssignmentUnitTests.Tests.Aggregation
{
    public class AggregationRuleFactoryTests
    {
        private Mock<Attribute> mockAttribute;
        private readonly Guid guId = Guid.Empty;

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
            Init(OtherTypeName.Other);

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
            Init(OtherTypeName.Other);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => AggregationRuleFactory.CreateTextRule(mockAttribute.Object));
        }

        public void Init(string aggregationRuleType)
        {
            mockAttribute = new Mock<Attribute>(guId, null, null, aggregationRuleType, null, DataMiner.ConnectionType.MSSQL, null, false, false);
        }
    }
}
