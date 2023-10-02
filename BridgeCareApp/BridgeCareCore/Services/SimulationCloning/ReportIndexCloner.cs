using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
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

        internal static List<ReportIndexDTO> CloneList(IEnumerable<ReportIndexDTO> reportIndexes)
        {
            var clone = new List<ReportIndexDTO>();
            foreach (var reportIndex in reportIndexes)
            {
                var childClone = Clone(reportIndex);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}
