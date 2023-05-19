using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting
{
    /// <summary>
    /// Provides data to lookup an object type based on a name
    /// </summary>
    /// <remarks>
    /// This object is required to wrap the Dictionary during lookup so that it can be configured during IoC Container
    /// </remarks>
    public class ReportLookupLibrary : IReportLookupLibrary
    {
        private readonly List<IReportFactory> _reportFactories;

        public ReportLookupLibrary(List<IReportFactory> reportFactories)
        {
            _reportFactories = reportFactories;
        }

        public Dictionary<string, Type> Lookup { get; set; }

        private IReportFactory GetReportFactory(string reportName)
        {
            var factory = _reportFactories.FirstOrDefault(rf => rf.Name == reportName);
            return factory;
        }

        public IReport GetReport(string reportName, DataPersistenceCore.UnitOfWork.UnitOfDataPersistenceWork uow, DTOs.ReportIndexDTO results, Hubs.Interfaces.IHubService hubService)
        {
            var factory = GetReportFactory(reportName);
            if (factory == null)
            {
                return new FailureReport();
            }
            var report = factory.Create(uow, results, hubService);
            return report;
        }
        public bool CanGenerateReport(string type) {
            var factory = GetReportFactory(type);
            return factory != null;
        }
        public IList<IReportFactory> ReportList
        {
            get { return _reportFactories; }

        }
    }
}
