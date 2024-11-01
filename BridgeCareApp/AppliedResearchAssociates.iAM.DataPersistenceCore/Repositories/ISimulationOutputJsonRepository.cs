using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

public interface ISimulationOutputJsonRepository
{
   List<SimulationOutputJsonDTO> GetSimulationOutputViaJson(Guid simulationId);
}
