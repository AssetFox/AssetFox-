using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSInventorySegmentsReportFactory: IReportFactory
    {
        public string Name => "PAMSInventoryLookupSegments";

        public IReport Create(IUnitOfWork uow, ReportIndexDTO results, IHubService hubService)
        {
            return new PAMSInventorySegmentsReport(uow, Name, results);
        }
    }
}
