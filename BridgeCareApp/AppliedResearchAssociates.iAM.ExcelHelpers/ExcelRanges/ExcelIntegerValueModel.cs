using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;
namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelIntegerValueModel: IExcelModel
    {
        public int Value { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
