﻿using System;
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
    public class CommittedProjectRepository : ICommittedProjectRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ICommittedProjectConsequenceRepository _committedProjectConsequenceRepo;
        private readonly IAMContext _context;

        public CommittedProjectRepository(ICommittedProjectConsequenceRepository committedProjectConsequenceRepo,
            IAMContext context)
        {
            _committedProjectConsequenceRepo = committedProjectConsequenceRepo ??
                                               throw new ArgumentNullException(nameof(committedProjectConsequenceRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void CreateCommittedProjects(List<CommittedProject> committedProjects, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _context.Simulation
                .Include(_ => _.Network)
                .ThenInclude(_ => _.Facilities)
                .ThenInclude(_ => _.Sections)
                .Include(_ => _.BudgetLibrarySimulationJoin)
                .ThenInclude(_ => _.BudgetLibrary)
                .ThenInclude(_ => _.Budgets)
                .Single(_ => _.Id == simulationId);

            if (!simulationEntity.Network.Facilities.Any(_ => _.Sections.Any()))
            {
                throw new RowNotInTableException($"No sections found for simulation having id {simulationId}");
            }

            if (simulationEntity.BudgetLibrarySimulationJoin == null || !simulationEntity.BudgetLibrarySimulationJoin.BudgetLibrary.Budgets.Any())
            {
                throw new RowNotInTableException($"No budgets found for simulation having id {simulationId}");
            }

            var attributeNames = committedProjects.SelectMany(_ => _.Consequences.Select(_ => _.Attribute.Name))
                .Distinct().ToList();
            var attributeEntities = _context.Attribute
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

            var committedProjectEntities = committedProjects
                .Select(_ => _.ToEntity(simulationEntity.Id)).ToList();

            if (IsRunningFromXUnit)
            {
                _context.CommittedProject.AddRange(committedProjectEntities);
            }
            else
            {
                _context.BulkInsert(committedProjectEntities);
            }

            var consequencePerAttributeIdPerProjectId = committedProjects
                .ToDictionary(_ => _.Id, _ => _.Consequences.Select(__ => (attributeIdPerName[__.Attribute.Name], __)).ToList());

            if (consequencePerAttributeIdPerProjectId.Values.Any())
            {
                _committedProjectConsequenceRepo.CreateCommittedProjectConsequences(consequencePerAttributeIdPerProjectId);
            }
        }

        public void GetSimulationCommittedProjects(Simulation simulation)
        {
            if (!_context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having nme {simulation.Name}");
            }

            _context.CommittedProject
                .Include(_ => _.Budget)
                .Include(_ => _.Section)
                .ThenInclude(_ => _.Facility)
                .Include(_ => _.CommittedProjectConsequences)
                .Where(_ => _.Simulation.Name == simulation.Name)
                .ForEach(_ => _.CreateCommittedProject(simulation));
        }
    }
}
