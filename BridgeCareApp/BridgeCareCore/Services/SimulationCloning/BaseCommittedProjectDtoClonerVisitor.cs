using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;

namespace BridgeCareCore.Services
{
    public class BaseCommittedProjectDtoClonerVisitor : IBaseCommittedProjectDtoVisitor<Dictionary<Guid, Guid>, BaseCommittedProjectDTO>
    {
        public BaseCommittedProjectDTO Visit(SectionCommittedProjectDTO dto, Dictionary<Guid, Guid> helper)
        {
            var clone = new SectionCommittedProjectDTO
            {

            };
            CopyAbstractFields(dto, clone, helper);
            clone.Id = Guid.NewGuid();
            clone.LocationKeys["ID"] = Guid.NewGuid().ToString();
            return clone;
        }

        private void CopyAbstractFields(BaseCommittedProjectDTO dto, BaseCommittedProjectDTO clone, Dictionary<Guid, Guid> budgetIdMap)
        {
            clone.Cost = dto.Cost;
            clone.Treatment = dto.Treatment;
            clone.ShadowForAnyTreatment = dto.ShadowForAnyTreatment;
            clone.ShadowForSameTreatment = dto.ShadowForSameTreatment;
            clone.Category = dto.Category;
            clone.Year = dto.Year;
            clone.ProjectSource = dto.ProjectSource;
            clone.ProjectSourceId = dto.ProjectSourceId;
            clone.SimulationId = dto.SimulationId;
            if (dto.ScenarioBudgetId != null)
            {
                clone.ScenarioBudgetId = budgetIdMap[dto.ScenarioBudgetId.Value];
            }
            else
            {
                clone.ScenarioBudgetId = null;
            }
            clone.Treatment = dto.Treatment;
            clone.LocationKeys = new Dictionary<string, string>(dto.LocationKeys);
        }
        
    }
}
