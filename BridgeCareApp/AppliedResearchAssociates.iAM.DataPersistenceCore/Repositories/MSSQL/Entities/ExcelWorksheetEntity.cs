using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    internal class ExcelWorksheetEntity
    {
        public Guid Id { get; set; }
        public string SerializedWorksheetContent { get; set; }
    }
}
