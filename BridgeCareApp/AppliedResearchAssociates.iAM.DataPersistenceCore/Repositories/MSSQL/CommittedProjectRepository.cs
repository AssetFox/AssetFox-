using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using AppliedResearchAssociates.iAM.Hubs.Services;
using AppliedResearchAssociates.iAM.Hubs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Microsoft.Extensions.DependencyModel;
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
            Guid guid = simulation.Id;
            double noTreatmentDefaultCost = 0.0;
            var selectableTreatmentRepository = _unitOfWork.SelectableTreatmentRepo;
            var selectableTreatments = simulation.Treatments;

            var simulationEntity = _unitOfWork.Context.Simulation.FirstOrDefault(_ => _.Id == simulation.Id)
                ?? throw new RowNotInTableException("No simulation was found for the given scenario.");

            var noTreatmentEntity = selectableTreatmentRepository.GetDefaultTreatment(simulation.Id)
                ?? throw new RowNotInTableException("Simulation has no default treatments");

            var assets = _unitOfWork.Context.MaintainableAsset
                .Where(_ => _.NetworkId == simulation.Network.Id)
                .Include(_ => _.MaintainableAssetLocation)
                .ToList();

            var projects = _unitOfWork.Context.CommittedProject
                .Include(_ => _.CommittedProjectLocation)
                .Include(_ => _.ScenarioBudget)
                .Where(_ => _.SimulationId == simulation.Id)
                .OrderBy(_ => _.Year).ToList();

            var keyPropertyNames = (List<string>)_unitOfWork.AdminSettingsRepo.GetKeyFields();

            foreach (var project in projects)
            {
                var asset = assets.FirstOrDefault(a => project.CommittedProjectLocation.ToDomain().MatchOn(a.MaintainableAssetLocation.ToDomain()));
                if (asset != null)
                {
                    if (simulationEntity.NoTreatmentBeforeCommittedProjects)
                    {
                        var defaultNoTreatment = selectableTreatmentRepository.GetDefaultNoTreatment(simulation.Id);
                        noTreatmentDefaultCost = GetDefaultNoTreatmentCost(defaultNoTreatment, asset.Id);
                    }
                    project.CreateCommittedProject(simulation, selectableTreatments, asset.Id, simulationEntity.NoTreatmentBeforeCommittedProjects, noTreatmentDefaultCost, noTreatmentEntity, keyPropertyNames);
                }
            }
        }

        public void SetCommittedProjectTemplate(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            var fileSize = stream.Length;
            var bytes = br.ReadBytes(Convert.ToInt32(fileSize));
            br.Close();
            stream.Close();

            var existingComittedProjectTemplate = _unitOfWork.Context.CommittedProjectSettings.Where(_ => _.Key == "Committed Project Template").FirstOrDefault();
            if (existingComittedProjectTemplate == null)
                _unitOfWork.Context.CommittedProjectSettings.Add(new CommittedProjectSettingsEntity
                {
                    Key = "Committed Project Template",
                    Value = string.Format(Convert.ToBase64String(bytes))
                });
            else
            {
                existingComittedProjectTemplate.Value = string.Format(Convert.ToBase64String(bytes));
                _unitOfWork.Context.CommittedProjectSettings.Update(existingComittedProjectTemplate);
            }
            _unitOfWork.Context.SaveChanges();
         }


        public double GetDefaultNoTreatmentCost(TreatmentDTO treatment, Guid assetId)
        {
            double totalCost = 0.0;
            treatment.Costs.ForEach(c =>
            {
                var compiler = new CalculateEvaluateCompiler();
                var attributes = InstantiateCompilerAndGetExpressionAttributes(c.Equation.Expression, compiler);
                var calculator = compiler.GetCalculator(c.Equation.Expression);
                var scope = new CalculateEvaluateScope();

                var aggResults = _unitOfWork.Context.AggregatedResult.AsNoTracking().Include(_ => _.Attribute)
                    .Where(_ => _.MaintainableAssetId == assetId).ToList().Where(_ => attributes.Any(a => a.Id == _.AttributeId)).ToList();
                var latestAggResults = new List<AggregatedResultEntity>();
                foreach (var attr in attributes)
                {
                    var attrs = aggResults.Where(_ => _.AttributeId == attr.Id).ToList();
                    if (attrs.Count == 0)
                        continue;
                    var latestYear = attrs.Max(_ => _.Year);
                    var latestAggResult = attrs.FirstOrDefault(_ => _.Year == latestYear);
                    latestAggResults.Add(latestAggResult);
                }
                latestAggResults.ForEach(_ =>
                {
                    if (_.Attribute.DataType == "NUMBER")
                    {
                        scope.SetNumber(_.Attribute.Name, _.NumericValue.Value);
                    }
                    else
                    {
                        scope.SetText(_.Attribute.Name, _.TextValue);
                    }
                });
                var currentCost = calculator.Delegate(scope);
                totalCost += currentCost;
            });
            return totalCost;
        }

        public string DownloadCommittedProjectTemplate()
        {
            try
            {
                return _unitOfWork.Context.CommittedProjectSettings.Where(_ => _.Key == "Committed Project Template").FirstOrDefault().Value;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public string DownloadSelectedCommittedProjectTemplate(string filename)
        {
            try
            {
                return _unitOfWork.Context.CommittedProjectTemplates.Where(_ => _.Key == filename).FirstOrDefault().Value;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<String> getUploadedCommittedProjectTemplates()
        {
            var names = _unitOfWork.Context.CommittedProjectTemplates
                .Select(t => t.Key)
                .ToList();

            return names;
        }

        public void AddCommittedProjectTemplate(Stream stream, string filename)
        {
            BinaryReader br = new BinaryReader(stream);
            var fileSize = stream.Length;
            var bytes = br.ReadBytes(Convert.ToInt32(fileSize));
            br.Close();
            stream.Close();

            var existingComittedProjectTemplate = _unitOfWork.Context.CommittedProjectTemplates.Where(_ => _.Key == filename).FirstOrDefault();
            if (existingComittedProjectTemplate == null)
                _unitOfWork.Context.CommittedProjectTemplates.Add(new CommittedProjectTreatmentEntity
                {
                    Key = filename,
                    Value = string.Format(Convert.ToBase64String(bytes))
                });
            else
            {
                existingComittedProjectTemplate.Value = string.Format(Convert.ToBase64String(bytes));
                _unitOfWork.Context.CommittedProjectTemplates.Update(existingComittedProjectTemplate);
            }
            _unitOfWork.Context.SaveChanges();
        }

        private List<AttributeEntity> InstantiateCompilerAndGetExpressionAttributes(string mergedCriteriaExpression, CalculateEvaluateCompiler compiler)
        {
            var modifiedExpression = mergedCriteriaExpression
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("@", "")
                    .Replace("|", "'")
                    .ToUpper();

            var pattern = "\\[[^\\]]*\\]";
            var rg = new Regex(pattern);
            var match = rg.Matches(mergedCriteriaExpression);
            var hashMatch = new HashSet<string>();
            foreach (Match m in match)
            {
                hashMatch.Add(m.Value.Substring(1, m.Value.Length - 2));
            }

            var attributes = _unitOfWork.Context.Attribute.AsNoTracking()
                .Where(_ => hashMatch.Contains(_.Name))
                .Select(attribute => new AttributeEntity
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    DataType = attribute.DataType
                }).AsNoTracking().ToList();

            attributes.ForEach(attribute =>
            {
                compiler.ParameterTypes[attribute.Name] = attribute.DataType == "NUMBER"
                    ? CalculateEvaluateParameterType.Number
                    : CalculateEvaluateParameterType.Text;
            });

            return attributes;
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
            _unitOfWork.AsTransaction(() =>
            {                
                _unitOfWork.Context.UpsertAll(committedProjectEntities);
                var committedProjectLocations = committedProjectEntities.Select(_ => _.CommittedProjectLocation).ToList();
                _unitOfWork.Context.UpsertAll(committedProjectLocations);
            });
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

            _unitOfWork.AsTransaction(() =>
                _unitOfWork.Context.DeleteAll<CommittedProjectEntity>(_ => _.SimulationId == simulationId));

            // Update last modified date
            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
        }

        public void DeleteSpecificCommittedProjects(List<Guid> projectIds)
        {
            var simulationIds = _unitOfWork.Context.CommittedProject
                .Where(_ => projectIds.Contains(_.Id))
                .Select(_ => _.SimulationId);

            _unitOfWork.AsTransaction(() => 
                _unitOfWork.Context.DeleteAll<CommittedProjectEntity>(_ => projectIds.Contains(_.Id)));
              

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
        }

        public string GetNetworkKeyAttribute(Guid simulationId)
        {
            var simulation = _unitOfWork.Context.Simulation.AsNoTracking().Include(_ => _.Network).FirstOrDefault(_ => _.Id == simulationId);
            return _unitOfWork.AttributeRepo.GetAttributeName(simulation.Network.KeyAttributeId);
        }
    }
}
