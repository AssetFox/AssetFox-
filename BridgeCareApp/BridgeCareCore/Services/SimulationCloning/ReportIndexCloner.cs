using System;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SimulationCloning;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class ReportIndexCloner
    {
        internal static ReportIndexDTO Clone(ReportIndexDTO reportIndex)
        {
            var clone = new ReportIndexDTO
            {
                CreationDate = reportIndex.CreationDate,
                ExpirationDate = reportIndex.ExpirationDate,
                Result = reportIndex.Result,
                SimulationId = reportIndex.SimulationId,
                Type = reportIndex.Type,
            };
            return clone;
        }

    }
}
