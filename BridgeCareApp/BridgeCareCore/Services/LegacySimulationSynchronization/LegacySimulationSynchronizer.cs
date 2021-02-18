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
        private readonly UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public LegacySimulationSynchronizer(IHubContext<BridgeCareHub> hub, UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _hubContext = hub;
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        private DataAccessor GetDataAccessor() => new DataAccessor(_unitOfDataPersistenceWork.LegacyConnection, null);

        private void SynchronizeExplorerData()
        {
            sendRealTimeMessage("Upserting attributes...");

            _unitOfDataPersistenceWork.AttributeRepo.UpsertAttributes(_unitOfDataPersistenceWork.AttributeMetaDataRepo.GetAllAttributes().ToList());
        }

        private void SynchronizeNetwork(Simulation simulation)
        {
            var network = _unitOfDataPersistenceWork.NetworkRepo.GetPennDotNetwork();

            if (network == null)
            {
                sendRealTimeMessage("Creating the network...");

                _unitOfDataPersistenceWork.NetworkRepo.CreateNetwork(simulation.Network);
            }
        }

        private Task SynchronizeLegacyNetworkData(Simulation simulation)
        {
            if (!_unitOfDataPersistenceWork.NetworkRepo.CheckPennDotNetworkHasData())
            {
                _unitOfDataPersistenceWork.NetworkRepo.DeleteNetworkData();

                sendRealTimeMessage("Creating the network's facilities and sections...");

                _unitOfDataPersistenceWork.FacilityRepo.CreateFacilities(simulation.Network.Facilities.ToList(), simulation.Network.Id);
            }

            return Task.CompletedTask;
        }

        private Task SynchronizeLegacySimulation(Simulation simulation)
        {
            _unitOfDataPersistenceWork.SimulationRepo.DeleteSimulationAndAllRelatedData();

            // TODO: hard-coding simulation id for alpha 1
            simulation.Id = new Guid(DataPersistenceConstants.TestSimulationId);
            simulation.Name = $"*{simulation.Name} Alpha 1";

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

            return Task.CompletedTask;
        }

        public async Task Synchronize(int simulationId)
        {
            try
            {
                using var transaction = _unitOfDataPersistenceWork.DbContextTransaction;

                
                var dataAccessor = GetDataAccessor();
                _unitOfDataPersistenceWork.LegacyConnection.Open();
                var simulation = dataAccessor.GetStandAloneSimulation(NetworkId, simulationId);
                simulation.Network.Id = new Guid(DataPersistenceConstants.PennDotNetworkId);
                _unitOfDataPersistenceWork.LegacyConnection.Close();

                SynchronizeExplorerData();

                SynchronizeNetwork(simulation);

                await SynchronizeLegacyNetworkData(simulation);

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
