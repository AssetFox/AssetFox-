using System;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        public Task SynchronizeLegacySimulation(int legacySimulationId)
        {
            using var connection = new SqlConnection(_unitOfWork.Config.GetConnectionString("BridgeCareLegacyConnex"));
            using var transaction = _unitOfWork.DbContextTransaction;

            try
            {
                // delete all existing simulation data before migrating
                // TODO: this is for alpha 1 only; will have a clean database when doing the full migration
                _unitOfWork.SimulationRepo.DeleteSimulationAndAllRelatedData();

                // open the sql connection and create new instance of data accessor
                connection.Open();
                var dataAccessor = new DataAccessor(connection, null);

                var broadcastingMessage = "Upserting attributes...";
                sendRealTimeMessage(broadcastingMessage, legacySimulationId);

                // ensure all attributes have been created
                _unitOfWork.AttributeRepo.UpsertAttributes(_unitOfWork.AttributeMetaDataRepo.GetAllAttributes().ToList());

                sendRealTimeMessage("Getting stand alone simulation...", legacySimulationId);
                // get the stand alone simulation
                var simulation = dataAccessor.GetStandAloneSimulation(NetworkId, legacySimulationId);
                // TODO: hard-coding simulation id for alpha 1
                simulation.Id = new Guid(DataPersistenceConstants.TestSimulationId);
                simulation.Name = $"*{simulation.Name} Alpha 1";

                sendRealTimeMessage("Joining attributes with equations and criteria...", legacySimulationId);
                // join attributes with equations and criteria per the explorer object
                var explorer = simulation.Network.Explorer;
                _unitOfWork.AttributeRepo.JoinAttributesWithEquationsAndCriteria(explorer);

                // create network unless penndot network already exists
                var networks = _unitOfWork.NetworkRepo.GetAllNetworks().ToList();
                if (networks.Any())
                {
                    explorer.Networks.First().Id = networks.First().Id;
                }
                else
                {
                    explorer.Networks.First().Id = new Guid(DataPersistenceConstants.PennDotNetworkId);
                    _unitOfWork.NetworkRepo.CreateNetwork(explorer.Networks.First());
                }

                sendRealTimeMessage("Creating the network's facilities and sections...", legacySimulationId);
                // create the network's facilities and sections
                _unitOfWork.FacilityRepo.CreateFacilities(explorer.Networks.First().Facilities.ToList(), explorer.Networks.First().Id);

                sendRealTimeMessage("Inserting simulation data...", legacySimulationId);
                // insert simulation data into our new data source
                _unitOfWork.SimulationRepo.CreateSimulation(simulation);
                _unitOfWork.InvestmentPlanRepo.CreateInvestmentPlan(simulation.InvestmentPlan, simulation.Id);
                _unitOfWork.AnalysisMethodRepo.CreateAnalysisMethod(simulation.AnalysisMethod, simulation.Id);
                _unitOfWork.PerformanceCurveRepo.CreatePerformanceCurveLibrary($"{simulation.Name} Performance Curve Library", simulation.Id);
                _unitOfWork.PerformanceCurveRepo.CreatePerformanceCurves(simulation.PerformanceCurves.ToList(), simulation.Id);
                _unitOfWork.SelectableTreatmentRepo.CreateTreatmentLibrary($"{simulation.Name} Treatment Library", simulation.Id);
                _unitOfWork.SelectableTreatmentRepo.CreateSelectableTreatments(simulation.Treatments.ToList(), simulation.Id);

                _unitOfWork.Commit();

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        private void sendRealTimeMessage(string message, int legacySimulationId)
        {
            if (!IsRunningFromXUnit)
            {
                _hubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastDataMigration", message, legacySimulationId);
            }
        }
    }
}
