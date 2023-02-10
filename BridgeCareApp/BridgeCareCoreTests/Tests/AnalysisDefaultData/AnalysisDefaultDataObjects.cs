using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using BridgeCareCore.Models.DefaultData;

namespace BridgeCareCoreTests.Tests
{
    public static class AnalysisDefaultDataObjects
    {
        public static AnalysisDefaultData Default =>
            new AnalysisDefaultData
            {
                OptimizationStrategy = OptimizationStrategy.Benefit,
                SpendingStrategy = SpendingStrategy.NoSpending,
            };
    }
}
