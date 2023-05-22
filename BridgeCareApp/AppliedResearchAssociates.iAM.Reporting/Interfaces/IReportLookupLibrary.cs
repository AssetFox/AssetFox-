using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public interface IReportLookupLibrary
    {
        bool CanGenerateReport(string type);
        IReport GetReport(
            string reportName,
            DataPersistenceCore.UnitOfWork.UnitOfDataPersistenceWork _dataRepository,
            DTOs.ReportIndexDTO results,
            Hubs.Interfaces.IHubService _hubService);
        IList<IReportFactory> ReportList { get; }
    }
}
