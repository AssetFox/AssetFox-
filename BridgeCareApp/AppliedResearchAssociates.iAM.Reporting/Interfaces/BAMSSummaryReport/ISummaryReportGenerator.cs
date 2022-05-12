using System;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport
{
    public interface ISummaryReportGenerator
    {
        void GenerateReport(Guid networkId, Guid simulationId);

        byte[] FetchFromFileLocation(Guid networkId, Guid simulationId);
    }
}
