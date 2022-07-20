﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class DictionaryBasedReportGenerator : IReportGenerator
    {
        private readonly ReportLookupLibrary _reportLookup;
        private UnitOfDataPersistenceWork _dataRepository;
        private readonly IHubService _hubService;

        public DictionaryBasedReportGenerator(UnitOfDataPersistenceWork dataRepository, ReportLookupLibrary lookupLibrary, IHubService hubService)
        {
            _dataRepository = dataRepository;
            _reportLookup = lookupLibrary;
            _hubService = hubService ?? throw new ArgumentNullException(nameof(hubService));
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
        private async Task<IReport> Generate(string reportName, ReportIndexDTO results)
        {
            var generatedReport = _reportLookup.GetReport(reportName);
            if (generatedReport is FailureReport failure) { 
                await failure.Run($"No report was found with the name {reportName}");
                return await Task.FromResult(failure);
            }
            await generatedReport.Run($"{reportName} did not have a constructor with repository and name parameters.");
                return generatedReport;
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
                    ReportId = item.Id,
                    ReportName = item.Type
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

            if (_reportLookup.CanGenerateReport(reportInformation.Type))
            {
                validReport = await Generate(reportInformation.Type, reportInformation);
                validReport.ID = reportInformation.Id;
                validReport.SimulationID = reportInformation.SimulationId;
                return validReport;
            }
            else
            {
                return null;
            }
        }
    }
}
