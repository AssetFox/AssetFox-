using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using Newtonsoft.Json;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DTOs;
using System.Threading;
using AppliedResearchAssociates.iAM.Common.Logging;

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
    public class BAMSInventoryReport : IReport
    {
        private const string DEFAULT_VALUE = "N";
        private const int DEFAULT_COLUMNS = 2;

        private IUnitOfWork _unitofwork;
        private Guid _networkId;
        private Dictionary<string, AttributeDescription> _fieldDescriptions;

        public Guid ID { get; set; }
        public Guid? SimulationID { get => null; set { } }
        public string Results { get; private set; }
        public ReportType Type => ReportType.HTML;
        public string ReportTypeName { get; private set; }
        public List<string> Errors { get; private set; }
        public bool IsComplete { get; private set; }
        public string Status { get; private set; }
        public string Criteria { get; set; }
        private InventoryParameters _failedQuery = new InventoryParameters { keyProperties = new List<string> { string.Empty, "-1" } };

        private List<SegmentAttributeDatum> segmentData;
        private InventoryParameters segmentIds;

        public BAMSInventoryReport(IUnitOfWork uow, string name, ReportIndexDTO results)
        {
            _unitofwork = uow;
            ReportTypeName = name;
            _fieldDescriptions = MakeDescriptionLookup();
            // results is ignored for this report

            ID = Guid.NewGuid();
            Errors = new List<string>();
            Status = "Report definition created.";
            Results = string.Empty;
            IsComplete = false;         
            _networkId = _unitofwork.NetworkRepo.GetMainNetwork().Id;
        }

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            segmentIds = Parse(parameters);
            var keyProperties = segmentIds.keyProperties;
            if (keyProperties[1] == "-1" && string.IsNullOrEmpty(keyProperties[0])) return; // report failed due to bad parameters
            if (!Validate(keyProperties)) return; // report failed due to validation

            // Check if asset actually exists
            string providedKey;
            if (Convert.ToInt32(segmentIds.keyProperties[1]) < 1)
            {
                providedKey = $"BMSID: {segmentIds.keyProperties[0]}";
                segmentData = _unitofwork.AssetDataRepository.GetAssetAttributes("BMSID", segmentIds.keyProperties[0]);
            }
            else
            {
                providedKey = $"BRKEY_:  {segmentIds.keyProperties[1]}";
                segmentData = _unitofwork.AssetDataRepository.GetAssetAttributes("BRKEY_", segmentIds.keyProperties[1]);
            }
            if (segmentData.Count() < 1)
            {
                // No data returned for key
                Errors.Add($"Provided key {providedKey} not found");
                return;
            }

            var resultsString = new StringBuilder();
            resultsString.Append("<table class=\"report-cell\">");
            resultsString.Append(CreateHTMLSection("Key Fields", new List<string>() { "BRKEY_", "BMSID" }));
            resultsString.Append(CreateHTMLSection("Location", new List<string>() { "DISTRICT", "COUNTY", "MUNI_CODE", "FEATURE_INTERSECTED", "FEATURE_CARRIED", "LOCATION" }));
            resultsString.Append(CreateHTMLSection("Age and Service", new List<string>() { "YEAR_BUILT", "YEAR_RECON", "SERVTYPON", "SERVTYPUND" }));
            resultsString.Append(CreateHTMLSection("Management", new List<string>() { "CUSTODIAN", "OWNER_CODE", "MPO_NAME", "SUBM_AGENCY", "NBISLEN", "HISTSIGN", "CRGIS_SHOPKEY_NUM", "BUS_PLAN_NETWORK" }));
            resultsString.Append(CreateHTMLSection("Deck Information", new List<string>() { "DKSTRUCTYP", "DEPT_DKSTRUCTYP", "DECKSURF_TYPE", "DKMEMBTYPE", "DKPROTECT", "DECK_WIDTH", "SKEW" }));
            resultsString.Append(CreateHTMLSection("Span Information", new List<string>() { "NUMBER_SPANS", "MATERIALMAIN", "DESIGNMAIN", "MATERIALAPPR", "DESIGNAPPR" , "LENGTH", "DECK_AREA", "TOT_LENGTH", "MAIN_FC_GROUP_NUM", "APPR_FC_GROUP_NUM" }));
            resultsString.Append(CreateHTMLSection("Current Conditions", new List<string>() { "DECK", "DECK_DURATION_N", "SUP", "SUP_DURATION_N", "SUB", "SUB_DURATION_N", "CULV", "CULV_DURATION_N" }));
            resultsString.Append(CreateHTMLSection("Previous Conditions", new List<string>() { "PREV_DECK", "PREV_SUP", "PREV_SUB", "PREV_CULV" }));
            resultsString.Append(CreateHTMLSection("Risk Scores", new List<string>() { "RISK_SCORE", "Old_Risk_Score" }));
            resultsString.Append(CreateHTMLSection("Posting", new List<string>() { "POST_STATUS_DATE", "POST_STATUS", "SPEC_RESTRICT_POST", "POST_LIMIT_WEIGHT" }));
            resultsString.Append(CreateHTMLSection("Roadway Information", new List<string>() { "ADTTOTAL", "FUNC_CLASS", "VCLROVER", "VCLRUNDER", "NHS_IND" }));
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

        private bool Validate(List<string> keyProperties)
        {
            if (Convert.ToInt32(keyProperties[1]) < 1 && string.IsNullOrEmpty(keyProperties[0]))
            {
                // No parameters provided
                return false;
            }

            if (Convert.ToInt32(keyProperties[1]) > 0 && !string.IsNullOrEmpty(keyProperties[0]))
            {
                // Both parameters provided.  Check to see if they are the same asset
                var BRKeyGuid = _unitofwork.AssetDataRepository.KeyProperties["BRKEY_"].FirstOrDefault(_ => _.KeyValue.Value == keyProperties[1].ToString());
                if (BRKeyGuid == null)
                {
                    // BRKey was not found
                    Errors.Add($"Unable to find BRKey {keyProperties[1]}.  Did not attempt to find {keyProperties[0]}");
                    return false;
                }
                var BMSIDGuid = _unitofwork.AssetDataRepository.KeyProperties["BMSID"].FirstOrDefault(_ => _.KeyValue.Value == keyProperties[0]);
                if (BMSIDGuid == null)
                {
                    // BMSID was not found
                    Errors.Add($"Unable to find BMSID {keyProperties[0]}.  Will not use {keyProperties[1]}");
                    return false;
                }
                if (BRKeyGuid.AssetId != BMSIDGuid.AssetId)
                {
                    // Keys were provided for two different assets
                    Errors.Add($"The BRKey {keyProperties[1]} and BMSID {keyProperties[0]}.  No report will be provided");
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
                var returnVal = segmentData.FirstOrDefault(_ => _.Name == attributeName);
                return (returnVal == null) ? DEFAULT_VALUE : returnVal.Value;
            }
            else
            {
                Dictionary<int, SegmentAttributeDatum> attrubuteValueHistory;
                if (Convert.ToInt32(segmentIds.keyProperties[1]) < 1)
                {
                    attrubuteValueHistory = _unitofwork.AssetDataRepository.GetAttributeValueHistory("BMSID", segmentIds.keyProperties[0], attributeName);
                }
                else
                {
                    attrubuteValueHistory = _unitofwork.AssetDataRepository.GetAttributeValueHistory("BRKEY_", segmentIds.keyProperties[1], attributeName);
                }
                if (attrubuteValueHistory.Count < 2) return DEFAULT_VALUE;  // The default value is returned if there is either no values OR one value (the previous value is still unknown)

                return attrubuteValueHistory.OrderByDescending(_ => _.Key).ElementAt(1).Value.Value;  // The first Value comes from the Dictionary, the second comes from the SegmentAttributeDatum
            }
        }
        private string GetDescription(string attributeName)
        {
            if (_fieldDescriptions.ContainsKey(attributeName))
            {
                return _fieldDescriptions[attributeName].Pub100ACode + " " + _fieldDescriptions[attributeName].Description;
            } else
            {
                return attributeName;
            }
        }

        private string CreateHTMLSection(string sectionName, List<string> attributes, int numberColumns = DEFAULT_COLUMNS, bool previous = false)
        {
            var sectionString = new StringBuilder($"<tr><th colspan=\"4\" class=\"report-header report-cell\">{sectionName}</th></tr>");
            int remainingColumns = numberColumns;
            foreach (var attribute in attributes)
            {
                if (remainingColumns == numberColumns)
                {
                    // This is the first column
                    sectionString.Append($"<tr><td class=\"report-description report-cell\">{GetDescription(attribute)}</td><td class=\"report-data report-cell\">{GetAttribute(attribute, previous)}</td>");
                    remainingColumns--;
                }
                else
                {
                    sectionString.Append($"<td class=\"report-description report-columnsplit report-cell\">{GetDescription(attribute)}</td><td class=\"report-data report-cell\">{GetAttribute(attribute, previous)}</td>");
                    remainingColumns--;
                }
                if (remainingColumns == 0)
                {
                    remainingColumns = numberColumns;
                    sectionString.Append($"</tr>");
                }
            }

            return sectionString.ToString();
        }


        /// <summary>
        /// Describes how the name of a field is shown to the user.
        /// </summary>
        /// <remarks>
        /// Split into two parts as we expect PennDOT to change their mind on where the Pub100A code is shown
        /// </remarks>
        private class AttributeDescription
        {
            public string Description { get; set; }
            public string Pub100ACode { get; set; }
        }

        /// <summary>
        /// Provides a list of attribute descriptions for use in the report
        /// </summary>
        /// <returns>
        /// Dictionary with a key of the database attribute name (e.g., DKSTRUCTTYP) versus its long name (e.g., Deck Structure Type)
        /// and code from Pub 100A (5B01)
        /// </returns>
        private Dictionary<string, AttributeDescription> MakeDescriptionLookup()
        {
            var descriptions = new Dictionary<string, AttributeDescription>();

            descriptions.Add("ADTTOTAL", new AttributeDescription() { Description = "ADT Total", Pub100ACode = "5C10" });
            descriptions.Add("ADTYEAR", new AttributeDescription() { Description = "ADT Year", Pub100ACode = "" });
            descriptions.Add("APPR_FC_GROUP_NUM", new AttributeDescription() { Description = "FC Group Number (Approach)", Pub100ACode = "6A44" });
            descriptions.Add("APPRALIGN", new AttributeDescription() { Description = "Approach Alignment", Pub100ACode = "" });
            descriptions.Add("AROADWIDTH", new AttributeDescription() { Description = "Approach Road Width", Pub100ACode = "" });
            descriptions.Add("B_Has_Risk", new AttributeDescription() { Description = "Bridge Has Risk", Pub100ACode = "" });
            descriptions.Add("BMSDATE", new AttributeDescription() { Description = "BMS Entry Date", Pub100ACode = "" });
            descriptions.Add("BR_Cond", new AttributeDescription() { Description = "Bridge Condition", Pub100ACode = "" });
            descriptions.Add("BRIDGE_ID", new AttributeDescription() { Description = "Bridge ID", Pub100ACode = "" });
            descriptions.Add("BRKEY", new AttributeDescription() { Description = "Bridge Key", Pub100ACode = "" });
            descriptions.Add("BRKEY2", new AttributeDescription() { Description = "Bridge Key", Pub100ACode = "" });
            descriptions.Add("BUS_PLAN_NETWORK", new AttributeDescription() { Description = "Business Plan Network", Pub100ACode = "6A19" });
            descriptions.Add("COUNTY", new AttributeDescription() { Description = "County", Pub100ACode = "5A05" });
            descriptions.Add("CRGIS_SHOPKEY_NUM", new AttributeDescription() { Description = "GIS Key", Pub100ACode = "5E05" });
            descriptions.Add("CULV", new AttributeDescription() { Description = "Culvert", Pub100ACode = "" });
            descriptions.Add("CULV_DUR", new AttributeDescription() { Description = "Culvert Time In Service", Pub100ACode = "" });
            descriptions.Add("CUSTODIAN", new AttributeDescription() { Description = "Maintenance Responsibility", Pub100ACode = "5A20" });
            descriptions.Add("DECK", new AttributeDescription() { Description = "Deck", Pub100ACode = "" });
            descriptions.Add("DECK_AREA", new AttributeDescription() { Description = "Deck Area", Pub100ACode = "5B19" });
            descriptions.Add("DECK_AREA2", new AttributeDescription() { Description = "Deck Area", Pub100ACode = "" });
            descriptions.Add("DECK_DUR", new AttributeDescription() { Description = "Deck Time In Service", Pub100ACode = "" });
            descriptions.Add("DECK_WIDTH", new AttributeDescription() { Description = "Deck Width", Pub100ACode = "5B07" });
            descriptions.Add("DECKGEEOM", new AttributeDescription() { Description = "Deck Geometery", Pub100ACode = "" });
            descriptions.Add("DEFHWY", new AttributeDescription() { Description = "Defense Highway Indicator", Pub100ACode = "" });
            descriptions.Add("DEPT_DKSTRUCTYP", new AttributeDescription() { Description = "Deck Sheet Type (PennDOT)", Pub100ACode = "6A38" });
            descriptions.Add("DESIGNAPPR", new AttributeDescription() { Description = "Approach Span Design", Pub100ACode = "5B16" });
            descriptions.Add("DESIGNMAIN", new AttributeDescription() { Description = "Main Span Design", Pub100ACode = "5B13" });
            descriptions.Add("DISTRICT", new AttributeDescription() { Description = "District", Pub100ACode = "5A04" });
            descriptions.Add("DKMEMBTYPE", new AttributeDescription() { Description = "Deck Membrane Type", Pub100ACode = "5B03" });
            descriptions.Add("DKPROTECT", new AttributeDescription() { Description = "Deck Protection", Pub100ACode = "5B04" });
            descriptions.Add("DKSTRUCTYP", new AttributeDescription() { Description = "Deck Strucutre Type", Pub100ACode = "5B01" });
            descriptions.Add("DECKSURF_TYPE", new AttributeDescription() { Description = "Deck Surface Type", Pub100ACode = "5B02" });
            descriptions.Add("FamilyID", new AttributeDescription() { Description = "Performance Family", Pub100ACode = "" });
            descriptions.Add("FEATURE_CARRIED", new AttributeDescription() { Description = "Feature Carried", Pub100ACode = "5A08" });
            descriptions.Add("FEATURE_INTERSECTED", new AttributeDescription() { Description = "Feature Intersected", Pub100ACode = "5A07" });
            descriptions.Add("Final_Extra_cols_OWNER_CODE", new AttributeDescription() { Description = "Owner Code (Extra Information)", Pub100ACode = "" });
            descriptions.Add("FracCrit", new AttributeDescription() { Description = "Fracture Critical Indicator", Pub100ACode = "" });
            descriptions.Add("FUNC_CLASS", new AttributeDescription() { Description = "Functional Class", Pub100ACode = "5C22" });
            descriptions.Add("FUNC_OBSOL", new AttributeDescription() { Description = "Functional Obsolence Indicator", Pub100ACode = "" });
            descriptions.Add("H20_IR", new AttributeDescription() { Description = "H20 (IR)", Pub100ACode = "4B11" });
            descriptions.Add("H20_OR", new AttributeDescription() { Description = "H20 (OR)", Pub100ACode = "4B09" });
            descriptions.Add("H20_RATIO", new AttributeDescription() { Description = "Ratio OR / Max Legal Load", Pub100ACode = "" });
            descriptions.Add("HBRR_ELIG", new AttributeDescription() { Description = "Historic Bridge Eligiblity", Pub100ACode = "" });
            descriptions.Add("HISTSIGN", new AttributeDescription() { Description = "Historical Sign", Pub100ACode = "5E04" });
            descriptions.Add("HS20_IR", new AttributeDescription() { Description = "HS20 (IR)", Pub100ACode = "4B07" });
            descriptions.Add("HS20_OR", new AttributeDescription() { Description = "HS20 (OR)", Pub100ACode = "4B05" });
            descriptions.Add("HS20_RATIO", new AttributeDescription() { Description = "Ratio OR / Max Legal Load", Pub100ACode = "" });
            descriptions.Add("INSPDATE", new AttributeDescription() { Description = "Inspection Date", Pub100ACode = "" });
            descriptions.Add("INSPSTAT", new AttributeDescription() { Description = "Inspection Status", Pub100ACode = "" });
            descriptions.Add("INSPTYPE", new AttributeDescription() { Description = "Inspection Type", Pub100ACode = "" });
            descriptions.Add("InternetReport", new AttributeDescription() { Description = "Internet Report Indicator", Pub100ACode = "" });
            descriptions.Add("InterState", new AttributeDescription() { Description = "Interstate indicator", Pub100ACode = "" });
            descriptions.Add("LANE", new AttributeDescription() { Description = "Number of Lanes", Pub100ACode = "" });
            descriptions.Add("LAT", new AttributeDescription() { Description = "Latitude", Pub100ACode = "5A10" });
            descriptions.Add("LENGTH", new AttributeDescription() { Description = "Length", Pub100ACode = "5B16" });
            descriptions.Add("LOCATION", new AttributeDescription() { Description = "Location / Structure Name", Pub100ACode = "5A02" });
            descriptions.Add("LONG", new AttributeDescription() { Description = "Longitude", Pub100ACode = "5A11" });
            descriptions.Add("MAIN_FC_GROUP_NUM", new AttributeDescription() { Description = "FC Group Number (Main)", Pub100ACode = "6A44" });
            descriptions.Add("MATERIAL_TYPE", new AttributeDescription() { Description = "Material Type", Pub100ACode = "" });
            descriptions.Add("MATERIALAPPR", new AttributeDescription() { Description = "Approach Span Material", Pub100ACode = "5B15" });
            descriptions.Add("MATERIALMAIN", new AttributeDescription() { Description = "Main Span Material", Pub100ACode = "5B12" });
            descriptions.Add("MaxSpan", new AttributeDescription() { Description = "Maximum Span Length", Pub100ACode = "" });
            descriptions.Add("MIN_RATIO", new AttributeDescription() { Description = "Minimum Ratio OR / Max Legal Load", Pub100ACode = "" });
            descriptions.Add("ML80_IR", new AttributeDescription() { Description = "ML80 (IR)", Pub100ACode = "4B12" });
            descriptions.Add("ML80_OR", new AttributeDescription() { Description = "ML80 (OR)", Pub100ACode = "4B12" });
            descriptions.Add("ML80_RATIO", new AttributeDescription() { Description = "Ratio OR / Max Legal Load", Pub100ACode = "" });
            descriptions.Add("MPO", new AttributeDescription() { Description = "MPO Code", Pub100ACode = "" });
            descriptions.Add("MPO_Name", new AttributeDescription() { Description = "MPO Name", Pub100ACode = "5A23" });
            descriptions.Add("MUNI_CODE", new AttributeDescription() { Description = "Municipality Code", Pub100ACode = "5A06" });
            descriptions.Add("NBI_RATING", new AttributeDescription() { Description = "NBI Rating", Pub100ACode = "" });
            descriptions.Add("NBISLEN", new AttributeDescription() { Description = "NBIS Length", Pub100ACode = "5E01" });
            descriptions.Add("NHS_IND", new AttributeDescription() { Description = "NHS Indicator", Pub100ACode = "5C29" });
            descriptions.Add("NUMBER_SPANS", new AttributeDescription() { Description = "Number of Spans", Pub100ACode = "5B11/5B14" });
            descriptions.Add("Old_Risk_Score", new AttributeDescription() { Description = "Old Risk Score", Pub100ACode = "" });
            descriptions.Add("OVER_WATER", new AttributeDescription() { Description = "Over Water Indicator", Pub100ACode = "" });
            descriptions.Add("OWNER_CODE", new AttributeDescription() { Description = "Owner Code", Pub100ACode = "5A21" });
            descriptions.Add("P3_Bridge", new AttributeDescription() { Description = "P3 Bridge Indicator", Pub100ACode = "" });
            descriptions.Add("P3_Data", new AttributeDescription() { Description = "P3 Information", Pub100ACode = "" });
            descriptions.Add("PAINT_COND", new AttributeDescription() { Description = "Paint", Pub100ACode = "" });
            descriptions.Add("PAINT_EXTENT", new AttributeDescription() { Description = "Paint Extent", Pub100ACode = "" });
            descriptions.Add("Parallel_Struct", new AttributeDescription() { Description = "Parallel Structure Indicator", Pub100ACode = "" });
            descriptions.Add("POST_LIMIT_COMB", new AttributeDescription() { Description = "Combination (Tons)", Pub100ACode = "VP05" });
            descriptions.Add("POST_LIMIT_WEIGHT", new AttributeDescription() { Description = "Single (Tons)", Pub100ACode = "VP04" });
            descriptions.Add("POST_STATUS", new AttributeDescription() { Description = "Post Status", Pub100ACode = "VP02" });
            descriptions.Add("POST_STATUS_DATE", new AttributeDescription() { Description = "Post Status Date", Pub100ACode = "VP01" });
            descriptions.Add("PREV_CULV", new AttributeDescription() { Description = "Culvert", Pub100ACode = "" });
            descriptions.Add("PREV_CULV_DUR", new AttributeDescription() { Description = "Culvert Time In Service", Pub100ACode = "" });
            descriptions.Add("PREV_DECK", new AttributeDescription() { Description = "Deck", Pub100ACode = "" });
            descriptions.Add("PREV_DECK_DUR", new AttributeDescription() { Description = "Deck Time In Service", Pub100ACode = "" });
            descriptions.Add("PREV_SUB", new AttributeDescription() { Description = "Substructure", Pub100ACode = "" });
            descriptions.Add("PREV_SUB_DUR", new AttributeDescription() { Description = "Substructure Time In Service", Pub100ACode = "" });
            descriptions.Add("PREV_SUP", new AttributeDescription() { Description = "Superstructure", Pub100ACode = "" });
            descriptions.Add("PREV_SUP_DUR", new AttributeDescription() { Description = "Superstructure Time In Service", Pub100ACode = "" });
            descriptions.Add("PROPWORK", new AttributeDescription() { Description = "Proposed Work Indicator", Pub100ACode = "" });
            descriptions.Add("RISK_SCORE", new AttributeDescription() { Description = "New Risk Score", Pub100ACode = "" });
            descriptions.Add("ROADWIDTH", new AttributeDescription() { Description = "Road Width", Pub100ACode = "" });
            descriptions.Add("ROUTENum", new AttributeDescription() { Description = "Route Number", Pub100ACode = "" });
            descriptions.Add("SERVTYPON", new AttributeDescription() { Description = "Type of Service On", Pub100ACode = "5A17" });
            descriptions.Add("SERVTYPUND", new AttributeDescription() { Description = "Type of Service Under", Pub100ACode = "5A18" });
            descriptions.Add("SKEW", new AttributeDescription() { Description = "Skew", Pub100ACode = "5B09" });
            descriptions.Add("SPEC_RESTRICT_POST", new AttributeDescription() { Description = "Spec Restrict Post", Pub100ACode = "VP03" });
            descriptions.Add("Steel_Type", new AttributeDescription() { Description = "Steel Type", Pub100ACode = "" });
            descriptions.Add("STRUCT_DEF", new AttributeDescription() { Description = "Structural Deficient Indicator", Pub100ACode = "" });
            descriptions.Add("StructConfig", new AttributeDescription() { Description = "Structural Configuration", Pub100ACode = "SP10" });
            descriptions.Add("STRUCTURE_TYPE", new AttributeDescription() { Description = "Structure Type", Pub100ACode = "" });
            descriptions.Add("SUB", new AttributeDescription() { Description = "Substructure", Pub100ACode = "" });
            descriptions.Add("SUB_DUR", new AttributeDescription() { Description = "Substructure Time In Service", Pub100ACode = "" });
            descriptions.Add("SUBM_AGENCY", new AttributeDescription() { Description = "Submitting Agency", Pub100ACode = "6A06" });
            descriptions.Add("SUP", new AttributeDescription() { Description = "Superstructure", Pub100ACode = "" });
            descriptions.Add("SUP_DUR", new AttributeDescription() { Description = "Superstructure Time In Service", Pub100ACode = "" });
            descriptions.Add("TK527_IR", new AttributeDescription() { Description = "TK527 (IR)", Pub100ACode = "4B13" });
            descriptions.Add("TK527_OR", new AttributeDescription() { Description = "TK527 (OR)", Pub100ACode = "4B13" });
            descriptions.Add("TK527_RATIO", new AttributeDescription() { Description = "Ratio OR / Max Legal Load", Pub100ACode = "" });
            descriptions.Add("TOT_LENGTH", new AttributeDescription() { Description = "Total Length", Pub100ACode = "5B20" });
            descriptions.Add("TRUCKPCT", new AttributeDescription() { Description = "Percentage Trucks", Pub100ACode = "" });
            descriptions.Add("VCLROVER", new AttributeDescription() { Description = "Over Street Clearance", Pub100ACode = "4A15" });
            descriptions.Add("VCLRUNDER", new AttributeDescription() { Description = "Under Street Clearance", Pub100ACode = "4A16" });
            descriptions.Add("YEAR_BUILT", new AttributeDescription() { Description = "Year Built", Pub100ACode = "5A15" });
            descriptions.Add("YEAR_RECON", new AttributeDescription() { Description = "Year Reconstructed", Pub100ACode = "5A16" });
            descriptions.Add("CULV_DURATION_N", new AttributeDescription() { Description = "Culvert Duration", Pub100ACode = "" });
            descriptions.Add("DECK_DURATION_N", new AttributeDescription() { Description = "Deck Duration", Pub100ACode = "" });
            descriptions.Add("SUB_DURATION_N", new AttributeDescription() { Description = "Substructure Duration", Pub100ACode = "" });
            descriptions.Add("SUP_DURATION_N", new AttributeDescription() { Description = "Superstructure Duration", Pub100ACode = "" });


            return descriptions;
        }
    }
}
