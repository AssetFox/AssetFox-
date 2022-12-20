using System;
using System.Linq;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using MoreLinq;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Data.Attributes;
using OfficeOpenXml.FormulaParsing.Utilities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

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
            TreatmentCategory convertedCategory = default(TreatmentCategory);
            if (Enum.TryParse(typeof(TreatmentCategory), entity.Category, true, out var convertedCategoryOut))
            {
                convertedCategory = (TreatmentCategory)convertedCategoryOut;
            }

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
                        ShadowForAnyTreatment= entity.ShadowForAnyTreatment,
                        ShadowForSameTreatment= entity.ShadowForSameTreatment,
                        Category = convertedCategory,
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

        public static CommittedProjectEntity ToEntity(this BaseCommittedProjectDTO dto, IList<AttributeEntity> attributes)
        {
            var result = new CommittedProjectEntity
            {
                Id = dto.Id,
                SimulationId = dto.SimulationId,
                Name = dto.Treatment,
                Cost = dto.Cost,
                ScenarioBudgetId = dto.ScenarioBudgetId,
                ShadowForAnyTreatment = dto.ShadowForAnyTreatment,
                ShadowForSameTreatment = dto.ShadowForSameTreatment,
                Category = dto.Category.ToString(),
                Year = dto.Year,
                CommittedProjectConsequences = new List<CommittedProjectConsequenceEntity>()
            };
            foreach (var consequence in dto.Consequences)
            {
                result.CommittedProjectConsequences.Add(consequence.ToEntity(attributes));
            }

            if (dto is SectionCommittedProjectDTO)
            {
                // TODO:  Switch to looking up key field in network object
                string keyField = "BRKEY_";

                if (dto.LocationKeys.ContainsKey(keyField) && dto.LocationKeys.ContainsKey("ID"))
                {
                    result.CommittedProjectLocation = new CommittedProjectLocationEntity(
                        Guid.Parse(dto.LocationKeys["ID"]),
                        DataPersistenceConstants.SectionLocation,
                        dto.LocationKeys[keyField]
                        )
                    {
                        CommittedProjectId = result.Id,
                        Id = Guid.NewGuid()
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
            string keyField = "BRKEY_";

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

        public static void CreateCommittedProject(
            this CommittedProjectEntity entity,
            Simulation simulation,
            Guid maintainableAssetId,
            bool noTreatmentForCommittedProjects,
            ScenarioSelectableTreatmentEntity noTreatmentEntity)
        {
            var asset = simulation.Network.Assets.Single(_ =>
                _.Id == maintainableAssetId);

            // Ensure a no treatment committed project does not already exist
            try
            {
                var existingCommittedProject = simulation.CommittedProjects.SingleOrDefault(_ => _.Asset.Id == asset.Id && _.Year == entity.Year);
                if (existingCommittedProject != null)
                {
                    if (existingCommittedProject.Name == noTreatmentEntity.Name)
                    {
                        simulation.CommittedProjects.Remove(existingCommittedProject);
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"{asset.Id} has multiple committed projects in year {entity.Year}");
            }
            catch (ArgumentException)
            {
                throw new InvalidOperationException($"{asset.Id} has a project type that was automatically added but is not the default treatment");
            }
            

            var committedProject = simulation.CommittedProjects.GetAdd(new CommittedProject(asset, entity.Year));
            committedProject.Id = entity.Id;
            committedProject.Name = entity.Name;
            committedProject.ShadowForAnyTreatment = entity.ShadowForAnyTreatment;
            committedProject.ShadowForSameTreatment = entity.ShadowForSameTreatment;
            committedProject.Cost = entity.Cost; 
            committedProject.Budget = entity.ScenarioBudget != null ? simulation.InvestmentPlan.Budgets.Single(_ => _.Name == entity.ScenarioBudget.Name) : null;
            committedProject.LastModifiedDate = entity.LastModifiedDate;

            if (entity.CommittedProjectConsequences.Any())
            {
                entity.CommittedProjectConsequences.ForEach(_ => _.CreateCommittedProjectConsequence(committedProject));
            }
            if (noTreatmentForCommittedProjects)
            {
                int startYear = simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;
                for (int year = startYear; year < committedProject.Year; year++)
                {
                    var existingCommittedProject = simulation.CommittedProjects
                        .Where(_ => _.Year == year && _.Asset.Id == committedProject.Asset.Id);
                    if (!existingCommittedProject.Any())
                    {
                        var projectToAdd = simulation.CommittedProjects.GetAdd(new CommittedProject(asset, year));
                        projectToAdd.Id = Guid.NewGuid();
                        projectToAdd.Name = noTreatmentEntity.Name;
                        projectToAdd.ShadowForAnyTreatment = 0;
                        projectToAdd.ShadowForSameTreatment = 0;
                        projectToAdd.Cost = 0; // TODO -- this is wrong. See CommittedProjectService.GetTreatmentCost for what we may need to do here. But it's not simple.
                        projectToAdd.Budget = entity.ScenarioBudget != null ? simulation.InvestmentPlan.Budgets.Single(_ => _.Name == entity.ScenarioBudget.Name) : null; ; // TODO: fix
                        //projectToAdd.Budget = null;  // This would be the better way, but it fails vaildation
                        projectToAdd.LastModifiedDate = noTreatmentEntity.LastModifiedDate;
                        projectToAdd.TemplateTreatment = noTreatmentEntity.ToDomain(simulation);
                    }
                    else
                    {
                        if (existingCommittedProject.Count() > 1)
                        {
                            throw new InvalidOperationException("Should not see more than one project per committed project query");
                        }
                    }
                }
                
            }
        }

        private static SelectableTreatment MapNoTreatmentToDomain(Simulation simulation, SelectableTreatmentEntity noTreatmentEntity)
        {
            var domain = simulation.AddTreatment();
            throw new NotImplementedException();
        }
    }
}
