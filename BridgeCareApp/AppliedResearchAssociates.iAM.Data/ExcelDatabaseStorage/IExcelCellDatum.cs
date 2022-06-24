using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.Data.ExcelDatabaseStorage
{
    /// <summary><para>Do NOT use this if you want to write to a spreadsheet.</para>
    /// This is different from the excel-related interfaces
    /// in the ExcelHelpers project. In that project, the intent is to store
    /// information that could be written to a spreadsheet. Such information might
    /// include font weight, font size, borders, or cell content. By contrast,
    /// here we store cell content only.</summary>
    public interface IExcelCellDatum
    {
        T Accept<T, THelper>(IExcelCellDatumVisitor<THelper, T> visitor, THelper helper);
    }
}
