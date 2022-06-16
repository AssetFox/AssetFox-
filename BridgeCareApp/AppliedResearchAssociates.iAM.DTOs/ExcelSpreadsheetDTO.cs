using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class ExcelSpreadsheetDTO
    {
        public Guid Id { get; set; }
        public string SerializedWorksheetContent { get; set; }
    }
}
