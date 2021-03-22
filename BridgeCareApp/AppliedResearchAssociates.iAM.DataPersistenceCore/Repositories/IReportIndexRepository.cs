using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Reporting;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IReportIndexRepository
    {
        bool Add(IReport report);
        IReport Get(Guid reportId);
        bool DeleteReport(Guid reportId);
        bool DeleteAllScenarioReports(Guid scenarioId);
        /// <summary>
        /// Deletes any ReportIndex with an ExpirationDate before the current date
        /// </summary>
        bool DeleteExpiredReports();
    }
}
