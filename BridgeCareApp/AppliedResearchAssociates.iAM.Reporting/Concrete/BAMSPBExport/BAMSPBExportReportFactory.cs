using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class BAMSPBExportReportFactory : IReportFactory
    {
        public string Name => "BAMSPBExportReport";

        public IReport Create(UnitOfDataPersistenceWork uow, ReportIndexDTO results, IHubService hubService)
        {
            var report = new BAMSPBExportReport(uow, Name, results, hubService);
            return report;
        }
    }
}
