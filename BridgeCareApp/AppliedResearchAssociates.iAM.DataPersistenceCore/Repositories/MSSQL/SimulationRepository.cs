using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationRepository : ISimulationRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ISimulationAnalysisDetailRepository _simulationAnalysisDetailRepo;
        private readonly IConfiguration _config;
        private readonly IAMContext _context;

        public SimulationRepository(ISimulationAnalysisDetailRepository simulationAnalysisDetailRepo, IConfiguration config, IAMContext context)
        {
            _simulationAnalysisDetailRepo = simulationAnalysisDetailRepo ??
                                            throw new ArgumentNullException(nameof(simulationAnalysisDetailRepo));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void CreateSimulation(Simulation simulation)
        {
            using (var contextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!_context.Network.Any(_ => _.Id == simulation.Network.Id))
                    {
                        throw new RowNotInTableException($"No network found having id {simulation.Network.Id}");
                    }

                    _context.Simulation.Add(simulation.ToEntity());
                    _context.SaveChanges();

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
            if (!_context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id {network.Id}");
            }

            var entities = _context.Simulation.Where(_ => _.NetworkId == network.Id).ToList();

            entities.ForEach(_ => _.CreateSimulation(network));
        }

        public List<SimulationDTO> GetAllInNetwork(Guid networkId)
        {
            if (!_context.Network.Any(_ => _.Id == networkId))
            {
                //throw new RowNotInTableException($"No network found having id {networkId}");
                return null;
            }

            var entities = _context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Where(_ => _.NetworkId == networkId);

            return entities.Select(_ => _.ToDto()).ToList();
        }

        public void GetSimulationInNetwork(Guid simulationId, Network network)
        {
            if (!_context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id {network.Id}");
            }

            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _context.Simulation.Single(_ => _.Id == simulationId);

            simulationEntity.CreateSimulation(network);
        }

        public SimulationDTO GetSimulation(Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationDTO = _context.Simulation
                .Include(_ => _.SimulationOutput)
                .Single(_ => _.Id == simulationId)
                .ToDto();

            var simulationAnalysisDetail = _simulationAnalysisDetailRepo.GetSimulationAnalysisDetail(simulationId);

            simulationDTO.LastRun = simulationAnalysisDetail.LastRun;
            simulationDTO.Status = simulationAnalysisDetail.Status;
            simulationDTO.RunTime = simulationAnalysisDetail.RunTime;

            return simulationDTO;
        }

        public void DeleteSimulationAndAllRelatedData()
        {
            if (IsRunningFromXUnit)
            {
                _context.Simulation.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.Equation.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.CriterionLibrary.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.BudgetLibrary.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.BudgetPriorityLibrary.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.CashFlowRuleLibrary.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.DeficientConditionGoalLibrary.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.PerformanceCurveLibrary.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.RemainingLifeLimitLibrary.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.TargetConditionGoalLibrary.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.TreatmentLibrary.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.TextAttributeValueHistory.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.NumericAttributeValueHistory.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.Section.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);

                _context.Facility.ToList()
                    .ForEach(_ => _context.Entry(_).State = EntityState.Deleted);
            }
            else
            {
                using var connection = new SqlConnection(_config.GetConnectionString("BridgeCareConnex"));
                connection.Open();
                var command = new SqlCommand("DeleteAllForAlphaMigration", connection)
                {
                    CommandTimeout = 1800, CommandType = CommandType.StoredProcedure
                };
                command.ExecuteNonQuery();
            }
        }
    }
}
