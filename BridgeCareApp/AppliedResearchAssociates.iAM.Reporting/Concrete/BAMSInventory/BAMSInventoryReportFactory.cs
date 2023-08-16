using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class BAMSInventoryPrimaryReportFactory : IReportFactory
    {
        public string Name => "BAMSInventoryLookup(P)";

        public IReport Create(IUnitOfWork uow, ReportIndexDTO results, IHubService hubService)
        {
            return new BAMSInventoryReport(uow, Name, results);
        }
    }

    public class BAMSInventoryRawReportFactory : IReportFactory
    {
        public string Name => "BAMSInventoryLookup(R)";

        public IReport Create(IUnitOfWork uow, ReportIndexDTO results, IHubService hubService)
        {
            return new BAMSInventoryReport(uow, Name, results);
        }
    }

}
