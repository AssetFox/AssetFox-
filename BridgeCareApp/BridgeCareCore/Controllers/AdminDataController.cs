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
        public AdminDataController(IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor contextAccessor, IReportGenerator generator) :
                         base(esecSecurity, unitOfWork, hubService, contextAccessor)
        {
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
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::GetPrimaryNetwork - {e.Message}");
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

        [HttpPost]
        [Route("SetInventoryReports/{InventoryReports}")]
        [ClaimAuthorize("AdminAccess")]
        public async Task<IActionResult> SetInventoryReports(string inventoryReports)
        {
            try
            {
                await Task.Factory.StartNew(async () =>
                {

                    //For verifying Inventory Reports
                    var reportCriteriaCheck = false;
                    var reportFactoryList = new List<IReportFactory>();
                    reportFactoryList.Add(new BAMSInventoryReportFactory());
                    reportFactoryList.Add(new PAMSInventorySectionsReportFactory());
                    reportFactoryList.Add(new PAMSInventorySegmentsReportFactory());
                    ReportLookupLibrary library = new ReportLookupLibrary(reportFactoryList);
                    IList<string> InventoryReportsList = inventoryReports.Split(',').ToList();

                    //Checking every report being passed in from the parameter
                    foreach (string inventoryReport in InventoryReportsList)
                    {
                        var reportObject = await _generator.Generate(inventoryReport);
                        //If report is in factory list
                        if (library.CanGenerateReport(inventoryReport))
                        {
                            reportCriteriaCheck = true;
                        }
                        else if (!library.CanGenerateReport(inventoryReport))
                        {
                            reportCriteriaCheck = false;
                            throw new Exception("Report Type Does Not Exist.");
                        }
                        else if (reportObject.Type != ReportType.HTML)
                        {
                            reportCriteriaCheck = false;
                            throw new Exception("You can't use this particular report for an inventory report");
                        }

                        //If all reports in list exist, save to database.

                    };
                    if (reportCriteriaCheck)
                    {
                        UnitOfWork.AdminDataRepo.SetInventoryReports(inventoryReports);
                    }
                });
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"{SiteError}::SetPrimaryNetwork - {e.Message}");
                return BadRequest($"{SiteError}::SetInventoryReports - {e.Message}");
            }
        }

    }
}
