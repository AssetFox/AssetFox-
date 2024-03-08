using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSDistressProgressionReportFactory : IReportFactory
    {
        public string Name => "PAMSDistressProgressionReport";

        public IReport Create(IUnitOfWork uow, ReportIndexDTO results, IHubService hubService, string suffix = "")
        {
            var report = new PAMSDistressProgressionReport(uow, Name, results, hubService);
            return report;
        }
    }
}
