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
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_185='Y' where F_185='N' and fedaid like '1 - %'")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_185='Y' where F_185='N' and fedaid like '2 - %'")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_185='Y' where F_185='N' and fedaid like '0 - %' and BridgeLength>=20")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_185='Y' where F_185='N' and fedaid like '0 - %' and BridgeLength>=8 and BridgeLength<20 and ownercode like '01 - %'")
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
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_581='Y' where F_581='N' and fedaid like '1 - %'")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_581='Y' where F_581='N' and fedaid like '2 - %'")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_581='Y' where F_581='N' and fedaid like '0 - %' and BridgeLength>=20")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_581='Y' where F_581='N' and fedaid like '0 - %' and BridgeLength>=8 and BridgeLength<20 and ownercode like '01 - %'")
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
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_STP='Y' where F_STP='N' and fedaid like '1 - %' and BridgeLength>=20")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_STP='Y' where F_STP='N' and fedaid like '2 - %' and BridgeLength>=20")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_STP='Y' where F_STP='N' and fedaid like '0 - %'  and BridgeLength>=20")
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

        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '1 - %' and funcclass like '01 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '1 - %' and funcclass like '02 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '1 - %' and funcclass like '11 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '1 - %' and funcclass like '12 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '1 - %' and funcclass like '14 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '2 - %' and funcclass like '01 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '2 - %' and funcclass like '02 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '2 - %' and funcclass like '11 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '2 - %' and funcclass like '12 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_NHPP='Y' where F_NHPP='N' and fedaid like '2 - %' and funcclass like '14 - %' ")
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
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_BOF='Y' where F_BOF='N' and fedaid like '0 - %' and BridgeLength>=20 and funcclass like '08 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_BOF='Y' where F_BOF='N' and fedaid like '0 - %' and BridgeLength>=20 and funcclass like '09 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_BOF='Y' where F_BOF='N' and fedaid like '0 - %' and BridgeLength>=20 and funcclass like '19 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_BOF='Y' where F_BOF='N' and fedaid like '0 - %' and BridgeLength>=20 and BPN='L'")
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
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_183='Y' where F_183='N' and fedaid like '0 - %' and BridgeLength>=8 and funcclass like '09 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_183='Y' where F_183='N' and fedaid like '0 - %' and BridgeLength>=8 and funcclass like '19 - %' ")
        //con.Execute("update BAMs_Lite_plus_Data_Base set F_183='Y' where F_183='N' and fedaid like '0 - %' and BridgeLength>=8 and BPN='L' ")
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
