using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.DataUnitTests
{
    public static class DatabaseResetter
    {
        public static void ResetDatabase(UnitOfDataPersistenceWork unitOfWork)
        {
            unitOfWork.Context.Database.EnsureDeleted();
            unitOfWork.Context.Database.EnsureCreated();
        }

        public static void EnsureDatabaseExists(UnitOfDataPersistenceWork unitOfWork)
        {
            unitOfWork.Context.Database.EnsureCreated();
        }
    }
}
