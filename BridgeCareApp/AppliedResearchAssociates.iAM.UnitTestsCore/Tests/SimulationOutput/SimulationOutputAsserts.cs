﻿using System;
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
            var trimmedSerializedOutput = splitSerializeOutput.Select(str => ExtractTrailingDecimal(str)).ToList();
            var trimmedSerializedLoaded = splitSerializeLoaded.Select(str => ExtractTrailingDecimal(str)).ToList();
            trimmedSerializedOutput.Sort((pair1, pair2) => StringDecimalPairComparer.Compare(pair1, pair2));
            trimmedSerializedLoaded.Sort((pair1, pair2) => StringDecimalPairComparer.Compare(pair1, pair2));
            if (trimmedSerializedLoaded != trimmedSerializedOutput)
            {
                var expectedLength = expectedSerializedOutput.Length;
                var actualLength = actualSerializedOutput.Length;
                for (int i = 0; i < trimmedSerializedOutput.Count; i++)
                {
                    var outputI = trimmedSerializedOutput[i];
                    var loadedI = trimmedSerializedLoaded[i];
                    Assert.Equal(outputI.String, loadedI.String);
                    var decimalDifference = outputI.Decimal - loadedI.Decimal;
                    var absDecimalDifference = Math.Abs(decimalDifference);
                    Assert.True(absDecimalDifference < 0.01m);
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

        private static StringDecimalPair ExtractTrailingDecimal(string str)
        {
            var trimmed = str.Trim().TrimEnd(',');
            var returnValue = StringDecimalRoundoff.ExtractTrailingDecimal(trimmed);
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
