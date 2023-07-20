using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using Network = AppliedResearchAssociates.iAM.Data.Networking.Network;
using AppliedResearchAssociates.iAM.Data.Networking;
using System.Threading;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Common.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : INetworkRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public NetworkRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        } 

        public void CreateNetwork(Network network)
        {
            // prevent EF from attempting to create the network's child entities (create them
            // separately as part of a bulk insert)

            //_unitOfWork.Context.Upsert(networkEntity, networkEntity.Id, _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddEntity(network.ToEntity(), _unitOfWork.UserEntity?.Id);

            _unitOfWork.MaintainableAssetRepo.CreateMaintainableAssets(network.MaintainableAssets.ToList(), network.Id);
        }

        public void CreateNetwork(Analysis.Network network) => _unitOfWork.Context.AddEntity(network.ToEntity(), _unitOfWork.UserEntity?.Id);

        public List<Network> GetAllNetworks()
        {
            var domain = _unitOfWork.Context.Network
                .Include(n => n.MaintainableAssets)
                .ThenInclude(ma => ma.MaintainableAssetLocation)
                .Select(e => e.ToDomain(_unitOfWork.EncryptionKey))
                .ToList();
            return domain;
        }

        public Task<List<NetworkDTO>> Networks()
        {
            if (!_unitOfWork.Context.Network.Any())
            {
                return Task.Factory.StartNew(() => new List<NetworkDTO>());
            }

            return Task.Factory.StartNew(() => {
                var attributeDbSet = _unitOfWork.Context.Attribute.ToList();
                return _unitOfWork.Context.Network
                .Include(_ => _.BenefitQuantifier)
                .ThenInclude(_ => _.Equation)
                .Include(_ => _.NetworkRollupDetail)
                .Include(_ => _.AttributeJoins)
                .Select(_ => _.ToDto(attributeDbSet, _unitOfWork.EncryptionKey))
                .ToList();
            });
        }

        public List<NetworkDTO> GetNetworksByIdsNoChildren(List<Guid> ids)
        {
            return _unitOfWork.Context.Network.Where(_ => ids.Contains(_.Id)).Select(_ => _.ToDto(null, _unitOfWork.EncryptionKey)).ToList();
        }

        public NetworkEntity GetMainNetwork()
        {
            var mainNetworkId =new Guid(_unitOfWork.Context.AdminSettings.Where(_ => _.Key == "PrimaryNetwork").SingleOrDefault().Value);

            if (!_unitOfWork.Context.Network.Any(_ => _.Id == mainNetworkId))
            {
                throw new RowNotInTableException("Unable to find primary network ID specified in appsettings.json");
            }

            return _unitOfWork.Context.Network
                .Single(_ => _.Id == mainNetworkId);
        }

        public Analysis.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer, bool areFacilitiesRequired = true)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var networkEntity = _unitOfWork.Context.Network.AsNoTracking()
                .Single(_ => _.Id == networkId);

            if (areFacilitiesRequired)
            {

                var allAttributes = _unitOfWork.AttributeRepo.GetAttributes();
                var attributeIdLookup = new Dictionary<Guid, string>();
                foreach (var attribute in allAttributes)
                {
                    attributeIdLookup[attribute.Id] = attribute.Name;
                }

                networkEntity.MaintainableAssets = _unitOfWork.Context.MaintainableAsset
                    .Include(a => a.MaintainableAssetLocation)
                    .Include(a => a.AggregatedResults)
                    .AsSplitQuery()
                    .Where(_ => _.NetworkId == networkId)
                    .Select(asset => new MaintainableAssetEntity
                    {
                        Id = asset.Id,
                        SpatialWeighting = asset.SpatialWeighting,
                        AssetName = asset.AssetName,
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
                                Name = attributeIdLookup[result.AttributeId],
                            }
                        }).ToList()
                    }).AsNoTracking().ToList();
            }

            var domain = networkEntity.ToDomain(explorer);
            return domain;
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

        public void DeleteNetwork(Guid networkId, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null)
        {
            try
            {
                queueLog ??= new DoNothingWorkQueueLog();
                _unitOfWork.BeginTransaction();
                                   
                if(cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                {
                    _unitOfWork.Rollback();
                    return;
                }
                queueLog.UpdateWorkQueueStatus("Deleting Benefit Quantifier");

                _unitOfWork.BenefitQuantifierRepo.DeleteBenefitQuantifier(networkId);
                                   
                if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                {
                    _unitOfWork.Rollback();
                    return;
                }
                queueLog.UpdateWorkQueueStatus("Deleting Simulations");

                _unitOfWork.SimulationRepo.DeleteSimulationsByNetworkId(networkId);

                if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                {
                    _unitOfWork.Rollback();
                    return;
                }

                var primaryNetwork = _unitOfWork.AdminSettingsRepo.GetPrimaryNetwork();
                if(primaryNetwork != null && primaryNetwork == GetNetworkName(networkId))
                {
                    _unitOfWork.AdminSettingsRepo.DeleteAdminSetting(AdminSettingsRepository.primaryNetworkKey);
                }

                queueLog.UpdateWorkQueueStatus("Deleting Maintainable Assets");

                _unitOfWork.Context.DeleteEntity<NetworkEntity>(_ => _.Id == networkId);
                

                if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                {
                    _unitOfWork.Rollback();
                    return;
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void UpsertNetworkRollupDetail(Guid networkId, string status)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException("The specified network was not found.");
            }

            var networkRollupDetailEntity = new NetworkRollupDetailEntity { NetworkId = networkId, Status = status };

            _unitOfWork.Context.Upsert(networkRollupDetailEntity, _ => _.NetworkId == networkId,
                _unitOfWork.UserEntity?.Id);
        }

        public string GetNetworkName(Guid networkId)
        {
            var entity = _unitOfWork.Context.Network.SingleOrDefault(n => n.Id == networkId);
            return entity.Name;
        }

        public string GetNetworkKeyAttribute(Guid networkId)
        {
            var entity = _unitOfWork.Context.Network.Where(_ => _.Id == networkId).Select(_ => _.KeyAttributeId).FirstOrDefault();
            if (entity == default)
            {
                throw new RowNotInTableException("The specified network was not found.");
            }

            return _unitOfWork.AttributeRepo.GetAttributeName(entity);
        }
    }
}
