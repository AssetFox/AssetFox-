﻿using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.TreatmentSupersedeRule
{
    public class TreatmentSupersedeRuleTestSetup
    {
        public static Guid TreatmentEntityId { get; private set; }

        public static TreatmentSupersedeRuleDTO TreatmentSuperdedeRuleDto => TestEntitiesForTreatmentSupersedeRules.ScenarioTreatments.FirstOrDefault().ToDto().SupersedeRules.FirstOrDefault();

        public static Analysis.TreatmentSupersedeRule TreatmentSupersedeRuleDomain(SimulationEntity simulationEntity, Simulation simulation)
        {
            var treatmentEntity = simulationEntity.SelectableTreatments.FirstOrDefault();
            var selectableTreatment = treatmentEntity.CreateSelectableTreatment(simulation);
            TreatmentEntityId = selectableTreatment.Id;
            var supersedeRule = selectableTreatment.AddSupersedeRule();
            var preventTreatmentEntity = simulationEntity.SelectableTreatments.Last();
            var preventTreatment = preventTreatmentEntity.CreateSelectableTreatment(simulation);
            supersedeRule.Treatment = preventTreatment;            
            return supersedeRule;
        }

        public static Analysis.SelectableTreatment TreatmentSupersedeDomain(SimulationEntity simulationEntity, Simulation simulation)
        {
            var treatmentEntity = simulationEntity.SelectableTreatments.Last();
            var selectableTreatment = treatmentEntity.CreateSelectableTreatment(simulation);            
            return selectableTreatment;
        }
    }
}
