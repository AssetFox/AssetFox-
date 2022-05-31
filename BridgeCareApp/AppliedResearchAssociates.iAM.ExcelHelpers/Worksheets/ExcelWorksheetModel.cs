using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/Worksheets/ExcelWorksheetModel.cs
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/Worksheets/ExcelWorksheetModel.cs
{
    public class ExcelWorksheetModel
    {
        public string TabName { get; set; }
        public List<IExcelWorksheetContentModel> Content { get; set; }
    }
}
