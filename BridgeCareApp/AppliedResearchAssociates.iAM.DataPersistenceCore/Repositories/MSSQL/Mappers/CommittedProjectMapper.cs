using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.DTOs.Enums;
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
                Year = domain.Year,
                treatmentCategory = domain.treatmentCategory,
                ProjectSource = domain.ProjectSource.ToString()
            };

            entity.CommittedProjectLocation = maintainableAsset.MaintainableAssetLocation.ToCommittedProjectLocation(entity);

            return entity;
        }

        public static BaseCommittedProjectDTO ToDTO(this CommittedProjectEntity entity, string networkKeyAttribute)
        {
            TreatmentCategory convertedCategory = entity.treatmentCategory != default(TreatmentCategory) ? entity.treatmentCategory : default(TreatmentCategory);
            if (Enum.TryParse(typeof(TreatmentCategory), entity.Category, true, out var convertedCategoryOut))
            {
                convertedCategory = (TreatmentCategory)convertedCategoryOut;
            }

            switch (entity.CommittedProjectLocation.Discriminator)
            {
            case DataPersistenceConstants.SectionLocation:
                if (entity.ScenarioBudgetId != null && entity.ScenarioBudget == null)
                {
                    throw new InvalidOperationException($"Scenario budget is not present in committed project.");
                }

                if (string.IsNullOrEmpty(entity.ProjectSource) || !Enum.TryParse(entity.ProjectSource, out ProjectSourceDTO projectSourceDTO))
                {
                    projectSourceDTO = ProjectSourceDTO.None;
                }

                var commit = new SectionCommittedProjectDTO()
                {
                    Id = entity.Id,
                    Cost = entity.Cost,
                    ScenarioBudgetId = entity.ScenarioBudgetId,
                    SimulationId = entity.SimulationId,
                    Treatment = entity.Name,
                    Year = entity.Year,
                    ProjectSource = projectSourceDTO,
                    ShadowForAnyTreatment = entity.ShadowForAnyTreatment,
                    ShadowForSameTreatment = entity.ShadowForSameTreatment,
                    Category = convertedCategory,
                    LocationKeys = entity.CommittedProjectLocation?.ToLocationKeys(networkKeyAttribute)
                };
                return commit;
                default:
                    throw new ArgumentException($"Location type of {entity.CommittedProjectLocation.Discriminator} is not supported.");
            }
        }

        public static CommittedProjectEntity ToEntity(this BaseCommittedProjectDTO dto, IList<AttributeEntity> attributes, string networkKeyAttribute, BaseEntityProperties baseEntityProperties = null)
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
                ProjectSource = dto.ProjectSource.ToString()
            };

            if (dto is SectionCommittedProjectDTO)
            {
                if (dto.VerifyLocation(networkKeyAttribute))
                {
                    result.CommittedProjectLocation = new CommittedProjectLocationEntity(
                        Guid.Parse(dto.LocationKeys["ID"]),
                        DataPersistenceConstants.SectionLocation,
                        dto.LocationKeys[networkKeyAttribute]
                        )
                    {
                        CommittedProjectId = result.Id
                    };
                }
                else
                {
                    throw new ArgumentException($"The necessary key location fields are not present in committed project {dto.Id}");
                }
            }
            else
            {
                throw new ArgumentException($"Cannot convert the DTO location for committed project with the ID ${dto.Id}");
            }
            BaseEntityPropertySetter.SetBaseEntityProperties( result, baseEntityProperties );
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

        public static Dictionary<string, string> SectionLocationKeys(
            string idKey,
            Guid id,
            string networkKeyAttribute,
            string locationIdentifier)
        {
            var dictionary = new Dictionary<string, string>
            {
                { idKey, id.ToString() },
                    { networkKeyAttribute, locationIdentifier }
                };
            return dictionary;
        }

        public static Dictionary<string, string> ToLocationKeys(this CommittedProjectLocationEntity entity, string networkKeyAttribute)
        {
            const string IdKey = "ID";
            switch (entity.Discriminator)
            {
            case DataPersistenceConstants.SectionLocation:
                var result = SectionLocationKeys(IdKey, entity.Id, networkKeyAttribute, entity.LocationIdentifier);
                return result;
            default:
                throw new ArgumentException($"Location type of {entity.Discriminator} is not supported.");
            }
        }

        public static void CreateCommittedProject(
            this CommittedProjectEntity entity,
            Simulation simulation,
            IReadOnlyCollection<SelectableTreatment> selectableTreatments,
            Guid maintainableAssetId,
            bool noTreatmentForCommittedProjects,
            double noTreatmentDefaultCost,
            ScenarioSelectableTreatmentEntity noTreatmentEntity,
            List<string> keyPropertyNames = null)
        {
            var asset = simulation.Network.Assets.Single(_ => _.Id == maintainableAssetId);

            /* Allow multiple committed projects for the same asset in the same year, enforcing uniqueness based on treatment.
                Projects can share the same asset and year as long as they involve different treatments. Adding a new project with 
                a treatment already existing for the same asset and year results in an error, ensuring diversity in treatment 
                strategies without duplication for any asset within a single year. */

            void throwError_DuplicateTreatmentProjects(Exception innerException, string treatmentName)
            {
                string assetLabel;
                if (keyPropertyNames is null)
                {
                    assetLabel = asset.Id.ToString();
                }
                else
                {
                    var attributeByName = simulation.Network.Explorer.AllAttributes.ToDictionary(a => a.Name);
                    var keyProperties = keyPropertyNames.Select(name => $"[{name}]: {asset.GetHistory(attributeByName[name]).MostRecentValue}");
                    assetLabel = string.Join(", ", keyProperties);
                }

                throw new InvalidOperationException($"Asset ({assetLabel}) has duplicate treatments in year {entity.Year}.", innerException);
            }

            try
            {
                var existingCommittedProjectsForThisAssetYear =
                    simulation.CommittedProjects
                    .Where(cp => (cp.Asset.Id, cp.Year) == (asset.Id, entity.Year))
                    .ToList();

                var projectToAddTreatmentName = entity.Name;
                // Check if a project with the same treatment name already exists for the asset and year.
                var projectWithActiveTreatmentAlreadyExists = existingCommittedProjectsForThisAssetYear.Any(cp => cp.Name == projectToAddTreatmentName);

                if (projectWithActiveTreatmentAlreadyExists)
                {
                    throwError_DuplicateTreatmentProjects(null, projectToAddTreatmentName);
                }

                var mainProject =
                    existingCommittedProjectsForThisAssetYear.SingleOrDefault(cp => cp.Name != noTreatmentEntity.Name) ??
                    existingCommittedProjectsForThisAssetYear.FirstOrDefault();

                foreach (var otherProject in existingCommittedProjectsForThisAssetYear.Where(cp => cp != mainProject))
                {
                    _ = simulation.CommittedProjects.Remove(otherProject);
                }
            }
            catch (InvalidOperationException e)
            {
                throwError_DuplicateTreatmentProjects(e, entity.Name);
            }

            var committedProject = simulation.CommittedProjects.GetAdd(new CommittedProject(asset, entity.Year));
            committedProject.Id = entity.Id;
            committedProject.Name = entity.Name;
            committedProject.Cost = entity.Cost;
            var treatmentName = entity.Name;
            var treatment = selectableTreatments.FirstOrDefault(t => t.Name == treatmentName);

            committedProject.TemplateTreatment = treatment;
            var projectSource = ProjectSourceDTO.Committed;

            if (Enum.TryParse(entity.ProjectSource, true, out ProjectSourceDTO parsedProjectSource))
            {
                committedProject.ProjectSource = parsedProjectSource;
                projectSource = parsedProjectSource;
            }
            else
            {
                committedProject.ProjectSource = ProjectSourceDTO.Committed; 
            }

            committedProject.Budget = entity.ScenarioBudget != null ? simulation.InvestmentPlan.Budgets.Single(_ => _.Name == entity.ScenarioBudget.Name) : null;
            committedProject.LastModifiedDate = entity.LastModifiedDate;

            if (noTreatmentForCommittedProjects)
            {
                int startYear = simulation.InvestmentPlan.FirstYearOfAnalysisPeriod;
                var noTreatment = selectableTreatments.FirstOrDefault(t => noTreatmentEntity.Name == t.Name);
                for (int year = startYear; year < committedProject.Year; year++)
                {
                    var existingCommittedProject = simulation.CommittedProjects
                        .Where(_ => _.Year == year && _.Asset.Id == committedProject.Asset.Id);
                    if (!existingCommittedProject.Any())
                    {
                        var projectToAdd = simulation.CommittedProjects.GetAdd(new CommittedProject(asset, year));
                        projectToAdd.Id = Guid.NewGuid();
                        projectToAdd.Name = noTreatmentEntity.Name;
                        projectToAdd.Cost = noTreatmentDefaultCost;
                        projectToAdd.Budget = entity.ScenarioBudget != null ? simulation.InvestmentPlan.Budgets.Single(_ => _.Name == entity.ScenarioBudget.Name) : null; ; // TODO: fix
                        //projectToAdd.Budget = null;  // This would be the better way, but it fails vaildation
                        projectToAdd.ProjectSource = projectSource;  // Replicate project source
                        projectToAdd.LastModifiedDate = noTreatmentEntity.LastModifiedDate;
                        //projectToAdd.TemplateTreatment = noTreatmentEntity.ToDomain(simulation, null);
                        projectToAdd.TemplateTreatment = noTreatment;
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
    }
}
