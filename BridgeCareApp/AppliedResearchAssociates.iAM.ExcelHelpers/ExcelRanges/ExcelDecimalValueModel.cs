using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelDecimalValueModel: IExcelModel
    {
        public decimal Value { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
