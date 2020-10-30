﻿namespace AppliedResearchAssociates.iAM
{
    public static class iAM
    {
        public static bool UsesRemainingLife(this OptimizationStrategy optimizationStrategy) => optimizationStrategy == OptimizationStrategy.RemainingLife || optimizationStrategy == OptimizationStrategy.RemainingLifeToCostRatio;
    }
}