namespace AppliedResearchAssociates.iAM.Analysis.Input.DataTransfer
{
    public sealed class PerformanceCurve
    {
        public string AttributeID { get; set; }

        public string CriterionExpression { get; set; }

        public string EquationExpression { get; set; }

        public bool Shift { get; set; }
    }
}
