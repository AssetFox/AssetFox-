using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public interface ISummaryReportGenerator
    {
        byte[] GenerateReport(Guid networkId, Guid simulationId, UserInfoDTO userInfo);
    }
}
