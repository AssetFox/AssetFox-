using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.TestHelpers;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationTestSetup
    {
        public static SimulationEntity EntityInDb(UnitOfDataPersistenceWork unitOfWork, Guid networkId)
        {
            var name = RandomStrings.WithPrefix("Simulation");
            var entity = new SimulationEntity
            {
                NetworkId = networkId,
                Name = name,
            };
            unitOfWork.Context.Add(entity);
            unitOfWork.Context.SaveChanges();
            return entity;
        }
    }
}
