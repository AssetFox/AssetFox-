using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.GraphQL;

namespace BridgeCareCore.Services
{
    public class SimulationCloningService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SimulationCloningService(IUnitOfWork unitOfWork)
        {
                _unitOfWork = unitOfWork;
        }
        public void Clone(CloneSimulationDTO dto)
        {
            // load it
            var query = new Query();
            var simulationId = dto.scenarioId.ToString();
            var completeSimulation = query.GetSimulation(_unitOfWork, simulationId);

            // do the clone
            var cloneSimulation = CompleteSimulationCloner.Clone(completeSimulation);

            // save it


        }
    }
}
