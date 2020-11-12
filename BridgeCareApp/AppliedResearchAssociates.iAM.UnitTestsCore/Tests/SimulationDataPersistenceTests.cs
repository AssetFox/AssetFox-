using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAccess;
using DA = AppliedResearchAssociates.iAM.DataAssignment;
using DM = AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    [TestFixture]
    public class SimulationDataPersistenceTests
    {
        private static readonly int _networkId = 13;
        private static readonly int _simulationId = 1171;
        private static readonly string _sqlConnectionString = "data source=RMD-PPATORN2-LT\\SQLSERVER2014;initial catalog=DbBackup;persist security info=True;user id=sa;password=20Pikachu^;MultipleActiveResultSets=True;App=EntityFramework";

        private SqlConnection _sqlConnection;
        private DataAccessor _dataAccessor;
        private IAMContext _context;
        private INetworkRepository _networkRepository;
        private ISimulationRepository _simulationRepository;
        private IAttributeMetaDataRepository _attributeMetaDataRepository;
        private IAttributeRepository _attributeRepository;
        private IAnalysisMethodRepository _analysisMethodRepository;
        private IInvestmentPlanRepository _investmentPlanRepository;
        private IPerformanceCurveRepository _performanceCurveRepository;

        [SetUp]
        public void Setup()
        {
            _sqlConnection = new SqlConnection(_sqlConnectionString);
            _sqlConnection.Open();
            _dataAccessor = new DataAccessor(_sqlConnection, null);
            _context = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseInMemoryDatabase(databaseName: "IAMv2").Options);
            _networkRepository = new NetworkRepository(_context);
            _simulationRepository = new SimulationRepository(_context);
            _attributeMetaDataRepository = new AttributeMetaDataRepository();
            _attributeRepository = new AttributeRepository(new MaintainableAssetRepository(_context), _context);
            _analysisMethodRepository = new AnalysisMethodRepository(_context);
            _investmentPlanRepository = new InvestmentPlanRepository(_context);
            _performanceCurveRepository = new PerformanceCurveRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _sqlConnection.Close();
            _context.Database.EnsureDeleted();
        }

        private DA.Networking.Network CreateDataAssignmentNetwork(string name)
        {
            var network = new DA.Networking.Network(new List<DA.Networking.MaintainableAsset>(), Guid.NewGuid(), name);

            _networkRepository.CreateNetwork(network);

            return network;
        }

        [Test]
        public void TestCreateDataAccessorStandAloneSimulation()
        {
            var simulation = _dataAccessor.GetStandAloneSimulation(_networkId, _simulationId);

            Assert.IsNotNull(simulation);
            Assert.IsInstanceOf<Simulation>(simulation);
        }

        [Test]
        public void TestCreateSimulationEntityFromStandAloneSimulation()
        {
            var simulation = _dataAccessor.GetStandAloneSimulation(_networkId, _simulationId);

            var network = CreateDataAssignmentNetwork(simulation.Network.Name);

            var newSimulationEntitiesCount = _simulationRepository.CreateSimulations(new List<Simulation> { simulation }, network.Id);
            Assert.AreEqual(1, newSimulationEntitiesCount);

            var dataSourceSimulations = _simulationRepository.GetAllInNetwork(network.Id).ToList();

            Assert.AreEqual(newSimulationEntitiesCount, dataSourceSimulations.Count());
            Assert.AreEqual(simulation.Name, dataSourceSimulations[0].Name);
        }

        [Test]
        public void TestCreateAnalysisMethodEntityFromStandAloneSimulationAnalysisMethod()
        {
            var simulation = _dataAccessor.GetStandAloneSimulation(_networkId, _simulationId);

            var network = CreateDataAssignmentNetwork(simulation.Network.Name);

            _simulationRepository.CreateSimulations(new List<Simulation> { simulation }, network.Id);

            var attributes = _attributeMetaDataRepository.GetAllAttributes();
            _attributeRepository.UpsertAttributes(new List<DM.Attributes.Attribute>
            {
                attributes.Single(_ => _.Name == simulation.AnalysisMethod.Weighting.Name)
            });

            _analysisMethodRepository.CreateAnalysisMethod(simulation.AnalysisMethod, simulation.Name);

            var dataSourceAnalysisMethod = _analysisMethodRepository.GetSimulationAnalysisMethod(simulation.Name);

            Assert.IsNotNull(dataSourceAnalysisMethod);
            Assert.IsInstanceOf<AnalysisMethod>(dataSourceAnalysisMethod);

            Assert.AreEqual(simulation.AnalysisMethod.Weighting.Name, dataSourceAnalysisMethod.Weighting.Name);
            Assert.AreEqual(simulation.AnalysisMethod.OptimizationStrategy, dataSourceAnalysisMethod.OptimizationStrategy);
            Assert.AreEqual(simulation.AnalysisMethod.SpendingStrategy, dataSourceAnalysisMethod.SpendingStrategy);
            Assert.AreEqual(simulation.AnalysisMethod.Description, dataSourceAnalysisMethod.Description);
            Assert.AreEqual(simulation.AnalysisMethod.ShouldDeteriorateDuringCashFlow, dataSourceAnalysisMethod.ShouldDeteriorateDuringCashFlow);
            Assert.AreEqual(simulation.AnalysisMethod.ShouldUseExtraFundsAcrossBudgets, dataSourceAnalysisMethod.ShouldUseExtraFundsAcrossBudgets);
            Assert.AreEqual(simulation.AnalysisMethod.ShouldApplyMultipleFeasibleCosts, dataSourceAnalysisMethod.ShouldApplyMultipleFeasibleCosts);
        }

        [Test]
        public void TestCreateInvestmentPlanEntityFromStandAloneSimulationInvestmentPlan()
        {
            var simulation = _dataAccessor.GetStandAloneSimulation(_networkId, _simulationId);

            var network = CreateDataAssignmentNetwork(simulation.Network.Name);

            _simulationRepository.CreateSimulations(new List<Simulation> { simulation }, network.Id);

            _investmentPlanRepository.CreateInvestmentPlan(simulation.InvestmentPlan, simulation.Name);

            var dataSourceInvestmentPlan = _investmentPlanRepository.GetSimulationInvestmentPlan(simulation.Name);

            Assert.IsNotNull(dataSourceInvestmentPlan);
            Assert.IsInstanceOf<InvestmentPlan>(dataSourceInvestmentPlan);

            Assert.AreEqual(simulation.InvestmentPlan.FirstYearOfAnalysisPeriod, dataSourceInvestmentPlan.FirstYearOfAnalysisPeriod);
            Assert.AreEqual(simulation.InvestmentPlan.InflationRatePercentage, dataSourceInvestmentPlan.InflationRatePercentage);
            Assert.AreEqual(simulation.InvestmentPlan.MinimumProjectCostLimit, dataSourceInvestmentPlan.MinimumProjectCostLimit);
            Assert.AreEqual(simulation.InvestmentPlan.NumberOfYearsInAnalysisPeriod, dataSourceInvestmentPlan.NumberOfYearsInAnalysisPeriod);
        }

        [Test]
        public void TestCreatePerformanceCurveEntitiesFromStandAloneSimulationPerformanceCurves()
        {
            var simulation = _dataAccessor.GetStandAloneSimulation(_networkId, _simulationId);

            var network = CreateDataAssignmentNetwork(simulation.Network.Name);

            _simulationRepository.CreateSimulations(new List<Simulation> { simulation }, network.Id);

            var attributes = _attributeMetaDataRepository.GetAllAttributes();
            var attributeNames = simulation.PerformanceCurves.Select(_ => _.Attribute.Name).Distinct().ToList();
            _attributeRepository.UpsertAttributes(attributes.Where(_ => attributeNames.Contains(_.Name)).ToList());

            _performanceCurveRepository.CreatePerformanceCurveLibrary(new PerformanceCurveLibraryEntity
                {
                    Id = Guid.NewGuid(), Name = $"{simulation.Name} Performance Curve Library"
                },
                simulation.Name);

            var newPerformanceCurveEntitiesCount = _performanceCurveRepository
                .CreatePerformanceCurves(simulation.PerformanceCurves.ToList(), simulation.Name);

            Assert.AreEqual(simulation.PerformanceCurves.Count, newPerformanceCurveEntitiesCount);

        }
    }
}
