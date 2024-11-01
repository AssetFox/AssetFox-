using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Analysis;

internal static class Static
{
    public static IEnumerable<int> Count(int from = 0, int by = 1)
    {
        while (true)
        {
            yield return from;
            from += by;
        }
    }

    public static IEnumerable<int> RangeFromBounds(int start, int end, int stride = 1) => Math.Sign(stride) switch
    {
        1 => TowardPositiveInfinity(start, end, stride),
        -1 => TowardNegativeInfinity(start, end, stride),
        _ => throw new ArgumentException("Stride must be non-zero.", nameof(stride)),
    };

    private static IEnumerable<int> TowardNegativeInfinity(int start, int end, int stride)
    {
        while (start >= end)
        {
            yield return start;
            start += stride;
        }
    }

    private static IEnumerable<int> TowardPositiveInfinity(int start, int end, int stride)
    {
        while (start <= end)
        {
            yield return start;
            start += stride;
        }
    }
}
