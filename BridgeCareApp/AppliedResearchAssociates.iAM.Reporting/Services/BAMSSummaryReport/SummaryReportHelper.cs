using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis.Engine;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport
{
    public class SummaryReportHelper : ISummaryReportHelper
    {
        public T checkAndGetValue<T>(IDictionary itemsArray, string itemName)
        {
            var itemValue = default(T);

            if (itemsArray == null) { return itemValue; }
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName)) { return itemValue; }

            if (itemsArray.Contains(itemName)) { itemValue = (T)itemsArray[itemName]; }

            //return value
            return itemValue;
        }

        // Identifying 185 Bridges
        public bool BridgeFunding185(AssetDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];

            return
                fedAid == "1" ||
                fedAid == "2" ||
                fedAid == "0" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 8 && bridgeLength < 20 && ownerCode.StartsWith("01");
        }

        // Identifying 581 Bridges
        public bool BridgeFunding581(AssetDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];

            return
                fedAid == "1" ||
                fedAid == "2" ||
                fedAid == "0" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 8 && bridgeLength < 20 && ownerCode.StartsWith("01");
        }

        // Identifing STP Bridges
        public bool BridgeFundingSTP(AssetDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "1" && bridgeLength >= 20 ||
                fedAid == "2" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 20;
        }

        // Identifying NHPP Bridges
        public bool BridgeFundingNHPP(AssetDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }

            return
                (fedAid == "1" || fedAid == "2") &&
                (functionalClass == "01" || functionalClass == "02" || functionalClass == "11" || functionalClass == "12" || functionalClass == "14");
        }

        // Identifying BOF Bridges
        public bool BridgeFundingBOF(AssetDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var busPlanNetwork = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "0" && bridgeLength >= 20 &&
                (functionalClass == "08" || functionalClass == "09" || functionalClass == "19" || busPlanNetwork == "L");
        }

        // Identifying 183 Bridges
        public bool BridgeFunding183(AssetDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var busPlanNetwork = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "0" && bridgeLength >= 8 &&
                (functionalClass == "09" || functionalClass == "19" || busPlanNetwork == "L");
        }


        public bool BridgeFundingBRIP(AssetDetail section)
        {
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var functionalClassArray = new string[] { "01", "02" };
            return (bridgeLength >= 20 && functionalClassArray.Contains(functionalClass));
        }

        public bool BridgeFundingState(AssetDetail section)
        {
            var internetReport = section.ValuePerTextAttribute["INTERNET_REPORT"];
            return (internetReport.ToUpper() == "STATE");
        }

        public bool BridgeFundingNA(AssetDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var functionalClassArray = new string[] { "01", "02", "03", "06", "07", "11", "12", "14", "16", "17", "NN" };
            return fedAid == "0" && (functionalClassArray.Contains(functionalClass));
        }


        public bool BridgeFunding183(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var busPlanNetwork = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "0" && bridgeLength >= 8 &&
                (functionalClass == "09" || functionalClass == "19" || busPlanNetwork == "L");
        }

        public bool BridgeFunding185(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];

            return
                fedAid == "1" ||
                fedAid == "2" ||
                fedAid == "0" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 8 && bridgeLength < 20 && ownerCode.StartsWith("01");
        }

        public bool BridgeFunding581(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];

            return
                fedAid == "1" ||
                fedAid == "2" ||
                fedAid == "0" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 8 && bridgeLength < 20 && ownerCode.StartsWith("01");
        }

        public bool BridgeFundingBOF(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var busPlanNetwork = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "0" && bridgeLength >= 20 &&
                (functionalClass == "08" || functionalClass == "09" || functionalClass == "19" || busPlanNetwork == "L");
        }

        public bool BridgeFundingNHPP(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }

            return
                (fedAid == "1" || fedAid == "2") &&
                (functionalClass == "01" || functionalClass == "02" || functionalClass == "11" || functionalClass == "12" || functionalClass == "14");
        }

        public bool BridgeFundingSTP(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "1" && bridgeLength >= 20 ||
                fedAid == "2" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 20;
        }


        public bool BridgeFundingBRIP(AssetSummaryDetail section)
        {
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2) {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var functionalClassArray = new string[] { "01", "02" };
            return (bridgeLength >= 20 && functionalClassArray.Contains(functionalClass));
        }

        public bool BridgeFundingState(AssetSummaryDetail section)
        {
            var internetReport = section.ValuePerTextAttribute["INTERNET_REPORT"];
            return (internetReport.ToUpper() == "STATE");
        }

        public bool BridgeFundingNA(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var functionalClassArray = new string[] { "01", "02", "03", "06", "07", "11", "12", "14", "16", "17", "NN" };
            return fedAid == "0" && (functionalClassArray.Contains(functionalClass));
        }



        private static readonly Dictionary<string, string> FunctionalClassDescriptions =
            new Dictionary<string, string>()
            {
                { "01", "01 - Rural - Principal Arterial - Interstate" },
                { "02", "02 - Rural - Principal Arterial - Other" },
                { "03", "03 - Rural - Other Freeway/Expressway" },
                { "06", "06 - Rural - Minor Arterial" },
                { "07", "07 - Rural - Major Collector" },
                { "08", "08 - Rural - Minor Collector" },
                { "09", "09 - Rural - Local" },
                { "NN", "NN - Other" },
                { "11", "11 - Urban - Principal Arterial - Interstate" },
                { "12", "12 - Urban - Principal Arterial - Other Freeway & Expressways" },
                { "14", "14 - Urban - Other Principal Arterial" },
                { "16", "16 - Urban - Minor Arterial" },
                { "17", "17 - Urban - Collector" },
                { "19", "19 - Urban - Local" },
                { "99", "99 - Urban - Ramp" }
            };

        public string FullFunctionalClassDescription(string functionalClassAbbreviation)
        {
            return FunctionalClassDescriptions.ContainsKey(functionalClassAbbreviation) ? FunctionalClassDescriptions[functionalClassAbbreviation] : FunctionalClassDescriptions["NN"];
        }
    }
}
