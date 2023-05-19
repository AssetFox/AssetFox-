using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BridgeCareCore.Security;
using Humanizer;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Reporting.Interfaces;
using AppliedResearchAssociates.iAM.Reporting;
using System.Linq;

namespace BridgeCareCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminDataController : BridgeCareCoreBaseController
    {
        public const string SiteError = "Site Error";
        private readonly IReportGenerator _generator;
        private readonly IReportLookupLibrary _factory;
        public AdminDataController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor, IReportGenerator generator, IReportLookupLibrary factory) :
                         base(esecSecurity, unitOfWork, hubService, contextAccessor)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }
        [HttpGet]
        [Route("GetKeyFields")]
        [Authorize]
        public async Task<IActionResult> GetKeyFields()
        {
            try
            {
                var KeyFields = UnitOfWork.AdminDataRepo.GetKeyFields();
                return Ok(KeyFields);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::GetKeyFields - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetKeyFields/{KeyFields}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetKeyFields(string KeyFields)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {                    
                    UnitOfWork.AdminDataRepo.SetKeyFields(KeyFields);
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetPrimaryNetwork - {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetPrimaryNetwork")]
        [Authorize]
        public async Task<IActionResult> GetPrimaryNetwork()
        {
            try
            {
                var name = UnitOfWork.AdminDataRepo.GetPrimaryNetwork();
                return Ok(name);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::GetPrimaryNetwork - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetPrimaryNetwork/{name}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetPrimaryNetwork(string name)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.AdminDataRepo.SetPrimaryNetwork(name);
                });
                    return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetPrimaryNetwork - {e.Message}");
                return BadRequest($"{SiteError}::SetPrimaryNetwork - {e.Message}");
            }
        }

        [HttpGet]
        [Route("GetSimulationReportNames")]
        [Authorize]
        public async Task<IActionResult> GetSimulationReportNames()
        {
            try
            {
                var SimulationReportNames = UnitOfWork.AdminDataRepo.GetSimulationReportNames();
                return Ok(SimulationReportNames);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::GetSimulationReportNames - {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("SetInventoryReports/{InventoryReports}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetInventoryReports(string inventoryReports)
        {
            try
            {
                var reportCriteriaCheck = true;
                IList<string> InventoryReportsList = inventoryReports.Split(',').ToList();
                   
                //Checking every report being passed in from the parameter
                foreach (string inventoryReport in InventoryReportsList)
                {
                    try
                    {
                        var reportObject = await _generator.Generate(inventoryReport);
                        //If cannot be created in lookup library (Existence Check)
                        if (!_factory.CanGenerateReport(inventoryReport))
                        {
                            reportCriteriaCheck = false;
                            throw new InvalidOperationException($"You can't use {inventoryReport} for an inventory report.");
                        }
                        //Report type isn't HTML
                        else if (reportObject.Type != ReportType.HTML)
                        {
                            reportCriteriaCheck = false;
                            throw new InvalidOperationException($"You can't use {inventoryReport} for an inventory report.");
                        }                       
                    }
                    catch (Exception e)
                    {
                        HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetInventoryReports - {e.Message}");
                        return BadRequest($"{SiteError}::SetInventoryReports - {e.Message}");
                    }
                    
                };
                //If all reports in list exist and use the right type, save to database.
                if (reportCriteriaCheck)
                {
                    UnitOfWork.AdminDataRepo.SetInventoryReports(inventoryReports);
                }
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetInventoryReports - {e.Message}");
                return BadRequest($"{SiteError}::SetInventoryReports - {e.Message}");
            }
        }

    }
}
