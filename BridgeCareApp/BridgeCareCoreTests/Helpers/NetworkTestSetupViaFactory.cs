using AppliedResearchAssociates.iAM.Data.Attributes;
using AppliedResearchAssociates.iAM.Data.Networking;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using BridgeCareCore.Utils;
using DataAttribute = AppliedResearchAssociates.iAM.Data.Attributes.Attribute;
using TNetwork = AppliedResearchAssociates.iAM.Data.Networking.Network;


namespace BridgeCareCoreTests.Helpers
{
    public static class NetworkTestSetupViaFactory
    {



        public static TNetwork ModelForEntityInDbViaFactory(IUnitOfWork unitOfWork, DataAttribute attribute, NetworkCreationParameters parameters, string networkName)
        {
            var network = ModelViaFactory(unitOfWork, attribute, parameters, networkName);

            // insert network domain data into the data source
            unitOfWork.NetworkRepo.CreateNetwork(network);
            return network;
        }

        public static TNetwork ModelViaFactory(IUnitOfWork unitOfWork, DataAttribute attribute, NetworkCreationParameters parameters, string networkName)
        {
            var allDataSource = parameters.NetworkDefinitionAttribute.DataSource;
            var mappedDataSource = AllDataSourceMapper.ToSpecificDto(allDataSource);
            var attributeConnection = AttributeConnectionBuilder.Build(attribute, mappedDataSource, unitOfWork);
            var data = AttributeDataBuilder.GetData(attributeConnection);
            var network = NetworkFactory.CreateNetworkFromAttributeDataRecords(
                  data, parameters.DefaultEquation);
            network.Name = networkName;
            return network;
        }
    }
}
