using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class DictionaryBasedReportGenerator : IReportGenerator
    {
        private Dictionary<string, Type> _reportLookup;
        private UnitOfDataPersistenceWork _dataRepository;

        public DictionaryBasedReportGenerator(UnitOfDataPersistenceWork dataRepository, Dictionary<string, Type> lookup)
        {
            _dataRepository = dataRepository;
            _reportLookup = lookup;
        }

        /// <summary>
        /// Create a spoecific report
        /// </summary>
        /// <param name="reportName">String representing a report name</param>
        /// <returns>Report object</returns>
        public async Task<IReport> Generate(string reportName)
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
                    generatedReport = (IReport)Activator.CreateInstance(_reportLookup[reportName], _dataRepository, reportName);
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
    }
}
