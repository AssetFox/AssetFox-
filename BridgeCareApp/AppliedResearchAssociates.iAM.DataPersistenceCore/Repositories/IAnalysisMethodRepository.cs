using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAnalysisMethodRepository
    {
        void CreateAnalysisMethod(AnalysisMethod domain, string simulationName);
        AnalysisMethod GetSimulationAnalysisMethod(string simulationName);
    }
}
