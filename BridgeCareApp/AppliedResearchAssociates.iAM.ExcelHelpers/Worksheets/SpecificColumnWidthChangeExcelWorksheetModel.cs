using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class SpecificColumnWidthChangeExcelWorksheetModel: IExcelWorksheetContentModel
    {
        /// <summary>one-based</summary>
        public int ColumnNumber { get; set; }
        public Func<double, double> WidthChange { get; set; }

        public TOutput Accept<TOutput, THelper>(IExcelWorksheetModelVisitor<THelper, TOutput> visitor, THelper helper)
            => visitor.Visit(this, helper);
    }
}
