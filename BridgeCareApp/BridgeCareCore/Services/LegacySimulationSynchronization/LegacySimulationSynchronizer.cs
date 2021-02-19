using System;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BridgeCareCore.Services.LegacySimulationSynchronization
{
    public class LegacySimulationSynchronizer
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private const int NetworkId = 13;

        private readonly IHubContext<BridgeCareHub> _hubContext;
        private readonly UnitOfWork _unitOfWork;

        public LegacySimulationSynchronizer(IHubContext<BridgeCareHub> hub, UnitOfWork unitOfWork)
        {
            _hubContext = hub;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        private DataAccessor GetDataAccessor() => new DataAccessor(_unitOfWork.LegacyConnection, null);

        private void SynchronizeExplorerData()
        {
            sendRealTimeMessage("Upserting attributes...");

            _unitOfWork.AttributeRepo.UpsertAttributes(_unitOfWork.AttributeMetaDataRepo.GetAllAttributes().ToList());
        }

        private void SynchronizeNetwork(Simulation simulation)
        {
            var network = _unitOfWork.NetworkRepo.GetPennDotNetwork();

            if (network == null)
            {
                sendRealTimeMessage("Creating the network...");

                _unitOfWork.NetworkRepo.CreateNetwork(simulation.Network);
            }
        }

        private Task SynchronizeLegacyNetworkData(Simulation simulation)
        {
            if (!_unitOfWork.NetworkRepo.CheckPennDotNetworkHasData())
            {
                _unitOfWork.NetworkRepo.DeleteNetworkData();

                sendRealTimeMessage("Creating the network's facilities and sections...");

                _unitOfWork.FacilityRepo.CreateFacilities(simulation.Network.Facilities.ToList(), simulation.Network.Id);
            }

            return Task.CompletedTask;
        }

        private Task SynchronizeLegacySimulation(Simulation simulation)
        {
            _unitOfWork.SimulationRepo.DeleteSimulationAndAllRelatedData();

            // TODO: hard-coding simulation id for alpha 1
            simulation.Id = new Guid(DataPersistenceConstants.TestSimulationId);
            simulation.Name = $"*{simulation.Name} Alpha 1";

            sendRealTimeMessage("Joining attributes with equations and criteria...");

            _unitOfWork.AttributeRepo.JoinAttributesWithEquationsAndCriteria(simulation.Network.Explorer);

            sendRealTimeMessage("Inserting simulation data...");

            _unitOfWork.SimulationRepo.CreateSimulation(simulation);
            _unitOfWork.InvestmentPlanRepo.CreateInvestmentPlan(simulation.InvestmentPlan, simulation.Id);
            _unitOfWork.AnalysisMethodRepo.CreateAnalysisMethod(simulation.AnalysisMethod, simulation.Id);
            _unitOfWork.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{simulation.Name} Performance Curve Library", simulation.Id);
            _unitOfWork.PerformanceCurveRepo.CreatePerformanceCurves(simulation.PerformanceCurves.ToList(), simulation.Id);
            _unitOfWork.SelectableTreatmentRepo.CreateTreatmentLibrary($"{simulation.Name} Treatment Library", simulation.Id);
            _unitOfWork.SelectableTreatmentRepo.CreateSelectableTreatments(simulation.Treatments.ToList(), simulation.Id);

            _unitOfWork.CommittedProjectRepo.CreateCommittedProjects(simulation.CommittedProjects.ToList(), simulation.Id);

            return Task.CompletedTask;
        }

        public async Task Synchronize(int simulationId)
        {
            try
            {
                using var transaction = _unitOfWork.DbContextTransaction;

                
                var dataAccessor = GetDataAccessor();
                _unitOfWork.LegacyConnection.Open();
                var simulation = dataAccessor.GetStandAloneSimulation(NetworkId, simulationId);
                simulation.Network.Id = new Guid(DataPersistenceConstants.PennDotNetworkId);
                _unitOfWork.LegacyConnection.Close();

                SynchronizeExplorerData();

                SynchronizeNetwork(simulation);

                await SynchronizeLegacyNetworkData(simulation);

                await SynchronizeLegacySimulation(simulation);

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw;
            }
            finally
            {
                _unitOfWork.Connection.Close();
                _unitOfWork.LegacyConnection.Close();
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
