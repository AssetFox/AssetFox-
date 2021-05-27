using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Static;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public static class SimulationLogReport
    {
        private static int PrefixLength = "Warning: ".Length;

        public static string ToPrefix(SimulationLogStatus status) => status switch
        {
            SimulationLogStatus.Error => "Error: ",
            SimulationLogStatus.Warning => "Warning: ",
            SimulationLogStatus.Information => "Info: ",
            _ => throw new NotImplementedException($"Unknown status {status}"),
        };


        public static string ToLogLine(SimulationLogDTO dto)
        {
            var prefix = ToPrefix((SimulationLogStatus)dto.Status);
            var paddedPrefix = prefix.PadRight(PrefixLength);
            return $"{paddedPrefix}{dto.Message}";
        }

        public static string ToLog(IEnumerable<SimulationLogDTO> dtos)
        {
            var builder = new StringBuilder();
            foreach (var dto in dtos)
            {
                builder.AppendLine(ToLogLine(dto));
            }
            return builder.ToString();
        }
    }
}
