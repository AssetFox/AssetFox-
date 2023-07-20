using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.Analysis;

public static class OptimizationStrategyExtensions
{
    public static bool UsesRemainingLife(this OptimizationStrategy optimizationStrategy) => optimizationStrategy == OptimizationStrategy.RemainingLife || optimizationStrategy == OptimizationStrategy.RemainingLifeToCostRatio;
}
