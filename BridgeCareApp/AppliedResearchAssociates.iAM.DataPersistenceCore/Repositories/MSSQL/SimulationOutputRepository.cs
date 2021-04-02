using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationOutputRepository : ISimulationOutputRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public SimulationOutputRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateSimulationOutput(Guid simulationId, SimulationOutput simulationOutput)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking()
                .Single(_ => _.Id == simulationId);

            if (simulationOutput == null)
            {
                throw new InvalidOperationException($"No results found for simulation {simulationEntity.Name}. Please ensure that the simulation analysis has been run.");
            }

            var settings = new Newtonsoft.Json.Converters.StringEnumConverter();
            var simulationOutputString = JsonConvert.SerializeObject(simulationOutput, settings);

            _unitOfWork.Context.Upsert(
                new SimulationOutputEntity {SimulationId = simulationId, Output = simulationOutputString},
                _ => _.SimulationId == simulationId, _unitOfWork.UserEntity?.Id);
        }

        public void GetSimulationOutput(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException($"Found no simulation having id {simulation.Id}");
            }

            if (_unitOfWork.Context.SimulationOutput.Any(_ => _.SimulationId == simulation.Id))
            {
                _unitOfWork.Context.SimulationOutput.Single(_ => _.SimulationId == simulation.Id)
                    .FillSimulationResults(simulation);
            }
        }

        public SimulationOutput GetSimulationOutput(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"Found no simulation having id {simulationId}");
            }

            if (!_unitOfWork.Context.SimulationOutput.Any(_ => _.SimulationId == simulationId))
            {
                throw new RowNotInTableException($"No simulation analysis results were found for simulation having id {simulationId}. Please ensure that the simulation analysis has been run.");
            }

            var simulationOutputString = _unitOfWork.Context.SimulationOutput.Single(_ => _.SimulationId == simulationId).Output;

            return JsonConvert.DeserializeObject<SimulationOutput>(simulationOutputString, new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });
        }
    }
}
