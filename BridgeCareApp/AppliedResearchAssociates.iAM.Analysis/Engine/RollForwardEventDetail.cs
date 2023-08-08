using System;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public sealed record RollForwardEventDetail(int Year, Guid AssetId, string AssetName, string CommittedProject);
