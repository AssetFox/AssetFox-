using System;
using System.Collections.Generic;
using System.Text;

namespace AppliedResearchAssociates.iAM.DTOs.Abstract
{
    public interface IBaseCommittedProjectDtoVisitor<TOutput>
    {
        TOutput Visit(SectionCommittedProjectDTO dto);
    }
}
