

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
using Claim = BridgeCareCore.Security.SecurityConstants.Claim;

namespace BridgeCareCore.StartupExtension
{
    public static class Security
    {
        public static void AddSecurityConfig(this IServiceCollection services, IConfiguration Configuration)
        {
            var securityType = SecurityConfigurationReader.GetSecurityType(Configuration);

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
                // Deficient Condition Goal
                options.AddPolicy(Policy.ViewDeficientConditionGoalFromlLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.DeficientConditionGoalViewPermittedFromLibraryAccess, Claim.DeficientConditionGoalViewAnyFromLibraryAccess));
                options.AddPolicy(Policy.ViewDeficientConditionGoalFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.DeficientConditionGoalViewAnyFromScenarioAccess, Claim.DeficientConditionGoalViewPermittedFromScenarioAccess));
                options.AddPolicy(Policy.ModifyDeficientConditionGoalFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.DeficientConditionGoalModifyAnyFromLibraryAccess, Claim.DeficientConditionGoalModifyPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ModifyDeficientConditionGoalFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.DeficientConditionGoalModifyAnyFromScenarioAccess, Claim.DeficientConditionGoalModifyPermittedFromScenarioAccess));

                //Investment
                options.AddPolicy(Policy.ViewInvestmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.InvestmentViewAnyFromScenarioAccess, Claim.InvestmentViewPermittedFromScenarioAccess));
                options.AddPolicy(Policy.ViewInvestmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.InvestmentViewAnyFromLibraryAccess, Claim.InvestmentViewPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ModifyInvestmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.InvestmentModifyAnyFromScenarioAccess, Claim.InvestmentModifyPermittedFromScenarioAccess));
                options.AddPolicy(Policy.ModifyInvestmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.InvestmentModifyAnyFromLibraryAccess, Claim.InvestmentModifyPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ImportInvestmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.InvestmentImportAnyFromLibraryAccess, Claim.InvestmentImportPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ImportInvestmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.InvestmentImportAnyFromScenarioAccess, Claim.InvestmentImportPermittedFromScenarioAccess));

                // Performance Curve
                options.AddPolicy(Policy.ViewPerformanceCurveFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.PerformanceCurveViewAnyFromLibraryAccess, Claim.PerformanceCurveViewPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ViewPerformanceCurveFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.PerformanceCurveViewAnyFromScenarioAccess, Claim.PerformanceCurveViewPermittedFromScenarioAccess));
                options.AddPolicy(Policy.ModifyPerformanceCurveFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.PerformanceCurveViewAnyFromLibraryAccess,
                                                                   Claim.PerformanceCurveUpdateAnyFromLibraryAccess,
                                                                   Claim.PerformanceCurveAddPermittedFromLibraryAccess,
                                                                   Claim.PerformanceCurveUpdatePermittedFromLibraryAccess));
                options.AddPolicy(Policy.ModifyPerformanceCurveFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.PerformanceCurveModifyAnyFromScenarioAccess, Claim.PerformanceCurveModifyPermittedFromScenarioAccess));
                options.AddPolicy(Policy.DeletePerformanceCurveFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.PerformanceCurveDeleteAnyFromLibraryAccess, Claim.PerformanceCurveDeletePermittedFromLibraryAccess));
                options.AddPolicy(Policy.ImportPerformanceCurveFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.PerformanceCurveImportAnyFromLibraryAccess, Claim.PerformanceCurveImportPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ImportPerformanceCurveFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.PerformanceCurveImportAnyFromScenarioAccess, Claim.PerformanceCurveImportPermittedFromScenarioAccess));                
                options.AddPolicy(Policy.ModifyOrDeletePerformanceCurveFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.PerformanceCurveViewAnyFromLibraryAccess,
                                                                   Claim.PerformanceCurveUpdateAnyFromLibraryAccess,
                                                                   Claim.PerformanceCurveAddPermittedFromLibraryAccess,
                                                                   Claim.PerformanceCurveUpdatePermittedFromLibraryAccess,
                                                                   Claim.PerformanceCurveDeleteAnyFromLibraryAccess,
                                                                   Claim.PerformanceCurveDeletePermittedFromLibraryAccess));

                //  Reamining Life Limit
                options.AddPolicy(Policy.ViewRemainingLifeLimitFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.RemainingLifeLimitViewAnyFromLibraryAccess, Claim.RemainingLifeLimitViewPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ViewRemainingLifeLimitFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.RemainingLifeLimitViewAnyFromScenarioAccess, Claim.RemainingLifeLimitViewPermittedFromScenarioAccess));
                options.AddPolicy(Policy.ModifyRemainingLifeLimitFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.RemainingLifeLimitAddAnyFromLibraryAccess,
                                                                   Claim.RemainingLifeLimitUpdateAnyFromLibraryAccess,
                                                                   Claim.RemainingLifeLimitAddPermittedFromLibraryAccess,
                                                                   Claim.RemainingLifeLimitUpdatePermittedFromLibraryAccess));
                options.AddPolicy(Policy.ModifyRemainingLifeLimitFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.RemainingLifeLimitModifyAnyFromScenarioAccess, Claim.RemainingLifeLimitModifyPermittedFromScenarioAccess));
                options.AddPolicy(Policy.DeleteRemainingLifeLimitFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.RemainingLifeLimitDeleteAnyFromLibraryAccess, Claim.RemainingLifeLimitDeletePermittedFromLibraryAccess));

                // Target Condition Goal
                options.AddPolicy(Policy.ViewTargetConditionGoalFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TargetConditionGoalViewAnyFromLibraryAccess, Claim.TargetConditionGoalViewPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ViewTargetConditionGoalFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TargetConditionGoalViewAnyFromScenarioAccess, Claim.TargetConditionGoalViewPermittedFromScenarioAccess));
                options.AddPolicy(Policy.ModifyTargetConditionGoalFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TargetConditionGoalAddAnyFromLibraryAccess,
                                                                   Claim.TargetConditionGoalUpdateAnyFromLibraryAccess,
                                                                   Claim.TargetConditionGoalAddPermittedFromLibraryAccess,
                                                                   Claim.TargetConditionGoalUpdatePermittedFromLibraryAccess));
                options.AddPolicy(Policy.ModifyTargetConditionGoalFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TargetConditionGoalModifyAnyFromScenarioAccess, Claim.TargetConditionGoalModifyPermittedFromScenarioAccess));
                options.AddPolicy(Policy.DeleteTargetConditionGoalFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TargetConditionGoalDeleteAnyFromLibraryAccess, Claim.TargetConditionGoalDeletePermittedFromLibraryAccess));
                options.AddPolicy(Policy.ModifyOrDeleteTargetConditionGoalFromLibrary,
                   policy => policy.RequireClaim(ClaimTypes.Name, Claim.TargetConditionGoalAddAnyFromLibraryAccess,
                                                                  Claim.TargetConditionGoalUpdateAnyFromLibraryAccess,
                                                                  Claim.TargetConditionGoalAddPermittedFromLibraryAccess,
                                                                  Claim.TargetConditionGoalUpdatePermittedFromLibraryAccess,
                                                                  Claim.TargetConditionGoalDeleteAnyFromLibraryAccess,
                                                                  Claim.TargetConditionGoalDeletePermittedFromLibraryAccess));

                // Treatment
                options.AddPolicy(Policy.ViewTreatmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TreatmentViewAnyFromLibraryAccess, Claim.TreatmentViewPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ViewTreatmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TreatmentViewAnyFromScenarioAccess, Claim.TreatmentViewPermittedFromScenarioAccess));
                options.AddPolicy(Policy.ModifyTreatmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TreatmentAddAnyFromLibraryAccess,
                                                                   Claim.TreatmentUpdateAnyFromLibraryAccess,
                                                                   Claim.TreatmentAddPermittedFromLibraryAccess,
                                                                   Claim.TreatmentUpdatePermittedFromLibraryAccess));
                options.AddPolicy(Policy.ModifyTreatmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TreatmentModifyAnyFromScenarioAccess,
                                                                   Claim.TreatmentModifyPermittedFromScenarioAccess));
                options.AddPolicy(Policy.DeleteTreatmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TreatmentDeleteAnyFromLibraryAccess, Claim.TreatmentDeletePermittedFromLibraryAccess));
                options.AddPolicy(Policy.ImportTreatmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TreatmentImportAnyFromLibraryAccess, Claim.TreatmentImportPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ImportTreatmentFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TreatmentImportAnyFromScenarioAccess, Claim.TreatmentImportPermittedFromScenarioAccess));                
                options.AddPolicy(Policy.ModifyOrDeleteTreatmentFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.TreatmentAddAnyFromLibraryAccess,
                                                                   Claim.TreatmentUpdateAnyFromLibraryAccess,
                                                                   Claim.TreatmentAddPermittedFromLibraryAccess,
                                                                   Claim.TreatmentUpdatePermittedFromLibraryAccess,
                                                                   Claim.TreatmentDeleteAnyFromLibraryAccess,
                                                                   Claim.TreatmentDeletePermittedFromLibraryAccess));

                // Analysis Method
                options.AddPolicy(Policy.ViewAnalysisMethod,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.AnalysisMethodViewAnyAccess, Claim.AnalysisMethodViewPermittedAccess));
                options.AddPolicy(Policy.ModifyAnalysisMethod,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.AnalysisMethodModifyAnyAccess, Claim.AnalysisMethodModifyPermittedAccess));

                // Attributes
                options.AddPolicy(Policy.ModifyAttributes, policy => policy.RequireClaim(ClaimTypes.Name, Claim.AttributesAddAccess, Claim.AttributesUpdateAccess));

                // Simulation
                options.AddPolicy(Policy.ViewSimulation,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.SimulationViewPermittedAccess,
                                                                   Claim.SimulationViewAnyAccess,
                                                                   Claim.SimulationAccess));
                options.AddPolicy(Policy.DeleteSimulation,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.SimulationDeletePermittedAccess,
                                                                   Claim.SimulationDeleteAnyAccess,
                                                                   Claim.SimulationAccess));
                options.AddPolicy(Policy.UpdateSimulation,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.SimulationUpdatePermittedAccess,
                                                                   Claim.SimulationUpdateAnyAccess,
                                                                   Claim.SimulationAccess));
                options.AddPolicy(Policy.RunSimulation,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.SimulationRunPermittedAccess,
                                                                   Claim.SimulationRunAnyAccess,
                                                                   Claim.SimulationAccess));

                options.AddPolicy(Policy.CloneSimulation,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.SimulationClonePermittedAccess,
                                                                   Claim.SimulationCloneAnyAccess,
                                                                   Claim.SimulationAccess));

                // Budget Priority
                options.AddPolicy(Policy.ViewBudgetPriorityFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.BudgetPriorityViewAnyFromLibraryAccess, Claim.BudgetPriorityViewPermittedFromLibraryAccess));                
                options.AddPolicy(Policy.ModifyBudgetPriorityFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.BudgetPriorityUpdateAnyFromLibraryAccess,
                                                                   Claim.BudgetPriorityUpdatePermittedFromLibraryAccess,
                                                                   Claim.BudgetPriorityAddPermittedFromLibraryAccess,
                                                                   Claim.BudgetPriorityAddAnyFromLibraryAccess));
                options.AddPolicy(Policy.DeleteBudgetPriorityFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.BudgetPriorityDeleteAnyFromLibraryAccess, Claim.BudgetPriorityDeletePermittedFromLibraryAccess));
                options.AddPolicy(Policy.ViewBudgetPriorityFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.BudgetPriorityViewPermittedFromScenarioAccess, Claim.BudgetPriorityViewAnyFromScenarioAccess));
                options.AddPolicy(Policy.ModifyBudgetPriorityFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.BudgetPriorityModifyPermittedFromScenarioAccess, Claim.BudgetPriorityModifyAnyFromScenarioAccess));

                // Calculated Attributes
                options.AddPolicy(Policy.ModifyCalculatedAttributesFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.CalculatedAttributesModifyFromLibraryAccess, Claim.CalculatedAttributesChangeDefaultLibraryAccess));
                options.AddPolicy(Policy.ModifyCalculatedAttributesFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.CalculatedAttributesModifyFromScenarioAccess, Claim.CalculatedAttributesChangeInScenarioAccess));

                // Cash Flow
                options.AddPolicy(Policy.ViewCashFlowFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.CashFlowViewAnyFromLibraryAccess, Claim.CashFlowViewPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ViewCashFlowFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.CashFlowViewAnyFromScenarioAccess, Claim.CashFlowViewPermittedFromScenarioAccess));
                options.AddPolicy(Policy.ModifyCashFlowFromLibrary,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.CashFlowModifyAnyFromLibraryAccess, Claim.CashFlowModifyPermittedFromLibraryAccess));
                options.AddPolicy(Policy.ModifyCashFlowFromScenario,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.CashFlowModifyAnyFromScenarioAccess, Claim.CashFlowModifyPermittedFromScenarioAccess));

                // Committed Projects
                options.AddPolicy(Policy.ImportCommittedProjects,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.CommittedProjectImportAnyAccess, Claim.CommittedProjectImportPermittedAccess));
                options.AddPolicy(Policy.ModifyCommittedProjects,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.CommittedProjectModifyPermittedAccess, Claim.CommittedProjectModifyAnyAccess));
                options.AddPolicy(Policy.ViewCommittedProjects,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.CommittedProjectViewPermittedAccess, Claim.CommittedProjectViewAnyAccess));

                // GraphQL
                options.AddPolicy(Policy.UseGraphQL,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.UseAnyGraphQLAccess, Claim.UsePermittedGraphQLAccess));

                // Admin Settings
                options.AddPolicy(Policy.ModifyAdminSiteSettings,
                    policy => policy.RequireClaim(ClaimTypes.Name, Claim.AdminSiteSettingsAccess));
            });

            services.AddSingleton<IEsecSecurity, EsecSecurity>();
            services.AddSingleton<IAuthorizationHandler, RestrictAccessHandler>();
            services.AddSingleton<IRoleClaimsMapper, RoleClaimsMapper>();
            services.AddScoped<IClaimHelper, ClaimHelper>();
            services.AddScoped<IGraphApiClientService, GraphApiClientService>();
        }
    }
}
