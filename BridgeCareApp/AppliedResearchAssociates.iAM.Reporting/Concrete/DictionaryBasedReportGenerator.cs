using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class DictionaryBasedReportGenerator : IReportGenerator
    {
        private Dictionary<string, Type> _reportLookup;
        private UnitOfDataPersistenceWork _dataRepository;

        public DictionaryBasedReportGenerator(UnitOfDataPersistenceWork dataRepository, ReportLookupLibrary lookupLibrary)
        {
            _dataRepository = dataRepository;
            _reportLookup = lookupLibrary.Lookup;
        }

        /// <summary>
        /// Create a spoecific report
        /// </summary>
        /// <param name="reportName">String representing a report name</param>
        /// <returns>Report object</returns>
        public async Task<IReport> Generate(string reportName)
        {
            return await Generate(reportName, null);
        }

        /// <summary>
        /// Specific generator used to recreate reports from data persistence
        /// </summary>
        private async Task<IReport> Generate(string reportName, ReportIndex results)
        {
            if (!_reportLookup.ContainsKey(reportName))
            {
                var failure = new FailureReport();
                await failure.Run($"No report was found with the name {reportName}");
                return failure;
            }

            if (typeof(IReport).IsAssignableFrom(_reportLookup[reportName]))
            {
                IReport generatedReport;
                try
                {
                    generatedReport = (IReport)Activator.CreateInstance(_reportLookup[reportName], _dataRepository, reportName, results);
                }
                catch
                {
                    generatedReport = new FailureReport();
                    await generatedReport.Run($"{reportName} did not have a constructor with repository and name parameters.");
                }
                return generatedReport;
            }
            else
            {
                var failure = new FailureReport();
                await failure.Run($"{reportName} is not a valid report.");
                return failure;
            }
        }

        /// <summary>
        /// Cleanup any expired reports in the _dataRepository
        /// </summary>
        public void Cleanup()
        {
            _dataRepository.ReportIndexRepository.DeleteExpiredReports();
        }

        public List<ReportListItem> GetAllReportsForScenario(Guid simulationId)
        {
            var reportList = _dataRepository.ReportIndexRepository.GetAllForScenario(simulationId);
            var itemList = new List<ReportListItem>();
            foreach (var item in reportList)
            {
                var listEntry = new ReportListItem()
                {
                    ReportId = item.ID,
                    ReportName = item.ReportTypeName
                };
                itemList.Add(listEntry);
            }
            return itemList;
        }

        public async Task<IReport> GetExisting(Guid reportId)
        {
            IReport validReport;
            var reportInformation = _dataRepository.ReportIndexRepository.Get(reportId);
            if (reportInformation == null) return null;

            if (_reportLookup.ContainsKey(reportInformation.ReportTypeName))
            {
                validReport = await Generate(reportInformation.ReportTypeName, reportInformation);
                validReport.ID = reportInformation.ID;
                validReport.SimulationID = reportInformation.SimulationID;
                return validReport;
            }
            else
            {
                return null;
            }
        }
    }
}
