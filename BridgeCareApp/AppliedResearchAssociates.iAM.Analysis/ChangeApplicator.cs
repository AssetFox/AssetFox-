using System;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed class ChangeApplicator
{
    internal ChangeApplicator(Action action, double? number)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
        Number = number;
    }

    public Action Action { get; }

    public double? Number { get; }
}
