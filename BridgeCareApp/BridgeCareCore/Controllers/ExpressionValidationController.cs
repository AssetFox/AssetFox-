using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpressionValidationController : BridgeCareCoreBaseController
    {
        private readonly IExpressionValidationService _expressionValidationService;

        public ExpressionValidationController(IExpressionValidationService expressionValidationService,
            IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork, IHubService hubService,
            IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor) =>
            _expressionValidationService = expressionValidationService ??
                                           throw new ArgumentNullException(nameof(expressionValidationService));

        [HttpPost]
        [Route("GetEquationValidationResult")]
        [Authorize]
        public async Task<IActionResult> GetEquationValidationResult([FromBody] EquationValidationParameters model)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _expressionValidationService.ValidateEquation(model));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Expression Validation error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetCriterionValidationResult")]
        [Authorize]
        public async Task<IActionResult> GetCriterionValidationResult([FromBody] ValidationParameter model)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    _expressionValidationService.ValidateCriterion(model.Expression, model.CurrentUserCriteriaFilter));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Expression Validation error::{e.Message}");
                throw;
            }
        }
    }
}
