using BridgeCareCore.Helpers.Excel.Visitors;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Helpers.Excel
{
    public class ExcelSingleBorderModel: IExcelModel
    {
        public ExcelBorderStyle BorderStyle { get; set; }
        public RectangleEdge Edge { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
