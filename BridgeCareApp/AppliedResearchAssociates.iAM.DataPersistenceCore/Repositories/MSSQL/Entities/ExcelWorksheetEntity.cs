using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class ExcelWorksheetEntity: BaseEntity
    {
        public Guid Id { get; set; }
        public string SerializedWorksheetContent { get; set; }
        public Guid DataSourceId { get; set; }
        public virtual DataSourceEntity DataSource { get; set; }
    }
}
