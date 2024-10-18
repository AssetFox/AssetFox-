﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Models;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController : BridgeCareCoreBaseController
    {
        public const string AttributeError = "Attribute Error";
        private readonly AttributeService _attributeService;

        public AttributeController(AttributeService attributeService, IEsecSecurity esecSecurity, UnitOfDataPersistenceWork unitOfWork,
            IHubService hubService, IHttpContextAccessor httpContextAccessor) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AttributeError}::GetAttributes - {e.Message}", e);
                return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AttributeError}::GetAggregationRuleTypes - {e.Message}", e);
                return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AttributeError}::GetAttributeDataSourceTypes - {e.Message}", e);
                return Ok();
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AttributeError}::GetAttributesSelectValues - {e.Message}", e);
                return Ok();
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
                convertedAttributes.ForEach(_ => checkAttributeNameValidity(_));
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AttributeRepo.UpsertAttributes(convertedAttributes);
                });

                return Ok();
            }
            catch (Exception e)
            {
                if (e is InvalidAttributeUpsertException)
                {
                    HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AttributeError}::CreateAttributes - {e.Message}", e);
                }
                else if (e is InvalidAttributeException)
                {
                    HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AttributeError}::CreateAttributes - {e.Message}", e);
                }
                else
                {
                    HubService.SendRealTimeErrorMessage(UserInfo.Name,  $"{AttributeError}::CreateAttributes - {e.Message}", e);
                }
                return Ok();
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
                checkAttributeNameValidity(convertedAttributeDto);
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AttributeRepo.UpsertAttributes(convertedAttributeDto);
                });

                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AttributeError}::CreateAttribute - {e.Message}", e);
                return Ok();
            }
        }

        private void checkAttributeNameValidity(AttributeDTO attr)
        {
            if (attr.Name == null || !AppliedResearchAssociates.iAM.Analysis.Attribute.NamePattern.IsMatch(attr.Name))
            {
                throw new MalformedInputException($"Invalid name {attr.Name}. A valid attribute name must be alphanumeric and have no spaces");
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
                HubService.SendRealTimeErrorMessage(UserInfo.Name, $"{AttributeError}::CheckCommand - {e.Message}", e);
                return Ok();
            }
        }
    }
}
