using System;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface ISummaryReportGenerator
    {
        void GenerateReport(Guid networkId, Guid simulationId);

        byte[] FetchFromFileLocation(Guid networkId, Guid simulationId);
    }
}
