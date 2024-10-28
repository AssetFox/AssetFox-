using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System;

namespace BridgeCareCore.Services.SimulationCloning;

internal class SimulationOutputJsonCloner
{
    internal static SimulationOutputJsonDTO Clone(SimulationOutputJsonDTO simulationOutputJson, Guid ownerId)
    {        
        var clone = new SimulationOutputJsonDTO
        {
            Id = Guid.NewGuid(),
            Output=simulationOutputJson.Output,
            OutputType = simulationOutputJson.OutputType,            
        };
        return clone;
    }

    internal static List<SimulationOutputJsonDTO> CloneList(IEnumerable<SimulationOutputJsonDTO> simulationOutputJsons, Guid ownerId)
    {
        var clone = new List<SimulationOutputJsonDTO>();
        foreach (var simulationOutputJson in simulationOutputJsons)
        {
            var childClone = Clone(simulationOutputJson, ownerId);
            clone.Add(childClone);
        }
        return clone;

    }
}
