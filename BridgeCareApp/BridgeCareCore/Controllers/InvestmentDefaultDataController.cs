using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentDefaultDataController : BridgeCareCoreBaseController
    {
        public readonly IInvestmentDefaultDataService _InvestmentDefaultDataService;

        public InvestmentDefaultDataController(IInvestmentDefaultDataService InvestmentDefaultDataService, IEsecSecurity esecSecurity,
            UnitOfDataPersistenceWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor) :
            base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _InvestmentDefaultDataService = InvestmentDefaultDataService ?? throw new ArgumentNullException(nameof(InvestmentDefaultDataService));

        [HttpGet]
        [Route("GetInvestmentDefaultData")]
        [Authorize]
        public async Task<IActionResult> GetInvestmentDefaultData()
        {
            try
            {
                var result = await _InvestmentDefaultDataService.GetInvestmentDefaultData();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment Default Data Error::GetInvestmentDefaultData - {e.Message}");
                throw;
            }
        }
    }
}
