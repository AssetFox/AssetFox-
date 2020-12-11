using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BridgeCareCore.Services.LegacySimulationSynchronization
{
    public class LegacySimulationSynchronizer
    {
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

        public LegacySimulationSynchronizer(IAttributeMetaDataRepository attributeMetaDataRepo, IAttributeRepository attributeRepo, INetworkRepository networkRepo,
            IFacilityRepository facilityRepo, ISimulationRepository simulationRepo, IInvestmentPlanRepository investmentPlanRepo,
            IAnalysisMethodRepository analysisMethodRepo, IPerformanceCurveRepository performanceCurveRepo, ISelectableTreatmentRepository selectableTreatmentRepo,
            IConfiguration config)
        {
            _attributeMetaDataRepo = attributeMetaDataRepo;
            _attributeRepo = attributeRepo;
            _networkRepo = networkRepo;
            _facilityRepo = facilityRepo;
            _simulationRepo = simulationRepo;
            _investmentPlanRepo = investmentPlanRepo;
            _analysisMethodRepo = analysisMethodRepo;
            _performanceCurveRepo = performanceCurveRepo;
            _selectableTreatmentRepo = selectableTreatmentRepo;
            _config = config;
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

                    // ensure all attributes have been created
                    _attributeRepo.UpsertAttributes(_attributeMetaDataRepo.GetAllAttributes().ToList());

                    // get the stand alone simulation
                    var simulation = dataAccessor.GetStandAloneSimulation(NetworkId, legacySimulationId);

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
                        _networkRepo.CreateNetwork(explorer.Networks.First());
                    }

                    // create the network's facilities and sections
                    _facilityRepo.CreateFacilities(explorer.Networks.First().Facilities.ToList(), explorer.Networks.First().Id);

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
    }
}
