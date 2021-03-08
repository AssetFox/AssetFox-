using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using BridgeCareCore.Hubs;
using BridgeCareCore.Services.LegacySimulationSynchronization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public class SimulationAnalysisDataPersistenceTestHelper : TestHelper
    {
        private const int NetworkId = 13;
        private const int SimulationId = 1171;

        private readonly SqlConnection _sqlConnection;
        private readonly DataAccessor _dataAccessor;

        public IHubContext<BridgeCareHub> HubContext { get; set; }

        public Simulation StandAloneSimulation { get; set; }

        public SimulationAnalysisDataPersistenceTestHelper()
        {
            _sqlConnection = new SqlConnection(Config.GetConnectionString("BridgeCareLegacyConnex"));
            _sqlConnection.Open();
            _dataAccessor = new DataAccessor(_sqlConnection, null);
        }

        public Simulation GetStandAloneSimulation() => _dataAccessor.GetStandAloneSimulation(NetworkId, SimulationId);

        public override void CreateNetwork()
        {
            UnitOfDataPersistenceWork.NetworkRepo.CreateNetwork(StandAloneSimulation.Network);

            UnitOfDataPersistenceWork.FacilityRepo.CreateFacilities(StandAloneSimulation.Network.Facilities.ToList(), StandAloneSimulation.Network.Id);
        }

        public void CreateAttributeCriteriaAndEquationJoins() => UnitOfDataPersistenceWork.AttributeRepo.JoinAttributesWithEquationsAndCriteria(StandAloneSimulation.Network.Explorer);

        public void AddTreatmentSchedulings()
        {
            var year = StandAloneSimulation.InvestmentPlan.FirstYearOfAnalysisPeriod + 1;
            StandAloneSimulation.Treatments.ForEach(_ =>
            {
                var scheduling = _.Schedulings.GetAdd(new TreatmentScheduling());
                scheduling.OffsetToFutureYear = year;
                scheduling.Treatment = _;
                if (year <= StandAloneSimulation.InvestmentPlan.LastYearOfAnalysisPeriod)
                {
                    year++;
                }
            });
        }

        public async void SynchronizeLegacySimulation()
        {
            var legacySimulationSynchronizer = new LegacySimulationSynchronizer(HubContext, UnitOfDataPersistenceWork);
            await legacySimulationSynchronizer.Synchronize(SimulationId);
        }

        public void AddTreatmentSupersessions()
        {
            var treatmentsList = StandAloneSimulation.Treatments.ToList();
            StandAloneSimulation.Treatments.ForEach((_, index) =>
            {
                var supersession = _.AddSupersession();
                supersession.Criterion.Expression = _.FeasibilityCriteria.FirstOrDefault()?.Expression;
                var nextIndex = index + 1;
                supersession.Treatment = nextIndex > StandAloneSimulation.Treatments.Count() - 1
                    ? treatmentsList[0]
                    : treatmentsList[nextIndex];
            });
        }

        public void AddCommittedProjects()
        {
            var selectableTreatment = StandAloneSimulation.Treatments.First();

            var budget = StandAloneSimulation.InvestmentPlan.Budgets
                .Single(_ => _.Name == selectableTreatment.Budgets.First().Name);

            var committedProject = StandAloneSimulation.CommittedProjects
                .GetAdd(new CommittedProject(StandAloneSimulation.Network.Sections.First(), StandAloneSimulation.InvestmentPlan.FirstYearOfAnalysisPeriod));
            committedProject.Name = $"Committed Project Test";
            committedProject.Budget = budget;
            selectableTreatment.Consequences.DistinctBy(_ => _.Attribute).ForEach(_ =>
            {
                var consequence = committedProject.Consequences.GetAdd(new TreatmentConsequence());
                consequence.Attribute = _.Attribute;
                consequence.Change.Expression = _.Change.Expression;
            });
            committedProject.ShadowForAnyTreatment = selectableTreatment.ShadowForAnyTreatment;
            committedProject.ShadowForSameTreatment = selectableTreatment.ShadowForSameTreatment;
            committedProject.Cost = Convert.ToDouble(budget.YearlyAmounts.First().Value);
        }

        public void SetupAll()
        {
            StandAloneSimulation = GetStandAloneSimulation();
            CreateAttributes();
        }

        public void SetupForNetwork()
        {
            SetupAll();
        }

        public void SetupForSimulation()
        {
            SetupAll();
            CreateNetwork();
        }

        public void SetupForAnalysisMethod()
        {
            SetupAll();
            CreateNetwork();
            UnitOfDataPersistenceWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
            UnitOfDataPersistenceWork.InvestmentPlanRepo.CreateInvestmentPlan(StandAloneSimulation.InvestmentPlan, StandAloneSimulation.Id);
        }

        public void SetupForPerformanceCurves()
        {
            SetupAll();
            CreateNetwork();
            UnitOfDataPersistenceWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
        }

        public void SetupForSimulationOutput()
        {
            SetupAll();
            CreateNetwork();
            UnitOfDataPersistenceWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
        }

        public void SetupForInvestmentPlan()
        {
            SetupAll();
            CreateNetwork();
            UnitOfDataPersistenceWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
        }

        public void SetupForCommittedProjects()
        {
            SetupAll();
            CreateNetwork();
            UnitOfDataPersistenceWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
            UnitOfDataPersistenceWork.InvestmentPlanRepo.CreateInvestmentPlan(StandAloneSimulation.InvestmentPlan, StandAloneSimulation.Id);
            AddCommittedProjects();
        }

        public void SetupForSelectableTreatments()
        {
            SetupAll();
            CreateNetwork();
            UnitOfDataPersistenceWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
            UnitOfDataPersistenceWork.InvestmentPlanRepo.CreateInvestmentPlan(StandAloneSimulation.InvestmentPlan, StandAloneSimulation.Id);
            AddTreatmentSchedulings();
            AddTreatmentSupersessions();
        }

        public void SetupForFullSimulationAnalysisIntegration()
        {
            SetupAll();
        }

        public void SetupForLegacySimulationSynchronization()
        {
            StandAloneSimulation = GetStandAloneSimulation();
        }

        public override void CleanUp()
        {
            _sqlConnection.Close();
            DbContext.Database.EnsureDeleted();
            UnitOfDataPersistenceWork.Dispose();
        }
    }
}
