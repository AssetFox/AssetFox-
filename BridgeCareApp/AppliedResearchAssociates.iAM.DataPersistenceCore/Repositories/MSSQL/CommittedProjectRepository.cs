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
using AppliedResearchAssociates.iAM.DTOs.Abstract;

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
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }
            var assets = _unitOfWork.Context.MaintainableAsset
                .Where(_ => _.NetworkId == simulation.Network.Id)
                .Include(_ => _.MaintainableAssetLocation)
                .ToList();

            var projects = _unitOfWork.Context.CommittedProject
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
                    ScenarioBudget = new ScenarioBudgetEntity { Name = project.ScenarioBudget.Name },
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
                        _.CreateCommittedProject(simulation, asset.Id);
                    }
                });
            }

        }

        public List<BaseCommittedProjectDTO> GetCommittedProjectsForExport(Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            //var entity = _unitOfWork.Context.CommittedProject.Where(_ => _.SimulationId == simulationId)
            //    .Select(project => new CommittedProjectEntity
            //    {
            //        Name = project.Name,
            //        Year = project.Year,
            //        ShadowForAnyTreatment = project.ShadowForAnyTreatment,
            //        ShadowForSameTreatment = project.ShadowForSameTreatment,
            //        Cost = project.Cost,
            //        ScenarioBudget = new ScenarioBudgetEntity { Name = project.ScenarioBudget.Name },
            //        CommittedProjectConsequences = project.CommittedProjectConsequences.Select(consequence =>
            //            new CommittedProjectConsequenceEntity
            //            {
            //                Attribute = new AttributeEntity { Name = consequence.Attribute.Name },
            //                ChangeValue = consequence.ChangeValue
            //            }).ToList(),
            //        CommittedProjectLocation = project.CommittedProjectLocation
            //    })
            //    .Select(_ => _.ToDTO()) // TODO:  Just create the DTO directly?
            //    .ToList();

            return _unitOfWork.Context.CommittedProject
                .Where(_ => _.SimulationId == simulationId)
                .Include(_ => _.ScenarioBudget)
                .Include(_ => _.CommittedProjectConsequences)
                .Include(_ => _.CommittedProjectLocation)
                .Select(_ => _.ToDTO())
                .ToList();
        }

        public void CreateCommittedProjects(List<BaseCommittedProjectDTO> committedProjects)
        {
            var committedProjectEntities = committedProjects.Select(_ => _.ToEntity(_unitOfWork.Context.Attribute.ToList())).ToList();

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
