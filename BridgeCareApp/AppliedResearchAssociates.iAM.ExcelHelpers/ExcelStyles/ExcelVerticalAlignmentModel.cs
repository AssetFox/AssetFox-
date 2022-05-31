using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/ExcelStyles/ExcelVerticalAlignmentModel.cs
using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;
using OfficeOpenXml.Style;

namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
using BridgeCareCore.Helpers.Excel.Visitors;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelStyles/ExcelVerticalAlignmentModel.cs
{
    public class ExcelVerticalAlignmentModel : IExcelModel
    {
        public ExcelVerticalAlignment Alignment { get; set; }

        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
