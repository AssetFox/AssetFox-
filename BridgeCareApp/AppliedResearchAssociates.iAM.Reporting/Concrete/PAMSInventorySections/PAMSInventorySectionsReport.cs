using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Common;
using AppliedResearchAssociates.iAM.Common.Logging;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Reporting.Models;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace AppliedResearchAssociates.iAM.Reporting
{
    public class PAMSInventorySectionsReport : IReport
    {

        public static string networkType { get; private set; }
        private const string DEFAULT_VALUE = "N";
        private const int DEFAULT_COLUMNS = 2;
        private IUnitOfWork _unitofwork;
        private Guid _networkId;
        private Dictionary<string, AttributeDescription> _fieldDescriptions;

        public string Suffix { get; private set; }
        public Guid ID { get; set; }
        public Guid? SimulationID { get => null; set { } }
        public Guid? NetworkID { get; set; }
        public string Results { get; private set; }
        public ReportType Type => ReportType.HTML;
        public string ReportTypeName { get; private set; }
        public List<string> Errors { get; private set; }
        public bool IsComplete { get; private set; }
        public string Status { get; private set; }
        public string Criteria { get; set; }

        private PAMSParameters _failedQuery = new PAMSParameters { County = "unknown", SR = 0, SEG = "0" };

        private List<SegmentAttributeDatum> _sectionData;
        private InventoryParameters sectionIds;

        public PAMSInventorySectionsReport(IUnitOfWork uow, string name, ReportIndexDTO results, string suffix)
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
            Suffix = suffix;
            if (suffix == ReportSuffixType.primaryDataSuffix)
            {
                var primaryNetworkId = _unitofwork.AdminSettingsRepo.GetPrimaryNetworkId();
                if (primaryNetworkId == null)
                {
                    Errors.Add("Does not have a primary network");
                }
                else
                {
                    _networkId = primaryNetworkId.Value;
                }
            }
            else
            {
                var rawNetworkId = _unitofwork.AdminSettingsRepo.GetRawDataNetworkId();
                if (rawNetworkId == null)
                {
                    Errors.Add("Does not have a raw network");
                }
                else
                {
                    _networkId = rawNetworkId.Value;
                }
            }
        }

         public async Task Run(string parameters, CancellationToken? cancellationToken = null, IWorkQueueLog workQueueLog = null)
        {
            if (Errors.Count > 0) return; // Errors occured in the GetAsset method

            var sectionIds = JsonConvert.DeserializeObject<PAMSParameters>(parameters);
            _sectionData = GetAsset(sectionIds);
            if (Errors.Count > 0) return; // Errors occured in the GetAsset method

            var crspieces = _sectionData.FirstOrDefault(_ => _.Name == "CRS").Value.Split(new[] { '_' }, 4);
            var routeArray = crspieces[3].Split(new[] { '-' }, 2);
            _sectionData.Add(new SegmentAttributeDatum("FROMSEGMENT", routeArray[0]));
            _sectionData.Add(new SegmentAttributeDatum("TOSEGMENT", routeArray[1]));

            var resultsString = new StringBuilder();
            resultsString.Append("<table class=\"report-cell\">");
            resultsString.Append(CreateHTMLSection("ID", new List<string>() { "CRS", "", "COUNTY", "SR", "FROMSEGMENT", "TOSEGMENT", "Member Segments" }));
            resultsString.Append(CreateHTMLSection("Description", new List<string>() { "DIRECTION", "DISTRICT", "MPO_RPO", "U_R_CODE", "BUSIPLAN", "AADT", "ADTT", "TRK_PERCENT", "SURFACE", "", "FED_AID", "IS_HPMS", "LANES", "", "SEGMENT_LENGTH", "WIDTH", "AGE", ""}));
            resultsString.Append(CreateHTMLSection("Surface Attributes", new List<string>() { "SURFACE_NAME", "SURFACEID", "L_S_TYPE","R_S_TYPE", "YR_BUILT", "", "YEAR_LAST_OVERLAY", "LAST_STRUCTURAL_OVERLAY" }));
            resultsString.Append(CreateHTMLSection("Survey Information", new List<string>() { "Survey Date", ""}));
            resultsString.Append(CreateHTMLSection("Measured Conditions", new List<string>() { "OPI", "ROUGHNESS" }));
            resultsString.Append(CreateHTMLDistressSection("Surface Defects"));

            resultsString.Append("</table>");
            Results = resultsString.ToString();
            IsComplete = true;
            return;

        }

        private class IncomingParameters {
            public PAMSParameters KeyProperties { get; set; }
        }

        private PAMSParameters Parse(string parameters)
        {
            try
            {
                IncomingParameters incoming = JsonConvert.DeserializeObject<IncomingParameters>(parameters);
                if (incoming.KeyProperties == null)
                {
                    Errors.Add($"Unable to run.  No query parameters provided in request body.");
                    return _failedQuery;
                }
                return incoming.KeyProperties;
                //return new PAMSParameters();
            }
            catch (Exception e)
            {
                Errors.Add($"Failed to parse JSON in request body due to {e.Message}");
                return _failedQuery;
            }
        }

        public void NetworkStringType(string interimReportType)
        {
            networkType = interimReportType;
        }

        private List<SegmentAttributeDatum> GetAsset(PAMSParameters keyProperties)
        {
            List<SegmentAttributeDatum> result = new List<SegmentAttributeDatum>();
            //var attributeList = new List<string>() {"County", "SR"};

            var allAttributes = _unitofwork.AttributeRepo.GetAttributes();
            allAttributes.Add(new AttributeDTO() { Name = "Segment", Command = "SEG", DataSource = allAttributes.Single(_ => _.Name == "COUNTY").DataSource});
            var queryDictionary= new Dictionary<AttributeDTO, string>();
            try
            {
                queryDictionary.Add(allAttributes.Single(_ => _.Name == "COUNTY"), keyProperties.County);
                queryDictionary.Add(allAttributes.Single(_ => _.Name == "SR"), keyProperties.SR.ToString());
                queryDictionary.Add(allAttributes.Single(_ => _.Name == "SEG"), keyProperties.SEG.ToString());
            }
            catch
            {
                var errorMessage = $"Unable to find the segment in the database (County: {keyProperties.County}, Route: {keyProperties.SR}, Segment: {keyProperties.SEG}";
                Errors.Add(errorMessage);
                return new List<SegmentAttributeDatum>();
                //throw new RowNotInTableException(errorMessage);
            }

            if (Suffix == ReportSuffixType.rawDataSuffix)
            {
                try
                {
                    var queryStringDictionary = queryDictionary.ToDictionary(_ => _.Key.Name, _ => _.Value);
                    var tmpsectionData = _unitofwork.DataSourceRepo.GetRawData(queryDictionary);
                    var sectionId = tmpsectionData["SEG"];
                    result = _unitofwork.AssetDataRepository.GetPAMSAssetAttributes(queryStringDictionary, sectionId);
                }
                catch (Exception)
                {
                    var errorMessage = $"Unable to access raw data";
                    Errors.Add(errorMessage);
                    //throw new InvalidOperationException(errorMessage);
                }
            }
            else if(Suffix == ReportSuffixType.primaryDataSuffix)
            {
                try
                {
                    var tmpsectionData = _unitofwork.DataSourceRepo.GetRawData(queryDictionary);
                    var sectionId = tmpsectionData["CRS_Data"];
                    result = _unitofwork.AssetDataRepository.GetAssetAttributes("CRS", sectionId);
                }
                catch (Exception)
                {
                    var errorMessage = $"Unable to access primary data";
                    Errors.Add(errorMessage);
                    //throw new InvalidOperationException(errorMessage);
                }
            }



            return result;
        }


        private string GetAttribute(string attributeName, bool previous=false)
        {
            var returnVal = _sectionData.FirstOrDefault(_ => _.Name.ToUpper() == attributeName.ToUpper());

            var returnstr = string.Empty;

            if (attributeName is "")
            {
                returnstr = "";
            }
            else if (returnVal == null || returnVal.Value == null)
            {
                returnstr = DEFAULT_VALUE;
            }
            else
            {
                returnstr = returnVal.Value;
            }

            return returnstr;
           // return (returnVal == null) ? DEFAULT_VALUE : returnVal.Value;
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

            var tmpsurfaceID = GetAttribute("SURFACEID", previous);
            int surfaceID;
 
            bool success = int.TryParse(tmpsurfaceID, out surfaceID);
            if (!success)
            {
                surfaceID=0;
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
                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >");

                        Decimal tmpnum;
                        var returnVal = GetAttribute(distressArray[tmprow, tmpcol + 1], previous);
                        string tmpstring = returnVal;

                        if (Decimal.TryParse(tmpstring, out tmpnum))
                        {
                            //tmpstring = Round(tmpnum, 2, MidpointRounding.AwayFromZero);
                            tmpstring = tmpnum.ToString("0.00");
                        }

                        sectionString.Append(tmpstring);
                        sectionString.Append("</td>");

                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >");

                        returnVal = GetAttribute(distressArray[tmprow, tmpcol + 2], previous);
                        tmpstring = returnVal;

                        if (Decimal.TryParse(tmpstring, out tmpnum))
                        {
                            tmpstring = tmpnum.ToString("0.00");
                        }

                        sectionString.Append(tmpstring);
                        sectionString.Append("</td>");

                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >");

                        returnVal = GetAttribute(distressArray[tmprow, tmpcol + 3], previous);
                        tmpstring = returnVal;

                        if (Decimal.TryParse(tmpstring, out tmpnum))
                        {
                            tmpstring = tmpnum.ToString("0.00");
                        }

                        sectionString.Append(tmpstring);
                        sectionString.Append("</td>");

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


                //sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CPCCPASF", previous)}</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >");

                Decimal tmpnum1;
                var returnVal1 = GetAttribute("CPCCPASF", previous);
                string tmpstring1 = returnVal1;

                if (Decimal.TryParse(tmpstring1, out tmpnum1))
                {
                    tmpstring1 = tmpnum1.ToString("0.00");
                }

                sectionString.Append(tmpstring1);
                sectionString.Append("</td>");

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
                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >");

                        Decimal tmpnum2;
                        var returnVal2 = GetAttribute(patchingArray[tmprow, tmpcol + 1], previous);
                        string tmpstring2 = returnVal2;

                        if (Decimal.TryParse(tmpstring2, out tmpnum2))
                        {
                            tmpstring2 = tmpnum2.ToString("0.00");
                        }

                        sectionString.Append(tmpstring2);
                        sectionString.Append("</td>");

                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >");

                        Decimal tmpnum3;
                        var returnVal3 = GetAttribute(patchingArray[tmprow, tmpcol + 2], previous);
                        var tmpstring3 = returnVal3;

                        if (Decimal.TryParse(tmpstring3, out tmpnum3))
                        {
                            tmpstring3 = tmpnum3.ToString("0.00");
                        }

                        sectionString.Append(tmpstring3);
                        sectionString.Append("</td>");

                        sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >");

                        Decimal tmpnum4;
                        var returnVal4 = GetAttribute(patchingArray[tmprow, tmpcol + 3], previous);
                        var tmpstring4 = returnVal4;

                        if (Decimal.TryParse(tmpstring4, out tmpnum4))
                        {
                            tmpstring4 = tmpnum4.ToString("0.00");
                        }

                        sectionString.Append(tmpstring4);
                        sectionString.Append("</td>");

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
               // sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CBPATCSF", previous)}</td>");

                /////
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >");

                Decimal tmpnum5;
                var returnVal5 = GetAttribute("CBPATCSF", previous);
                string tmpstring5 = returnVal5;

                if (Decimal.TryParse(tmpstring5, out tmpnum5))
                {
                    tmpstring5 = tmpnum5.ToString("0.00");
                }

                sectionString.Append(tmpstring5);
                sectionString.Append("</td>");

                //////

                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" ></td></tr>");

                sectionString.Append($"<tr><td class=\"report-description report-cell\" style=\"text-align:left;width:25%;vertical-align:middle;background-color:transparent;\">Concrete Patching</td>");
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CPCCPACT", previous)}</td>");
                //sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >{GetAttribute("CPCCPASF", previous)}</td>");

                /////
                sectionString.Append($"<td class=\"report-data report-cell\" style=\"text-align:center;width:25%;vertical-align:middle;background-color:transparent;\" >");

                Decimal tmpnum6;
                var returnVal6 = GetAttribute("CPCCPASF", previous);
                string tmpstring6 = returnVal6;

                if (Decimal.TryParse(tmpstring6, out tmpnum6))
                {
                    tmpstring6 = tmpnum6.ToString("0.00");
                }

                sectionString.Append(tmpstring6);
                sectionString.Append("</td>");

                //////



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
            descriptions.Add("COPI", new AttributeDescription() { Description = "" });
            descriptions.Add("CNTY", new AttributeDescription() { Description = "County ID" });
            descriptions.Add("COUNTY", new AttributeDescription() { Description = "County" });
            descriptions.Add("CRS", new AttributeDescription() { Description = "Section" });
            descriptions.Add("CRS_DATA", new AttributeDescription() { Description = "Section" });
            descriptions.Add("DIR", new AttributeDescription() { Description = "Direction" });
            descriptions.Add("DIRECTION", new AttributeDescription() { Description = "Direction" });
            descriptions.Add("DIS_IND", new AttributeDescription() { Description = "" });
            descriptions.Add("DIST", new AttributeDescription() { Description = "District" });
            descriptions.Add("DISTRICT", new AttributeDescription() { Description = "District" });
            descriptions.Add("ESALS", new AttributeDescription() { Description = "" });
            descriptions.Add("EXP_IND", new AttributeDescription() { Description = "" });
            descriptions.Add("F_CLASS", new AttributeDescription() { Description = "" });
            descriptions.Add("F_CLASS_NAME", new AttributeDescription() { Description = "" });
            descriptions.Add("FAMILY", new AttributeDescription() { Description = "" });
            descriptions.Add("FED AID NAME", new AttributeDescription() { Description = "" });
            descriptions.Add("FED_AID", new AttributeDescription() { Description = "Federal Aid?" });
            descriptions.Add("FEDAID", new AttributeDescription() { Description = "Federal Aid" });
            descriptions.Add("FROMSEGMENT", new AttributeDescription() { Description = "From Section" });
            descriptions.Add("ID", new AttributeDescription() { Description = "ID" });
            descriptions.Add("INSPECT DATE", new AttributeDescription() { Description = "Inspection Date" });
            descriptions.Add("INSPECTYEAR", new AttributeDescription() { Description = "" });
            descriptions.Add("INTERSTATE", new AttributeDescription() { Description = "" });
            descriptions.Add("IS_HPMS", new AttributeDescription() { Description = "HPMS?" });
            descriptions.Add("L_S_TYPE", new AttributeDescription() { Description = "Left Shoulder Type" });
            descriptions.Add("LANES", new AttributeDescription() { Description = "Lanes" });
            descriptions.Add("LAST_STRUCTURAL_OVERLAY", new AttributeDescription() { Description = "Last Structural Overlay" });
            descriptions.Add("LENGTH", new AttributeDescription() { Description = "Length" });
            descriptions.Add("MPO/RPO", new AttributeDescription() { Description = "MPO/RPO Code" });
            descriptions.Add("MPO_RPO", new AttributeDescription() { Description = "MPO/RPO" });
            descriptions.Add("NHS_IND", new AttributeDescription() { Description = "" });
            descriptions.Add("OPI", new AttributeDescription() { Description = "OPI" });
            descriptions.Add("PAVED_THICKNESS", new AttributeDescription() { Description = "" });
            descriptions.Add("R_S_TYPE", new AttributeDescription() { Description = "Right Shoulder Type" });
            descriptions.Add("RISKSCORE", new AttributeDescription() { Description = "RiskScore" });
            descriptions.Add("ROUGAVE", new AttributeDescription() { Description = "" });
            descriptions.Add("ROUGHNESS", new AttributeDescription() { Description = "Roughness" });
            descriptions.Add("SEG", new AttributeDescription() { Description = "" });
            descriptions.Add("SEGMENT_LENGTH", new AttributeDescription() { Description = "Segment Length" });
            descriptions.Add("SR", new AttributeDescription() { Description = "Route" });
            descriptions.Add("SURDATA", new AttributeDescription() { Description = "" });
            descriptions.Add("SURFACE", new AttributeDescription() { Description = "Surface" });
            descriptions.Add("SURFACE_NAME", new AttributeDescription() { Description = "Surface" });
            descriptions.Add("SURFACEID", new AttributeDescription() { Description = "Surface ID" });
            descriptions.Add("THICKNESS", new AttributeDescription() { Description = "" });
            descriptions.Add("TOSEGMENT", new AttributeDescription() { Description = "To Section" });
            descriptions.Add("TRK_PCNT", new AttributeDescription() { Description = "Truck %" });
            descriptions.Add("TRK_PERCENT", new AttributeDescription() { Description = "Truck %" });
            descriptions.Add("TRUEDATE", new AttributeDescription() { Description = "" });
            descriptions.Add("U_R NAME", new AttributeDescription() { Description = "" });
            descriptions.Add("U_R_CODE", new AttributeDescription() { Description = "Urban/Rural" });
            descriptions.Add("UPLOADDATE", new AttributeDescription() { Description = "" });
            descriptions.Add("WIDTH", new AttributeDescription() { Description = "Width" });
            descriptions.Add("YEAR_LAST_OVERLAY", new AttributeDescription() { Description = "Last Overlay" });
            descriptions.Add("YR_BUILT", new AttributeDescription() { Description = "Year Built" });
            descriptions.Add("YR_LST_RESURFACE", new AttributeDescription() { Description = "" });
            descriptions.Add("YR_LST_STRUCT_OVER", new AttributeDescription() { Description = "" });

            return descriptions;
        }



    }
}
