using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
{
    public class ExcelHorizontalAlignmentModel: IExcelModel
    {
        public ExcelHorizontalAlignment Alignment { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
