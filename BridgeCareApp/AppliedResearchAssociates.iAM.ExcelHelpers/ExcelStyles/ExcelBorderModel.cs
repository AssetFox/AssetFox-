<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/ExcelStyles/ExcelBorderModel.cs
========
﻿using BridgeCareCore.Helpers.Excel.Visitors;
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelStyles/ExcelBorderModel.cs
using OfficeOpenXml.Style;
﻿using AppliedResearchAssociates.iAM.ExcelHelpers.Visitors;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/ExcelStyles/ExcelBorderModel.cs
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelStyles/ExcelBorderModel.cs
{
    public class ExcelBorderModel: IExcelModel
    {
        public ExcelBorderStyle BorderStyle { get; set; }
        public T Accept<THelper, T>(IExcelModelVisitor<THelper, T> visitor, THelper helper) =>
            visitor.Visit(this, helper);
    }
}
