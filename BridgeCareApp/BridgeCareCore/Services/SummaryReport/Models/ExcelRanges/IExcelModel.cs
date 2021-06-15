using BridgeCareCore.Services.SummaryReport.Visitors;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    public interface IExcelModel
    {
        T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper);
    }
}
