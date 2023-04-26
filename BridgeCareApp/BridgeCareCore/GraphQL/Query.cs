using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting;
using HotChocolate;
using HotChocolate.Data;

namespace BridgeCareCore.GraphQL
{
    public class Query
    {
        [UseFiltering]
        [UseSorting]
        public List<SimulationDTO> GetSimulations([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork) =>
            _unitOfWork.SimulationRepo.GetAllScenario();

        [UseFiltering]
        [UseSorting]
        /// <summary>
        /// Method that GetSimulationsOutput query uses in order to retrieve simulation output object via simulation id.
        /// Was created due GetSimulationsOutput query giving a simulation error when simulation isn't received beforehand.
        /// </summary>
        /// <param name="simulationId">The simulation id passed in.</param>
        /// <returns>Simulation object based on simulation id.</returns>
        public SimulationOutput GetSimulationOutputs([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork, string simulationId) 
            {
            _unitOfWork.SimulationRepo.GetSimulation(new Guid(simulationId));
            var simulationOutput = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(new Guid(simulationId));
            return simulationOutput;
            }
            

    }
    
}
