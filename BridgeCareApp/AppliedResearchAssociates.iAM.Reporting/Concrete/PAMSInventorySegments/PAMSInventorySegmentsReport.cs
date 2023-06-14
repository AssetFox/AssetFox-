using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.Data;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting.Models;
using Microsoft.VisualBasic;
using NetTopologySuite.Algorithm;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSInventorySegmentsReport : IReport
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

        private PAMSParameters _failedQuery = new PAMSParameters { County = "unknown", Route = 0, Segment = 0 };

        private Dictionary<string, string> _sectionData;
        private InventoryParameters sectionIds;

        public PAMSInventorySegmentsReport(IUnitOfWork uow, string name, ReportIndexDTO results)
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

            var sectionIds = Parse(parameters);
            _sectionData = GetAsset(sectionIds);

            var crspieces = _sectionData.FirstOrDefault(_ => _.Key == "CRS_Data").Value.Split(new[] { '_' }, 4);
            var routeArray = crspieces[3].Split(new[] { '-' }, 2);
            _sectionData.Add("FROMSEGMENT", routeArray[0].ToString());
            _sectionData.Add("TOSEGMENT", routeArray[1].ToString());

            var resultsString = new StringBuilder();
            resultsString.Append("<table class=\"report-cell\">");
            resultsString.Append(CreateHTMLSection("ID", new List<string>() { "COUNTY", "SR", "TOSEGMENT", "CRS_DATA" }));
            resultsString.Append(CreateHTMLSection("Description", new List<string>() { "DIRECTION", "DIST", "MPO/RPO", "U_R_CODE", "BUSIPLAN", "AADT", "ADTT", "TRK_PCNT", "SURDATA", "", "FEDAID", "HPMS", "LANES", "", "LENGTH", "WIDTH", "AGE","" }));
            resultsString.Append(CreateHTMLSection("Surface Attributes", new List<string>() { "SURFACE NAME", "SURFACE", "L_S_TYPE", "R_S_TYPE", "YR_BUILT","", "YR_LST_RESURFACE", "YR_LST_STRUCT_OVER" }));
            resultsString.Append(CreateHTMLSection("Survey Information", new List<string>() { "Survey Date" }));
            resultsString.Append(CreateHTMLSection("Measured Conditions", new List<string>() { "OPI", "ROUGAVE" }));
            resultsString.Append(CreateHTMLDistressSection("Surface Defects"));

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

        //private List<SegmentAttributeDatum> GetAsset(PAMSParameters keyProperties)
        private Dictionary<string, string> GetAsset(PAMSParameters keyProperties)
        {

            //var attributeList = new List<string>() {"County","SR"};

            var allAttributes = _unitofwork.AttributeRepo.GetAttributes();
            allAttributes.Add(new AttributeDTO() { Name = "Segment", Command = "SEG", DataSource = allAttributes.Single(_ => _.Name == "COUNTY").DataSource });
            var queryDictionary = new Dictionary<AttributeDTO, string>();
            queryDictionary.Add(allAttributes.Single(_ => _.Name == "COUNTY"), keyProperties.County);
            queryDictionary.Add(allAttributes.Single(_ => _.Name == "SR"), keyProperties.Route.ToString());
            queryDictionary.Add(allAttributes.Single(_ => _.Name == "Segment"), keyProperties.Segment.ToString());

            var tmpsectionData = _unitofwork.DataSourceRepo.GetRawData(queryDictionary);
            var sectionId = tmpsectionData["CRS_Data"];
            var result = _unitofwork.AssetDataRepository.GetAssetAttributes("CRS", sectionId);

           // return result;
            return tmpsectionData;
        }


        private string GetAttribute(string attributeName, bool previous = false)
        {
            var returnVal = _sectionData.FirstOrDefault(_ => _.Key.ToUpper() == attributeName.ToUpper());

            var returnstr = string.Empty;

            if (attributeName is "")
            {
                returnstr = "";
            }
            else if (returnVal.Value == null)
            {
                returnstr = DEFAULT_VALUE;
            }
            else
            {
                returnstr = returnVal.Value;
            }

             return returnstr;
            //return (returnVal.Value == null) ? DEFAULT_VALUE : returnVal.Value;
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
            var sectionString = new StringBuilder($"<tr><th colspan=\"4\" class=\"report-header report-cell\" style=\"text-align:left;vertical-align:middle;background-color:lightgrey;\">{sectionName}</th></tr>");
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


        private string CreateHTMLDistressSection(string sectionName, bool previous = false)
        {
            var sectionString = new StringBuilder($"<tr><th colspan=\"4\" class=\"report-header report-cell\" style=\"text-align:left;vertical-align:middle;background-color:lightgrey;\">{sectionName}</th></tr>");

            var tmpsurfaceID = GetAttribute("SURFACE", previous);
            int surfaceID;

            bool success = int.TryParse(tmpsurfaceID, out surfaceID);
            if (!success)
            {
                surfaceID = 0;
            }

            if (surfaceID < 63)
            {


                sectionString.Append($"<tr><td colspan=\"4\" class=\"report-description report-cell\" style=\"text-align:left;font-style: italic;vertical-align:middle;background-color:transparent;\">Asphalt</td></tr>");
                sectionString.Append($"<tr><td class=\"report-data report-cell\" style=\"text-align:left;text-decoration: underline;vertical-align:middle;background-color:transparent;\">Type</td>" +
                                          "<td class=\"report-data report-cell\" style=\"text-align:center;text-decoration: underline;vertical-align:middle;background-color:transparent;\">L</td>" +
                                          "<td class=\"report-data report-cell\" style=\"text-align:center;text-decoration: underline;vertical-align:middle;background-color:transparent;\">M</td>" +
                                          "<td class=\"report-data report-cell\" style=\"text-align:center;text-decoration: underline;vertical-align:middle;background-color:transparent;\">H</td></tr>");

                string[,] distressArray = new string[9, 4]
                    {
                    { "Edge Deterioration","BEDGDTR1", "BEDGDTR2", "BEDGDTR3" },
                    { "Fatigue Cracking","BFATICR1", "BFATICR2", "BFATICR3" },
                    { "Left Rut Depth","BLRUTDP1", "BLRUTDP2", "BLRUTDP3" },
                    { "Right Rut Depth","BRRUTDP1", "BRRUTDP2", "BRRUTDP3" },
                    { "Left Edge Joint","BLTEDGE1", "BLTEDGE2", "BLTEDGE3" },
                    { "Misc.Cracking", "BMISCCK1", "BMISCCK2", "BMISCCK3" },
                    { "Raveling / Weathering","BRAVLWT1", "BRAVLWT2", "BRAVLWT3" },
                    { "Trans.Cracking(Ct.)","BTRNSCT1", "BTRNSCT2", "BTRNSCT3" },
                    { "Trans.Cracking(Length)","BTRNSFT1", "BTRNSFT2", "BTRNSFT3" }
                    };

                for (int tmprow = 0; tmprow < distressArray.GetLength(0); tmprow++)
                {
                    for (int tmpcol = 0; tmpcol < 1; tmpcol++)
                    {
                        sectionString.Append($"<tr><td class=\"report-description report-cell\" style=\"text-align:left;width:25%;vertical-align:middle;background-color:transparent;\" >{GetDescription(distressArray[tmprow, tmpcol])}</td>");
                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute(distressArray[tmprow, tmpcol + 1], previous)}</td>");
                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute(distressArray[tmprow, tmpcol + 2], previous)}</td>");
                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute(distressArray[tmprow, tmpcol + 3], previous)}</td>");
                    }
                    sectionString.Append($"</tr>");
                }

                sectionString.Append($"<tr><td colspan=\"4\" class=\"report-description report-cell\"></td></tr>");
                sectionString.Append($"<tr><td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\"></td>" +
                              "<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;text-decoration: underline;background-color:transparent;\">Count</td>" +
                              "<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;text-decoration: underline;background-color:transparent;\">Area</td>" +
                              "<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\"></td></tr>");

                sectionString.Append($"<tr><td class=\"report-description report-cell\" style=\"text-align:left;width:25%;vertical-align:middle;background-color:transparent;\" >Patching</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CPCCPACT", previous)}</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CPCCPASF", previous)}</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" ></td></tr>");
                sectionString.Append($"<tr><td colspan=\"4\" class=\"report-description report-cell\"></td></tr>");
            }
            else
            {
                sectionString.Append($"<tr><td colspan=\"4\" class=\"report-description report-cell\" style=\"text-align:left;font-style: italic;vertical-align:middle;background-color:lightred;\">Jointed Concrete</td></tr>");

                sectionString.Append($"<tr><td class=\"report-description report-cell\" style=\"text-align:left;width:25%;vertical-align:middle;background-color:transparent;\">Number of Slabs</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CNSLABCT", previous)}</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" ></td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" ></td></tr>");

                sectionString.Append($"<tr><td class=\"report-description report-cell\" style=\"text-align:left;width:25%;vertical-align:middle;background-color:transparent;\">Joint Count</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CJOINTCT", previous)}</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" ></td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" ></td></tr>");
                sectionString.Append($"<tr><td colspan=\"4\" class=\"report-description report-cell\"></td></tr>");

                sectionString.Append($"<tr><td class=\"report-data report-cell\" style=\"text-align:left;text-decoration: underline;vertical-align:middle;background-color:transparent;\">Type</td>" +
                                          "<td class=\"report-data report-cell\" style=\"text-align:center;text-decoration: underline;vertical-align:middle;background-color:transparent;\">L</td>" +
                                          "<td class=\"report-data report-cell\" style=\"text-align:center;text-decoration: underline;vertical-align:middle;background-color:transparent;\">M</td>" +
                                          "<td class=\"report-data report-cell\" style=\"text-align:center;text-decoration: underline;vertical-align:middle;background-color:transparent;\">H</td></tr>");

                string[,] patchingArray = new string[8, 4]
                {
                { "Broken Slab","CBRKSLB1", "CBRKSLB2", "CBRKSLB3" },
                { "Faulted Joint","CFLTJNT1", "CFLTJNT2", "CFLTJNT3" },
                { "Longitudinal Joint Spalling","CLNGJNT1", "CLNGJNT2", "CLNGJNT3" },
                { "Longitudinal Cracking","CLNGCRK1", "CLNGCRK2", "CLNGCRK3" },

                { "Transverse Joint Spalling","CTRNJNT1", "CTRNJNT2", "CTRNJNT3" },
                { "Transverse Cracking","CTRNCRK1", "CTRNCRK2", "CTRNCRK3" },
                { "Left Rut Depth","CLJCPRU1", "CLJCPRU2", "CLJCPRU3" },
                { "Right Rut Depth","CRJCPRU1", "CRJCPRU2", "CRJCPRU3" },
                };

                for (int tmprow = 0; tmprow < patchingArray.GetLength(0); tmprow++)
                {

                    for (int tmpcol = 0; tmpcol < 1; tmpcol++)
                    {
                        sectionString.Append($"<tr><td class=\"report-description report-cell\" style=\"text-align:left;width:25%;vertical-align:middle;background-color:transparent;\" >{GetDescription(patchingArray[tmprow, tmpcol])}</td>");
                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute(patchingArray[tmprow, tmpcol + 1], previous)}</td>");
                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute(patchingArray[tmprow, tmpcol + 2], previous)}</td>");
                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute(patchingArray[tmprow, tmpcol + 3], previous)}</td>");
                    }
                    sectionString.Append($"</tr>");
                }

                sectionString.Append($"<tr><td colspan=\"4\" class=\"report-description report-cell\"></td></tr>");
                sectionString.Append($"<tr><td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\"></td>" +
                  "<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;text-decoration: underline;background-color:transparent;\">Count</td>" +
                  "<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;text-decoration: underline;background-color:transparent;\">Area</td>" +
                  "<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\"></td></tr>");

                sectionString.Append($"<tr><td class=\"report-description report-cell\" style=\"text-align:left;width:25%;vertical-align:middle;background-color:transparent;\" >Bituminous Patching</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CBPATCCT", previous)}</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CBPATCSF", previous)}</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" ></td></tr>");

                sectionString.Append($"<tr><td class=\"report-description report-cell\" style=\"text-align:left;width:25%;vertical-align:middle;background-color:transparent;\">Concrete Patching</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CPCCPACT", previous)}</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CPCCPASF", previous)}</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" ></td></tr>");

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

            descriptions.Add("AADT", new AttributeDescription() { Description = "ADT" });
            descriptions.Add("AGE", new AttributeDescription() { Description = "Age" });
            descriptions.Add("BUSIPLAN", new AttributeDescription() { Description = "BPN" });
            descriptions.Add("CNTY", new AttributeDescription() { Description = "County ID" });
            descriptions.Add("COPI", new AttributeDescription() { Description = "COPI" });
            descriptions.Add("COUNTY", new AttributeDescription() { Description = "County" });
            descriptions.Add("CRS_DATA", new AttributeDescription() { Description = "Section" });
            descriptions.Add("CRSDATA", new AttributeDescription() { Description = "Section" });
            descriptions.Add("DIRECTION", new AttributeDescription() { Description = "Direction" });
            descriptions.Add("DIS_IND", new AttributeDescription() { Description = "DIS_IND" });
            descriptions.Add("DIST", new AttributeDescription() { Description = "District" });
            descriptions.Add("ESALS", new AttributeDescription() { Description = "ESALS" });
            descriptions.Add("EXP_IND", new AttributeDescription() { Description = "EXP_IND" });
            descriptions.Add("F_CLASS", new AttributeDescription() { Description = "F_CLASSD" });
            descriptions.Add("F_CLASS_NAME", new AttributeDescription() { Description = "F_CLASS_NAME" });
            descriptions.Add("FAMILY", new AttributeDescription() { Description = "Family" });
            descriptions.Add("FED_AID", new AttributeDescription() { Description = "Federal Aid?" });
            descriptions.Add("FEDAID", new AttributeDescription() { Description = "Federal Aid?" });
            descriptions.Add("FROMSEGMENT", new AttributeDescription() { Description = "From Segment" });
            descriptions.Add("HPMS", new AttributeDescription() { Description = "HPMS?" });
            descriptions.Add("ID", new AttributeDescription() { Description = "ID" });
            descriptions.Add("INTERSTATE", new AttributeDescription() { Description = "Interstate" });
            descriptions.Add("L_S_TYPE", new AttributeDescription() { Description = "Left Shoulder Type" });
            descriptions.Add("LANES", new AttributeDescription() { Description = "Lanes" });
            descriptions.Add("LENGTH", new AttributeDescription() { Description = "Length" });
            descriptions.Add("MPO/RPO", new AttributeDescription() { Description = "MPO/RPO" });
            descriptions.Add("NHS_IND", new AttributeDescription() { Description = "NHS_IND" });
            descriptions.Add("PAVED_THICKNESS", new AttributeDescription() { Description = "PAVED_THICKNESS" });
            descriptions.Add("R_S_TYPE", new AttributeDescription() { Description = "Right Shoulder Type" });
            descriptions.Add("RISKSCORE", new AttributeDescription() { Description = "RiskScore" });
            descriptions.Add("ROUGAVE", new AttributeDescription() { Description = "Roughness" });
            descriptions.Add("SECTION", new AttributeDescription() { Description = "SEC" });
            descriptions.Add("SEG", new AttributeDescription() { Description = "SEG" });
            descriptions.Add("SR", new AttributeDescription() { Description = "Route" });
            descriptions.Add("SURDATA", new AttributeDescription() { Description = "Surface" });
            descriptions.Add("SURFACE", new AttributeDescription() { Description = "Surface ID" });
            descriptions.Add("SURFACE NAME", new AttributeDescription() { Description = "Surface" });
            descriptions.Add("TOSEGMENT", new AttributeDescription() { Description = "Segment" });
            descriptions.Add("THICKNESS", new AttributeDescription() { Description = "THICKNESS" });
            descriptions.Add("TRK_PCNT", new AttributeDescription() { Description = "Truck %" });
            descriptions.Add("TRUEDATE", new AttributeDescription() { Description = "TrueDate" });
            descriptions.Add("U_R NAME", new AttributeDescription() { Description = "U_R NAME" });
            descriptions.Add("U_R_CODE", new AttributeDescription() { Description = "Urban/Rural" });
            descriptions.Add("WIDTH", new AttributeDescription() { Description = "Width" });
            descriptions.Add("YR_BUILT", new AttributeDescription() { Description = "Year Built" });
            descriptions.Add("YR_LST_RESURFACE", new AttributeDescription() { Description = "Last Overlay" });
            descriptions.Add("YR_LST_STRUCT_OVER", new AttributeDescription() { Description = "Last Structural Overlay" });


            return descriptions;
        }



    }
}
