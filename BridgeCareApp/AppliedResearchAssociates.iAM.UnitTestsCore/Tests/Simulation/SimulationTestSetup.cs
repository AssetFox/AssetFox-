using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationTestSetup
    {
        public static SimulationDTO TestSimulation(Guid? id = null, string name = null, Guid? owner = null)
        {
            var resolveName = name ?? RandomStrings.Length11();
            var resolveId = id ?? Guid.NewGuid();
            var users = new List<SimulationUserDTO>();
            if (owner != null)
            {
                var newUser = new SimulationUserDTO
                {
                    IsOwner = true,
                    UserId = owner.Value,
                };
                users.Add(newUser);
            }
            var returnValue = new SimulationDTO
            {
                Id = resolveId,
                NetworkId = NetworkTestSetup.NetworkId,
                Name = resolveName,
                Users = users,
            };
            return returnValue;
        }

        public static SimulationDTO CreateSimulation(UnitOfDataPersistenceWork unitOfWork, Guid? id = null, string name = null, Guid? owner = null, Guid? networkId = null)
        {
            var resolveNetworkId = networkId ?? NetworkTestSetup.NetworkId;
            CalculatedAttributeTestSetup.CreateCalculatedAttributeLibrary(unitOfWork);
            var dto = TestSimulation(id, name, owner);
            unitOfWork.SimulationRepo.CreateSimulation(resolveNetworkId, dto);
            return dto;
        }
    }
}
