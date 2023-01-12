using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using MoreLinq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CommittedProjectRepository : ICommittedProjectRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CommittedProjectRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void GetSimulationCommittedProjects(Simulation simulation)
        {
            var simulationEntity = _unitOfWork.Context.Simulation.FirstOrDefault(_ => _.Id == simulation.Id);
            if (simulationEntity == null)
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }
            var noTreatment = simulationEntity.NoTreatmentBeforeCommittedProjects;
            ScenarioSelectableTreatmentEntity noTreatmentEntity = null;
            if (noTreatment)
            {
                var selectableTreatmentRepository = _unitOfWork.SelectableTreatmentRepo;
                noTreatmentEntity = selectableTreatmentRepository.GetDefaultTreatment(simulation.Id);
            }
            var assets = _unitOfWork.Context.MaintainableAsset
                .Where(_ => _.NetworkId == simulation.Network.Id)
                .Include(_ => _.MaintainableAssetLocation)
                .ToList();

            var projects = _unitOfWork.Context.CommittedProject
                .Include(_ => _.CommittedProjectLocation)
                .Include(_ => _.ScenarioBudget)
                .Include(_ => _.CommittedProjectConsequences)
                .ThenInclude(_ => _.Attribute)
                .Where(_ => _.SimulationId == simulation.Id).ToList();
            foreach (var project in projects)
            {
                new CommittedProjectEntity
                {
                    Id = project.Id,
                    Name = project.Name,
                    ShadowForAnyTreatment = project.ShadowForAnyTreatment,
                    ShadowForSameTreatment = project.ShadowForSameTreatment,
                    Cost = project.Cost,
                    Year = project.Year,
                    CommittedProjectLocation = project.CommittedProjectLocation,
                    ScenarioBudget = project.ScenarioBudget != null ? new ScenarioBudgetEntity { Name = project.ScenarioBudget.Name } : null,
                    LastModifiedDate = project.LastModifiedDate,
                    CommittedProjectConsequences = project.CommittedProjectConsequences.Select(consequence =>
                        new CommittedProjectConsequenceEntity
                        {
                            Id = consequence.Id,
                            ChangeValue = consequence.ChangeValue,
                            Attribute = new AttributeEntity { Name = consequence.Attribute.Name }
                        }).ToList(),
                };
            }
                //.Select(project => new CommittedProjectEntity
                //{
                //    Id = project.Id,
                //    Name = project.Name,
                //    ShadowForAnyTreatment = project.ShadowForAnyTreatment,
                //    ShadowForSameTreatment = project.ShadowForSameTreatment,
                //    Cost = project.Cost,
                //    Year = project.Year,
                //    CommittedProjectLocation = project.CommittedProjectLocation,
                //    ScenarioBudget = new ScenarioBudgetEntity {Name = project.ScenarioBudget.Name},
                //    CommittedProjectConsequences = project.CommittedProjectConsequences.Select(consequence =>
                //        new CommittedProjectConsequenceEntity
                //        {
                //            Id = consequence.Id,
                //            ChangeValue = consequence.ChangeValue,
                //            Attribute = new AttributeEntity {Name = consequence.Attribute.Name}
                //        }).ToList(),
                //}).AsNoTracking().ToList();

            if (projects.Any())
            {
                projects.ForEach(_ => {
                    var asset = assets.FirstOrDefault(a => _.CommittedProjectLocation.ToDomain().MatchOn(a.MaintainableAssetLocation.ToDomain()));
                    if (asset != null)
                    {
                        _.CreateCommittedProject(simulation, asset.Id, noTreatment, noTreatmentEntity);
                    }
                });
            }

        }

        public List<SectionCommittedProjectDTO> GetSectionCommittedProjectDTOs(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }
            
            var networkKeyAttribute = GetNetworkKeyAttribute(simulationId);
            var allProjectsInScenario = _unitOfWork.Context.CommittedProject.AsNoTracking()
                .Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.ScenarioBudget)
                .Include(_ => _.CommittedProjectConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CommittedProjectLocation)
                .Include(_=>_.Simulation.Network);

            return allProjectsInScenario
                .Where(_ => _.CommittedProjectLocation.Discriminator == DataPersistenceConstants.SectionLocation)
                .Select(_ => (SectionCommittedProjectDTO)_.ToDTO(networkKeyAttribute))
                .ToList();
        }               

        public List<BaseCommittedProjectDTO> GetCommittedProjectsForExport(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var networkKeyAttribute = GetNetworkKeyAttribute(simulationId);
            return _unitOfWork.Context.CommittedProject
                .Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.ScenarioBudget)
                .Include(_ => _.CommittedProjectConsequences)
                .ThenInclude(_ => _.Attribute)
                .Include(_ => _.CommittedProjectLocation)
                .Include(_ => _.Simulation.Network)
                .Select(_ => _.ToDTO(networkKeyAttribute))
                .ToList();
        }

        public void UpsertCommittedProjects(List<SectionCommittedProjectDTO> projects)
        {
            // Test for existing simulation
            var simulationIds = projects.Select(_ => _.SimulationId).Distinct().ToList();
            foreach (var simulation in simulationIds)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation))
                {
                    throw new RowNotInTableException($"Unable to find simulation ID {simulation} in database");
                }
            }

            // Test for existing budget
            var budgetIds = _unitOfWork.Context.ScenarioBudget.AsNoTracking()
                .Where(_ => simulationIds.Contains(_.SimulationId))
                .Select(_ => _.Id)
                .ToList();
            var badBudgets = projects
                .Where(_ => _.ScenarioBudgetId != null && !budgetIds.Contains(_.ScenarioBudgetId ?? Guid.Empty))
                .ToList();
            if (badBudgets.Any())
            {
                var budgetList = new StringBuilder();
                badBudgets.ForEach(budget => budgetList.Append(budget.ToString() + ", "));
                throw new RowNotInTableException($"Unable to find the following budget IDs in its matching simulation: {budgetList}");
            }

            var attributes = _unitOfWork.Context.Attribute.AsNoTracking().ToList();
            
            // Create entities and assign IDs
            var committedProjectEntities = projects.Select(p =>
                {
                    AssignIdWhenNull(p);
                    return p.ToEntity(attributes, GetNetworkKeyAttribute(p.SimulationId));
                }).ToList();

            var committedProjectConsequenceEntities = committedProjectEntities
                    .Where(_ => _.CommittedProjectConsequences.Any())
                    .SelectMany(_ => _.CommittedProjectConsequences)
                    .ToList();

            var locations = committedProjectEntities.Select(_ => _.CommittedProjectLocation).ToList();

            // Determine the committed projects that exist
            var allProvidedEntityIds = committedProjectEntities.Select(_ => _.Id).ToList();
            var allExistingCommittedProjectIds = new List<Guid>();
            foreach (var simulation in simulationIds)
            {
                var simulationProjects = _unitOfWork.Context.CommittedProject
                    .Where(_ => _.SimulationId == simulation && allProvidedEntityIds.Contains(_.Id))
                    .Select(_ => _.Id);
                allExistingCommittedProjectIds.AddRange(simulationProjects);
            }

            _unitOfWork.BeginTransaction();
            try
            {
                // Upsert(update/insert) all
                _unitOfWork.Context.UpsertAll(committedProjectEntities, _unitOfWork.UserEntity?.Id);

                _unitOfWork.Commit();
            }
            catch(Exception e)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public void DeleteSimulationCommittedProjects(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (!_unitOfWork.Context.CommittedProject.Any(_ => _.SimulationId == simulationId))
            {
                return;
            }

            try
            {
                _unitOfWork.BeginTransaction();
                _unitOfWork.Context.DeleteAll<CommittedProjectEntity>(_ => _.SimulationId == simulationId);
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
            }

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public void DeleteSpecificCommittedProjects(List<Guid> projectIds)
        {
            var simulationIds = _unitOfWork.Context.CommittedProject
                .Where(_ => projectIds.Contains(_.Id))
                .Select(_ => _.SimulationId);

            try
            {
                _unitOfWork.BeginTransaction();
                _unitOfWork.Context.DeleteAll<CommittedProjectEntity>(_ => projectIds.Contains(_.Id));
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw;
            }

            // Update last modified date
            foreach (var simulationId in simulationIds)
            {
                var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
                _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
            }
        }

        public Guid GetSimulationId(Guid projectId)
        {
            var project = _unitOfWork.Context.CommittedProject.FirstOrDefault(_ => _.Id == projectId);
            if (project == null)
            {
                throw new RowNotInTableException($"Unable to find project with ID {projectId}");
            }
            return project.SimulationId;
        }

        private void AssignIdWhenNull(BaseCommittedProjectDTO dto)
        {
            if (dto.Id == Guid.Empty) dto.Id = Guid.NewGuid();
            dto.Consequences.Where(c => c.Id == Guid.Empty)
                .ForEach(n => n.Id = Guid.NewGuid());
        }

        public string GetNetworkKeyAttribute(Guid simulationId)
        {
            var simulation = _unitOfWork.Context.Simulation.AsNoTracking().Include(_ => _.Network).FirstOrDefault(_ => _.Id == simulationId);
            return _unitOfWork.AttributeRepo.GetAttributeName(simulation.Network.KeyAttributeId);
        }
    }
}
