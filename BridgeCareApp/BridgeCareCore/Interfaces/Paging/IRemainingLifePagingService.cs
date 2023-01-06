using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;
using System.Collections.Generic;
using System;
using BridgeCareCore.Services.Paging.Generics;

namespace BridgeCareCore.Interfaces
{
    public interface IRemainingLifeLimitPagingService : IPagingService<RemainingLifeLimitDTO, RemainingLifeLimitLibraryDTO>
    {
    }
}
