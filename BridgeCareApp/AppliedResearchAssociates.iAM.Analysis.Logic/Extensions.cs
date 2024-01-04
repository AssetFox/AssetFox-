namespace AppliedResearchAssociates.iAM.Analysis.Logic;

internal static class Extensions
{
    public static decimal RoundToCent(this decimal value) => Math.Round(value, 2);
}
