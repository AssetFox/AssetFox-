using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers.Extensions;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationOutputAsserts
    {
        public static void AssertCouldRepresentSameSimulationOutput(string expectedSerializedOutput, string actualSerializedOutput)
        {
            var splitSerializeOutput = StringExtensions.ToLines(expectedSerializedOutput);
            var splitSerializeLoaded = StringExtensions.ToLines(actualSerializedOutput);
            var trimmedSerializedOutput = splitSerializeOutput.Select(str => TrimIrrelevantPortionsAndRoundoff(str)).ToList();
            var trimmedSerializedLoaded = splitSerializeLoaded.Select(str => TrimIrrelevantPortionsAndRoundoff(str)).ToList();
            trimmedSerializedOutput.Sort();
            trimmedSerializedLoaded.Sort();
            if (trimmedSerializedLoaded != trimmedSerializedOutput)
            {
                var expectedLength = expectedSerializedOutput.Length;
                var actualLength = actualSerializedOutput.Length;
                for (int i = 0; i < trimmedSerializedOutput.Count; i++)
                {
                    var outputI = trimmedSerializedOutput[i];
                    var loadedI = trimmedSerializedLoaded[i];
                    Assert.Equal(outputI, loadedI);
                }
            }
        }

        private static bool EndsWithDecimal(string str)
        {
            var length = str.Length;
            var index = length - 1;
            while (index > 0)
            {
                var c = str[index];
                if (c == '.')
                {
                    return true;
                }
                if (c >= '0' && c <= '9')
                {
                    index--;
                    continue;
                }
                return false;
            }
            return false;
        }

        private static string TrimIrrelevantPortionsAndRoundoff(string str)
        {
            var trimmed = str.Trim().TrimEnd(',');
            var returnValue = StringDecimalRoundoff.RoundoffTrailingDecimal(trimmed, 2);
            return returnValue;
        }

        public static void CouldBeEquivalent(SimulationOutput expected, SimulationOutput actual)
        {
            var serializeExpected = JsonConvert.SerializeObject(expected, Formatting.Indented);
            var serializeActual = JsonConvert.SerializeObject(actual, Formatting.Indented);
            AssertCouldRepresentSameSimulationOutput(serializeExpected, serializeActual);
        }

    }
}
