﻿using Xunit;
using System;
using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.DataMinerUnitTests.TestUtils;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class AttributeFactoryTests
    {
        AttributeMetaDatum attributeMetaDatum;

        [Fact]
        public void CreateForStringTypeTest()
        {
            // Arrange
            Init(AttributeTypeNames.String, string.Empty);

            // Act
            var result = AttributeFactory.Create(attributeMetaDatum);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TextAttribute>(result);
        }

        [Fact]
        public void CreateForStringTypeWithValueTest()
        {
            // Arrange
            Init(AttributeTypeNames.String, CommonTestParameterValues.StringValue);

            // Act
            var result = AttributeFactory.Create(attributeMetaDatum);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TextAttribute>(result);
        }

        [Fact]
        public void CreateForNumberDoubleTypeTest()
        {
            // Arrange
            Init(AttributeTypeNames.Number, CommonTestParameterValues.DoubleNumber);

            // Act
            var result = AttributeFactory.Create(attributeMetaDatum);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NumericAttribute>(result);
        }

        [Fact]
        public void CreateForNumberIntTypeTest()
        {
            // Arrange
            Init(AttributeTypeNames.Number, CommonTestParameterValues.IntNumber);

            // Act
            var result = AttributeFactory.Create(attributeMetaDatum);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<NumericAttribute>(result);
        }


        [Fact]
        public void CreateForNumberInvalidValueTest()
        {
            // Arrange
            Init(AttributeTypeNames.Number, string.Empty);

            // Act, Assert
            Assert.Throws<InvalidCastException>(() => AttributeFactory.Create(attributeMetaDatum));
        }

        [Fact]
        public void CreateForNoTypeTest()
        {
            // Arrange
            Init(string.Empty, string.Empty);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => AttributeFactory.Create(attributeMetaDatum));
        }

        private void Init(string type, string defaultValue)
        {
            attributeMetaDatum = new AttributeMetaDatum();
            attributeMetaDatum.Type = type;
            attributeMetaDatum.DefaultValue = defaultValue;
        }
    }
}
