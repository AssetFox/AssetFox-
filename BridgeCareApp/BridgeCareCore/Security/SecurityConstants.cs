﻿namespace BridgeCareCore.Security
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
            // TODO remove below 2 later
            public const string Admin = "UserIsAdmin";
            public const string AdminOrDistrictEngineer = "UserIsAdminOrDistrictEngineer";
            ////


            public const string ViewDeficientConditionGoalFromlLibrary = "ViewDeficientConditionGoalFromlLibrary";
            public const string ViewDeficientConditionGoalFromScenario = "ViewDeficientConditionGoalFromScenario";
            public const string ModifyDeficientConditionGoalFromLibrary = "ModifyDeficientConditionGoalFromLibrary";
            public const string ModifyDeficientConditionGoalFromScenario = "ModifyDeficientConditionGoalFromScenario";
            public const string ViewInvestmentFromScenario = "ViewInvestmentFromScenario";
            public const string ModifyInvestmentFromScenario = "ModifyInvestmentFromScenario";
            public const string ViewInvestmentFromLibrary = "ViewInvestmentFromLibrary";
            public const string ModifyInvestmentFromLibrary = "ModifyInvestmentFromLibrary";
            public const string ImportInvestmentFromLibrary = "ImportInvestmentFromLibrary";
            public const string ImportInvestmentFromScenario = "ImportInvestmentFromScenario";
            public const string ViewPerformanceCurveFromLibrary = "ViewPerformanceCurveFromLibrary";
            public const string ViewPerformanceCurveFromScenario = "ViewPerformanceCurveFromScenario";
            public const string ModifyPerformanceCurveFromLibrary = "ModifyPerformanceCurveFromLibrary";
            public const string ModifyPerformanceCurveFromScenario = "ModifyPerformanceCurveFromScenario";
            public const string DeletePerformanceCurveFromLibrary = "DeletePerformanceCurveFromLibrary";
            public const string ImportPerformanceCurveFromLibrary = "ImportPerformanceCurveFromLibrary";
            public const string ImportPerformanceCurveFromScenario = "ImportPerformanceCurveFromScenario";
            public const string ViewRemainingLifeLimitFromLibrary = "ViewRemainingLifeLimitFromLibrary";
            public const string ViewRemainingLifeLimitFromScenario = "ViewRemainingLifeLimitFromScenario";
            public const string ModifyRemainingLifeLimitFromLibrary = "ModifyRemainingLifeLimitFromLibrary";
            public const string ModifyRemainingLifeLimitFromScenario = "ModifyRemainingLifeLimitFromScenario";
            public const string DeleteRemainingLifeLimitFromLibrary = "DeleteRemainingLifeLimitFromLibrary";
            public const string ViewTargetConditionGoalFromLibrary = "ViewTargetConditionGoalFromLibrary";
            public const string ViewTargetConditionGoalFromScenario = "ViewTargetConditionGoalFromScenario";
            public const string ModifyTargetConditionGoalFromLibrary = "ModifyTargetConditionGoalFromLibrary";
            public const string ModifyTargetConditionGoalFromScenario = "ModifyTargetConditionGoalFromScenario";
            public const string DeleteTargetConditionGoalFromLibrary = "DeleteTargetConditionGoalFromLibrary";
            public const string ViewTreatmentFromLibrary = "ViewTreatmentFromLibrary";
            public const string ViewTreatmentFromScenario = "ViewTreatmentFromScenario";
            public const string ModifyTreatmentFromLibrary = "ModifyTreatmentFromLibrary";
            public const string ModifyTreatmentFromScenario = "ModifyTreatmentFromScenario";
            public const string DeleteTreatmentFromLibrary = "DeleteTreatmentFromLibrary";
            public const string ImportTreatmentFromLibrary = "ImportTreatmentFromLibrary";
            public const string ImportTreatmentFromScenario = "ImportTreatmentFromScenario";
            public const string ViewAnalysisMethod = "ViewAnalysisMethod";
            public const string ModifyAnalysisMethod = "ModifyAnalysisMethod";
            public const string ModifyAttributes = "ModifyAttributes";
            public const string ViewSimulation = "ViewSimulation";
            public const string DeleteSimulation = "DeleteSimulation";
            public const string UpdateSimulation = "UpdateSimulation";
            public const string RunSimulation = "RunSimulation";
            public const string ViewBudgetPriorityFromLibrary = "ViewBudgetPriorityFromLibrary";
            public const string ModifyBudgetPriorityFromLibrary = "ModifyBudgetPriorityFromLibrary";
            public const string DeleteBudgetPriorityFromLibrary = "DeleteBudgetPriorityFromLibrary";
            public const string ViewBudgetPriorityFromScenario = "ViewBudgetPriorityFromScenario";
            public const string ModifyBudgetPriorityFromScenario = "ModifyBudgetPriorityFromScenario";
            public const string ModifyCalculatedAttributesFromLibrary = "ModifyCalculatedAttributesFromLibrary";
            public const string ModifyCalculatedAttributesFromScenario = "ModifyCalculatedAttributesFromScenario";
            public const string ViewCashFlowFromLibrary = "ViewCashFlowFromLibrary";
            public const string ViewCashFlowFromScenario = "ViewCashFlowFromScenario";
            public const string ModifyCashFlowFromLibrary = "ModifyCashFlowFromLibrary";
            public const string ModifyCashFlowFromScenario = "ModifyCashFlowFromScenario";
            public const string ImportCommittedProjects = "ImportCommittedProjects";
            public const string ModifyCommittedProjects = "ModifyCommittedProjects";
            public const string ViewCommittedProjects = "ViewCommittedProjects";
        }

        public static class Role
        {            
            public const string Administrator = "Administrator";
            public const string Editor = "Editor";
            public const string ReadOnly = "ReadOnly";
        }

        public static class Claim
        {            
            public const string AttributesUpdateAccess = "AttributesUpdateAccess";
            public const string AttributesAddAccess = "AttributesAddAccess";
            public const string AttributesViewAccess = "AttributesViewAccess";
            public const string DataSourceModifyAccess = "DataSourceModifyAccess";            
            public const string DataSourceViewAccess = "DataSourceViewAccess";
            public const string NetworkAggregateAccess = "NetworkAggregateAccess";
            public const string NetworkAddAccess = "NetworkAddAccess";
            public const string NetworkViewAccess = "NetworkViewAccess";
            public const string SimulationUpdateAnyAccess = "SimulationUpdateAnyAccess";
            public const string SimulationDeleteAnyAccess = "SimulationDeleteAnyAccess";
            public const string SimulationRunAnyAccess = "SimulationRunAnyAccess";
            public const string SimulationViewAnyAccess = "SimulationViewAnyAccess";
            public const string AnnouncementModifyAccess = "AnnouncementModifyAccess";
            public const string AnnouncementViewAccess = "AnnouncementViewAccess";
            public const string BudgetPriorityAddAnyFromLibraryAccess = "BudgetPriorityAddAnyFromLibraryAccess";
            public const string BudgetPriorityUpdateAnyFromLibraryAccess = "BudgetPriorityUpdateAnyFromLibraryAccess";
            public const string BudgetPriorityDeleteAnyFromLibraryAccess = "BudgetPriorityDeleteAnyFromLibraryAccess";
            public const string BudgetPriorityViewAnyFromLibraryAccess = "BudgetPriorityViewAnyFromLibraryAccess";
            public const string BudgetPriorityModifyAnyFromScenarioAccess = "BudgetPriorityModifyAnyFromScenarioAccess";
            public const string BudgetPriorityViewAnyFromScenarioAccess = "BudgetPriorityViewAnyFromScenarioAccess";
            public const string CalculatedAttributesModifyFromScenarioAccess = "CalculatedAttributesModifyFromScenarioAccess";
            public const string CalculatedAttributesChangeInScenarioAccess = "CalculatedAttributesChangeInScenarioAccess";
            public const string CalculatedAttributesViewAccess = "CalculatedAttributesViewAccess";
            public const string CalculatedAttributesModifyFromLibraryAccess = "CalculatedAttributesModifyFromLibraryAccess";
            public const string CalculatedAttributesChangeDefaultLibraryAccess = "CalculatedAttributesChangeDefaultLibraryAccess";
            public const string CashFlowModifyAnyFromLibraryAccess = "CashFlowModifyAnyFromLibraryAccess";
            public const string CashFlowViewAnyFromLibraryAccess = "CashFlowViewAnyFromLibraryAccess";
            public const string CashFlowModifyAnyFromScenarioAccess = "CashFlowModifyAnyFromScenarioAccess";
            public const string CashFlowViewAnyFromScenarioAccess = "CashFlowViewAnyFromScenarioAccess";
            public const string DeficientConditionGoalModifyAnyFromLibraryAccess = "DeficientConditionGoalModifyAnyFromLibraryAccess";
            public const string DeficientConditionGoalViewAnyFromLibraryAccess = "DeficientConditionGoalViewAnyFromLibraryAccess";
            public const string DeficientConditionGoalModifyAnyFromScenarioAccess = "DeficientConditionGoalModifyAnyFromScenarioAccess";
            public const string DeficientConditionGoalViewAnyFromScenarioAccess = "DeficientConditionGoalViewAnyFromScenarioAccess";
            public const string InvestmentModifyAnyFromLibraryAccess = "InvestmentModifyAnyFromLibraryAccess";
            public const string InvestmentImportAnyFromLibraryAccess = "InvestmentImportAnyFromLibraryAccess";
            public const string InvestmentViewAnyFromLibraryAccess = "InvestmentViewAnyFromLibraryAccess";
            public const string InvestmentModifyAnyFromScenarioAccess = "InvestmentModifyAnyFromScenarioAccess";
            public const string InvestmentImportAnyFromScenarioAccess = "InvestmentImportAnyFromScenarioAccess";
            public const string InvestmentViewAnyFromScenarioAccess = "InvestmentViewAnyFromScenarioAccess";
            public const string PerformanceCurveAddAnyFromLibraryAccess = "PerformanceCurveAddAnyFromLibraryAccess";
            public const string PerformanceCurveUpdateAnyFromLibraryAccess = "PerformanceCurveUpdateAnyFromLibraryAccess";
            public const string PerformanceCurveDeleteAnyFromLibraryAccess = "PerformanceCurveDeleteAnyFromLibraryAccess";
            public const string PerformanceCurveImportAnyFromLibraryAccess = "PerformanceCurveImportAnyFromLibraryAccess";
            public const string PerformanceCurveViewAnyFromLibraryAccess = "PerformanceCurveViewAnyFromLibraryAccess";
            public const string PerformanceCurveModifyAnyFromScenarioAccess = "PerformanceCurveModifyAnyFromScenarioAccess";
            public const string PerformanceCurveViewAnyFromScenarioAccess = "PerformanceCurveViewAnyFromScenarioAccess";
            public const string PerformanceCurveImportAnyFromScenarioAccess = "PerformanceCurveImportAnyFromScenarioAccess";
            public const string RemainingLifeLimitAddAnyFromLibraryAccess = "RemainingLifeLimitAddAnyFromLibraryAccess";
            public const string RemainingLifeLimitUpdateAnyFromLibraryAccess = "RemainingLifeLimitUpdateAnyFromLibraryAccess";
            public const string RemainingLifeLimitDeleteAnyFromLibraryAccess = "RemainingLifeLimitDeleteAnyFromLibraryAccess";
            public const string RemainingLifeLimitViewAnyFromLibraryAccess = "RemainingLifeLimitViewAnyFromLibraryAccess";
            public const string RemainingLifeLimitModifyAnyFromScenarioAccess = "RemainingLifeLimitModifyAnyFromScenarioAccess";
            public const string RemainingLifeLimitViewAnyFromScenarioAccess = "RemainingLifeLimitViewAnyFromScenarioAccess";
            public const string TargetConditionGoalAddAnyFromLibraryAccess = "TargetConditionGoalAddAnyFromLibraryAccess";
            public const string TargetConditionGoalUpdateAnyFromLibraryAccess = "TargetConditionGoalUpdateAnyFromLibraryAccess";
            public const string TargetConditionGoalDeleteAnyFromLibraryAccess = "TargetConditionGoalDeleteAnyFromLibraryAccess";
            public const string TargetConditionGoalViewAnyFromLibraryAccess = "TargetConditionGoalViewAnyFromLibraryAccess";
            public const string TargetConditionGoalModifyAnyFromScenarioAccess = "TargetConditionGoalModifyAnyFromScenarioAccess";
            public const string TargetConditionGoalViewAnyFromScenarioAccess = "TargetConditionGoalViewAnyFromScenarioAccess";
            public const string TreatmentAddAnyFromLibraryAccess = "TreatmentAddAnyFromLibraryAccess";
            public const string TreatmentUpdateAnyFromLibraryAccess = "TreatmentUpdateAnyFromLibraryAccess";
            public const string TreatmentDeleteAnyFromLibraryAccess = "TreatmentDeleteAnyFromLibraryAccess";
            public const string TreatmentImportAnyFromLibraryAccess = "TreatmentImportAnyFromLibraryAccess";
            public const string TreatmentViewAnyFromLibraryAccess = "TreatmentViewAnyFromLibraryAccess";
            public const string TreatmentModifyAnyFromScenarioAccess = "TreatmentModifyAnyFromScenarioAccess";
            public const string TreatmentImportAnyFromScenarioAccess = "TreatmentImportAnyFromScenarioAccess";
            public const string TreatmentViewAnyFromScenarioAccess = "TreatmentViewAnyFromScenarioAccess";
            public const string UserCriteriaModifyAccess = "UserCriteriaModifyAccess";
            public const string UserCriteriaViewAccess = "UserCriteriaViewAccess";
            public const string AnalysisMethodModifyAnyAccess = "AnalysisMethodModifyAnyAccess";
            public const string AnalysisMethodViewAnyAccess = "AnalysisMethodViewAnyAccess";
            public const string CommittedProjectModifyAnyAccess = "CommittedProjectModifyAnyAccess";
            public const string CommittedProjectImportAnyAccess = "CommittedProjectImportAnyAccess";
            public const string CommittedProjectViewAnyAccess = "CommittedProjectViewAnyAccess";
            public const string SimulationUpdatePermittedAccess = "SimulationUpdatePermittedAccess";
            public const string SimulationDeletePermittedAccess = "SimulationDeletePermittedAccess";
            public const string SimulationRunPermittedAccess = "SimulationRunPermittedAccess";
            public const string SimulationViewPermittedAccess = "SimulationViewPermittedAccess";
            public const string BudgetPriorityAddPermittedFromLibraryAccess = "BudgetPriorityAddPermittedFromLibraryAccess";
            public const string BudgetPriorityUpdatePermittedFromLibraryAccess = "BudgetPriorityUpdatePermittedFromLibraryAccess";
            public const string BudgetPriorityDeletePermittedFromLibraryAccess = "BudgetPriorityDeletePermittedFromLibraryAccess";
            public const string BudgetPriorityViewPermittedFromLibraryAccess = "BudgetPriorityViewPermittedFromLibraryAccess";
            public const string BudgetPriorityModifyPermittedFromScenarioAccess = "BudgetPriorityModifyPermittedFromScenarioAccess";
            public const string BudgetPriorityViewPermittedFromScenarioAccess = "BudgetPriorityViewPermittedFromScenarioAccess";
            public const string CashFlowModifyPermittedFromLibraryAccess = "CashFlowModifyPermittedFromLibraryAccess";
            public const string CashFlowViewPermittedFromLibraryAccess = "CashFlowViewPermittedFromLibraryAccess";
            public const string CashFlowModifyPermittedFromScenarioAccess = "CashFlowModifyPermittedFromScenarioAccess";
            public const string CashFlowViewPermittedFromScenarioAccess = "CashFlowViewPermittedFromScenarioAccess";
            public const string DeficientConditionGoalModifyPermittedFromLibraryAccess = "DeficientConditionGoalModifyPermittedFromLibraryAccess";
            public const string DeficientConditionGoalViewPermittedFromLibraryAccess = "DeficientConditionGoalViewPermittedFromLibraryAccess";
            public const string DeficientConditionGoalModifyPermittedFromScenarioAccess = "DeficientConditionGoalModifyPermittedFromScenarioAccess";
            public const string DeficientConditionGoalViewPermittedFromScenarioAccess = "DeficientConditionGoalViewPermittedFromScenarioAccess";
            public const string InvestmentModifyPermittedFromLibraryAccess = "InvestmentModifyPermittedFromLibraryAccess";
            public const string InvestmentImportPermittedFromLibraryAccess = "InvestmentImportPermittedFromLibraryAccess";
            public const string InvestmentViewPermittedFromLibraryAccess = "InvestmentViewPermittedFromLibraryAccess";
            public const string InvestmentModifyPermittedFromScenarioAccess = "InvestmentModifyPermittedFromScenarioAccess";
            public const string InvestmentImportPermittedFromScenarioAccess = "InvestmentImportPermittedFromScenarioAccess";
            public const string InvestmentViewPermittedFromScenarioAccess = "InvestmentViewPermittedFromScenarioAccess";
            public const string PerformanceCurveAddPermittedFromLibraryAccess = "PerformanceCurveAddPermittedFromLibraryAccess";
            public const string PerformanceCurveUpdatePermittedFromLibraryAccess = "PerformanceCurveUpdatePermittedFromLibraryAccess";
            public const string PerformanceCurveDeletePermittedFromLibraryAccess = "PerformanceCurveDeletePermittedFromLibraryAccess";
            public const string PerformanceCurveImportPermittedFromLibraryAccess = "PerformanceCurveImportPermittedFromLibraryAccess";
            public const string PerformanceCurveViewPermittedFromLibraryAccess = "PerformanceCurveViewPermittedFromLibraryAccess";
            public const string PerformanceCurveModifyPermittedFromScenarioAccess = "PerformanceCurveModifyPermittedFromScenarioAccess";
            public const string PerformanceCurveImportPermittedFromScenarioAccess = "PerformanceCurveImportPermittedFromScenarioAccess";
            public const string PerformanceCurveViewPermittedFromScenarioAccess = "PerformanceCurveViewPermittedFromScenarioAccess";
            public const string RemainingLifeLimitAddPermittedFromLibraryAccess = "RemainingLifeLimitAddPermittedFromLibraryAccess";
            public const string RemainingLifeLimitUpdatePermittedFromLibraryAccess = "RemainingLifeLimitUpdatePermittedFromLibraryAccess";
            public const string RemainingLifeLimitDeletePermittedFromLibraryAccess = "RemainingLifeLimitDeletePermittedFromLibraryAccess";
            public const string RemainingLifeLimitViewPermittedFromLibraryAccess = "RemainingLifeLimitViewPermittedFromLibraryAccess";
            public const string RemainingLifeLimitModifyPermittedFromScenarioAccess = "RemainingLifeLimitModifyPermittedFromScenarioAccess";
            public const string RemainingLifeLimitViewPermittedFromScenarioAccess = "RemainingLifeLimitViewPermittedFromScenarioAccess";
            public const string TargetConditionGoalAddPermittedFromLibraryAccess = "TargetConditionGoalAddPermittedFromLibraryAccess";
            public const string TargetConditionGoalUpdatePermittedFromLibraryAccess = "TargetConditionGoalUpdatePermittedFromLibraryAccess";
            public const string TargetConditionGoalDeletePermittedFromLibraryAccess = "TargetConditionGoalDeletePermittedFromLibraryAccess";
            public const string TargetConditionGoalViewPermittedFromLibraryAccess = "TargetConditionGoalViewPermittedFromLibraryAccess";
            public const string TargetConditionGoalModifyPermittedFromScenarioAccess = "TargetConditionGoalModifyPermittedFromScenarioAccess";
            public const string TargetConditionGoalViewPermittedFromScenarioAccess = "TargetConditionGoalViewPermittedFromScenarioAccess";
            public const string TreatmentAddPermittedFromLibraryAccess = "TreatmentAddPermittedFromLibraryAccess";
            public const string TreatmentUpdatePermittedFromLibraryAccess = "TreatmentUpdatePermittedFromLibraryAccess";
            public const string TreatmentDeletePermittedFromLibraryAccess = "TreatmentDeletePermittedFromLibraryAccess";
            public const string TreatmentImportPermittedFromLibraryAccess = "TreatmentImportPermittedFromLibraryAccess";
            public const string TreatmentViewPermittedFromLibraryAccess = "TreatmentViewPermittedFromLibraryAccess";
            public const string TreatmentModifyPermittedFromScenarioAccess = "TreatmentModifyPermittedFromScenarioAccess";
            public const string TreatmentImportPermittedFromScenarioAccess = "TreatmentImportPermittedFromScenarioAccess";
            public const string TreatmentViewPermittedFromScenarioAccess = "TreatmentViewPermittedFromScenarioAccess";
            public const string AnalysisMethodModifyPermittedAccess = "AnalysisMethodModifyPermittedAccess";
            public const string AnalysisMethodViewPermittedAccess = "AnalysisMethodViewPermittedAccess";
            public const string CommittedProjectModifyPermittedAccess = "CommittedProjectModifyPermittedAccess";
            public const string CommittedProjectImportPermittedAccess = "CommittedProjectImportPermittedAccess";
            public const string CommittedProjectViewPermittedAccess = "CommittedProjectViewPermittedAccess";
            public const string AdminAccess = "AdminAccess";
        }
    }
}
