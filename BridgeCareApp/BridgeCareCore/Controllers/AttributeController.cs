using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using BridgeCareCore.Hubs;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Models;
using BridgeCareCore.Models.Validation;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using OfficeOpenXml;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeController : BridgeCareCoreBaseController
    {
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
        [Authorize]
        public async Task<IActionResult> Attributes()
        {
            try
            {
                var result = await UnitOfWork.AttributeRepo.GetAttributesAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetAggregationRuleTypes")]
        [Authorize]
        public async Task<IActionResult> GetAggregationRuleTypes()
        {
            try
            {
                var result = await UnitOfWork.AttributeRepo.GetAggregationRuleTypes();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetAttributeDataSourceTypes")]
        [Authorize]
        public async Task<IActionResult> GetAttributeDataSourceTypes()
        {
            try
            {
                var result = await UnitOfWork.AttributeRepo.GetAttributeDataSourceTypes();
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }
        

        [HttpPost]
        [Route("GetAttributesSelectValues")]
        [Authorize]
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CreateAttributes")]
        [Authorize]
        public async Task<IActionResult> CreateAttributes(List<AttributeDTO> attributeDTOs)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.AttributeRepo.UpsertAttributes(attributeDTOs);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CreateAttribute")]
        [Authorize]
        public async Task<IActionResult> CreateAttribute(AllAttributeDTO attributeDto)
        {
            try
            {
                var convertedAttributeDto = AttributeService.ConvertAllAttribute(attributeDto);
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    UnitOfWork.AttributeRepo.UpsertAttributes(convertedAttributeDto);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CheckCommand/{sqlCommand}")]
        [Authorize]
        public async Task<IActionResult> CheckCommand(string sqlCommand)
        {
            try
            {
                IList<ParseError> errors = null; ;
                await Task.Factory.StartNew(() =>
                {
                    TSql100Parser parser = new TSql100Parser(false);

                    parser.Parse(new StringReader(sqlCommand), out errors);
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Attribute error::{e.Message}");
                throw;
            }
        }

        // Wjwjwj commented out 6/20 8am while working on attribute import.
        // I'm guessing in the new world this will no longer exist?
        //[HttpPost]
        //[Route("ImportAttributesExcelFile")]
        //[Authorize]
        //public async Task<IActionResult> ImportAttributesExcelFile(
        //    string keyColumnName,
        //    string inspectionDateColumnName,
        //    string spatialWeighting)
        //{
        //    try
        //    {
        //        if (!ContextAccessor.HttpContext.Request.HasFormContentType)
        //        {
        //            throw new ConstraintException("Request MIME type is invalid.");
        //        }

        //        if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
        //        {
        //            throw new ConstraintException("Attributes file not found.");
        //        }

        //        var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());

        //        var result = await Task.Factory.StartNew(() =>
        //        {
        //            return _attributeImportService.ImportExcelAttributes(keyColumnName, inspectionDateColumnName, spatialWeighting, excelPackage);
        //        });

        //        if (result.WarningMessage != null)
        //        {
        //            HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
        //        }
        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
        //        throw;
        //    }
        //}
    }
}
