using System;
using AppliedResearchAssociates.iAM.Data.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using BridgeCareCoreTests.Helpers;
using Xunit;

namespace BridgeCareCoreTests.Tests.Integration
{
    public class SimulationRunTests
    {
        //[Fact]
        [Fact (Skip ="Fails as we apparently need a CalculatedAttribute library. Setup is likely nowhere near rich enough.")]
        public void RunSimulation()
        {
            var config = TestConfiguration.Get();
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var dataSourceDto = DataSourceTestSetup.DtoForSqlDataSourceInDb(TestHelper.UnitOfWork, connectionString);
            var districtAttributeDomain = AttributeConnectionAttributes.String(connectionString, dataSourceDto.Id);
            var districtAttribute = AttributeDtoDomainMapper.ToDto(districtAttributeDomain, dataSourceDto);
            UnitTestsCoreAttributeTestSetup.EnsureAttributeExists(districtAttribute);

            var networkName = RandomStrings.WithPrefix("Network");
            var allDataSourceDto = AllDataSourceDtoFakeFrontEndFactory.ToAll(dataSourceDto);

            var networkDefinitionAttribute = AllAttributeDtos.BrKey(allDataSourceDto);
            var parameters = new NetworkCreationParameters
            {
                DefaultEquation = "[Deck_Area]",
                NetworkDefinitionAttribute = networkDefinitionAttribute
            };
            var network = NetworkIntegrationTestSetup.ModelForEntityInDbViaFactory(
                TestHelper.UnitOfWork, districtAttributeDomain, parameters, networkName);

            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var simulationDto = new SimulationDTO
            {
                Id = simulationId,
                NetworkId = network.Id,
                Name = simulationName,
            };
            TestHelper.UnitOfWork.SimulationRepo.CreateSimulation(network.Id, simulationDto);
            var explorer = TestHelper.UnitOfWork.AttributeRepo.GetExplorer();
            var analysisNetwork = TestHelper.UnitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(
                network.Id, explorer);
            TestHelper.UnitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, analysisNetwork);
        }
    }
}
