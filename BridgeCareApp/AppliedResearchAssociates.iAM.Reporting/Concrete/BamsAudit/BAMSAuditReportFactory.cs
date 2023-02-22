using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class BAMSAuditReportFactory : IReportFactory
    {
        public string Name => "BAMSAuditReport";

        public IReport Create(UnitOfDataPersistenceWork uow, ReportIndexDTO results, IHubService hubService)
        {
            var report = new BAMSAuditReport(uow, Name, results, hubService);
            return report;
        }
    }
}
