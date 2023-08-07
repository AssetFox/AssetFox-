using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSInventorySectionsReportPrimaryFactory : IReportFactory
    {
        public string Name => "PAMSInventoryLookupSections(P)";
        public static string reportTypeIdentifier = "PRIMARY";

        public IReport Create(IUnitOfWork uow, ReportIndexDTO results, IHubService hubService)
        {
            return new PAMSInventorySectionsReport(uow, Name, results);
        }
    }

    public class PAMSInventorySectionsReportRawFactory : IReportFactory
    {
        public string Name => "PAMSInventoryLookupSections(R)";
        string reportTypeIdentifier = "RAW";

        public IReport Create(IUnitOfWork uow, ReportIndexDTO results, IHubService hubService)
        {
            return new PAMSInventorySectionsReport(uow, Name, results);
        }
    }

}
