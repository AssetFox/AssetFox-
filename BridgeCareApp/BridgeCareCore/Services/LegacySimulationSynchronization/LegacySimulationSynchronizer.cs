using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using BridgeCareCore.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BridgeCareCore.Services.LegacySimulationSynchronization
{
    public class LegacySimulationSynchronizer
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private const int NetworkId = 13;

        private readonly IAttributeMetaDataRepository _attributeMetaDataRepo;
        private readonly IAttributeRepository _attributeRepo;
        private readonly INetworkRepository _networkRepo;
        private readonly IFacilityRepository _facilityRepo;
        private readonly ISimulationRepository _simulationRepo;
        private readonly IInvestmentPlanRepository _investmentPlanRepo;
        private readonly IAnalysisMethodRepository _analysisMethodRepo;
        private readonly IPerformanceCurveRepository _performanceCurveRepo;
        private readonly ISelectableTreatmentRepository _selectableTreatmentRepo;
        private readonly IConfiguration _config;
        private readonly IHubContext<BridgeCareHub> HubContext;

        public LegacySimulationSynchronizer(IAttributeMetaDataRepository attributeMetaDataRepo, IAttributeRepository attributeRepo, INetworkRepository networkRepo,
            IFacilityRepository facilityRepo, ISimulationRepository simulationRepo, IInvestmentPlanRepository investmentPlanRepo,
            IAnalysisMethodRepository analysisMethodRepo, IPerformanceCurveRepository performanceCurveRepo, ISelectableTreatmentRepository selectableTreatmentRepo,
            IConfiguration config, IHubContext<BridgeCareHub> hub)
        {
            _attributeMetaDataRepo = attributeMetaDataRepo ?? throw new ArgumentNullException(nameof(attributeMetaDataRepo));
            _attributeRepo = attributeRepo ?? throw new ArgumentNullException(nameof(attributeRepo));
            _networkRepo = networkRepo ?? throw new ArgumentNullException(nameof(networkRepo));
            _facilityRepo = facilityRepo ?? throw new ArgumentNullException(nameof(facilityRepo));
            _simulationRepo = simulationRepo ?? throw new ArgumentNullException(nameof(simulationRepo));
            _investmentPlanRepo = investmentPlanRepo ?? throw new ArgumentNullException(nameof(investmentPlanRepo));
            _analysisMethodRepo = analysisMethodRepo ?? throw new ArgumentNullException(nameof(analysisMethodRepo));
            _performanceCurveRepo = performanceCurveRepo ?? throw new ArgumentNullException(nameof(performanceCurveRepo));
            _selectableTreatmentRepo = selectableTreatmentRepo ?? throw new ArgumentNullException(nameof(selectableTreatmentRepo));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            HubContext = hub;
        }

        public void SynchronizeLegacySimulation(int legacySimulationId)
        {
            using (var sqlConnection = new SqlConnection(_config.GetConnectionString("BridgeCareLegacyConnex")))
            {
                try
                {
                    // open the sql connection and create new instance of data accessor
                    sqlConnection.Open();
                    var dataAccessor = new DataAccessor(sqlConnection, null);

                    var broadcastingMessage = "upserting attributes";
                    sendRealTimeMessage(broadcastingMessage, legacySimulationId);

                    // ensure all attributes have been created
                    _attributeRepo.UpsertAttributes(_attributeMetaDataRepo.GetAllAttributes().ToList());

                    sendRealTimeMessage("getting stand alone simulation", legacySimulationId);
                    // get the stand alone simulation
                    var simulation = dataAccessor.GetStandAloneSimulation(NetworkId, legacySimulationId);
                    // TODO: hard-coding simulation id for alpha 1
                    simulation.Id = new Guid(DataPersistenceConstants.TestSimulationId);
                    simulation.Name = $"{simulation.Name} Alpha 1";

                    // delete all existing simulation data before migrating
                    // TODO: this is for alpha 1 only; will have a clean database when doing the full migration
                    _simulationRepo.DeleteSimulationAndAllRelatedData();

                    sendRealTimeMessage("joining attributes with equations and criteria", legacySimulationId);
                    // join attributes with equations and criteria per the explorer object
                    var explorer = simulation.Network.Explorer;
                    _attributeRepo.JoinAttributesWithEquationsAndCriteria(explorer);

                    // create network unless penndot network already exists
                    var networks = _networkRepo.GetAllNetworks().ToList();
                    if (networks.Any())
                    {
                        explorer.Networks.First().Id = networks.First().Id;
                    }
                    else
                    {
                        explorer.Networks.First().Id = new Guid(DataPersistenceConstants.PennDotNetworkId);
                        _networkRepo.CreateNetwork(explorer.Networks.First());
                    }

                    sendRealTimeMessage("creating the network's facilities and sections", legacySimulationId);
                    // create the network's facilities and sections
                    _facilityRepo.CreateFacilities(explorer.Networks.First().Facilities.ToList(), explorer.Networks.First().Id);

                    sendRealTimeMessage("insert simulation data into the new data source", legacySimulationId);
                    // insert simulation data into our new data source
                    _simulationRepo.CreateSimulation(simulation);
                    _investmentPlanRepo.CreateInvestmentPlan(simulation.InvestmentPlan, simulation.Id);
                    _analysisMethodRepo.CreateAnalysisMethod(simulation.AnalysisMethod, simulation.Id);
                    _performanceCurveRepo.CreatePerformanceCurveLibrary($"{simulation.Name} Performance Curve Library", simulation.Id);
                    _performanceCurveRepo.CreatePerformanceCurves(simulation.PerformanceCurves.ToList(), simulation.Id);
                    _selectableTreatmentRepo.CreateTreatmentLibrary($"{simulation.Name} Treatment Library", simulation.Id);
                    _selectableTreatmentRepo.CreateSelectableTreatments(simulation.Treatments.ToList(), simulation.Id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        private void sendRealTimeMessage(string message, int legacySimulationId)
        {
            if (!IsRunningFromXUnit)
            {
                HubContext
                    .Clients
                    .All
                    .SendAsync("BroadcastDataMigration", message, legacySimulationId);
            }
        }
    }
}
