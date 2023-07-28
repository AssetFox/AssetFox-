using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting;
using HotChocolate;
using HotChocolate.Data;

namespace BridgeCareCore.GraphQL
{
    /// <summary>
    /// The GraphQL query.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Get all simulations.
        /// </summary>
        /// <param name="_unitOfWork">An instance of a transaction with the database.</param>
        /// <returns>List of all simulations.</returns>
        [UseFiltering]
        [UseSorting]
        public List<SimulationDTO> GetSimulations([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork) => _unitOfWork.SimulationRepo.GetAllScenario();

        /// <summary>
        /// Get a simulation object and its repositories.
        /// </summary>
        /// <param name="_unitOfWork">An instance of a transaction with the database.</param>
        /// <param name="simulationId">The simulation ID passed in.</param>
        /// <returns>Simulation object with access to all its repositories, based on simulation ID.</returns>
        [UseFiltering]
        [UseSorting]
        public CompleteSimulationDTO GetSimulation([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork, string simulationId)
        {
            var fullSimulation = _unitOfWork.CompleteSimulationRepo.GetSimulation(simulationId);

            return fullSimulation;
        }



        /// <summary>
        /// Get the simulation results of an analysis.
        /// </summary>
        /// <param name="_unitOfWork">An instance of a transaction with the database.</param>
        /// <param name="simulationId">The simulation ID passed in.</param>
        /// <returns>Simulation output object, based on simulation ID.</returns>
        [UseFiltering]
        [UseSorting]
        public SimulationOutput GetSimulationOutput([Service(ServiceKind.Synchronized)] IUnitOfWork _unitOfWork, string simulationId)
        {
            var simulationGuid = new Guid(simulationId);
            _unitOfWork.SimulationRepo.GetSimulation(simulationGuid);
            var simulationOutput = _unitOfWork.SimulationOutputRepo.GetSimulationOutputViaJson(simulationGuid);
            return simulationOutput;
        }
    }
}
