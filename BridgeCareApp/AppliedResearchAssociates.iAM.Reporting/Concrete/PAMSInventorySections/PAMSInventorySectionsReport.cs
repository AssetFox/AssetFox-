using System;
using System.Collections;
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
using Humanizer;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using static System.Collections.Specialized.BitVector32;

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

        private List<SegmentAttributeDatum> _sectionData;
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
            //_networkId = _unitofwork.NetworkRepo.GetMainNetwork().Id;

        }

        // public Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null) => throw new NotImplementedException();

        public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            
            var sectionIds = Parse(parameters);
            _sectionData = GetAsset(sectionIds);
            var crspieces = _sectionData.FirstOrDefault(_ => _.Name == "CRS").Value.Split(new[] { '_' }, 4);
            var routeArray = crspieces[3].Split(new[] { '-' }, 2);
            _sectionData.Add(new SegmentAttributeDatum("FROMSEGMENT", routeArray[0]));
            _sectionData.Add(new SegmentAttributeDatum("TOSEGMENT", routeArray[1]));
            //_sectionData.Add(new SegmentAttributeDatum("SECTION", crspieces[1]));

            var resultsString = new StringBuilder();
            resultsString.Append("<table class=\"report-cell\">");
            resultsString.Append(CreateHTMLSection("ID", new List<string>() { "CRS", "County", "SR", "FROMSEGMENT", "ToSegment", "Member Segments" }));
            resultsString.Append(CreateHTMLSection("Description", new List<string>() { "Direction", "District", "MPO_RPO", "U_R_CODE", "BPN", "ADT", "ADTT", "Truck %", "Surface", "Federal Aid?", "HPMS?", "Lanes", "Length", "Width", "Age" }));
            resultsString.Append(CreateHTMLSection("Surface Attributes", new List<string>() { "Surface", "Surface ID", "Left Shoulder Type", "Right Shoulder Type", "Year Built", "Last Overlay", "Last Structural Overlay" }));
            resultsString.Append(CreateHTMLSection("Survey Information", new List<string>() { "Survey Date" }));
            resultsString.Append(CreateHTMLSection("Measured Conditions", new List<string>() { "OPI", "Roughness" }));
            resultsString.Append(CreateHTMLSection("Surface Defects", new List<string>() { "Type" }));

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

            var tmpsectionData = _unitofwork.DataSourceRepo.GetRawData(queryDictionary);
            var sectionId = tmpsectionData["CRS_Data"];
            var result = _unitofwork.AssetDataRepository.GetAssetAttributes("CRS", sectionId);
            return result;
        }


        private string GetAttribute(string attributeName, bool previous=false)
        {
            var returnVal = _sectionData.FirstOrDefault(_ => _.Name == attributeName.ToUpper());
            return (returnVal == null) ? DEFAULT_VALUE : returnVal.Value;
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
            descriptions.Add("County", new AttributeDescription() { Description = "County" });
            descriptions.Add("CPCCPACT", new AttributeDescription() { Description = "CONCRETE PCC PATCH COUNT" });
            descriptions.Add("CPCCPASF", new AttributeDescription() { Description = "CONCRETE PCC PATCH AREA (SF)" });
            descriptions.Add("CRJCPRU1", new AttributeDescription() { Description = "CONCRETE RIGHT JCP RUTTING - L" });
            descriptions.Add("CRJCPRU2", new AttributeDescription() { Description = "CONCRETE RIGHT JCP RUTTING - M" });
            descriptions.Add("CRJCPRU3", new AttributeDescription() { Description = "CONCRETE RIGHT JCP RUTTING - H" });
            descriptions.Add("CRS_Data", new AttributeDescription() { Description = "Section" });
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
            descriptions.Add("SR", new AttributeDescription() { Description = "Route" });
            descriptions.Add("SURFACE NAME", new AttributeDescription() { Description = "" });
            descriptions.Add("SURFACE", new AttributeDescription() { Description = "" });
            descriptions.Add("TRK_PCNT", new AttributeDescription() { Description = "" });
            descriptions.Add("U_R_CODE", new AttributeDescription() { Description = "Urban/RuralXXX" });
            descriptions.Add("WIDTH", new AttributeDescription() { Description = "" });
            descriptions.Add("YR_LST_RESURFACE", new AttributeDescription() { Description = "" });
            descriptions.Add("YR_BUILT", new AttributeDescription() { Description = "" });
            descriptions.Add("DIR", new AttributeDescription() { Description = "Direction" });
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
            descriptions.Add("CRS", new AttributeDescription() { Description = "Section" });
            descriptions.Add("MPO_RPO", new AttributeDescription() { Description = "MPO/RPO" });
            descriptions.Add("DISTRICT", new AttributeDescription() { Description = "" });
            descriptions.Add("FED_AID", new AttributeDescription() { Description = "" });
            descriptions.Add("SURFACEID", new AttributeDescription() { Description = "" });
            descriptions.Add("SEGMENT_LENGTH", new AttributeDescription() { Description = "" });
            descriptions.Add("TRK_PERCENT", new AttributeDescription() { Description = "" });
            descriptions.Add("OPI", new AttributeDescription() { Description = "OPIxx" });
            descriptions.Add("LAST_STRUCTURAL_OVERLAY", new AttributeDescription() { Description = "" });
            descriptions.Add("YEAR_LAST_OVERLAY", new AttributeDescription() { Description = "" });
            descriptions.Add("SURFACE_NAME", new AttributeDescription() { Description = "" });
            descriptions.Add("ROUGHNESS", new AttributeDescription() { Description = "" });
            descriptions.Add("FROMSEGMENT", new AttributeDescription() { Description = "From SegmentXXX" });
            descriptions.Add("TOSEGMENT", new AttributeDescription() { Description = "To Segment" });
           // descriptions.Add("SECTION", new AttributeDescription() { Description = "Section" });

            return descriptions;
        }



    }
}
