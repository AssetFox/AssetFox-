using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CommittedProjectRepository : ICommittedProjectRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public CommittedProjectRepository(UnitOfDataPersistenceWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateCommittedProjects(List<CommittedProject> committedProjects, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Where(_ => _.Id == simulationId)
                .Select(simulation => new SimulationEntity
                {
                    Id = simulation.Id,
                    Network = new NetworkEntity
                    {
                        MaintainableAssets = simulation.Network.MaintainableAssets.Select(asset => new MaintainableAssetEntity
                        {
                            Id = asset.Id,
                            SectionName = asset.SectionName,
                            SpatialWeighting = asset.SpatialWeighting
                            //Area = asset.Area
                        }).ToList()
                    },
                    BudgetLibrarySimulationJoin = new BudgetLibrarySimulationEntity
                    {
                        BudgetLibrary = new BudgetLibraryEntity
                        {
                            Budgets = simulation.BudgetLibrarySimulationJoin.BudgetLibrary.Budgets
                                .Select(budget => new BudgetEntity { Id = budget.Id, Name = budget.Name })
                                .ToList()
                        }
                    }
                }).Single();

            if (!simulationEntity.Network.MaintainableAssets.Any())
            {
                throw new RowNotInTableException($"No maintainable assets found for simulation having id {simulationId}");
            }

            if (simulationEntity.BudgetLibrarySimulationJoin == null || !simulationEntity.BudgetLibrarySimulationJoin.BudgetLibrary.Budgets.Any())
            {
                throw new RowNotInTableException($"No budgets found for simulation having id {simulationId}");
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
                throw new RowNotInTableException($"No simulation found having id {simulation.Id}");
            }

            _unitOfWork.Context.CommittedProject
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
                            FacilityName = project.MaintainableAsset.FacilityName,
                            SectionName = project.MaintainableAsset.SectionName,
                            SpatialWeighting = project.MaintainableAsset.SpatialWeighting
                            //Area = project.MaintainableAsset.Area
                        },
                    Budget = new BudgetEntity {Name = project.Budget.Name},
                    CommittedProjectConsequences = project.CommittedProjectConsequences.Select(consequence =>
                        new CommittedProjectConsequenceEntity
                        {
                            Id = consequence.Id,
                            ChangeValue = consequence.ChangeValue,
                            Attribute = new AttributeEntity {Name = consequence.Attribute.Name}
                        }).ToList()
                }).AsNoTracking().ToList()
                .ForEach(_ => _.CreateCommittedProject(simulation));
        }
    }
}
