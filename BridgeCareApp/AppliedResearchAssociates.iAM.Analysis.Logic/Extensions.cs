namespace AppliedResearchAssociates.iAM.Analysis.Logic;

internal static class Extensions
{
    public static decimal RoundToCent(this decimal value) => Math.Round(value, 2);

    /// <summary>
    ///     CP model optimization values (e.g. coefficients and bounds) must be long integers, so we
    ///     convert decimal money values to the $0.01 unit for maximum practical accuracy of money
    ///     values in CP model optimization.
    /// </summary>
    /// <remarks>
    ///     If the converted money values ever approach or go beyond the precision limit of long
    ///     integers, then we might see overflow errors here or in the optimization, in which case
    ///     we might need to convert money values to a coarser unit (for example, $1 or $1,000).
    /// </remarks>
    public static long ToCpModelMoneyInteger(this decimal value) => (long)Math.Round(value * 100);
}
