using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class AuditReportFactory : IReportFactory
    {
        public string Name => "AuditReport";

        public IReport Create(UnitOfDataPersistenceWork uow, ReportIndexDTO results, IHubService hubService)
        {
            var report = new AuditReport(uow, Name, results, hubService);
            return report;
        }
    }
}
