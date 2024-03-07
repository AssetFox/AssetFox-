using System;

namespace AppliedResearchAssociates.iAM.Analysis;

public sealed record ConsequenceApplicator(Attribute Target, Action Change, double? NewValue);
