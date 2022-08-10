using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataUnitTests;
using AppliedResearchAssociates.iAM.DataUnitTests.Tests;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.TestHelpers;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.Attributes;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;
using BridgeCareCore.Models;
using Xunit;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public class SimulationRunTests
    {
        private TestHelper _testHelper => TestHelper.Instance;

        [Fact]
        public void RunSimulation()
        {
            var config = _testHelper.Config;
            var connectionString = TestConnectionStrings.BridgeCare(config);
            var dataSourceDto = DataSourceTestSetup.DtoForSqlDataSourceInDb(_testHelper.UnitOfWork, connectionString);
            var districtAttributeDomain = AttributeConnectionAttributes.String(connectionString, dataSourceDto.Id);
            var districtAttribute = AttributeMapper.ToDto(districtAttributeDomain, dataSourceDto);
            UnitTestsCoreAttributeTestSetup.EnsureAttributeExists(districtAttribute);

            var networkName = RandomStrings.WithPrefix("Network");
            var allDataSourceDto = AllDataSourceDtoFakeFrontEndFactory.ToAll(dataSourceDto);

            var networkDefinitionAttribute = AllAttributeDtos.BrKey(allDataSourceDto);
            var parameters = new NetworkCreationParameters
            {
                DefaultEquation = "[Deck_Area]",
                NetworkDefinitionAttribute = networkDefinitionAttribute
            };
            var network = NetworkTestSetup.ModelForEntityInDbViaFactory(
                _testHelper.UnitOfWork, districtAttributeDomain, parameters, networkName);

            var simulationId = Guid.NewGuid();
            var simulationName = RandomStrings.WithPrefix("Simulation");
            var simulationDto = new SimulationDTO
            {
                Id = simulationId,
                NetworkId = network.Id,
                Name = simulationName,
            };
            _testHelper.UnitOfWork.SimulationRepo.CreateSimulation(network.Id, simulationDto);
            var explorer = _testHelper.UnitOfWork.AttributeRepo.GetExplorer();
            var analysisNetwork = _testHelper.UnitOfWork.NetworkRepo.GetSimulationAnalysisNetwork(
                network.Id, explorer);
            _testHelper.UnitOfWork.SimulationRepo.GetSimulationInNetwork(simulationId, analysisNetwork);
        }
    }
}
