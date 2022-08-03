using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class BudgetDetailEntity: BaseEntity
    {
        // WjJake -- In the Json output, this is connected to the budget by Name. Wondering if that's still what we want?
        public Guid SimulationYearDetailId { get; set; }
        public virtual SimulationYearDetailEntity SimulationYearDetail { get; set; }    

        public decimal AvailableFunding { get; }
        
        public string BudgetName { get; }
    }
}
