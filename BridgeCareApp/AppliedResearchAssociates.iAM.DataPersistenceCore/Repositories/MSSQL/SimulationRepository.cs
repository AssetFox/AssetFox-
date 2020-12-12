using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationRepository : MSSQLRepository, ISimulationRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public SimulationRepository(IAMContext context) : base(context) { }

        public void CreateSimulation(Simulation simulation)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Network.Any(_ => _.Id == simulation.Network.Id))
                    {
                        throw new RowNotInTableException($"No network found having id {simulation.Network.Id}");
                    }

                    Context.Simulation.Add(simulation.ToEntity());
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
            if (!Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id {network.Id}");
            }

            var entities = Context.Simulation.Where(_ => _.NetworkId == network.Id).ToList();

            entities.ForEach(_ => _.CreateSimulation(network));
        }

        public void GetSimulationInNetwork(Guid simulationId, Network network)
        {
            if (!Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id {network.Id}");
            }

            if (!Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = Context.Simulation.Single(_ => _.Id == simulationId);

            simulationEntity.CreateSimulation(network);
        }

        public SimulationDTO GetSimulation(Guid simulationId)
        {
            if (!Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            return Context.Simulation
                .Include(_ => _.SimulationOutput)
                .Single(_ => _.Id == simulationId)
                .ToDto();
        }

        public void DeleteSimulationAndAllRelatedData()
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                if (IsRunningFromXUnit)
                {
                    Context.Simulation.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.Equation.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.CriterionLibrary.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.BudgetLibrary.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.BudgetPriorityLibrary.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.CashFlowRuleLibrary.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.DeficientConditionGoalLibrary.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.PerformanceCurveLibrary.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.RemainingLifeLimitLibrary.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.TargetConditionGoalLibrary.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.TreatmentLibrary.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.TextAttributeValueHistory.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.NumericAttributeValueHistory.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.Section.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);

                    Context.Facility.ToList()
                        .ForEach(_ => Context.Entry(_).State = EntityState.Deleted);
                }
                else
                {
                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.Simulation");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.Equation");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.CriterionLibrary");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.BudgetLibrary");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.BudgetPriorityLibrary");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.CashFlowRuleLibrary");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.DeficientConditionGoalLibrary");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.PerformanceCurveLibrary");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.RemainingLifeLimitLibrary");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.TargetConditionGoalLibrary");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.TreatmentLibrary");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.TextAttributeValueHistory");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.NumericAttributeValueHistory");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.Section");

                    Context.Database.ExecuteSqlRaw("DELETE FROM dbo.Facility");
                }

                Context.SaveChanges();

                contextTransaction.Commit();
            }
        }
    }
}
