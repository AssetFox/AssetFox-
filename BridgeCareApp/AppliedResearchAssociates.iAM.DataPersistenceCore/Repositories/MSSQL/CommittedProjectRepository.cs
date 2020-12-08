using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class CommittedProjectRepository : MSSQLRepository, ICommittedProjectRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ICommittedProjectConsequenceRepository _committedProjectConsequenceRepo;

        public CommittedProjectRepository(ICommittedProjectConsequenceRepository committedProjectConsequenceRepo, IAMContext context) : base(context) =>
            _committedProjectConsequenceRepo = committedProjectConsequenceRepo ?? throw new ArgumentNullException(nameof(committedProjectConsequenceRepo));

        public void CreateCommittedProjects(List<CommittedProject> committedProjects, string simulationName)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Name == simulationName))
                    {
                        throw new RowNotInTableException($"No simulation found having name {simulationName}");
                    }

                    var simulationEntity = Context.Simulation
                        .Include(_ => _.Network)
                        .ThenInclude(_ => _.Facilities)
                        .ThenInclude(_ => _.Sections)
                        .Include(_ => _.InvestmentPlanSimulationJoin)
                        .ThenInclude(_ => _.InvestmentPlan)
                        .ThenInclude(_ => _.Budgets)
                        .Single(_ => _.Name == simulationName);

                    if (!simulationEntity.Network.Facilities.Any(_ => _.Sections.Any()))
                    {
                        throw new RowNotInTableException($"No sections found for simulation {simulationName}");
                    }

                    if (!simulationEntity.InvestmentPlanSimulationJoin.InvestmentPlan.Budgets.Any())
                    {
                        throw new RowNotInTableException($"No budgets found for simulation {simulationName}");
                    }

                    var attributeNames = committedProjects.SelectMany(_ => _.Consequences.Select(_ => _.Attribute.Name))
                        .Distinct().ToList();
                    var attributeEntities = Context.Attribute
                        .Where(_ => attributeNames.Contains(_.Name)).ToList();

                    if (!attributeEntities.Any())
                    {
                        throw new RowNotInTableException("No attributes found for committed project consequences.");
                    }

                    var attributeNamesFromDataSource = attributeEntities.Select(_ => _.Name).ToList();
                    if (!attributeNames.All(attributeName => attributeNamesFromDataSource.Contains(attributeName)))
                    {
                        var attributeNamesNotFound = attributeNames.Except(attributeNamesFromDataSource).ToList();
                        if (attributeNamesNotFound.Count() == 1)
                        {
                            throw new RowNotInTableException($"No attribute found having name {attributeNamesNotFound[0]}.");
                        }

                        throw new RowNotInTableException($"No attributes found having names: {string.Join(", ", attributeNamesNotFound)}.");
                    }

                    var attributeIdPerName = attributeEntities.ToDictionary(_ => _.Name, _ => _.Id);

                    var committedProjectConsequenceCommittedProjectIdAttributeIdTupleTuple =
                        new List<((Guid committedProjectId, Guid attributeId) committedProjectIdAttributeIdTuple, TreatmentConsequence
                            committedProjectConsequence)>();

                    var committedProjectEntities = committedProjects.Select(_ =>
                    {
                        var budgetEntity = simulationEntity.InvestmentPlanSimulationJoin.InvestmentPlan
                            .Budgets.Single(__ => __.Name == _.Budget.Name);

                        var sectionEntity = simulationEntity.Network.Facilities
                            .Single(__ => __.Sections.Any(___ => ___.Name == _.Section.Name))
                            .Sections.Single(__ => __.Name == _.Section.Name);

                        var entity = _.ToEntity(simulationEntity.Id, budgetEntity.Id, sectionEntity.Id);

                        if (_.Consequences.Any())
                        {
                            _.Consequences.ForEach(__ => committedProjectConsequenceCommittedProjectIdAttributeIdTupleTuple.Add(
                                ((entity.Id, attributeIdPerName[__.Attribute.Name]), __)
                            ));
                        }

                        return entity;
                    }).ToList();

                    if (IsRunningFromXUnit)
                    {
                        Context.CommittedProject.AddRange(committedProjectEntities);
                    }
                    else
                    {
                        Context.BulkInsert(committedProjectEntities);
                    }

                    Context.SaveChanges();

                    if (committedProjectConsequenceCommittedProjectIdAttributeIdTupleTuple.Any())
                    {
                        _committedProjectConsequenceRepo.CreateCommittedProjectConsequences(committedProjectConsequenceCommittedProjectIdAttributeIdTupleTuple);
                    }

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public void GetSimulationCommittedProjects(Simulation simulation)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having nme {simulation.Name}");
            }

            Context.CommittedProject
                .Include(_ => _.Budget)
                .Include(_ => _.Section)
                .ThenInclude(_ => _.Facility)
                .Include(_ => _.CommittedProjectConsequences)
                .Where(_ => _.Simulation.Name == simulation.Name)
                .ForEach(_ => _.CreateCommittedProject(simulation));
        }
    }
}
