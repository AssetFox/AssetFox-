using System;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface ISummaryReportGenerator
    {
        byte[] GenerateReport(Guid networkId, Guid simulationId);
    }
}
