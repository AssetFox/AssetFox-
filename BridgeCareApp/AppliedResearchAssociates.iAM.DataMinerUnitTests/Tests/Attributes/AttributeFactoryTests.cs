using Xunit;
using System;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests.Tests.Attributes
{
    public class AttributeFactoryTests
    {
        AttributeMetaDatum attributeMetaDatum;

        [Fact]
        public void CreateForStringTypeTest()
        {
            Init("STRING", string.Empty);

            var result = AttributeFactory.Create(attributeMetaDatum);
            Assert.NotNull(result);
            Assert.IsType<TextAttribute>(result);
        }

        [Fact]
        public void CreateForNumberTypeTest()
        {
            Init("NUMBER", "0");

            var result = AttributeFactory.Create(attributeMetaDatum);
            Assert.NotNull(result);
            Assert.IsType<NumericAttribute>(result);
        }


        [Fact]
        public void CreateForNumberInvalidValueTest()
        {
            Init("NUMBER", string.Empty);
            Assert.Throws<InvalidCastException>(() => AttributeFactory.Create(attributeMetaDatum));
        }

        [Fact]
        public void CreateForNoTypeTest()
        {
            Init("", string.Empty);
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
