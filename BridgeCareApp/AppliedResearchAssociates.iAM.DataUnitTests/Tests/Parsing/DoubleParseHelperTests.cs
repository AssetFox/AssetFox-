using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Data.Helpers;
using Xunit;

namespace AppliedResearchAssociates.iAM.DataUnitTests.Tests.Parsing
{
    public class DoubleParseHelperTests
    {
        [Fact]
        public void TryParseNullableDouble_PiInputAsString_Pi()
        {
            var piString = Math.PI.ToString();
            var parsePiString = DoubleParseHelper.TryParseNullableDouble(piString);
            Assert.Equal(Math.PI, parsePiString);
        }

        [Fact]
        public void TryParseNullableDouble_InputIsNotDouble_Null()
        {
            var parseHello = DoubleParseHelper.TryParseNullableDouble("hello");
            Assert.Null(parseHello);
        }

        [Fact]
        public void TryParseNullableDouble_NullInput_Null()
        {
            var parseNull = DoubleParseHelper.TryParseNullableDouble(null);
            Assert.Null(parseNull);
        }

        public void TryParseNullableDouble_LongInput_Succeeds()
        {
            long input = 1000000000000000000L;
        }
    }
}
