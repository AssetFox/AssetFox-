using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

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

        public static SimulationEntity TestSimulation(Guid? id = null, string name = null, Guid? owner = null)
        {
            var resolveName = name ?? RandomStrings.Length11();
            var resolveId = id ?? Guid.NewGuid();
            var users = new List<SimulationUserEntity>();
            var returnValue = new SimulationEntity
            {
                Id = resolveId,
                NetworkId = NetworkTestSetup.NetworkId,
                Name = resolveName,
                NumberOfYearsOfTreatmentOutlook = 2,
                SimulationUserJoins = users
            };
            if (owner != null)
                users.Add(new SimulationUserEntity() { IsOwner = true, UserId = owner.Value, SimulationId = resolveId });
            return returnValue;
        }


        public static SimulationEntity CreateSimulation(UnitOfDataPersistenceWork unitOfWork, Guid? id = null, string name = null, Guid? owner = null)
        {
            var entity = TestSimulation(id, name, owner);
            unitOfWork.Context.AddEntity(entity);
            return entity;
        }

    }
}
