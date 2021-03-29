﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AppliedResearchAssociates.iAM.DataAccess;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.UnitTestsCore.Mocks;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.TestData
{
    public class SimulationAnalysisDataPersistenceTestHelper : TestHelper
    {
        private const int NetworkId = 13;

        private readonly SqlConnection _sqlConnection;
        private readonly DataAccessor _dataAccessor;

        public Simulation StandAloneSimulation { get; set; }

        public SimulationAnalysisDataPersistenceTestHelper()
        {
            _sqlConnection = UnitOfWork.GetLegacyConnection();
            _sqlConnection.Open();
            _dataAccessor = new DataAccessor(_sqlConnection, null);
        }

        public void SetStandAloneSimulation(int simulationId)
        {
            StandAloneSimulation = _dataAccessor.GetStandAloneSimulation(NetworkId, simulationId);
            StandAloneSimulation.Network.Id = new Guid(DataPersistenceConstants.PennDotNetworkId);
        }

        public void ReduceNumberOfFacilitiesAndSections(Simulation simulation)
        {
            var facilities = new List<Facility> { simulation.Network.Facilities.First(_ => _.Sections.Any()) };
            simulation.Network.ClearFacilities();
            simulation.CommittedProjects.Clear();
            ReUpFacilitiesSectionsAndHistories(facilities, simulation.Network);
        }

        public void ReduceNumberOfFacilitiesAndSectionsWithCommittedProjects(Simulation simulation)
        {
            var facilities = new List<Facility>();
            CommittedProject committedProject = null;
            if (simulation.CommittedProjects.Any())
            {
                committedProject = simulation.CommittedProjects.First();
                facilities.Add(simulation.Network.Facilities.Single(_ => _.Id == committedProject.Section.Facility.Id));
            }
            else
            {
                facilities.Add(simulation.Network.Facilities.First(_ => _.Sections.Any()));
            }

            simulation.Network.ClearFacilities();
            simulation.CommittedProjects.Clear();
            ReUpFacilitiesSectionsAndHistories(facilities, simulation.Network);
            if (committedProject != null)
            {
                var section = simulation.Network.Facilities.First().Sections.First();
                simulation.CommittedProjects.Clear();
                var newCommittedProject = simulation.CommittedProjects.GetAdd(new CommittedProject(section, committedProject.Year));
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

        public void ReUpFacilitiesSectionsAndHistories(List<Facility> facilities, Network network) =>
            facilities.ForEach(_ =>
            {
                var facility = network.AddFacility();
                facility.Id = _.Id;
                facility.Name = _.Name;

                if (_.Sections.Any())
                {
                    _.Sections.ForEach(__ =>
                    {
                        var section = facility.AddSection();
                        section.Id = __.Id;
                        section.Name = __.Name;
                        section.Area = __.Area;
                        section.AreaUnit = __.AreaUnit;

                        if (__.HistoricalAttributes.Any())
                        {
                            __.HistoricalAttributes.ForEach(___ =>
                            {
                                if (network.Explorer.NumberAttributes.Any(numAttr => numAttr.Name == ___.Name))
                                {
                                    var attribute = network.Explorer.NumberAttributes.Single(numAttr =>
                                        numAttr.Name == ___.Name);
                                    CopyOldHistoryToNewHistory(__.GetHistory(attribute), section.GetHistory(attribute));
                                }
                                else
                                {
                                    var attribute = network.Explorer.TextAttributes.Single(txtAttr =>
                                        txtAttr.Name == ___.Name);
                                    CopyOldHistoryToNewHistory(__.GetHistory(attribute), section.GetHistory(attribute));
                                }
                            });
                        }
                    });
                }
            });

        private void CopyOldHistoryToNewHistory<T>(AttributeValueHistory<T> oldHistory, AttributeValueHistory<T> newHistory)
        {
            newHistory.MostRecentValue = oldHistory.MostRecentValue;
            newHistory.ForEach(oldHistory.Add);
        }

        public override void CreateNetwork()
        {
            UnitOfWork.NetworkRepo.CreateNetwork(StandAloneSimulation.Network);

            var sections = StandAloneSimulation.Network.Facilities.Where(_ => _.Sections.Any()).SelectMany(_ => _.Sections).ToList();
            UnitOfWork.MaintainableAssetRepo.CreateMaintainableAssets(sections, StandAloneSimulation.Network.Id);
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

        public void SynchronizeLegacySimulation(int simulationId)
        {
            var mockLegacySimulationSynchronizer = new MockLegacySimulationSynchronizerService(MockHubContext.Object, UnitOfWork, this);
            mockLegacySimulationSynchronizer.Synchronize(simulationId, TestUser.Username);
        }

        public void SynchronizeLegacySimulationWithCommittedProjects(int simulationId)
        {
            var mockLegacySimulationSynchronizer = new MockLegacySimulationSynchronizerService(MockHubContext.Object, UnitOfWork, this);
            mockLegacySimulationSynchronizer.Synchronize(simulationId, TestUser.Username, true);
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

        public override void CleanUp()
        {
            _sqlConnection.Close();
            UnitOfWork.Context.Database.EnsureDeleted();
            UnitOfWork.Dispose();
        }
    }
}
