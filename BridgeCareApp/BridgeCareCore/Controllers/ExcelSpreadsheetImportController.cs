using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BridgeCareCore.Controllers
{
    public class ExcelSpreadsheetImportController : BridgeCareCoreBaseController
    {
        public ExcelSpreadsheetImportController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor) : base(esecSecurity, unitOfWork, hubService, contextAccessor)
        {
        }
    }
}
