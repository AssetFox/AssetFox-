using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting.Interfaces
{
    public interface IReportFactory
    {
        string Name { get; }

        IReport Create(IUnitOfWork uow, ReportIndexDTO results, IHubService hubService, string suffix = "");
    }
}
