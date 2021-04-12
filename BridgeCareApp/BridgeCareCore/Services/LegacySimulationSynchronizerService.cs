﻿using System;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Services
{
    public class LegacySimulationSynchronizerService
    {
        private const int LegacyNetworkId = 13;

        private readonly IHubContext<BridgeCareHub> _hubContext;
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public LegacySimulationSynchronizerService(IHubContext<BridgeCareHub> hub, UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _hubContext = hub;
            _unitOfWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        private void SynchronizeExplorerData()
        {
            SendRealTimeMessage("Upserting attributes...");

            _unitOfWork.AttributeRepo.UpsertAttributes(_unitOfWork.AttributeMetaDataRepo
                .GetAllAttributes().ToList());
        }

        private void SynchronizeNetwork(Network network)
        {
            if (_unitOfWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                return;
            }

            SendRealTimeMessage("Creating the network...");

            _unitOfWork.NetworkRepo.CreateNetwork(network);
        }

        private void SynchronizeLegacyNetworkData(Network network)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id {network.Id}.");
            }

            var facilityNames = network.Facilities.Select(_ => _.Name).ToHashSet();
            var sectionNamesAndAreas = network.Sections.Select(_ => _.Id).ToHashSet();
            var assetFacilityNames = _unitOfWork.Context.MaintainableAsset.Select(_ => _.FacilityName).ToHashSet();
            var assetSectionNamesAndAreas = _unitOfWork.Context.MaintainableAsset.Select(_ => _.Id).ToHashSet();
            if (!assetFacilityNames.SetEquals(facilityNames) || !assetSectionNamesAndAreas.SetEquals(sectionNamesAndAreas))
            {
                _unitOfWork.NetworkRepo.DeleteNetworkData();
                SendRealTimeMessage("Creating the network's maintainable assets...");
                var sections = network.Facilities.Where(_ => _.Sections.Any()).SelectMany(_ => _.Sections).ToList();
                _unitOfWork.MaintainableAssetRepo.CreateMaintainableAssets(sections, network.Id);
            }
        }

        private void SynchronizeLegacySimulation(Simulation simulation)
        {
            if (simulation.Network.Explorer.CalculatedFields.Any())
            {
                SendRealTimeMessage("Joining attributes with equations and criteria...");

                _unitOfWork.AttributeRepo.JoinAttributesWithEquationsAndCriteria(simulation.Network.Explorer);
            }

            SendRealTimeMessage("Inserting simulation data...");

            _unitOfWork.SimulationRepo.CreateSimulation(simulation);

            if (simulation.InvestmentPlan != null)
            {
                _unitOfWork.InvestmentPlanRepo.CreateInvestmentPlan(simulation.InvestmentPlan, simulation.Id);
            }

            if (simulation.AnalysisMethod != null)
            {
                _unitOfWork.AnalysisMethodRepo.CreateAnalysisMethod(simulation.AnalysisMethod, simulation.Id);
            }

            if (simulation.PerformanceCurves.Any())
            {
                _unitOfWork.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{simulation.Name} Performance Curve Library", simulation.Id);
                _unitOfWork.PerformanceCurveRepo.CreatePerformanceCurves(simulation.PerformanceCurves.ToList(), simulation.Id);
            }

            if (simulation.CommittedProjects.Any())
            {
                _unitOfWork.CommittedProjectRepo.CreateCommittedProjects(simulation.CommittedProjects.ToList(), simulation.Id);
            }

            if (simulation.Treatments.Any())
            {
                _unitOfWork.SelectableTreatmentRepo.CreateTreatmentLibrary($"{simulation.Name} Treatment Library", simulation.Id);
                _unitOfWork.SelectableTreatmentRepo.CreateSelectableTreatments(simulation.Treatments.ToList(), simulation.Id);
            }
        }

        public void Synchronize(int simulationId, string username)
        {
            try
            {
                _unitOfWork.SetUser(username);

                using var legacyConnection = _unitOfWork.GetLegacyConnection();

                var dataAccessor = new DataAccessor(legacyConnection, null);
                legacyConnection.Open();

                var simulation = dataAccessor.GetStandAloneSimulation(LegacyNetworkId, simulationId);

                if (simulation != null)
                {
                    var network = simulation.Network;
                    if (network != null)
                    {
                        network.Id = new Guid(DataPersistenceConstants.PennDotNetworkId);
                    }

                    _unitOfWork.BeginTransaction();

                    SynchronizeExplorerData();

                    SynchronizeNetwork(network);

                    SynchronizeLegacyNetworkData(network);

                    SynchronizeLegacySimulation(simulation);

                    _unitOfWork.Commit();
                }
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private void SendRealTimeMessage(string message) =>
            _hubContext?
                .Clients?
                .All?
                .SendAsync("BroadcastDataMigration", message);
    }
}