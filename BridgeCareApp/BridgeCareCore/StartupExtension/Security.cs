

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
                options.AddPolicy(SecurityConstants.Policy.AdminOrDistrictEngineer,
                    policy => policy.Requirements.Add(
                        new UserHasAllowedRoleRequirement(Role.Administrator, Role.DistrictEngineer)));
                options.AddPolicy(SecurityConstants.Policy.Admin,
                    policy => policy.Requirements.Add(
                        new UserHasAllowedRoleRequirement(Role.Administrator)));

                // Deficient Condition Goal
                options.AddPolicy("ViewDeficientConditionGoalFromlLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalViewPermittedFromLibraryAccess", "DeficientConditionGoalViewAnyFromLibraryAccess"));
                options.AddPolicy("ViewDeficientConditionGoalFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalViewAnyFromScenarioAccess", "DeficientConditionGoalViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyDeficientConditionGoalFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalModifyAnyFromLibraryAccess", "DeficientConditionGoalModifyPermittedFromLibraryAccess"));
                options.AddPolicy("ModifyDeficientConditionGoalFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "DeficientConditionGoalModifyAnyFromScenarioAccess", "DeficientConditionGoalModifyPermittedFromScenarioAccess"));

                //Investment
                options.AddPolicy("ViewInvestmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentViewAnyFromScenarioAccess", "InvestmentViewPermittedFromScenarioAccess"));
                options.AddPolicy("ViewInvestmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentViewAnyFromLibraryAccess", "InvestmentViewPermittedFromLibraryAccess"));
                options.AddPolicy("ModifyInvestmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentModifyAnyFromScenarioAccess", "InvestmentModifyPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyInvestmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentModifyAnyFromLibraryAccess", "InvestmentModifyPermittedFromLibraryAccess"));
                options.AddPolicy("ImportInvestmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentImportAnyFromLibraryAccess", "InvestmentImportPermittedFromLibraryAccess"));
                options.AddPolicy("ImportInvestmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "InvestmentImportAnyFromScenarioAccess", "InvestmentImportPermittedFromScenarioAccess"));

                // Performance Curve
                options.AddPolicy("ViewPerformanceCurveFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveViewAnyFromLibraryAccess", "PerformanceCurveViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewPerformanceCurveFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveViewAnyFromScenarioAccess", "PerformanceCurveViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyPerformanceCurveFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveAddAnyFromLibraryAccess",
                                                                   "PerformanceCurveUpdateAnyFromLibraryAccess",
                                                                   "PerformanceCurveAddPermittedFromLibraryAccess",
                                                                   "PerformanceCurveUpdatePermittedFromLibraryAccess"));
                options.AddPolicy("ModifyPerformanceCurveFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveModifyAnyFromScenarioAccess", "PerformanceCurveModifyPermittedFromScenarioAccess"));
                options.AddPolicy("DeletePerformanceCurveFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveDeleteAnyFromLibraryAccess", "PerformanceCurveDeletePermittedFromLibraryAccess"));
                options.AddPolicy("ImportPerformanceCurveFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveImportAnyFromLibraryAccess", "PerformanceCurveImportPermittedFromLibraryAccess"));
                options.AddPolicy("ImportPerformanceCurveFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "PerformanceCurveImportAnyFromScenarioAccess", "PerformanceCurveImportPermittedFromScenarioAccess"));

                //  Reamining Life Limit
                options.AddPolicy("ViewRemainingLifeLimitFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitViewAnyFromLibraryAccess", "RemainingLifeLimitViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewRemainingLifeLimitFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitViewAnyFromScenarioAccess", "RemainingLifeLimitViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyRemainingLifeLimitFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitAddAnyFromLibraryAccess",
                                                                   "RemainingLifeLimitUpdateAnyFromLibraryAccess",
                                                                   "RemainingLifeLimitAddPermittedFromLibraryAccess",
                                                                   "RemainingLifeLimitUpdatePermittedFromLibraryAccess"));
                options.AddPolicy("ModifyRemainingLifeLimitFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitModifyAnyFromScenarioAccess", "RemainingLifeLimitModifyPermittedFromScenarioAccess"));
                options.AddPolicy("DeleteRemainingLifeLimitFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "RemainingLifeLimitDeleteAnyFromLibraryAccess", "RemainingLifeLimitDeletePermittedFromLibraryAccess"));

                // Target Condition Goal
                options.AddPolicy("ViewTargetConditionGoalFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalViewAnyFromLibraryAccess", "TargetConditionGoalViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewTargetConditionGoalFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalViewAnyFromScenarioAccess", "TargetConditionGoalViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyTargetConditionGoalFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalAddAnyFromLibraryAccess",
                                                                   "TargetConditionGoalUpdateAnyFromLibraryAccess",
                                                                   "TargetConditionGoalAddPermittedFromLibraryAccess",
                                                                   "TargetConditionGoalUpdatePermittedFromLibraryAccess"));
                options.AddPolicy("ModifyTargetConditionGoalFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalModifyAnyFromScenarioAccess", "TargetConditionGoalModifyPermittedFromScenarioAccess"));
                options.AddPolicy("DeleteTargetConditionGoalFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TargetConditionGoalDeleteAnyFromLibraryAccess", "TargetConditionGoalDeletePermittedFromLibraryAccess"));

                // Treatment
                options.AddPolicy("ViewTreatmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentViewAnyFromLibraryAccess", "TreatmentViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewTreatmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentViewAnyFromScenarioAccess", "TreatmentViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyTreatmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentAddAnyFromLibraryAccess",
                                                                   "TreatmentUpdateAnyFromLibraryAccess",
                                                                   "TreatmentAddPermittedFromLibraryAccess",
                                                                   "TreatmentUpdatePermittedFromLibraryAccess"));
                options.AddPolicy("ModifyTreatmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentModifyAnyFromScenarioAccess",
                                                                   "TreatmentModifyPermittedFromScenarioAccess"));
                options.AddPolicy("DeleteTreatmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentDeleteAnyFromLibraryAccess", "TreatmentDeletePermittedFromLibraryAccess"));
                options.AddPolicy("ImportTreatmentFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentImportAnyFromLibraryAccess", "TreatmentImportPermittedFromLibraryAccess"));
                options.AddPolicy("ImportTreatmentFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "TreatmentImportAnyFromScenarioAccess", "TreatmentImportPermittedFromScenarioAccess"));

                // Analysis Method
                options.AddPolicy("ViewAnalysisMethod",
                    policy => policy.RequireClaim(ClaimTypes.Name, "AnalysisMethodViewAnyAccess", "AnalysisMethodViewPermittedAccess"));
                options.AddPolicy("ModifyAnalysisMethod",
                    policy => policy.RequireClaim(ClaimTypes.Name, "AnalysisMethodModifyAnyAccess", "AnalysisMethodModifyPermittedAccess"));

                // Attributes
                options.AddPolicy("ModifyAttributes", policy => policy.RequireClaim(ClaimTypes.Name, "AttributesAddAccess", "AttributesUpdateAccess"));

                // Simulation
                options.AddPolicy("ViewSimulation",
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationViewPermittedAccess", "SimulationViewAnyAccess"));
                options.AddPolicy("DeleteSimulation",
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationDeletePermittedAccess", "SimulationDeleteAnyAccess"));
                options.AddPolicy("UpdateSimulation",
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationUpdatePermittedAccess", "SimulationUpdateAnyAccess"));
                options.AddPolicy("RunSimulation",
                    policy => policy.RequireClaim(ClaimTypes.Name, "SimulationRunPermittedAccess", "SimulationRunAnyAccess"));

                // Budget Priority
                options.AddPolicy("ViewBudgetPriorityFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityViewAnyFromLibraryAccess", "BudgetPriorityViewPermittedFromLibraryAccess"));
                options.AddPolicy("ModifyBudgetPriorityFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityUpdateAnyFromLibraryAccess",
                                                                   "BudgetPriorityUpdatePermittedFromLibraryAccess",
                                                                   "BudgetPriorityAddPermittedFromLibraryAccess",
                                                                   "BudgetPriorityAddAnyFromLibraryAccess"));
                options.AddPolicy("DeleteBudgetPriorityFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityDeleteAnyFromLibraryAccess", "BudgetPriorityDeletePermittedFromLibraryAccess"));
                options.AddPolicy("ViewBudgetPriorityFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityViewPermittedFromScenarioAccess", "BudgetPriorityViewAnyFromScenarioAccess"));
                options.AddPolicy("ModifyBudgetPriorityFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "BudgetPriorityModifyPermittedFromScenarioAccess", "BudgetPriorityModifyAnyFromScenarioAccess"));

                // Calculated Attributes
                options.AddPolicy("ModifyCalculatedAttributesFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CalculatedAttributesModifyFromLibraryAccess", "CalculatedAttributesChangeInLibraryAccess"));
                options.AddPolicy("ModifyCalculatedAttributesFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CalculatedAttributesModifyFromScenarioAccess", "CalculatedAttributesChangeInScenarioAccess"));

                // Cash Flow
                options.AddPolicy("ViewCashFlowFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowViewAnyFromLibraryAccess", "CashFlowViewPermittedFromLibraryAccess"));
                options.AddPolicy("ViewCashFlowFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowViewAnyFromScenarioAccess", "CashFlowViewPermittedFromScenarioAccess"));
                options.AddPolicy("ModifyCashFlowFromLibrary",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowModifyAnyFromLibraryAccess", "CashFlowModifyPermittedFromLibraryAccess"));
                options.AddPolicy("ModifyCashFlowFromScenario",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CashFlowModifyAnyFromScenarioAccess", "CashFlowModifyPermittedFromScenarioAccess"));

                // Committed Projects
                options.AddPolicy("ImportCommittedProjects",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CommittedProjectImportAnyAccess", "CommittedProjectImportPermittedAccess"));
                options.AddPolicy("ModifyCommittedProjects",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CommittedProjectModifyPermittedAccess", "CommittedProjectModifyAnyAccess"));
                options.AddPolicy("ViewCommittedProjects",
                    policy => policy.RequireClaim(ClaimTypes.Name, "CommittedProjectViewPermittedAccess", "CommittedProjectViewAnyAccess"));
            });

            services.AddSingleton<IEsecSecurity, EsecSecurity>();
            services.AddSingleton<IAuthorizationHandler, RestrictAccessHandler>();
            services.AddSingleton<IRoleClaimsMapper, RoleClaimsMapper>();
        }
    }
}
