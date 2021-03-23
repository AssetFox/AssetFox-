using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IReportIndexRepository
    {
        bool Add(ReportIndex report);
        ReportIndex Get(Guid reportId);
        bool DeleteReport(Guid reportId);
        bool DeleteAllScenarioReports(Guid scenarioId);
        /// <summary>
        /// Deletes any ReportIndex with an ExpirationDate before the current date
        /// </summary>
        bool DeleteExpiredReports();
    }
}
