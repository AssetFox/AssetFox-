using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM
{
    public static class SimulationLogMessages
    {
        private static string SectionString(Section section)
            => $"(section {section.Name} {section.Id})";

        public static string SectionCalculationReturned(Section section, PerformanceCurve performanceCurve, string key, double value)
        {
            var valueString = DoubleWarningStrings.InfinityOrNanWarning(value);
            return $"Calculation for {key} on {SectionString(section)} using performance curve ({performanceCurve.Name} {performanceCurve.Id}) returned {valueString}";
        }

        public static string SpatialWeightCalculationReturned(Section section, double value)
        {
            var valueString = DoubleWarningStrings.InfinityOrNanWarning(value);
            return $"Spatial weight for {SectionString(section)} returned {valueString}";
        }

        internal static string CalculatedFieldReturned(Section section, string fieldName, double value)
        {
            var valueString = DoubleWarningStrings.InfinityOrNanWarning(value);
            return $"Calculated field {fieldName} on {SectionString(section)} returned {valueString}";
        }

        internal static string ConditionalTreatmentConsequenceReturned(Section section, ConditionalTreatmentConsequence consequence, double value)
        {
            var valueString = DoubleWarningStrings.InfinityOrNanWarning(value);
            return $"Conditional treatment consequence {consequence.Attribute.Name} on {SectionString(section)} returned {valueString}";
        }

        internal static string TreatementCostReturned(Section section, TreatmentCost cost, SelectableTreatment selectableTreatment, double value)
        {
            var valueString = DoubleWarningStrings.InfinityOrNanWarning(value);
            return $"Cost {cost.Equation.Expression} for treatment {selectableTreatment.Name } on {SectionString(section)} returned {valueString}";
        }
    }
}
