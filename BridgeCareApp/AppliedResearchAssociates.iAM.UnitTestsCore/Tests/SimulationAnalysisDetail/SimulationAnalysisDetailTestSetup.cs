using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationAnalysisDetailTestSetup
    {
        public static void CreateAnalysisDetail(UnitOfDataPersistenceWork unitOfWork, Guid simulationId)
        {
            var analysisDetail = SimulationAnalysisDetailDtos.ForSimulation(simulationId);
            unitOfWork.SimulationAnalysisDetailRepo.UpsertSimulationAnalysisDetail(analysisDetail);
        }
    }
}
