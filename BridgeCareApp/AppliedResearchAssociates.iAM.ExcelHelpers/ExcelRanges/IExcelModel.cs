using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    /// <summary>A class that implements this interface represents a change
    /// to be made to a cell or group of cells in an Excel worksheet. The change
    /// could be content or style. Implementing classes do not "know" which cell or
    /// range is to be impacted; they merely model the change itself.</summary>
    public interface IExcelModel
    {
        T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper);
    }
}
