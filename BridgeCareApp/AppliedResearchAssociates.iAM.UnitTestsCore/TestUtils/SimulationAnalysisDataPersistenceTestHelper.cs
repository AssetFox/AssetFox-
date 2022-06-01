using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using System.IO;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils
{
    public class SimulationAnalysisDataPersistenceTestHelper : TestHelper
    {
        private const int NetworkId = 13;
        public UnitOfDataPersistenceWork UnitOfWorkForAnalysis { get; }
        public readonly IAMContext DbContextForAnalysis;
        public IConfiguration ConfigForAnalysis { get; }
        public Simulation StandAloneSimulation { get; set; }

        public SimulationAnalysisDataPersistenceTestHelper()
        {
            ConfigForAnalysis = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testConnections.json")
                .Build();

            var connectionString = Config.GetConnectionString("BridgeCareConnexRealDb");

            DbContextForAnalysis = new IAMContext(new DbContextOptionsBuilder<IAMContext>()
                .UseSqlServer(Config.GetConnectionString("BridgeCareConnexRealDb"))
                .Options);

            UnitOfWorkForAnalysis = new UnitOfDataPersistenceWork(ConfigForAnalysis, DbContextForAnalysis);
        }

        public void SetStandAloneSimulation(int simulationId)
        {
            var explorer = UnitOfWorkForAnalysis.AttributeRepo.GetExplorer();
            var network = UnitOfWorkForAnalysis.NetworkRepo.GetSimulationAnalysisNetwork(new Guid(DataPersistenceConstants.PennDotNetworkId), explorer, true);
            UnitOfWorkForAnalysis.SimulationRepo.GetSimulationInNetwork(new Guid("F70E0EAD-EC60-4D3B-B05D-D1FC1933EB60"), network);
            StandAloneSimulation = network.Simulations.First();
            UnitOfWorkForAnalysis.InvestmentPlanRepo.GetSimulationInvestmentPlan(StandAloneSimulation);
            UnitOfWorkForAnalysis.AnalysisMethodRepo.GetSimulationAnalysisMethod(StandAloneSimulation, "");
            UnitOfWorkForAnalysis.PerformanceCurveRepo.GetScenarioPerformanceCurves(StandAloneSimulation);
            UnitOfWorkForAnalysis.SelectableTreatmentRepo.GetScenarioSelectableTreatments(StandAloneSimulation);

            StandAloneSimulation.Network.Id = new Guid(DataPersistenceConstants.PennDotNetworkId);
        }

        public void ReduceNumberOfFacilitiesAndSections(Simulation simulation)
        {
            // Originally, this logic reduced to a single facility's sections. Facilities no longer
            // exist, but this should probably be updated so that it still reduces the number of
            // sections to be dealt with.
            var assets = simulation.Network.Assets/*.Take(10)*/.ToList();

            simulation.Network.ClearAssets();
            simulation.CommittedProjects.Clear();
            ReUpSectionsAndHistories(assets, simulation.Network);
        }

        public void ReduceNumberOfFacilitiesAndSectionsWithCommittedProjects(Simulation simulation)
        {
            var assets = new List<MaintainableAsset>();
            CommittedProject committedProject = null;

            // Originally, this logic reduced to a single facility's sections. Facilities no longer
            // exist, but this should probably be updated so that it still reduces the number of
            // sections to be dealt with.
            if (simulation.CommittedProjects.Any())
            {
                committedProject = simulation.CommittedProjects.First();
                assets.Add(simulation.Network.Assets.Single(_ => _.Id == committedProject.Asset.Network.Id)); // Only one asset? Too much reduction.
            }
            else
            {
                assets.AddRange(simulation.Network.Assets); // All assets? Not enough reduction.
            }

            simulation.Network.ClearAssets();
            simulation.CommittedProjects.Clear();
            ReUpSectionsAndHistories(assets, simulation.Network);
            if (committedProject != null)
            {
                var asset = simulation.Network.Assets.First();
                simulation.CommittedProjects.Clear();
                var newCommittedProject = simulation.CommittedProjects.GetAdd(new CommittedProject(asset, committedProject.Year));
                newCommittedProject.Name = committedProject.Name;
                newCommittedProject.ShadowForAnyTreatment = committedProject.ShadowForAnyTreatment;
                newCommittedProject.ShadowForSameTreatment = committedProject.ShadowForSameTreatment;
                newCommittedProject.Budget = committedProject.Budget;
                newCommittedProject.Cost = committedProject.Cost;
                committedProject.Consequences.ForEach(consequence =>
                {
                    var newConsequence = newCommittedProject.Consequences.GetAdd(new TreatmentConsequence());
                    newConsequence.Attribute = consequence.Attribute;
                    newConsequence.Change.Expression = consequence.Change.Expression;
                });
            }
        }

        public void ReUpSectionsAndHistories(List<MaintainableAsset> assets, Network network) =>
            assets.ForEach(_ =>
            {
                assets.ForEach(__ =>
                {
                    var asset = network.AddAsset();
                    asset.Id = __.Id;
                    asset.AssetName = __.AssetName;

                    if (__.HistoricalAttributes.Any())
                    {
                        __.HistoricalAttributes.ForEach(___ =>
                        {
                            if (network.Explorer.NumberAttributes.Any(numAttr => numAttr.Name == ___.Name))
                            {
                                var attribute = network.Explorer.NumberAttributes.Single(numAttr =>
                                    numAttr.Name == ___.Name);
                                CopyOldHistoryToNewHistory(__.GetHistory(attribute), asset.GetHistory(attribute));
                            }
                            else
                            {
                                var attribute = network.Explorer.TextAttributes.Single(txtAttr =>
                                    txtAttr.Name == ___.Name);
                                CopyOldHistoryToNewHistory(__.GetHistory(attribute), asset.GetHistory(attribute));
                            }
                        });
                    }
                });
            });

        private void CopyOldHistoryToNewHistory<T>(AttributeValueHistory<T> oldHistory, AttributeValueHistory<T> newHistory)
        {
            newHistory.MostRecentValue = oldHistory.MostRecentValue;
            newHistory.ForEach(oldHistory.Add);
        }

        public override void CreateNetwork()
        {
            UnitOfWork.NetworkRepo.CreateNetwork(StandAloneSimulation.Network);

            var assets = StandAloneSimulation.Network.Assets.ToList();
            UnitOfWork.MaintainableAssetRepo.CreateMaintainableAssets(assets, StandAloneSimulation.Network.Id);
        }

        public void CreateAttributeCriteriaAndEquationJoins() =>
            UnitOfWork.AttributeRepo.JoinAttributesWithEquationsAndCriteria(StandAloneSimulation.Network
                .Explorer);

        public void AddTreatmentSchedulings()
        {
            var year = StandAloneSimulation.InvestmentPlan.FirstYearOfAnalysisPeriod + 1;
            StandAloneSimulation.Treatments.ForEach(selectableTreatment =>
            {
                var scheduling = selectableTreatment.Schedulings.GetAdd(new TreatmentScheduling());
                scheduling.OffsetToFutureYear = year;
                scheduling.Treatment = selectableTreatment;
            });
        }


        public void AddTreatmentSupersessions()
        {
            var treatmentsList = StandAloneSimulation.Treatments.ToList();
            StandAloneSimulation.Treatments.ForEach((selectableTreatment, index) =>
            {
                var supersession = selectableTreatment.AddSupersession();
                supersession.Criterion.Expression = selectableTreatment.FeasibilityCriteria.FirstOrDefault()?.Expression;
                var nextIndex = index + 1;
                supersession.Treatment = nextIndex > StandAloneSimulation.Treatments.Count() - 1
                    ? treatmentsList[0]
                    : treatmentsList[nextIndex];
            });
        }

        public void SetupForSimulation()
        {
            CreateAttributes();
            CreateNetwork();
        }

        public void SetupForAll()
        {
            SetupForSimulation();
            UnitOfWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
        }

        public void SetupWithInvestmentPlan()
        {
            SetupForSimulation();
            UnitOfWork.SimulationRepo.CreateSimulation(StandAloneSimulation);
            UnitOfWork.InvestmentPlanRepo.CreateInvestmentPlan(StandAloneSimulation.InvestmentPlan, StandAloneSimulation.Id);
        }

        public void SetupForSelectableTreatments()
        {
            SetupWithInvestmentPlan();
            AddTreatmentSchedulings();
            AddTreatmentSupersessions();
        }
    }
}
