﻿using System;

namespace AppliedResearchAssociates.iAMCore
{
    internal sealed class ChangeApplicator
    {
        public ChangeApplicator(Action action, double? number)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Number = number;
        }

        public Action Action { get; }

        public double? Number { get; }
    }
}
