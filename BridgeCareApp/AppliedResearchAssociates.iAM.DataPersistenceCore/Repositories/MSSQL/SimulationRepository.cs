using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationRepository : MSSQLRepository, ISimulationRepository
    {
        /*private readonly IAnalysisMethodRepository _analysisMethodRepo;
        private readonly ICommittedProjectRepository _committedProjectRepo;
        private readonly IInvestmentPlanRepository _investmentPlanRepo;
        private readonly IPerformanceCurveRepository _performanceCurveRepo;
        private readonly ISelectableTreatmentRepository _selectableTreatmentRepo;*/
        
        public SimulationRepository(/*IAnalysisMethodRepository analysisMethodRepo,
            ICommittedProjectRepository committedProjetRepo,
            IInvestmentPlanRepository investmentPlanRepo,
            IPerformanceCurveRepository performanceCurveRepo,
            ISelectableTreatmentRepository selectableTreatmentRepo,*/
            IAMContext context) : base(context)
        {
            /*_analysisMethodRepo = analysisMethodRepo ?? throw new ArgumentNullException(nameof(analysisMethodRepo));
            _committedProjectRepo = committedProjetRepo ?? throw new ArgumentNullException(nameof(committedProjetRepo));
            _investmentPlanRepo = investmentPlanRepo ?? throw new ArgumentNullException(nameof(investmentPlanRepo));
            _performanceCurveRepo = performanceCurveRepo ?? throw new ArgumentNullException(nameof(performanceCurveRepo));
            _selectableTreatmentRepo = selectableTreatmentRepo ?? throw new ArgumentNullException(nameof(selectableTreatmentRepo));*/
        }

        public void CreateSimulation(Simulation simulation)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Network.Any(_ => _.Name == simulation.Network.Name))
                    {
                        throw new RowNotInTableException($"No network found having name {simulation.Network.Name}");
                    }

                    var network = Context.Network.Single(_ => _.Name == simulation.Network.Name);

                    Context.Simulation.Add(simulation.ToEntity(network.Id));
                    Context.SaveChanges();

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void GetAllInNetwork(Network network)
        {
            if (!Context.Network.Any(_ => _.Name == network.Name))
            {
                throw new RowNotInTableException($"No network found having name {network.Name}");
            }

            var entities = Context.Simulation.Where(_ => _.Network.Name == network.Name).ToList();

            entities.ForEach(_ => _.CreateSimulation(network));
        }
    }
}
