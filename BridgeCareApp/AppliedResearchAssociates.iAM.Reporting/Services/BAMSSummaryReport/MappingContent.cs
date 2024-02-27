using System;
using System.Collections.Generic;
using System.ComponentModel;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport
{
    public enum SpanType
    {
        [Description("Multi")]
        M,
        [Description("Single")]
        S
    }
    public static class MappingContent
    {
        public static Dictionary<SpanType, string> SpanTypeMap = new Dictionary<SpanType, string>(){
            {SpanType.M, "Multi" },
            {SpanType.S, "Single" }
        };

        public static Func<string, string> OwnerCodeForReport = GetOwnerCodeValue;
        private static string GetOwnerCodeValue(string nameFromSimObject)
        {
            switch (nameFromSimObject)
            {
            case "01":
                return "01 - State Highway Agency";
            case "02":
                return "02 - County Highway Agency";
            case "03":
                return "03 - Town or Township Highway Agency";
            case "04":
                return "04 - City, Municipal Highway Agency or Borough";
            case "11":
                return "11 - State Park, Forest or Reservation Agency";
            case "12":
                return "12 - Local Park, Forest or Reservation Agency";
            case "21":
                return "21 - Other State Agency";
            case "25":
                return "25 - Other local Agency";
            case "26":
                return "26 - Private (Other than Railroad)";
            case "27":
                return "27 - Railroad";
            case "31":
                return "31 - State Toll Authority";
            case "32":
                return "32 - Local Toll Authority";
            case "60":
                return "60 - Other Federal Agencies";
            case "62":
                return "62 - Bureau of Indian Affairs";
            case "64":
                return "64 - U.S. Forest Service";
            case "66":
                return "66 - National Park Service";
            case "68":
                return "68 - Bureau of Land Management";
            case "69":
                return "69 - Bureau of Reclamation";
            case "70":
                return "70 - Military Reservation Corps Engineers";
            case "80":
                return "80 - Unknown";
            case "XX":
                return "XX - Demolished/Replaced";
            default: return nameFromSimObject;
            }
        }

        public static string GetDeckSurfaceType(string name)
        {
            switch (name)
            {
            case "None":
                return "00 - None (e.g., steel grid)";
            case "Concrete":
                return "01 - Concrete";
            case "Concrete Overlay":
                return "02 - Concrete Overlay*";
            case "Latex Concrete":
                return "03 - Latex Concrete";
            case "P":
                return "04 - Low Slump Concrete P PPC Overlay";
            case "Epoxy Overlay":
                return "05 - Epoxy Overlay";
            case "Bituminous":
                return "06 - Bituminous (applies only to structures with no deck)";
            case "Wood or Timber":
                return "07 - Wood or Timber";
            case "Not applicable":
                return "N - Not applicable";
            case "Gravel":
                return "08 - Gravel";
            case "Other":
                return "09 - Other";
            default: return name;
            }
        }

        public static (string previousPick, string currentPick) GetCashFlowProjectPick(TreatmentCause treatmentCause, AssetDetail prevYearSection)
        {
            if(prevYearSection.TreatmentCause == treatmentCause)
            {
                return ("BAMS Pick CF", "BAMS Pick CFE"); // middle and last year
            }
            else
            {
                return ("BAMS Pick CFB", "BAMS Pick CFE"); // first and last years
            }
        }

        public static string GetNonCashFlowProjectPick(TreatmentCause treatmentCause)
        {
            switch (treatmentCause)
            {
            case TreatmentCause.NoSelection:
            case TreatmentCause.ScheduledTreatment:
            case TreatmentCause.SelectedTreatment:
                return "BAMS Pick";
            case TreatmentCause.CommittedProject:                
                return "Committed";
            default:
                return treatmentCause.ToString();
            }
        }
    }

    public static class BridgeDataExtensions
    {
        public static string ToSpanTypeName(this SpanType spanType)
        {
            switch (spanType)
            {
            case SpanType.M: return "M";
            case SpanType.S: return "S";
            default: return "";
            }
        }
    }
}
