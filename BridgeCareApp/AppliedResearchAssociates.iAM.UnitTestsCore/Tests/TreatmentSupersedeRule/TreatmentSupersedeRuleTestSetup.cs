using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TreatmentSupersedeRule
{
    public class TreatmentSupersedeRuleTestSetup
    {
        public static Guid TreatmentEntityId { get; private set; }

        public static TreatmentSupersedeRuleDTO TreatmentSuperdedeRuleDto => TestEntitiesForTreatmentSupersedeRules.ScenarioTreatments.FirstOrDefault().ToDto(TreatmentDtos).SupersedeRules.FirstOrDefault();

        public static TreatmentDTO ToDtoWithSupersedeRules(TreatmentDTO dto, ScenarioSelectableTreatmentEntity scenarioSelectableTreatmentEntity)
        {
            dto.SupersedeRules = scenarioSelectableTreatmentEntity.ScenarioTreatmentSupersedeRules.Select(_ => _.ToDto(simpleTreatmentDtos))?.ToList();
            return dto;
        }

        public static List<TreatmentDTO> TreatmentDtos => TestEntitiesForTreatmentSupersedeRules.ScenarioTreatments.Select(t => ToDtoWithSupersedeRules(t.ToDto(), t)).ToList();
        

        public static List<TreatmentDTO> simpleTreatmentDtos => TestEntitiesForTreatmentSupersedeRules.ScenarioTreatments.Select(t => t.ToDto()).ToList();

        public static Analysis.TreatmentSupersedeRule TreatmentSupersedeRuleDomain(SimulationEntity simulationEntity, Simulation simulation)
        {
            var treatmentEntity = simulationEntity.SelectableTreatments.FirstOrDefault();
            var selectableTreatment = treatmentEntity.CreateSelectableTreatment(simulation, null);
            TreatmentEntityId = selectableTreatment.Id;
            var supersedeRule = selectableTreatment.AddSupersedeRule();
            var preventTreatmentEntity = simulationEntity.SelectableTreatments.Last();
            var preventTreatment = preventTreatmentEntity.CreateSelectableTreatment(simulation, null);
            supersedeRule.Treatment = preventTreatment;            
            return supersedeRule;
        }

        public static Analysis.SelectableTreatment TreatmentSupersedeDomain(SimulationEntity simulationEntity, Simulation simulation)
        {
            var treatmentEntity = simulationEntity.SelectableTreatments.Last();
            var selectableTreatment = treatmentEntity.CreateSelectableTreatment(simulation, null);            
            return selectableTreatment;
        }        
    }
}
