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

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public SimulationRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateSimulation(Simulation simulation)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == simulation.Network.Id))
            {
                throw new RowNotInTableException($"No network found having id {simulation.Network.Id}");
            }

            _unitOfWork.Context.Simulation.Add(simulation.ToEntity());
            _unitOfWork.Context.SaveChanges();
        }

        public void GetAllInNetwork(Network network)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id {network.Id}");
            }

            var entities = _unitOfWork.Context.Simulation.Where(_ => _.NetworkId == network.Id).ToList();

            entities.ForEach(_ => _.CreateSimulation(network));
        }

        public List<SimulationDTO> GetAllInNetwork(Guid networkId)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
            {
                //throw new RowNotInTableException($"No network found having id {networkId}");
                return null;
            }

            var entities = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Where(_ => _.NetworkId == networkId);

            return entities.Select(_ => _.ToDto()).ToList();
        }

        public void GetSimulationInNetwork(Guid simulationId, Network network)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id {network.Id}");
            }

            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);

            simulationEntity.CreateSimulation(network);
        }

        public SimulationDTO GetSimulation(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationDTO = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationOutput)
                .Single(_ => _.Id == simulationId)
                .ToDto();

            var simulationAnalysisDetail = _unitOfWork.SimulationAnalysisDetailRepo.GetSimulationAnalysisDetail(simulationId);

            simulationDTO.LastRun = simulationAnalysisDetail.LastRun;
            simulationDTO.Status = simulationAnalysisDetail.Status;
            simulationDTO.RunTime = simulationAnalysisDetail.RunTime;

            return simulationDTO;
        }

        public void DeleteSimulationAndAllRelatedData()
        {
            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.Simulation.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.Equation.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.CriterionLibrary.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.BudgetLibrary.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.BudgetPriorityLibrary.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.CashFlowRuleLibrary.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.DeficientConditionGoalLibrary.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.PerformanceCurveLibrary.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.RemainingLifeLimitLibrary.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.TargetConditionGoalLibrary.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfWork.Context.TreatmentLibrary.ToList()
                    .ForEach(_ => _unitOfWork.Context.Entry(_).State = EntityState.Deleted);
            }
            else
            {

                /*var command = new SqlCommand("DeleteAllExceptNetworkDataForAlphaMigration", _unitOfWork.Connection)
                {
                    CommandTimeout = 1800, CommandType = CommandType.StoredProcedure
                };
                _unitOfWork.Connection.Open();
                command.ExecuteNonQuery();
                _unitOfWork.Connection.Close();*/
                _unitOfWork.Context.Database.ExecuteSqlRaw(
                    "DELETE FROM dbo.Simulation;" +
                    "DELETE FROM dbo.Equation;" +
                    "DELETE FROM dbo.CriterionLibrary;" +
                    "DELETE FROM dbo.BudgetLibrary;" +
                    "DELETE FROM dbo.BudgetPriorityLibrary;" +
                    "DELETE FROM dbo.CashFlowRuleLibrary;" +
                    "DELETE FROM dbo.DeficientConditionGoalLibrary;" +
                    "DELETE FROM dbo.PerformanceCurveLibrary;" +
                    "DELETE FROM dbo.RemainingLifeLimitLibrary;" +
                    "DELETE FROM dbo.TargetConditionGoalLibrary;" +
                    "DELETE FROM dbo.TreatmentLibrary;");
            }

            _unitOfWork.Context.SaveChanges();
        }
    }
}
