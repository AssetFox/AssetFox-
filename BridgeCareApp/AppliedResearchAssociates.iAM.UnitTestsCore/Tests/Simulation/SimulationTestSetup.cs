using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes.CalculatedAttributes;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class SimulationTestSetup { 

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



        public static SimulationDTO CreateSimulation(UnitOfDataPersistenceWork unitOfWork, Guid? id = null, string name = null, Guid? owner = null, Guid? networkId = null)
        {
            var resolveNetworkId = networkId ?? NetworkTestSetup.NetworkId;
            CalculatedAttributeTestSetup.CreateCalculatedAttributeLibrary(unitOfWork);
            var dto = SimulationDtos.Dto(id, name, owner);
            unitOfWork.SimulationRepo.CreateSimulation(resolveNetworkId, dto);
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
