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
        private const string DEFAULT_VALUE = "N";

        private UnitOfDataPersistenceWork _unitofwork;
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
        private InventoryParameters segmentIds;

        public InventoryReport(UnitOfDataPersistenceWork uow, string name, ReportIndexEntity results)
        {
            _unitofwork = uow;
            ReportTypeName = name;
            // results is ignored for this report

            ID = Guid.NewGuid();
            Errors = new List<string>();
            Status = "Report definition created.";
            Results = String.Empty;
            IsComplete = false;
            _networkId = _unitofwork.NetworkRepo.GetPennDotNetwork().Id;
        }

        public async Task Run(string parameters)
        {
            segmentIds = Parse(parameters);
            if (segmentIds.BRKey == -1 && string.IsNullOrEmpty(segmentIds.BMSID)) return; // report failed due to bad parameters
            if (!Validate(segmentIds)) return; // report failed due to validation

            // Check if asset actually exists
            string providedKey;
            if (segmentIds.BRKey < 1)
            {
                providedKey = $"BMSID: {segmentIds.BMSID}";
                segmentData = _unitofwork.AssetDataRepository.GetAssetAttributes("BMSID", segmentIds.BMSID);
            }
            else
            {
                providedKey = $"BRKEY:  {segmentIds.BRKey}";
                segmentData = _unitofwork.AssetDataRepository.GetAssetAttributes("BRKEY", segmentIds.BRKey.ToString());
            }
            if (segmentData.Count() < 1)
            {
                // No data returned for key
                Errors.Add($"Provided key {providedKey} not found");
                return;
            }

            var resultsString = new StringBuilder();
            resultsString.Append("<table>");
            resultsString.Append(CreateHTMLSection("Key Fields", new List<string>() { "BRKEY", "BMSID" }));
            resultsString.Append(CreateHTMLSection("Location", new List<string>() { "DISTRICT", "COUNTY", "MUNI_CODE", "FEATURE_INTERSECTED", "FEATURE_CARRIED", "LOCATION" }));
            resultsString.Append(CreateHTMLSection("Age and Service", new List<string>() { "YEAR_BUILT", "YEAR_RECON", "FEATURE_CARRIED", "FEATURE_INTERSECTED" }));
            resultsString.Append(CreateHTMLSection("Management", new List<string>() { "OWNER_CODE", "MPO", "SUBM_AGENCY", "NBISLEN", "HBRR_ELIG", "BUS_PLAN_NETWORK" }));
            resultsString.Append(CreateHTMLSection("Deck Information", new List<string>() { "DECKGEEOM", "DECKSURF_TYPE", "DECK_WIDTH" }));
            resultsString.Append(CreateHTMLSection("Span Information", new List<string>() { "NUMBER_SPANS", "MATERIAL_TYPE", "SPANTYPE", "LENGTH", "DECK_AREA", "FUNC_CLASS" }));
            resultsString.Append(CreateHTMLSection("Current Condition", new List<string>() { "DECK", "SUP", "SUB", "CULV" }));
            resultsString.Append(CreateHTMLSection("Previous Conditions", new List<string>() { "DECK", "SUP", "SUB", "CULV" }, true));
            resultsString.Append(CreateHTMLSection("Risk Scores", new List<string>() { "RISK_SCORE" }));
            resultsString.Append(CreateHTMLSection("Posting", new List<string>() { "POST_STATUS_DATE", "POST_STATUS", "POST_LIMIT_COMB", "POST_LIMIT_WEIGHT" }));
            resultsString.Append(CreateHTMLSection("Roadway Information", new List<string>() { "ADTTOTAL", "FUNC_CLASS" }));
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
                var BRKeyGuid = _unitofwork.AssetDataRepository.KeyProperties["BRKEY"].FirstOrDefault(_ => _.KeyValue.Value == parameters.BRKey.ToString());
                if (BRKeyGuid == null)
                {
                    // BRKey was not found
                    Errors.Add($"Unable to find BRKey {parameters.BRKey}.  Did not attempt to find {parameters.BMSID}");
                    return false;
                }
                var BMSIDGuid = _unitofwork.AssetDataRepository.KeyProperties["BMSID"].FirstOrDefault(_ => _.KeyValue.Value == parameters.BMSID);
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

        private string GetAttribute(string attributeName, bool previous)
        {
            string result;

            if (!previous)
            {
                try
                {
                    return segmentData.First(_ => _.Name == attributeName).Value;
                }
                catch (Exception e)
                {
                    Errors.Add($"Unable to find attribute {attributeName}");
                    return "";
                }
            }
            else
            {
                Dictionary<int, SegmentAttributeDatum> attrubuteValueHistory;
                if (segmentIds.BRKey < 1)
                {
                    attrubuteValueHistory = _unitofwork.AssetDataRepository.GetAttributeValueHistory("BMSID", segmentIds.BMSID, attributeName);
                }
                else
                {
                    attrubuteValueHistory = _unitofwork.AssetDataRepository.GetAttributeValueHistory("BRKEY", segmentIds.BRKey.ToString(), attributeName);
                }
                if (attrubuteValueHistory.Count < 2) return DEFAULT_VALUE;  // The default value is returned if there is either no values OR one value (the previous value is still unknown)

                return attrubuteValueHistory.OrderByDescending(_ => _.Key).ElementAt(1).Value.Value;  // The first Value comes from the Dictionary, the second comes from the SegmentAttributeDatum
            }
        }

        private string CreateHTMLSection(string sectionName, List<string> attributes, bool previous = false)
        {
            var sectionString = new StringBuilder($"<tr><th colspan=\"4\">{sectionName}</th></tr>");

            for (int i = 0; i < attributes.Count(); i=i+2)
            {
                if (i == (attributes.Count() - 1))
                {
                    // The attribute list has an odd length, just do one element
                    sectionString.Append($"<tr><td class=\"description\">{attributes[i]}</td><td class=\"data\">{GetAttribute(attributes[i], previous)}</td></tr>");
                }
                else
                {
                    // Do two elements at a time
                    sectionString.Append($"<tr><td class=\"description\">{attributes[i]}</td><td class=\"data\">{GetAttribute(attributes[i], previous)}</td>");
                    sectionString.Append($"<td class=\"description columnsplit\">{attributes[i+1]}</td><td class=\"data\">{GetAttribute(attributes[i+1], previous)}</td></tr>");
                }
            }

            return sectionString.ToString();
        }
    }
}
