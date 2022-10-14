using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
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

        public static SimulationDTO CreateSimulation(UnitOfDataPersistenceWork unitOfWork, Guid? id = null, string name = null, Guid? owner = null)
        {
            CalculatedAttributeTestSetup.CreateCalculatedAttributeLibrary(unitOfWork);
            var dto = TestSimulation(id, name, owner);
            unitOfWork.SimulationRepo.CreateSimulation(NetworkTestSetup.NetworkId, dto);;
            return dto;
        }

        public static Simulation DomainSimulation(UnitOfDataPersistenceWork unitOfWork)
        {
            var simulationEntity = EntityInDb(unitOfWork, NetworkTestSetup.NetworkId);
            var simulationDto = unitOfWork.SimulationRepo.GetSimulation(simulationEntity.Id);
            var networkId = NetworkTestSetup.NetworkId;
            var explorer = unitOfWork.AttributeRepo.GetExplorer();
            var network = unitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(networkId, explorer);
            var date = new DateTime(2022, 10, 6);
            SimulationMapper.CreateSimulation(simulationEntity, network, date, date);
            var simulationObject = network.Simulations.Single(s => s.Id == simulationDto.Id);
            return simulationObject;
        }
    }
}
