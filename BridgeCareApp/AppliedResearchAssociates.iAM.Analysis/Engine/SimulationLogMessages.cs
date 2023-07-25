namespace AppliedResearchAssociates.iAM.Analysis.Engine;

public static class SimulationLogMessages
{
    public static string UserFriendlyString(double x)
        => double.IsNaN(x) ? "'not a number'" :
           double.IsPositiveInfinity(x) ? "infinity" :
           double.IsNegativeInfinity(x) ? "negative infinity" :
           x.ToString();

    private static string AssetString(AnalysisMaintainableAsset asset)
        => $"(asset {asset.AssetName} {asset.Id})";

    public static string AssetCalculationReturned(AnalysisMaintainableAsset asset, PerformanceCurve performanceCurve, string key, double value)
    {
        var valueString = UserFriendlyString(value);
        return $"Calculation for {key} on with equation {performanceCurve.Equation?.Expression} on {AssetString(asset)} using performance curve ({performanceCurve.Name} {performanceCurve.Id}) returned {valueString}";
    }

    public static string SpatialWeightCalculationReturned(AnalysisMaintainableAsset asset, Equation equation, double value)
    {
        var valueString = UserFriendlyString(value);
        return $"Spatial weight {equation?.Expression} for {AssetString(asset)} returned {valueString}";
    }

    internal static string CalculatedFieldReturned(AnalysisMaintainableAsset asset, Equation equation, string fieldName, double value)
    {
        var valueString = UserFriendlyString(value);
        return $"Calculated field {fieldName} with equation {equation?.Expression} on {AssetString(asset)} returned {valueString}";
    }

    internal static string TreatmentConsequenceReturned(AnalysisMaintainableAsset asset, Treatment treatment, Equation equation, ConditionalTreatmentConsequence consequence, double value)
    {
        var valueString = UserFriendlyString(value);
        return $"Consequence {consequence.Attribute.Name} for treatment {treatment?.Name} with equation {equation?.Expression} on {AssetString(asset)} returned {valueString}";
    }

    internal static string TreatmentCostReturned(AnalysisMaintainableAsset asset, TreatmentCost cost, SelectableTreatment selectableTreatment, double value)
    {
        var valueString = UserFriendlyString(value);
        return $"Cost {cost?.Equation?.Expression} for treatment {selectableTreatment.Name } on {AssetString(asset)} returned {valueString}";
    }
}
