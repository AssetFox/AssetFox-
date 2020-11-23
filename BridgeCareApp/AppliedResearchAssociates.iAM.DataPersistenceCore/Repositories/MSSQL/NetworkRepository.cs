using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM;

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

        public IEnumerable<DataAssignment.Networking.Network> GetAllNetworks()
        {
            if (Context.Network.Count() == 0)
            {
                throw new RowNotInTableException($"Cannot find networks in the database");
            }

            // consumer of this call will only need the network information. Not the maintainable assest information
            return Context.Network.Select(_ => _.ToDomain()).ToList();
        }

        public Domains.Network GetSimulationAnalysisNetwork(string networkName)
        {
            if (!Context.Network.Any(_ => _.Name == networkName))
            {
                throw new RowNotInTableException($"No network found having name {networkName}");
            }

            return Context.Network
                .Include(_ => _.Facilities)
                .ThenInclude(_ => _.Sections)
                .Single(_ => _.Name == networkName)
                .ToSimulationAnalysisNetworkDomain();
        }
    }
}
