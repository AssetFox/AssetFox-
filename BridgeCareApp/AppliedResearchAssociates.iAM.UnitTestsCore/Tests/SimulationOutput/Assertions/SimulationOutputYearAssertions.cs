﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using Xunit;
using AppliedResearchAssociates.iAM.Common;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationOutputYearAssertions
    {
        public static void Same(SimulationYearDetail expectedYear, SimulationYearDetail actualYear)
        {
            Assert.Equal(expectedYear.Year, actualYear.Year);
            Assert.Equal(expectedYear.ConditionOfNetwork, actualYear.ConditionOfNetwork);
            Assert.Equal(expectedYear.DeficientConditionGoals.Count, actualYear.DeficientConditionGoals.Count);
            Assert.Equal(expectedYear.TargetConditionGoals.Count, actualYear.TargetConditionGoals.Count);
            Assert.Equal(expectedYear.Assets.Count, actualYear.Assets.Count);
            Assert.Equal(expectedYear.Budgets.Count, actualYear.Budgets.Count);
            var expectedDeficientGoals = expectedYear.DeficientConditionGoals.OrderBy(g => g.AttributeName).ToList();
            var actualDeficientGoals = actualYear.DeficientConditionGoals.OrderBy(g => g.AttributeName).ToList();
            for (int i=0; i<expectedDeficientGoals.Count; i++)
            {
                DeficientConditionGoalDetailAsserts.Same(expectedDeficientGoals[i], actualDeficientGoals[i]);
            }
            expectedYear.Assets.Sort(a => a.AssetId);
            actualYear.Assets.Sort(a => a.AssetId);
            for (int i=0; i<expectedYear.Assets.Count; i++)
            {
                AssetDetailAssertions.Same(expectedYear.Assets[i], actualYear.Assets[i]);
            }
        }
    }
}
