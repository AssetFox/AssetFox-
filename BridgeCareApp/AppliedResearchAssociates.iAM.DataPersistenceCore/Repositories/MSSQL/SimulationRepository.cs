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

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public SimulationRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateSimulation(Simulation simulation)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == simulation.Network.Id))
            {
                throw new RowNotInTableException($"No network found having id {simulation.Network.Id}");
            }

            _unitOfDataPersistenceWork.Context.Simulation.Add(simulation.ToEntity());
            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void GetAllInNetwork(Network network)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id {network.Id}");
            }

            var entities = _unitOfDataPersistenceWork.Context.Simulation.Where(_ => _.NetworkId == network.Id).ToList();

            entities.ForEach(_ => _.CreateSimulation(network));
        }

        public List<SimulationDTO> GetAllInNetwork(Guid networkId)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == networkId))
            {
                //throw new RowNotInTableException($"No network found having id {networkId}");
                return null;
            }

            var entities = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Where(_ => _.NetworkId == networkId);

            return entities.Select(_ => _.ToDto()).ToList();
        }

        public void GetSimulationInNetwork(Guid simulationId, Network network)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException($"No network found having id {network.Id}");
            }

            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation.Single(_ => _.Id == simulationId);

            simulationEntity.CreateSimulation(network);
        }

        public SimulationDTO GetSimulation(Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationDTO = _unitOfDataPersistenceWork.Context.Simulation
                .Include(_ => _.SimulationOutput)
                .Single(_ => _.Id == simulationId)
                .ToDto();

            var simulationAnalysisDetail = _unitOfDataPersistenceWork.SimulationAnalysisDetailRepo.GetSimulationAnalysisDetail(simulationId);

            simulationDTO.LastRun = simulationAnalysisDetail.LastRun;
            simulationDTO.Status = simulationAnalysisDetail.Status;
            simulationDTO.RunTime = simulationAnalysisDetail.RunTime;

            return simulationDTO;
        }

        public void DeleteSimulationAndAllRelatedData()
        {
            if (IsRunningFromXUnit)
            {
                _unitOfDataPersistenceWork.Context.Simulation.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.Equation.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.CriterionLibrary.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.BudgetLibrary.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.BudgetPriorityLibrary.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.CashFlowRuleLibrary.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.DeficientConditionGoalLibrary.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.PerformanceCurveLibrary.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.RemainingLifeLimitLibrary.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.TargetConditionGoalLibrary.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);

                _unitOfDataPersistenceWork.Context.TreatmentLibrary.ToList()
                    .ForEach(_ => _unitOfDataPersistenceWork.Context.Entry(_).State = EntityState.Deleted);
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
                _unitOfDataPersistenceWork.Context.Database.ExecuteSqlRaw(
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

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
