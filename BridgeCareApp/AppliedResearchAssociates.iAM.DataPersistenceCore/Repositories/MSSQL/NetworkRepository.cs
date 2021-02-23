using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.Domains;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : INetworkRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public NetworkRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) => _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

        public void CreateNetwork(DataAssignment.Networking.Network network)
        {
            // prevent EF from attempting to create the network's child entities (create them
            // separately as part of a bulk insert)
            var networkEntity = new NetworkEntity
            {
                Id = new Guid(DataPersistenceConstants.PennDotNetworkId),
                Name = network.Name
            };
            _unitOfDataPersistenceWork.Context.AddOrUpdate(networkEntity, networkEntity.Id);
            _unitOfDataPersistenceWork.Context.SaveChanges();

            // convert maintainable assets and all child domains to entities
            var maintainableAssetEntities = network.MaintainableAssets.Select(_ => _.ToEntity(network.Id)).ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.MaintainableAsset.AddRange(maintainableAssetEntities);
                _unitOfDataPersistenceWork.Context.SaveChanges();
                _unitOfDataPersistenceWork.Context.MaintainableAssetLocation.AddRange(maintainableAssetEntities.Select(_ => _.MaintainableAssetLocation).ToList());
            }
            else
            {
                // bulk insert maintainable assets
                _unitOfDataPersistenceWork.Context.BulkInsert(maintainableAssetEntities);
                _unitOfDataPersistenceWork.Context.SaveChanges();
                // bulk insert maintainable asset locations
                _unitOfDataPersistenceWork.Context.BulkInsert(maintainableAssetEntities.Select(_ => _.MaintainableAssetLocation).ToList());
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void CreateNetwork(Network network)
        {
            _unitOfDataPersistenceWork.Context.Network.Add(network.ToEntity());
            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public Task<List<NetworkDTO>> GetAllNetworks() =>
            Task.Factory
                .StartNew(() => _unitOfDataPersistenceWork.Context.Network.Select(_ => _.ToDto()).ToList());

        public NetworkEntity GetPennDotNetwork()
        {
            var penndotNetworkId = new Guid(DataPersistenceConstants.PennDotNetworkId);

            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == penndotNetworkId))
            {
                return null;
            }

            return _unitOfDataPersistenceWork.Context.Network
                .Single(_ => _.Id == penndotNetworkId);
        }

        public bool CheckPennDotNetworkHasData()
        {
            var penndotNetworkId = new Guid(DataPersistenceConstants.PennDotNetworkId);
            var facilityCount = _unitOfDataPersistenceWork.Context.Facility.Count(_ => _.NetworkId == penndotNetworkId);
            var sectionCount = _unitOfDataPersistenceWork.Context.Section.Count(_ => _.Facility.NetworkId == penndotNetworkId);
            var numericAttributeValueHistoryCount = _unitOfDataPersistenceWork.Context.NumericAttributeValueHistory
                .Count(_ => _.Section.Facility.NetworkId == penndotNetworkId);
            var textAttributeValueHistoryCount = _unitOfDataPersistenceWork.Context.TextAttributeValueHistory
                .Count(_ => _.Section.Facility.NetworkId == penndotNetworkId);

            return facilityCount > 0 && sectionCount > 0 && numericAttributeValueHistoryCount > 0 &&
                   textAttributeValueHistoryCount > 0;
        }

        public Domains.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer, bool areFacilitiesRequired = true)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var networkEntity = _unitOfDataPersistenceWork.Context.Network
                .Single(_ => _.Id == networkId);
            if (areFacilitiesRequired)
            {


                var facilityEntities = _unitOfDataPersistenceWork.Context.Facility
                    .Where(_ => _.Network.Id == networkId).ToList();

                if (facilityEntities.Any())
                {
                    networkEntity.Facilities = ToHashSetExtension.ToHashSet(facilityEntities);

                    var sectionEntities = _unitOfDataPersistenceWork.Context.Section
                        .Where(_ => _.Facility.Network.Id == networkId).ToList();

                    if (sectionEntities.Any())
                    {
                        var numericAttributeValueHistoryEntities = _unitOfDataPersistenceWork.Context.NumericAttributeValueHistory
                            .Where(_ => _.Section.Facility.Network.Id == networkId).ToList();

                        var textAttributeValueHistoryEntities = _unitOfDataPersistenceWork.Context.TextAttributeValueHistory
                            .Where(_ => _.Section.Facility.Network.Id == networkId).ToList();

                        if (numericAttributeValueHistoryEntities.Any() || textAttributeValueHistoryEntities.Any())
                        {
                            var numericValueHistoryAttributeIds = numericAttributeValueHistoryEntities.Select(_ => _.AttributeId).Distinct();
                            var textValueHistoryAttributeIds = textAttributeValueHistoryEntities.Select(_ => _.AttributeId).Distinct();
                            var attributeIds = numericValueHistoryAttributeIds.Union(textValueHistoryAttributeIds);

                            var attributeEntities = _unitOfDataPersistenceWork.Context.Attribute.Where(_ => attributeIds.Contains(_.Id)).ToList();

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
                _unitOfDataPersistenceWork.Context.Facility.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);
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
                _unitOfDataPersistenceWork.Context.Database.ExecuteSqlRaw(
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
                    "ALTER TABLE [dbo].[NumericAttributeValueHistory] WITH NOCHECK ADD CONSTRAINT[FK_NumericAttributeValueHistory_Section_SectionId] FOREIGN KEY([SectionId]) REFERENCES[dbo].[Section]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[NumericAttributeValueHistory] CHECK CONSTRAINT[FK_NumericAttributeValueHistory_Section_SectionId];" +
                    "ALTER TABLE [dbo].[TextAttributeValueHistory] WITH NOCHECK ADD CONSTRAINT[FK_TextAttributeValueHistory_Section_SectionId] FOREIGN KEY([SectionId]) REFERENCES[dbo].[Section]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[TextAttributeValueHistory] CHECK CONSTRAINT[FK_TextAttributeValueHistory_Section_SectionId];" +
                    "ALTER TABLE [dbo].[NumericAttributeValueHistory] WITH NOCHECK ADD CONSTRAINT[FK_NumericAttributeValueHistory_Attribute_AttributeId] FOREIGN KEY([AttributeId]) REFERENCES[dbo].[Attribute]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[NumericAttributeValueHistory] CHECK CONSTRAINT[FK_NumericAttributeValueHistory_Attribute_AttributeId];" +
                    "ALTER TABLE [dbo].[TextAttributeValueHistory] WITH NOCHECK ADD CONSTRAINT[FK_TextAttributeValueHistory_Attribute_AttributeId] FOREIGN KEY([AttributeId]) REFERENCES[dbo].[Attribute]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[TextAttributeValueHistory] CHECK CONSTRAINT[FK_TextAttributeValueHistory_Attribute_AttributeId];" +
                    "ALTER TABLE [dbo].[Facility] WITH NOCHECK ADD CONSTRAINT[FK_Facility_Network_NetworkId] FOREIGN KEY([NetworkId]) REFERENCES[dbo].[Network]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[Facility] CHECK CONSTRAINT[FK_Facility_Network_NetworkId];" +
                    "ALTER TABLE [dbo].[Section] WITH NOCHECK ADD CONSTRAINT[FK_Section_Facility_FacilityId] FOREIGN KEY([FacilityId]) REFERENCES[dbo].[Facility]([Id]) ON DELETE CASCADE; ALTER TABLE[dbo].[Section] CHECK CONSTRAINT[FK_Section_Facility_FacilityId];");
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
