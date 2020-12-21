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
    public class NetworkRepository : INetworkRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public NetworkRepository(UnitOfWork.UnitOfWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateNetwork(DataAssignment.Networking.Network network)
        {
            // prevent EF from attempting to create the network's child entities (create them
            // separately as part of a bulk insert)
            var networkEntity = new NetworkEntity
            {
                Id = new Guid(DataPersistenceConstants.PennDotNetworkId),
                Name = network.Name
            };
            _unitOfWork.Context.AddOrUpdate(networkEntity, networkEntity.Id);
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

        public void CreateNetwork(Network network)
        {
            _unitOfWork.Context.Network.Add(network.ToEntity());
            _unitOfWork.Context.SaveChanges();
        }

        public List<DataAssignment.Networking.Network> GetAllNetworks()
        {
            /*if (_unitOfWork.Context.Network.Count() == 0)
            {
                throw new RowNotInTableException($"Cannot find networks in the database");
            }*/

            // consumer of this call will only need the network information. Not the maintainable assest information
            return _unitOfWork.Context.Network.Select(_ => _.ToDomain()).ToList();
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
    }
}
