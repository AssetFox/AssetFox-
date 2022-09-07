

using System.Security.Claims;
using AppliedResearchAssociates.iAM.DataPersistenceCore;
using BridgeCareCore.Security;
using BridgeCareCore.Security.Interfaces;
using BridgeCareCore.Utils;
using BridgeCareCore.Utils.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Policy = BridgeCareCore.Security.SecurityConstants.Policy;

namespace BridgeCareCore.StartupExtension
{
    public static class Security
    {
        public static void AddSecurityConfig(this IServiceCollection services, IConfiguration Configuration)
        {
            var securityType = Configuration.GetSection("SecurityType").Value;

            if (securityType == SecurityConstants.SecurityTypes.Esec)
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            RequireExpirationTime = true,
                            RequireSignedTokens = true,
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = true,
                            IssuerSigningKey = SecurityFunctions.GetPublicKey(Configuration.GetSection("EsecConfig"))
                        };
                    });
            }

            if (securityType == SecurityConstants.SecurityTypes.B2C)
            {
                services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                    .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
            }

            services.AddAuthorization(options =>
            {
                // TODO remove below 2 later
                options.AddPolicy(Policy.AdminOrDistrictEngineer,
                    policy => policy.Requirements.Add(
                        new UserHasAllowedRoleRequirement(Role.Administrator, Role.DistrictEngineer)));
                options.AddPolicy(Policy.Admin,
                    policy => policy.Requirements.Add(
                        new UserHasAllowedRoleRequirement(Role.Administrator)));
                ////


                // Deficient Condition Goal
                options.AddPolicy(Policy.ViewDeficientConditionGoalFromlLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalViewPermittedFromLibraryAccess", "DeficientConditionGoalViewAnyFromLibraryAccess"));
                options.AddPolicy(Policy.ViewDeficientConditionGoalFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalViewAnyFromScenarioAccess", "DeficientConditionGoalViewPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.ModifyDeficientConditionGoalFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalModifyAnyFromLibraryAccess", "DeficientConditionGoalModifyPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ModifyDeficientConditionGoalFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalModifyAnyFromScenarioAccess", "DeficientConditionGoalModifyPermittedFromScenarioAccess"));

                //Investment
                options.AddPolicy(Policy.ViewInvestmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentViewAnyFromScenarioAccess", "InvestmentViewPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.ViewInvestmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentViewAnyFromLibraryAccess", "InvestmentViewPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ModifyInvestmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentModifyAnyFromScenarioAccess", "InvestmentModifyPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.ModifyInvestmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentModifyAnyFromLibraryAccess", "InvestmentModifyPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ImportInvestmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentImportAnyFromLibraryAccess", "InvestmentImportPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ImportInvestmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentImportAnyFromScenarioAccess", "InvestmentImportPermittedFromScenarioAccess"));

                // Performance Curve
                options.AddPolicy(Policy.ViewPerformanceCurveFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveViewAnyFromLibraryAccess", "PerformanceCurveViewPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ViewPerformanceCurveFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveViewAnyFromScenarioAccess", "PerformanceCurveViewPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.ModifyPerformanceCurveFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveAddAnyFromLibraryAccess",
                                                                   "PerformanceCurveUpdateAnyFromLibraryAccess",
                                                                   "PerformanceCurveAddPermittedFromLibraryAccess",
                                                                   "PerformanceCurveUpdatePermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ModifyPerformanceCurveFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveModifyAnyFromScenarioAccess", "PerformanceCurveModifyPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.DeletePerformanceCurveFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveDeleteAnyFromLibraryAccess", "PerformanceCurveDeletePermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ImportPerformanceCurveFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveImportAnyFromLibraryAccess", "PerformanceCurveImportPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ImportPerformanceCurveFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveImportAnyFromScenarioAccess", "PerformanceCurveImportPermittedFromScenarioAccess"));

                //  Reamining Life Limit
                options.AddPolicy(Policy.ViewRemainingLifeLimitFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitViewAnyFromLibraryAccess", "RemainingLifeLimitViewPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ViewRemainingLifeLimitFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitViewAnyFromScenarioAccess", "RemainingLifeLimitViewPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.ModifyRemainingLifeLimitFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitAddAnyFromLibraryAccess",
                                                                   "RemainingLifeLimitUpdateAnyFromLibraryAccess",
                                                                   "RemainingLifeLimitAddPermittedFromLibraryAccess",
                                                                   "RemainingLifeLimitUpdatePermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ModifyRemainingLifeLimitFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitModifyAnyFromScenarioAccess", "RemainingLifeLimitModifyPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.DeleteRemainingLifeLimitFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitDeleteAnyFromLibraryAccess", "RemainingLifeLimitDeletePermittedFromLibraryAccess"));

                // Target Condition Goal
                options.AddPolicy(Policy.ViewTargetConditionGoalFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalViewAnyFromLibraryAccess", "TargetConditionGoalViewPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ViewTargetConditionGoalFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalViewAnyFromScenarioAccess", "TargetConditionGoalViewPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.ModifyTargetConditionGoalFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalAddAnyFromLibraryAccess",
                                                                   "TargetConditionGoalUpdateAnyFromLibraryAccess",
                                                                   "TargetConditionGoalAddPermittedFromLibraryAccess",
                                                                   "TargetConditionGoalUpdatePermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ModifyTargetConditionGoalFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalModifyAnyFromScenarioAccess", "TargetConditionGoalModifyPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.DeleteTargetConditionGoalFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalDeleteAnyFromLibraryAccess", "TargetConditionGoalDeletePermittedFromLibraryAccess"));

                // Treatment
                options.AddPolicy(Policy.ViewTreatmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentViewAnyFromLibraryAccess", "TreatmentViewPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ViewTreatmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentViewAnyFromScenarioAccess", "TreatmentViewPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.ModifyTreatmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentAddAnyFromLibraryAccess",
                                                                   "TreatmentUpdateAnyFromLibraryAccess",
                                                                   "TreatmentAddPermittedFromLibraryAccess",
                                                                   "TreatmentUpdatePermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ModifyTreatmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentModifyAnyFromScenarioAccess",
                                                                   "TreatmentModifyPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.DeleteTreatmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentDeleteAnyFromLibraryAccess", "TreatmentDeletePermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ImportTreatmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentImportAnyFromLibraryAccess", "TreatmentImportPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ImportTreatmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentImportAnyFromScenarioAccess", "TreatmentImportPermittedFromScenarioAccess"));

                // Analysis Method
                options.AddPolicy(Policy.ViewAnalysisMethod,
                    policy => policy.RequireClaim(ClaimTypes.Name, "AnalysisMethodViewAnyAccess", "AnalysisMethodViewPermittedAccess"));
                options.AddPolicy(Policy.ModifyAnalysisMethod,
                    policy => policy.RequireClaim(ClaimTypes.Name, "AnalysisMethodModifyAnyAccess", "AnalysisMethodModifyPermittedAccess"));

                // Attributes
                options.AddPolicy(Policy.ModifyAttributes, policy => policy.RequireClaim(ClaimTypes.Name, "AttributesAddAccess", "AttributesUpdateAccess"));

                // Simulation
                options.AddPolicy(Policy.ViewSimulation,
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationViewPermittedAccess", "SimulationViewAnyAccess"));
                options.AddPolicy(Policy.DeleteSimulation,
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationDeletePermittedAccess", "SimulationDeleteAnyAccess"));
                options.AddPolicy(Policy.UpdateSimulation,
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationUpdatePermittedAccess", "SimulationUpdateAnyAccess"));
                options.AddPolicy(Policy.RunSimulation,
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationRunPermittedAccess", "SimulationRunAnyAccess"));

                // Budget Priority
                options.AddPolicy(Policy.ViewBudgetPriorityFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityViewAnyFromLibraryAccess", "BudgetPriorityViewPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ModifyBudgetPriorityFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityUpdateAnyFromLibraryAccess",
                                                                   "BudgetPriorityUpdatePermittedFromLibraryAccess",
                                                                   "BudgetPriorityAddPermittedFromLibraryAccess",
                                                                   "BudgetPriorityAddAnyFromLibraryAccess"));
                options.AddPolicy(Policy.DeleteBudgetPriorityFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityDeleteAnyFromLibraryAccess", "BudgetPriorityDeletePermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ViewBudgetPriorityFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityViewPermittedFromScenarioAccess", "BudgetPriorityViewAnyFromScenarioAccess"));
                options.AddPolicy(Policy.ModifyBudgetPriorityFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityModifyPermittedFromScenarioAccess", "BudgetPriorityModifyAnyFromScenarioAccess"));

                // Calculated Attributes
                options.AddPolicy(Policy.ModifyCalculatedAttributesFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "CalculatedAttributesModifyFromLibraryAccess", "CalculatedAttributesChangeInLibraryAccess"));
                options.AddPolicy(Policy.ModifyCalculatedAttributesFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "CalculatedAttributesModifyFromScenarioAccess", "CalculatedAttributesChangeInScenarioAccess"));

                // Cash Flow
                options.AddPolicy(Policy.ViewCashFlowFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowViewAnyFromLibraryAccess", "CashFlowViewPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ViewCashFlowFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowViewAnyFromScenarioAccess", "CashFlowViewPermittedFromScenarioAccess"));
                options.AddPolicy(Policy.ModifyCashFlowFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowModifyAnyFromLibraryAccess", "CashFlowModifyPermittedFromLibraryAccess"));
                options.AddPolicy(Policy.ModifyCashFlowFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowModifyAnyFromScenarioAccess", "CashFlowModifyPermittedFromScenarioAccess"));

                // Committed Projects
                options.AddPolicy(Policy.ImportCommittedProjects,
                    policy => policy.RequireClaim(ClaimTypes.Name, "CommittedProjectImportAnyAccess", "CommittedProjectImportPermittedAccess"));
                options.AddPolicy(Policy.ModifyCommittedProjects,
                    policy => policy.RequireClaim(ClaimTypes.Name, "CommittedProjectModifyPermittedAccess", "CommittedProjectModifyAnyAccess"));
                options.AddPolicy(Policy.ViewCommittedProjects,
                    policy => policy.RequireClaim(ClaimTypes.Name, "CommittedProjectViewPermittedAccess", "CommittedProjectViewAnyAccess"));
            });

            services.AddSingleton<IEsecSecurity, EsecSecurity>();
            services.AddSingleton<IAuthorizationHandler, RestrictAccessHandler>();
            services.AddSingleton<IRoleClaimsMapper, RoleClaimsMapper>();
            services.AddScoped<IClaimHelper, ClaimHelper>();
        }
    }
}
