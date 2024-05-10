using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using AppliedResearchAssociates.iAM.Common.Logging;
using System.Threading;
using AppliedResearchAssociates.iAM.Common;
using Microsoft.Data.SqlClient;


namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class SimulationRepository : ISimulationRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;
        public const string NoSimulationWasFoundForTheGivenScenario = "No simulation was found for the given scenario.";

        public SimulationRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ??
                          throw new ArgumentNullException(nameof(unitOfWork));
        }

        public SimulationCloningResultDTO CreateSimulation(CompleteSimulationDTO completeSimulationDTO, string keyAttribute, SimulationCloningCommittedProjectErrors simulationCloningCommittedProjectErrors, BaseEntityProperties baseEntityProperties)
        {
            var attributes = _unitOfWork.Context.Attribute.AsNoTracking().ToList();
            var entity = CompleteSimulationMapper.ToNewEntity(completeSimulationDTO, attributes, keyAttribute, baseEntityProperties);

            foreach (var treatment in entity.SelectableTreatments)
            {
                foreach (var supercedeRule in treatment.ScenarioTreatmentSupersedeRules)
                {
                    if (supercedeRule.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.MergedCriteriaExpression == null)
                    {
                        supercedeRule.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.MergedCriteriaExpression = string.Empty;
                        supercedeRule.CriterionLibraryScenarioTreatmentSupersedeRuleJoin.CriterionLibrary.Name = string.Empty;
                    }
                }
            }

            _unitOfWork.AsTransaction(() =>
            {
                _unitOfWork.Context.AddEntity(entity);
            }); 
            var simulation = _unitOfWork.SimulationRepo.GetSimulation(completeSimulationDTO.Id);
            var warningMessage = simulationCloningCommittedProjectErrors.BudgetsPreventingCloning.Any() && simulationCloningCommittedProjectErrors.NumberOfCommittedProjectsAffected > 0
                    ? $"The following committed project budgets were not found which has prevented {simulationCloningCommittedProjectErrors.NumberOfCommittedProjectsAffected} committed project(s) from being cloned: {string.Join(", ", simulationCloningCommittedProjectErrors.BudgetsPreventingCloning)}"
                    : null;
            var cloningResult = new SimulationCloningResultDTO
            {
                Simulation = simulation,
                WarningMessage = warningMessage,
            };
            return cloningResult;
        }

        public List<SimulationDTO> GetAllScenario()
        {
            if (!_unitOfWork.Context.Simulation.Any())
            {
                return new List<SimulationDTO>();
            }

            var users = _unitOfWork.Context.User.ToList();

            var simulationEntities = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Include(_ => _.SimulationReportDetail)
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Include(_ => _.Network)
                .ToList();

            return simulationEntities.Select(_ => _.ToDto(users.FirstOrDefault(__ => __.Id == _.CreatedBy)))
                .ToList();
        }

        public List<SimulationDTO> GetUserScenarios()
        {
            var users = _unitOfWork.Context.User.ToList();
            var simulations = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Include(_ => _.SimulationReportDetail)
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Include(_ => _.Network)
                .ToList().Select(_ => _.ToDto(users.FirstOrDefault(__ => __.Id == _.CreatedBy)))
                .Where(_ => _.Owner == _unitOfWork.CurrentUser.Username)
                .OrderByDescending(s => s.LastModifiedDate)
                .ToList();

            return simulations;
        }

        public List<SimulationDTO> GetSharedScenarios(bool hasAdminAccess, bool hasSimulationAccess)
        {
            var users = _unitOfWork.Context.User.ToList();
            var simulations = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Include(_ => _.SimulationReportDetail)
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Include(_ => _.Network)
                .ToList().Select(_ => _.ToDto(users.FirstOrDefault(__ => __.Id == _.CreatedBy)))
                .Where(_ => _.Owner != _unitOfWork.CurrentUser.Username &&
                    (hasAdminAccess ||
                    hasSimulationAccess ||
                    _.Users.Any(__ => __.Username == _unitOfWork.CurrentUser.Username))
                 )
                .OrderByDescending(s => s.LastModifiedDate)
                .ToList();
            return simulations;
        }

        public List<SimulationDTO> GetScenariosWithIds(List<Guid> simulationIds)
        {
            var users = _unitOfWork.Context.User.ToList();

            var simulations = new List<SimulationDTO>();

            if (simulationIds?.Any() == true)
            {
                simulations = _unitOfWork.Context.Simulation
                             .Include(_ => _.SimulationAnalysisDetail)
                             .Include(_ => _.SimulationUserJoins)
                             .ThenInclude(_ => _.User)
                             .Include(_ => _.Network)
                             .ToList()
                             .Where(_ => simulationIds.Contains(_.Id))
                             .Select(_ => _.ToDto(users.FirstOrDefault(__ => __.Id == _.CreatedBy)))
                             .ToList();
            }

            return simulations;
        }

        public void GetSimulationInNetwork(Guid simulationId, Network network)
        {
            if (!_unitOfWork.Context.Network.Any(_ => _.Id == network.Id))
            {
                throw new RowNotInTableException("The specified network was not found.");
            }

            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking().Single(_ => _.Id == simulationId);
            var simulationAnalysisDetail = _unitOfWork.Context.SimulationAnalysisDetail.AsNoTracking()
                .SingleOrDefault(_ => _.SimulationId == simulationId);

            var defaultDate = new DateTime(1900, 1, 1);
            var lastRun = simulationAnalysisDetail?.LastRun ?? defaultDate;
            var lastModifiedDate = simulationAnalysisDetail?.LastModifiedDate ?? defaultDate;
            simulationEntity.CreateSimulation(network, lastRun, lastModifiedDate);
        }

        public void CreateSimulation(Guid networkId, SimulationDTO dto)
        {
            _unitOfWork.AsTransaction(() =>
            {
                if (!_unitOfWork.Context.Network.Any(_ => _.Id == networkId))
                {
                    throw new RowNotInTableException($"No network found having id {networkId}");
                }

                var defaultLibrary = _unitOfWork.Context.CalculatedAttributeLibrary.Where(_ => _.IsDefault == true)
                    .Include(_ => _.CalculatedAttributes)
                    .ThenInclude(_ => _.Attribute)
                    .Include(_ => _.CalculatedAttributes)
                    .ThenInclude(_ => _.Equations)
                    .ThenInclude(_ => _.CriterionLibraryCalculatedAttributeJoin)
                    .ThenInclude(_ => _.CriterionLibrary)
                    .Include(_ => _.CalculatedAttributes)
                    .ThenInclude(_ => _.Equations)
                    .ThenInclude(_ => _.EquationCalculatedAttributeJoin)
                    .ThenInclude(_ => _.Equation)
                    .Select(_ => _.ToDto())
                    .ToList();

                if (defaultLibrary.Count == 0)
                {
                    throw new RowNotInTableException($"No default library for Calculated Attributes has been found. Please contact admin");
                }

                var simulationEntity = dto.ToEntity(networkId);
                // if there are multiple default libraries (This should not happen). Take the first one

                _unitOfWork.Context.AddEntity(simulationEntity, _unitOfWork.UserEntity?.Id);
                if (dto.Users.Any())
                {
                    var usersToAdd = dto.Users.Select(_ => _.ToEntity(dto.Id)).ToList();
                    _unitOfWork.Context.AddAll(usersToAdd,
                        _unitOfWork.UserEntity?.Id);
                }
                ICalculatedAttributesRepository _calculatedAttributesRepo = _unitOfWork.CalculatedAttributeRepo;
                // Assiging new Ids because this object will be assiged to a simulation
                defaultLibrary[0].CalculatedAttributes.ForEach(_ =>
                {
                    _.Id = Guid.NewGuid();
                    _.Equations.ForEach(e =>
                    {
                        e.Id = Guid.NewGuid();
                        if (e.CriteriaLibrary != null)
                        {
                            e.CriteriaLibrary.Id = Guid.NewGuid();
                            e.CriteriaLibrary.IsSingleUse = true;
                        }
                        e.Equation.Id = Guid.NewGuid();
                    });
                });
                _calculatedAttributesRepo.UpsertScenarioCalculatedAttributesNonAtomic(defaultLibrary[0].CalculatedAttributes, simulationEntity.Id);
            });
        }

        public SimulationDTO GetSimulation(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException(NoSimulationWasFoundForTheGivenScenario);
            }

            var users = _unitOfWork.Context.User.ToList();

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Include(_ => _.Network)
                .Single(_ => _.Id == simulationId);

            var creatingUser = users.FirstOrDefault(_ => _.Id == simulationEntity.CreatedBy);
            return simulationEntity.ToDto(creatingUser);
        }

        public string GetSimulationName(Guid simulationId)
        {
            var selectedSimulation = _unitOfWork.Context.Simulation.AsNoTracking().FirstOrDefault(_ => _.Id == simulationId);
            // We either need to return null here or an error.  An empty string is possible for an existing simulation.
            return (selectedSimulation == null) ? null : selectedSimulation.Name;
        }

        public void UpdateSimulationAndPossiblyUsers(SimulationDTO dto)
        {
            _unitOfWork.AsTransaction(() =>
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == dto.Id))
                {
                    throw new RowNotInTableException("No simulation was found for the given scenario.");
                }

                var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == dto.Id);
                if (simulationEntity.Name != dto.Name || simulationEntity.NoTreatmentBeforeCommittedProjects != dto.NoTreatmentBeforeCommittedProjects)
                {
                    simulationEntity.Name = dto.Name;
                    simulationEntity.NoTreatmentBeforeCommittedProjects = dto.NoTreatmentBeforeCommittedProjects;

                    _unitOfWork.Context.UpdateEntity(simulationEntity, dto.Id, _unitOfWork.UserEntity?.Id);
                }

                if (dto.Users.Any())
                {
                    _unitOfWork.Context.DeleteAll<SimulationUserEntity>(_ => _.SimulationId == dto.Id);
                    _unitOfWork.Context.AddAll(dto.Users.Select(_ => _.ToEntity(dto.Id)).ToList(),
                        _unitOfWork.UserEntity?.Id);
                }
            });
        }

        public void DeleteSimulation(Guid simulationId, CancellationToken? cancellationToken = null, IWorkQueueLog queueLog = null)
        {

            // Create parameters for the stored procedure
            var retMessageParam = new SqlParameter("@RetMessage", SqlDbType.VarChar, 250);
            string retMessage = "";

            queueLog ??= new DoNothingWorkQueueLog();

            if (_unitOfWork.Context.Database.CurrentTransaction != null)
            {
                throw new InvalidOperationException(UnitOfDataPersistenceWorkExtensions.CannotStartTransactionWhileAnotherTransactionIsInProgress);
            }
            try
            {
                _unitOfWork.BeginTransaction();
                if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
                {
                    _unitOfWork.Rollback();
                    return;
                }

                queueLog.UpdateWorkQueueStatus("Deleting Simulation");

                // Create parameters for the stored procedure
                var simGuidListParam = new SqlParameter("@SimGuidList", simulationId.ToString());
                retMessageParam.Direction = ParameterDirection.Output;

                // Execute the stored procedure
                _unitOfWork.Context.Database.ExecuteSqlRaw("EXEC usp_delete_simulation @SimGuidList, @RetMessage OUTPUT", simGuidListParam, retMessageParam);

                // Capture the success output value
                retMessage = retMessageParam.Value as string;

                _unitOfWork.Commit();

            }
            catch
            {
                // Capture the fail output value
                retMessage = retMessageParam.Value as string;
               _unitOfWork.Rollback();
                throw;
            }
        }

        public void DeleteSimulationsByNetworkId(Guid networkId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.NetworkId == networkId))
            {
                return;
            }

            _unitOfWork.Context.Database.SetCommandTimeout(TimeSpan.FromSeconds(3600));

            var ids = _unitOfWork.Context.Simulation.Where(_ => _.NetworkId == networkId).Select(_ => _.Id);

            string tmpguidstr = string.Join(",", ids.Select(g => g.ToString()));

            // RegEx Explained: \s means "match any whitespace token", and + means "match one or more of the proceeding token
            tmpguidstr = System.Text.RegularExpressions.Regex.Replace(tmpguidstr, @"\s+", string.Empty);

            // Create parameters for the stored procedure
            var retMessageParam = new SqlParameter("@RetMessage", SqlDbType.VarChar, 250);
            string retMessage = "";
            var simGuidListParam = new SqlParameter("@SimGuidList", tmpguidstr);
            retMessageParam.Direction = ParameterDirection.Output;

            // Execute the stored procedure
            _unitOfWork.Context.Database.ExecuteSqlRaw("EXEC usp_delete_simulation @SimGuidList, @RetMessage OUTPUT", simGuidListParam, retMessageParam);

            // Capture the success output value
            retMessage = retMessageParam.Value as string;

        }

        // the method is used only by other repositories.
        public void UpdateLastModifiedDate(SimulationEntity entity)
        {
            entity.LastModifiedDate = DateTime.Now; // updating the last modified date
            _unitOfWork.Context.Upsert(entity, entity.Id, _unitOfWork.UserEntity?.Id);
        }

        public SimulationDTO GetCurrentUserOrSharedScenario(Guid simulationId, bool hasAdminAccess, bool hasSimulationAccess)
        {
            var users = _unitOfWork.Context.User.ToList();
            var simulation = _unitOfWork.Context.Simulation
                .Include(_ => _.SimulationAnalysisDetail)
                .Include(_ => _.SimulationUserJoins)
                .ThenInclude(_ => _.User)
                .Include(_ => _.Network)
                .ToList().Select(_ => _.ToDto(users.FirstOrDefault(__ => __.Id == _.CreatedBy)))
                .FirstOrDefault(_ => _.Id == simulationId && (_.Owner == _unitOfWork.CurrentUser.Username ||
                    (_.Owner != _unitOfWork.CurrentUser.Username && hasAdminAccess || hasSimulationAccess ||
                    _.Users.Any(__ => __.Username == _unitOfWork.CurrentUser.Username))));

            return simulation;
        }

        public bool GetNoTreatmentBeforeCommitted(Guid simulationId)
        {
            return GetSimulation(simulationId).NoTreatmentBeforeCommittedProjects;
        }

        public void SetNoTreatmentBeforeCommitted(Guid simulationId)
        {
            UpdateSimulationNoTreatmentBeforeCommitted(simulationId, true);
        }

        public void RemoveNoTreatmentBeforeCommitted(Guid simulationId)
        {
            UpdateSimulationNoTreatmentBeforeCommitted(simulationId, false);
        }

        private void UpdateSimulationNoTreatmentBeforeCommitted(Guid simulationId, bool noTreatmentBeforeCommitted)
        {
            var simulationDto = GetSimulation(simulationId);
            simulationDto.NoTreatmentBeforeCommittedProjects = noTreatmentBeforeCommitted;
            UpdateSimulationAndPossiblyUsers(simulationDto);
        }
    }
}
