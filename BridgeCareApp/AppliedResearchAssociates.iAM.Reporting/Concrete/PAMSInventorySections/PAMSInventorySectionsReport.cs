using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting.Models;
using AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.CountySummary;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSInventorySectionsReport : IReport
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

        private PAMSParameters _failedQuery = new PAMSParameters { County = "unknown",Route=0,Segment=0};

        private List<SegmentAttributeDatum> sectionData;
        private InventoryParameters sectionIds;
      
        public PAMSInventorySectionsReport(IUnitOfWork uow, string name, ReportIndexDTO results)
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

        // public Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null) => throw new NotImplementedException();

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            
            var sectionIds = Parse(parameters);
            var sectionData = GetAsset(sectionIds);

            var resultsString = new StringBuilder();
            resultsString.Append("<table class=\"report-cell\">");

            //resultsString.Append(CreateHTMLSection("Key Fields", new List<string>() { "BRKEY_", "BMSID" }));
            //resultsString.Append(CreateHTMLSection("Location", new List<string>() { "DISTRICT", "COUNTY", "MUNI_CODE", "FEATURE_INTERSECTED", "FEATURE_CARRIED", "LOCATION" }));
            //resultsString.Append(CreateHTMLSection("Age and Service", new List<string>() { "YEAR_BUILT", "YEAR_RECON", "SERVTYPON", "SERVTYPUND" }));
            //resultsString.Append(CreateHTMLSection("Management", new List<string>() { "CUSTODIAN", "OWNER_CODE", "MPO_NAME", "SUBM_AGENCY", "NBISLEN", "HISTSIGN", "CRGIS_SHOPKEY_NUM", "BUS_PLAN_NETWORK" }));
            //resultsString.Append(CreateHTMLSection("Deck Information", new List<string>() { "DKSTRUCTYP", "DEPT_DKSTRUCTYP", "DECKSURF_TYPE", "DKMEMBTYPE", "DKPROTECT", "DECK_WIDTH", "SKEW" }));
            //resultsString.Append(CreateHTMLSection("Span Information", new List<string>() { "NUMBER_SPANS", "MATERIALMAIN", "DESIGNMAIN", "MATERIALAPPR", "DESIGNAPPR", "LENGTH", "DECK_AREA", "TOT_LENGTH", "MAIN_FC_GROUP_NUM", "APPR_FC_GROUP_NUM" }));
            //resultsString.Append(CreateHTMLSection("Current Conditions", new List<string>() { "DECK", "DECK_DURATION_N", "SUP", "SUP_DURATION_N", "SUB", "SUB_DURATION_N", "CULV", "CULV_DURATION_N" }));
            //resultsString.Append(CreateHTMLSection("Previous Conditions", new List<string>() { "PREV_DECK", "PREV_SUP", "PREV_SUB", "PREV_CULV" }));
            //resultsString.Append(CreateHTMLSection("Risk Scores", new List<string>() { "RISK_SCORE", "Old_Risk_Score" }));
            //resultsString.Append(CreateHTMLSection("Posting", new List<string>() { "POST_STATUS_DATE", "POST_STATUS", "SPEC_RESTRICT_POST", "POST_LIMIT_WEIGHT" }));
            //resultsString.Append(CreateHTMLSection("Roadway Information", new List<string>() { "ADTTOTAL", "FUNC_CLASS", "VCLROVER", "VCLRUNDER", "NHS_IND" }));

            //resultsString.Append(CreateHTMLSection("Key Fields", new List<string>() { "County", "Route", "Section" }));
            resultsString.Append(CreateHTMLSection("ID", new List<string>() { "Section", "County", "Route", "From Section", "To Section", "Member Segments" }));
            resultsString.Append(CreateHTMLSection("Description", new List<string>() { "Direction", "District", "MPO/RPO", "Urban/Rural", "BPN", "ADT", "ADTT", "Truck %", "Surface", "Federal Aid?", "HPMS?", "Lanes", "Length", "Width", "Age" }));
            resultsString.Append(CreateHTMLSection("Surface Attributes", new List<string>() { "Surface", "Surface ID", "Left Shoulder Type", "Right Shoulder Type", "Year Built", "Last Overlay", "Last Structural Overlay" }));
            resultsString.Append(CreateHTMLSection("Survey Information", new List<string>() { "Survey Date" }));
            resultsString.Append(CreateHTMLSection("Measured Conditions", new List<string>() { "OPI", "Roughness" }));
            resultsString.Append(CreateHTMLSection("Surface Defects", new List<string>() { "Type" }));
            //resultsString.Append(CreateHTMLSection("", new List<string>() { }));


            resultsString.Append("</table>");
            Results = resultsString.ToString();
            IsComplete = true;
            return;

        }

        private PAMSParameters Parse(string parameters)
        {
            try
            {
                PAMSParameters query = JsonConvert.DeserializeObject<PAMSParameters>(parameters);
                if (query == null)
                {
                    Errors.Add($"Unable to run.  No query parameters provided in request body.");
                    return _failedQuery;
                }
                return query;
            }
            catch (Exception e)
            {
                Errors.Add($"Failed to parse JSON in request body due to {e.Message}");
                return _failedQuery;
            }
        }

        private List<SegmentAttributeDatum> GetAsset(PAMSParameters keyProperties)
        {

            //var attributeList = new List<string>() {"County","SR"};

            var allAttributes = _unitofwork.AttributeRepo.GetAttributes();
            allAttributes.Add(new AttributeDTO() { Name = "Segment", Command = "SEG", DataSource = allAttributes.Single(_ => _.Name == "COUNTY").DataSource});
            var queryDictionary= new Dictionary<AttributeDTO, string>();
            queryDictionary.Add(allAttributes.Single(_ => _.Name == "COUNTY"), keyProperties.County);
            queryDictionary.Add(allAttributes.Single(_ => _.Name == "SR"), keyProperties.Route.ToString());
            queryDictionary.Add(allAttributes.Single(_ => _.Name == "Segment"), keyProperties.Segment.ToString());

            var sectionData = _unitofwork.DataSourceRepo.GetRawData(queryDictionary);
            var sectionId = sectionData["CRS_Data"];
            var result = _unitofwork.AssetDataRepository.GetAssetAttributes("CRS",sectionId);

            return result;
        }

        //private Dictionary<string, AttributeDescription> MakeDescriptionLookup()
        //        private static List<Guid> GetRequiredAttributeIds(List<AttributeDTO> attributeDTOs)
        //        IDictionary<int, string> numberNames = new Dictionary<int, string>();
        //        numberNames.Add(1,"One"); //adding a key/value using the Add() method
        //numberNames.Add(2,"Two");
        //numberNames.Add(3,"Three");
        private static IDictionary<AttributeDTO, string> GetAttributes(List<AttributeDTO> attributeDTOs, List<string> strings)
        {
            var requiredAttributes = new List<string>
            {
                "SEGMENT_LENGTH","WIDTH","DISTRICT","CNTY","SR","DIRECTION","INTERSTATE","LANES","SURFACE_NAME","RISKSCORE"
            };

            // GetAttributes.Add(1, "One"); //adding a key/value using the Add() method
            return null; //attributeDTOs.Where(_ => requiredAttributes.Contains(_.Name)).Select(_ => _.Id).ToList();
        }


        private string GetAttribute(string attributeName, bool previous=false)
        {


            var returnVal = sectionData.FirstOrDefault(_ => _.Name == attributeName);
            return (returnVal == null) ? DEFAULT_VALUE : returnVal.Value;


            //string result;

            //if (!previous)
            //{
            //    var returnVal = sectionData.FirstOrDefault(_ => _.Name == attributeName);
            //    return (returnVal == null) ? DEFAULT_VALUE : returnVal.Value;
            //}
            //else
            //{
            //    Dictionary<int, SegmentAttributeDatum> attrubuteValueHistory;
            //    if (Convert.ToInt32(sectionIds.keyProperties[1]) < 1)
            //    {
            //        attrubuteValueHistory = _unitofwork.AssetDataRepository.GetAttributeValueHistory("BMSID", sectionIds.keyProperties[0], attributeName);
            //    }
            //    else
            //    {
            //        attrubuteValueHistory = _unitofwork.AssetDataRepository.GetAttributeValueHistory("BRKEY_", sectionIds.keyProperties[1], attributeName);
            //    }
            //    if (attrubuteValueHistory.Count < 2) return DEFAULT_VALUE;  // The default value is returned if there is either no values OR one value (the previous value is still unknown)

            //    return attrubuteValueHistory.OrderByDescending(_ => _.Key).ElementAt(1).Value.Value;  // The first Value comes from the Dictionary, the second comes from the SegmentAttributeDatum
            //}
        }
        private string GetDescription(string attributeName)
        {
            if (_fieldDescriptions.ContainsKey(attributeName))
            {
                // return _fieldDescriptions[attributeName].Pub100ACode + " " + _fieldDescriptions[attributeName].Description;
                return _fieldDescriptions[attributeName].Description;
            }
            else
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



        private class AttributeDescription
        {
            public string Description { get; set; }
            //  public string Pub100ACode { get; set; }
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

            //descriptions.Add("ADTTOTAL", new AttributeDescription() { Description = "ADT Total", Pub100ACode = "5C10" });
            //descriptions.Add("ADTYEAR", new AttributeDescription() { Description = "ADT Year", Pub100ACode = "" });
            //descriptions.Add("APPR_FC_GROUP_NUM", new AttributeDescription() { Description = "FC Group Number (Approach)", Pub100ACode = "6A44" });
            //descriptions.Add("APPRALIGN", new AttributeDescription() { Description = "Approach Alignment", Pub100ACode = "" });
            //descriptions.Add("AROADWIDTH", new AttributeDescription() { Description = "Approach Road Width", Pub100ACode = "" });
            //descriptions.Add("B_Has_Risk", new AttributeDescription() { Description = "Bridge Has Risk", Pub100ACode = "" });
            //descriptions.Add("BMSDATE", new AttributeDescription() { Description = "BMS Entry Date", Pub100ACode = "" });
            //descriptions.Add("BR_Cond", new AttributeDescription() { Description = "Bridge Condition", Pub100ACode = "" });
            //descriptions.Add("BRIDGE_ID", new AttributeDescription() { Description = "Bridge ID", Pub100ACode = "" });
            //descriptions.Add("BRKEY", new AttributeDescription() { Description = "Bridge Key", Pub100ACode = "" });
            //descriptions.Add("BRKEY2", new AttributeDescription() { Description = "Bridge Key", Pub100ACode = "" });
            //descriptions.Add("BUS_PLAN_NETWORK", new AttributeDescription() { Description = "Business Plan Network", Pub100ACode = "6A19" });
            //descriptions.Add("COUNTY", new AttributeDescription() { Description = "County", Pub100ACode = "5A05" });
            //descriptions.Add("CRGIS_SHOPKEY_NUM", new AttributeDescription() { Description = "GIS Key", Pub100ACode = "5E05" });
            //descriptions.Add("CULV", new AttributeDescription() { Description = "Culvert", Pub100ACode = "" });
            //descriptions.Add("CULV_DUR", new AttributeDescription() { Description = "Culvert Time In Service", Pub100ACode = "" });
            //descriptions.Add("CUSTODIAN", new AttributeDescription() { Description = "Maintenance Responsibility", Pub100ACode = "5A20" });
            //descriptions.Add("DECK", new AttributeDescription() { Description = "Deck", Pub100ACode = "" });
            //descriptions.Add("DECK_AREA", new AttributeDescription() { Description = "Deck Area", Pub100ACode = "5B19" });
            //descriptions.Add("DECK_AREA2", new AttributeDescription() { Description = "Deck Area", Pub100ACode = "" });
            //descriptions.Add("DECK_DUR", new AttributeDescription() { Description = "Deck Time In Service", Pub100ACode = "" });
            //descriptions.Add("DECK_WIDTH", new AttributeDescription() { Description = "Deck Width", Pub100ACode = "5B07" });
            //descriptions.Add("DECKGEEOM", new AttributeDescription() { Description = "Deck Geometery", Pub100ACode = "" });
            //descriptions.Add("DEFHWY", new AttributeDescription() { Description = "Defense Highway Indicator", Pub100ACode = "" });
            //descriptions.Add("DEPT_DKSTRUCTYP", new AttributeDescription() { Description = "Deck Sheet Type (PennDOT)", Pub100ACode = "6A38" });
            //descriptions.Add("DESIGNAPPR", new AttributeDescription() { Description = "Approach Span Design", Pub100ACode = "5B16" });
            //descriptions.Add("DESIGNMAIN", new AttributeDescription() { Description = "Main Span Design", Pub100ACode = "5B13" });
            //descriptions.Add("DISTRICT", new AttributeDescription() { Description = "District", Pub100ACode = "5A04" });
            //descriptions.Add("DKMEMBTYPE", new AttributeDescription() { Description = "Deck Membrane Type", Pub100ACode = "5B03" });
            //descriptions.Add("DKPROTECT", new AttributeDescription() { Description = "Deck Protection", Pub100ACode = "5B04" });
            //descriptions.Add("DKSTRUCTYP", new AttributeDescription() { Description = "Deck Strucutre Type", Pub100ACode = "5B01" });
            //descriptions.Add("DECKSURF_TYPE", new AttributeDescription() { Description = "Deck Surface Type", Pub100ACode = "5B02" });
            //descriptions.Add("FamilyID", new AttributeDescription() { Description = "Performance Family", Pub100ACode = "" });
            //descriptions.Add("FEATURE_CARRIED", new AttributeDescription() { Description = "Feature Carried", Pub100ACode = "5A08" });
            //descriptions.Add("FEATURE_INTERSECTED", new AttributeDescription() { Description = "Feature Intersected", Pub100ACode = "5A07" });
            //descriptions.Add("Final_Extra_cols_OWNER_CODE", new AttributeDescription() { Description = "Owner Code (Extra Information)", Pub100ACode = "" });
            //descriptions.Add("FracCrit", new AttributeDescription() { Description = "Fracture Critical Indicator", Pub100ACode = "" });
            //descriptions.Add("FUNC_CLASS", new AttributeDescription() { Description = "Functional Class", Pub100ACode = "5C22" });
            //descriptions.Add("FUNC_OBSOL", new AttributeDescription() { Description = "Functional Obsolence Indicator", Pub100ACode = "" });
            //descriptions.Add("H20_IR", new AttributeDescription() { Description = "H20 (IR)", Pub100ACode = "4B11" });
            //descriptions.Add("H20_OR", new AttributeDescription() { Description = "H20 (OR)", Pub100ACode = "4B09" });
            //descriptions.Add("H20_RATIO", new AttributeDescription() { Description = "Ratio OR / Max Legal Load", Pub100ACode = "" });
            //descriptions.Add("HBRR_ELIG", new AttributeDescription() { Description = "Historic Bridge Eligiblity", Pub100ACode = "" });
            //descriptions.Add("HISTSIGN", new AttributeDescription() { Description = "Historical Sign", Pub100ACode = "5E04" });
            //descriptions.Add("HS20_IR", new AttributeDescription() { Description = "HS20 (IR)", Pub100ACode = "4B07" });
            //descriptions.Add("HS20_OR", new AttributeDescription() { Description = "HS20 (OR)", Pub100ACode = "4B05" });
            //descriptions.Add("HS20_RATIO", new AttributeDescription() { Description = "Ratio OR / Max Legal Load", Pub100ACode = "" });
            //descriptions.Add("INSPDATE", new AttributeDescription() { Description = "Inspection Date", Pub100ACode = "" });
            //descriptions.Add("INSPSTAT", new AttributeDescription() { Description = "Inspection Status", Pub100ACode = "" });
            //descriptions.Add("INSPTYPE", new AttributeDescription() { Description = "Inspection Type", Pub100ACode = "" });
            //descriptions.Add("InternetReport", new AttributeDescription() { Description = "Internet Report Indicator", Pub100ACode = "" });
            //descriptions.Add("InterState", new AttributeDescription() { Description = "Interstate indicator", Pub100ACode = "" });
            //descriptions.Add("LANE", new AttributeDescription() { Description = "Number of Lanes", Pub100ACode = "" });
            //descriptions.Add("LAT", new AttributeDescription() { Description = "Latitude", Pub100ACode = "5A10" });
            //descriptions.Add("LENGTH", new AttributeDescription() { Description = "Length", Pub100ACode = "5B16" });
            //descriptions.Add("LOCATION", new AttributeDescription() { Description = "Location / Structure Name", Pub100ACode = "5A02" });
            //descriptions.Add("LONG", new AttributeDescription() { Description = "Longitude", Pub100ACode = "5A11" });
            //descriptions.Add("MAIN_FC_GROUP_NUM", new AttributeDescription() { Description = "FC Group Number (Main)", Pub100ACode = "6A44" });
            //descriptions.Add("MATERIAL_TYPE", new AttributeDescription() { Description = "Material Type", Pub100ACode = "" });
            //descriptions.Add("MATERIALAPPR", new AttributeDescription() { Description = "Approach Span Material", Pub100ACode = "5B15" });
            //descriptions.Add("MATERIALMAIN", new AttributeDescription() { Description = "Main Span Material", Pub100ACode = "5B12" });
            //descriptions.Add("MaxSpan", new AttributeDescription() { Description = "Maximum Span Length", Pub100ACode = "" });
            //descriptions.Add("MIN_RATIO", new AttributeDescription() { Description = "Minimum Ratio OR / Max Legal Load", Pub100ACode = "" });
            //descriptions.Add("ML80_IR", new AttributeDescription() { Description = "ML80 (IR)", Pub100ACode = "4B12" });
            //descriptions.Add("ML80_OR", new AttributeDescription() { Description = "ML80 (OR)", Pub100ACode = "4B12" });
            //descriptions.Add("ML80_RATIO", new AttributeDescription() { Description = "Ratio OR / Max Legal Load", Pub100ACode = "" });
            //descriptions.Add("MPO", new AttributeDescription() { Description = "MPO Code", Pub100ACode = "" });
            //descriptions.Add("MPO_Name", new AttributeDescription() { Description = "MPO Name", Pub100ACode = "5A23" });
            //descriptions.Add("MUNI_CODE", new AttributeDescription() { Description = "Municipality Code", Pub100ACode = "5A06" });
            //descriptions.Add("NBI_RATING", new AttributeDescription() { Description = "NBI Rating", Pub100ACode = "" });
            //descriptions.Add("NBISLEN", new AttributeDescription() { Description = "NBIS Length", Pub100ACode = "5E01" });
            //descriptions.Add("NHS_IND", new AttributeDescription() { Description = "NHS Indicator", Pub100ACode = "5C29" });
            //descriptions.Add("NUMBER_SPANS", new AttributeDescription() { Description = "Number of Spans", Pub100ACode = "5B11/5B14" });
            //descriptions.Add("Old_Risk_Score", new AttributeDescription() { Description = "Old Risk Score", Pub100ACode = "" });
            //descriptions.Add("OVER_WATER", new AttributeDescription() { Description = "Over Water Indicator", Pub100ACode = "" });
            //descriptions.Add("OWNER_CODE", new AttributeDescription() { Description = "Owner Code", Pub100ACode = "5A21" });
            //descriptions.Add("P3_Bridge", new AttributeDescription() { Description = "P3 Bridge Indicator", Pub100ACode = "" });
            //descriptions.Add("P3_Data", new AttributeDescription() { Description = "P3 Information", Pub100ACode = "" });
            //descriptions.Add("PAINT_COND", new AttributeDescription() { Description = "Paint", Pub100ACode = "" });
            //descriptions.Add("PAINT_EXTENT", new AttributeDescription() { Description = "Paint Extent", Pub100ACode = "" });
            //descriptions.Add("Parallel_Struct", new AttributeDescription() { Description = "Parallel Structure Indicator", Pub100ACode = "" });
            //descriptions.Add("POST_LIMIT_COMB", new AttributeDescription() { Description = "Combination (Tons)", Pub100ACode = "VP05" });
            //descriptions.Add("POST_LIMIT_WEIGHT", new AttributeDescription() { Description = "Single (Tons)", Pub100ACode = "VP04" });
            //descriptions.Add("POST_STATUS", new AttributeDescription() { Description = "Post Status", Pub100ACode = "VP02" });
            //descriptions.Add("POST_STATUS_DATE", new AttributeDescription() { Description = "Post Status Date", Pub100ACode = "VP01" });
            //descriptions.Add("PREV_CULV", new AttributeDescription() { Description = "Culvert", Pub100ACode = "" });
            //descriptions.Add("PREV_CULV_DUR", new AttributeDescription() { Description = "Culvert Time In Service", Pub100ACode = "" });
            //descriptions.Add("PREV_DECK", new AttributeDescription() { Description = "Deck", Pub100ACode = "" });
            //descriptions.Add("PREV_DECK_DUR", new AttributeDescription() { Description = "Deck Time In Service", Pub100ACode = "" });
            //descriptions.Add("PREV_SUB", new AttributeDescription() { Description = "Substructure", Pub100ACode = "" });
            //descriptions.Add("PREV_SUB_DUR", new AttributeDescription() { Description = "Substructure Time In Service", Pub100ACode = "" });
            //descriptions.Add("PREV_SUP", new AttributeDescription() { Description = "Superstructure", Pub100ACode = "" });
            //descriptions.Add("PREV_SUP_DUR", new AttributeDescription() { Description = "Superstructure Time In Service", Pub100ACode = "" });
            //descriptions.Add("PROPWORK", new AttributeDescription() { Description = "Proposed Work Indicator", Pub100ACode = "" });
            //descriptions.Add("RISK_SCORE", new AttributeDescription() { Description = "New Risk Score", Pub100ACode = "" });
            //descriptions.Add("ROADWIDTH", new AttributeDescription() { Description = "Road Width", Pub100ACode = "" });
            //descriptions.Add("ROUTENum", new AttributeDescription() { Description = "Route Number", Pub100ACode = "" });
            //descriptions.Add("SERVTYPON", new AttributeDescription() { Description = "Type of Service On", Pub100ACode = "5A17" });
            //descriptions.Add("SERVTYPUND", new AttributeDescription() { Description = "Type of Service Under", Pub100ACode = "5A18" });
            //descriptions.Add("SKEW", new AttributeDescription() { Description = "Skew", Pub100ACode = "5B09" });
            //descriptions.Add("SPEC_RESTRICT_POST", new AttributeDescription() { Description = "Spec Restrict Post", Pub100ACode = "VP03" });
            //descriptions.Add("Steel_Type", new AttributeDescription() { Description = "Steel Type", Pub100ACode = "" });
            //descriptions.Add("STRUCT_DEF", new AttributeDescription() { Description = "Structural Deficient Indicator", Pub100ACode = "" });
            //descriptions.Add("StructConfig", new AttributeDescription() { Description = "Structural Configuration", Pub100ACode = "SP10" });
            //descriptions.Add("STRUCTURE_TYPE", new AttributeDescription() { Description = "Structure Type", Pub100ACode = "" });
            //descriptions.Add("SUB", new AttributeDescription() { Description = "Substructure", Pub100ACode = "" });
            //descriptions.Add("SUB_DUR", new AttributeDescription() { Description = "Substructure Time In Service", Pub100ACode = "" });
            //descriptions.Add("SUBM_AGENCY", new AttributeDescription() { Description = "Submitting Agency", Pub100ACode = "6A06" });
            //descriptions.Add("SUP", new AttributeDescription() { Description = "Superstructure", Pub100ACode = "" });
            //descriptions.Add("SUP_DUR", new AttributeDescription() { Description = "Superstructure Time In Service", Pub100ACode = "" });
            //descriptions.Add("TK527_IR", new AttributeDescription() { Description = "TK527 (IR)", Pub100ACode = "4B13" });
            //descriptions.Add("TK527_OR", new AttributeDescription() { Description = "TK527 (OR)", Pub100ACode = "4B13" });
            //descriptions.Add("TK527_RATIO", new AttributeDescription() { Description = "Ratio OR / Max Legal Load", Pub100ACode = "" });
            //descriptions.Add("TOT_LENGTH", new AttributeDescription() { Description = "Total Length", Pub100ACode = "5B20" });
            //descriptions.Add("TRUCKPCT", new AttributeDescription() { Description = "Percentage Trucks", Pub100ACode = "" });
            //descriptions.Add("VCLROVER", new AttributeDescription() { Description = "Over Street Clearance", Pub100ACode = "4A15" });
            //descriptions.Add("VCLRUNDER", new AttributeDescription() { Description = "Under Street Clearance", Pub100ACode = "4A16" });
            //descriptions.Add("YEAR_BUILT", new AttributeDescription() { Description = "Year Built", Pub100ACode = "5A15" });
            //descriptions.Add("YEAR_RECON", new AttributeDescription() { Description = "Year Reconstructed", Pub100ACode = "5A16" });
            //descriptions.Add("CULV_DURATION_N", new AttributeDescription() { Description = "Culvert Duration", Pub100ACode = "" });
            //descriptions.Add("DECK_DURATION_N", new AttributeDescription() { Description = "Deck Duration", Pub100ACode = "" });
            //descriptions.Add("SUB_DURATION_N", new AttributeDescription() { Description = "Substructure Duration", Pub100ACode = "" });
            //descriptions.Add("SUP_DURATION_N", new AttributeDescription() { Description = "Superstructure Duration", Pub100ACode = "" });

            descriptions.Add("AADT", new AttributeDescription() { Description = "" });
            descriptions.Add("Age", new AttributeDescription() { Description = "Age" });
            descriptions.Add("BEDGDTR1", new AttributeDescription() { Description = "BITUMINIOUS EDGE DETER. LENGTH - L" });
            descriptions.Add("BEDGDTR2", new AttributeDescription() { Description = "BITUMINIOUS EDGE DETER. LENGTH - M" });
            descriptions.Add("BEDGDTR3", new AttributeDescription() { Description = "BITUMINIOUS EDGE DETER. LENGTH - H" });
            descriptions.Add("BFATICR1", new AttributeDescription() { Description = "BITUMINIOUS FATIGUE CRACK - L" });
            descriptions.Add("BFATICR2", new AttributeDescription() { Description = "BITUMINIOUS FATIGUE CRACK - M" });
            descriptions.Add("BFATICR3", new AttributeDescription() { Description = "BITUMINIOUS FATIGUE CRACK - H" });
            descriptions.Add("BLRUTDP1", new AttributeDescription() { Description = "BITUMINIOUS RUTTING LEFT - L" });
            descriptions.Add("BLRUTDP2", new AttributeDescription() { Description = "BITUMINIOUS RUTTING LEFT - M" });
            descriptions.Add("BLRUTDP3", new AttributeDescription() { Description = "BITUMINIOUS RUTTING LEFT - H" });
            descriptions.Add("BLTEDGE1", new AttributeDescription() { Description = "BITUMINIOUS LEFT EDGE JOINT - L" });
            descriptions.Add("BLTEDGE2", new AttributeDescription() { Description = "BITUMINIOUS LEFT EDGE JOINT - M" });
            descriptions.Add("BLTEDGE3", new AttributeDescription() { Description = "BITUMINIOUS LEFT EDGE JOINT - H" });
            descriptions.Add("BMISCCK1", new AttributeDescription() { Description = "BITUMINIOUS MISCELLANEOUS CRACK - L" });
            descriptions.Add("BMISCCK2", new AttributeDescription() { Description = "BITUMINIOUS MISCELLANEOUS CRACK - M" });
            descriptions.Add("BMISCCK3", new AttributeDescription() { Description = "BITUMINIOUS MISCELLANEOUS CRACK - H" });
            descriptions.Add("BPATCHCT", new AttributeDescription() { Description = "BITUMINIOUS PATCHING COUNT" });
            descriptions.Add("BPATCHSF", new AttributeDescription() { Description = "BITUMINIOUS PATCHING AREA (SF)" });
            descriptions.Add("BRAVLWT2", new AttributeDescription() { Description = "BITUMINIOUS RAVEL./WEATHER. AREA - M" });
            descriptions.Add("BRAVLWT3", new AttributeDescription() { Description = "BITUMINIOUS RAVEL./WEATHER. AREA - H" });
            descriptions.Add("BRRUTDP1", new AttributeDescription() { Description = "BITUMINIOUS RUTTING RIGHT - L" });
            descriptions.Add("BRRUTDP2", new AttributeDescription() { Description = "BITUMINIOUS RUTTING RIGHT - M" });
            descriptions.Add("BRRUTDP3", new AttributeDescription() { Description = "BITUMINIOUS RUTTING RIGHT - H" });
            descriptions.Add("BTRNSCT1", new AttributeDescription() { Description = "BITUMINIOUS TRANS. CRACK COUNT - L" });
            descriptions.Add("BTRNSCT2", new AttributeDescription() { Description = "BITUMINIOUS TRANS. CRACK COUNT - M" });
            descriptions.Add("BTRNSCT3", new AttributeDescription() { Description = "BITUMINIOUS TRANS. CRACK COUNT - H" });
            descriptions.Add("BTRNSFT1", new AttributeDescription() { Description = "BITUMINIOUS TRANS. CRACK LENGTH - L" });
            descriptions.Add("BTRNSFT2", new AttributeDescription() { Description = "BITUMINIOUS TRANS. CRACK LENGTH - M" });
            descriptions.Add("BTRNSFT3", new AttributeDescription() { Description = "BITUMINIOUS TRANS. CRACK LENGTH - H" });
            descriptions.Add("BUSIPLAN", new AttributeDescription() { Description = "" });
            descriptions.Add("CBPATCCT", new AttributeDescription() { Description = "CONCRETE BITUMINIOUS PATCH COUNT" });
            descriptions.Add("CBPATCSF", new AttributeDescription() { Description = "CONCRETE BITUMINIOUS PATCH AREA (SF)" });
            descriptions.Add("CBRKSLB1", new AttributeDescription() { Description = "CONCRETE BROKEN SLAB COUNT - L" });
            descriptions.Add("CBRKSLB2", new AttributeDescription() { Description = "CONCRETE BROKEN SLAB COUNT - M" });
            descriptions.Add("CBRKSLB3", new AttributeDescription() { Description = "CONCRETE BROKEN SLAB COUNT - H" });
            descriptions.Add("CFLTJNT2", new AttributeDescription() { Description = "CONCRETE FAULTED JOINT COUNT - M" });
            descriptions.Add("CFLTJNT3", new AttributeDescription() { Description = "CONCRETE FAULTED JOINT COUNT - H" });
            descriptions.Add("CJOINTCT", new AttributeDescription() { Description = "CONCRETE JOINT COUNT" });
            descriptions.Add("CLJCPRU1", new AttributeDescription() { Description = "CONCRETE LEFT JCP RUTTING - L" });
            descriptions.Add("CLJCPRU2", new AttributeDescription() { Description = "CONCRETE LEFT JCP RUTTING - M" });
            descriptions.Add("CLJCPRU3", new AttributeDescription() { Description = "CONCRETE LEFT JCP RUTTING - H" });
            descriptions.Add("CLNGCRK1", new AttributeDescription() { Description = "CONCRETE LONG. CRACK COUNT - L" });
            descriptions.Add("CLNGCRK2", new AttributeDescription() { Description = "CONCRETE LONG. CRACK COUNT - M" });
            descriptions.Add("CLNGCRK3", new AttributeDescription() { Description = "CONCRETE LONG. CRACK COUNT - H" });
            descriptions.Add("CLNGJNT1", new AttributeDescription() { Description = "CONCRETE LONG. JOINT SPALL LENGTH - L" });
            descriptions.Add("CLNGJNT2", new AttributeDescription() { Description = "CONCRETE LONG. JOINT SPALL LENGTH - M" });
            descriptions.Add("CLNGJNT3", new AttributeDescription() { Description = "CONCRETE LONG. JOINT SPALL LENGTH - H" });
            descriptions.Add("CNSLABCT", new AttributeDescription() { Description = "CONCRETE SLAB COUNT" });
            descriptions.Add("CNTY", new AttributeDescription() { Description = "" });
            descriptions.Add("County", new AttributeDescription() { Description = "County1" });
            descriptions.Add("CPCCPACT", new AttributeDescription() { Description = "CONCRETE PCC PATCH COUNT" });
            descriptions.Add("CPCCPASF", new AttributeDescription() { Description = "CONCRETE PCC PATCH AREA (SF)" });
            descriptions.Add("CRJCPRU1", new AttributeDescription() { Description = "CONCRETE RIGHT JCP RUTTING - L" });
            descriptions.Add("CRJCPRU2", new AttributeDescription() { Description = "CONCRETE RIGHT JCP RUTTING - M" });
            descriptions.Add("CRJCPRU3", new AttributeDescription() { Description = "CONCRETE RIGHT JCP RUTTING - H" });
            descriptions.Add("CRS_Data", new AttributeDescription() { Description = "CRS_Data1" });
            descriptions.Add("CTRNCRK1", new AttributeDescription() { Description = "CONCRETE TRANS. CRACK COUNT - L" });
            descriptions.Add("CTRNCRK2", new AttributeDescription() { Description = "CONCRETE TRANS. CRACK COUNT - M" });
            descriptions.Add("CTRNCRK3", new AttributeDescription() { Description = "CONCRETE TRANS. CRACK COUNT - H" });
            descriptions.Add("CTRNJNT1", new AttributeDescription() { Description = "CONCRETE TRANS. JOINT SPALL COUNT - L" });
            descriptions.Add("CTRNJNT2", new AttributeDescription() { Description = "CONCRETE TRANS. JOINT SPALL COUNT - M" });
            descriptions.Add("CTRNJNT3", new AttributeDescription() { Description = "CONCRETE TRANS. JOINT SPALL COUNT - H" });
            descriptions.Add("DIRECTION", new AttributeDescription() { Description = "" });
            descriptions.Add("DIS_IND", new AttributeDescription() { Description = "" });
            descriptions.Add("DIST", new AttributeDescription() { Description = "" });
            descriptions.Add("EXP_IND", new AttributeDescription() { Description = "" });
            descriptions.Add("Family", new AttributeDescription() { Description = "" });
            descriptions.Add("FedAid", new AttributeDescription() { Description = "" });
            descriptions.Add("Interstate", new AttributeDescription() { Description = "" });
            descriptions.Add("HMPS", new AttributeDescription() { Description = "" });
            descriptions.Add("L_S_TYPE", new AttributeDescription() { Description = "" });
            descriptions.Add("LANES", new AttributeDescription() { Description = "" });
            descriptions.Add("YR_LST_STRUCT_OVER", new AttributeDescription() { Description = "" });
            descriptions.Add("MPO/RPO", new AttributeDescription() { Description = "MPO/RPO Code" });
            descriptions.Add("NHS_IND", new AttributeDescription() { Description = "" });
            descriptions.Add("COPI", new AttributeDescription() { Description = "" });
            descriptions.Add("R_S_TYPE", new AttributeDescription() { Description = "" });
            descriptions.Add("RiskScore", new AttributeDescription() { Description = "" });
            descriptions.Add("ROUGAVE", new AttributeDescription() { Description = "" });
            descriptions.Add("LENGTH", new AttributeDescription() { Description = "" });
            descriptions.Add("SR", new AttributeDescription() { Description = "" });
            descriptions.Add("SURFACE NAME", new AttributeDescription() { Description = "" });
            descriptions.Add("SURFACE", new AttributeDescription() { Description = "" });
            descriptions.Add("TRK_PCNT", new AttributeDescription() { Description = "" });
            descriptions.Add("U_R_CODE", new AttributeDescription() { Description = "" });
            descriptions.Add("WIDTH", new AttributeDescription() { Description = "" });
            descriptions.Add("YR_LST_RESURFACE", new AttributeDescription() { Description = "" });
            descriptions.Add("YR_BUILT", new AttributeDescription() { Description = "" });

            descriptions.Add("Section", new AttributeDescription() { Description = "Section1" });

            //Unique Fields from ExcelSpreadsheet 
            descriptions.Add("DIR", new AttributeDescription() { Description = "" });
            descriptions.Add("ESALS", new AttributeDescription() { Description = "" });
            descriptions.Add("F_CLASS", new AttributeDescription() { Description = "" });
            descriptions.Add("F_CLASS_NAME", new AttributeDescription() { Description = "" });
            descriptions.Add("FED AID NAME", new AttributeDescription() { Description = "" });
            descriptions.Add("ID", new AttributeDescription() { Description = "ID" });
            descriptions.Add("INSPECT DATE", new AttributeDescription() { Description = "" });
            descriptions.Add("INSPECTYEAR", new AttributeDescription() { Description = "" });
            descriptions.Add("PAVED_THICKNESS", new AttributeDescription() { Description = "" });
            descriptions.Add("SEG", new AttributeDescription() { Description = "" });
            descriptions.Add("SURDATA", new AttributeDescription() { Description = "" });
            descriptions.Add("THICKNESS", new AttributeDescription() { Description = "" });
            descriptions.Add("TRUEDATE", new AttributeDescription() { Description = "" });
            descriptions.Add("U_R NAME", new AttributeDescription() { Description = "" });
            descriptions.Add("UPLOADDATE", new AttributeDescription() { Description = "" });



            return descriptions;
        }



    }
}
