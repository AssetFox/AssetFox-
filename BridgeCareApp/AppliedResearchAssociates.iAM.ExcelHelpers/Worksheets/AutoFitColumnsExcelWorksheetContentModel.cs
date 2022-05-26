using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class AutoFitColumnsExcelWorksheetContentModel: IExcelWorksheetContentModel
    {
        public double MinWidth { get; set; }


        public TOutput Accept<TOutput, THelper>(IExcelWorksheetModelVisitor<THelper, TOutput> visitor, THelper helper)
            => visitor.Visit(this, helper);
    }
}
