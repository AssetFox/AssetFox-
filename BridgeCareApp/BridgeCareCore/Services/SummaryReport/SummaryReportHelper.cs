using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Interfaces.SummaryReport;

namespace BridgeCareCore.Services.SummaryReport
{
    public class SummaryReportHelper : ISummaryReportHelper
    {
        // Identifying 185 Bridges
        public bool BridgeFunding185(SectionDetail section)
        {
            // ----------------------------------------------------------
            // TODO: REMOVE WHEN "FEDAID" PARAMETER IS AVAILABLE
            return false;
            // ----------------------------------------------------------


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
        public bool BridgeFunding581(SectionDetail section)
        {
            // ----------------------------------------------------------
            // TODO: REMOVE WHEN "FEDAID" PARAMETER IS AVAILABLE
            return false;
            // ----------------------------------------------------------


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
        public bool BridgeFundingSTP(SectionDetail section)
        {
            // ----------------------------------------------------------
            // TODO: REMOVE WHEN "FEDAID" PARAMETER IS AVAILABLE
            return false;
            // ----------------------------------------------------------


            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "1" && bridgeLength >= 20 ||
                fedAid == "2" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 20;
        }

        // Identifying NHPP Bridges
        public bool BridgeFundingNHPP(SectionDetail section)
        {
            // ----------------------------------------------------------
            // TODO: REMOVE WHEN "FEDAID" PARAMETER IS AVAILABLE
            return false;
            // ----------------------------------------------------------


            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);

            return
                (fedAid == "1" || fedAid == "2") &&
                (functionalClass == "01" || functionalClass == "02" || functionalClass == "11" || functionalClass == "12" || functionalClass == "14");
        }

        // Identifying BOF Bridges
        public bool BridgeFundingBOF(SectionDetail section)
        {
            // ----------------------------------------------------------
            // TODO: REMOVE WHEN "FEDAID" PARAMETER IS AVAILABLE
            return false;
            // ----------------------------------------------------------


            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            var busPlanNetwork = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "0" && bridgeLength >= 20 &&
                (functionalClass == "08" || functionalClass == "09" || functionalClass == "19" || busPlanNetwork == "L");
        }


        // Identifying 183 Bridges
        public bool BridgeFunding183(SectionDetail section)
        {
            // ----------------------------------------------------------
            // TODO: REMOVE WHEN "FEDAID" PARAMETER IS AVAILABLE
            return false;
            // ----------------------------------------------------------


            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            var busPlanNetwork = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "0" && bridgeLength >= 8 &&
                (functionalClass == "09" || functionalClass == "19" || busPlanNetwork == "L");
        }

        public bool BridgeFunding183(SectionSummaryDetail section)
        {
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            var busPlanNetwork = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "0" && bridgeLength >= 8 &&
                (functionalClass == "09" || functionalClass == "19" || busPlanNetwork == "L");
        }
        public bool BridgeFunding185(SectionSummaryDetail section)
        {
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];

            return
                fedAid == "1" ||
                fedAid == "2" ||
                fedAid == "0" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 8 && bridgeLength < 20 && ownerCode.StartsWith("01");
        }
        public bool BridgeFunding581(SectionSummaryDetail section)
        {
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];
            var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];

            return
                fedAid == "1" ||
                fedAid == "2" ||
                fedAid == "0" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 8 && bridgeLength < 20 && ownerCode.StartsWith("01");
        }
        public bool BridgeFundingBOF(SectionSummaryDetail section)
        {
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            var busPlanNetwork = section.ValuePerTextAttribute["BUS_PLAN_NETWORK"];
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "0" && bridgeLength >= 20 &&
                (functionalClass == "08" || functionalClass == "09" || functionalClass == "19" || busPlanNetwork == "L");
        }
        public bool BridgeFundingNHPP(SectionSummaryDetail section)
        {
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);

            return
                (fedAid == "1" || fedAid == "2") &&
                (functionalClass == "01" || functionalClass == "02" || functionalClass == "11" || functionalClass == "12" || functionalClass == "14");
        }
        public bool BridgeFundingSTP(SectionSummaryDetail section)
        {
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var bridgeLength = section.ValuePerNumericAttribute["LENGTH"];

            return
                fedAid == "1" && bridgeLength >= 20 ||
                fedAid == "2" && bridgeLength >= 20 ||
                fedAid == "0" && bridgeLength >= 20;
        }


        private static readonly Dictionary<string, string> FunctionalClassDescriptions =
            new Dictionary<string, string>()
            {
                {"01", "Rural Interstate"},
                {"02", "Rural - Other Principal"},
                {"06", "Rural Minor Arterial"},
                {"07", "Rural Major Collector"},
                {"08", "Rural Minor Collector"},
                {"09", "Rural Local"},
                {"11", "Urban Interstate"},
                {"12", "Urban Other Freeway/Expressway"},
                {"14", "Urban Other Principal"},
                {"16", "Urban Minor Arterial"},
                {"17", "Urban Major Collector"},
                {"19", "Urban Local"},
                {"NN", "Other"},
                {"99", "Ramp" },
            };

        public string FullFunctionalClassDescription(string functionalClassAbbreviation)
        {
            return FunctionalClassDescriptions.ContainsKey(functionalClassAbbreviation) ? FunctionalClassDescriptions[functionalClassAbbreviation] : functionalClassAbbreviation;
        }
    }
}
