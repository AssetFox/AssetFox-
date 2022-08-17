using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.TestHelpers.Extensions;
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
            var trimmedSerializedOutput = splitSerializeOutput.Select(str => str.Trim().TrimEnd(',')).ToList();
            var trimmedSerializedLoaded = splitSerializeLoaded.Select(str => str.Trim().TrimEnd(',')).ToList();
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
                Assert.Equal(expectedSerializedOutput.Length, actualSerializedOutput.Length);
                Assert.Equal(trimmedSerializedOutput, trimmedSerializedLoaded);
            }
        }

        public static void CouldBeEquivalent(SimulationOutput expected, SimulationOutput actual)
        {
            var serializeExpected = JsonConvert.SerializeObject(expected, Formatting.Indented);
            var serializeActual = JsonConvert.SerializeObject(actual, Formatting.Indented);
            AssertCouldRepresentSameSimulationOutput(serializeExpected, serializeActual);
        }

    }
}
