using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;

public class SimulationOutputJsonRepository : ISimulationOutputJsonRepository
{
    private readonly UnitOfDataPersistenceWork _unitOfWork;

    public SimulationOutputJsonRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public List<SimulationOutputJsonDTO> GetSimulationOutputViaJson(Guid simulationId)
    {
        var simulationOutputJsons = new List<SimulationOutputJsonDTO>();
        var simulationOutputJsonEntities = _unitOfWork.Context.SimulationOutputJson.Where(_ => _.SimulationId == simulationId)?.ToList() ?? new();

        // TO DTOs
        foreach (var entity in simulationOutputJsonEntities)
        {
            var dto = new SimulationOutputJsonDTO
            {
                Id = entity.Id,
                Output = entity.Output,
                OutputType = (SimulationOutputEnum)entity.OutputType,
            };
            simulationOutputJsons.Add(dto);
        }

        return simulationOutputJsons;
    }
}
