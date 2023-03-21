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
    public class BAMSSummaryReportFactory : IReportFactory
    {
        public string Name => "BAMSSummaryReport";

        public IReport Create(IUnitOfWork uow, ReportIndexDTO results, IHubService hubService)
        {
            var report = new BAMSSummaryReport(uow, Name, results, hubService);
            return report;
        }
    }
}
