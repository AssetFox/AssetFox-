using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{
    public class AggregatedSelectValuesResultDtoCacheEntry
    {
        public AggregatedSelectValuesResultDTO Dto { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
