namespace BridgeCareCore.Security
{
    public static class SecurityConstants
    {
        public static class SecurityTypes
        {
            public const string Esec = "ESEC";
            public const string B2C = "B2C";
        }

        public static class Policy
        {
            public const string Admin = "UserIsAdmin";
            public const string AdminOrDistrictEngineer = "UserIsAdminOrDistrictEngineer";
        }

        public static class Role
        {            
            public const string Administrator = "Administrator";

            // TODO remove later:
            public const string BAMSAdmin = "PD-BAMS-Administrator";            
        }
        public static class Claim
        {
            public static string AttributesUpdateAccess = "AttributesUpdateAccess";
            public static string AttributesAddAccess = "AttributesAddAccess";
            public static string AttributesViewAccess = "AttributesViewAccess";
            public static string DataSourceModifyAccess = "DataSourceModifyAccess";
            public static string DataSourceAddAccess = "DataSourceAddAccess";
            public static string DataSourceViewAccess = "DataSourceViewAccess";
            public static string NetworkAggregateAccess = "NetworkAggregateAccess";
            public static string NetworkAddAccess = "NetworkAddAccess";
            public static string NetworkViewAccess = "NetworkViewAccess";
            public static string SimulationUpdateAnyAccess = "SimulationUpdateAnyAccess";
            public static string SimulationDeleteAnyAccess = "SimulationDeleteAnyAccess";
            public static string SimulationRunAnyAccess = "SimulationRunAnyAccess";
            public static string SimulationViewAnyAccess = "SimulationViewAnyAccess";
            public static string AnnouncementModifyAccess = "AnnouncementModifyAccess";
            public static string AnnouncementViewAccess = "AnnouncementViewAccess";
            public static string BudgetPriorityAddAnyFromLibraryAccess = "BudgetPriorityAddAnyFromLibraryAccess";
            public static string BudgetPriorityUpdateAnyFromLibraryAccess = "BudgetPriorityUpdateAnyFromLibraryAccess";
            public static string BudgetPriorityDeleteAnyFromLibraryAccess = "BudgetPriorityDeleteAnyFromLibraryAccess";
            public static string BudgetPriorityViewAnyFromLibraryAccess = "BudgetPriorityViewAnyFromLibraryAccess";
            public static string BudgetPriorityModifyAnyFromScenarioAccess = "BudgetPriorityModifyAnyFromScenarioAccess";
            public static string BudgetPriorityViewAnyFromScenarioAccess = "BudgetPriorityViewAnyFromScenarioAccess";
            public static string CalculatedAttributesModifyFromScenarioAccess = "CalculatedAttributesModifyFromScenarioAccess";
            public static string CalculatedAttributesChangeInScenarioAccess = "CalculatedAttributesChangeInScenarioAccess";
            public static string CalculatedAttributesViewAccess = "CalculatedAttributesViewAccess";
            public static string CalculatedAttributesModifyFromLibraryAccess = "CalculatedAttributesModifyFromLibraryAccess";
            public static string CalculatedAttributesChangeDefaultLibraryAccess = "CalculatedAttributesChangeDefaultLibraryAccess";
            public static string CashFlowModifyAnyFromLibraryAccess = "CashFlowModifyAnyFromLibraryAccess";
            public static string CashFlowViewAnyFromLibraryAccess = "CashFlowViewAnyFromLibraryAccess";
            public static string CashFlowModifyAnyFromScenarioAccess = "CashFlowModifyAnyFromScenarioAccess";
            public static string CashFlowViewAnyFromScenarioAccess = "CashFlowViewAnyFromScenarioAccess";
            public static string DeficientConditionGoalModifyAnyFromLibraryAccess = "DeficientConditionGoalModifyAnyFromLibraryAccess";
            public static string DeficientConditionGoalViewAnyFromLibraryAccess = "DeficientConditionGoalViewAnyFromLibraryAccess";
            public static string DeficientConditionGoalModifyAnyFromScenarioAccess = "DeficientConditionGoalModifyAnyFromScenarioAccess";
            public static string InvestmentModifyAnyFromLibraryAccess = "InvestmentModifyAnyFromLibraryAccess";
            public static string InvestmentImportAnyFromLibraryAccess = "InvestmentImportAnyFromLibraryAccess";
            public static string InvestmentViewAnyFromLibraryAccess = "InvestmentViewAnyFromLibraryAccess";
            public static string InvestmentModifyAnyFromScenarioAccess = "InvestmentModifyAnyFromScenarioAccess";
            public static string InvestmentImportAnyFromScenarioAccess = "InvestmentImportAnyFromScenarioAccess";
            public static string InvestmentViewAnyFromScenarioAccess = "InvestmentViewAnyFromScenarioAccess";
            public static string PerformanceCurveAddAnyFromLibraryAccess = "PerformanceCurveAddAnyFromLibraryAccess";
            public static string PerformanceCurveUpdateAnyFromLibraryAccess = "PerformanceCurveUpdateAnyFromLibraryAccess";
            public static string PerformanceCurveDeleteAnyFromLibraryAccess = "PerformanceCurveDeleteAnyFromLibraryAccess";
            public static string PerformanceCurveImportAnyFromLibraryAccess = "PerformanceCurveImportAnyFromLibraryAccess";
            public static string PerformanceCurveViewAnyFromLibraryAccess = "PerformanceCurveViewAnyFromLibraryAccess";
            public static string PerformanceCurveModifyAnyFromScenarioAccess = "PerformanceCurveModifyAnyFromScenarioAccess";
            public static string PerformanceCurveViewAnyFromScenarioAccess = "PerformanceCurveViewAnyFromScenarioAccess";
            public static string RemainingLifeLimitAddAnyFromLibraryAccess = "RemainingLifeLimitAddAnyFromLibraryAccess";
            public static string RemainingLifeLimitUpdateAnyFromLibraryAccess = "RemainingLifeLimitUpdateAnyFromLibraryAccess";
            public static string RemainingLifeLimitDeleteAnyFromLibraryAccess = "RemainingLifeLimitDeleteAnyFromLibraryAccess";
            public static string RemainingLifeLimitViewAnyFromLibraryAccess = "RemainingLifeLimitViewAnyFromLibraryAccess";
            public static string RemainingLifeLimitModifyAnyFromScenarioAccess = "RemainingLifeLimitModifyAnyFromScenarioAccess";
            public static string RemainingLifeLimitViewAnyFromScenarioAccess = "RemainingLifeLimitViewAnyFromScenarioAccess";
            public static string TargetConditionGoalAddAnyFromLibraryAccess = "TargetConditionGoalAddAnyFromLibraryAccess";
            public static string TargetConditionGoalUpdateAnyFromLibraryAccess = "TargetConditionGoalUpdateAnyFromLibraryAccess";
            public static string TargetConditionGoalDeleteAnyFromLibraryAccess = "TargetConditionGoalDeleteAnyFromLibraryAccess";
            public static string TargetConditionGoalViewAnyFromLibraryAccess = "TargetConditionGoalViewAnyFromLibraryAccess";
            public static string TargetConditionGoalModifyAnyFromScenarioAccess = "TargetConditionGoalModifyAnyFromScenarioAccess";
            public static string TargetConditionGoalViewAnyFromScenarioAccess = "TargetConditionGoalViewAnyFromScenarioAccess";
            public static string TreatmentAddAnyFromLibraryAccess = "TreatmentAddAnyFromLibraryAccess";
            public static string TreatmentUpdateAnyFromLibraryAccess = "TreatmentUpdateAnyFromLibraryAccess";
            public static string TreatmentDeleteAnyFromLibraryAccess = "TreatmentDeleteAnyFromLibraryAccess";
            public static string TreatmentImportAnyFromLibraryAccess = "TreatmentImportAnyFromLibraryAccess";
            public static string TreatmentViewAnyFromLibraryAccess = "TreatmentViewAnyFromLibraryAccess";
            public static string TreatmentModifyAnyFromScenarioAccess = "TreatmentModifyAnyFromScenarioAccess";
            public static string TreatmentViewAnyFromScenarioAccess = "TreatmentViewAnyFromScenarioAccess";
            public static string UserCriteriaModifyAccess = "UserCriteriaModifyAccess";
            public static string UserCriteriaViewAccess = "UserCriteriaViewAccess";
            public static string AnalysisMethodModifyAnyAccess = "AnalysisMethodModifyAnyAccess";
            public static string AnalysisMethodViewAnyAccess = "AnalysisMethodViewAnyAccess";
            public static string CommittedProjectModifyAnyAccess = "CommittedProjectModifyAnyAccess";
            public static string CommittedProjectImportAnyAccess = "CommittedProjectImportAnyAccess";
            public static string CommittedProjectViewAnyAccess = "CommittedProjectViewAnyAccess";
            public static string SimulationUpdatePermittedAccess = "SimulationUpdatePermittedAccess";
            public static string SimulationDeletePermittedAccess = "SimulationDeletePermittedAccess";
            public static string SimulationRunPermittedAccess = "SimulationRunPermittedAccess";
            public static string SimulationViewPermittedAccess = "SimulationViewPermittedAccess";
            public static string BudgetPriorityAddPermittedFromLibraryAccess = "BudgetPriorityAddPermittedFromLibraryAccess";
            public static string BudgetPriorityUpdatePermittedFromLibraryAccess = "BudgetPriorityUpdatePermittedFromLibraryAccess";
            public static string BudgetPriorityDeletePermittedFromLibraryAccess = "BudgetPriorityDeletePermittedFromLibraryAccess";
            public static string BudgetPriorityViewPermittedFromLibraryAccess = "BudgetPriorityViewPermittedFromLibraryAccess";
            public static string BudgetPriorityModifyPermittedFromScenarioAccess = "BudgetPriorityModifyPermittedFromScenarioAccess";
            public static string BudgetPriorityViewPermittedFromScenarioAccess = "BudgetPriorityViewPermittedFromScenarioAccess";
            public static string CashFlowModifyPermittedFromLibraryAccess = "CashFlowModifyPermittedFromLibraryAccess";
            public static string CashFlowViewPermittedFromLibraryAccess = "CashFlowViewPermittedFromLibraryAccess";
            public static string CashFlowModifyPermittedFromScenarioAccess = "CashFlowModifyPermittedFromScenarioAccess";
            public static string CashFlowViewPermittedFromScenarioAccess = "CashFlowViewPermittedFromScenarioAccess";
            public static string DeficientConditionGoalModifyPermittedFromLibraryAccess = "DeficientConditionGoalModifyPermittedFromLibraryAccess";
            public static string DeficientConditionGoalViewPermittedFromLibraryAccess = "DeficientConditionGoalViewPermittedFromLibraryAccess";
            public static string DeficientConditionGoalModifyPermittedFromScenarioAccess = "DeficientConditionGoalModifyPermittedFromScenarioAccess";
            public static string DeficientConditionGoalViewPermittedFromScenarioAccess = "DeficientConditionGoalViewPermittedFromScenarioAccess";
            public static string InvestmentModifyPermittedFromLibraryAccess = "InvestmentModifyPermittedFromLibraryAccess";
            public static string InvestmentImportPermittedFromLibraryAccess = "InvestmentImportPermittedFromLibraryAccess";
            public static string InvestmentViewPermittedFromLibraryAccess = "InvestmentViewPermittedFromLibraryAccess";
            public static string InvestmentModifyPermittedFromScenarioAccess = "InvestmentModifyPermittedFromScenarioAccess";
            public static string InvestmentImportPermittedFromScenarioAccess = "InvestmentImportPermittedFromScenarioAccess";
            public static string InvestmentViewPermittedFromScenarioAccess = "InvestmentViewPermittedFromScenarioAccess";
            public static string PerformanceCurveAddPermittedFromLibraryAccess = "PerformanceCurveAddPermittedFromLibraryAccess";
            public static string PerformanceCurveUpdatePermittedFromLibraryAccess = "PerformanceCurveUpdatePermittedFromLibraryAccess";
            public static string PerformanceCurveDeletePermittedFromLibraryAccess = "PerformanceCurveDeletePermittedFromLibraryAccess";
            public static string PerformanceCurveImportPermittedFromLibraryAccess = "PerformanceCurveImportPermittedFromLibraryAccess";
            public static string PerformanceCurveViewPermittedFromLibraryAccess = "PerformanceCurveViewPermittedFromLibraryAccess";
            public static string PerformanceCurveModifyPermittedFromScenarioAccess = "PerformanceCurveModifyPermittedFromScenarioAccess";
            public static string PerformanceCurveViewPermittedFromScenarioAccess = "PerformanceCurveViewPermittedFromScenarioAccess";
            public static string RemainingLifeLimitAddPermittedFromLibraryAccess = "RemainingLifeLimitAddPermittedFromLibraryAccess";
            public static string RemainingLifeLimitUpdatePermittedFromLibraryAccess = "RemainingLifeLimitUpdatePermittedFromLibraryAccess";
            public static string RemainingLifeLimitDeletePermittedFromLibraryAccess = "RemainingLifeLimitDeletePermittedFromLibraryAccess";
            public static string RemainingLifeLimitViewPermittedFromLibraryAccess = "RemainingLifeLimitViewPermittedFromLibraryAccess";
            public static string RemainingLifeLimitModifyPermittedFromScenarioAccess = "RemainingLifeLimitModifyPermittedFromScenarioAccess";
            public static string RemainingLifeLimitViewPermittedFromScenarioAccess = "RemainingLifeLimitViewPermittedFromScenarioAccess";
            public static string TargetConditionGoalAddPermittedFromLibraryAccess = "TargetConditionGoalAddPermittedFromLibraryAccess";
            public static string TargetConditionGoalUpdatePermittedFromLibraryAccess = "TargetConditionGoalUpdatePermittedFromLibraryAccess";
            public static string TargetConditionGoalDeletePermittedFromLibraryAccess = "TargetConditionGoalDeletePermittedFromLibraryAccess";
            public static string TargetConditionGoalViewPermittedFromLibraryAccess = "TargetConditionGoalViewPermittedFromLibraryAccess";
            public static string TargetConditionGoalModifyPermittedFromScenarioAccess = "TargetConditionGoalModifyPermittedFromScenarioAccess";
            public static string TargetConditionGoalViewPermittedFromScenarioAccess = "TargetConditionGoalViewPermittedFromScenarioAccess";
            public static string TreatmentAddPermittedFromLibraryAccess = "TreatmentAddPermittedFromLibraryAccess";
            public static string TreatmentUpdatePermittedFromLibraryAccess = "TreatmentUpdatePermittedFromLibraryAccess";
            public static string TreatmentDeletePermittedFromLibraryAccess = "TreatmentDeletePermittedFromLibraryAccess";
            public static string TreatmentImportPermittedFromLibraryAccess = "TreatmentImportPermittedFromLibraryAccess";
            public static string TreatmentViewPermittedFromLibraryAccess = "TreatmentViewPermittedFromLibraryAccess";
            public static string TreatmentModifyPermittedFromScenarioAccess = "TreatmentModifyPermittedFromScenarioAccess";
            public static string TreatmentViewPermittedFromScenarioAccess = "TreatmentViewPermittedFromScenarioAccess";
            public static string AnalysisMethodModifyPermittedAccess = "AnalysisMethodModifyPermittedAccess";
            public static string AnalysisMethodViewPermittedAccess = "AnalysisMethodViewPermittedAccess";
            public static string CommittedProjectModifyPermittedAccess = "CommittedProjectModifyPermittedAccess";
            public static string CommittedProjectImportPermittedAccess = "CommittedProjectImportPermittedAccess";
            public static string CommittedProjectViewPermittedAccess = "CommittedProjectViewPermittedAccess";

        }
    }
}
