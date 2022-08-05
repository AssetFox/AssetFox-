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
        public Guid Id { get; set; }

        public Guid SimulationYearDetailId { get; set; }

        public virtual SimulationYearDetailEntity SimulationYearDetail { get; set; }    

        public decimal AvailableFunding { get; set; }
        
        public string BudgetName { get; set; }
    }
}
