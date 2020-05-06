﻿using System;

namespace AppliedResearchAssociates.iAM
{
    public sealed class TargetConditionGoal : ConditionGoal
    {
        public double Target { get; }

        public int? Year { get; }

        public override bool IsMet(double actual) => Attribute.IsDecreasingWithDeterioration ? actual >= Target : actual <= Target;
    }
}
