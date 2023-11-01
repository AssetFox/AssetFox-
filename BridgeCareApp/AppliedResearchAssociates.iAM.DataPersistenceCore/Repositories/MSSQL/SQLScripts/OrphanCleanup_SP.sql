
Create PROCEDURE dbo.usp_orphan_cleanup(
@RetMessage VARCHAR(250) OUTPUT
)
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
Delete FROM dbo.[ScenarioTreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[ScenarioSelectableTreatment] AS parent WHERE parent.Id = [ScenarioTreatmentSupersedeRule].PreventScenarioSelectableTreatmentId);
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
Delete FROM dbo.[TreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[SelectableTreatment] AS parent WHERE parent.Id = [TreatmentSupersedeRule].PreventSelectableTreatmentId);
Delete FROM dbo.[TreatmentSupersedeRule] WHERE NOT EXISTS (SELECT * FROM dbo.[SelectableTreatment] AS parent WHERE parent.Id = [TreatmentSupersedeRule].TreatmentId);
Delete FROM dbo.[UserCriteria_Filter] WHERE NOT EXISTS (SELECT * FROM dbo.[User] AS parent WHERE parent.Id = [UserCriteria_Filter].UserId);
--14 exceptions

     COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
  			Set @RetMessage = 'Failed ' + @RetMessage;
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Rolled Back Orphan SP:  ' + @ErrorMessage;
			ROLLBACK TRANSACTION;
			RAISERROR  (@RetMessage, 16, 1);  
    END CATCH
END;

--  DECLARE @RetMessage varchar(100); EXEC usp_orphan_cleanup @RetMessage OUTPUT; 



