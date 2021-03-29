using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpressionValidationController : HubControllerBase
    {
        private readonly ExpressionValidationService _expressionValidationService;

        public ExpressionValidationController(ExpressionValidationService expressionValidationService,
            IHubService hubService) : base(hubService) =>
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
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Expression Validation error::{e.Message}");
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
                _hubService.SendRealTimeMessage(HubConstant.BroadcastError, $"Expression Validation error::{e.Message}");
                throw;
            }
        }
    }
}
