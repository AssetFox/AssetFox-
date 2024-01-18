BEGIN TRANSACTION;
GO

CREATE OR ALTER PROCEDURE dbo.usp_orphan_cleanup(@RetMessage VARCHAR(250) OUTPUT)
	          AS
	              BEGIN 
	
	            DECLARE @CustomErrorMessage NVARCHAR(MAX),
	           @ErrorNumber int,
	           @ErrorSeverity int,
	           @ErrorState int,
	           @ErrorProcedure nvarchar(126),
	           @ErrorLine int,
	           @ErrorMessage nvarchar(4000);
	           Set  @RetMessage = 'Success';
	           DECLARE @CurrentDateTime DATETIME;
	           DECLARE @BatchSize INT = 100000;
	           DECLARE @RowsDeleted INT = 1;
	
	            BEGIN TRY
	           Begin Transaction
	
	
	          Delete FROM dbo.[AnalysisMaintainableAsset] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [AnalysisMaintainableAsset].SpatialWeightingId);
	          Delete FROM dbo.[AnalysisMaintainableAsset] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [AnalysisMaintainableAsset].SpatialWeightingId);
	          Delete FROM dbo.[AnalysisMaintainableAsset] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [AnalysisMaintainableAsset].SpatialWeightingId);
	          Delete FROM dbo.[AnalysisMaintainableAsset] WHERE NOT EXISTS (SELECT * FROM dbo.[Network] AS parent WHERE parent.Id = [AnalysisMaintainableAsset].NetworkId);
	          Delete FROM dbo.[AnalysisMethod] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [AnalysisMethod].AttributeId);
	          Delete FROM dbo.[AnalysisMethod] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [AnalysisMethod].SimulationId);
	          Delete FROM dbo.[AssetDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[MaintainableAsset] AS parent WHERE parent.Id = [AssetDetail].MaintainableAssetId);
	          Delete FROM dbo.[AssetDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[SimulationYearDetail] AS parent WHERE parent.Id = [AssetDetail].SimulationYearDetailId);
	          Delete FROM dbo.[AssetDetailValueIntId] WHERE NOT EXISTS (SELECT * FROM dbo.[AssetDetail] AS parent WHERE parent.Id = [AssetDetailValueIntId].AssetDetailId);
	          Delete FROM dbo.[AssetDetailValueIntId] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [AssetDetailValueIntId].AttributeId);
	          Delete FROM dbo.[AssetSummaryDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[MaintainableAsset] AS parent WHERE parent.Id = [AssetSummaryDetail].MaintainableAssetId);
	          Delete FROM dbo.[AssetSummaryDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[SimulationOutput] AS parent WHERE parent.Id = [AssetSummaryDetail].SimulationOutputId);
	          Delete FROM dbo.[AssetSummaryDetailValueIntId] WHERE NOT EXISTS (SELECT * FROM dbo.[AssetSummaryDetail] AS parent WHERE parent.Id = [AssetSummaryDetailValueIntId].AssetSummaryDetailId);
	          Delete FROM dbo.[AssetSummaryDetailValueIntId] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [AssetSummaryDetailValueIntId].AttributeId);
	          --Delete FROM dbo.[Attribute] WHERE NOT EXISTS (SELECT * FROM dbo.[DataSource] AS parent WHERE parent.Id = [Attribute].DataSourceId);
	          Delete FROM dbo.[Attribute_Equation_CriterionLibrary] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [Attribute_Equation_CriterionLibrary].AttributeId);
	          Delete FROM dbo.[Attribute_Equation_CriterionLibrary] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [Attribute_Equation_CriterionLibrary].CriterionLibraryId);
	          Delete FROM dbo.[Attribute_Equation_CriterionLibrary] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [Attribute_Equation_CriterionLibrary].EquationId);
	          Delete FROM dbo.[AttributeDatum] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [AttributeDatum].AttributeId);
	          Delete FROM dbo.[AttributeDatum] WHERE NOT EXISTS (SELECT * FROM dbo.[MaintainableAsset] AS parent WHERE parent.Id = [AttributeDatum].MaintainableAssetId);
	          Delete FROM dbo.[AttributeDatumLocation] WHERE NOT EXISTS (SELECT * FROM dbo.[AttributeDatum] AS parent WHERE parent.Id = [AttributeDatumLocation].AttributeDatumId);
	          Delete FROM dbo.[Benefit] WHERE NOT EXISTS (SELECT * FROM dbo.[AnalysisMethod] AS parent WHERE parent.Id = [Benefit].AnalysisMethodId);
	          Delete FROM dbo.[Benefit] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [Benefit].AttributeId);
	          Delete FROM dbo.[BenefitQuantifier] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [BenefitQuantifier].EquationId);
	          Delete FROM dbo.[BenefitQuantifier] WHERE NOT EXISTS (SELECT * FROM dbo.[Network] AS parent WHERE parent.Id = [BenefitQuantifier].NetworkId);
	          Delete FROM dbo.[Budget] WHERE NOT EXISTS (SELECT * FROM dbo.[BudgetLibrary] AS parent WHERE parent.Id = [Budget].BudgetLibraryId);
	          Delete FROM dbo.[BudgetAmount] WHERE NOT EXISTS (SELECT * FROM dbo.[Budget] AS parent WHERE parent.Id = [BudgetAmount].BudgetId);
	          Delete FROM dbo.[BudgetDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[SimulationYearDetail] AS parent WHERE parent.Id = [BudgetDetail].SimulationYearDetailId);
	          Delete FROM dbo.[BudgetLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[BudgetLibrary] AS parent WHERE parent.Id = [BudgetLibrary_User].BudgetLibraryId);
	          Delete FROM dbo.[BudgetLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [BudgetLibrary_User].UserId);
	          Delete FROM dbo.[BudgetPercentagePair] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioBudget] AS parent WHERE parent.Id = [BudgetPercentagePair].ScenarioBudgetId);
	          Delete FROM dbo.[BudgetPercentagePair] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioBudgetPriority] AS parent WHERE parent.Id = [BudgetPercentagePair].ScenarioBudgetPriorityId);
	          Delete FROM dbo.[BudgetPriority] WHERE NOT EXISTS (SELECT * FROM dbo.[BudgetPriorityLibrary] AS parent WHERE parent.Id = [BudgetPriority].BudgetPriorityLibraryId);
	          Delete FROM dbo.[BudgetPriorityLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[BudgetPriorityLibrary] AS parent WHERE parent.Id = [BudgetPriorityLibrary_User].LibraryId);
	          Delete FROM dbo.[BudgetPriorityLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [BudgetPriorityLibrary_User].UserId);
	          Delete FROM dbo.[BudgetUsageDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[TreatmentConsiderationDetail] AS parent WHERE parent.Id = [BudgetUsageDetail].TreatmentConsiderationDetailId);
	          Delete FROM dbo.[CalculatedAttribute] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [CalculatedAttribute].AttributeId);
	          Delete FROM dbo.[CalculatedAttribute] WHERE NOT EXISTS (SELECT * FROM dbo.[CalculatedAttributeLibrary] AS parent WHERE parent.Id = [CalculatedAttribute].CalculatedAttributeLibraryId);
	          Delete FROM dbo.[CalculatedAttributeLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[CalculatedAttributeLibrary] AS parent WHERE parent.Id = [CalculatedAttributeLibrary_User].LibraryId);
	          Delete FROM dbo.[CalculatedAttributeLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [CalculatedAttributeLibrary_User].UserId);
	          Delete FROM dbo.[CalculatedAttributePair] WHERE NOT EXISTS (SELECT * FROM dbo.[CalculatedAttribute] AS parent WHERE parent.Id = [CalculatedAttributePair].CalculatedAttributeId);
	          Delete FROM dbo.[CalculatedAttributePair_Criteria] WHERE NOT EXISTS (SELECT * FROM dbo.[CalculatedAttributePair] AS parent WHERE parent.Id = [CalculatedAttributePair_Criteria].CalculatedAttributePairId);
	          Delete FROM dbo.[CalculatedAttributePair_Criteria] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CalculatedAttributePair_Criteria].CriterionLibraryId);
	          Delete FROM dbo.[CalculatedAttributePair_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[CalculatedAttributePair] AS parent WHERE parent.Id = [CalculatedAttributePair_Equation].CalculatedAttributePairId);
	          Delete FROM dbo.[CalculatedAttributePair_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [CalculatedAttributePair_Equation].EquationId);
	          Delete FROM dbo.[CashFlowConsiderationDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[TreatmentConsiderationDetail] AS parent WHERE parent.Id = [CashFlowConsiderationDetail].TreatmentConsiderationDetailId);
	          Delete FROM dbo.[CashFlowDistributionRule] WHERE NOT EXISTS (SELECT * FROM dbo.[CashFlowRule] AS parent WHERE parent.Id = [CashFlowDistributionRule].CashFlowRuleId);
	          Delete FROM dbo.[CashFlowRule] WHERE NOT EXISTS (SELECT * FROM dbo.[CashFlowRuleLibrary] AS parent WHERE parent.Id = [CashFlowRule].CashFlowRuleLibraryId);
	          Delete FROM dbo.[CashFlowRuleLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[CashFlowRuleLibrary] AS parent WHERE parent.Id = [CashFlowRuleLibrary_User].LibraryId);
	          Delete FROM dbo.[CashFlowRuleLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [CashFlowRuleLibrary_User].UserId);
	          Delete FROM dbo.[CommittedProject] WHERE NOT EXISTS (SELECT * FROM dbo.[AnalysisMaintainableAsset] AS parent WHERE parent.Id = [CommittedProject].AnalysisMaintainableAssetEntityId);
	          Delete FROM dbo.[CommittedProject] WHERE NOT EXISTS (SELECT * FROM dbo.[MaintainableAsset] AS parent WHERE parent.Id = [CommittedProject].MaintainableAssetEntityId);
	          --Delete FROM dbo.[CommittedProject] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioBudget] AS parent WHERE parent.Id = [CommittedProject].ScenarioBudgetId);
	          Delete FROM dbo.[CommittedProject] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [CommittedProject].SimulationId);
	          Delete FROM dbo.[CommittedProjectConsequence] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [CommittedProjectConsequence].AttributeId);
	          Delete FROM dbo.[CommittedProjectConsequence] WHERE NOT EXISTS (SELECT * FROM dbo.[CommittedProject] AS parent WHERE parent.Id = [CommittedProjectConsequence].CommittedProjectId);
	          Delete FROM dbo.[CommittedProjectLocation] WHERE NOT EXISTS (SELECT * FROM dbo.[CommittedProject] AS parent WHERE parent.Id = [CommittedProjectLocation].CommittedProjectId);
	          Delete FROM dbo.[CriterionLibrary_AnalysisMethod] WHERE NOT EXISTS (SELECT * FROM dbo.[AnalysisMethod] AS parent WHERE parent.Id = [CriterionLibrary_AnalysisMethod].AnalysisMethodId);
	          Delete FROM dbo.[CriterionLibrary_AnalysisMethod] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_AnalysisMethod].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_Budget] WHERE NOT EXISTS (SELECT * FROM dbo.[Budget] AS parent WHERE parent.Id = [CriterionLibrary_Budget].BudgetId);
	          Delete FROM dbo.[CriterionLibrary_Budget] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_Budget].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_BudgetPriority] WHERE NOT EXISTS (SELECT * FROM dbo.[BudgetPriority] AS parent WHERE parent.Id = [CriterionLibrary_BudgetPriority].BudgetPriorityId);
	          Delete FROM dbo.[CriterionLibrary_BudgetPriority] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_BudgetPriority].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_CashFlowRule] WHERE NOT EXISTS (SELECT * FROM dbo.[CashFlowRule] AS parent WHERE parent.Id = [CriterionLibrary_CashFlowRule].CashFlowRuleId);
	          Delete FROM dbo.[CriterionLibrary_CashFlowRule] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_CashFlowRule].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_DeficientConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_DeficientConditionGoal].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_DeficientConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[DeficientConditionGoal] AS parent WHERE parent.Id = [CriterionLibrary_DeficientConditionGoal].DeficientConditionGoalId);
	          Delete FROM dbo.[CriterionLibrary_PerformanceCurve] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_PerformanceCurve].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_PerformanceCurve] WHERE NOT EXISTS (SELECT * FROM dbo.[PerformanceCurve] AS parent WHERE parent.Id = [CriterionLibrary_PerformanceCurve].PerformanceCurveId);
	          Delete FROM dbo.[CriterionLibrary_RemainingLifeLimit] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_RemainingLifeLimit].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_RemainingLifeLimit] WHERE NOT EXISTS (SELECT * FROM dbo.[RemainingLifeLimit] AS parent WHERE parent.Id = [CriterionLibrary_RemainingLifeLimit].RemainingLifeLimitId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioBudget] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioBudget].CriterionLibraryId);
	          --Delete FROM dbo.[CriterionLibrary_ScenarioBudget] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioBudget] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioBudget].ScenarioBudgetId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioBudgetPriority] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioBudgetPriority].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioBudgetPriority] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioBudgetPriority] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioBudgetPriority].ScenarioBudgetPriorityId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioCashFlowRule] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioCashFlowRule].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioCashFlowRule] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioCashFlowRule] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioCashFlowRule].ScenarioCashFlowRuleId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioDeficientConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioDeficientConditionGoal].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioDeficientConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioDeficientConditionGoal] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioDeficientConditionGoal].ScenarioDeficientConditionGoalId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioPerformanceCurve] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioPerformanceCurve].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioPerformanceCurve] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioPerformanceCurve] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioPerformanceCurve].ScenarioPerformanceCurveId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioRemainingLifeLimit] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioRemainingLifeLimit].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioRemainingLifeLimit] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioRemainingLifeLimit] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioRemainingLifeLimit].ScenarioRemainingLifeLimitId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTargetConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTargetConditionGoal].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTargetConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioTargetConditionGoals] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTargetConditionGoal].ScenarioTargetConditionGoalId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTreatment] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTreatment].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTreatment] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioSelectableTreatment] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTreatment].ScenarioSelectableTreatmentId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTreatmentConsequence] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTreatmentConsequence].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTreatmentConsequence] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioConditionalTreatmentConsequences] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTreatmentConsequence].ScenarioConditionalTreatmentConsequenceId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTreatmentCost] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTreatmentCost].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTreatmentCost] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioTreatmentCost] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTreatmentCost].ScenarioTreatmentCostId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTreatmentSupersedeRule].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_ScenarioTreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioTreatmentSupersedeRule] AS parent WHERE parent.Id = [CriterionLibrary_ScenarioTreatmentSupersedeRule].ScenarioTreatmentSupersedeRuleId);
	          Delete FROM dbo.[CriterionLibrary_TargetConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_TargetConditionGoal].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_TargetConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[TargetConditionGoal] AS parent WHERE parent.Id = [CriterionLibrary_TargetConditionGoal].TargetConditionGoalId);
	          Delete FROM dbo.[CriterionLibrary_Treatment] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_Treatment].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_Treatment] WHERE NOT EXISTS (SELECT * FROM dbo.[SelectableTreatment] AS parent WHERE parent.Id = [CriterionLibrary_Treatment].SelectableTreatmentId);
	          Delete FROM dbo.[CriterionLibrary_TreatmentConsequence] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_TreatmentConsequence].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_TreatmentConsequence] WHERE NOT EXISTS (SELECT * FROM dbo.[TreatmentConsequence] AS parent WHERE parent.Id = [CriterionLibrary_TreatmentConsequence].ConditionalTreatmentConsequenceId);
	          Delete FROM dbo.[CriterionLibrary_TreatmentCost] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_TreatmentCost].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_TreatmentCost] WHERE NOT EXISTS (SELECT * FROM dbo.[TreatmentCost] AS parent WHERE parent.Id = [CriterionLibrary_TreatmentCost].TreatmentCostId);
	          Delete FROM dbo.[CriterionLibrary_TreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_TreatmentSupersedeRule].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_TreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[TreatmentSupersedeRule] AS parent WHERE parent.Id = [CriterionLibrary_TreatmentSupersedeRule].TreatmentSupersedeRuleId);
	          Delete FROM dbo.[CriterionLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [CriterionLibrary_User].CriterionLibraryId);
	          Delete FROM dbo.[CriterionLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [CriterionLibrary_User].UserId);
	          Delete FROM dbo.[DeficientConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [DeficientConditionGoal].AttributeId);
	          Delete FROM dbo.[DeficientConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[DeficientConditionGoalLibrary] AS parent WHERE parent.Id = [DeficientConditionGoal].DeficientConditionGoalLibraryId);
	          Delete FROM dbo.[DeficientConditionGoalDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [DeficientConditionGoalDetail].AttributeId);
	          Delete FROM dbo.[DeficientConditionGoalDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[SimulationYearDetail] AS parent WHERE parent.Id = [DeficientConditionGoalDetail].SimulationYearDetailId);
	          Delete FROM dbo.[DeficientConditionGoalLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[DeficientConditionGoalLibrary] AS parent WHERE parent.Id = [DeficientConditionGoalLibrary_User].LibraryId);
	          Delete FROM dbo.[DeficientConditionGoalLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [DeficientConditionGoalLibrary_User].UserId);
	          Delete FROM dbo.[ExcelRawData] WHERE NOT EXISTS (SELECT * FROM dbo.[DataSource] AS parent WHERE parent.Id = [ExcelRawData].DataSourceId);
	          Delete FROM dbo.[InvestmentPlan] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [InvestmentPlan].SimulationId);
	          Delete FROM dbo.[MaintainableAsset] WHERE NOT EXISTS (SELECT * FROM dbo.[Network] AS parent WHERE parent.Id = [MaintainableAsset].NetworkId);
	          Delete FROM dbo.[MaintainableAssetLocation] WHERE NOT EXISTS (SELECT * FROM dbo.[MaintainableAsset] AS parent WHERE parent.Id = [MaintainableAssetLocation].MaintainableAssetId);
	          Delete FROM dbo.[NetworkAttribute] WHERE NOT EXISTS (SELECT * FROM dbo.[Network] AS parent WHERE parent.Id = [NetworkAttribute].NetworkId);
	          Delete FROM dbo.[NetworkRollupDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[Network] AS parent WHERE parent.Id = [NetworkRollupDetail].NetworkId);
	          Delete FROM dbo.[NumericAttributeValueHistory] WHERE NOT EXISTS (SELECT * FROM dbo.[AnalysisMaintainableAsset] AS parent WHERE parent.Id = [NumericAttributeValueHistory].SectionId);
	          Delete FROM dbo.[NumericAttributeValueHistory] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [NumericAttributeValueHistory].AttributeId);
	          Delete FROM dbo.[PerformanceCurve] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [PerformanceCurve].AttributeId);
	          Delete FROM dbo.[PerformanceCurve] WHERE NOT EXISTS (SELECT * FROM dbo.[PerformanceCurveLibrary] AS parent WHERE parent.Id = [PerformanceCurve].PerformanceCurveLibraryId);
	          Delete FROM dbo.[PerformanceCurve_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [PerformanceCurve_Equation].EquationId);
	          Delete FROM dbo.[PerformanceCurve_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[PerformanceCurve] AS parent WHERE parent.Id = [PerformanceCurve_Equation].PerformanceCurveId);
	          Delete FROM dbo.[PerformanceCurveLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[PerformanceCurveLibrary] AS parent WHERE parent.Id = [PerformanceCurveLibrary_User].LibraryId);
	          Delete FROM dbo.[PerformanceCurveLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [PerformanceCurveLibrary_User].UserEntityId);
	          Delete FROM dbo.[PerformanceCurveLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [PerformanceCurveLibrary_User].UserId);
	          Delete FROM dbo.[RemainingLifeLimit] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [RemainingLifeLimit].AttributeId);
	          Delete FROM dbo.[RemainingLifeLimit] WHERE NOT EXISTS (SELECT * FROM dbo.[RemainingLifeLimitLibrary] AS parent WHERE parent.Id = [RemainingLifeLimit].RemainingLifeLimitLibraryId);
	          Delete FROM dbo.[RemainingLifeLimitLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[RemainingLifeLimitLibrary] AS parent WHERE parent.Id = [RemainingLifeLimitLibrary_User].LibraryId);
	          Delete FROM dbo.[RemainingLifeLimitLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [RemainingLifeLimitLibrary_User].UserId);
	          Delete FROM dbo.[ReportIndex] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ReportIndex].SimulationID);
	          Delete FROM dbo.[ScenarioBudget] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ScenarioBudget].SimulationId);
	          Delete FROM dbo.[ScenarioBudgetAmount] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioBudget] AS parent WHERE parent.Id = [ScenarioBudgetAmount].ScenarioBudgetId);
	          Delete FROM dbo.[ScenarioBudgetPriority] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ScenarioBudgetPriority].SimulationId);
	          Delete FROM dbo.[ScenarioCalculatedAttribute] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [ScenarioCalculatedAttribute].AttributeId);
	          Delete FROM dbo.[ScenarioCalculatedAttribute] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ScenarioCalculatedAttribute].SimulationId);
	          Delete FROM dbo.[ScenarioCalculatedAttributePair] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioCalculatedAttribute] AS parent WHERE parent.Id = [ScenarioCalculatedAttributePair].ScenarioCalculatedAttributeId);
	          Delete FROM dbo.[ScenarioCalculatedAttributePair_Criteria] WHERE NOT EXISTS (SELECT * FROM dbo.[CriterionLibrary] AS parent WHERE parent.Id = [ScenarioCalculatedAttributePair_Criteria].CriterionLibraryId);
	          Delete FROM dbo.[ScenarioCalculatedAttributePair_Criteria] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioCalculatedAttributePair] AS parent WHERE parent.Id = [ScenarioCalculatedAttributePair_Criteria].ScenarioCalculatedAttributePairId);
	          --Delete FROM dbo.[ScenarioCalculatedAttributePair_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [ScenarioCalculatedAttributePair_Equation].EquationId);
	          Delete FROM dbo.[ScenarioCalculatedAttributePair_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioCalculatedAttributePair] AS parent WHERE parent.Id = [ScenarioCalculatedAttributePair_Equation].ScenarioCalculatedAttributePairId);
	          Delete FROM dbo.[ScenarioCashFlowDistributionRule] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioCashFlowRule] AS parent WHERE parent.Id = [ScenarioCashFlowDistributionRule].ScenarioCashFlowRuleId);
	          Delete FROM dbo.[ScenarioCashFlowRule] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ScenarioCashFlowRule].SimulationId);
	          Delete FROM dbo.[ScenarioConditionalTreatmentConsequences] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [ScenarioConditionalTreatmentConsequences].AttributeId);
	          Delete FROM dbo.[ScenarioConditionalTreatmentConsequences] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioSelectableTreatment] AS parent WHERE parent.Id = [ScenarioConditionalTreatmentConsequences].ScenarioSelectableTreatmentId);
	          Delete FROM dbo.[ScenarioDeficientConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [ScenarioDeficientConditionGoal].AttributeId);
	          Delete FROM dbo.[ScenarioDeficientConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ScenarioDeficientConditionGoal].SimulationId);
	          Delete FROM dbo.[ScenarioPerformanceCurve] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [ScenarioPerformanceCurve].AttributeId);
	          Delete FROM dbo.[ScenarioPerformanceCurve] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ScenarioPerformanceCurve].SimulationId);
	          Delete FROM dbo.[ScenarioPerformanceCurve_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [ScenarioPerformanceCurve_Equation].EquationId);
	          Delete FROM dbo.[ScenarioPerformanceCurve_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioPerformanceCurve] AS parent WHERE parent.Id = [ScenarioPerformanceCurve_Equation].ScenarioPerformanceCurveId);
	          Delete FROM dbo.[ScenarioRemainingLifeLimit] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [ScenarioRemainingLifeLimit].AttributeId);
	          Delete FROM dbo.[ScenarioRemainingLifeLimit] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ScenarioRemainingLifeLimit].SimulationId);
	          Delete FROM dbo.[ScenarioSelectableTreatment] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ScenarioSelectableTreatment].SimulationId);
	          --Delete FROM dbo.[ScenarioSelectableTreatment_ScenarioBudget] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioBudget] AS parent WHERE parent.Id = [ScenarioSelectableTreatment_ScenarioBudget].ScenarioBudgetId);
	          Delete FROM dbo.[ScenarioSelectableTreatment_ScenarioBudget] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioSelectableTreatment] AS parent WHERE parent.Id = [ScenarioSelectableTreatment_ScenarioBudget].ScenarioSelectableTreatmentId);
	          Delete FROM dbo.[ScenarioTargetConditionGoals] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [ScenarioTargetConditionGoals].AttributeId);
	          Delete FROM dbo.[ScenarioTargetConditionGoals] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [ScenarioTargetConditionGoals].SimulationId);
	          Delete FROM dbo.[ScenarioTreatmentConsequence_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [ScenarioTreatmentConsequence_Equation].EquationId);
	          Delete FROM dbo.[ScenarioTreatmentConsequence_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioConditionalTreatmentConsequences] AS parent WHERE parent.Id = [ScenarioTreatmentConsequence_Equation].ScenarioConditionalTreatmentConsequenceId);
	          Delete FROM dbo.[ScenarioTreatmentCost] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioSelectableTreatment] AS parent WHERE parent.Id = [ScenarioTreatmentCost].ScenarioSelectableTreatmentId);
	          --Delete FROM dbo.[ScenarioTreatmentCost_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [ScenarioTreatmentCost_Equation].EquationId);
	          Delete FROM dbo.[ScenarioTreatmentCost_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioTreatmentCost] AS parent WHERE parent.Id = [ScenarioTreatmentCost_Equation].ScenarioTreatmentCostId);
	          Delete FROM dbo.[ScenarioTreatmentPerformanceFactor] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioSelectableTreatment] AS parent WHERE parent.Id = [ScenarioTreatmentPerformanceFactor].ScenarioSelectableTreatmentId);
	          Delete FROM dbo.[ScenarioTreatmentScheduling] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioSelectableTreatment] AS parent WHERE parent.Id = [ScenarioTreatmentScheduling].TreatmentId);
	          --Delete FROM dbo.[ScenarioTreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioSelectableTreatment] AS parent WHERE parent.Id = [ScenarioTreatmentSupersedeRule].PreventTreatmentId);
	          Delete FROM dbo.[ScenarioTreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioSelectableTreatment] AS parent WHERE parent.Id = [ScenarioTreatmentSupersedeRule].TreatmentId);
	          Delete FROM dbo.[SelectableTreatment] WHERE NOT EXISTS (SELECT * FROM dbo.[TreatmentLibrary] AS parent WHERE parent.Id = [SelectableTreatment].TreatmentLibraryId);
	          Delete FROM dbo.[Simulation] WHERE NOT EXISTS (SELECT * FROM dbo.[Network] AS parent WHERE parent.Id = [Simulation].NetworkId);
	          Delete FROM dbo.[Simulation_User] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [Simulation_User].SimulationId);
	          Delete FROM dbo.[Simulation_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [Simulation_User].UserId);
	          Delete FROM dbo.[SimulationAnalysisDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [SimulationAnalysisDetail].SimulationId);
	          --Delete FROM dbo.[SimulationLog] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [SimulationLog].SimulationId);
	          --Delete FROM dbo.[SimulationOutput] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [SimulationOutput].SimulationId);
	          Delete FROM dbo.[SimulationOutputJson] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [SimulationOutputJson].SimulationId);
	          --Delete FROM dbo.[SimulationOutputJson] WHERE NOT EXISTS (SELECT * FROM dbo.[SimulationOutput] AS parent WHERE parent.Id = [SimulationOutputJson].SimulationOutputId);
	          --Delete FROM dbo.[SimulationReportDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[Simulation] AS parent WHERE parent.Id = [SimulationReportDetail].SimulationId);
	          Delete FROM dbo.[SimulationYearDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[SimulationOutput] AS parent WHERE parent.Id = [SimulationYearDetail].SimulationOutputId);
	          Delete FROM dbo.[TargetConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [TargetConditionGoal].AttributeId);
	          Delete FROM dbo.[TargetConditionGoal] WHERE NOT EXISTS (SELECT * FROM dbo.[TargetConditionGoalLibrary] AS parent WHERE parent.Id = [TargetConditionGoal].TargetConditionGoalLibraryId);
	          Delete FROM dbo.[TargetConditionGoalDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [TargetConditionGoalDetail].AttributeId);
	          Delete FROM dbo.[TargetConditionGoalDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[SimulationYearDetail] AS parent WHERE parent.Id = [TargetConditionGoalDetail].SimulationYearDetailId);
	          Delete FROM dbo.[TargetConditionGoalLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[TargetConditionGoalLibrary] AS parent WHERE parent.Id = [TargetConditionGoalLibrary_User].LibraryId);
	          Delete FROM dbo.[TargetConditionGoalLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [TargetConditionGoalLibrary_User].UserId);
	          Delete FROM dbo.[TextAttributeValueHistory] WHERE NOT EXISTS (SELECT * FROM dbo.[AnalysisMaintainableAsset] AS parent WHERE parent.Id = [TextAttributeValueHistory].SectionId);
	          Delete FROM dbo.[TextAttributeValueHistory] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [TextAttributeValueHistory].AttributeId);
	          Delete FROM dbo.[TreatmentConsequence] WHERE NOT EXISTS (SELECT * FROM dbo.[Attribute] AS parent WHERE parent.Id = [TreatmentConsequence].AttributeId);
	          Delete FROM dbo.[TreatmentConsequence] WHERE NOT EXISTS (SELECT * FROM dbo.[SelectableTreatment] AS parent WHERE parent.Id = [TreatmentConsequence].SelectableTreatmentId);
	          --Delete FROM dbo.[TreatmentConsequence_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [TreatmentConsequence_Equation].EquationId);
	          Delete FROM dbo.[TreatmentConsequence_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[TreatmentConsequence] AS parent WHERE parent.Id = [TreatmentConsequence_Equation].ConditionalTreatmentConsequenceId);
	          Delete FROM dbo.[TreatmentConsiderationDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[AssetDetail] AS parent WHERE parent.Id = [TreatmentConsiderationDetail].AssetDetailId);
	          Delete FROM dbo.[TreatmentCost] WHERE NOT EXISTS (SELECT * FROM dbo.[SelectableTreatment] AS parent WHERE parent.Id = [TreatmentCost].TreatmentId);
	          --Delete FROM dbo.[TreatmentCost_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[Equation] AS parent WHERE parent.Id = [TreatmentCost_Equation].EquationId);
	          Delete FROM dbo.[TreatmentCost_Equation] WHERE NOT EXISTS (SELECT * FROM dbo.[TreatmentCost] AS parent WHERE parent.Id = [TreatmentCost_Equation].TreatmentCostId);
	          Delete FROM dbo.[TreatmentLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[TreatmentLibrary] AS parent WHERE parent.Id = [TreatmentLibrary_User].LibraryId);
	          --Delete FROM dbo.[TreatmentLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [TreatmentLibrary_User].UserEntityId);
	          --Delete FROM dbo.[TreatmentLibrary_User] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [TreatmentLibrary_User].UserId);
	          Delete FROM dbo.[TreatmentOptionDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[AssetDetail] AS parent WHERE parent.Id = [TreatmentOptionDetail].AssetDetailId);
	          Delete FROM dbo.[TreatmentPerformanceFactor] WHERE NOT EXISTS (SELECT * FROM dbo.[SelectableTreatment] AS parent WHERE parent.Id = [TreatmentPerformanceFactor].TreatmentId);
	          Delete FROM dbo.[TreatmentRejectionDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[AssetDetail] AS parent WHERE parent.Id = [TreatmentRejectionDetail].AssetDetailId);
	          Delete FROM dbo.[TreatmentScheduling] WHERE NOT EXISTS (SELECT * FROM dbo.[SelectableTreatment] AS parent WHERE parent.Id = [TreatmentScheduling].TreatmentId);
	          Delete FROM dbo.[TreatmentSchedulingCollisionDetail] WHERE NOT EXISTS (SELECT * FROM dbo.[AssetDetail] AS parent WHERE parent.Id = [TreatmentSchedulingCollisionDetail].AssetDetailId);
	          Delete FROM dbo.[TreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[SelectableTreatment] AS parent WHERE parent.Id = [TreatmentSupersedeRule].PreventTreatmentId);
	          Delete FROM dbo.[TreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[SelectableTreatment] AS parent WHERE parent.Id = [TreatmentSupersedeRule].TreatmentId);
	          Delete FROM dbo.[UserCriteria_Filter] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [UserCriteria_Filter].UserId);
	          --14 exceptions
	
	           COMMIT TRANSACTION;
	          END TRY
	          BEGIN CATCH
				        Set @RetMessage = 'Failed ' + @RetMessage;
		        Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()
		        Print 'Rolled Back Orphan SP:  ' + @ErrorMessage;
		        ROLLBACK TRANSACTION;
		        RAISERROR  (@RetMessage, 16, 1);  
	          END CATCH
      END
GO

CREATE OR ALTER PROCEDURE dbo.usp_delete_network(@NetworkId AS uniqueidentifier=NULL,@RetMessage VARCHAR(250) OUTPUT)
	          AS
	              BEGIN 
		          BEGIN TRY

 	                DECLARE @CustomErrorMessage NVARCHAR(MAX),
	                @ErrorNumber int,
	                @ErrorSeverity int,
	                @ErrorState int,
	                @ErrorProcedure nvarchar(126),
	                @ErrorLine int,
	                @ErrorMessage nvarchar(4000);
	                Set  @RetMessage = 'Success';
	                DECLARE @CurrentDateTime DATETIME;
	                DECLARE @BatchSize INT = 100000;
	                DECLARE @RowsDeleted INT = 0;

                -----Start BenefitQuantifier Path-----------------------------------------

			                --Network --> BenefitQuantifier

                            BEGIN TRY

                          ALTER TABLE BenefitQuantifier NOCHECK CONSTRAINT all

		  	                Print 'BenefitQuantifier ';

			                Delete l2 
			                FROM Network AS l1
			                JOIN BenefitQuantifier AS l2 ON l2.NetworkId = l1.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE BenefitQuantifier WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Network --> BenefitQuantifier'
		                         RAISERROR (@CustomErrorMessage, 16, 1);
				                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------End BenefitQuantifier------------------------------------------

			                -----Start NetworkRollupDetail Path-----------------------------------------

			                --Network --> NetworkRollupDetail

                           BEGIN TRY

			                ALTER TABLE NetworkRollupDetail NOCHECK CONSTRAINT all

			                Print 'NetworkRollupDetail ';

			                Delete l2 
			                FROM Network AS l1
			                JOIN NetworkRollupDetail AS l2 ON l2.NetworkId = l1.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE NetworkRollupDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Network -->  NetworkRollupDetail'
		                         RAISERROR (@CustomErrorMessage, 16, 1);
				                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------End NetworkRollupDetail------------------------------------------
			                -----Start NetworkAttribute Path-----------------------------------------

			                --Network --> NetworkAttribute

                            BEGIN TRY

                           ALTER TABLE NetworkAttribute NOCHECK CONSTRAINT all

		                   Print 'NetworkAttribute ';

			                Delete l2 
			                FROM Network AS l1
			                JOIN NetworkAttribute AS l2 ON l2.NetworkId = l1.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE NetworkAttribute WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Network --> NetworkAttribute'
		                         RAISERROR (@CustomErrorMessage, 16, 1);
				                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------End NetworkAttribute------------------------------------------
			                -----Start AnalysisMaintainableAsset Path-----------------------------------------

			                --Network --> AnalysisMaintainableAsset

                            BEGIN TRY

                          ALTER TABLE AnalysisMaintainableAsset NOCHECK CONSTRAINT all

		                  Print 'AnalysisMaintainableAsset ';

			                Delete l2 
			                FROM Network AS l1
			                JOIN AnalysisMaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE AnalysisMaintainableAsset WITH CHECK CHECK CONSTRAINT all;

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Network --> AnalysisMaintainableAsset'
		                         RAISERROR (@CustomErrorMessage, 16, 1);
				                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

                   ------End AnalysisMaintainableAsset------------------------------------------
	                ----- Start MaintainableAsset Path
	                -----Start AggregatedResult Path-----------------------------------------

			                --MaintainableAsset --> AggregatedResult

	                BEGIN TRY

		                Set @RowsDeleted = 0;

		                ALTER TABLE AggregatedResult NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AggregatedResult DISABLE;
		
		                Print 'AggregatedResult ';

		                Select l3.* INTO #tempAggregatedResult
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                JOIN AggregatedResult AS l3 ON l3.MaintainableAssetId = l2.Id
		                WHERE Not l1.Id IN (@NetworkId);

		                Select @RowsDeleted = Count(l3.Id) 
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                JOIN AggregatedResult AS l3 ON l3.MaintainableAssetId = l2.Id
		                WHERE l1.Id IN (@NetworkId);

		                Drop  Table AggregatedResult;

		                Select * into AggregatedResult from #tempAggregatedResult;

		                Drop table #tempAggregatedResult;
				
		                ----Print 'Rows Affected Network --> MaintainableAsset-->AggregatedResult: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		                ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all
		                --ALTER INDEX ALL ON AggregatedResult REBUILD;
					
	                END TRY
	                BEGIN CATCH
		                ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AggregatedResult REBUILD;
		                Print 'Query Error in  Network --> MaintainableAsset-->AggregatedResult ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network --> MaintainableAsset-->AggregatedResult'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;
	                END CATCH;

                -----End AggregatedResult -----------------------------------------------------------------


                -------Start AttributeDatum Path-

                --MaintainableAsset --> AttributeDatum --> AttributeDatumLocation -->  -->  --> 

	                BEGIN TRY

		                Set @RowsDeleted = 0;

		                ALTER TABLE AttributeDatumLocation NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AttributeDatumLocation DISABLE;

		                Print 'AttributeDatumLocation ';

		                SELECT l4.*  INTO #tempAttributeDatumLocation
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
		                Join AttributeDatumLocation As l4 ON l4.AttributeDatumId = l3.Id
		                WHERE Not l1.Id IN (@NetworkId);

		                Select @RowsDeleted = Count(l3.Id) 
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
		                Join AttributeDatumLocation As l4 ON l4.AttributeDatumId = l3.Id
		                WHERE l1.Id IN (@NetworkId);

		                Drop  Table AttributeDatumLocation;

		                Select * into AttributeDatumLocation from #tempAttributeDatumLocation;

		                Drop table #tempAttributeDatumLocation;
				
		                --Print 'Rows Affected Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

		                ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AttributeDatumLocation REBUILD;
					
	                END TRY
	                BEGIN CATCH
			                ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all;
			                --ALTER INDEX ALL ON AttributeDatumLocation REBUILD;
			                Print 'Query Error in Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocationt ***Failed***';
			                SELECT ERROR_NUMBER() AS ErrorNumber
			                ,ERROR_SEVERITY() AS ErrorSeverity
			                ,ERROR_STATE() AS ErrorState
			                ,ERROR_PROCEDURE() AS ErrorProcedure
			                ,ERROR_LINE() AS ErrorLine
			                ,ERROR_MESSAGE() AS ErrorMessage;

			                SELECT @CustomErrorMessage = 'Query Error in  Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocation'
			                RAISERROR (@CustomErrorMessage, 16, 1);
			                Set @RetMessage = @CustomErrorMessage;
	                END CATCH;



                --------End AttributeDatumLocation---------------------------------------------------------------

                --------MaintainableAsset --> AttributeDatum--
          
	                BEGIN TRY

		                Set @RowsDeleted = 0;

		                ALTER TABLE AttributeDatum NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AttributeDatum DISABLE;

		                Print 'AttributeDatum ';

		                SELECT l3.*  INTO #tempAttributeDatumA
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
		                WHERE Not l1.Id IN (@NetworkId);

		                Select @RowsDeleted = Count(l3.Id) 
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
		                WHERE l1.Id IN (@NetworkId);

		                DROP TABLE AttributeDatum;

		                Select * into AttributeDatum from #tempAttributeDatumA;

		                Drop table #tempAttributeDatumA;
				
		                --Print 'Rows Affected Network --> MaintainableAsset-->AttributeDatum: ' +  convert(NVARCHAR(50), @RowsDeleted);

		                ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AttributeDatum REBUILD;

	                END TRY
	                BEGIN CATCH
		                ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AttributeDatum REBUILD
		                Print 'Query Error in Network --> MaintainableAsset-->AttributeDatum-->AttributeDatum ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network --> MaintainableAsset-->AttributeDatum-->AttributeDatum'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;
	                END CATCH

	                ----------------------------------------------------------------------
	                -----End AttributeDatum Path--------------------------------------

	                -----Start AssetSummaryDetail Path-----------------------------------------
	                --MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId -->  -->  --> 

	                BEGIN TRY

		                Set @RowsDeleted = 0;

		                ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AssetSummaryDetailValueIntId DISABLE;

		                Print 'AssetSummaryDetailValueIntId ';

		                Select l4.* INTO #tempAssetSummaryDetailValueIntId
						                FROM Network AS l1
						                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						                JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
						                Join AssetSummaryDetailValueIntId As l4 ON l4.AssetSummaryDetailId = l3.Id
						                WHERE Not l1.Id IN (@NetworkId);

		                Select @RowsDeleted = Count(l4.Id) 
						                FROM Network AS l1
						                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						                JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
						                Join AssetSummaryDetailValueIntId As l4 ON l4.AssetSummaryDetailId = l3.Id
						                WHERE l1.Id IN (@NetworkId);

		                Drop  Table AssetSummaryDetailValueIntId;

		                Select * into AssetSummaryDetailValueIntId from #tempAssetSummaryDetailValueIntId;

		                Drop table #tempAssetSummaryDetailValueIntId;
					
		                --Print 'Rows Affected MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		                ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AssetSummaryDetailValueIntId REBUILD;

	                END TRY 
	                BEGIN CATCH
		                ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AssetSummaryDetailValueIntId REBUILD;
		                Print 'Query Error in Network --> MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;

	                END CATCH

			                -----------------------------------------------------------------------
    		                -------MaintainableAsset --> AssetSummaryDetail -----

	                BEGIN TRY

		                ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all
		                SET @RowsDeleted = 1;
		    
		                Print 'AssetSummaryDetail ';

		                WHILE @RowsDeleted > 0
		                BEGIN
		                SELECT TOP (@BatchSize) l3.Id INTO #tempAssetSummaryDetail
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		                WHERE l1.Id IN (@NetworkId);

		                DELETE FROM AssetSummaryDetail WHERE Id in (SELECT Id FROM #tempAssetSummaryDetail);

		                SET @RowsDeleted = @@ROWCOUNT;

		                DROP TABLE #tempAssetSummaryDetail;
					
		                --Print 'Rows Affected --MaintainableAsset --> AssetSummaryDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
 
		                END
		
		                ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all

	                END TRY 
	                BEGIN CATCH
		                ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AssetSummaryDetail REBUILD;
		                Print 'Query Error in Network --> MaintainableAsset --> AssetSummaryDetail ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetSummaryDetail'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;
	                END CATCH

	                -----End AssetSummaryDetail Path--------------------------------------

                --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail -->  --> 
                ---Start BudgetUsageDetail --------------------------------------------------------------------

	                BEGIN TRY

		                Set @RowsDeleted = 0;

		                ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON BudgetUsageDetail DISABLE;

		                Print 'BudgetUsageDetail1 ';

		                Select l5.* INTO  #tempBudgetUsageDetail
			                FROM Network AS l1
			                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			                JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			                JOIN BudgetUsageDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			                WHERE Not l1.Id IN (@NetworkId);

		                Select @RowsDeleted = Count(l5.Id) 
			                FROM Network AS l1
			                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			                JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			                JOIN BudgetUsageDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

		                Drop  Table BudgetUsageDetail;

		                Select * into BudgetUsageDetail from #tempBudgetUsageDetail;

		                Drop table #tempBudgetUsageDetail;
					
		                --Print 'Rows Affected --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		                ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON BudgetUsageDetail DISABLE;

	                END TRY
	                BEGIN CATCH
		                ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON BudgetUsageDetail DISABLE;
		                Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;
	                END CATCH
	
	                ---End BudgetUsageDetail----------------------------------------------------------------------

	                -- MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail -->  --> 
	                ---Start CashFlowConsiderationDetail --------------------------------------------------------------------

	                BEGIN TRY

		                Set @RowsDeleted = 0;

		                ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON CashFlowConsiderationDetail DISABLE;

		                Print 'CashFlowConsiderationDetail ';

		                Select l5.* INTO  #tempCashFlowConsiderationDetail
			                FROM Network AS l1
			                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			                JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			                JOIN CashFlowConsiderationDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			                WHERE Not l1.Id IN (@NetworkId);

		                Select @RowsDeleted = Count(l5.Id) 
			                FROM Network AS l1
			                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			                JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			                JOIN CashFlowConsiderationDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

		                Drop  Table CashFlowConsiderationDetail;

		                Select * into CashFlowConsiderationDetail from #tempCashFlowConsiderationDetail;

		                Drop table #tempCashFlowConsiderationDetail;
					
		                --Print 'Rows Affected --MaintainableAsset --> AssetDetail --> CashFlowConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
		
		                ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON CashFlowConsiderationDetail DISABLE;

	                END TRY
	                BEGIN CATCH
		                ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON CashFlowConsiderationDetail DISABLE;
		                Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> CashFlowConsiderationDetail ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> CashFlowConsiderationDetail'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;
	                END CATCH
	
	                ---End CashFlowConsiderationDetail --------------------------------------------------------------------
	                ---Start TreatmentConsiderationDetail --------------------------------------------------------------------
	
		                --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail 

	                BEGIN TRY

		                Set @RowsDeleted = 0;

		                ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentConsiderationDetail DISABLE;

		                Print 'TreatmentConsiderationDetail1 ';

		                SELECT l4.* INTO #tempTreatmentConsiderationDetail
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		                JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
		                WHERE Not l1.Id IN (@NetworkId);

		                Select @RowsDeleted = Count(l4.Id) 
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		                JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
		                WHERE l1.Id IN (@NetworkId);

		                Drop  Table TreatmentConsiderationDetail;

		                Select * into TreatmentConsiderationDetail from #tempTreatmentConsiderationDetail;

		                Drop table #tempTreatmentConsiderationDetail;
					
		                --Print 'Rows Affected --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
		
		                ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentConsiderationDetail DISABLE;

	                END TRY
	                BEGIN CATCH
		                ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentConsiderationDetail DISABLE;
		                Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;
	                END CATCH

	                ---End TreatmentConsiderationDetail --------------------------------------------------------------------
	                ---Start AssetDetailValueIntId --------------------------------------------------------------------

	                -- Network --> MaintainableAsset --> AssetDetail --> AssetDetailValueIntId -

	                BEGIN TRY

		                Set @RowsDeleted = 0;

		                ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AssetDetailValueIntId DISABLE;

		                Print 'AssetDetailValueIntId ';

		                Select l4.* INTO #tempAssetDetailValueIntId
		                FROM Network AS l1 
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		                JOIN AssetDetailValueIntId AS l4 ON l4.AssetDetailId = l3.Id
		                WHERE Not l1.Id IN (@NetworkId);

		                Select @RowsDeleted = Count(l4.Id) 
		                FROM Network AS l1 
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		                JOIN AssetDetailValueIntId AS l4 ON l4.AssetDetailId = l3.Id
		                WHERE l1.Id IN (@NetworkId);

		                Drop  Table AssetDetailValueIntId;

		                Select * into AssetDetailValueIntId from #tempAssetDetailValueIntId;

		                Drop table #tempAssetDetailValueIntId;
					
		                --Print 'Rows Affected Network --> MaintainableAsset --> AssetDetail --> AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		                ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AssetDetailValueIntId REBUILD;

	                END TRY 
	                BEGIN CATCH
		                ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AssetDetailValueIntId REBUILD;
		                Print 'Query Error in Network --> MaintainableAsset --> AssetSummaryDetail --> AssetDetailValueIntId ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetSummaryDetail --> AssetDetailValueIntId'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;

	                END CATCH

	                ---End AssetDetailValueIntId --------------------------------------------------------------------
	                ---Start TreatmentOptionDetail --------------------------------------------------------------------

	                -- MaintainableAsset --> AssetDetail --> TreatmentOptionDetail -->  -->  --> 

	                BEGIN TRY

		                ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

		                SET @RowsDeleted = 1;
		                Print 'TreatmentOptionDetail ';

		                WHILE @RowsDeleted > 0
		                BEGIN

		                SELECT TOP (@BatchSize) l4.Id INTO #tempTreatmentOptionDetail
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		                JOIN TreatmentOptionDetail AS l4 ON l4.AssetDetailId = l3.Id
		                WHERE l1.Id IN (@NetworkId);

		                DELETE FROM TreatmentOptionDetail WHERE Id in (SELECT Id FROM #tempTreatmentOptionDetail);

		                SET @RowsDeleted = @@ROWCOUNT;

		                DROP TABLE #tempTreatmentOptionDetail;
		                --WAITFOR DELAY '00:00:01';

		                --Print 'Rows Affected MaintainableAsset --> AssetDetail --> TreatmentOptionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
		                END
		                ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all

	                END TRY 
	                BEGIN CATCH
		                ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentOptionDetail REBUILD;
		                Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentOptionDetaild ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentOptionDetail'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;
	                END CATCH


                ---End TreatmentOptionDetail --------------------------------------------------------------------
                ---Start TreatmentOptionDetail --------------------------------------------------------------------
	                -----------------------------------------------------------------

		                -- AssetDetail --> TreatmentRejectionDetail 
			
		                BEGIN TRY

		                Set @RowsDeleted = 0;

		                ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentRejectionDetail DISABLE;

		                Print 'TreatmentRejectionDetail ';

		                Select l4.* INTO #tempTreatmentRejectionDetail		
				                FROM Network AS l1
				                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
				                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
				                JOIN TreatmentRejectionDetail AS l4 ON l4.AssetDetailId = l3.Id
				                WHERE Not l1.Id IN (@NetworkId);

		                Select @RowsDeleted = Count(l4.Id) 	
				                FROM Network AS l1
				                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
				                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
				                JOIN TreatmentRejectionDetail AS l4 ON l4.AssetDetailId = l3.Id
				                WHERE l1.Id IN (@NetworkId);

		                Drop  Table TreatmentRejectionDetail;

		                Select * into TreatmentRejectionDetail from #tempTreatmentRejectionDetail;

		                Drop table #tempTreatmentRejectionDetail;
					
		                --Print 'Rows Affected Network --> MaintainableAsset --> AssetDetail --> TreatmentRejectionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		                ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentRejectionDetail REBUILD;

	                END TRY 
	                BEGIN CATCH
		                ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentOptionDetail REBUILD;
		                Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentRejectionDetail ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentRejectionDetail'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;

	                END CATCH

	                -----------------------------------------------------------------

	                -- AssetDetail --> TreatmentSchedulingCollisionDetail 

	                BEGIN TRY

		                ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentSchedulingCollisionDetail DISABLE;

		                SET @RowsDeleted = 1;
		                Print 'TreatmentSchedulingCollisionDetail ';

		                WHILE @RowsDeleted > 0
		                BEGIN

		                SELECT TOP (@BatchSize) l4.Id INTO #tempTreatmentSchedulingCollisionDetail
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		                JOIN TreatmentSchedulingCollisionDetail AS l4 ON l4.AssetDetailId = l3.Id
		                WHERE l1.Id IN (@NetworkId);

		                DELETE FROM TreatmentSchedulingCollisionDetail WHERE Id in (SELECT Id FROM #tempTreatmentSchedulingCollisionDetail);

		                SET @RowsDeleted = @@ROWCOUNT;

		                DROP TABLE #tempTreatmentSchedulingCollisionDetail;
		                --WAITFOR DELAY '00:00:01';
						
		                --Print 'Rows Affected Network --> MaintainableAsset-->AssetDetail --> TreatmentSchedulingCollisionDetail : ' +  convert(NVARCHAR(50), @RowsDeleted);
		                ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentSchedulingCollisionDetail REBUILD;
		                END

	                END TRY 
	                BEGIN CATCH
		                ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON TreatmentSchedulingCollisionDetail REBUILD;
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset-->AssetDetail -->  TreatmentSchedulingCollisionDetail'
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;

	                END CATCH

	                -----------------------------------------------------------------
	
			                --MaintainableAsset\AssetDetail

	                BEGIN TRY

		                ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

		                SET @RowsDeleted = 1;
		                Print 'AssetDetail ';

		                WHILE @RowsDeleted > 0
		                BEGIN

		                SELECT TOP (@BatchSize) l3.Id INTO #tempAssetDetailId
		                FROM Network AS l1
		                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		                Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		                WHERE l1.Id IN (@NetworkId);

		                DELETE FROM AssetDetail WHERE Id in (SELECT Id FROM #tempAssetDetailId);

		                SET @RowsDeleted = @@ROWCOUNT;

		                DROP TABLE #tempAssetDetailId;
		                ----WAITFOR DELAY '00:00:01';
				
		                --Print 'Rows Affected Network --> MaintainableAsset-->AssetDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
		                END

		                ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all

	                END TRY
	                BEGIN CATCH
		                ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all;
		                --ALTER INDEX ALL ON AssetDetail REBUILD;
		                Print 'Query Error in Network --> MaintainableAsset --> AssetDetail ***Failed***';
		                SELECT ERROR_NUMBER() AS ErrorNumber
		                ,ERROR_SEVERITY() AS ErrorSeverity
		                ,ERROR_STATE() AS ErrorState
		                ,ERROR_PROCEDURE() AS ErrorProcedure
		                ,ERROR_LINE() AS ErrorLine
		                ,ERROR_MESSAGE() AS ErrorMessage;

		                SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail '
		                RAISERROR (@CustomErrorMessage, 16, 1);
		                Set @RetMessage = @CustomErrorMessage;

	                END CATCH


		                ---End  --Network -->MaintainableAsset --> AssetDetail Path---------
	                  -----Start CommittedProject Path-----------------------------------------
			
		                --Network --> MaintainableAsset --> CommittedProject --> CommittedProjectConsequence -->  -->  -->  -->  -->  --> 

                         BEGIN TRY

                            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			                Print 'CommittedProjectConsequence ';

			                Delete l4 
			                FROM Network AS l1
			                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                Join CommittedProject  AS l3 ON l3.MaintainableAssetEntityId = l2.Id
			                JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network Delete CommittedProjectConsequence: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
 	
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectConsequence'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -------------------------------------

			                --Network --> MaintainableAsset --> CommittedProject --> CommittedProjectLocation -->  -->  -->  -->  -->  -->

			                 BEGIN TRY

                            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			                Print 'CommittedProjectLocation ';

			                Delete l4 
			                FROM Network AS l1
			                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                Join CommittedProject  AS l3 ON l3.MaintainableAssetEntityId = l2.Id
			                JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network Delete CommittedProjectLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
 	
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectLocation'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                --------------------------------------

			                --Network --> MaintainableAsset --> CommittedProject 

                            BEGIN TRY

			                ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

			                Print 'CommittedProject ';

			                Delete l3 
			                FROM Network AS l1
			                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                Join CommittedProject AS l3 ON l3.MaintainableAssetEntityId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network Delete CommittedProject: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE CommittedProject WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CommittedProject'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH 

			                -------------------------------------


			                -----End CommittedProject Path-----------------------------------------

		                ----Start Network --> MaintainableAsset --> MaintainableAssetLocation  Path
			
			                BEGIN TRY

                            ALTER TABLE MaintainableAssetLocation NOCHECK CONSTRAINT all

		  	                Print 'MaintainableAssetLocation ';

			                Delete l3
			                FROM Network AS l1
			                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                JOIN MaintainableAssetLocation AS l3 ON l3.MaintainableAssetId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network --> MaintainableAsset-->MaintainableAssetLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE MaintainableAssetLocation WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset-->MaintainableAssetLocation'
		                         RAISERROR (@CustomErrorMessage, 16, 1);
				                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

	                -------End -------MaintainableAssetLocation----------------------------------------------------

			                -- Start MaintainableAsset

 			                BEGIN TRY

                            ALTER TABLE MaintainableAsset NOCHECK CONSTRAINT all

			                Print 'MaintainableAsset';

			                Delete l2 
			                FROM Network AS l1
			                Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network --> MaintainableAsset: ' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE MaintainableAsset WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset'
		                         RAISERROR (@CustomErrorMessage, 16, 1);
				                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------Network --> MaintainableAsset Path--------------------------------------------------------------------
					   			 
		                ----- Start Simulation Path
			
		                    --SET @CurrentDateTime = GETDATE();
			                --PRINT 'Start Simulation Delete: ' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
			                -----Start AnalysisMethod Path-----------------------------------------

                            BEGIN TRY
 
                            ALTER TABLE Benefit NOCHECK CONSTRAINT all

			                Print 'Benefit ';
			                --AnalysisMethod	Benefit							FK_Benefit_AnalysisMethod_AnalysisMethodId
			                --AnalysisMethod	CriterionLibrary_AnalysisMethod	FK_CriterionLibrary_AnalysisMethod_AnalysisMethod_AnalysisMethodId

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
			                JOIN Benefit AS l4 ON l4.AnalysisMethodId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network --> Benefit: ' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE Benefit WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Benefit'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -----------------------------------------------------------------------

                            BEGIN TRY


                            ALTER TABLE CriterionLibrary_AnalysisMethod NOCHECK CONSTRAINT all

			                Print 'CriterionLibrary_AnalysisMethod ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
			                JOIN CriterionLibrary_AnalysisMethod AS l4 ON l4.AnalysisMethodId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network --> CriterionLibrary_AnalysisMethod: ' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE CriterionLibrary_AnalysisMethod WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_AnalysisMethod'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
			                ------------------------------------------------------------------

                            BEGIN TRY

                            ALTER TABLE AnalysisMethod NOCHECK CONSTRAINT all

			                Print 'AnalysisMethod ';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network --> AnalysisMethod: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE AnalysisMethod WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in AnalysisMethod'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
 
 			                -----End Simulation --> AnalysisMethod Path-----------------------------------------
 			                -----------------------------------------------------------------------
			                -----Start Simulation --> CommittedProject Path-----------------------------------------

			                --Simulation --> CommittedProject --> CommittedProjectConsequence 

                         BEGIN TRY

                            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			                Print 'CommittedProjectConsequence ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
			                JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network --> CommittedProjectConsequence: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
 	
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectConsequence'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -------------------------------------

			                --Simulation --> CommittedProject --> CommittedProjectLocation 

			                 BEGIN TRY

                            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			                Print 'CommittedProjectLocation ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
			                JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network --> CommittedProjectLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
 	
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectLocation'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                --------------------------------------

			                --Simulation --> CommittedProject

                            BEGIN TRY

			                ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

			                Print 'CommittedProject ';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId)

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected Network --> CommittedProject: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE CommittedProject WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CommittedProject'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH 


			                -----End CommittedProject Path-----------------------------------------
		                   -----Start Simulation --> InvestmentPlan 


                            BEGIN TRY

			                ALTER TABLE InvestmentPlan NOCHECK CONSTRAINT all

			                Print 'InvestmentPlan';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN InvestmentPlan AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE InvestmentPlan WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in InvestmentPlan'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -----End Simulation --> InvestmentPlan 

		                -----Start Simulation --> ReportIndex 


                            BEGIN TRY
			                Print 'ReportIndex';

			                ALTER TABLE ReportIndex NOCHECK CONSTRAINT all

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ReportIndex AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ReportIndex WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ReportIndex'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -----End Simulation --> ReportIndex ---------------------------
			                -- Start Network --> Simulation --> ScenarioBudget --> BudgetPercentagePair -->

                            BEGIN TRY

			                ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

			                Print 'BudgetPercentagePair';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			                JOIN BudgetPercentagePair AS l4 ON l4.ScenarioBudgetId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE BudgetPercentagePair WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in BudgetPercentagePair'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -- End Simulation --> ScenarioBudget --> BudgetPercentagePair -->

			                -- Start Simulation --> ScenarioBudget --> CommittedProjectConsequence -->

                            BEGIN TRY

                            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			                Print 'CommittedProjectConsequence';

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			                JOIN CommittedProject AS l4 ON l4.ScenarioBudgetId = l3.Id
			                JOIN CommittedProjectConsequence AS l5 ON l5.CommittedProjectId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
 	
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectConsequence'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -------------------------------------

			                --Simulation --> ScenarioBudget --> CommittedProject --> CommittedProjectLocation

			                 BEGIN TRY

                            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			                Print 'CommittedProjectLocation ';

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			                JOIN CommittedProject AS l4 ON l4.ScenarioBudgetId = l3.Id
			                JOIN CommittedProjectLocation AS l5 ON l5.CommittedProjectId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
 	
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectLocation'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                --------------------------------------

		                --Simulation --> ScenarioBudget --> CommittedProject

                            BEGIN TRY

			                ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

			                Print 'CommittedProject ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			                JOIN CommittedProject AS l4 ON l4.ScenarioBudgetId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CommittedProject WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CommittedProject'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH 

			                -----End ScenarioBudget -CommittedProject Path------------

		                -----Start Network --> Simulation --> ScenarioBudget --> CriterionLibrary_ScenarioBudget

                            BEGIN TRY

			                ALTER TABLE CriterionLibrary_ScenarioBudget NOCHECK CONSTRAINT all

			                Print 'CriterionLibrary_ScenarioBudget ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			                JOIN CriterionLibrary_ScenarioBudget AS l4 ON l4.ScenarioBudgetId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioBudget WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioBudget'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
			                -----------------------------------------

			                --Network --> Simulation --> ScenarioBudget --> ScenarioBudgetAmount --> 

                            BEGIN TRY

			                ALTER TABLE ScenarioBudgetAmount NOCHECK CONSTRAINT all

			                Print 'ScenarioBudgetAmount';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioBudgetAmount AS l4 ON l4.ScenarioBudgetId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioBudgetAmount WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioBudgetAmount'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ----------------------------------------------

		                --Network --> Simulation --> ScenarioBudget --> ScenarioSelectableTreatment_ScenarioBudget --> 

                            BEGIN TRY

			                Print 'ScenarioSelectableTreatment_ScenarioBudget';

			                ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget NOCHECK CONSTRAINT all

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioSelectableTreatment_ScenarioBudget AS l4 ON l4.ScenarioBudgetId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioSelectableTreatment_ScenarioBudget'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
  
			                ------------------------------------------------------------

			                --Network --> Simulation --> ScenarioBudget 

                            BEGIN TRY

			                Print 'ScenarioBudget';

			                ALTER TABLE ScenarioBudget NOCHECK CONSTRAINT all

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioBudget WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioBudget'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -----End --Network --> Simulation --> ScenarioBudget Path-----------------------------------------

			                ------Start  --Network --> Simulation --> ScenarioBudgetPriority -----------

			                --Network --> Simulation --> ScenarioBudgetPriority --> CriterionLibrary_ScenarioBudgetPriority --> 

                            BEGIN TRY

			                ALTER TABLE CriterionLibrary_ScenarioBudgetPriority NOCHECK CONSTRAINT all

			                Print 'CriterionLibrary_ScenarioBudgetPriority ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudgetPriority AS l3 ON l3.SimulationId = l2.Id
			                JOIN CriterionLibrary_ScenarioBudgetPriority AS l4 ON l4.ScenarioBudgetPriorityId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioBudgetPriority WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioBudgetPriority'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------------------------------------------------------------

			                --Network --> Simulation --> ScenarioBudgetPriority --> BudgetPercentagePair --> 

                            BEGIN TRY

			                Print 'BudgetPercentagePair';

			                ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudgetPriority AS l3 ON l3.SimulationId = l2.Id
			                JOIN BudgetPercentagePair AS l4 ON l4.ScenarioBudgetPriorityId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE BudgetPercentagePair WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in BudgetPercentagePair'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------------------------------------------------------------

			                --Network --> Simulation --> ScenarioBudgetPriority

                            BEGIN TRY

			                ALTER TABLE ScenarioBudgetPriority NOCHECK CONSTRAINT all

			                Print 'ScenarioBudgetPriority ';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioBudgetPriority AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioBudgetPriority WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioBudgetPriority'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------End Network -->  Simulation --> ScenarioBudgetPriority-------------------------------------------------------------------

			                ------Start  ScenarioCalculatedAttribute-------------------------------------------------------------------
			                --Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair --> ScenarioCalculatedAttributePair_Criteria
			                --Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair --> ScenarioCalculatedAttributePair_Equation


                            BEGIN TRY

			                ALTER TABLE ScenarioCalculatedAttributePair_Criteria NOCHECK CONSTRAINT all

			                Print 'ScenarioCalculatedAttributePair_Criteria';

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioCalculatedAttribute AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioCalculatedAttributePair AS l4 ON l4.ScenarioCalculatedAttributeId = l3.Id
			                JOIN ScenarioCalculatedAttributePair_Criteria AS l5 ON l5.ScenarioCalculatedAttributePairId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioCalculatedAttributePair_Criteria WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttributePair_Criteria'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------------------------------------------------------------

			                --Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair --> ScenarioCalculatedAttributePair_Equation

                            BEGIN TRY

			                ALTER TABLE ScenarioCalculatedAttributePair_Equation NOCHECK CONSTRAINT all

			                Print 'ScenarioCalculatedAttributePair_Equation';

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioCalculatedAttribute AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioCalculatedAttributePair AS l4 ON l4.ScenarioCalculatedAttributeId = l3.Id
			                JOIN ScenarioCalculatedAttributePair_Equation AS l5 ON l5.ScenarioCalculatedAttributePairId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioCalculatedAttributePair_Equation WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttributePair_Equation'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------------------------------------------------------------
			                --Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair 

			                BEGIN TRY

			                ALTER TABLE ScenarioCalculatedAttributePair NOCHECK CONSTRAINT all

			                Print 'ScenarioCalculatedAttributePair';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioCalculatedAttribute AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioCalculatedAttributePair AS l4 ON l4.ScenarioCalculatedAttributeId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioCalculatedAttributePair WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttributePair'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------
			                --Network --> Simulation --> ScenarioCalculatedAttribute 

                            BEGIN TRY

			                Print 'ScenarioCalculatedAttribute ';

			                ALTER TABLE ScenarioCalculatedAttribute NOCHECK CONSTRAINT all

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioCalculatedAttribute AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioCalculatedAttribute WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttribute'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ----End --Network --> Simulation --> ScenarioCalculatedAttribute-------------------------------------------------------------------

			                ------Start  Network --> Simulation --> ScenarioCashFlowRule-------------------------------------------------------------------
			                ----Network --> Simulation --> ScenarioCashFlowRule --> CriterionLibrary_ScenarioCashFlowRule --> 

                            BEGIN TRY

			                ALTER TABLE CriterionLibrary_ScenarioCashFlowRule NOCHECK CONSTRAINT all

			                Print 'CriterionLibrary_ScenarioCashFlowRule ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioCashFlowRule AS l3 ON l3.SimulationId = l2.Id
			                JOIN CriterionLibrary_ScenarioCashFlowRule AS l4 ON l4.ScenarioCashFlowRuleId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioCashFlowRule WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioCashFlowRule'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------------------------------------------------------------

			                --Network --> Simulation --> ScenarioCashFlowRule --> ScenarioCashFlowDistributionRule --> 

			                BEGIN TRY

			                ALTER TABLE ScenarioCashFlowDistributionRule NOCHECK CONSTRAINT all

			                Print 'ScenarioCashFlowDistributionRule ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioCashFlowRule AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioCashFlowDistributionRule AS l4 ON l4.ScenarioCashFlowRuleId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioCashFlowDistributionRule WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioCashFlowDistributionRule'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------
			                --Network --> Simulation --> ScenarioCashFlowRule 

                            BEGIN TRY

			                ALTER TABLE ScenarioCashFlowRule NOCHECK CONSTRAINT all

			                Print 'ScenarioCashFlowRule ';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioCashFlowRule AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioCashFlowRule WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioCashFlowRule'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------End ScenarioCashFlowRule-------------------------------------------------------------------
			                ------Start  ScenarioDeficientConditionGoal-------------------------------------------------------------------
			                --Network --> Simulation --> ScenarioDeficientConditionGoal --> CriterionLibrary_ScenarioDeficientConditionGoal --> 

                            BEGIN TRY

			                Print 'CriterionLibrary_ScenarioDeficientConditionGoal';

			                ALTER TABLE CriterionLibrary_ScenarioDeficientConditionGoal NOCHECK CONSTRAINT all

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioDeficientConditionGoal AS l3 ON l3.SimulationId = l2.Id
			                JOIN CriterionLibrary_ScenarioDeficientConditionGoal AS l4 ON l4.ScenarioDeficientConditionGoalId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioDeficientConditionGoal WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioDeficientConditionGoal'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------------------------------------------------------------

			                --Network --> Simulation --> ScenarioDeficientConditionGoal 

                            BEGIN TRY

			                ALTER TABLE ScenarioDeficientConditionGoal NOCHECK CONSTRAINT all

			                Print 'ScenarioDeficientConditionGoal ';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioDeficientConditionGoal AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioDeficientConditionGoal WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioDeficientConditionGoal'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------End ScenarioDeficientConditionGoal-------------------------------------------------------------------
			                ------Start  ScenarioPerformanceCurve-------------------------------------------------------------------
			                --Network --> Simulation --> ScenarioPerformanceCurve --> CriterionLibrary_ScenarioPerformanceCurve --> 
			                --Network --> Simulation --> ScenarioPerformanceCurve --> ScenarioPerformanceCurve_Equation --> 

                            BEGIN TRY

			                Print 'CriterionLibrary_ScenarioPerformanceCurve';

			                ALTER TABLE CriterionLibrary_ScenarioPerformanceCurve NOCHECK CONSTRAINT all

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioPerformanceCurve AS l3 ON l3.SimulationId = l2.Id
			                JOIN CriterionLibrary_ScenarioPerformanceCurve AS l4 ON l4.ScenarioPerformanceCurveId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioPerformanceCurve WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioPerformanceCurve'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------------------------------------------------------------
			                --Network --> Simulation --> ScenarioPerformanceCurve --> ScenarioPerformanceCurve_Equation --> 

                            BEGIN TRY

			                ALTER TABLE ScenarioPerformanceCurve_Equation NOCHECK CONSTRAINT all

			                Print 'ScenarioPerformanceCurve_Equation ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioPerformanceCurve AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioPerformanceCurve_Equation AS l4 ON l4.ScenarioPerformanceCurveId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioPerformanceCurve_Equation WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioPerformanceCurve_Equation'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

			                --Network --> Simulation --> ScenarioPerformanceCurve

                            BEGIN TRY

			                Print 'ScenarioPerformanceCurve';

			                ALTER TABLE ScenarioPerformanceCurve NOCHECK CONSTRAINT all

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioPerformanceCurve AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioPerformanceCurve WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioPerformanceCurve'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------End ScenarioPerformanceCurve-------------------------------------------------------------------
			                ------Start ScenarioRemainingLifeLimit-------------------------------------------------------------------

			                --Network --> Simulation --> ScenarioRemainingLifeLimit --> CriterionLibrary_ScenarioRemainingLifeLimit --> 

                            BEGIN TRY

			                ALTER TABLE CriterionLibrary_ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

			                Print 'CriterionLibrary_ScenarioRemainingLifeLimit';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioRemainingLifeLimit AS l3 ON l3.SimulationId = l2.Id
			                JOIN CriterionLibrary_ScenarioRemainingLifeLimit AS l4 ON l4.ScenarioRemainingLifeLimitId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioRemainingLifeLimit WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioRemainingLifeLimit'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

			                --Network --> Simulation --> ScenarioRemainingLifeLimit 

                            BEGIN TRY

			                ALTER TABLE ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

			                Print 'ScenarioRemainingLifeLimit';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioRemainingLifeLimit AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioRemainingLifeLimit WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioRemainingLifeLimit'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------End ScenarioRemainingLifeLimit----------------------------------------------------------
			                ------Start ScenarioSelectableTreatment--------------------------------------------------------

 
                  --Network --> Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersedeRule --> CriterionLibrary_ScenarioTreatmentSupersedeRule --> 

			                BEGIN TRY

			                ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersedeRule NOCHECK CONSTRAINT all

			                Print 'CriterionLibrary_ScenarioTreatmentSupersedeRule';

			                Delete l5
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioTreatmentSupersedeRule AS l4 ON l4.TreatmentId = l3.Id
			                JOIN CriterionLibrary_ScenarioTreatmentSupersedeRule AS l5  ON l5.ScenarioTreatmentSupersedeRuleId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersedeRule WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentSupersedeRule'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

                   --Network --> Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersedeRule  

                         BEGIN TRY

			                ALTER TABLE ScenarioTreatmentSupersedeRule NOCHECK CONSTRAINT all

			                Print 'ScenarioTreatmentSupersedeRule ';

			                Delete l4
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioTreatmentSupersedeRule AS l4 ON l4.TreatmentId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioTreatmentSupersedeRule WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentSupersedeRule'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

		                --Simulation --> ScenarioSelectableTreatment --> ScenarioConditionalTreatmentConsequences   --> CriterionLibrary_ScenarioTreatmentConsequence

                            BEGIN TRY

			                Print 'CriterionLibrary_ScenarioTreatmentConsequence';

			                ALTER TABLE CriterionLibrary_ScenarioTreatmentConsequence NOCHECK CONSTRAINT all

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioConditionalTreatmentConsequences AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			                JOIN  CriterionLibrary_ScenarioTreatmentConsequence AS l5 ON l5.ScenarioConditionalTreatmentConsequenceId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioTreatmentConsequence WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentConsequence'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
		
		                ---------------------------------------------------------------------------

		                --Simulation --> ScenarioSelectableTreatment --> ScenarioConditionalTreatmentConsequences   --> ScenarioTreatmentConsequence_Equation

                            BEGIN TRY

			                --ALTER TABLE ScenarioTreatmentConsequence_Equation NOCHECK CONSTRAINT all

			                Print 'ScenarioTreatmentConsequence_Equation';

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioConditionalTreatmentConsequences AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			                JOIN  ScenarioTreatmentConsequence_Equation AS l5 ON l5.ScenarioConditionalTreatmentConsequenceId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                --ALTER TABLE ScenarioTreatmentConsequence_Equation WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentConsequence_Equation'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------
			
			                --Simulation --> ScenarioSelectableTreatment --> ScenarioConditionalTreatmentConsequences

                            BEGIN TRY

			                ALTER TABLE ScenarioConditionalTreatmentConsequences NOCHECK CONSTRAINT all

			                Print 'ScenarioConditionalTreatmentConsequences';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioConditionalTreatmentConsequences AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioConditionalTreatmentConsequences WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioConditionalTreatmentConsequences'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

	  	                --------------------------------------------------------------------------

			                --Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentCost --> ScenarioTreatmentCost_Equation

                            BEGIN TRY

			                --ALTER TABLE ScenarioTreatmentCost_Equation NOCHECK CONSTRAINT all

			                Print 'ScenarioTreatmentCost_Equation';

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioTreatmentCost AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			                JOIN ScenarioTreatmentCost_Equation AS l5 ON l5.ScenarioTreatmentCostId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                --ALTER TABLE ScenarioTreatmentCost_Equation WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentCost_Equation'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH


		                ---------------------------------------------------------------------------

		                --Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentCost --> CriterionLibrary_ScenarioTreatmentCost

                            BEGIN TRY

			                Print 'CriterionLibrary_ScenarioTreatmentCost ';

			                ALTER TABLE CriterionLibrary_ScenarioTreatmentCost NOCHECK CONSTRAINT all

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioTreatmentCost AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			                JOIN CriterionLibrary_ScenarioTreatmentCost AS l5 ON l5.ScenarioTreatmentCostId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioTreatmentCost WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentCost'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------
		                --simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentCost

                            BEGIN TRY

			                ALTER TABLE ScenarioTreatmentCost NOCHECK CONSTRAINT all

			                Print 'ScenarioTreatmentCost ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioTreatmentCost AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioTreatmentCost WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentCost'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH


		                ---------------------------------------------------------------------------

		                --Simulation --> ScenarioSelectableTreatment --> ScenarioSelectableTreatment_ScenarioBudget --> 

                            BEGIN TRY

			                ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget NOCHECK CONSTRAINT all

			                Print 'ScenarioSelectableTreatment_ScenarioBudget ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioSelectableTreatment_ScenarioBudget AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioSelectableTreatment_ScenarioBudget'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

			                --Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentPerformanceFactor --> 

                            BEGIN TRY

			                Print 'ScenarioTreatmentPerformanceFactor';

			                ALTER TABLE ScenarioTreatmentPerformanceFactor NOCHECK CONSTRAINT all

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioTreatmentPerformanceFactor AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioTreatmentPerformanceFactor WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentPerformanceFactor'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------
			                --Simulation --> ScenarioSelectableTreatment --> CriterionLibrary_ScenarioTreatment --> 

                            BEGIN TRY

			                ALTER TABLE CriterionLibrary_ScenarioTreatment NOCHECK CONSTRAINT all

			                Print 'CriterionLibrary_ScenarioTreatment ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN CriterionLibrary_ScenarioTreatment AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioTreatment WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatment'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

			                --Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentScheduling -->

                            BEGIN TRY

			                ALTER TABLE ScenarioTreatmentScheduling NOCHECK CONSTRAINT all

			                Print 'ScenarioTreatmentScheduling ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                JOIN ScenarioTreatmentScheduling AS l4 ON l4.TreatmentId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioTreatmentScheduling WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentScheduling'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

			                --Simulation --> ScenarioSelectableTreatment 15

                            BEGIN TRY

			                ALTER TABLE ScenarioSelectableTreatment NOCHECK CONSTRAINT all

			                Print 'ScenarioSelectableTreatment ';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioSelectableTreatment WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioSelectableTreatment'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---End ScenarioSelectableTreatment --------------------------------------------
			                ---------------------------------------------------------------------------
			                ---Start ScenarioTargetConditionGoals --------------------------------------------

			                --Simulation --> ScenarioTargetConditionGoals --> CriterionLibrary_ScenarioTargetConditionGoal --> 

                            BEGIN TRY

			                Print 'CriterionLibrary_ScenarioTargetConditionGoal';

			                ALTER TABLE CriterionLibrary_ScenarioTargetConditionGoal NOCHECK CONSTRAINT all

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioTargetConditionGoals AS l3 ON l3.SimulationId = l2.Id
			                JOIN CriterionLibrary_ScenarioTargetConditionGoal AS l4 ON l4.ScenarioTargetConditionGoalId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE CriterionLibrary_ScenarioTargetConditionGoal WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTargetConditionGoal'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

		                --Simulation --> ScenarioTargetConditionGoals 

                            BEGIN TRY

			                ALTER TABLE ScenarioTargetConditionGoals NOCHECK CONSTRAINT all

			                Print 'ScenarioTargetConditionGoals ';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN ScenarioTargetConditionGoals AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE ScenarioTargetConditionGoals WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in ScenarioTargetConditionGoals'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---End ScenarioTargetConditionGoals --------------------------------------------
			                ---------------------------------------------------------------------------
		                --Simulation --> Simulation_User 

                            BEGIN TRY

			                ALTER TABLE Simulation_User NOCHECK CONSTRAINT all

			                Print 'Simulation_User ';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN Simulation_User AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE Simulation_User WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Simulation_User'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

		                --Simulation --> SimulationAnalysisDetail 

                            BEGIN TRY

			                Print 'SimulationAnalysisDetail';

			                ALTER TABLE SimulationAnalysisDetail NOCHECK CONSTRAINT all

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationAnalysisDetail AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE SimulationAnalysisDetail WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in SimulationAnalysisDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

                            BEGIN TRY

			                Print 'SimulationLog';

			                ALTER TABLE SimulationLog NOCHECK CONSTRAINT all

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationLog AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE SimulationLog WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in SimulationLog'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -------End SimulationLog-----------------------------------------
			
			                ----Start SimulationOutput Path---------------------------------------

			                -----Start AssetSummaryDetail Path-----------------------------------------

			                -------SimulationOutput --> AssetSummaryDetail --> AssetSummaryDetailValueIntI----
		                --SET @CurrentDateTime = GETDATE();
		                --PRINT 'Start SimulationOutput Delete: ' + CONVERT(NVARCHAR, @CurrentDateTime, 120);

                        BEGIN TRY

                          ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
		                  SET @RowsDeleted = 1;
		                  Print 'AssetSummaryDetailValueIntId ';

		                   WHILE @RowsDeleted > 0
				                BEGIN

				                --Delete TOP (@BatchSize) l5 
				                SELECT TOP  (@BatchSize) l5.Id  INTO #tempAssetSummaryDetailValueIntIdSim	
				                FROM Network AS l1
				                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
				                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
				                JOIN AssetSummaryDetail AS l4 ON l4.SimulationOutputId = l3.Id
				                Join AssetSummaryDetailValueIntId As l5 ON l5.AssetSummaryDetailId = l4.Id
				                WHERE l1.Id IN (@NetworkId);

				                DELETE FROM AssetSummaryDetailValueIntId WHERE Id in (SELECT Id FROM #tempAssetSummaryDetailValueIntIdSim);

				                SET @RowsDeleted = @@ROWCOUNT;

				                DROP TABLE #tempAssetSummaryDetailValueIntIdSim;
				                --WAITFOR DELAY '00:00:01';
			                END	

                            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all;

                            END TRY 
			                BEGIN CATCH
			                     ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in AssetSummaryDetailValueIntId'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                -----------------------------------------------------------------

    		                -------SimulationOutput --> AssetSummaryDetail -----

                            BEGIN TRY

			                Print 'AssetSummaryDetail';

                          ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN AssetSummaryDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in AssetSummaryDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH


			                ----------------------------------------------------------------------
			                -----End AssetSummaryDetail Path--------------------------------------

			                -----Start SimulationYearDetail Path-----------------------------------------

			                --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail 

			                BEGIN TRY

			                Print 'BudgetUsageDetail2';

                            ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all

		                    Delete l7  
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			                JOIN TreatmentConsiderationDetail AS l6 ON l6.AssetDetailId = l5.Id
			                JOIN BudgetUsageDetail AS l7 ON l7.TreatmentConsiderationDetailId = l6.Id
			                WHERE l1.Id IN (@NetworkId);

		                    SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in BudgetUsageDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

		                --Print '*******BudgetUsageDetail**************'
	
	                ------------------------------------------------------------------
    		                -------SimulationOutput --> AssetSummaryDetail -----

                            BEGIN TRY

			                Print 'AssetSummaryDetail';

                          ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN AssetSummaryDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in AssetSummaryDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH


			                ----------------------------------------------------------------------
			                -----End AssetSummaryDetail Path--------------------------------------

			                -----Start SimulationYearDetail Path-----------------------------------------

			                --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail 

			                BEGIN TRY

			                Print 'BudgetUsageDetail';

                            ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all

		                    Delete l7  
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			                JOIN TreatmentConsiderationDetail AS l6 ON l6.AssetDetailId = l5.Id
			                JOIN BudgetUsageDetail AS l7 ON l7.TreatmentConsiderationDetailId = l6.Id
			                WHERE l1.Id IN (@NetworkId);

		                    SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in BudgetUsageDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

		                --Print '*******BudgetUsageDetail**************'
	
	                ------------------------------------------------------------------

				                --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail

			                BEGIN TRY

			                Print 'CashFlowConsiderationDetail';

                            ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all

		                    Delete l7  
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			                JOIN TreatmentConsiderationDetail AS l6 ON l6.AssetDetailId = l5.Id
			                JOIN CashFlowConsiderationDetail AS l7 ON l7.TreatmentConsiderationDetailId = l6.Id
			                WHERE l1.Id IN (@NetworkId);

		                    SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE CashFlowConsiderationDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in CashFlowConsiderationDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
	                ------------------------------------------------------------------
	
		                --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail 

			                BEGIN TRY

			                Print 'TreatmentConsiderationDetail';

                            ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all

		                    Delete l6  
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			                JOIN TreatmentConsiderationDetail AS l6 ON l6.AssetDetailId = l5.Id
			                WHERE l1.Id IN (@NetworkId);

		                    SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE TreatmentConsiderationDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in TreatmentConsiderationDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
	                ------------------------------------------------------------------


                --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentOptionDetail -->  -->  -->  -->  -->  --> 


			                BEGIN TRY

			                Print 'TreatmentOptionDetail';

                            ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

			                Delete l6 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			                JOIN TreatmentOptionDetail AS l6 ON l6.AssetDetailId = l5.Id
			                WHERE l1.Id IN (@NetworkId);

		                    SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentOptionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
			
                            ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in TreatmentOptionDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
		                -----------------------------------------------------------------
		                ---------------------------------------------------------------
			                --SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId -->  -->  -->  -->  -->  --> 


			                BEGIN TRY

			                Print 'AssetDetailValueIntId';

                            ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all
			                SET @RowsDeleted = 1;

			                WHILE @RowsDeleted > 0
			                BEGIN

						                --Delete TOP (@BatchSize) l6 
						                SELECT TOP  (@BatchSize) l6.Id  INTO #tempAssetDetailValueIntId2
						                FROM Network AS l1
						                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
						                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
						                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
						                JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
						                JOIN AssetDetailValueIntId AS l6 ON l6.AssetDetailId = l5.Id
						                WHERE l1.Id IN (@NetworkId);

						                DELETE FROM AssetDetailValueIntId WHERE Id in (SELECT Id FROM #tempAssetDetailValueIntId2);

						                SET @RowsDeleted = @@ROWCOUNT;

						                DROP TABLE #tempAssetDetailValueIntId2;
					
						                --Print 'Rows Affected Network --> --SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
			                END			
						
						                ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
                            END TRY 
			                BEGIN CATCH
			                ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in AssetDetailValueIntId'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
		                -----------------------------------------------------------------

		                --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail -->  -->  -->  -->  -->  --> 

			                BEGIN TRY

			                Print 'TreatmentRejectionDetail';

                            ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all
			                SET @RowsDeleted = 1;

			                WHILE @RowsDeleted > 0
			                BEGIN

						                --Delete TOP (@BatchSize) l6 
						                SELECT TOP  (@BatchSize) l6.Id  INTO #tempTreatmentRejectionDetailSim
						                FROM Network AS l1
						                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
						                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
						                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
						                JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
						                JOIN TreatmentRejectionDetail AS l6 ON l6.AssetDetailId = l5.Id
						                WHERE l1.Id IN (@NetworkId);

						                DELETE FROM TreatmentRejectionDetail WHERE Id in (SELECT Id FROM #tempTreatmentRejectionDetailSim);

						                SET @RowsDeleted = @@ROWCOUNT;

						                DROP TABLE #tempTreatmentRejectionDetailSim;
						                --WAITFOR DELAY '00:00:01';
						
						                --Print 'Rows Affected Network --> --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentRejectionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
			                END						
						
						                ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in TreatmentRejectionDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
	                ----End TreatmentRejectionDetail-------------------------------------------------------------
	                -----------------------------------------------------------------
	                --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail 


			                BEGIN TRY

			                Print 'TreatmentSchedulingCollisionDetail';

                            ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all

			                Delete l6 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			                JOIN TreatmentSchedulingCollisionDetail AS l6 ON l6.AssetDetailId = l5.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in TreatmentSchedulingCollisionDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
	                -----------------------------------------------------------------
	
			                --SimulationOutput\SimulationYearDetail\AssetDetail
			
			                BEGIN TRY

			                Print 'Simulation\SimulationOutput\SimulationYearDetail\AssetDetail';

                            ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

			                Delete l5
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected  Network ->  Simulation ->  SimulationOutput --> SimulationYearDetail-->AssetDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in AssetDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
	                -----------------------------------------------------------------


			                --SimulationOutput\SimulationYearDetail\BudgetDetail
			
			                BEGIN TRY

			                Print 'SimulationOutput\SimulationYearDetail\BudgetDetailBudgetDetail';

                            ALTER TABLE BudgetDetail NOCHECK CONSTRAINT all

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN BudgetDetail AS l5 ON l5.SimulationYearDetailId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE BudgetDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in BudgetDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
	                ------------------------------------------------------------------

			                --SimulationOutput\SimulationYearDetail\DeficientConditionGoalDetail
			
			                BEGIN TRY

                            ALTER TABLE DeficientConditionGoalDetail NOCHECK CONSTRAINT all

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN DeficientConditionGoalDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE DeficientConditionGoalDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in DeficientConditionGoalDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
	                ------------------------------------------------------------------
		                --SimulationOutput\SimulationYearDetail\TargetConditionGoalDetail
			
			                BEGIN TRY

			                Print 'TargetConditionGoalDetail';

                            ALTER TABLE TargetConditionGoalDetail NOCHECK CONSTRAINT all

			                Delete l5 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                JOIN TargetConditionGoalDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE TargetConditionGoalDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in TargetConditionGoalDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
	
	                ------------------------------------------------------------------

			                --SimulationOutput\SimulationYearDetail
			
			                BEGIN TRY

			                Print 'SimulationYearDetail ';

                            ALTER TABLE SimulationYearDetail NOCHECK CONSTRAINT all

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

                            ALTER TABLE SimulationYearDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in SimulationYearDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ------------------------------------------------------------------
			                --SimulationOutputJson Delete records where SimulationOutput is the parent

                            BEGIN TRY

			                ALTER TABLE SimulationOutputJson NOCHECK CONSTRAINT all

			                Print 'SimulationOutputJson ';

			                Delete l4 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                Join SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                JOIN SimulationOutputJson AS l4 ON l4.SimulationOutputId = l3.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE SimulationOutputJson WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in SimulationOutputJson'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------
			                --SimulationOutput

                            BEGIN TRY

                            ALTER TABLE SimulationOutput NOCHECK CONSTRAINT all

			                Print 'SimulationOutput ';

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE SimulationOutput WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in SimulationOutput'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

		                   ---------------------------------------------------------------------------
		                ----End SimulationOutput Delete----------------------------------------
		                    --SET @CurrentDateTime = GETDATE();
			                --PRINT 'End SimulationOutput Delete: ' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
 
			                --SimulationOutputJson Delete records where Simulation is the parent

                            BEGIN TRY

			                Print 'SimulationOutputJson';

			                ALTER TABLE SimulationOutputJson NOCHECK CONSTRAINT all

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationOutputJson AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected  Network ->  Simulation ->  SimulationOutputJson: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE SimulationOutputJson WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in SimulationOutputJson'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------

			                --SimulationReportDetail

                            BEGIN TRY

			                Print 'SimulationReportDetail';

			                ALTER TABLE SimulationReportDetail NOCHECK CONSTRAINT all

			                Delete l3 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                JOIN SimulationReportDetail AS l3 ON l3.SimulationId = l2.Id
			                WHERE l1.Id IN (@NetworkId);

			                ALTER TABLE SimulationReportDetail WITH CHECK CHECK CONSTRAINT all

 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in SimulationReportDetail'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

			                ---------------------------------------------------------------------------
			                --Simulation

                            BEGIN TRY

			                Print 'Simulation';

			                ALTER TABLE Simulation NOCHECK CONSTRAINT all

			                Delete l2 
			                FROM Network AS l1
			                JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected  Network --> Simulation: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE Simulation WITH CHECK CHECK CONSTRAINT all;
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Simulation'
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
		
		                 --   SET @CurrentDateTime = GETDATE();
			                --PRINT 'End Simulation Delete: ' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
			                ------End Simulation Delete-------------------------------------------------------------------
			                -----End  ----Network --> Simulation Path---------

			                -- Start Network

                            BEGIN TRY

			                Print 'Network';

                            ALTER TABLE Network NOCHECK CONSTRAINT all

			                Delete l1 
			                FROM  Network AS l1
			                WHERE l1.Id IN (@NetworkId);

			                SET @RowsDeleted = @@ROWCOUNT;
			                --Print 'Rows Affected  Network: ' +  convert(NVARCHAR(50), @RowsDeleted);

			                ALTER TABLE Network WITH CHECK CHECK CONSTRAINT all
 
                            END TRY 
			                BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

 		                         SELECT @CustomErrorMessage = 'Query Error in Network'
		                         RAISERROR (@CustomErrorMessage, 16, 1);
				                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

		                   ---------------------------------------------------------------------------
			                ------End Network Delete-------------------------------------------------------------------
                    Print 'Delete Network Committed End';
 	                RAISERROR (@RetMessage, 0, 1);
	                END TRY
	                BEGIN CATCH
  			                Set @RetMessage = 'Failed ' + @RetMessage;
			                Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			                Print 'Overall Catch in Network SP:  ' + @ErrorMessage;

			                RAISERROR  (@RetMessage, 16, 1);  
	                END CATCH;

                    END
GO

CREATE OR ALTER PROCEDURE dbo.usp_delete_simulation(@SimGuidList NVARCHAR(MAX)=NULL,@RetMessage VARCHAR(250)=NULL OUTPUT)
	          AS
	          BEGIN 
 	            DECLARE @CustomErrorMessage NVARCHAR(MAX),
	            @ErrorNumber int,
	            @ErrorSeverity int,
	            @ErrorState int,
	            @ErrorProcedure nvarchar(126),
	            @ErrorLine int,
	            @ErrorMessage nvarchar(4000);
	            Set  @RetMessage = 'Success';
	            DECLARE @CurrentDateTime DATETIME;
	            DECLARE @BatchSize INT = 100000;  -- Adjust batch size as needed
	            DECLARE @RowsDeleted INT = 1;

             ---------------------------------------------
             CREATE TABLE #SimTempGuids
                (
                   -- Guid UNIQUEIDENTIFIER
		            Guid NVARCHAR(36)
                );
	
	            IF @SimGuidList IS NULL OR LEN(@SimGuidList) = 0
	            BEGIN
		              PRINT 'String is NULL or empty';
		              Set  @SimGuidList = '00000000-0000-0000-0000-000000000000';
	            END

                INSERT INTO #SimTempGuids (Guid)
	            SELECT LEFT(LTRIM(RTRIM(value)), 36)
                FROM STRING_SPLIT(@SimGuidList, ',');

	            --Select *, '' as 'aaa' from #SimTempGuids;

	            UPDATE #SimTempGuids
	            SET Guid = '00000000-0000-0000-0000-000000000000'
	            WHERE TRY_CAST(Guid AS UNIQUEIDENTIFIER) IS NULL OR Guid = '';
	
	            --Select *, '' as 'bbb' from #SimTempGuids;

		            Begin Transaction
	            BEGIN TRY

            -----------------------------------------------------------------------------


                        BEGIN TRY
 
                        ALTER TABLE Benefit NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN AnalysisMethod AS l2 ON l2.SimulationId = l1.Id
			            JOIN Benefit AS l3 ON l3.AnalysisMethodId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE Benefit WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in Benefit'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            -----------------------------------------------------------------------

                        BEGIN TRY


                        ALTER TABLE CriterionLibrary_AnalysisMethod NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN AnalysisMethod AS l2 ON l2.SimulationId = l1.Id
			            JOIN CriterionLibrary_AnalysisMethod AS l3 ON l3.AnalysisMethodId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE CriterionLibrary_AnalysisMethod WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_AnalysisMethod'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
			            ------------------------------------------------------------------

                        BEGIN TRY

                        ALTER TABLE AnalysisMethod NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN AnalysisMethod AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE AnalysisMethod WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in AnalysisMethod'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
 
 			            -----------------------------------------------------------------------

                     BEGIN TRY

                        ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN CommittedProject AS l2 ON l2.SimulationId = l1.Id
			            JOIN CommittedProjectConsequence AS l3 ON l3.CommittedProjectId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
 	
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CommittedProjectConsequence'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            -------------------------------------

			             BEGIN TRY

                        ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN CommittedProject AS l2 ON l2.SimulationId = l1.Id
			            JOIN CommittedProjectLocation AS l3 ON l3.CommittedProjectId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
 	
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CommittedProjectLocation'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            --------------------------------------

                        BEGIN TRY

			            ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN CommittedProject AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CommittedProject WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CommittedProject'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH 

			            -------------------------------------

                        BEGIN TRY

                        ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			            JOIN CommittedProject AS l3 ON l3.ScenarioBudgetId = l2.Id
			            JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
 	
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CommittedProjectConsequence'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            -------------------------------------

			             BEGIN TRY

                        ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			            JOIN CommittedProject AS l3 ON l3.ScenarioBudgetId = l2.Id
			            JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
 	
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CommittedProjectLocation'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            -----------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioBudgetAmount NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioBudgetAmount AS l3 ON l3.ScenarioBudgetId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioBudgetAmount WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioBudgetAmount'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ----------------------------------------------

                        BEGIN TRY

			            ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			            JOIN BudgetPercentagePair AS l3 ON l3.ScenarioBudgetId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE BudgetPercentagePair WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in BudgetPercentagePair'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            -------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioBudget NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioBudget WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioBudget'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------

                        BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioBudgetPriority NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioBudgetPriority AS l2 ON l2.SimulationId = l1.Id
			            JOIN CriterionLibrary_ScenarioBudgetPriority AS l3 ON l3.ScenarioBudgetPriorityId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioBudgetPriority WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioBudgetPriority'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioBudgetPriority AS l2 ON l2.SimulationId = l1.Id
			            JOIN BudgetPercentagePair AS l3 ON l3.ScenarioBudgetPriorityId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE BudgetPercentagePair WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in BudgetPercentagePair'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH


			            ------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioBudgetPriority NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ScenarioBudgetPriority AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioBudgetPriority WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioBudgetPriority'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ----------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioCalculatedAttributePair_Criteria NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN ScenarioCalculatedAttribute AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioCalculatedAttributePair AS l3 ON l3.ScenarioCalculatedAttributeId = l2.Id
			            JOIN ScenarioCalculatedAttributePair_Criteria AS l4 ON l4.ScenarioCalculatedAttributePairId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioCalculatedAttributePair_Criteria WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttributePair_Criteria'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioCalculatedAttributePair_Equation NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN ScenarioCalculatedAttribute AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioCalculatedAttributePair AS l3 ON l3.ScenarioCalculatedAttributeId = l2.Id
			            JOIN ScenarioCalculatedAttributePair_Equation AS l4 ON l4.ScenarioCalculatedAttributePairId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioCalculatedAttributePair_Equation WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttributePair_Equation'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ------------------------------------------------------------

			            BEGIN TRY

			            ALTER TABLE ScenarioCalculatedAttributePair NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioCalculatedAttribute AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioCalculatedAttributePair AS l3 ON l3.ScenarioCalculatedAttributeId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioCalculatedAttributePair WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttributePair'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioCalculatedAttribute NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ScenarioCalculatedAttribute AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioCalculatedAttribute WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttribute'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            --------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioCashFlowRule NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioCashFlowRule AS l2 ON l2.SimulationId = l1.Id
			            JOIN CriterionLibrary_ScenarioCashFlowRule AS l3 ON l3.ScenarioCashFlowRuleId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioCashFlowRule WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioCashFlowRule'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ------------------------------------------------------------

			            BEGIN TRY

			            ALTER TABLE ScenarioCashFlowDistributionRule NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioCashFlowRule AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioCashFlowDistributionRule AS l3 ON l3.ScenarioCashFlowRuleId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioCashFlowDistributionRule WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioCashFlowDistributionRule'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioCashFlowRule NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ScenarioCashFlowRule AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioCashFlowRule WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioCashFlowRule'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            --------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioDeficientConditionGoal NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioDeficientConditionGoal AS l2 ON l2.SimulationId = l1.Id
			            JOIN CriterionLibrary_ScenarioDeficientConditionGoal AS l3 ON l3.ScenarioDeficientConditionGoalId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioDeficientConditionGoal WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioDeficientConditionGoal'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioDeficientConditionGoal NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ScenarioDeficientConditionGoal AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioDeficientConditionGoal WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioDeficientConditionGoal'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioPerformanceCurve NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioPerformanceCurve AS l2 ON l2.SimulationId = l1.Id
			            JOIN CriterionLibrary_ScenarioPerformanceCurve AS l3 ON l3.ScenarioPerformanceCurveId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioPerformanceCurve WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioPerformanceCurve'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioPerformanceCurve_Equation NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioPerformanceCurve AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioPerformanceCurve_Equation AS l3 ON l3.ScenarioPerformanceCurveId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioPerformanceCurve_Equation WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioPerformanceCurve_Equation'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioPerformanceCurve NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ScenarioPerformanceCurve AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioPerformanceCurve WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioPerformanceCurve'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioRemainingLifeLimit AS l2 ON l2.SimulationId = l1.Id
			            JOIN CriterionLibrary_ScenarioRemainingLifeLimit AS l3 ON l3.ScenarioRemainingLifeLimitId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioRemainingLifeLimit WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioRemainingLifeLimit'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ScenarioRemainingLifeLimit AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioRemainingLifeLimit WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioRemainingLifeLimit'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            --------------------------------------------------------------------

			            BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersedeRule NOCHECK CONSTRAINT all

			            Delete l4
			            FROM Simulation AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioTreatmentSupersedeRule AS l3 ON l3.TreatmentId = l2.Id
			            JOIN CriterionLibrary_ScenarioTreatmentSupersedeRule AS l4  ON l4.ScenarioTreatmentSupersedeRuleId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersedeRule WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentSupersedeRule'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

               --Network --> Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersedeRule  

                     BEGIN TRY

			            ALTER TABLE ScenarioTreatmentSupersedeRule NOCHECK CONSTRAINT all

			            Delete l3
			            FROM Simulation AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioTreatmentSupersedeRule AS l3 ON l3.TreatmentId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioTreatmentSupersedeRule WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentSupersedeRule'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

		             BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioTreatmentConsequence NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioConditionalTreatmentConsequences AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			            JOIN CriterionLibrary_ScenarioTreatmentConsequence AS l4 ON l4.ScenarioConditionalTreatmentConsequenceId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioTreatmentConsequence WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentConsequence'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
		
		            ---------------------------------------------------------------------------

                        BEGIN TRY

			            --ALTER TABLE ScenarioTreatmentConsequence_Equation NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioConditionalTreatmentConsequences AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			            JOIN  ScenarioTreatmentConsequence_Equation AS l4 ON l4.ScenarioConditionalTreatmentConsequenceId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            --ALTER TABLE ScenarioTreatmentConsequence_Equation WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentConsequence_Equation'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioConditionalTreatmentConsequences NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioConditionalTreatmentConsequences AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioConditionalTreatmentConsequences WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioConditionalTreatmentConsequences'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

	  	            --------------------------------------------------------------------------

                        BEGIN TRY

			            --ALTER TABLE ScenarioTreatmentCost_Equation NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioTreatmentCost AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			            JOIN ScenarioTreatmentCost_Equation AS l4 ON l4.ScenarioTreatmentCostId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            --ALTER TABLE ScenarioTreatmentCost_Equation WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentCost_Equation'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH


		            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioTreatmentCost NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioTreatmentCost AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			            JOIN  CriterionLibrary_ScenarioTreatmentCost AS l4 ON l4.ScenarioTreatmentCostId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioTreatmentCost WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentCost'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioTreatmentCost NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioTreatmentCost AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioTreatmentCost WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentCost'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH


		            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioSelectableTreatment_ScenarioBudget AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioSelectableTreatment_ScenarioBudget'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioTreatment NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN CriterionLibrary_ScenarioTreatment AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioTreatment WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatment'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioTreatmentScheduling NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            JOIN ScenarioTreatmentScheduling AS l3 ON l3.TreatmentId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioTreatmentScheduling WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentScheduling'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioSelectableTreatment NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioSelectableTreatment WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioSelectableTreatment'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH


			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE CriterionLibrary_ScenarioTargetConditionGoal NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN ScenarioTargetConditionGoals AS l2 ON l2.SimulationId = l1.Id
			            JOIN CriterionLibrary_ScenarioTargetConditionGoal AS l3 ON l3.ScenarioTargetConditionGoalId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE CriterionLibrary_ScenarioTargetConditionGoal WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTargetConditionGoal'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ScenarioTargetConditionGoals NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ScenarioTargetConditionGoals AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ScenarioTargetConditionGoals WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ScenarioTargetConditionGoals'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE InvestmentPlan NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN InvestmentPlan AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE InvestmentPlan WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in InvestmentPlan'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE ReportIndex NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN ReportIndex AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE ReportIndex WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in ReportIndex'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE Simulation_User NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN Simulation_User AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE Simulation_User WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in Simulation_User'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE SimulationAnalysisDetail NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM  Simulation  AS l1
			            JOIN SimulationAnalysisDetail AS l2 ON l2.SimulationId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE SimulationAnalysisDetail WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in SimulationAnalysisDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

                      ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
		              SET @RowsDeleted = 1;
		              --Print 'AssetSummaryDetailValueIntId ';

		               WHILE @RowsDeleted > 0
				            BEGIN
					            BEGIN TRY
						            Begin Transaction

						            Delete TOP (@BatchSize) l4
						            FROM  Simulation  AS l1
						            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
						            JOIN AssetSummaryDetail AS l3 ON l3.SimulationOutputId = l2.Id
						            Join AssetSummaryDetailValueIntId As l4 ON l4.AssetSummaryDetailId = l3.Id
						            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

						            SET @RowsDeleted = @@ROWCOUNT;
						            COMMIT TRANSACTION
						            Print 'Rows Affected AssetSummaryDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					            END TRY
					            BEGIN CATCH
							            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							            Set @RetMessage = 'Failed';
							            Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							            Print 'Rolled Back AssetSummaryDetailValueIntId Delete Transaction in Simulation SP:  ' + @ErrorMessage;
							            ROLLBACK TRANSACTION;
							            RAISERROR  (@RetMessage, 16, 1); 
							            Return -1;
					            END CATCH;
				            END

                        ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in AssetSummaryDetailValueIntId'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            -----------------------------------------------------------------------

                        BEGIN TRY

                      ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN AssetSummaryDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in AssetSummaryDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ----------------------------------------------------------------------

			            BEGIN TRY

                        ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all

			            Delete l6 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			            JOIN TreatmentConsiderationDetail AS l5 ON l5.AssetDetailId = l4.Id
			            JOIN BudgetUsageDetail AS l6 ON l6.TreatmentConsiderationDetailId = l5.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in BudgetUsageDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

	            ------------------------------------------------------------------

			            BEGIN TRY

                        ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all
			
			            Delete l6 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			            JOIN TreatmentConsiderationDetail AS l5 ON l5.AssetDetailId = l4.Id
			            JOIN CashFlowConsiderationDetail AS l6 ON l6.TreatmentConsiderationDetailId = l5.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE CashFlowConsiderationDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CashFlowConsiderationDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------
	
			            BEGIN TRY

                        ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all

			            Delete l5 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			            JOIN TreatmentConsiderationDetail AS l5 ON l5.AssetDetailId = l4.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE TreatmentConsiderationDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TreatmentConsiderationDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------

			            BEGIN TRY

                        ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

			            Delete l5 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			            JOIN TreatmentOptionDetail AS l5 ON l5.AssetDetailId = l4.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TreatmentOptionDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
		            -----------------------------------------------------------------

			            BEGIN TRY

                        ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all

			            SET @RowsDeleted = 1;
		  	            --Print 'AssetDetailValueIntId ';

		               WHILE @RowsDeleted > 0
				            BEGIN
					            BEGIN TRY
						            Begin Transaction

						            Delete TOP (@BatchSize) l5 
						            FROM  Simulation  AS l1
						            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
						            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
						            JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
						            JOIN AssetDetailValueIntId AS l5 ON l5.AssetDetailId = l4.Id
						            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

						            SET @RowsDeleted = @@ROWCOUNT;
						            COMMIT TRANSACTION
						            Print 'Rows Affected AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					            END TRY
					            BEGIN CATCH
							            ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							            Set @RetMessage = 'Failed';
							            Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							            Print 'Rolled Back AssetDetailValueIntId Delete Transaction in Simulation SP  ' + @ErrorMessage;
							            ROLLBACK TRANSACTION;
							            RAISERROR  (@RetMessage, 16, 1); 
							            Return -1;
					            END CATCH;
				            END


                        ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in AssetDetailValueIntId'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
		            -----------------------------------------------------------------

			            BEGIN TRY

                        ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all

		  	            --Print 'TreatmentRejectionDetail ';

		               WHILE @RowsDeleted > 0
				            BEGIN
					            BEGIN TRY
						            Begin Transaction

						            Delete TOP (@BatchSize) l5 
						            FROM  Simulation  AS l1
						            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
						            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
						            JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
						            JOIN TreatmentRejectionDetail AS l5 ON l5.AssetDetailId = l4.Id
						            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

 						            SET @RowsDeleted = @@ROWCOUNT;
						            COMMIT TRANSACTION
						            Print 'Rows Affected TreatmentRejectionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
					            END TRY
					            BEGIN CATCH
							            ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all
  							            Set @RetMessage = 'Failed';
							            Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							            Print 'Rolled Back TreatmentRejectionDetail Delete Transaction in Simulation SP  ' + @ErrorMessage;
							            ROLLBACK TRANSACTION;
							            RAISERROR  (@RetMessage, 16, 1); 
							            Return -1;
					            END CATCH;
				            END		


                        ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TreatmentRejectionDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            -----------------------------------------------------------------

			            BEGIN TRY

                        ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all

			            Delete l5 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			            JOIN TreatmentSchedulingCollisionDetail AS l5 ON l5.AssetDetailId = l4.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TreatmentSchedulingCollisionDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            -----------------------------------------------------------------
	
			            BEGIN TRY

                        ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in AssetDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            -----------------------------------------------------------------
			
			            BEGIN TRY

                        ALTER TABLE BudgetDetail NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            JOIN BudgetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE BudgetDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in BudgetDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------
			
			            BEGIN TRY

                        ALTER TABLE DeficientConditionGoalDetail NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            JOIN DeficientConditionGoalDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE DeficientConditionGoalDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in DeficientConditionGoalDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------
			
			            BEGIN TRY

                        ALTER TABLE TargetConditionGoalDetail NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            JOIN TargetConditionGoalDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE TargetConditionGoalDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TargetConditionGoalDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------
			
			            BEGIN TRY

                        ALTER TABLE SimulationYearDetail NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM  Simulation  AS l1
			            JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			            JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

                        ALTER TABLE SimulationYearDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in SimulationYearDetail'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ------------------------------------------------------------------
			
			            --SimulationOutputJson Delete records where Simulation is the parent

                        BEGIN TRY

			            ALTER TABLE SimulationOutputJson NOCHECK CONSTRAINT all

			            BEGIN TRY
				            Begin Transaction

					            Delete l2 
					            FROM  Simulation AS l1
					            JOIN SimulationOutputJson AS l2 ON l2.SimulationId = l1.Id
					            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

 					            SET @RowsDeleted = @@ROWCOUNT;
					            COMMIT TRANSACTION
					            Print 'Rows Affected SimulationOutputJson: ' +  convert(NVARCHAR(50), @RowsDeleted);
					            END TRY
					            BEGIN CATCH
							            ALTER TABLE SimulationOutputJson WITH CHECK CHECK CONSTRAINT all
  							            Set @RetMessage = 'Failed';
							            Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							            Print 'Rolled Back SimulationOutputJson Delete Transaction in Simulation SP  ' + @ErrorMessage;
							            ROLLBACK TRANSACTION;
							            RAISERROR  (@RetMessage, 16, 1); 
							            Return -1;
					            END CATCH;

			            ALTER TABLE SimulationOutputJson WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in SimulationOutputJson'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ---------------------------------------------------------------------------

                        BEGIN TRY

			            ALTER TABLE Simulation NOCHECK CONSTRAINT all

			            Delete l1 
			            FROM  Simulation  AS l1
			            WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

			            ALTER TABLE Simulation WITH CHECK CHECK CONSTRAINT all

 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in Simulation'
                             RAISERROR  (@CustomErrorMessage, 16, 1);  
                             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
		
		            -------------------------------------------------------------
	
	            DROP TABLE #SimTempGuids;
                COMMIT TRANSACTION
                Print 'Simulation  Delete Transaction Committed';
   	            --RAISERROR (@RetMessage, 0, 1);
	            END TRY
	            BEGIN CATCH
  			            Set @RetMessage = 'Failed';
			            Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			            Print 'Rolled Back Simulation Delete Transaction in Simulation SP:  ' + @ErrorMessage;
			            ROLLBACK TRANSACTION;
			            RAISERROR  (@RetMessage, 16, 1);  
	            END CATCH;

              END
GO

CREATE OR ALTER PROCEDURE dbo.usp_delete_simulationoutput(@SimOutputGuidList NVARCHAR(MAX)=NULL,@RetMessage VARCHAR(250) OUTPUT)
	          AS
	          BEGIN 
	            DECLARE @CustomErrorMessage NVARCHAR(MAX),
	            @ErrorNumber int,
	            @ErrorSeverity int,
	            @ErrorState int,
	            @ErrorProcedure nvarchar(126),
	            @ErrorLine int,
	            @ErrorMessage nvarchar(4000);
	            Set  @RetMessage = 'Success';
	            DECLARE @CurrentDateTime DATETIME;
	            DECLARE @BatchSize INT = 200000;  -- Adjust batch size as needed
	            DECLARE @RowsDeleted INT = 1;

                CREATE TABLE #SimOutputTempGuids
                (
                    --Guid UNIQUEIDENTIFIER
		             Guid NVARCHAR(36)
                );


	            IF @SimOutputGuidList IS NULL OR LEN(@SimOutputGuidList) = 0
	            BEGIN
		              PRINT 'String is NULL or empty';
		              Set  @SimOutputGuidList = '00000000-0000-0000-0000-000000000000';
	            END

                INSERT INTO #SimOutputTempGuids (Guid)
	            SELECT LEFT(LTRIM(RTRIM(value)), 36)
                FROM STRING_SPLIT(@SimOutputGuidList, ',');

	            UPDATE #SimOutputTempGuids
	            SET Guid = '00000000-0000-0000-0000-000000000000'
	            WHERE TRY_CAST(Guid AS UNIQUEIDENTIFIER) IS NULL OR Guid = '';
	
	            Begin Transaction
	            BEGIN TRY

			            -----Start AssetSummaryDetail Path-----------------------------------------

			            -------SimulationOutput --> AssetSummaryDetail --> AssetSummaryDetailValueIntI----

                        BEGIN TRY

                      ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
		  
		              SET @RowsDeleted = 1;
		              --Print 'AssetSummaryDetailValueIntId ';

		               WHILE @RowsDeleted > 0
				            BEGIN
					            BEGIN TRY
						            Begin Transaction

							            Delete TOP (@BatchSize) l3
							            FROM SimulationOutput AS l1
							            JOIN AssetSummaryDetail AS l2 ON l2.SimulationOutputId = l1.Id
							            Join AssetSummaryDetailValueIntId As l3 ON l3.AssetSummaryDetailId = l2.Id
							            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

							            SET @RowsDeleted = @@ROWCOUNT;
						            COMMIT TRANSACTION
						            Print 'Rows Affected AssetSummaryDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					            END TRY
					            BEGIN CATCH
							            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							            Set @RetMessage = 'Failed';
							            Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							            Print 'Rolled Back AssetSummaryDetailValueIntId Delete Transaction in SimulationOutput SP:  ' + @ErrorMessage;
							            ROLLBACK TRANSACTION;
							            RAISERROR  (@RetMessage, 16, 1); 
							            Return -1;
					            END CATCH;
				            END

                        ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in AssetSummaryDetailValueIntId'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            -----------------------------------------------------------------------

    		            -------SimulationOutput --> AssetSummaryDetail -----

                        BEGIN TRY

                      ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM SimulationOutput AS l1
			            JOIN AssetSummaryDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in AssetSummaryDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH



			            ----------------------------------------------------------------------
			            -----End AssetSummaryDetail Path--------------------------------------

			            -----Start SimulationYearDetail Path-----------------------------------------

			            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail 

			            BEGIN TRY

                        ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all

			            Delete l5 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			            JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			            JOIN BudgetUsageDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in BudgetUsageDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------

			            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail

			            BEGIN TRY

                        ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all

			            Delete l5 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			            JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			            JOIN CashFlowConsiderationDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE CashFlowConsiderationDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in CashFlowConsiderationDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------
	
		            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail 

			            BEGIN TRY

                        ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			            JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE TreatmentConsiderationDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TreatmentConsiderationDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------

            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentOptionDetail -->  -->  -->  -->  -->  --> 

			            BEGIN TRY

                        ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			            JOIN TreatmentOptionDetail AS l4 ON l4.AssetDetailId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TreatmentOptionDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            -----------------------------------------------------------------

	            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId -->  -->  -->  -->  -->  --> 
	
			            BEGIN TRY

                        ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all

			            SET @RowsDeleted = 1;
		  	            --Print 'AssetDetailValueIntId ';

		               WHILE @RowsDeleted > 0
				            BEGIN
					            BEGIN TRY
						            Begin Transaction
						            Delete TOP (@BatchSize) l4 
						            FROM SimulationOutput AS l1
						            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
						            JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
						            JOIN AssetDetailValueIntId AS l4 ON l4.AssetDetailId = l3.Id
						            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

						            SET @RowsDeleted = @@ROWCOUNT;
						            COMMIT TRANSACTION
						            Print 'Rows Affected AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					            END TRY
					            BEGIN CATCH
							            ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							            Set @RetMessage = 'Failed';
							            Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							            Print 'Rolled Back AssetDetailValueIntId Delete Transaction in SimulationOutput SP  ' + @ErrorMessage;
							            ROLLBACK TRANSACTION;
							            RAISERROR  (@RetMessage, 16, 1); 
							            Return -1;
					            END CATCH;
				            END

                        ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in AssetDetailValueIntId'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
		            -----------------------------------------------------------------

		            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail -->  -->  -->  -->  -->  --> 

			            BEGIN TRY

                        ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all
		  
		               SET @RowsDeleted = 1;
		              --Print 'TreatmentRejectionDetail ';

		               WHILE @RowsDeleted > 0
				            BEGIN
					            BEGIN TRY
						            Begin Transaction

						            Delete TOP (@BatchSize) l4
						            FROM SimulationOutput AS l1
						            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
						            JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
						            JOIN TreatmentRejectionDetail AS l4 ON l4.AssetDetailId = l3.Id
						            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

 						            SET @RowsDeleted = @@ROWCOUNT;
						            COMMIT TRANSACTION
						            Print 'Rows Affected TreatmentRejectionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
					            END TRY
					            BEGIN CATCH
							            ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all
  							            Set @RetMessage = 'Failed';
							            Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							            Print 'Rolled Back TreatmentRejectionDetail Delete Transaction in SimulationOutput SP  ' + @ErrorMessage;
							            ROLLBACK TRANSACTION;
							            RAISERROR  (@RetMessage, 16, 1); 
							            Return -1;
					            END CATCH;
				            END		
			
			            ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TreatmentRejectionDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            -----------------------------------------------------------------

	            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail 


			            BEGIN TRY

                        ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all

			            Delete l4 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			            JOIN TreatmentSchedulingCollisionDetail AS l4 ON l4.AssetDetailId = l3.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TreatmentSchedulingCollisionDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            -----------------------------------------------------------------


			            --SimulationOutput\SimulationYearDetail\AssetDetail
			
			            BEGIN TRY

                        ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in AssetDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            -----------------------------------------------------------------


			            --SimulationOutput\SimulationYearDetail\BudgetDetail
			
			            BEGIN TRY

                        ALTER TABLE BudgetDetail NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            JOIN BudgetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE BudgetDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in BudgetDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------


				            --SimulationOutput\SimulationYearDetail\DeficientConditionGoalDetail
			
			            BEGIN TRY

                        ALTER TABLE DeficientConditionGoalDetail NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            JOIN DeficientConditionGoalDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE DeficientConditionGoalDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in DeficientConditionGoalDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------

	
				            --SimulationOutput\SimulationYearDetail\TargetConditionGoalDetail
			
			            BEGIN TRY

                        ALTER TABLE TargetConditionGoalDetail NOCHECK CONSTRAINT all

			            Delete l3 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            JOIN TargetConditionGoalDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE TargetConditionGoalDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in TargetConditionGoalDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH
	
	            ------------------------------------------------------------------

			            --SimulationOutput\SimulationYearDetail
			
			            BEGIN TRY

                        ALTER TABLE SimulationYearDetail NOCHECK CONSTRAINT all

			            Delete l2 
			            FROM SimulationOutput AS l1
			            JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

                        ALTER TABLE SimulationYearDetail WITH CHECK CHECK CONSTRAINT all

                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in SimulationYearDetail'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

			            ------------------------------------------------------------------

  			            --SimulationOutput

                        BEGIN TRY

                        ALTER TABLE SimulationOutput NOCHECK CONSTRAINT all

			            Delete l1 
			            FROM  SimulationOutput AS l1
			            WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

			            ALTER TABLE SimulationOutput WITH CHECK CHECK CONSTRAINT all
 
                        END TRY 
			            BEGIN CATCH
                             SELECT ERROR_NUMBER() AS ErrorNumber
                                   ,ERROR_SEVERITY() AS ErrorSeverity
                                   ,ERROR_STATE() AS ErrorState
                                   ,ERROR_PROCEDURE() AS ErrorProcedure
                                   ,ERROR_LINE() AS ErrorLine
                                   ,ERROR_MESSAGE() AS ErrorMessage;

 		                     SELECT @CustomErrorMessage = 'Query Error in SimulationOutput'
		                     RAISERROR (@CustomErrorMessage, 16, 1);
				             Set @RetMessage = @CustomErrorMessage;

                        END CATCH

		               ---------------------------------------------------------------------------
			            ------End SimulationOutput Delete-------------------------------------------------------------------

	            DROP TABLE #SimOutputTempGuids;
                COMMIT TRANSACTION
                Print 'Delete Transaction Committed';
   	            RAISERROR (@RetMessage, 0, 1);
	            END TRY
	            BEGIN CATCH
  			            Set @RetMessage = 'Failed ' + @RetMessage;
			            Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			            Print 'Rolled Back Entire Delete Transaction in SimulationOutput SP:  ' + @ErrorMessage;
			            ROLLBACK TRANSACTION;
			            RAISERROR  (@RetMessage, 16, 1);  
	            END CATCH;

              END
GO

CREATE OR ALTER PROCEDURE dbo.usp_delete_aggregations(@NetworkId AS uniqueidentifier=NULL,@RetMessage VARCHAR(250) OUTPUT)
	      	          AS
	      	          BEGIN 

	                    DECLARE @CustomErrorMessage NVARCHAR(MAX),
	                    @ErrorNumber int,
	                    @ErrorSeverity int,
	                    @ErrorState int,
	                    @ErrorProcedure nvarchar(126),
	                    @ErrorLine int,
	                    @ErrorMessage nvarchar(4000);
	                    Set  @RetMessage = 'Success';
	                    DECLARE @CurrentDateTime DATETIME;
	                    DECLARE @BatchSize INT = 100000;
	                    DECLARE @RowsDeleted INT = 1;

	                    BEGIN TRY
                    -----------------------------------------------------------------------

                    --AggregatedResult
                    --AttributeDatum
                    --AttributeDatumLocation


	                    -----Start AggregatedResult Path-----------------------------------------

			                    --MaintainableAsset --> AggregatedResult

	                    BEGIN TRY

                              ALTER TABLE AggregatedResult NOCHECK CONSTRAINT all
		                      SET @RowsDeleted = 1;
		                      --Print 'AggregatedResult ';

		                       WHILE @RowsDeleted > 0
				                    BEGIN
					                    BEGIN TRY
						                    Begin Transaction

						                    --Delete TOP (@BatchSize) l3
						                    SELECT TOP  (@BatchSize) l3.Id  INTO #tempAggregatedResult
						                    FROM Network AS l1
						                    Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						                    JOIN AggregatedResult AS l3 ON l3.MaintainableAssetId = l2.Id
						                    WHERE l1.Id IN (@NetworkId);
						
						                    --DELETE ar
						                    --FROM AggregatedResult As ar
						                    --JOIN #tempAggregatedResult T ON T.Id = ar.Id;

						                    DELETE FROM AggregatedResult WHERE Id in (SELECT Id FROM #tempAggregatedResult);

						                    SET @RowsDeleted = @@ROWCOUNT;

						                    DROP TABLE #tempAggregatedResult;
						                    --WAITFOR DELAY '00:00:01';

						                    COMMIT TRANSACTION
						
						                    Print 'Rows Affected Network --> MaintainableAsset-->AggregatedResult: ' +  convert(NVARCHAR(50), @RowsDeleted);
					                    END TRY
					                    BEGIN CATCH
							                    ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all
  							                    Set @RetMessage = 'Failed';
							                    Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							                    Print 'Rolled Back AggregatedResult Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
							                    ROLLBACK TRANSACTION;
							                    RAISERROR  (@RetMessage, 16, 1); 
							                    Return -1;
					                    END CATCH;
				                    END

                                ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all

                                END TRY 
			                    BEGIN CATCH
                                     SELECT ERROR_NUMBER() AS ErrorNumber
                                           ,ERROR_SEVERITY() AS ErrorSeverity
                                           ,ERROR_STATE() AS ErrorState
                                           ,ERROR_PROCEDURE() AS ErrorProcedure
                                           ,ERROR_LINE() AS ErrorLine
                                           ,ERROR_MESSAGE() AS ErrorMessage;

 		                             SELECT @CustomErrorMessage = 'Query Error in  Network --> MaintainableAsset-->AggregatedResult'
		                             RAISERROR (@CustomErrorMessage, 16, 1);
				                     Set @RetMessage = @CustomErrorMessage;

                                END CATCH

			                    ------End -----------------------------------------------------------------


			                    -------Start AttributeDatum Path-

			                    --MaintainableAsset --> AttributeDatum --> AttributeDatumLocation -->  -->  --> 

                                BEGIN TRY

                              ALTER TABLE AttributeDatumLocation NOCHECK CONSTRAINT all
		                      SET @RowsDeleted = 1;
		  	                    --Print 'AttributeDatumLocation ';

		                      WHILE @RowsDeleted > 0
				                    BEGIN
					                    BEGIN TRY
						                    Begin Transaction

							                    SELECT TOP (@BatchSize) l4.Id  INTO #tempAttributeDatumLocation
							                    FROM Network AS l1
							                    Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
							                    JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
							                    Join AttributeDatumLocation As l4 ON l4.AttributeDatumId = l3.Id
							                    WHERE l1.Id IN (@NetworkId);

						                    --DELETE adl
						                    --FROM AttributeDatumLocation As adl
						                    --JOIN #tempAttributeDatumLocation T ON T.Id = adl.Id;

						                    DELETE FROM AttributeDatumLocation WHERE Id in (SELECT Id FROM #tempAttributeDatumLocation);

						                    SET @RowsDeleted = @@ROWCOUNT;

						                    DROP TABLE #tempAttributeDatumLocation;
						                    --WAITFOR DELAY '00:00:01';

						                    COMMIT TRANSACTION
						
						                    Print 'Rows Affected Network --> MaintainableAsset-->AttributeDatumLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);
					                    END TRY
					                    BEGIN CATCH
							                    ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all
  							                    Set @RetMessage = 'Failed';
							                    Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							                    Print 'Rolled Back AttributeDatumLocation Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
							                    ROLLBACK TRANSACTION;
							                    RAISERROR  (@RetMessage, 16, 1); 
							                    Return -1;
					                    END CATCH;
				                    END

                                ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all

                                END TRY 
			                    BEGIN CATCH
                                     SELECT ERROR_NUMBER() AS ErrorNumber
                                           ,ERROR_SEVERITY() AS ErrorSeverity
                                           ,ERROR_STATE() AS ErrorState
                                           ,ERROR_PROCEDURE() AS ErrorProcedure
                                           ,ERROR_LINE() AS ErrorLine
                                           ,ERROR_MESSAGE() AS ErrorMessage;

 		                             SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocation'
		                             RAISERROR (@CustomErrorMessage, 16, 1);
				                     Set @RetMessage = @CustomErrorMessage;

                                END CATCH

			                    -----------------------------------------------------------------------

    		                    --------MaintainableAsset --> AttributeDatum--

                                BEGIN TRY

			                    ALTER TABLE AttributeDatum NOCHECK CONSTRAINT all
			                    SET @RowsDeleted = 1;
			                    Print 'AttributeDatum ';

			                    WHILE @RowsDeleted > 0
				                    BEGIN
					                    BEGIN TRY
					                    Begin Transaction

						                    Select  TOP (@BatchSize) l3.Id  INTO #tempAttributeDatum
						                    FROM Network AS l1
						                    Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						                    JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
						                    WHERE l1.Id IN (@NetworkId);

						                    --DELETE ad
						                    --FROM AttributeDatum As ad
						                    --JOIN #tempAttributeDatum T ON T.Id = ad.Id;

						                    DELETE FROM AttributeDatum WHERE Id in (SELECT Id FROM #tempAttributeDatum);

						                    SET @RowsDeleted = @@ROWCOUNT;

						                    DROP TABLE #tempAttributeDatum;
						                    --WAITFOR DELAY '00:00:01';

						                    COMMIT TRANSACTION
						
						                    Print 'Rows Affected Network --> MaintainableAsset-->AttributeDatum: ' +  convert(NVARCHAR(50), @RowsDeleted);
					                    END TRY
					                    BEGIN CATCH
						                    ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all
						                    Set @RetMessage = 'Failed';
						                    Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
						                    Print 'Rolled Back AttributeDatum Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
						                    ROLLBACK TRANSACTION;
						                    RAISERROR  (@RetMessage, 16, 1); 
						                    Return -1;
					                    END CATCH;
				                    END

                                ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all

                                END TRY 
			                    BEGIN CATCH
                                     SELECT ERROR_NUMBER() AS ErrorNumber
                                           ,ERROR_SEVERITY() AS ErrorSeverity
                                           ,ERROR_STATE() AS ErrorState
                                           ,ERROR_PROCEDURE() AS ErrorProcedure
                                           ,ERROR_LINE() AS ErrorLine
                                           ,ERROR_MESSAGE() AS ErrorMessage;

 		                             SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset-->AttributeDatum'
		                             RAISERROR (@CustomErrorMessage, 16, 1);
				                     Set @RetMessage = @CustomErrorMessage;

                                END CATCH


			                    ----------------------------------------------------------------------
			                    -----End AttributeDatum Path--------------------------------------

                       --COMMIT TRANSACTION
                        Print 'Delete Attribute End';
   	                    RAISERROR (@RetMessage, 0, 1);
	                    END TRY
	                    BEGIN CATCH
  			                    Set @RetMessage = 'Failed ' + @RetMessage;
			                    Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			                    Print 'Error in AttributeDelete SP:  ' + @ErrorMessage;
			                    --ROLLBACK TRANSACTION;
			                    RAISERROR  (@RetMessage, 16, 1);  
	                    END CATCH;
	      
                      END
GO

BEGIN EXEC('
	                            DECLARE @RetMessage varchar(100);
	                            EXEC usp_orphan_cleanup @RetMessage OUTPUT;
                                EXEC usp_orphan_cleanup @RetMessage OUTPUT;
                                EXEC usp_orphan_cleanup @RetMessage OUTPUT; 
                            ')  END
GO

BEGIN
                EXEC('
                --Run these to avoid FK conflicts, more may be necessary
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_SimulationYearDetail_Id'') 
                ALTER INDEX [IX_SimulationYearDetail_Id] ON [dbo].[SimulationYearDetail] REBUILD;
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AssetSummaryDetail_Id'') 
                ALTER INDEX [IX_AssetSummaryDetail_Id] ON AssetSummaryDetail REBUILD;
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_TreatmentConsiderationDetail_Id'') 
                ALTER INDEX [IX_TreatmentConsiderationDetail_Id] ON [dbo].[TreatmentConsiderationDetail] REBUILD;

                --Drop SimulationOutput referencedFK''s with Cascade
                --Confirmed All Worked

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_AssetSummaryDetail_SimulationOutput_SimulationOutputId'')
                BEGIN 
                ALTER TABLE [dbo].[AssetSummaryDetail]  DROP  CONSTRAINT [FK_AssetSummaryDetail_SimulationOutput_SimulationOutputId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_SimulationYearDetail_SimulationOutput_SimulationOutputId'')
                BEGIN 
                ALTER TABLE [dbo].[SimulationYearDetail]  DROP  CONSTRAINT [FK_SimulationYearDetail_SimulationOutput_SimulationOutputId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_SimulationOutputJson_SimulationOutput_SimulationOutputId'')
                BEGIN 
                ALTER TABLE [dbo].[SimulationOutputJson]  DROP  CONSTRAINT [FK_SimulationOutputJson_SimulationOutput_SimulationOutputId];
                END


                --ReAdd  SimulationOutput referenced FK''s without Cascade
                --Confirmed All  Worked
                ALTER TABLE [dbo].[AssetSummaryDetail] WITH CHECK ADD CONSTRAINT [FK_AssetSummaryDetail_SimulationOutput_SimulationOutputId]  FOREIGN KEY ([SimulationOutputId]) REFERENCES [dbo].[SimulationOutput] ([Id])  ON DELETE NO ACTION;
                ALTER TABLE [dbo].[SimulationYearDetail] WITH CHECK ADD CONSTRAINT [FK_SimulationYearDetail_SimulationOutput_SimulationOutputId] FOREIGN KEY ([SimulationOutputId]) REFERENCES [dbo].[SimulationOutput] ([Id])  ON DELETE NO ACTION;
                ALTER TABLE [dbo].[SimulationOutputJson] WITH CHECK ADD CONSTRAINT [FK_SimulationOutputJson_SimulationOutput_SimulationOutputId]  FOREIGN KEY ([SimulationOutputId]) REFERENCES [dbo].[SimulationOutput] ([Id]) ON DELETE NO ACTION;

                --Drop Network referencedFK''s with Cascade
                --Confirmed All Worked

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_AnalysisMaintainableAsset_Network_NetworkId'')
                BEGIN ALTER TABLE [dbo].[AnalysisMaintainableAsset] DROP CONSTRAINT [FK_AnalysisMaintainableAsset_Network_NetworkId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_BenefitQuantifier_Network_NetworkId'')
                BEGIN
                ALTER TABLE [dbo].[BenefitQuantifier]  DROP  CONSTRAINT  [FK_BenefitQuantifier_Network_NetworkId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_MaintainableAsset_Network_NetworkId'')
                BEGIN
                ALTER TABLE [dbo].[MaintainableAsset]  DROP  CONSTRAINT  [FK_MaintainableAsset_Network_NetworkId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_NetworkAttribute_Network_NetworkId'')
                BEGIN
                ALTER TABLE [dbo].[NetworkAttribute]  DROP  CONSTRAINT  [FK_NetworkAttribute_Network_NetworkId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_NetworkRollupDetail_Network_NetworkId'')
                BEGIN
                ALTER TABLE [dbo].[NetworkRollupDetail]  DROP  CONSTRAINT  [FK_NetworkRollupDetail_Network_NetworkId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_Simulation_Network_NetworkId'')
                BEGIN
                ALTER TABLE [dbo].[Simulation]  DROP  CONSTRAINT  [FK_Simulation_Network_NetworkId];
                END



                --Drop Simulation referencedFK''s with Cascade
                --Confirmed All Worked

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_AnalysisMethod_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[AnalysisMethod]  DROP  CONSTRAINT  [FK_AnalysisMethod_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_InvestmentPlan_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[InvestmentPlan]  DROP  CONSTRAINT  [FK_InvestmentPlan_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioBudget_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioBudget]  DROP  CONSTRAINT  [FK_ScenarioBudget_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioBudgetPriority_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioBudgetPriority]  DROP  CONSTRAINT  [FK_ScenarioBudgetPriority_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioCalculatedAttribute_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioCalculatedAttribute]  DROP  CONSTRAINT  [FK_ScenarioCalculatedAttribute_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioCashFlowRule_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioCashFlowRule]  DROP  CONSTRAINT  [FK_ScenarioCashFlowRule_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioDeficientConditionGoal_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioDeficientConditionGoal]  DROP  CONSTRAINT  [FK_ScenarioDeficientConditionGoal_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioPerformanceCurve_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioPerformanceCurve]  DROP  CONSTRAINT  [FK_ScenarioPerformanceCurve_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioRemainingLifeLimit_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioRemainingLifeLimit]  DROP  CONSTRAINT  [FK_ScenarioRemainingLifeLimit_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioSelectableTreatment_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioSelectableTreatment]  DROP  CONSTRAINT  [FK_ScenarioSelectableTreatment_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioTargetConditionGoals_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioTargetConditionGoals]  DROP  CONSTRAINT  [FK_ScenarioTargetConditionGoals_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_Simulation_User_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[Simulation_User]  DROP  CONSTRAINT  [FK_Simulation_User_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_SimulationAnalysisDetail_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[SimulationAnalysisDetail]  DROP  CONSTRAINT  [FK_SimulationAnalysisDetail_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_SimulationLog_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[SimulationLog]  DROP  CONSTRAINT  [FK_SimulationLog_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_SimulationOutput_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[SimulationOutput]  DROP  CONSTRAINT  [FK_SimulationOutput_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_SimulationOutputJson_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[SimulationOutputJson]  DROP  CONSTRAINT  [FK_SimulationOutputJson_Simulation_SimulationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_SimulationReportDetail_Simulation_SimulationId'')
                BEGIN
                ALTER TABLE [dbo].[SimulationReportDetail]  DROP CONSTRAINT  [FK_SimulationReportDetail_Simulation_SimulationId];
                END
                --17


                --ReAdd  Simulation referenced FK''s without Cascade
                --Confirmed All  Worked
                ALTER TABLE [dbo].[AnalysisMethod]  WITH CHECK ADD  CONSTRAINT  [FK_AnalysisMethod_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[InvestmentPlan]  WITH CHECK ADD  CONSTRAINT  [FK_InvestmentPlan_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[ScenarioBudget]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioBudget_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[ScenarioBudgetPriority]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioBudgetPriority_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[ScenarioCalculatedAttribute]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioCalculatedAttribute_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[ScenarioCashFlowRule]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioCashFlowRule_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[ScenarioDeficientConditionGoal]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioDeficientConditionGoal_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[ScenarioPerformanceCurve]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioPerformanceCurve_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[ScenarioRemainingLifeLimit]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioRemainingLifeLimit_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[ScenarioSelectableTreatment]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioSelectableTreatment_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[ScenarioTargetConditionGoals]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioTargetConditionGoals_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[Simulation_User]  WITH CHECK ADD  CONSTRAINT  [FK_Simulation_User_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[SimulationAnalysisDetail]  WITH CHECK ADD  CONSTRAINT  [FK_SimulationAnalysisDetail_Simulation_SimulationId]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                ALTER TABLE [dbo].[SimulationOutputJson]  WITH CHECK ADD  CONSTRAINT  [FK_SimulationOutputJson_Simulation_SimulationId ]  FOREIGN KEY ([SimulationId]) REFERENCES [dbo].[Simulation] ([Id]);
                --14

                --ReAdd  Network referenced FK''s without Cascade
                --Confirmed All  Worked
                ALTER TABLE [dbo].[AnalysisMaintainableAsset]  WITH CHECK ADD  CONSTRAINT  [FK_AnalysisMaintainableAsset_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
                ALTER TABLE [dbo].[BenefitQuantifier]  WITH CHECK ADD  CONSTRAINT  [FK_BenefitQuantifier_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
                ALTER TABLE [dbo].[MaintainableAsset]  WITH CHECK ADD  CONSTRAINT  [FK_MaintainableAsset_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
                ALTER TABLE [dbo].[NetworkAttribute]  WITH CHECK ADD  CONSTRAINT  [FK_NetworkAttribute_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
                ALTER TABLE [dbo].[NetworkRollupDetail]  WITH CHECK ADD  CONSTRAINT  [FK_NetworkRollupDetail_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
                ALTER TABLE [dbo].[Simulation]  WITH CHECK ADD  CONSTRAINT  [FK_Simulation_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);

                --Run these to avoid FK conflicts because of Orphans

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioTreatmentConsequence_Equation_Equation_EquationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioTreatmentConsequence_Equation] DROP CONSTRAINT [FK_ScenarioTreatmentConsequence_Equation_Equation_EquationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioTreatmentCost_Equation_Equation_EquationId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioTreatmentCost_Equation] DROP CONSTRAINT [FK_ScenarioTreatmentCost_Equation_Equation_EquationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudget_ScenarioBudgetId'')
                BEGIN
                ALTER TABLE [dbo].[ScenarioSelectableTreatment_ScenarioBudget] DROP CONSTRAINT [FK_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudget_ScenarioBudgetId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_TreatmentConsequence_Equation_Equation_EquationId'')
                BEGIN
                ALTER TABLE [dbo].[TreatmentConsequence_Equation] DROP CONSTRAINT [FK_TreatmentConsequence_Equation_Equation_EquationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_TreatmentCost_Equation_Equation_EquationId'')
                BEGIN
                ALTER TABLE [dbo].[TreatmentCost_Equation] DROP CONSTRAINT [FK_TreatmentCost_Equation_Equation_EquationId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_CommittedProject_ScenarioBudget_ScenarioBudgetId'')
                BEGIN
                ALTER TABLE [dbo].[CommittedProject] DROP CONSTRAINT [FK_CommittedProject_ScenarioBudget_ScenarioBudgetId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_CriterionLibrary_ScenarioBudget_ScenarioBudget_ScenarioBudgetId'')
                BEGIN
                ALTER TABLE [dbo].[CriterionLibrary_ScenarioBudget] DROP CONSTRAINT [FK_CriterionLibrary_ScenarioBudget_ScenarioBudget_ScenarioBudgetId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_TreatmentLibrary_User_User_UserEntityId'')
                BEGIN
                ALTER TABLE [dbo].[TreatmentLibrary_User] DROP CONSTRAINT [FK_TreatmentLibrary_User_User_UserEntityId];
                END

                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_TreatmentLibrary_User_User_UserId'')
                BEGIN
                ALTER TABLE [dbo].[TreatmentLibrary_User] DROP CONSTRAINT [FK_TreatmentLibrary_User_User_UserId];
                END
                --9


                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_CriterionLibrary_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRuleId'')
                BEGIN 
                ALTER TABLE [dbo].[CriterionLibrary_ScenarioTreatmentSupersedeRule]  DROP  CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRuleId];
                END

                ALTER TABLE [dbo].[CriterionLibrary_ScenarioTreatmentSupersedeRule]  WITH CHECK ADD  CONSTRAINT  [FK_CriterionLibrary_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRuleId]  FOREIGN KEY ([ScenarioTreatmentSupersedeRuleId]) REFERENCES [dbo].[ScenarioTreatmentSupersedeRule] ([Id]) ON DELETE NO ACTION;


                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = ''FK_ScenarioTreatmentSupersedeRule_ScenarioSelectableTreatment_TreatmentId'')
                BEGIN 
                ALTER TABLE [dbo].[ScenarioTreatmentSupersedeRule]  DROP  CONSTRAINT [FK_ScenarioTreatmentSupersedeRule_ScenarioSelectableTreatment_TreatmentId];
                END

                ALTER TABLE [dbo].[ScenarioTreatmentSupersedeRule]  WITH CHECK ADD  CONSTRAINT  [FK_ScenarioTreatmentSupersedeRule_ScenarioSelectableTreatment_TreatmentId]  FOREIGN KEY ([TreatmentId]) REFERENCES [dbo].[ScenarioSelectableTreatment] ([Id]) ON DELETE NO ACTION;
                ')
                END
GO

BEGIN 
                EXEC('
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_MaintainableAsset_NetworkId'') 
                DROP INDEX [IX_MaintainableAsset_NetworkId] ON [dbo].[MaintainableAsset]


                CREATE NONCLUSTERED INDEX [IX_MaintainableAsset_NetworkId] ON [dbo].[MaintainableAsset]
                ([NetworkId] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_TreatmentConsiderationDetail_AssetDetailId'') 
                DROP INDEX [IX_TreatmentConsiderationDetail_AssetDetailId] ON [dbo].[TreatmentConsiderationDetail]

                CREATE NONCLUSTERED INDEX [IX_TreatmentConsiderationDetail_AssetDetailId] ON [dbo].[TreatmentConsiderationDetail]
                (
                [AssetDetailId] ASC
                )
                INCLUDE ([BudgetPriorityLevel],[TreatmentName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -----------------------------------------


                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_TreatmentOptionDetail_AssetDetailId'') 
                DROP INDEX [IX_TreatmentOptionDetail_AssetDetailId] ON [dbo].[TreatmentOptionDetail]

                CREATE NONCLUSTERED INDEX [IX_TreatmentOptionDetail_AssetDetailId] ON [dbo].[TreatmentOptionDetail]
                (
                [AssetDetailId] ASC) INCLUDE ([Benefit],[Cost],[RemainingLife],[TreatmentName],[ConditionChange])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_ScenarioBudget_SimulationId'')
                DROP INDEX [IX_ScenarioBudget_SimulationId] ON [dbo].[ScenarioBudget]


                CREATE NONCLUSTERED INDEX [IX_ScenarioBudget_SimulationId] ON [dbo].[ScenarioBudget]
                ([SimulationId] ASC) INCLUDE([Name],[BudgetOrder],[LibraryId],[IsModified])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]



                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_ScenarioBudgetAmount_ScenarioBudgetId'')
                DROP INDEX [IX_ScenarioBudgetAmount_ScenarioBudgetId] ON [dbo].[ScenarioBudgetAmount]

                CREATE NONCLUSTERED INDEX [IX_ScenarioBudgetAmount_ScenarioBudgetId] ON [dbo].[ScenarioBudgetAmount]
                (
                [ScenarioBudgetId] ASC) INCLUDE ([Year],[Value])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_BudgetPercentagePair_ScenarioBudgetPriorityId'')
                DROP INDEX [IX_BudgetPercentagePair_ScenarioBudgetPriorityId] ON [dbo].[BudgetPercentagePair]

                CREATE NONCLUSTERED INDEX [IX_BudgetPercentagePair_ScenarioBudgetPriorityId] ON [dbo].[BudgetPercentagePair]
                (	[ScenarioBudgetPriorityId] ASC) INCLUDE ([Percentage],[ScenarioBudgetId])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_ScenarioCalculatedAttributePair_ScenarioCalculatedAttribute_ScenarioCalculatedAttributeId'')
                DROP INDEX [IX_ScenarioCalculatedAttributePair_ScenarioCalculatedAttribute_ScenarioCalculatedAttributeId] ON [dbo].[ScenarioCalculatedAttributePair]

                CREATE NONCLUSTERED INDEX [IX_ScenarioCalculatedAttributePair_ScenarioCalculatedAttribute_ScenarioCalculatedAttributeId] ON [dbo].[ScenarioCalculatedAttributePair]
                ([ScenarioCalculatedAttributeId] ASC
                ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                -----------------------------------------


                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_ScenarioPerformanceCurve_SimulationId'')
                DROP INDEX [IX_ScenarioPerformanceCurve_SimulationId] ON [dbo].[ScenarioPerformanceCurve]

                CREATE NONCLUSTERED INDEX [IX_ScenarioPerformanceCurve_SimulationId] ON [dbo].[ScenarioPerformanceCurve]
                (
                [SimulationId] ASC) INCLUDE ([AttributeId],[Name],[Shift],[LibraryId],[IsModified])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatmentId'')
                DROP INDEX [IX_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioConditionalTreatmentConsequences]


                CREATE NONCLUSTERED INDEX [IX_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioConditionalTreatmentConsequences]
                (
                [ScenarioSelectableTreatmentId] ASC) INCLUDE ([AttributeId],[ChangeValue])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_ScenarioSelectableTreatment_SimulationId'')
                DROP INDEX [IX_ScenarioSelectableTreatment_SimulationId] ON [dbo].[ScenarioSelectableTreatment]

                CREATE NONCLUSTERED INDEX [IX_ScenarioSelectableTreatment_SimulationId] ON [dbo].[ScenarioSelectableTreatment]
                (
                [SimulationId] ASC) INCLUDE ([Description],[Name],[ShadowForAnyTreatment],[ShadowForSameTreatment],[AssetType],[Category],[IsModified],[LibraryId],[IsUnselectable])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_SimulationLog_SimulationId'')
                DROP INDEX [IX_SimulationLog_SimulationId] ON [dbo].[SimulationLog]

                CREATE NONCLUSTERED INDEX [IX_SimulationLog_SimulationId] ON [dbo].[SimulationLog]
                (
                [SimulationId] ASC) INCLUDE ([Status],[Subject],[Message])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_MaintainableAssetLocation_MaintainableAssetId'')
                DROP INDEX [IX_MaintainableAssetLocation_MaintainableAssetId] ON [dbo].[MaintainableAssetLocation]

                CREATE UNIQUE NONCLUSTERED INDEX [IX_MaintainableAssetLocation_MaintainableAssetId] ON [dbo].[MaintainableAssetLocation]
                (
                [MaintainableAssetId] ASC) INCLUDE ([Discriminator],[LocationIdentifier],[Start],[End],[Direction])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                -----------------------------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AssetSummaryDetail_MaintainableAssetId'')
                DROP INDEX [IX_AssetSummaryDetail_MaintainableAssetId] ON [dbo].[AssetSummaryDetail]

                CREATE NONCLUSTERED INDEX [IX_AssetSummaryDetail_MaintainableAssetId] ON [dbo].[AssetSummaryDetail]
                (
                [MaintainableAssetId] ASC ) INCLUDE ([SimulationOutputId])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_CashFlowConsiderationDetail_TreatmentConsiderationDetailId'')
                DROP INDEX [IX_CashFlowConsiderationDetail_TreatmentConsiderationDetailId] ON [dbo].[CashFlowConsiderationDetail]

                CREATE NONCLUSTERED INDEX [IX_CashFlowConsiderationDetail_TreatmentConsiderationDetailId] ON [dbo].[CashFlowConsiderationDetail]
                (
                [TreatmentConsiderationDetailId] ASC ) INCLUDE ([CashFlowRuleName],[ReasonAgainstCashFlow])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                -----------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_BudgetPercentagePair_ScenarioBudgetId'')
                DROP INDEX [IX_BudgetPercentagePair_ScenarioBudgetId] ON [dbo].[BudgetPercentagePair]

                CREATE NONCLUSTERED INDEX [IX_BudgetPercentagePair_ScenarioBudgetId] ON [dbo].[BudgetPercentagePair]
                (
                [ScenarioBudgetId] ASC ) INCLUDE ([Percentage],[ScenarioBudgetPriorityId])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -----------------------------------------


                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurveId'')
                DROP INDEX [IX_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurveId] ON [dbo].[CriterionLibrary_ScenarioPerformanceCurve]

                CREATE UNIQUE NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurveId] ON [dbo].[CriterionLibrary_ScenarioPerformanceCurve]
                (
                [ScenarioPerformanceCurveId] ASC
                ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -----------------------------------------

 
                IF EXISTS (SELECT name FROM sys.indexes  
                            WHERE name = N''IX_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibraryId'') 
                DROP INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibraryId] ON [dbo].[CriterionLibrary_ScenarioTreatmentConsequence]


                CREATE NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibraryId] ON [dbo].[CriterionLibrary_ScenarioTreatmentConsequence]
                (
                [CriterionLibraryId] ASC
                )  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                ----------------------------------------------------------------------------------------------------------------


                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequenceId'')
                DROP INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequenceId] ON [dbo].[CriterionLibrary_ScenarioTreatmentConsequence]

                CREATE UNIQUE NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequenceId] ON [dbo].[CriterionLibrary_ScenarioTreatmentConsequence]
                ([ScenarioConditionalTreatmentConsequenceId] ASC
                ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                ---------------------------------------------------------


                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_ScenarioTreatmentCost_ScenarioSelectableTreatmentId'')
                DROP INDEX [IX_ScenarioTreatmentCost_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioTreatmentCost]

                CREATE NONCLUSTERED INDEX [IX_ScenarioTreatmentCost_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioTreatmentCost]
                ([ScenarioSelectableTreatmentId] ASC
                ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                IF EXISTS (SELECT name FROM sys.indexes  WHERE name = N''IX_TreatmentConsequence_SelectableTreatmentId'')   
                    DROP INDEX IX_TreatmentConsequence_SelectableTreatmentId ON [dbo].[TreatmentConsequence];   
    
  
                CREATE NonCLUSTERED INDEX [IX_TreatmentConsequence_SelectableTreatmentId]
                    ON [dbo].TreatmentConsequence ([SelectableTreatmentId] ASC)
                INCLUDE ([AttributeId],[ChangeValue]
                ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
  

                ----------------------------------------------------------------------------------------


  
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_TreatmentConsequence_AttributeId'')   
                    DROP INDEX IX_TreatmentConsequence_AttributeId ON [dbo].[TreatmentConsequence];   
    
  
                CREATE NonCLUSTERED INDEX [IX_TreatmentConsequence_AttributeId]
                    ON [dbo].[TreatmentConsequence] ([AttributeId] ASC)
                INCLUDE ([SelectableTreatmentId],[ChangeValue]
                ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
   

                ----------------------------------------------------------------------------------------
  
  
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_SimulationOutput_ChangeTracking'')  
                DROP INDEX [IX_SimulationOutput_ChangeTracking] ON [dbo].SimulationOutput


                CREATE NONCLUSTERED INDEX IX_SimulationOutput_ChangeTracking ON dbo.SimulationOutput(Id)
                INCLUDE(CreatedDate,LastModifiedDate,CreatedBy,LastModifiedBy) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                ----------------------------------------------------------------------------------------
  
 
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatmentId'')  
                DROP INDEX [IX_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatmentId] ON [dbo].[CriterionLibrary_ScenarioTreatment]


                CREATE UNIQUE NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatmentId] ON [dbo].[CriterionLibrary_ScenarioTreatment]
                (
                [ScenarioSelectableTreatmentId] ASC
                )  INCLUDE( [CriterionLibraryId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                ----------------------------------------------------------------------------------------
   
   
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_CriterionLibrary_ScenarioTreatment_CriterionLibraryId'') 
                DROP INDEX [IX_CriterionLibrary_ScenarioTreatment_CriterionLibraryId] ON [dbo].[CriterionLibrary_ScenarioTreatment]


                CREATE NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioTreatment_CriterionLibraryId] ON [dbo].[CriterionLibrary_ScenarioTreatment]
                (
                [CriterionLibraryId] ASC
                )INCLUDE( [ScenarioSelectableTreatmentId])  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                ----------------------------------------------------------------------------------------
  
  
                IF EXISTS (SELECT name FROM sys.indexes  WHERE name = N''IX_CriterionLibrary_Id'') 
                DROP INDEX [IX_CriterionLibrary_Id] ON [dbo].[CriterionLibrary]

                CREATE NONCLUSTERED INDEX [IX_CriterionLibrary_Id] ON [dbo].[CriterionLibrary]
                (
                [Id] ASC
                )
                INCLUDE([IsSingleUse]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -------------------------------------------------------------------------
                --[ScenarioSelectableTreatment_ScenarioBudget] 

 
                IF EXISTS (SELECT name FROM sys.indexes  
                            WHERE name = N''IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudgetId'') 
                DROP INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudgetId] ON [dbo].[ScenarioSelectableTreatment_ScenarioBudget]


                CREATE NONCLUSTERED INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudgetId] ON [dbo].[ScenarioSelectableTreatment_ScenarioBudget]
                ( [ScenarioBudgetId] ASC
                ) INCLUDE([ScenarioSelectableTreatmentId])   WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                ------------------------------------------------------------------------------------------------
                --[ScenarioSelectableTreatment_ScenarioBudget] 

  
                IF EXISTS (SELECT name FROM sys.indexes  
                            WHERE name = N''IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatmentId'') 
                DROP INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioSelectableTreatment_ScenarioBudget]


                CREATE NONCLUSTERED INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioSelectableTreatment_ScenarioBudget]
                (
                [ScenarioSelectableTreatmentId] ASC
                ) INCLUDE([ScenarioBudgetId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                ----------------------------------------------------

  
                IF EXISTS (SELECT name FROM sys.indexes  
                            WHERE name = N''IX_Equation_Expression'') 
                DROP INDEX [IX_Equation_Expression] ON [dbo].[Equation]


                CREATE NONCLUSTERED INDEX [IX_Equation_Expression] ON [dbo].[Equation]
                (
                [CreatedBy] ASC
                ) INCLUDE([LastModifiedBy])
                WITH  (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -----------------------------------------------------------------------------
  
  
                IF EXISTS (SELECT name FROM sys.indexes  
                            WHERE name = N''IX_Attribute_DataSourceId'') 
                DROP INDEX [IX_Attribute_DataSourceId] ON [dbo].[Attribute]

                CREATE NONCLUSTERED INDEX [IX_Attribute_DataSourceId] ON [dbo].[Attribute]
                (
                [DataSourceId] ASC
                ) INCLUDE([Name],[DataType],[AggregationRuleType],[Command],[ConnectionType],[DefaultValue],[Minimum],[Maximum],[IsCalculated],[IsAscending]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -------------------------------------------------------------------------------------------------------------

  
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_Attribute_Name'') 
                DROP INDEX [IX_Attribute_Name] ON [dbo].[Attribute]

                CREATE UNIQUE NONCLUSTERED INDEX [IX_Attribute_Name] ON [dbo].[Attribute]
                ([Name] ASC) INCLUDE([DataSourceId],[DataType],[AggregationRuleType],[Command],[ConnectionType],[DefaultValue],[Minimum],[Maximum],[IsCalculated],[IsAscending])  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                ----------------------------------------------------------------------

  
                IF EXISTS (SELECT name FROM sys.indexes  
                            WHERE name = N''IX_ScenarioTreatmentConsequence_Equation_EquationId'') 
                DROP INDEX [IX_ScenarioTreatmentConsequence_Equation_EquationId] ON [dbo].[ScenarioTreatmentConsequence_Equation]


                CREATE UNIQUE NONCLUSTERED INDEX [IX_ScenarioTreatmentConsequence_Equation_EquationId] ON [dbo].[ScenarioTreatmentConsequence_Equation]
                (
                [EquationId] ASC
                ) INCLUDE([ScenarioConditionalTreatmentConsequenceId],[CreatedBy],[LastModifiedBy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                ------------------------------------------------------------------------------------

  
                IF EXISTS (SELECT name FROM sys.indexes  
                            WHERE name = N''IX_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequenceId'') 
                DROP INDEX [IX_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequenceId] ON [dbo].[ScenarioTreatmentConsequence_Equation]


                CREATE UNIQUE NONCLUSTERED INDEX [IX_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequenceId] ON [dbo].[ScenarioTreatmentConsequence_Equation]
                (
                [ScenarioConditionalTreatmentConsequenceId] ASC
                ) INCLUDE([EquationId],[CreatedBy],[LastModifiedBy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                ----------------------------------------------------------------------------------------------------------------

  
                IF EXISTS (SELECT name FROM sys.indexes  
                            WHERE name = N''IX_AssetSummaryDetail_SimulationOutput_SimulationOutputId'') 
                DROP INDEX [IX_AssetSummaryDetail_SimulationOutput_SimulationOutputId] ON [dbo].[AssetSummaryDetail]


                CREATE NONCLUSTERED INDEX [IX_AssetSummaryDetail_SimulationOutput_SimulationOutputId] ON [dbo].[AssetSummaryDetail]
                (
                [SimulationOutputId] ASC
                )
                INCLUDE([MaintainableAssetId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                ----------------------------------------------------------------------------------------------------------------
  
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_BudgetDetail_SimulationYearDetailId'') 
                DROP INDEX [IX_BudgetDetail_SimulationYearDetailId] ON [dbo].[BudgetDetail]

                CREATE NONCLUSTERED INDEX [IX_BudgetDetail_SimulationYearDetailId] ON [dbo].[BudgetDetail]
                ([SimulationYearDetailId] ASC)
                INCLUDE ([AvailableFunding],[BudgetName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
 
                 --22sec
                 --2:40 total
                ----------------------------------------------------------------------------------------
                 --Large Tables
                --------------------------------------------------------------------------------------------------------
  
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AssetDetail_MaintainableAsset_MaintainableAssetId'') DROP INDEX [IX_AssetDetail_MaintainableAsset_MaintainableAssetId] ON [dbo].[AssetDetail]

                CREATE NONCLUSTERED INDEX [IX_AssetDetail_MaintainableAsset_MaintainableAssetId] ON [dbo].[AssetDetail]
                ([MaintainableAssetId] ASC) INCLUDE([SimulationYearDetailId],[AppliedTreatment],[TreatmentCause],[TreatmentFundingIgnoresSpendingLimit],[TreatmentStatus]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                ----------------------------------------------------------------------------------------------------------------
  
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AssetDetail_SimulationYearDetail_SimulationYearDetailId'') DROP INDEX [IX_AssetDetail_SimulationYearDetail_SimulationYearDetailId] ON [dbo].[AssetDetail]

                CREATE NONCLUSTERED INDEX [IX_AssetDetail_SimulationYearDetail_SimulationYearDetailId] ON [dbo].[AssetDetail]
                ([SimulationYearDetailId] ASC) INCLUDE([MaintainableAssetId],[AppliedTreatment],[TreatmentCause],[TreatmentFundingIgnoresSpendingLimit],[TreatmentStatus]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                ------------------------------------------------------------------------------------------------------------------------------------------
   
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AggregatedResult_AttributeId'')  DROP INDEX IX_AggregatedResult_AttributeId ON [dbo].[AggregatedResult];   
    
                CREATE NonCLUSTERED INDEX [IX_AggregatedResult_AttributeId]  ON [dbo].[AggregatedResult] ([AttributeId] ASC)
                INCLUDE ([Discriminator],[Year],[TextValue],[NumericValue],[MaintainableAssetId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
 
                -------------------------------------------------------------------------------------------
  
                IF EXISTS (SELECT name FROM sys.indexes  WHERE name = N''IX_AggregatedResult_MaintainableAssetId'') DROP INDEX [IX_AggregatedResult_MaintainableAssetId] ON [dbo].[AggregatedResult]

                CREATE NONCLUSTERED INDEX [IX_AggregatedResult_MaintainableAssetId] ON [dbo].[AggregatedResult]
                ([MaintainableAssetId] ASC) INCLUDE([Discriminator],[Year],[TextValue],[NumericValue],[AttributeId] ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                 ----------------------------------------------------------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AttributeDatumLocation_AttributeDatumId'') DROP INDEX [IX_AttributeDatumLocation_AttributeDatumId] ON [dbo].[AttributeDatumLocation]

                CREATE UNIQUE NONCLUSTERED INDEX [IX_AttributeDatumLocation_AttributeDatumId] ON [dbo].[AttributeDatumLocation]
                ([AttributeDatumId] ASC ) INCLUDE ([Id],[Discriminator],[LocationIdentifier],[Start],[End],[Direction])	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                ----------------------------------------------------------------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AttributeDatum_MaintainableAssetId'') DROP INDEX [IX_AttributeDatum_MaintainableAssetId] ON [dbo].[AttributeDatum]

                CREATE NONCLUSTERED INDEX [IX_AttributeDatum_MaintainableAssetId]
                ON [dbo].[AttributeDatum] ([MaintainableAssetId])
                INCLUDE ([Id],[Discriminator],[TimeStamp],[NumericValue],[TextValue],[AttributeId])	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	
                -----------------------------------------------------------------------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AttributeDatum_AttributeId'') DROP INDEX IX_AttributeDatum_AttributeId ON [dbo].[AttributeDatum];   
    
                CREATE NonCLUSTERED INDEX [IX_AttributeDatum_AttributeId]
                    ON [dbo].[AttributeDatum] ([AttributeId] ASC)
                INCLUDE ([Discriminator],[TimeStamp],[NumericValue],[TextValue],[MaintainableAssetId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
 
                ----------------------------------------------------------------------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_BudgetUsageDetail_TreatmentConsiderationDetailId'') 
	                DROP INDEX [IX_BudgetUsageDetail_TreatmentConsiderationDetailId] ON [dbo].[BudgetUsageDetail]

                CREATE NONCLUSTERED INDEX [IX_BudgetUsageDetail_TreatmentConsiderationDetailId] ON [dbo].[BudgetUsageDetail]
                ([TreatmentConsiderationDetailId] ASC) INCLUDE ([BudgetName],[CoveredCost],[Status]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -------------------------------------------------------------------------------------------------------
 
                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AssetSummaryDetailValueIntId_AssetSummaryDetailId'') 
                DROP INDEX [IX_AssetSummaryDetailValueIntId_AssetSummaryDetailId] ON [dbo].[AssetSummaryDetailValueIntId]

                CREATE NONCLUSTERED INDEX [IX_AssetSummaryDetailValueIntId_AssetSummaryDetailId]
                ON [dbo].[AssetSummaryDetailValueIntId] ([AssetSummaryDetailId])
                INCLUDE ([Id],[Discriminator],[TextValue],[NumericValue],[AttributeId])

                ----------------------------------------------------------------------------------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_TreatmentRejectionDetail_AssetDetailId'') 
                DROP INDEX [IX_TreatmentRejectionDetail_AssetDetailId] ON [dbo].[TreatmentRejectionDetail]

                CREATE NONCLUSTERED INDEX [IX_TreatmentRejectionDetail_AssetDetailId]
                ON [dbo].[TreatmentRejectionDetail] ([AssetDetailId])
                INCLUDE ([Id],[TreatmentName],[TreatmentRejectionReason],[PotentialConditionChange])
                WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


                -----------------------------------------------------------------------------------------------------------------------------------------
  
                IF EXISTS (SELECT name FROM sys.indexes  WHERE name = N''IX_AssetDetailValueIntId_Attribute_AttributeId'') 
                DROP INDEX [IX_AssetDetailValueIntId_Attribute_AttributeId] ON [dbo].[AssetDetailValueIntId]

                CREATE NONCLUSTERED INDEX [IX_AssetDetailValueIntId_Attribute_AttributeId] ON [dbo].[AssetDetailValueIntId]([AttributeId] ASC)
                INCLUDE([AssetDetailId],[Discriminator],[TextValue],[NumericValue]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                ------Last-------------------------------------------------------------------------------------------------------------------------------------------

                IF EXISTS (SELECT name FROM sys.indexes WHERE name = N''IX_AssetDetailValueIntId_AssetDetailId'') 
                DROP INDEX [IX_AssetDetailValueIntId_AssetDetailId] ON [dbo].[AssetDetailValueIntId]

                CREATE NONCLUSTERED INDEX [IX_AssetDetailValueIntId_AssetDetailId] ON [dbo].[AssetDetailValueIntId] ([AssetDetailId])
                INCLUDE ([Id],[Discriminator],[TextValue],[NumericValue],[AttributeId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

                ---Total Time 32:28
                ---Last--------------------------------------------------------------------------------------------------------------
                   ')

            END
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20231205172506_AddDeleteSProcsandIndexes', N'6.0.1');
GO

COMMIT;
GO

