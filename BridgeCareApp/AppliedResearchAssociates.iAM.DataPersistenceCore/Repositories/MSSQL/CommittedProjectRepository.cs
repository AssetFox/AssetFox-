using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CommittedProjectRepository : ICommittedProjectRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CommittedProjectRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateCommittedProjects(List<CommittedProject> committedProjects, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                    .Include(s => s.Network)
                    .ThenInclude(n => n.MaintainableAssets)                    
                    .Include(s => s.Budgets)
                .Where(_ => _.Id == simulationId).Single();

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);

            if (!simulationEntity.Network.MaintainableAssets.Any())
            {
                throw new RowNotInTableException("No maintainable assets found for given scenario.");
            }

            if (!simulationEntity.Budgets.Any())
            {
                throw new RowNotInTableException("No budgets found for given scenario.");
            }

            var committedProjectEntities = committedProjects
                .Select(_ => _.ToEntity(simulationEntity)).ToList();

            _unitOfWork.Context.AddAll(committedProjectEntities, _unitOfWork.UserEntity?.Id);

            if (committedProjects.Any(_ => _.Consequences.Any()))
            {
                var allConsequences = committedProjects.Where(_ => _.Consequences.Any()).SelectMany(_ => _.Consequences)
                    .ToList();
                var attributeEntities = _unitOfWork.Context.Attribute.ToList();
                var attributeNames = attributeEntities.Select(_ => _.Name).ToList();
                if (!allConsequences.All(_ => attributeNames.Contains(_.Attribute.Name)))
                {
                    var missingAttributes = allConsequences.Select(_ => _.Attribute.Name)
                        .Except(attributeNames).ToList();
                    if (missingAttributes.Count == 1)
                    {
                        throw new RowNotInTableException($"No attribute found having name {missingAttributes[0]}.");
                    }

                    throw new RowNotInTableException(
                        $"No attributes found having the names: {string.Join(", ", missingAttributes)}.");
                }

                var attributeIdPerName = attributeEntities.ToDictionary(_ => _.Name, _ => _.Id);

                var consequenceAttributeIdTuplePerProjectId = committedProjects
                    .Where(_ => _.Consequences.Any())
                    .ToDictionary(_ => _.Id,
                        _ => _.Consequences.Select(__ => (attributeIdPerName[__.Attribute.Name], __)).ToList());

                _unitOfWork.CommittedProjectConsequenceRepo.CreateCommittedProjectConsequences(
                    consequenceAttributeIdTuplePerProjectId);
            }
        }

        public void GetSimulationCommittedProjects(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var projects = _unitOfWork.Context.CommittedProject
                .Where(_ => _.SimulationId == simulation.Id)
                .Select(project => new CommittedProjectEntity
                {
                    Id = project.Id,
                    Name = project.Name,
                    ShadowForAnyTreatment = project.ShadowForAnyTreatment,
                    ShadowForSameTreatment = project.ShadowForSameTreatment,
                    Cost = project.Cost,
                    Year = project.Year,
                    MaintainableAsset =
                        new MaintainableAssetEntity
                        {
                            Id = project.MaintainableAssetId,
                            SpatialWeighting = project.MaintainableAsset.SpatialWeighting,
                            MaintainableAssetLocation = new MaintainableAssetLocationEntity
                            {
                                LocationIdentifier = project.MaintainableAsset.MaintainableAssetLocation.LocationIdentifier
                            }
                        },
                    ScenarioBudget = new ScenarioBudgetEntity {Name = project.ScenarioBudget.Name},
                    CommittedProjectConsequences = project.CommittedProjectConsequences.Select(consequence =>
                        new CommittedProjectConsequenceEntity
                        {
                            Id = consequence.Id,
                            ChangeValue = consequence.ChangeValue,
                            Attribute = new AttributeEntity {Name = consequence.Attribute.Name}
                        }).ToList()
                }).AsNoTracking().ToList();

            if (projects.Any())
            {
                projects.ForEach(_ => _.CreateCommittedProject(simulation));
            }

        }

        public List<CommittedProjectEntity> GetCommittedProjectsForExport(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            return _unitOfWork.Context.CommittedProject.Where(_ => _.SimulationId == simulationId)
                .Select(project => new CommittedProjectEntity
                {
                    Name = project.Name,
                    Year = project.Year,
                    ShadowForAnyTreatment = project.ShadowForAnyTreatment,
                    ShadowForSameTreatment = project.ShadowForSameTreatment,
                    Cost = project.Cost,
                    ScenarioBudget = new ScenarioBudgetEntity {Name = project.ScenarioBudget.Name},
                    MaintainableAsset = new MaintainableAssetEntity
                    {
                        MaintainableAssetLocation = new MaintainableAssetLocationEntity
                        {
                            LocationIdentifier = project.MaintainableAsset.MaintainableAssetLocation
                                .LocationIdentifier
                        }
                    },
                    CommittedProjectConsequences = project.CommittedProjectConsequences.Select(consequence =>
                        new CommittedProjectConsequenceEntity
                        {
                            Attribute = new AttributeEntity {Name = consequence.Attribute.Name},
                            ChangeValue = consequence.ChangeValue
                        }).ToList()
                })
                .ToList();
        }

        public void CreateCommittedProjects(List<CommittedProjectEntity> committedProjectEntities)
        {
            _unitOfWork.Context.AddAll(committedProjectEntities, _unitOfWork.UserEntity?.Id);

            var committedProjectConsequenceEntities = committedProjectEntities
                .Where(_ => _.CommittedProjectConsequences.Any())
                .SelectMany(_ => _.CommittedProjectConsequences)
                .ToList();

            _unitOfWork.Context.AddAll(committedProjectConsequenceEntities, _unitOfWork.UserEntity?.Id);
        }

        public void DeleteCommittedProjects(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if (!_unitOfWork.Context.CommittedProject.Any(_ => _.SimulationId == simulationId))
            {
                return;
            }

            _unitOfWork.Context.DeleteAll<CommittedProjectEntity>(_ => _.SimulationId == simulationId);

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }
    }
}
