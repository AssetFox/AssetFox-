using System;
using System.Collections.Generic;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface ISummaryReportGenerator
    {
        byte[] GenerateReport(Guid simulationId, Guid networkId);
    }
}
