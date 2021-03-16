using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationRepository : ISimulationRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public SimulationRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork) =>
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ??
                                         throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));

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

        public Task<List<SimulationDTO>> GetAllInNetwork(Guid networkId)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            if (!_unitOfDataPersistenceWork.Context.Simulation.Any())
            {
                return Task.Factory.StartNew(() => new List<SimulationDTO>());
            }

            return Task.Factory.StartNew(() =>
            {
                var users = _unitOfDataPersistenceWork.Context.User.ToList();

                var simulationEntities = _unitOfDataPersistenceWork.Context.Simulation
                    .Include(_ => _.SimulationAnalysisDetail)
                    .Include(_ => _.SimulationReportDetail)
                    .Include(_ => _.SimulationUserJoins)
                    .ThenInclude(_ => _.User)
                    .Where(_ => _.NetworkId == networkId)
                    .ToList();

                return simulationEntities.Select(_ => _.ToDto(users.FirstOrDefault(__ => __.Id == _.CreatedBy)))
                    .ToList();
            });
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

        public void CreateSimulation(Guid networkId, SimulationDTO dto, UserInfoDTO userInfo)
        {
            if (!_unitOfDataPersistenceWork.Context.Network.Any(_ => _.Id == networkId))
            {
                throw new RowNotInTableException($"No network found having id {networkId}");
            }

            var user = _unitOfDataPersistenceWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var simulationEntity = dto.ToEntity(networkId);

            _unitOfDataPersistenceWork.Context.AddEntity(simulationEntity, user?.Id);

            if (dto.Users.Any())
            {
                _unitOfDataPersistenceWork.Context.BulkAddAll(dto.Users.Select(_ => _.ToEntity(dto.Id)).ToList(),
                    user?.Id);
            }
        }

        public Task<SimulationDTO> GetSimulation(Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            return Task.Factory.StartNew(() =>
            {
                var users = _unitOfDataPersistenceWork.Context.User.ToList();

                var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
                    .Include(_ => _.SimulationAnalysisDetail)
                    .Include(_ => _.SimulationUserJoins)
                    .ThenInclude(_ => _.User)
                    .Single(_ => _.Id == simulationId);

                return simulationEntity.ToDto(users.FirstOrDefault(_ => _.Id == simulationEntity.CreatedBy));
            });
        }

        public Task<SimulationDTO> CloneSimulation(Guid simulationId, UserInfoDTO userInfo)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            return Task.Factory.StartNew(() =>
            {
                var userEntity = _unitOfDataPersistenceWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

                var simulationToClone = _unitOfDataPersistenceWork.Context.Simulation.AsNoTracking()
                    .Include(_ => _.AnalysisMethod)
                    .ThenInclude(_ => _.Benefit)
                    .Include(_ => _.AnalysisMethod)
                    .ThenInclude(_ => _.CriterionLibraryAnalysisMethodJoin)
                    .Include(_ => _.InvestmentPlan)
                    .Include(_ => _.BudgetLibrarySimulationJoin)
                    .Include(_ => _.BudgetPriorityLibrarySimulationJoin)
                    .Include(_ => _.CashFlowRuleLibrarySimulationJoin)
                    .Include(_ => _.DeficientConditionGoalLibrarySimulationJoin)
                    .Include(_ => _.PerformanceCurveLibrarySimulationJoin)
                    .Include(_ => _.RemainingLifeLimitLibrarySimulationJoin)
                    .Include(_ => _.TargetConditionGoalLibrarySimulationJoin)
                    .Include(_ => _.TreatmentLibrarySimulationJoin)
                    .Include(_ => _.CommittedProjects)
                    .ThenInclude(_ => _.CommittedProjectConsequences)
                    .Single(_ => _.Id == simulationId);

                var newSimulationId = Guid.NewGuid();

                simulationToClone.Id = newSimulationId;
                _unitOfDataPersistenceWork.Context
                    .ReInitializeAllEntityBaseProperties(simulationToClone, userEntity?.Id);

                if (simulationToClone.AnalysisMethod != null)
                {
                    var newAnalysisMethodId = Guid.NewGuid();
                    simulationToClone.AnalysisMethod.Id = newAnalysisMethodId;
                    simulationToClone.AnalysisMethod.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod, userEntity?.Id);

                    if (simulationToClone.AnalysisMethod.Benefit != null)
                    {
                        simulationToClone.AnalysisMethod.Benefit.Id = Guid.NewGuid();
                        simulationToClone.AnalysisMethod.Benefit.AnalysisMethodId = newAnalysisMethodId;
                        _unitOfDataPersistenceWork.Context
                            .ReInitializeAllEntityBaseProperties(simulationToClone.AnalysisMethod.Benefit,
                                userEntity?.Id);
                    }

                    if (simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin != null)
                    {
                        simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin.AnalysisMethodId =
                            newAnalysisMethodId;
                        _unitOfDataPersistenceWork.Context
                            .ReInitializeAllEntityBaseProperties(
                                simulationToClone.AnalysisMethod.CriterionLibraryAnalysisMethodJoin, userEntity?.Id);
                    }
                }

                if (simulationToClone.InvestmentPlan != null)
                {
                    simulationToClone.InvestmentPlan.Id = Guid.NewGuid();
                    simulationToClone.InvestmentPlan.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.InvestmentPlan, userEntity?.Id);
                }

                if (simulationToClone.BudgetLibrarySimulationJoin != null)
                {
                    simulationToClone.BudgetLibrarySimulationJoin.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.BudgetLibrarySimulationJoin,
                            userEntity?.Id);
                }

                if (simulationToClone.BudgetPriorityLibrarySimulationJoin != null)
                {
                    simulationToClone.BudgetPriorityLibrarySimulationJoin.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.BudgetPriorityLibrarySimulationJoin,
                            userEntity?.Id);
                }

                if (simulationToClone.CashFlowRuleLibrarySimulationJoin != null)
                {
                    simulationToClone.CashFlowRuleLibrarySimulationJoin.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.CashFlowRuleLibrarySimulationJoin,
                            userEntity?.Id);
                }

                if (simulationToClone.DeficientConditionGoalLibrarySimulationJoin != null)
                {
                    simulationToClone.DeficientConditionGoalLibrarySimulationJoin.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(
                            simulationToClone.DeficientConditionGoalLibrarySimulationJoin, userEntity?.Id);
                }

                if (simulationToClone.PerformanceCurveLibrarySimulationJoin != null)
                {
                    simulationToClone.PerformanceCurveLibrarySimulationJoin.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.PerformanceCurveLibrarySimulationJoin,
                            userEntity?.Id);
                }

                if (simulationToClone.RemainingLifeLimitLibrarySimulationJoin != null)
                {
                    simulationToClone.RemainingLifeLimitLibrarySimulationJoin.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.RemainingLifeLimitLibrarySimulationJoin,
                            userEntity?.Id);
                }

                if (simulationToClone.TargetConditionGoalLibrarySimulationJoin != null)
                {
                    simulationToClone.TargetConditionGoalLibrarySimulationJoin.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.TargetConditionGoalLibrarySimulationJoin,
                            userEntity?.Id);
                }

                if (simulationToClone.TreatmentLibrarySimulationJoin != null)
                {
                    simulationToClone.TreatmentLibrarySimulationJoin.SimulationId = newSimulationId;
                    _unitOfDataPersistenceWork.Context
                        .ReInitializeAllEntityBaseProperties(simulationToClone.TreatmentLibrarySimulationJoin,
                            userEntity?.Id);
                }

                if (simulationToClone.CommittedProjects.Any())
                {
                    simulationToClone.CommittedProjects.ForEach(committedProject =>
                    {
                        committedProject.Id = Guid.NewGuid();
                        committedProject.SimulationId = newSimulationId;
                        _unitOfDataPersistenceWork.Context
                            .ReInitializeAllEntityBaseProperties(committedProject, userEntity?.Id);

                        if (committedProject.CommittedProjectConsequences.Any())
                        {
                            committedProject.CommittedProjectConsequences.ForEach(committedProjectConsequence =>
                            {
                                committedProjectConsequence.Id = Guid.NewGuid();
                                committedProjectConsequence.CommittedProjectId = committedProject.Id;
                                _unitOfDataPersistenceWork.Context
                                    .ReInitializeAllEntityBaseProperties(committedProjectConsequence, userEntity?.Id);
                            });
                        }
                    });
                }

                if (userEntity != null)
                {
                    simulationToClone.SimulationUserJoins = new List<SimulationUserEntity>{new SimulationUserEntity
                    {
                        SimulationId = newSimulationId,
                        UserId = userEntity.Id,
                        CanModify = true,
                        IsOwner = true,
                        CreatedBy = userEntity.Id,
                        LastModifiedBy = userEntity.Id
                    }};
                }

                _unitOfDataPersistenceWork.Context.AddEntity(simulationToClone);

                return simulationToClone.ToDto(userEntity);
            });
        }

        public void UpdatePermittedSimulation(UserInfoDTO userInfo, SimulationDTO dto)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == dto.Id))
            {
                throw new RowNotInTableException($"No simulation found having id {dto.Id}");
            }

            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ =>
                _.Id == dto.Id && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub && __.CanModify)))
            {
                throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
            }

            UpdateSimulation(dto, userInfo);
        }

        public void UpdateSimulation(SimulationDTO dto, UserInfoDTO userInfo)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == dto.Id))
            {
                throw new RowNotInTableException($"No simulation found having id {dto.Id}");
            }

            var user = _unitOfDataPersistenceWork.Context.User.SingleOrDefault(_ => _.Username == userInfo.Sub);

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation.Single(_ => _.Id == dto.Id);
            if (simulationEntity.Name != dto.Name)
            {
                simulationEntity.Name = dto.Name;

                _unitOfDataPersistenceWork.Context.Update(simulationEntity, dto.Id, user?.Id);
            }

            if (dto.Users.Any())
            {
                _unitOfDataPersistenceWork.Context.DeleteAll<SimulationUserEntity>(_ => _.SimulationId == dto.Id);
                _unitOfDataPersistenceWork.Context.BulkAddAll(dto.Users.Select(_ => _.ToEntity(dto.Id)).ToList(), user?.Id);
            }
        }

        public void DeletePermittedSimulation(UserInfoDTO userInfo, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ =>
                _.Id == simulationId && _.SimulationUserJoins.Any(__ => __.User.Username == userInfo.Sub && __.CanModify)))
            {
                throw new UnauthorizedAccessException("You are not authorized to modify the simulation.");
            }

            DeleteSimulation(simulationId);
        }

        public void DeleteSimulation(Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                return;
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation.Single(_ => _.Id == simulationId);

            _unitOfDataPersistenceWork.Context.Simulation.Remove(simulationEntity);

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }
    }
}
