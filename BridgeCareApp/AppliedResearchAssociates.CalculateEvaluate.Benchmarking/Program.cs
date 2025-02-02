﻿using BenchmarkDotNet.Running;

namespace AppliedResearchAssociates.CalculateEvaluate.Benchmarking
{
    internal static class Program
    {
        private static void Main()
        {
            _ = BenchmarkRunner.Run<CalculateEvaluateCompilerBenchmarks>();
            _ = BenchmarkRunner.Run<LegacyCalculateEvaluateBenchmarks>();
        }
    }
}
