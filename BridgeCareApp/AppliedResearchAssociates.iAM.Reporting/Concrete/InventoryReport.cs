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
    /// <summary>
    /// Creates HTML that reports on the values of all current attributes for a given asset (i.e., bridge)
    /// </summary>
    /// <remarks>
    /// The parameters object of the run method should resolve to a AppliedResearchAssociates.iAM.Reporting.InventoryParameters object.
    /// Use BRKey = 0 when that parameter is not known and an BMSID = String.Empty when that parameter is not known.  An error will occur
    /// if you send both parameters as known values and they do not exist on the same asset.
    /// </remarks>
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

        private List<SegmentAttributeDatum> segmentData;

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
            string providedKey;
            if (reportKeys.BRKey < 1)
            {
                providedKey = $"BMSID: {reportKeys.BMSID}";
                segmentData = _repository.AssetDataRepository.GetAssetAttributes("BMSID", reportKeys.BMSID);
            }
            else
            {
                providedKey = $"BRKEY:  {reportKeys.BRKey}";
                segmentData = _repository.AssetDataRepository.GetAssetAttributes("BRKEY_", reportKeys.BRKey.ToString());
            }
            if (segmentData.Count() < 1)
            {
                // No data returned for key
                Errors.Add($"Provided key {providedKey} not found");
                return;
            }

            var resultsString = new StringBuilder();
            resultsString.Append("<table>");
            resultsString.Append(CreateHTMLSection("Key Fields", new List<string>() { "BRKEY_", "BMSID" }));
            resultsString.Append(CreateHTMLSection("Location", new List<string>() { "DISTRICT", "COUNTY", "MUNI_CODE", "FEATURE_INTERSECTED", "FEATURE_CARRIED", "LOCATION" }));
            resultsString.Append("</table>");
            Results = resultsString.ToString();
            IsComplete = true;
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
                var BRKeyGuid = _repository.AssetDataRepository.KeyProperties["BRKEY_"].FirstOrDefault(_ => _.KeyValue.Value == parameters.BRKey.ToString());
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

        private string GetAttribute(string attributeName)
        {
            try
            {
                return segmentData.First(_ => _.Name == attributeName).Value;
            }
            catch(Exception e)
            {
                Errors.Add($"Unable to find attribute {attributeName}");
                return "";
            }
        }

        private string CreateHTMLSection(string sectionName, List<string> attributes)
        {
            var sectionString = new StringBuilder($"<tr><th colspan=\"4\">{sectionName}</th><tr>");

            for (int i = 0; i < attributes.Count(); i=i+2)
            {
                if (i == (attributes.Count() - 1))
                {
                    // The attribute list has an odd length, just do one element
                    sectionString.Append($"<tr><td class=\"description\">{attributes[i]}</td><td class=\"data\">{GetAttribute(attributes[i])}</td></tr>");
                }
                else
                {
                    // Do two elements at a time
                    sectionString.Append($"<tr><td class=\"description\">{attributes[i]}</td><td class=\"data\">{GetAttribute(attributes[i])}</td>");
                    sectionString.Append($"<td class=\"description columnsplit\">{attributes[i+1]}</td><td class=\"data\">{GetAttribute(attributes[i+1])}</td></tr>");
                }
            }

            return sectionString.ToString();
        }
    }
}
