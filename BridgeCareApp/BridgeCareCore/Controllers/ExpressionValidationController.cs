using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Security.Interfaces;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppliedResearchAssociates.CalculateEvaluate;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpressionValidationController : BridgeCareCoreBaseController
    {
        public const string ExpressionValidationError = "Expression Validation Error";

        private readonly IExpressionValidationService _expressionValidationService;

        public ExpressionValidationController(IExpressionValidationService expressionValidationService,
            IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService,
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
                if (e is CalculateEvaluateException)
                {
                    HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{ExpressionValidationError}::GetEquationValidationResult - {e.Message}", e);
                } else
                {
                    HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{ExpressionValidationError}::GetEquationValidationResult - {e.Message}", e);
                }
            }
            return Ok();
        }

        [HttpPost]
        [Route("GetCriterionValidationResult")]
        [Authorize]
        public async Task<IActionResult> GetCriterionValidationResult([FromBody] ValidationParameter model)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    _expressionValidationService.ValidateCriterion(model.Expression, model.CurrentUserCriteriaFilter, model.NetworkId));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{ExpressionValidationError}::GetCriterionValidationResult - {e.Message}", e);
            }
            return Ok();
        }

        [HttpPost]
        [Route("GetCriterionValidationResultNoCount")]
        [Authorize]
        public async Task<IActionResult> GetCriterionValidationResultNoCount([FromBody] ValidationParameter model)
        {
            try
            {
                var result = await Task.Factory.StartNew(() =>
                    _expressionValidationService.ValidateCriterionWithoutResults(model.Expression, model.CurrentUserCriteriaFilter));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{ExpressionValidationError}::GetCriterionValidationResultNoCount - {e.Message}", e);
            }
            return Ok();
        }
    }
}
