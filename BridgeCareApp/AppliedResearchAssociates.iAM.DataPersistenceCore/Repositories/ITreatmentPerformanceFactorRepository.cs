﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ITreatmentPerformanceFactorRepository
    {
       void UpsertOrDeleteScenarioTreatmentPerformanceFactors(Dictionary<Guid, List<TreatmentPerformanceFactorDTO>> scenarioTreatmentPerformanceFactorPerTreatmentId,
           Guid SimulationId);
    }
}
