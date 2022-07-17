using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;

namespace AppliedResearchAssociates.iAM.DataMinerUnitTests
{
    public static class DatabaseResetter
    {
        public static void ResetDatabase(IUnitOfWork unitOfWork)
        {
            unitOfWork.Context.Database.EnsureCreated();
            unitOfWork.Context.Database.EnsureDeleted();
        }
    }
}
