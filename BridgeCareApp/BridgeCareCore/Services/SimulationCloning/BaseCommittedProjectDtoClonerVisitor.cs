using System;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.CopyAnalysis;

namespace BridgeCareCore.Services.SimulationCloning
{
    public class BaseCommittedProjectDtoClonerVisitor : IBaseCommittedProjectDtoVisitor<BaseCommittedProjectDTO>
    {
        public BaseCommittedProjectDTO Visit(SectionCommittedProjectDTO dto)
        {
            var clone = new SectionCommittedProjectDTO
            {

            };
            CopyAbstractFields(dto, clone);
            return clone;
        }

        private void CopyAbstractFields(BaseCommittedProjectDTO dto, BaseCommittedProjectDTO clone)
        {            
            clone.Cost = dto.Cost;
            clone.Treatment = dto.Treatment;
            clone.ShadowForAnyTreatment = dto.ShadowForAnyTreatment;
            clone.ShadowForSameTreatment = dto.ShadowForSameTreatment;
            clone.Category = dto.Category;
            clone.Year = dto.Year;
            clone.SimulationId = dto.SimulationId;
            clone.ScenarioBudgetId = dto.ScenarioBudgetId;
            clone.Treatment = dto.Treatment;
            clone.Consequences = dto.Consequences;          
        }
        
    }
}
