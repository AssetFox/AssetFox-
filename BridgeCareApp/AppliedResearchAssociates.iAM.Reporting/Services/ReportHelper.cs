using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;

namespace AppliedResearchAssociates.iAM.Reporting.Services
{
    public class ReportHelper
    {
        public T CheckAndGetValue<T>(IDictionary itemsArray, string itemName)
        {
            var itemValue = default(T);

            if (itemsArray == null) { return itemValue; }
            if (string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName)) { return itemValue; }

            if (itemsArray.Contains(itemName)) { itemValue = (T)itemsArray[itemName]; }

            //return value
            return itemValue;
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

        public bool BridgeFundingBOF(AssetSummaryDetail section)
        {
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var NBISlen = section.ValuePerTextAttribute["NBISLEN"];

            return
                NBISlen is "Y" &&
                functionalClass is "08" or "09" or "18" or "19";
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
                (fedAid is "1" && functionalClass is "01" or "02" or "03" or "06" or "07" or "11" or "12" or "14" or "16" or "17") ||
                (fedAid is "0" && functionalClass is "99");
        }

        public bool BridgeFundingSTP(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);

            return fedAid is "1" or "2";
        }

        public bool BridgeFundingBRIP(AssetSummaryDetail section)
        {
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }
            var NBISlen = section.ValuePerTextAttribute["NBISLEN"];

            return
                NBISlen is "Y" &&
                functionalClass is "01" or "02";
        }

        public bool BridgeFundingState(AssetSummaryDetail section)
        {
            var internetReport = section.ValuePerTextAttribute["INTERNET_REPORT"];

            return internetReport is "State" or "Local";
        }

        public bool BridgeFundingNotApplicable(AssetSummaryDetail section)
        {
            if (string.IsNullOrEmpty(section.ValuePerTextAttribute["FEDAID"])) return false;
            var fedAid = section.ValuePerTextAttribute["FEDAID"].Substring(0, 1);
            var functionalClass = "";
            if (section.ValuePerTextAttribute["FUNC_CLASS"].Length >= 2)
            {
                functionalClass = section.ValuePerTextAttribute["FUNC_CLASS"].Substring(0, 2);
            }

            return
                (fedAid is "0" && functionalClass is "01" or "02" or "03" or "06" or "07" or "11" or "12" or "14" or "16" or "17") ||
                functionalClass is "NN";
        }

        public HashSet<string> GetPerformanceCurvesAttributes(Simulation simulation)
        {
            var currentAttributes = new HashSet<string>();
            // Distinct performance curve attributes
            foreach (var performanceCurve in simulation.PerformanceCurves)
            {
                currentAttributes.Add(performanceCurve.Attribute.Name);
            }
            return currentAttributes;
        }

        public string GetBenefitAttribute(Simulation simulation) => simulation.AnalysisMethod.Benefit.Attribute.Name;

        public HashSet<string> GetBudgets(List<SimulationYearDetail> years)
        {
            var budgets = new HashSet<string>();
            foreach (var item in years.FirstOrDefault()?.Budgets)
            {
                budgets.Add(item.BudgetName);
            }
            return budgets;
        }

        public List<AssetDetail> GetSectionsWithUnfundedTreatments(SimulationYearDetail simulationYearDetail)
        {
            var untreatedSections =
                    simulationYearDetail.Assets
                        .Where(section => section.TreatmentCause == TreatmentCause.NoSelection && section.TreatmentOptions.Count > 0)
                        .ToList();
            return untreatedSections;
        }

        public List<AssetDetail> GetSectionsWithFundedTreatments(SimulationYearDetail simulationYearDetail)
        {
            var treatedSections = simulationYearDetail.Assets.Where(section => section.TreatmentCause is not TreatmentCause.NoSelection);
            return treatedSections.ToList();
        }
    }
}
