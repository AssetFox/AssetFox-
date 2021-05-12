using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : INetworkRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public NetworkRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateNetwork(DataAssignment.Networking.Network network)
        {
            // prevent EF from attempting to create the network's child entities (create them
            // separately as part of a bulk insert)
            var networkEntity = new NetworkEntity
            {
                Id = new Guid(DataPersistenceConstants.PennDotNetworkId),
                Name = network.Name
            };
            _unitOfWork.Context.Upsert(networkEntity, networkEntity.Id, _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddEntity(network.ToEntity(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.MaintainableAssetRepo.CreateMaintainableAssets(network.MaintainableAssets.ToList(), network.Id);
        }

        public void CreateNetwork(Network network) => _unitOfWork.Context.AddEntity(network.ToEntity(), _unitOfWork.UserEntity?.Id);

        public List<DataAssignment.Networking.Network> GetAllNetworks() =>
            _unitOfWork.Context.Network.Select(_ => _.ToDomain()).ToList();

        public Task<List<NetworkDTO>> Networks()
        {
            if (!_unitOfWork.Context.Network.Any())
            {
                return Task.Factory.StartNew(() => new List<NetworkDTO>());
            }

            return Task.Factory.StartNew(() => _unitOfWork.Context.Network
                .Include(_ => _.BenefitQuantifier)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.NetworkRollupDetail)
                .Select(_ => _.ToDto())
                .ToList());
        }

        public NetworkEntity GetPennDotNetwork()
        {
            var penndotNetworkId = new Guid(DataPersistenceConstants.PennDotNetworkId);

            if (!_unitOfWork.Context.Network.Any(_ => _.Id == penndotNetworkId))
            {
                return null;
            }

            return _unitOfWork.Context.Network
                .Single(_ => _.Id == penndotNetworkId);
        }

        public bool CheckPennDotNetworkHasData()
        {
            var penndotNetworkId = new Guid(DataPersistenceConstants.PennDotNetworkId);
            var facilityCount = _unitOfWork.Context.Facility.Count(_ => _.NetworkId == penndotNetworkId);
            var sectionCount = _unitOfWork.Context.Section.Count(_ => _.Facility.NetworkId == penndotNetworkId);
            var numericAttributeValueHistoryCount = _unitOfWork.Context.NumericAttributeValueHistory
                .Count(_ => _.Section.Facility.NetworkId == penndotNetworkId);
            var textAttributeValueHistoryCount = _unitOfWork.Context.TextAttributeValueHistory
                .Count(_ => _.Section.Facility.NetworkId == penndotNetworkId);

            return facilityCount > 0 && sectionCount > 0 && numericAttributeValueHistoryCount > 0 &&
                   textAttributeValueHistoryCount > 0;
        }

        public Domains.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer, bool areFacilitiesRequired = true)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var networkEntity = _unitOfWork.Context.Network.AsNoTracking()
                .Single(_ => _.Id == networkId);

            if (areFacilitiesRequired)
            {
                networkEntity.MaintainableAssets = _unitOfWork.Context.MaintainableAsset
                    .Where(_ => _.NetworkId == networkId)
                    .Select(asset => new MaintainableAssetEntity
                    {
                        Id = asset.Id,
                        SpatialWeighting = asset.SpatialWeighting,
                        MaintainableAssetLocation = new MaintainableAssetLocationEntity
                        {
                            LocationIdentifier = asset.MaintainableAssetLocation.LocationIdentifier
                        },
                        AggregatedResults = asset.AggregatedResults.Select(result => new AggregatedResultEntity
                        {
                            Discriminator = result.Discriminator,
                            Year = result.Year,
                            TextValue = result.TextValue,
                            NumericValue = result.NumericValue,
                            Attribute = new AttributeEntity
                            {
                                Name = result.Attribute.Name
                            }
                        }).ToList()
                    }).AsNoTracking().ToList();
            }
            return networkEntity.ToDomain(explorer);
        }

        public void DeleteNetworkData()
        {
            /*_unitOfWork.Context.Database.ExecuteSqlRaw(
                "ALTER TABLE [dbo].[CommittedProject] DROP CONSTRAINT[FK_CommittedProject_Section_SectionId];" +
                "ALTER TABLE [dbo].[NumericAttributeValueHistory] DROP CONSTRAINT[FK_NumericAttributeValueHistory_Section_SectionId];" +
                "ALTER TABLE [dbo].[TextAttributeValueHistory] DROP CONSTRAINT[FK_TextAttributeValueHistory_Section_SectionId];" +
                "ALTER TABLE [dbo].[NumericAttributeValueHistory] DROP CONSTRAINT[FK_NumericAttributeValueHistory_Attribute_AttributeId];" +
                "ALTER TABLE [dbo].[TextAttributeValueHistory] DROP CONSTRAINT[FK_TextAttributeValueHistory_Attribute_AttributeId];" +
                "ALTER TABLE [dbo].[Facility] DROP CONSTRAINT[FK_Facility_Network_NetworkId];" +
                "ALTER TABLE [dbo].[Section] DROP CONSTRAINT[FK_Section_Facility_FacilityId];" +
                "TRUNCATE TABLE [dbo].[Facility];" +
                "TRUNCATE TABLE [dbo].[Section];" +
                "TRUNCATE TABLE [dbo].[NumericAttributeValueHistory];" +
                "TRUNCATE TABLE [dbo].[TextAttributeValueHistory];" +
                "ALTER TABLE [dbo].[CommittedProject] WITH NOCHECK ADD CONSTRAINT[FK_CommittedProject_Section_SectionId] FOREIGN KEY([SectionId]) REFERENCES[dbo].[Section]([Id]) ON DELETE NO ACTION; ALTER TABLE[dbo].[CommittedProject] CHECK CONSTRAINT[FK_CommittedProject_Section_SectionId];" +
                "ALTER TABLE [dbo].[NumericAttributeValueHistory] WITH NOCHECK ADD CONSTRAINT[FK_NumericAttributeValueHistory_Section_SectionId] FOREIGN KEY([SectionId]) REFERENCES[dbo].[Section]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[NumericAttributeValueHistory] CHECK CONSTRAINT[FK_NumericAttributeValueHistory_Section_SectionId];" +
                "ALTER TABLE [dbo].[TextAttributeValueHistory] WITH NOCHECK ADD CONSTRAINT[FK_TextAttributeValueHistory_Section_SectionId] FOREIGN KEY([SectionId]) REFERENCES[dbo].[Section]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[TextAttributeValueHistory] CHECK CONSTRAINT[FK_TextAttributeValueHistory_Section_SectionId];" +
                "ALTER TABLE [dbo].[NumericAttributeValueHistory] WITH NOCHECK ADD CONSTRAINT[FK_NumericAttributeValueHistory_Attribute_AttributeId] FOREIGN KEY([AttributeId]) REFERENCES[dbo].[Attribute]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[NumericAttributeValueHistory] CHECK CONSTRAINT[FK_NumericAttributeValueHistory_Attribute_AttributeId];" +
                "ALTER TABLE [dbo].[TextAttributeValueHistory] WITH NOCHECK ADD CONSTRAINT[FK_TextAttributeValueHistory_Attribute_AttributeId] FOREIGN KEY([AttributeId]) REFERENCES[dbo].[Attribute]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[TextAttributeValueHistory] CHECK CONSTRAINT[FK_TextAttributeValueHistory_Attribute_AttributeId];" +
                "ALTER TABLE [dbo].[Facility] WITH NOCHECK ADD CONSTRAINT[FK_Facility_Network_NetworkId] FOREIGN KEY([NetworkId]) REFERENCES[dbo].[Network]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[Facility] CHECK CONSTRAINT[FK_Facility_Network_NetworkId];" +
                "ALTER TABLE [dbo].[Section] WITH NOCHECK ADD CONSTRAINT[FK_Section_Facility_FacilityId] FOREIGN KEY([FacilityId]) REFERENCES[dbo].[Facility]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[Section] CHECK CONSTRAINT[FK_Section_Facility_FacilityId];");*/

            _unitOfWork.Context.Database.ExecuteSqlRaw(
                $"DELETE FROM [dbo].[MaintainableAsset] WHERE [dbo].[MaintainableAsset].[NetworkId] = '{DataPersistenceConstants.PennDotNetworkId}'");
            _unitOfWork.Context.SaveChanges();
        }

        public void UpsertNetworkRollupDetail(Guid networkId, string status)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}.");
            }

            var networkRollupDetailEntity = new NetworkRollupDetailEntity {NetworkId = networkId, Status = status};

            _unitOfWork.Context.Upsert(networkRollupDetailEntity, _ => _.NetworkId == networkId,
                _unitOfWork.UserEntity?.Id);
        }
    }
}
