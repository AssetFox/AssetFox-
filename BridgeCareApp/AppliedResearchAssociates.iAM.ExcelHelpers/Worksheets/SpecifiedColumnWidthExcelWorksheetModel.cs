using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class SpecifiedColumnWidthExcelWorksheetModel: IExcelWorksheetContentModel
    {
        /// <summary>one-based</summary>
        public int ColumnNumber { get; set; }
        public double Width { get; set; }

        public TOutput Accept<TOutput, THelper>(IExcelWorksheetModelVisitor<THelper, TOutput> visitor, THelper helper)
            => visitor.Visit(this, helper);
    }
}
