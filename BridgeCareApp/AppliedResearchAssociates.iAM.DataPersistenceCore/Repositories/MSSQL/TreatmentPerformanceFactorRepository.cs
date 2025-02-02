﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class TreatmentPerformanceFactorRepository : ITreatmentPerformanceFactorRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public TreatmentPerformanceFactorRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void UpsertScenarioTreatmentPerformanceFactors(Dictionary<Guid, List<TreatmentPerformanceFactorDTO>> scenarioTreatmentPerformanceFactorsPerTreatmentId,
            Guid SimulationId)
        {
            var scenarioTreatmentPerformanceFactorEntities = scenarioTreatmentPerformanceFactorsPerTreatmentId
                .SelectMany(_ => _.Value.Select(factor => factor
                    .ToScenarioEntity(_.Key)))
                .ToList();

            var entityIds = scenarioTreatmentPerformanceFactorEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioTreatmentPerformanceFactor.AsNoTracking()
                .Where(_ => _.ScenarioSelectableTreatment.SimulationId == SimulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.UpdateAll(scenarioTreatmentPerformanceFactorEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());
            _unitOfWork.Context.AddAll(scenarioTreatmentPerformanceFactorEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());
        }

        public void DeleteScenarioTreatmentPerformanceFactors(Dictionary<Guid, List<TreatmentPerformanceFactorDTO>> scenarioTreatmentPerformanceFactorsPerTreatmentId,
            Guid SimulationId)
        {
            var scenarioTreatmentPerformanceFactorEntities = scenarioTreatmentPerformanceFactorsPerTreatmentId
                .SelectMany(_ => _.Value.Select(factor => factor
                    .ToScenarioEntity(_.Key)))
                .ToList();

            var entityIds = scenarioTreatmentPerformanceFactorEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.ScenarioTreatmentPerformanceFactor.AsNoTracking()
                .Where(_ => _.ScenarioSelectableTreatment.SimulationId == SimulationId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.DeleteAll<ScenarioTreatmentPerformanceFactorEntity>(_ =>
                _.ScenarioSelectableTreatment.SimulationId == SimulationId && !entityIds.Contains(_.Id));
        }

        public void UpsertLibraryTreatmentPerformanceFactors(Dictionary<Guid, List<TreatmentPerformanceFactorDTO>> TreatmentPerformanceFactorPerTreatmentId, Guid LibraryId)
        {
            var TreatmentPerformanceFactorEntities = TreatmentPerformanceFactorPerTreatmentId
                .SelectMany(_ => _.Value.Select(factor => factor
                    .ToLibraryEntity(_.Key)))
                .ToList();

            var entityIds = TreatmentPerformanceFactorEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.TreatmentPerformanceFactor.AsNoTracking()
                .Where(_ => _.SelectableTreatment.TreatmentLibraryId == LibraryId && entityIds.Contains(_.Id))
                .Select(_ => _.Id).ToList();

            _unitOfWork.Context.UpdateAll(TreatmentPerformanceFactorEntities.Where(_ => existingEntityIds.Contains(_.Id)).ToList());
            _unitOfWork.Context.AddAll(TreatmentPerformanceFactorEntities.Where(_ => !existingEntityIds.Contains(_.Id)).ToList());
        }
    }
}
