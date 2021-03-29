using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Newtonsoft.Json;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using System.Text;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class InventoryReport : IReport
    {
        private UnitOfDataPersistenceWork _repository;
        private Guid _networkId;

        public Guid ID { get; set; }
        public Guid? SimulationID { get => null; set { } }
        public string Results { get; private set; }
        public ReportType Type => ReportType.HTML;
        public string ReportTypeName { get; private set; }
        public List<string> Errors { get; private set; }
        public bool IsComplete { get; private set; }
        public string Status { get; private set; }

        private InventoryParameters _failedQuery = new InventoryParameters() { BRKey = -1, BMSID = String.Empty };

        public InventoryReport(UnitOfDataPersistenceWork repository, string name, ReportIndexEntity results)
        {
            _repository = repository;
            ReportTypeName = name;
            // results is ignored for this report

            ID = Guid.NewGuid();
            Errors = new List<string>();
            Status = "Report definition created.";
            Results = String.Empty;
            IsComplete = false;
            _networkId = _repository.NetworkRepo.GetPennDotNetwork().Id;
        }

        public async Task Run(string parameters)
        {
            var reportKeys = Parse(parameters);
            if (reportKeys.BRKey == -1) return; // report failed due to bad parameters
            if (!Validate(reportKeys)) return; // report failed due to validation

            // Check if asset actually exists
            List<SegmentAttributeDatum> segmentData;
            string providedKey;
            if (reportKeys.BRKey < 1)
            {
                providedKey = $"BMSID: {reportKeys.BMSID}";
                segmentData = _repository.AssetDataRepository.GetAssetAttributes("BMSID", reportKeys.BMSID);
            }
            else
            {
                providedKey = $"BRKey:  {reportKeys.BRKey}";
                segmentData = _repository.AssetDataRepository.GetAssetAttributes("BRKey", reportKeys.BRKey.ToString());
            }
            if (segmentData.Count() < 1)
            {
                // No data returned for key
                Errors.Add($"Provided key {providedKey} not found");
                return;
            }

            var resultsString = new StringBuilder();
            resultsString.Append($"<p>BRKey: {segmentData.First(_ => _.Name == "BRKey").Value}");
            resultsString.Append($"<p>BMSID: {segmentData.First(_ => _.Name == "BMSID").Value}");
            Results = resultsString.ToString();
            return;
        }

        private InventoryParameters Parse(string parameters)
        {
            try
            {
                InventoryParameters query = JsonConvert.DeserializeObject<InventoryParameters>(parameters);
                if (query == null)
                {
                    Errors.Add($"Unable to run.  No query parameters provided in request body.");
                    return _failedQuery;
                }
                return query;
            }
            catch(Exception e)
            {
                Errors.Add($"Failed to parse JSON in request body due to {e.Message}");
                return _failedQuery;
            }
        }

        private bool Validate(InventoryParameters parameters)
        {
            if (parameters.BRKey < 1 && String.IsNullOrEmpty(parameters.BMSID))
            {
                // No parameters provided
                return false;
            }

            if (parameters.BRKey > 0 && !String.IsNullOrEmpty(parameters.BMSID))
            {
                // Both parameters provided.  Check to see if they are the same asset
                var BRKeyGuid = _repository.AssetDataRepository.KeyProperties["BRKey"].FirstOrDefault(_ => _.KeyValue.Value == parameters.BRKey.ToString());
                if (BRKeyGuid == null)
                {
                    // BRKey was not found
                    Errors.Add($"Unable to find BRKey {parameters.BRKey}.  Did not attempt to find {parameters.BMSID}");
                    return false;
                }
                var BMSIDGuid = _repository.AssetDataRepository.KeyProperties["BMSID"].FirstOrDefault(_ => _.KeyValue.Value == parameters.BMSID);
                if (BMSIDGuid == null)
                {
                    // BMSID was not found
                    Errors.Add($"Unable to find BMSID {parameters.BMSID}.  Will not use {parameters.BRKey}");
                    return false;
                }
                if (BRKeyGuid.SegmentId != BMSIDGuid.SegmentId)
                {
                    // Keys were provided for two different assets
                    Errors.Add($"The BRKey {parameters.BRKey} and BMSID {parameters.BMSID}.  No report will be provided");
                    return false;
                }
            }

            // All checks passed
            return true;
        }

    }
}
