using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

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
                    Context.Network.Add(new NetworkEntity
                    {
                        Id = network.Id,
                        Name = network.Name
                    });

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

        public IEnumerable<DataAssignment.Networking.Network> GetAllNetworks()
        {
            if (Context.Network.Count() == 0)
            {
                throw new RowNotInTableException($"Cannot find networks in the database");
            }

            // consumer of this call will only need the network information. Not the maintainable assest information
            return Context.Network.Select(_ => _.ToDomain()).ToList();
        }

        public Domains.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer)
        {
            if (!Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var networkEntity = Context.Network
                .Include(_ => _.Facilities)
                .ThenInclude(_ => _.Sections)
                .Single(_ => _.Id == networkId);

            if (networkEntity.Facilities.Any(_ => _.Sections.Any()))
            {
                var sectionsIds = networkEntity.Facilities.SelectMany(_ => _.Sections.Select(__ => __.Id)).ToList();

                var numericAttributeValueHistoryEntities = Context.NumericAttributeValueHistory
                    .Include(_ => _.Attribute)
                    .Where(_ => sectionsIds.Contains(_.SectionId)).ToList()
                    .GroupBy(_ => _.SectionId, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.ToHashSet());

                var numericAttributeValueHistoryMostRecentValueEntities = Context.NumericAttributeValueHistoryMostRecentValue
                    .Include(_ => _.Attribute)
                    .Where(_ => sectionsIds.Contains(_.SectionId)).ToList()
                    .GroupBy(_ => _.SectionId, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.ToHashSet());

                var textAttributeValueHistoryEntities = Context.TextAttributeValueHistory
                    .Include(_ => _.Attribute)
                    .Where(_ => sectionsIds.Contains(_.SectionId)).ToList()
                    .GroupBy(_ => _.SectionId, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.ToHashSet());

                var textAttributeValueHistoryMostRecentValueEntities = Context.TextAttributeValueHistoryMostRecentValue
                    .Include(_ => _.Attribute)
                    .Where(_ => sectionsIds.Contains(_.SectionId)).ToList()
                    .GroupBy(_ => _.SectionId, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.ToHashSet());

                networkEntity.Facilities.ForEach(facility =>
                {
                    facility.Sections.ForEach(section =>
                    {
                        if (numericAttributeValueHistoryEntities.ContainsKey(section.Id))
                        {
                            section.NumericAttributeValueHistories = numericAttributeValueHistoryEntities[section.Id];
                        }

                        if (numericAttributeValueHistoryMostRecentValueEntities.ContainsKey(section.Id))
                        {
                            section.NumericAttributeValueHistoryMostRecentValues = numericAttributeValueHistoryMostRecentValueEntities[section.Id];
                        }

                        if (textAttributeValueHistoryEntities.ContainsKey(section.Id))
                        {
                            section.TextAttributeValueHistories = textAttributeValueHistoryEntities[section.Id];
                        }

                        if (textAttributeValueHistoryMostRecentValueEntities.ContainsKey(section.Id))
                        {
                            section.TextAttributeValueHistoryMostRecentValues = textAttributeValueHistoryMostRecentValueEntities[section.Id];
                        }
                    });
                });
            }

            return networkEntity.ToSimulationAnalysisDomain(explorer);

            /*return Context.Network
                .Include(_ => _.Facilities)
                .ThenInclude(_ => _.Sections)
                .ThenInclude(_ => _.NumericAttributeValueHistories)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Facilities)
                .ThenInclude(_ => _.Sections)
                .ThenInclude(_ => _.TextAttributeValueHistories)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Facilities)
                .ThenInclude(_ => _.Sections)
                .ThenInclude(_ => _.NumericAttributeValueHistoryMostRecentValues)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.Facilities)
                .ThenInclude(_ => _.Sections)
                .ThenInclude(_ => _.TextAttributeValueHistoryMostRecentValues)
                .ThenInclude(_ => _.Attribute)
                .Single(_ => _.Name == networkName)
                .ToSimulationAnalysisDomain(explorer);*/
        }
    }
}
