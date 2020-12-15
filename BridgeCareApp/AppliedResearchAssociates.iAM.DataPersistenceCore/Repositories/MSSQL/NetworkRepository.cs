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
using MoreLinq;
using MoreLinq.Extensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class NetworkRepository : MSSQLRepository, INetworkRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public NetworkRepository(IAMContext context) : base(context) { }

        public void CreateNetwork(DataAssignment.Networking.Network network)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    // prevent EF from attempting to create the network's child entities (create them
                    // separately as part of a bulk insert)
                    var networkEntity = new NetworkEntity
                    {
                        Id = new Guid(DataPersistenceConstants.PennDotNetworkId), Name = network.Name
                    };
                    Context.AddOrUpdate(networkEntity, networkEntity.Id);

                    // convert maintainable assets and all child domains to entities
                    var maintainableAssetEntities = network.MaintainableAssets.Select(_ => _.ToEntity(network.Id)).ToList();

                    if (IsRunningFromXUnit)
                    {
                        Context.MaintainableAsset.AddRange(maintainableAssetEntities);
                        Context.MaintainableAssetLocation.AddRange(maintainableAssetEntities.Select(_ => _.MaintainableAssetLocation).ToList());
                    }
                    else
                    {
                        // bulk insert maintainable assets
                        Context.BulkInsert(maintainableAssetEntities);

                        // bulk insert maintainable asset locations
                        Context.BulkInsert(maintainableAssetEntities.Select(_ => _.MaintainableAssetLocation).ToList());
                    }

                    Context.SaveChanges();

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void CreateNetwork(Network network)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    Context.Network.Add(network.ToEntity());

                    Context.SaveChanges();

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public List<DataAssignment.Networking.Network> GetAllNetworks()
        {
            /*if (Context.Network.Count() == 0)
            {
                throw new RowNotInTableException($"Cannot find networks in the database");
            }*/

            // consumer of this call will only need the network information. Not the maintainable assest information
            return Context.Network.Select(_ => _.ToDomain()).ToList();
        }

        public Domains.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer, bool areFacilitiesRequired = true)
        {
            if (!Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var networkEntity = Context.Network
                .Single(_ => _.Id == networkId);
            if (areFacilitiesRequired)
            {


                var facilityEntities = Context.Facility
                    .Where(_ => _.Network.Id == networkId).ToList();

                if (facilityEntities.Any())
                {
                    networkEntity.Facilities = ToHashSetExtension.ToHashSet(facilityEntities);

                    var sectionEntities = Context.Section
                        .Where(_ => _.Facility.Network.Id == networkId).ToList();

                    if (sectionEntities.Any())
                    {
                        var numericAttributeValueHistoryEntities = Context.NumericAttributeValueHistory
                            .Where(_ => _.Section.Facility.Network.Id == networkId).ToList();

                        var textAttributeValueHistoryEntities = Context.TextAttributeValueHistory
                            .Where(_ => _.Section.Facility.Network.Id == networkId).ToList();

                        if (numericAttributeValueHistoryEntities.Any() || textAttributeValueHistoryEntities.Any())
                        {
                            var numericValueHistoryAttributeIds = numericAttributeValueHistoryEntities.Select(_ => _.AttributeId).Distinct();
                            var textValueHistoryAttributeIds = textAttributeValueHistoryEntities.Select(_ => _.AttributeId).Distinct();
                            var attributeIds = numericValueHistoryAttributeIds.Union(textValueHistoryAttributeIds);

                            var attributeEntities = Context.Attribute.Where(_ => attributeIds.Contains(_.Id)).ToList();

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
    }
}
