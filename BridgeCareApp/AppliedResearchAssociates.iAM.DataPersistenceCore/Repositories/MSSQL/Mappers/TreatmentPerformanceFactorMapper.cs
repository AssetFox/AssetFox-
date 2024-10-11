using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentPerformanceFactorMapper
    {
        public static TreatmentPerformanceFactorDTO ToDto(this TreatmentPerformanceFactorEntity entity) =>
            new TreatmentPerformanceFactorDTO
            {
                Id = entity.Id,
                Attribute = entity.Attribute,
                PerformanceFactor = entity.PerformanceFactor,
            };

        public static TreatmentPerformanceFactorDTO ToDto(this ScenarioTreatmentPerformanceFactorEntity entity) =>
            new TreatmentPerformanceFactorDTO
            {
                Id = entity.Id,
                Attribute = entity.Attribute,
                PerformanceFactor = entity.PerformanceFactor,
            };


        public static ScenarioTreatmentPerformanceFactorEntity ToScenarioEntity(this TreatmentPerformanceFactorDTO dto, Guid treatmentId) =>
            new ScenarioTreatmentPerformanceFactorEntity { Id = dto.Id, ScenarioSelectableTreatmentId = treatmentId, Attribute = dto.Attribute, PerformanceFactor = dto.PerformanceFactor };

        public static TreatmentPerformanceFactorEntity ToLibraryEntity(this TreatmentPerformanceFactorDTO dto, Guid treatmentId) =>
           new TreatmentPerformanceFactorEntity { Id = dto.Id, TreatmentId = treatmentId, Attribute = dto.Attribute, PerformanceFactor = dto.PerformanceFactor };
    }
}
