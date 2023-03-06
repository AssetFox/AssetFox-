using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using BridgeCareCore.Security;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController : BridgeCareCoreBaseController
    {
        public const string AttributeError = "Attribute Error";
        private readonly AttributeService _attributeService;
        private readonly AttributeImportService _attributeImportService;

        public AttributeController(AttributeService attributeService, AttributeImportService attributeImportService, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _attributeImportService = attributeImportService ?? throw new ArgumentNullException(nameof(attributeImportService));
        }

        [HttpGet]
        [Route("GetAttributes")]
        [ClaimAuthorize("AttributesViewAccess")]
        public async Task<IActionResult> Attributes()
        {
            try
            {
                var result = await UnitOfWork.AttributeRepo.GetAttributesAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::GetAttributes - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetAggregationRules")]
        [ClaimAuthorize("AttributesViewAccess")]
        public async Task<IActionResult> GetAggregationRules()
        {
            try
            {
                var result = await UnitOfWork.AttributeRepo.GetAggregationRules();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::GetAggregationRuleTypes - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetAttributeDataSourceTypes")]
        [ClaimAuthorize("AttributesViewAccess")]
        public async Task<IActionResult> GetAttributeDataSourceTypes()
        {
            try
            {
                var result = await UnitOfWork.AttributeRepo.GetAttributeDataSourceTypes();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::GetAttributeDataSourceTypes - {e.Message}");
                throw;
            }
        }
        

        [HttpPost]
        [Route("GetAttributesSelectValues")]
        [ClaimAuthorize("AttributesViewAccess")]
        public async Task<IActionResult> GetAttributeSelectValues([FromBody] List<string> attributeNames)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _attributeService.GetAttributeSelectValues(attributeNames));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::GetAttributesSelectValues - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CreateAttributes")]
        [Authorize (Policy = Policy.ModifyAttributes)]
        public async Task<IActionResult> CreateAttributes(List<AllAttributeDTO> attributeDTOs)
        {
            try
            {
                var convertedAttributes = attributeDTOs.Select(AttributeService.ConvertAllAttribute).ToList();
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AttributeRepo.UpsertAttributesAtomically(convertedAttributes);
                });

                return Ok();
            }
            catch (Exception e)
            {
                if (e is InvalidAttributeUpsertException)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::CreateAttributes - {e.Message}");
                }
                else if (e is InvalidAttributeException)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::CreateAttributes - {e.Message}");
                }
                else
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::CreateAttributes - {e.Message}");
                }
                throw;
            }
        }

        [HttpPost]
        [Route("CreateAttribute")]
        [Authorize(Policy = Policy.ModifyAttributes)]
        public async Task<IActionResult> CreateAttribute(AllAttributeDTO attributeDto)
        {
            try
            {
                var convertedAttributeDto = AttributeService.ConvertAllAttribute(attributeDto);
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AttributeRepo.UpsertAttributesAtomically(convertedAttributeDto);
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::CreateAttribute {attributeDto.Name} - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CheckCommand")]
        [Authorize(Policy = Policy.ModifyAttributes)]
        public async Task<IActionResult> CheckCommand(TestStringData sqlCommand)
        {
            try
            {
                IList<ParseError> errors = null; ;
                await Task.Factory.StartNew(() =>
                {
                    TSql100Parser parser = new TSql100Parser(false);

                    parser.Parse(new StringReader(sqlCommand.testString), out errors);
                });

                if(errors != null && errors.Count > 0)
                {
                    return Ok(new ValidationResult() { IsValid = false, ValidationMessage = "This sql command has the following error: " + errors.First().Message });
                }
                return Ok(new ValidationResult() { IsValid = true, ValidationMessage = "This sql command is valid"});
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::CheckCommand - {e.Message}");
                throw;
            }
        }
    }
}
