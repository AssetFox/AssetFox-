using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DTOs
{
    public class ExcelRawDataDTO
    {
        public Guid Id { get; set; }
        public Guid DataSourceId { get; set; }
        public string SerializedWorksheetContent { get; set; }
    }
}
