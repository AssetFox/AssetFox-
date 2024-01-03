using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CashFlowRuleRepository : ICashFlowRuleRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CashFlowRuleRepository(UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public DateTime GetLibraryModifiedDate(Guid cashLibraryId)
        {
            var dtos = _unitOfWork.Context.CashFlowRuleLibrary.Where(_ => _.Id == cashLibraryId).FirstOrDefault().LastModifiedDate;
            return dtos;
        }


        public void CreateCashFlowRules(List<CashFlowRule> cashFlowRules, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking()
                .Single(_ => _.Id == simulationId);

            var cashFlowRuleEntities = cashFlowRules
                .Select(_ => _.ToScenarioEntity(simulationId))
                .ToList();

            _unitOfWork.Context.AddAll(cashFlowRuleEntities, _unitOfWork.UserEntity?.Id);

            if (cashFlowRules.Any(_ => _.DistributionRules.Any()))
            {
                var distributionRules = cashFlowRules.Where(_ => _.DistributionRules.Any())
                    .SelectMany(cashFlowRule => cashFlowRule.DistributionRules
                        .Select((distributionRule, index) => distributionRule.ToScenarioEntity(cashFlowRule.Id, ++index)))
                    .ToList();

                _unitOfWork.Context.AddAll(distributionRules);
            }

            if (cashFlowRules.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var criterionJoins = new List<CriterionLibraryScenarioCashFlowRuleEntity>();

                var criteria = cashFlowRules.Where(curve => !curve.Criterion.ExpressionIsBlank)
                    .Select(cashFlowRule =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = cashFlowRule.Criterion.Expression,
                            Name = $"{cashFlowRule.Name} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryScenarioCashFlowRuleEntity
                        {
                            CriterionLibraryId = criterion.Id,
                            ScenarioCashFlowRuleId = cashFlowRule.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public List<CashFlowRuleLibraryDTO> GetCashFlowRuleLibraries()
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any())
            {
                return new List<CashFlowRuleLibraryDTO>();
            }

            return _unitOfWork.Context.CashFlowRuleLibrary
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CashFlowDistributionRules)
                .AsNoTracking()
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<CashFlowRuleLibraryDTO> GetCashFlowRuleLibrariesNoChildren()
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any())
            {
                return new List<CashFlowRuleLibraryDTO>();
            }

            return _unitOfWork.Context.CashFlowRuleLibrary.AsNoTracking()
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertCashFlowRuleLibrary(CashFlowRuleLibraryDTO dto)
        {
            var libraryExists = _unitOfWork.Context.CashFlowRuleLibrary.Any(bl => bl.Id == dto.Id);
            _unitOfWork.Context.Upsert(dto.ToEntity(), dto.Id, _unitOfWork.UserEntity?.Id);
            _unitOfWork.Context.SaveChanges();
        }

        public void UpsertCashFlowRuleLibraryAndRules(CashFlowRuleLibraryDTO dto)
        {
            _unitOfWork.AsTransaction(() =>
            {
                UpsertCashFlowRuleLibrary(dto);
                UpsertOrDeleteCashFlowRules(dto.CashFlowRules, dto.Id);
            });
        }

        public void UpsertOrDeleteCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid libraryId)
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified cash flow rule library was not found.");
            }

            var cashFlowRuleEntities = cashFlowRules
                .Select(_ => _.ToLibraryEntity(libraryId))
                .ToList();

            var entityIds = cashFlowRuleEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.CashFlowRule
                .Where(_ => _.CashFlowRuleLibrary.Id == libraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<CriterionLibraryCashFlowRuleEntity>(_ =>
                _.CashFlowRule.CashFlowRuleLibraryId == libraryId);

            _unitOfWork.Context.DeleteAll<CashFlowRuleEntity>(_ =>
                _.CashFlowRuleLibrary.Id == libraryId && !entityIds.Contains(_.Id));

            _unitOfWork.Context.UpdateAll(cashFlowRuleEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(),
                _unitOfWork.UserEntity?.Id);

            _unitOfWork.Context.AddAll(cashFlowRuleEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(),
                _unitOfWork.UserEntity?.Id);

            if (cashFlowRules.Any(_ => _.CashFlowDistributionRules.Any()))
            {
                var distributionRulesPerCashFlowRuleId = cashFlowRules.Where(_ => _.CashFlowDistributionRules.Any())
                    .ToDictionary(_ => _.Id, _ => _.CashFlowDistributionRules);
                _unitOfWork.CashFlowDistributionRuleRepo.UpsertOrDeleteCashFlowDistributionRules(
                    distributionRulesPerCashFlowRuleId, libraryId);
            }

            if (cashFlowRules.Any(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                       !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
            {
                var criterionJoins = new List<CriterionLibraryCashFlowRuleEntity>();

                var criteria = cashFlowRules
                    .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                    .Select(cashFlowRule =>
                    {
                        var criterion = new CriterionLibraryEntity
                        {
                            Id = Guid.NewGuid(),
                            MergedCriteriaExpression = cashFlowRule.CriterionLibrary.MergedCriteriaExpression,
                            Name = $"{cashFlowRule.Name} Criterion",
                            IsSingleUse = true
                        };
                        criterionJoins.Add(new CriterionLibraryCashFlowRuleEntity
                        {
                            CriterionLibraryId = criterion.Id,
                            CashFlowRuleId = cashFlowRule.Id
                        });
                        return criterion;
                    }).ToList();

                _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
            }
        }

        public void DeleteCashFlowRuleLibrary(Guid libraryId)
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == libraryId))
            {
                return;
            }

            _unitOfWork.Context.DeleteEntity<CashFlowRuleLibraryEntity>(_ => _.Id == libraryId);
        }

        public List<CashFlowRuleDTO> GetScenarioCashFlowRules(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            return _unitOfWork.Context.ScenarioCashFlowRule.AsNoTracking()
                .Include(_ => _.ScenarioCashFlowDistributionRules)
                .Include(_ => _.CriterionLibraryScenarioCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.SimulationId == simulationId)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public List<CashFlowRuleDTO> GetCashFlowRulesByLibraryId(Guid libraryId)
        {
            if (!_unitOfWork.Context.CashFlowRuleLibrary.Any(_ => _.Id == libraryId))
            {
                throw new RowNotInTableException("The specified cash flow library was not found.");
            }

            return _unitOfWork.Context.CashFlowRule.AsNoTracking()
                .Include(_ => _.CashFlowDistributionRules)
                .Include(_ => _.CriterionLibraryCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Where(_ => _.CashFlowRuleLibraryId == libraryId)
                .Select(_ => _.ToDto())
                .ToList();
        }

        public void UpsertOrDeleteScenarioCashFlowRules(List<CashFlowRuleDTO> cashFlowRules, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            _unitOfWork.AsTransaction(() =>
            {
                var simulationEntity = _unitOfWork.Context.Simulation.AsNoTracking()
                    .Single(_ => _.Id == simulationId);

                var cashFlowRuleEntities = cashFlowRules
                    .Select(_ => _.ToScenarioEntity(simulationId))
                    .ToList();

                var entityIds = cashFlowRuleEntities.Select(_ => _.Id).ToList();

                var existingEntityIds = _unitOfWork.Context.ScenarioCashFlowRule.AsNoTracking()
                    .Where(_ => _.SimulationId == simulationId && entityIds.Contains(_.Id))
                    .Select(_ => _.Id).ToList();

                _unitOfWork.Context.DeleteAll<CriterionLibraryScenarioCashFlowRuleEntity>(_ =>
                    _.ScenarioCashFlowRule.SimulationId == simulationId);

                _unitOfWork.Context.DeleteAll<ScenarioCashFlowRuleEntity>(_ =>
                    _.SimulationId == simulationId && !entityIds.Contains(_.Id));

                _unitOfWork.Context.UpdateAll(cashFlowRuleEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList(),
                    _unitOfWork.UserEntity?.Id);

                _unitOfWork.Context.AddAll(cashFlowRuleEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList(),
                    _unitOfWork.UserEntity?.Id);

                if (cashFlowRules.Any(_ => _.CashFlowDistributionRules.Any()))
                {
                    var distributionRulesPerCashFlowRuleId = cashFlowRules.Where(_ => _.CashFlowDistributionRules.Any())
                        .ToDictionary(_ => _.Id, _ => _.CashFlowDistributionRules);
                    _unitOfWork.CashFlowDistributionRuleRepo.UpsertOrDeleteScenarioCashFlowDistributionRules(
                        distributionRulesPerCashFlowRuleId, simulationId);
                }

                if (cashFlowRules.Any(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                           !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression)))
                {
                    var criterionJoins = new List<CriterionLibraryScenarioCashFlowRuleEntity>();

                    var criteria = cashFlowRules
                        .Where(_ => _.CriterionLibrary?.Id != null && _.CriterionLibrary?.Id != Guid.Empty &&
                                    !string.IsNullOrEmpty(_.CriterionLibrary.MergedCriteriaExpression))
                        .Select(cashFlowRule =>
                        {
                            var criterion = new CriterionLibraryEntity
                            {
                                Id = Guid.NewGuid(),
                                MergedCriteriaExpression = cashFlowRule.CriterionLibrary.MergedCriteriaExpression,
                                Name = $"{cashFlowRule.Name} Criterion",
                                IsSingleUse = true
                            };
                            criterionJoins.Add(new CriterionLibraryScenarioCashFlowRuleEntity
                            {
                                CriterionLibraryId = criterion.Id,
                                ScenarioCashFlowRuleId = cashFlowRule.Id
                            });
                            return criterion;
                        }).ToList();

                    _unitOfWork.Context.AddAll(criteria, _unitOfWork.UserEntity?.Id);
                    _unitOfWork.Context.AddAll(criterionJoins, _unitOfWork.UserEntity?.Id);
                }

                // Update last modified date
                _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
            });
        }

        public List<CashFlowRuleLibraryDTO> GetCashFlowRuleLibrariesNoChildrenAccessibleToUser(Guid userId)
        {
            return _unitOfWork.Context.CashFlowRuleLibraryUser
                .AsNoTracking()
                .Include(u => u.CashFlowRuleLibrary)
                .Where(u => u.UserId == userId)
                .Select(u => u.CashFlowRuleLibrary.ToDto())
                .ToList();
        }
        public void UpsertOrDeleteUsers(Guid cashFlowRuleLibraryId, IList<LibraryUserDTO> libraryUsers)
        {
            var existingEntities = _unitOfWork.Context.CashFlowRuleLibraryUser.Where(u => u.LibraryId == cashFlowRuleLibraryId).ToList();
            var existingUserIds = existingEntities.Select(u => u.UserId).ToList();
            var desiredUserIDs = libraryUsers.Select(lu => lu.UserId).ToList();
            var userIdsToDelete = existingUserIds.Except(desiredUserIDs).ToList();
            var userIdsToUpdate = existingUserIds.Intersect(desiredUserIDs).ToList();
            var userIdsToAdd = desiredUserIDs.Except(existingUserIds).ToList();
            var entitiesToAdd = libraryUsers.Where(u => userIdsToAdd.Contains(u.UserId)).Select(u => LibraryUserMapper.ToCashFlowRuleLibraryUserEntity(u, cashFlowRuleLibraryId)).ToList();
            var dtosToUpdate = libraryUsers.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToMaybeUpdate = existingEntities.Where(u => userIdsToUpdate.Contains(u.UserId)).ToList();
            var entitiesToUpdate = new List<CashFlowRuleLibraryUserEntity>();
            foreach (var dto in dtosToUpdate)
            {
                var entityToUpdate = entitiesToMaybeUpdate.FirstOrDefault(e => e.UserId == dto.UserId);
                if (entityToUpdate != null && entityToUpdate.AccessLevel != (int)dto.AccessLevel)
                {
                    entityToUpdate.AccessLevel = (int)dto.AccessLevel;
                    entitiesToUpdate.Add(entityToUpdate);
                }
            }
            _unitOfWork.Context.AddRange(entitiesToAdd);
            _unitOfWork.Context.UpdateRange(entitiesToUpdate);
            var entitiesToDelete = existingEntities.Where(u => userIdsToDelete.Contains(u.UserId)).ToList();
            _unitOfWork.Context.RemoveRange(entitiesToDelete);
            _unitOfWork.Context.SaveChanges();
        }

        private List<LibraryUserDTO> GetAccessForUser(Guid cashFlowRuleLibraryId, Guid userId)
        {
            var dtos = _unitOfWork.Context.CashFlowRuleLibraryUser
                .Where(u => u.LibraryId == cashFlowRuleLibraryId && u.UserId == userId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }

        public List<LibraryUserDTO> GetLibraryUsers(Guid cashFlowRuleLibraryId)
        {
            var dtos = _unitOfWork.Context.CashFlowRuleLibraryUser
                .Include(u => u.User)
                .Where(u => u.LibraryId == cashFlowRuleLibraryId)
                .Select(LibraryUserMapper.ToDto)
                .ToList();
            return dtos;
        }
        public LibraryUserAccessModel GetLibraryAccess(Guid libraryId, Guid userId)
        {
            var exists = _unitOfWork.Context.CashFlowRuleLibrary.Any(bl => bl.Id == libraryId);
            if (!exists)
            {
                return LibraryAccessModels.LibraryDoesNotExist();
            }
            var users = GetAccessForUser(libraryId, userId);
            var user = users.FirstOrDefault();
            return LibraryAccessModels.LibraryExistsWithUsers(userId, user);
        }
    }
}
