﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
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
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;

namespace BridgeCareCore.Controllers
{
    public class CreateAttributeRequest
    {
        public AllAttributeDTO Attribute { get; set; }
        public bool SetForAllAttributes { get; set; }
    }

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
        public async Task<IActionResult> CreateAttribute(CreateAttributeRequest request)
        {
            try
            {
                var attributeDto = request.Attribute;
                var setForAllAttributes = request.SetForAllAttributes;
                var convertedAttributeDto = AttributeService.ConvertAllAttribute(attributeDto);
                checkAttributeNameValidity(convertedAttributeDto);

                if (!setForAllAttributes)
                {
                    await Task.Factory.StartNew(() =>
                    {
                        UnitOfWork.AttributeRepo.UpsertAttributes(convertedAttributeDto);
                    });
                }
                else
                {
                    var targetAttributeDTOs = UnitOfWork.AttributeRepo.GetAttributes();
                    var dataSourceToBeCopied = convertedAttributeDto.DataSource;

                    // Swapping out the attribute that is being updated via the API, since we are upserting the whole list of attributes
                    var existingIndex = targetAttributeDTOs.FindIndex(attr => attr.Id == convertedAttributeDto.Id);
                    if (existingIndex != -1)
                    {
                        // Perform a complete swap of the attribute at the found index
                        targetAttributeDTOs[existingIndex] = convertedAttributeDto;
                    }
                    else
                    {
                        targetAttributeDTOs.Add(convertedAttributeDto);
                    }

                    targetAttributeDTOs.ForEach(_ => _.DataSource = dataSourceToBeCopied);

                    targetAttributeDTOs.ForEach(_ => checkAttributeNameValidity(_));
                    await Task.Factory.StartNew(() =>
                    {
                        UnitOfWork.AttributeRepo.UpsertAttributes(targetAttributeDTOs);
                    });
                }
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::CreateAttribute {request.Attribute?.Name} - {e.Message}");
                throw;
            }
        }

        private void checkColumnNames(AttributeDTO attributeToCheck)
        {
            /*if (attributeToCheck.Command != referenceAttribute.Command)
            {
                throw new MalformedInputException($"Invalid column name {attributeToCheck.Command}. " +
                    $"To set a data source for all attributes, the column names must match.");
            }*/
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{AttributeError}::CheckCommand - {e.Message}");
                throw;
            }
        }
    }
}
