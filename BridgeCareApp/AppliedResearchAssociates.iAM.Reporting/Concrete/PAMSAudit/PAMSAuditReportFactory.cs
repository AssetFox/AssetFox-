using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSAuditReportFactory : IReportFactory
    {
        public string Name => "PAMSAuditReport";

        public IReport Create(IUnitOfWork uow, ReportIndexDTO results, IHubService hubService)
        {
            var report = new PAMSAuditReport(uow, Name, results, hubService);
            return report;
        }
    }
}
