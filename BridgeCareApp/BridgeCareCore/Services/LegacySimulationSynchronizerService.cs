using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BridgeCareCore.Services
{
    public class LegacySimulationSynchronizerService
    {
        private const int LegacyNetworkId = 13;

        private readonly IHubContext<BridgeCareHub> _hubContext;
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public LegacySimulationSynchronizerService(IHubContext<BridgeCareHub> hub, UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _hubContext = hub;
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        private DataAccessor GetDataAccessor() => new DataAccessor(_unitOfDataPersistenceWork.LegacyConnection, null);

        private void SynchronizeExplorerData()
        {
            sendRealTimeMessage("Upserting attributes...");

            _unitOfDataPersistenceWork.AttributeRepo.UpsertAttributes(_unitOfDataPersistenceWork.AttributeMetaDataRepo
                .GetAllAttributes().ToList());
        }

        private void SynchronizeNetwork(Network network)
        {
            if (_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                return;
            }

            sendRealTimeMessage("Creating the network...");

            _unitOfDataPersistenceWork.NetworkRepo.CreateNetwork(network);
        }

        private Task SynchronizeNetworkData(Network network)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id");
            }

            _unitOfDataPersistenceWork.MaintainableAssetRepo.CreateMaintainableAssets(network.Facilities.ToList(),
                network.Id);

            var networkFacilityNames = network.Facilities.Select(_ => _.Name).ToHashSet();
            var networkSectionNamesAndAreas = network.Sections.Select(_ => $"{_.Name}{_.Area}").ToHashSet();
            var locationNames = new HashSet<string>();
            var assetNamesAndAreas = new HashSet<string>();
            _unitOfDataPersistenceWork.Context
                .MaintainableAssetLocation
                .Include(_ => _.MaintainableAsset)
                .ToList()
                .ForEach(_ =>
                {
                    var facilitySectionName = _.LocationIdentifier.Split("/");
                    locationNames.Add(facilitySectionName[0]);
                    assetNamesAndAreas.Add($"{facilitySectionName[1]}{_.MaintainableAsset.Area}");
                });

            if (!networkFacilityNames.SetEquals(locationNames) || !networkSectionNamesAndAreas.SetEquals(assetNamesAndAreas))
            {
                _unitOfDataPersistenceWork.NetworkRepo.DeleteNetworkData();
                sendRealTimeMessage("Creating the network's facilities and sections...");
                _unitOfDataPersistenceWork.FacilityRepo.CreateFacilities(simulation.Network.Facilities.ToList(), simulation.Network.Id);
            }

            return Task.CompletedTask;
        }

        private Task SynchronizeLegacySimulation(Simulation simulation)
        {
            sendRealTimeMessage("Joining attributes with equations and criteria...");

            _unitOfDataPersistenceWork.AttributeRepo.JoinAttributesWithEquationsAndCriteria(simulation.Network.Explorer);

            sendRealTimeMessage("Inserting simulation data...");

            _unitOfDataPersistenceWork.SimulationRepo.CreateSimulation(simulation);
            _unitOfDataPersistenceWork.InvestmentPlanRepo.CreateInvestmentPlan(simulation.InvestmentPlan, simulation.Id);
            _unitOfDataPersistenceWork.AnalysisMethodRepo.CreateAnalysisMethod(simulation.AnalysisMethod, simulation.Id);
            _unitOfDataPersistenceWork.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{simulation.Name} Performance Curve Library", simulation.Id);
            _unitOfDataPersistenceWork.PerformanceCurveRepo.CreatePerformanceCurves(simulation.PerformanceCurves.ToList(), simulation.Id);
            _unitOfDataPersistenceWork.SelectableTreatmentRepo.CreateTreatmentLibrary($"{simulation.Name} Treatment Library", simulation.Id);
            _unitOfDataPersistenceWork.SelectableTreatmentRepo.CreateSelectableTreatments(simulation.Treatments.ToList(), simulation.Id);

            _unitOfDataPersistenceWork.CommittedProjectRepo.CreateCommittedProjects(simulation.CommittedProjects.ToList(), simulation.Id);

            return Task.CompletedTask;
        }

        public async Task Synchronize(int simulationId, UserInfoDTO userInfo)
        {
            try
            {
                var dataAccessor = GetDataAccessor();
                _unitOfDataPersistenceWork.LegacyConnection.Open();

                var network = dataAccessor.GetExplorer().Networks.FirstOrDefault();
                if (network != null)
                {
                    network.Id = new Guid(DataPersistenceConstants.PennDotNetworkId);
                }
                
                var simulation = dataAccessor.GetStandAloneSimulation(LegacyNetworkId, simulationId);

                _unitOfDataPersistenceWork.LegacyConnection.Close();

                _unitOfDataPersistenceWork.BeginTransaction();

                SynchronizeExplorerData();

                SynchronizeNetwork(network);

                await SynchronizeNetworkData(simulation);

                await SynchronizeLegacySimulation(simulation);

                _unitOfDataPersistenceWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfDataPersistenceWork.Rollback();
                throw;
            }
            finally
            {
                _unitOfDataPersistenceWork.Connection.Close();
                _unitOfDataPersistenceWork.LegacyConnection.Close();
            }
        }

        private void sendRealTimeMessage(string message)
        {
            if (!IsRunningFromXUnit)
            {
                _hubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastDataMigration", message);
            }
        }
    }
}
