namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    public static class SimulationLogMessages
    {
        public static string UserFriendlyString(double x)
            => double.IsNaN(x) ? "'not a number'" :
               double.IsPositiveInfinity(x) ? "infinity" :
               double.IsNegativeInfinity(x) ? "negative infinity" :
               x.ToString();

        private static string SectionString(Section section)
            => $"(section {section.Name} {section.Id})";

        public static string SectionCalculationReturned(Section section, PerformanceCurve performanceCurve, string key, double value)
        {
            var valueString = UserFriendlyString(value);
            return $"Calculation for {key} on with equation {performanceCurve.Equation?.Expression} on {SectionString(section)} using performance curve ({performanceCurve.Name} {performanceCurve.Id}) returned {valueString}";
        }

        public static string SpatialWeightCalculationReturned(Section section, Equation equation, double value)
        {
            var valueString = UserFriendlyString(value);
            return $"Spatial weight {equation?.Expression} for {SectionString(section)} returned {valueString}";
        }

        internal static string CalculatedFieldReturned(Section section, Equation equation, string fieldName, double value)
        {
            var valueString = UserFriendlyString(value);
            return $"Calculated field {fieldName} with equation {equation?.Expression} on {SectionString(section)} returned {valueString}";
        }

        internal static string TreatmentConsequenceReturned(Section section, Treatment treatment, Equation equation, ConditionalTreatmentConsequence consequence, double value)
        {
            var valueString = UserFriendlyString(value);
            return $"Consequence {consequence.Attribute.Name} for treatment {treatment?.Name} with equation {equation?.Expression} on {SectionString(section)} returned {valueString}";
        }

        internal static string TreatmentCostReturned(Section section, TreatmentCost cost, SelectableTreatment selectableTreatment, double value)
        {
            var valueString = UserFriendlyString(value);
            return $"Cost {cost?.Equation?.Expression} for treatment {selectableTreatment.Name } on {SectionString(section)} returned {valueString}";
        }
    }
}
