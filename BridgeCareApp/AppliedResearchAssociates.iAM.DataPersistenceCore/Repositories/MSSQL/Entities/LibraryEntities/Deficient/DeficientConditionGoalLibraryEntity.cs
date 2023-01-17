using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient
{
    public class DeficientConditionGoalLibraryEntity : LibraryEntity
    {
        public DeficientConditionGoalLibraryEntity()
        {
            DeficientConditionGoals = new HashSet<DeficientConditionGoalEntity>();
            //DeficientConditionGoalLibrarySimulationJoins = new HashSet<DeficientConditionGoalLibrarySimulationEntity>();
            Users = new HashSet<DeficientConditionGoalLibraryUserEntity>();
        }

        public virtual ICollection<DeficientConditionGoalEntity> DeficientConditionGoals { get; set; }
        public virtual ICollection<DeficientConditionGoalLibraryUserEntity> Users { get; set; }
        //public virtual ICollection<DeficientConditionGoalLibrarySimulationEntity> DeficientConditionGoalLibrarySimulationJoins { get; set; }
    }
}
