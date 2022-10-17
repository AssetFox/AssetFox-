﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Abstract;
using BridgeCareCore.Controllers.BaseController;
using AppliedResearchAssociates.iAM.Hubs;
using AppliedResearchAssociates.iAM.Hubs.Interfaces;
using BridgeCareCore.Interfaces;
using BridgeCareCore.Interfaces.DefaultData;
using BridgeCareCore.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using BridgeCareCore.Models;
using BridgeCareCore.Utils.Interfaces;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;
using MoreLinq.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.Generics;

namespace BridgeCareCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestmentController : BridgeCareCoreBaseController
    {
        private static IInvestmentBudgetsService _investmentBudgetsService;
        public readonly IInvestmentDefaultDataService _investmentDefaultDataService;
        private readonly IClaimHelper _claimHelper;
        public const string RequestedToModifyNonexistentLibraryErrorMessage = "The request says to modify a library, but the library does not exist.";
        public const string RequestedToCreateExistingLibraryErrorMessage = "The request says to create a new library, but the library already exists.";

        private Guid UserId => UnitOfWork.CurrentUser?.Id ?? Guid.Empty;

        public InvestmentController(IInvestmentBudgetsService investmentBudgetsService, IEsecSecurity esecSecurity, IUnitOfWork unitOfWork, IHubService hubService, IHttpContextAccessor httpContextAccessor, IInvestmentDefaultDataService investmentDefaultDataService, IClaimHelper claimHelper) : base(esecSecurity, unitOfWork, hubService, httpContextAccessor)
        {
            _investmentBudgetsService = investmentBudgetsService ?? throw new ArgumentNullException(nameof(investmentBudgetsService));
            _investmentDefaultDataService = investmentDefaultDataService ?? throw new ArgumentNullException(nameof(investmentDefaultDataService));
            _claimHelper = claimHelper ?? throw new ArgumentNullException(nameof(claimHelper));
        }

        [HttpPost]
        [Route("GetScenarioInvestmentPage/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> GetScenarioInvestmentPage(Guid simulationId, InvestmentPagingRequestModel pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _investmentBudgetsService.GetScenarioInvestmentPage(simulationId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("GetLibraryInvestmentPage/{libraryId}")]
        [Authorize]
        public async Task<IActionResult> GetLibraryInvestmentPage(Guid libraryId, InvestmentPagingRequestModel pageRequest)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => _investmentBudgetsService.GetLibraryInvestmentPage(libraryId, pageRequest));
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Deterioration model error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetInvestment/{simulationId}")]
        [Authorize(Policy = Policy.ViewInvestmentFromScenario)]
        public async Task<IActionResult> GetInvestment(Guid simulationId)
        {
            try
            {
                var result = new InvestmentDTO();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    result = GetForScenario(simulationId);
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }
                
        [HttpPost]
        [Route("UpsertInvestment/{simulationId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromScenario)]
        public async Task<IActionResult> UpsertInvestment(Guid simulationId, InvestmentPagingSyncModel pagingSync)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);

                    var dtos = _investmentBudgetsService.GetSyncedInvestmentDataset(simulationId, pagingSync);
                    InvestmentDTO investment = new InvestmentDTO();
                    var investmentPlan = pagingSync.Investment;
                    investment.ScenarioBudgets = dtos;
                    investment.InvestmentPlan = investmentPlan;

                    UnitOfWork.BudgetRepo.UpsertOrDeleteScenarioBudgets(dtos, simulationId);
                    UnitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(investmentPlan, simulationId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetBudgetLibraries")]
        [Authorize(Policy = Policy.ViewInvestmentFromLibrary)]
        public async Task<IActionResult> GetBudgetLibraries()
        {
            try
            {
                var result = new List<BudgetLibraryDTO>();
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        result = UnitOfWork.BudgetRepo.GetBudgetLibrariesNoChildrenAccessibleToUser(UserId);
                    } else
                    {
                        result = UnitOfWork.BudgetRepo.GetBudgetLibrariesNoChildren();
                    }
                });

                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpsertBudgetLibrary")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> UpsertBudgetLibrary(InvestmentLibraryUpsertPagingRequestModel upsertRequest)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    var libraryAccess = UnitOfWork.BudgetRepo.GetLibraryAccess(upsertRequest.Library.Id, UserId);
                    if (libraryAccess.LibraryExists == upsertRequest.IsNewLibrary)
                    {
                        var errorMessage = libraryAccess.LibraryExists ? RequestedToCreateExistingLibraryErrorMessage : RequestedToModifyNonexistentLibraryErrorMessage;
                        throw new InvalidOperationException(errorMessage);
                    }
                    _claimHelper.CheckUserLibraryModifyAuthorization(libraryAccess, UserId);
                    var budgets = new List<BudgetDTO>();
                    if (upsertRequest.PagingSync.LibraryId != null)
                        budgets = _investmentBudgetsService.GetSyncedLibraryDataset(upsertRequest.PagingSync.LibraryId.Value, upsertRequest.PagingSync);
                    else if (!upsertRequest.IsNewLibrary)
                        budgets = _investmentBudgetsService.GetSyncedLibraryDataset(upsertRequest.Library.Id, upsertRequest.PagingSync);
                    if (upsertRequest.PagingSync.LibraryId != null && upsertRequest.PagingSync.LibraryId != upsertRequest.Library.Id)
                        budgets.ForEach(budget =>
                        {
                            budget.Id = Guid.NewGuid();
                            budget.BudgetAmounts.ForEach(_ => _.Id = Guid.NewGuid());
                        });
                    var dto = upsertRequest.Library;
                    dto.Budgets = budgets;
                    bool canModifyAccessLevels = false;
                    if (libraryAccess.LibraryExists)
                    {
                        canModifyAccessLevels = _claimHelper.CanModifyAccessLevels(libraryAccess, UserId);
                    } else
                    {
                        LibraryUserDtolistUpdater.GrantOwnerAccess(UserId, dto.Users);
                        canModifyAccessLevels = true;
                    }

                    UnitOfWork.BudgetRepo.UpsertBudgetLibrary(dto, canModifyAccessLevels);
                    UnitOfWork.BudgetRepo.UpsertOrDeleteBudgets(dto.Budgets, dto.Id);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                // WjJake or Amruta -- if unauthorized, we don't do anything to close out the transaction?
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("DeleteBudgetLibrary/{libraryId}")]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> DeleteBudgetLibrary(Guid libraryId)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    UnitOfWork.BeginTransaction();
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var access = UnitOfWork.BudgetRepo.GetLibraryAccess(libraryId, UserId);
                        // There was an existence check here where we just returned if the library did not exist. WJ deleted it because
                        // the new claim helper code checks for that. But is that the right approach?
                        _claimHelper.CheckUserLibraryDeleteAuthorization(access, UserId);
                    }
                    UnitOfWork.BudgetRepo.DeleteBudgetLibrary(libraryId);
                    UnitOfWork.Commit();
                });

                return Ok();
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetScenarioSimpleBudgetDetails/{simulationId}")]
        [Authorize(Policy = Policy.ViewInvestmentFromScenario)]
        public async Task<IActionResult> GetScenarioSimpleBudgetDetails(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    return UnitOfWork.BudgetRepo.GetScenarioSimpleBudgetDetails(simulationId);
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetInvestmentPlan/{simulationId}")]
        [Authorize(Policy = Policy.ViewInvestmentFromScenario)]
        public async Task<IActionResult> GetInvestmentPlan(Guid simulationId)
        {
            try
            {
                var result = await Task.Factory.StartNew(() => {
                    _claimHelper.CheckUserSimulationReadAuthorization(simulationId, UserId);
                    return UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(simulationId);
                });
                return Ok(result);
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportLibraryInvestmentBudgetsExcelFile")]
        [Authorize(Policy = Policy.ImportInvestmentFromLibrary)]
        public async Task<IActionResult> ImportLibraryInvestmentBudgetsExcelFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Investment budgets file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("libraryId", out var libraryId))
                {
                    throw new ConstraintException("Request contained no budget library id.");
                }

                var budgetLibraryId = Guid.Parse(libraryId.ToString());
                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());
                var overwriteBudgets = false;
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("overwriteBudgets"))
                {
                    overwriteBudgets = ContextAccessor.HttpContext.Request.Form["overwriteBudgets"].ToString() == "1";
                }

                var currentUserCriteriaFilter = new UserCriteriaDTO
                {
                    HasCriteria = false
                };
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("currentUserCriteriaFilter"))
                {
                    currentUserCriteriaFilter =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<UserCriteriaDTO>(
                            ContextAccessor.HttpContext.Request.Form["currentUserCriteriaFilter"]);
                }

                var result = new BudgetImportResultDTO();
                await Task.Factory.StartNew(() =>
                {
                    if (_claimHelper.RequirePermittedCheck())
                    {
                        var existingBudgetLibrary = UnitOfWork.BudgetRepo.GetBudgetLibraries().FirstOrDefault(_ => _.Id == libraryId);
                        if (existingBudgetLibrary != null)
                        {
                            var accessModel = UnitOfWork.BudgetRepo.GetLibraryAccess(budgetLibraryId, UserId);
                            _claimHelper.CheckUserLibraryRecreateAuthorization(accessModel, UserId);
                        }
                    }
                    result = _investmentBudgetsService.ImportLibraryInvestmentBudgetsFile(budgetLibraryId, excelPackage, currentUserCriteriaFilter, overwriteBudgets);
                });

                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result.BudgetLibrary);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("ImportScenarioInvestmentBudgetsExcelFile")]
        [Authorize(Policy = Policy.ImportInvestmentFromScenario)]
        public async Task<IActionResult> ImportScenarioInvestmentBudgetsExcelFile()
        {
            try
            {
                if (!ContextAccessor.HttpContext.Request.HasFormContentType)
                {
                    throw new ConstraintException("Request MIME type is invalid.");
                }

                if (ContextAccessor.HttpContext.Request.Form.Files.Count < 1)
                {
                    throw new ConstraintException("Investment budgets file not found.");
                }

                if (!ContextAccessor.HttpContext.Request.Form.TryGetValue("simulationId", out var id))
                {
                    throw new ConstraintException("Request contained no simulation id.");
                }

                var simulationId = Guid.Parse(id.ToString());

                var excelPackage = new ExcelPackage(ContextAccessor.HttpContext.Request.Form.Files[0].OpenReadStream());

                var overwriteBudgets = false;
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("overwriteBudgets"))
                {
                    overwriteBudgets = ContextAccessor.HttpContext.Request.Form["overwriteBudgets"].ToString() == "1";
                }

                var currentUserCriteriaFilter = new UserCriteriaDTO
                {
                    HasCriteria = false
                };
                if (ContextAccessor.HttpContext.Request.Form.ContainsKey("currentUserCriteriaFilter"))
                {
                    currentUserCriteriaFilter =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<UserCriteriaDTO>(
                            ContextAccessor.HttpContext.Request.Form["currentUserCriteriaFilter"]);
                }

                var result = new ScenarioBudgetImportResultDTO();
                await Task.Factory.StartNew(() =>
                {
                    _claimHelper.CheckUserSimulationModifyAuthorization(simulationId, UserId);
                    result = _investmentBudgetsService.ImportScenarioInvestmentBudgetsFile(simulationId, excelPackage, currentUserCriteriaFilter, overwriteBudgets);
                });

                if (result.WarningMessage != null)
                {
                    HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastWarning, result.WarningMessage);
                }
                return Ok(result.Budgets);
            }
            catch (UnauthorizedAccessException e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                return Ok();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportScenarioInvestmentBudgetsExcelFile/{simulationId}")]
        [Authorize]
        public async Task<IActionResult> ExportScenarioInvestmentBudgetsExcelFile(Guid simulationId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _investmentBudgetsService.ExportScenarioInvestmentBudgetsFile(simulationId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("ExportLibraryInvestmentBudgetsExcelFile/{budgetLibraryId}")]
        [Authorize]
        public async Task<IActionResult> ExportLibraryInvestmentBudgetsExcelFile(Guid budgetLibraryId)
        {
            try
            {
                var result =
                    await Task.Factory.StartNew(() => _investmentBudgetsService.ExportLibraryInvestmentBudgetsFile(budgetLibraryId));

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("DownloadInvestmentBudgetsTemplate")]
        [Authorize]
        public async Task<IActionResult> DownloadInvestmentBudgetsTemplate()
        {
            try
            {
                var filePath = AppDomain.CurrentDomain.BaseDirectory + "DownloadTemplates\\Investment_budgets_template.xlsx";
                var fileData = System.IO.File.ReadAllBytes(filePath);
                var result = await Task.Factory.StartNew(() => new FileInfoDTO
                {
                    FileName = "Investment_budgets_template",
                    FileData = Convert.ToBase64String(fileData),
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                HubService.SendRealTimeMessage(UserInfo.Name, HubConstant.BroadcastError, $"Investment error::{e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetHasPermittedAccess")]
        [Authorize]
        [Authorize(Policy = Policy.ModifyInvestmentFromLibrary)]
        public async Task<IActionResult> GetHasPermittedAccess()
        {
            return Ok(true);
        }

        private InvestmentDTO GetForScenario(Guid scenarioId)
        {
            var budgets = UnitOfWork.BudgetRepo.GetScenarioBudgets(scenarioId);

            var scenarioInvestmentPlan = UnitOfWork.InvestmentPlanRepo.GetInvestmentPlan(scenarioId);
            if (scenarioInvestmentPlan.Id == Guid.Empty)
            {
                var investmentDefaultData = _investmentDefaultDataService.GetInvestmentDefaultData().Result;
                scenarioInvestmentPlan.MinimumProjectCostLimit = investmentDefaultData.MinimumProjectCostLimit;
                scenarioInvestmentPlan.InflationRatePercentage = investmentDefaultData.InflationRatePercentage;
            }

            return new InvestmentDTO
            {
                ScenarioBudgets = budgets,
                InvestmentPlan = scenarioInvestmentPlan
            };
        }
    }
}
