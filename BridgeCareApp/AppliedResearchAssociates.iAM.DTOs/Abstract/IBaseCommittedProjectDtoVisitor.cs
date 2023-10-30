using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    public interface IBaseCommittedProjectDtoVisitor<THelper, TOutput>
    {
        TOutput Visit(SectionCommittedProjectDTO dto, THelper helper);
    }
}
