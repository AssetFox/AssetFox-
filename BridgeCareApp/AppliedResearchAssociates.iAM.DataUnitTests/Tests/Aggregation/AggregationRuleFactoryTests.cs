using Xunit;
using Moq;
using System;
using Attribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using AppliedResearchAssociates.iAM.Data.Aggregation;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Aggregation
{
    public class AggregationRuleFactoryTests
    {
        private Mock<Attribute> mockAttribute;
        private readonly Guid guId = Guid.Empty;
        private const string InvalidAggregationRuleType = "OTHER";

        [Fact]
        public void CreateAverageRuleTest()
        {
            // Arrange
            Init("Number","Average");

            // Act
            var result = AggregationRuleFactory.CreateNumericRule(mockAttribute.Object);

            // Assert
            Assert.IsType<AverageAggregationRule>(result);
        }

        [Fact]
        public void CreateNumericLastRuleTest()
        {
            // Arrange
            Init("Number", "Last");

            // Act
            var result = AggregationRuleFactory.CreateNumericRule(mockAttribute.Object);

            // Assert
            Assert.IsType<LastNumericAggregationRule>(result);
        }

        [Fact(Skip = "Invalid caught by attribute constructor")]
        public void CreateNumericRuleExceptionTest()
        {
            // Arrange
            Init("Number", InvalidAggregationRuleType);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => AggregationRuleFactory.CreateNumericRule(mockAttribute.Object));
        }

        [Fact]
        public void CreatePredominantTextRuleTest()
        {
            // Arrange
            Init("String", "Predominant");

            // Act
            var result = AggregationRuleFactory.CreateTextRule(mockAttribute.Object);

            // Assert
            Assert.IsType<PredominantTextAggregationRule>(result);
        }

        [Fact]
        public void CreateTextLastRuleTest()
        {
            // Arrange
            Init("String", "Last");

            // Act
            var result = AggregationRuleFactory.CreateTextRule(mockAttribute.Object);

            // Assert
            Assert.IsType<LastTextAggregationRule>(result);
        }

        [Fact(Skip = "Invalid caught by attribute constructor")]
        public void CreateTextRuleExceptionTest()
        {
            // Arrange
            Init("String", InvalidAggregationRuleType);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => AggregationRuleFactory.CreateTextRule(mockAttribute.Object));
        }

        public void Init(string dataType, string aggregationRuleType)
        {
            mockAttribute = new Mock<Attribute>(guId, null, dataType, aggregationRuleType, null, Data.ConnectionType.MSSQL, null, Guid.Empty, false, false);
        }
    }
}
