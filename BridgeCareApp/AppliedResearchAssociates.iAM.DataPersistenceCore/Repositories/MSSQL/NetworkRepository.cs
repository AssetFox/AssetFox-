using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : INetworkRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public NetworkRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateNetwork(DataAssignment.Networking.Network network)
        {
            // prevent EF from attempting to create the network's child entities (create them
            // separately as part of a bulk insert)
            var networkEntity = new NetworkEntity
            {
                Id = new Guid(DataPersistenceConstants.PennDotNetworkId),
                Name = network.Name
            };
            _unitOfWork.Context.Upsert(networkEntity, networkEntity.Id);
            _unitOfWork.Context.SaveChanges();

            // convert maintainable assets and all child domains to entities
            var maintainableAssetEntities = network.MaintainableAssets.Select(_ => _.ToEntity(network.Id)).ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.MaintainableAsset.AddRange(maintainableAssetEntities);
                _unitOfWork.Context.SaveChanges();
                _unitOfWork.Context.MaintainableAssetLocation.AddRange(maintainableAssetEntities.Select(_ => _.MaintainableAssetLocation).ToList());
            }
            else
            {
                // bulk insert maintainable assets
                _unitOfWork.Context.BulkInsert(maintainableAssetEntities);
                _unitOfWork.Context.SaveChanges();
                // bulk insert maintainable asset locations
                _unitOfWork.Context.BulkInsert(maintainableAssetEntities.Select(_ => _.MaintainableAssetLocation).ToList());
            }

            _unitOfWork.Context.SaveChanges();
        }

        public void CreateNetwork(Network network) =>
            _unitOfWork.Context.AddEntity(network.ToEntity(), _unitOfWork.UserEntity?.Id);

        public List<DataAssignment.Networking.Network> GetAllNetworks() =>
            _unitOfWork.Context.Network.Select(_ => _.ToDomain()).ToList();

        public Task<List<NetworkDTO>> Networks()
        {
            if (!_unitOfWork.Context.Network.Any())
            {
                return Task.Factory.StartNew(() => new List<NetworkDTO>());
            }

            return Task.Factory.StartNew(() =>
                _unitOfWork.Context.Network.Select(_ => _.ToDto()).ToList());
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

            var networkEntity = _unitOfWork.Context.Network
                .Single(_ => _.Id == networkId);

            if (areFacilitiesRequired)
            {
                var facilityEntities = _unitOfWork.Context.Facility
                    .Where(_ => _.Network.Id == networkId).ToList();

                if (facilityEntities.Any())
                {
                    networkEntity.Facilities = ToHashSetExtension.ToHashSet(facilityEntities);

                    var sectionEntities = _unitOfWork.Context.Section
                        .Where(_ => _.Facility.Network.Id == networkId).ToList();

                    if (sectionEntities.Any())
                    {
                        var numericAttributeValueHistoryEntities = _unitOfWork.Context.NumericAttributeValueHistory
                            .Where(_ => _.Section.Facility.Network.Id == networkId).ToList();

                        var textAttributeValueHistoryEntities = _unitOfWork.Context.TextAttributeValueHistory
                            .Where(_ => _.Section.Facility.Network.Id == networkId).ToList();

                        if (numericAttributeValueHistoryEntities.Any() || textAttributeValueHistoryEntities.Any())
                        {
                            var numericValueHistoryAttributeIds = numericAttributeValueHistoryEntities.Select(_ => _.AttributeId).Distinct();
                            var textValueHistoryAttributeIds = textAttributeValueHistoryEntities.Select(_ => _.AttributeId).Distinct();
                            var attributeIds = numericValueHistoryAttributeIds.Union(textValueHistoryAttributeIds);

                            var attributeEntities = _unitOfWork.Context.Attribute.Where(_ => attributeIds.Contains(_.Id)).ToList();

                            ForEachExtension.ForEach(numericAttributeValueHistoryEntities,
                                entity => entity.Attribute = attributeEntities.Single(_ => _.Id == entity.AttributeId));

                            ForEachExtension.ForEach(textAttributeValueHistoryEntities,
                                entity => entity.Attribute = attributeEntities.Single(_ => _.Id == entity.AttributeId));

                            var numericAttributeValueHistoriesDict = numericAttributeValueHistoryEntities.GroupBy(_ => _.SectionId, _ => _)
                                .ToDictionary(_ => _.Key, ToHashSetExtension.ToHashSet);

                            var textAttributeValueHistoriesDict = textAttributeValueHistoryEntities.GroupBy(_ => _.SectionId, _ => _)
                                .ToDictionary(_ => _.Key, ToHashSetExtension.ToHashSet);

                            ForEachExtension.ForEach(sectionEntities, section =>
                            {
                                if (numericAttributeValueHistoriesDict.ContainsKey(section.Id))
                                {
                                    section.NumericAttributeValueHistories = numericAttributeValueHistoriesDict[section.Id];
                                }

                                if (textAttributeValueHistoriesDict.ContainsKey(section.Id))
                                {
                                    section.TextAttributeValueHistories = textAttributeValueHistoriesDict[section.Id];
                                }
                            });
                        }
                    }

                    var sectionsDict = sectionEntities.GroupBy(_ => _.FacilityId, _ => _)
                        .ToDictionary(_ => _.Key, ToHashSetExtension.ToHashSet);

                    ForEachExtension.ForEach(networkEntity.Facilities, facility => facility.Sections = sectionsDict[facility.Id]);
                }
            }
            return networkEntity.ToSimulationAnalysisDomain(explorer);
        }

        public void DeleteNetworkData()
        {
            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.Facility.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);
            }
            else
            {
                /*var command = new SqlCommand("DeleteNetworkDataForAlphaMigration", _unitOfWork.Connection)
                {
                    CommandTimeout = 1800,
                    CommandType = CommandType.StoredProcedure
                };
                _unitOfWork.Connection.Open();
                command.ExecuteNonQuery();
                _unitOfWork.Connection.Close();*/
                _unitOfWork.Context.Database.ExecuteSqlRaw(
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
                    "ALTER TABLE [dbo].[Section] WITH NOCHECK ADD CONSTRAINT[FK_Section_Facility_FacilityId] FOREIGN KEY([FacilityId]) REFERENCES[dbo].[Facility]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[Section] CHECK CONSTRAINT[FK_Section_Facility_FacilityId];");
            }

            _unitOfWork.Context.SaveChanges();
        }
    }
}
