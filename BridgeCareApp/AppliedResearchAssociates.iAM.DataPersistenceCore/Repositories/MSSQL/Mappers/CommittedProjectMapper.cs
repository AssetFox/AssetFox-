﻿using System;
using System.Linq;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CommittedProjectMapper
    {
        public static CommittedProjectEntity ToEntity(this CommittedProject domain, SimulationEntity simulation)
        {
            var budget = simulation.Budgets.FirstOrDefault(_ => _.Name == domain.Budget.Name);

            var maintainableAsset = simulation.Network.MaintainableAssets.Single(_ =>
                _.Id == domain.Asset.Id);

            var entity = new CommittedProjectEntity
            {
                Id = domain.Id,
                SimulationId = simulation.Id,
                ScenarioBudgetId = budget?.Id,
                Name = budget?.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment,
                Cost = domain.Cost,
                Year = domain.Year
            };

            entity.CommittedProjectLocation = maintainableAsset.MaintainableAssetLocation.ToCommittedProjectLocation(entity);

            return entity;
        }

        public static BaseCommittedProjectDTO ToDTO(this CommittedProjectEntity entity)
        {
            
            switch (entity.CommittedProjectLocation.Discriminator)
            {
                case DataPersistenceConstants.SectionLocation:
                    var commit = new SectionCommittedProjectDTO()
                    {
                        Id = entity.Id,
                        Cost = entity.Cost,
                        ScenarioBudgetId = entity.ScenarioBudgetId,
                        SimulationId = entity.SimulationId,
                        Treatment = entity.Name,
                        Year = entity.Year,
                        LocationKeys = entity.CommittedProjectLocation.ToLocationKeys()
                    };
                    foreach (var consequence in entity.CommittedProjectConsequences)
                    {
                        commit.Consequences.Add(consequence.ToDTO());
                    }
                    return commit;
                default:
                    throw new ArgumentException($"Location type of {entity.CommittedProjectLocation.Discriminator} is not supported.");
            }
        }

        public static CommittedProjectEntity ToEntity(this BaseCommittedProjectDTO dto, ICollection<AttributeEntity> attributes)
        {
            var result = new CommittedProjectEntity
            {
                Id = dto.Id,
                SimulationId = dto.SimulationId,
                Name = dto.Treatment,
                Cost = dto.Cost,
                ScenarioBudgetId = dto.ScenarioBudgetId,
                ShadowForAnyTreatment = 1,
                ShadowForSameTreatment = 1,
                Year = dto.Year,
                CommittedProjectConsequences = new List<CommittedProjectConsequenceEntity>()
            };
            foreach (var consequence in dto.Consequences)
            {
                result.CommittedProjectConsequences.Add(consequence.ToEntity(attributes));
            }

            if (dto is SectionCommittedProjectDTO)
            {
                // TODO:  Switch to looking up key field in datasource object
                string keyField = "BRKEY";

                if (dto.LocationKeys.ContainsKey(keyField) && dto.LocationKeys.ContainsKey("ID"))
                {
                    result.CommittedProjectLocation = new CommittedProjectLocationEntity(
                        Guid.Parse(dto.LocationKeys["ID"]),
                        DataPersistenceConstants.SectionLocation,
                        dto.LocationKeys[keyField]
                        )
                    {
                        CommittedProjectId = result.Id,
                    };
                }
                else
                {
                    throw new ArgumentException($"The necessary key location fields are not present in {dto.Id}");
                }
            }
            else
            {
                throw new ArgumentException($"Cannot convert the DTO location for committed project with the ID ${dto.Id}");
            }

            return result;
        }
        public static CommittedProjectLocationEntity ToCommittedProjectLocation(this MaintainableAssetLocationEntity entity, CommittedProjectEntity commit)
        {
            return new CommittedProjectLocationEntity(Guid.NewGuid(), entity.Discriminator, entity.LocationIdentifier)
            {
                CommittedProjectId = commit.Id,
                Direction = entity.Direction,
                End = entity.End,
                Start = entity.Start
            };
        }

        public static Dictionary<string, string> ToLocationKeys(this CommittedProjectLocationEntity entity)
        {
            // TODO:  Switch to looking up key field in datasource object
            string keyField = "BRKEY";

            switch (entity.Discriminator)
            {
                case DataPersistenceConstants.SectionLocation:
                    var result = new Dictionary<string, string>();
                    result.Add("ID", entity.Id.ToString());
                    result.Add(keyField, entity.LocationIdentifier);
                    return result;
                default:
                    throw new ArgumentException($"Location type of {entity.Discriminator} is not supported.");
            }
        }

        public static void CreateCommittedProject(this CommittedProjectEntity entity, Simulation simulation, Guid maintainableAssetId)
        {
            var asset = simulation.Network.Assets.Single(_ =>
                _.Id == maintainableAssetId);

            var committedProject = simulation.CommittedProjects.GetAdd(new CommittedProject(asset, entity.Year));
            committedProject.Id = entity.Id;
            committedProject.Name = entity.Name;
            committedProject.ShadowForAnyTreatment = entity.ShadowForAnyTreatment;
            committedProject.ShadowForSameTreatment = entity.ShadowForSameTreatment;
            committedProject.Cost = entity.Cost;
            committedProject.Budget = simulation.InvestmentPlan.Budgets.Single(_ => _.Name == entity.ScenarioBudget.Name);

            if (entity.CommittedProjectConsequences.Any())
            {
                entity.CommittedProjectConsequences.ForEach(_ => _.CreateCommittedProjectConsequence(committedProject));
            }
        }
    }
}
