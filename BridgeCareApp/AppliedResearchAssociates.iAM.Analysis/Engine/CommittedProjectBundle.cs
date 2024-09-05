using System.Collections.Generic;
using System.Linq;

namespace AppliedResearchAssociates.iAM.Analysis.Engine;

internal sealed class CommittedProjectBundle : TreatmentBundle
{
    public CommittedProjectBundle(IEnumerable<CommittedProject> bundledProjects) : base(bundledProjects)
    {
    }

    public IEnumerable<CommittedProject> BundledProjects => BundledTreatments.Cast<CommittedProject>();
}
