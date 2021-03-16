﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
                .Include(_ => _.Network)
                .ThenInclude(_ => _.Facilities)
                .ThenInclude(_ => _.Sections)
                .ThenInclude(_ => _.CommittedProjects)
                .Include(_ => _.CommittedProjects)
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
            var attributeEntities = _unitOfWork.Context.Attribute
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

            //_unitOfWork.Context.CommittedProject.Add(committedProjectEntities[0]);

            foreach (var item in committedProjectEntities)
            {
                _unitOfWork.Context.CommittedProject.Add(item);
            }
            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.CommittedProject.AddRange(committedProjectEntities);
            }
            else
            {
                //_unitOfWork.Context.BulkInsertOrUpdate(committedProjectEntities);
            }

            _unitOfWork.Context.SaveChanges();

            var consequencePerAttributeIdPerProjectId = committedProjects
                .ToDictionary(_ => _.Id, _ => _.Consequences.Select(__ => (attributeIdPerName[__.Attribute.Name], __)).ToList());

            if (consequencePerAttributeIdPerProjectId.Values.Any())
            {
                _unitOfWork.CommittedProjectConsequenceRepo.CreateCommittedProjectConsequences(consequencePerAttributeIdPerProjectId);
            }
        }

        public void GetSimulationCommittedProjects(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException($"No simulation found having id {simulation.Id}");
            }

            _unitOfWork.Context.CommittedProject
                .Include(_ => _.Budget)
                .Include(_ => _.Section)
                .ThenInclude(_ => _.Facility)
                .Include(_ => _.CommittedProjectConsequences)
                .Where(_ => _.Simulation.Id == simulation.Id)
                .ForEach(_ => _.CreateCommittedProject(simulation));
        }
    }
}
