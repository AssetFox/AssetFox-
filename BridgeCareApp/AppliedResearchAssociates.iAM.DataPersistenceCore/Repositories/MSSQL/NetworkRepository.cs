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
using System.Threading;
using AppliedResearchAssociates.iAM.Common.Logging;
using Microsoft.Data.SqlClient;

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

        public NetworkEntity GetRawNetwork()
        {
            var rawDataNetworkId = new Guid(_unitOfWork.Context.AdminSettings.Where(_ => _.Key == "rawDataNetwork").SingleOrDefault().Value);

            if (!_unitOfWork.Context.Network.Any(_ => _.Id == rawDataNetworkId))
            {
                throw new RowNotInTableException("Unable to find raw Data network ID specified in appsettings.json");
            }

            return _unitOfWork.Context.Network
                .Single(_ => _.Id == rawDataNetworkId);
        }

        public Analysis.Network GetSimulationAnalysisNetwork(Guid networkId, Explorer explorer, bool areFacilitiesRequired = true, Guid? simulationId = null)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var networkEntity = _unitOfWork.Context.Network.AsNoTracking()
                .Single(_ => _.Id == networkId);

            if (areFacilitiesRequired)
            {
                var attributeIdLookup = getAttributeIdLookUp();
                networkEntity.MaintainableAssets = GetInitialQuery()
                                                    .Where(_ => _.NetworkId == networkId)
                                                    .Select(asset => GetMaintainableAssetEntity(asset, attributeIdLookup)).AsNoTracking().ToList();
            }

            if (!areFacilitiesRequired && simulationId != null)
            {
                // Load Assets corresponding to simulation's committed projects(this case is used by simulation pre-checks system)
                var attributeIdLookup = getAttributeIdLookUp();
                var assetIdsInCommittedProjectsForSimulation = _unitOfWork.MaintainableAssetRepo.GetAllIdsInCommittedProjectsForSimulation((Guid)simulationId, networkId);
                networkEntity.MaintainableAssets = GetInitialQuery()
                                                    .Where(_ => assetIdsInCommittedProjectsForSimulation.Contains(_.Id))
                                                    .Select(asset => GetMaintainableAssetEntity(asset, attributeIdLookup)).AsNoTracking().ToList();
            }

            var domain = networkEntity.ToDomain(explorer);
            return domain;

            Dictionary<Guid,string> getAttributeIdLookUp()
            {
                var attributeIdLookup = new Dictionary<Guid, string>();
                var allAttributes = _unitOfWork.AttributeRepo.GetAttributes();
                foreach (var attribute in allAttributes)
                {
                    attributeIdLookup[attribute.Id] = attribute.Name;
                }
                return attributeIdLookup;
            }
        }

        private IQueryable<MaintainableAssetEntity> GetInitialQuery()
        {
            return _unitOfWork.Context.MaintainableAsset
                .Include(a => a.MaintainableAssetLocation)
                .Include(a => a.AggregatedResults)
                .AsSplitQuery();
        }

        private static MaintainableAssetEntity GetMaintainableAssetEntity(MaintainableAssetEntity asset, Dictionary<Guid, string> attributeIdLookup)
        {
            return new MaintainableAssetEntity
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
            };
        }

        public void DeleteNetwork(Guid networkId, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null)
        {
            try
            {
                queueLog ??= new DoNothingWorkQueueLog();
                _unitOfWork.BeginTransaction(); 
                //_unitOfWork.Context.Database.BeginTransaction(IsolationLevel.ReadUncommitted); locks

                if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
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

                //Slow Starts here
                // _unitOfWork.Context.DeleteEntity<NetworkEntity>(_ => _.Id == networkId);

                _unitOfWork.Context.Database.SetCommandTimeout(TimeSpan.FromSeconds(18000));

                // Create parameters for the stored procedure
                var retMessageParam = new SqlParameter("@RetMessage", SqlDbType.VarChar, 250);
                string retMessage = "";
                var networkGuidParam = new SqlParameter("@NetworkGuid", networkId);
                retMessageParam.Direction = ParameterDirection.Output;

                // Execute the stored procedure
                _unitOfWork.Context.Database.ExecuteSqlRaw("EXEC usp_delete_network @NetworkGuid, @RetMessage OUTPUT", networkGuidParam, retMessageParam);

                // Capture the success output value
                retMessage = retMessageParam.Value as string;

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
