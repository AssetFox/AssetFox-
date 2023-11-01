
Create PROCEDURE dbo.usp_master_procedure AS 

    BEGIN 

 	DECLARE @CustomErrorMessage NVARCHAR(MAX),
	@ErrorNumber int,
	@ErrorSeverity int,
	@ErrorState int,
	@ErrorProcedure nvarchar(126),
	@ErrorLine int,
	@ErrorMessage nvarchar(4000);
	--DECLARE @CurrentDateTime DATETIME;
	DECLARE @RetMessage varchar(100);

	BEGIN TRY

	    -- Execute Orphan CleanUp the first time
    EXEC usp_orphan_cleanup @RetMessage OUTPUT;

    -- Execute Orphan CleanUp the second time
    EXEC usp_orphan_cleanup @RetMessage OUTPUT;

    -- Execute Orphan CleanUp the third time
    EXEC usp_orphan_cleanup @RetMessage OUTPUT; 

	--Run these to avoid FK conflicts, more may be necessary




--Run these to avoid FK conflicts, more may be necessary
ALTER INDEX [IX_SimulationYearDetail_Id] ON [dbo].[SimulationYearDetail] REBUILD;
ALTER INDEX [IX_AssetSummaryDetail_Id] ON AssetSummaryDetail REBUILD;
ALTER INDEX [IX_TreatmentConsiderationDetail_Id] ON [dbo].[TreatmentConsiderationDetail] REBUILD;

--Drop SimulationOutput referencedFK's with Cascade
--Confirmed All Worked

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_AssetSummaryDetail_SimulationOutput_SimulationOutputId')
BEGIN 
ALTER TABLE [dbo].[AssetSummaryDetail]  DROP  CONSTRAINT [FK_AssetSummaryDetail_SimulationOutput_SimulationOutputId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_SimulationYearDetail_SimulationOutput_SimulationOutputId')
BEGIN 
ALTER TABLE [dbo].[SimulationYearDetail]  DROP  CONSTRAINT [FK_SimulationYearDetail_SimulationOutput_SimulationOutputId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_SimulationOutputJson_SimulationOutput_SimulationOutputId')
BEGIN 
ALTER TABLE [dbo].[SimulationOutputJson]  DROP  CONSTRAINT [FK_SimulationOutputJson_SimulationOutput_SimulationOutputId];
END


--ReAdd  SimulationOutput referenced FK's without Cascade
--Confirmed All  Worked
ALTER TABLE [dbo].[AssetSummaryDetail] WITH CHECK ADD CONSTRAINT [FK_AssetSummaryDetail_SimulationOutput_SimulationOutputId]  FOREIGN KEY ([SimulationOutputId]) REFERENCES [dbo].[SimulationOutput] ([Id])  ON DELETE NO ACTION;
ALTER TABLE [dbo].[SimulationYearDetail] WITH CHECK ADD CONSTRAINT [FK_SimulationYearDetail_SimulationOutput_SimulationOutputId] FOREIGN KEY ([SimulationOutputId]) REFERENCES [dbo].[SimulationOutput] ([Id])  ON DELETE NO ACTION;
ALTER TABLE [dbo].[SimulationOutputJson] WITH CHECK ADD CONSTRAINT [FK_SimulationOutputJson_SimulationOutput_SimulationOutputId]  FOREIGN KEY ([SimulationOutputId]) REFERENCES [dbo].[SimulationOutput] ([Id]) ON DELETE NO ACTION;

  --Drop Network referencedFK's with Cascade
--Confirmed All Worked

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_AnalysisMaintainableAsset_Network_NetworkId')
BEGIN ALTER TABLE [dbo].[AnalysisMaintainableAsset] DROP CONSTRAINT [FK_AnalysisMaintainableAsset_Network_NetworkId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_BenefitQuantifier_Network_NetworkId')
BEGIN
ALTER TABLE [dbo].[BenefitQuantifier]  DROP  CONSTRAINT  [FK_BenefitQuantifier_Network_NetworkId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_MaintainableAsset_Network_NetworkId')
BEGIN
ALTER TABLE [dbo].[MaintainableAsset]  DROP  CONSTRAINT  [FK_MaintainableAsset_Network_NetworkId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_NetworkAttribute_Network_NetworkId')
BEGIN
ALTER TABLE [dbo].[NetworkAttribute]  DROP  CONSTRAINT  [FK_NetworkAttribute_Network_NetworkId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_NetworkRollupDetail_Network_NetworkId')
BEGIN
ALTER TABLE [dbo].[NetworkRollupDetail]  DROP  CONSTRAINT  [FK_NetworkRollupDetail_Network_NetworkId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_Simulation_Network_NetworkId')
BEGIN
ALTER TABLE [dbo].[Simulation]  DROP  CONSTRAINT  [FK_Simulation_Network_NetworkId];
END



--Drop Simulation referencedFK's with Cascade
--Confirmed All Worked

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_AnalysisMethod_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[AnalysisMethod]  DROP  CONSTRAINT  [FK_AnalysisMethod_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_InvestmentPlan_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[InvestmentPlan]  DROP  CONSTRAINT  [FK_InvestmentPlan_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioBudget_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[ScenarioBudget]  DROP  CONSTRAINT  [FK_ScenarioBudget_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioBudgetPriority_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[ScenarioBudgetPriority]  DROP  CONSTRAINT  [FK_ScenarioBudgetPriority_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioCalculatedAttribute_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[ScenarioCalculatedAttribute]  DROP  CONSTRAINT  [FK_ScenarioCalculatedAttribute_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioCashFlowRule_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[ScenarioCashFlowRule]  DROP  CONSTRAINT  [FK_ScenarioCashFlowRule_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioDeficientConditionGoal_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[ScenarioDeficientConditionGoal]  DROP  CONSTRAINT  [FK_ScenarioDeficientConditionGoal_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioPerformanceCurve_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[ScenarioPerformanceCurve]  DROP  CONSTRAINT  [FK_ScenarioPerformanceCurve_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioRemainingLifeLimit_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[ScenarioRemainingLifeLimit]  DROP  CONSTRAINT  [FK_ScenarioRemainingLifeLimit_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioSelectableTreatment_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[ScenarioSelectableTreatment]  DROP  CONSTRAINT  [FK_ScenarioSelectableTreatment_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioTargetConditionGoals_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[ScenarioTargetConditionGoals]  DROP  CONSTRAINT  [FK_ScenarioTargetConditionGoals_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_Simulation_User_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[Simulation_User]  DROP  CONSTRAINT  [FK_Simulation_User_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_SimulationAnalysisDetail_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[SimulationAnalysisDetail]  DROP  CONSTRAINT  [FK_SimulationAnalysisDetail_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_SimulationLog_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[SimulationLog]  DROP  CONSTRAINT  [FK_SimulationLog_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_SimulationOutput_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[SimulationOutput]  DROP  CONSTRAINT  [FK_SimulationOutput_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_SimulationOutputJson_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[SimulationOutputJson]  DROP  CONSTRAINT  [FK_SimulationOutputJson_Simulation_SimulationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_SimulationReportDetail_Simulation_SimulationId')
BEGIN
ALTER TABLE [dbo].[SimulationReportDetail]  DROP CONSTRAINT  [FK_SimulationReportDetail_Simulation_SimulationId];
END
--17


--ReAdd  Simulation referenced FK's without Cascade
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

--ReAdd  Network referenced FK's without Cascade
--Confirmed All  Worked
ALTER TABLE [dbo].[AnalysisMaintainableAsset]  WITH CHECK ADD  CONSTRAINT  [FK_AnalysisMaintainableAsset_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
ALTER TABLE [dbo].[BenefitQuantifier]  WITH CHECK ADD  CONSTRAINT  [FK_BenefitQuantifier_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
ALTER TABLE [dbo].[MaintainableAsset]  WITH CHECK ADD  CONSTRAINT  [FK_MaintainableAsset_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
ALTER TABLE [dbo].[NetworkAttribute]  WITH CHECK ADD  CONSTRAINT  [FK_NetworkAttribute_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
ALTER TABLE [dbo].[NetworkRollupDetail]  WITH CHECK ADD  CONSTRAINT  [FK_NetworkRollupDetail_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);
ALTER TABLE [dbo].[Simulation]  WITH CHECK ADD  CONSTRAINT  [FK_Simulation_Network_NetworkId ]  FOREIGN KEY ([NetworkId]) REFERENCES [dbo].[Network] ([Id]);

--Run these to avoid FK conflicts because of Orphans

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioTreatmentConsequence_Equation_Equation_EquationId')
BEGIN
ALTER TABLE [dbo].[ScenarioTreatmentConsequence_Equation] DROP CONSTRAINT [FK_ScenarioTreatmentConsequence_Equation_Equation_EquationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioTreatmentCost_Equation_Equation_EquationId')
BEGIN
ALTER TABLE [dbo].[ScenarioTreatmentCost_Equation] DROP CONSTRAINT [FK_ScenarioTreatmentCost_Equation_Equation_EquationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudget_ScenarioBudgetId')
BEGIN
ALTER TABLE [dbo].[ScenarioSelectableTreatment_ScenarioBudget] DROP CONSTRAINT [FK_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudget_ScenarioBudgetId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_TreatmentConsequence_Equation_Equation_EquationId')
BEGIN
ALTER TABLE [dbo].[TreatmentConsequence_Equation] DROP CONSTRAINT [FK_TreatmentConsequence_Equation_Equation_EquationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_TreatmentCost_Equation_Equation_EquationId')
BEGIN
ALTER TABLE [dbo].[TreatmentCost_Equation] DROP CONSTRAINT [FK_TreatmentCost_Equation_Equation_EquationId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_CommittedProject_ScenarioBudget_ScenarioBudgetId')
BEGIN
ALTER TABLE [dbo].[CommittedProject] DROP CONSTRAINT [FK_CommittedProject_ScenarioBudget_ScenarioBudgetId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_CriterionLibrary_ScenarioBudget_ScenarioBudget_ScenarioBudgetId')
BEGIN
ALTER TABLE [dbo].[CriterionLibrary_ScenarioBudget] DROP CONSTRAINT [FK_CriterionLibrary_ScenarioBudget_ScenarioBudget_ScenarioBudgetId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_TreatmentLibrary_User_User_UserEntityId')
BEGIN
ALTER TABLE [dbo].[TreatmentLibrary_User] DROP CONSTRAINT [FK_TreatmentLibrary_User_User_UserEntityId];
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WITH(NOLOCK) WHERE CONSTRAINT_NAME = 'FK_TreatmentLibrary_User_User_UserId')
BEGIN
ALTER TABLE [dbo].[TreatmentLibrary_User] DROP CONSTRAINT [FK_TreatmentLibrary_User_User_UserId];
END
--9


--Update Indexes



	IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AttributeDatum_MaintainableAssetID') 
		DROP INDEX [IX_AttributeDatum_MaintainableAssetID] ON [dbo].[AttributeDatum]
	
	CREATE NONCLUSTERED INDEX [IX_AttributeDatum_MaintainableAssetID]
	ON [dbo].[AttributeDatum] ([MaintainableAssetId])
	INCLUDE ([Discriminator],[TimeStamp],[NumericValue],[TextValue],[AttributeId]
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	

-----------------------------------------


  IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AttributeDatum_AttributeId')   
      DROP INDEX IX_AttributeDatum_AttributeId ON [dbo].[AttributeDatum];   
    
  CREATE NonCLUSTERED INDEX [IX_AttributeDatum_AttributeId]
      ON [dbo].[AttributeDatum] ([AttributeId] ASC)
  INCLUDE ([Discriminator],[TimeStamp],[NumericValue],[TextValue],[MaintainableAssetId]
  	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
   

-----------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssetSummaryDetailValueIntId_AssetSummaryDetailId_AssetSummaryDetailId') 
	DROP INDEX [IX_AssetSummaryDetailValueIntId_AssetSummaryDetailId_AssetSummaryDetailId] ON [dbo].[AssetSummaryDetailValueIntId]


CREATE NONCLUSTERED INDEX [IX_AssetSummaryDetailValueIntId_AssetSummaryDetailId_AssetSummaryDetailId]
ON [dbo].[AssetSummaryDetailValueIntId] ([AssetSummaryDetailId])
INCLUDE ([Discriminator],[TextValue],[NumericValue],[AttributeId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_MaintainableAsset_NetworkId') 
	DROP INDEX [IX_MaintainableAsset_NetworkId] ON [dbo].[MaintainableAsset]


CREATE NONCLUSTERED INDEX [IX_MaintainableAsset_NetworkId] ON [dbo].[MaintainableAsset]
([NetworkId] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_BudgetUsageDetail_TreatmentConsiderationDetailId') 
	DROP INDEX [IX_BudgetUsageDetail_TreatmentConsiderationDetailId] ON [dbo].[BudgetUsageDetail]

CREATE NONCLUSTERED INDEX [IX_BudgetUsageDetail_TreatmentConsiderationDetailId] ON [dbo].[BudgetUsageDetail]
([TreatmentConsiderationDetailId] ASC) INCLUDE ([BudgetName],[CoveredCost],[Status]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TreatmentConsiderationDetail_AssetDetailId') 
	DROP INDEX [IX_TreatmentConsiderationDetail_AssetDetailId] ON [dbo].[TreatmentConsiderationDetail]

CREATE NONCLUSTERED INDEX [IX_TreatmentConsiderationDetail_AssetDetailId] ON [dbo].[TreatmentConsiderationDetail]
(
	[AssetDetailId] ASC
)
INCLUDE ([BudgetPriorityLevel],[TreatmentName]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TreatmentOptionDetail_AssetDetailId') 
DROP INDEX [IX_TreatmentOptionDetail_AssetDetailId] ON [dbo].[TreatmentOptionDetail]

CREATE NONCLUSTERED INDEX [IX_TreatmentOptionDetail_AssetDetailId] ON [dbo].[TreatmentOptionDetail]
(
	[AssetDetailId] ASC) INCLUDE ([Benefit],[Cost],[RemainingLife],[TreatmentName],[ConditionChange])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TreatmentRejectionDetail_AssetDetailId') 
DROP INDEX [IX_TreatmentRejectionDetail_AssetDetailId] ON [dbo].[TreatmentRejectionDetail]


CREATE NONCLUSTERED INDEX [IX_TreatmentRejectionDetail_AssetDetailId] ON [dbo].[TreatmentRejectionDetail]
(
	[AssetDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ScenarioBudget_SimulationId')
DROP INDEX [IX_ScenarioBudget_SimulationId] ON [dbo].[ScenarioBudget]


CREATE NONCLUSTERED INDEX [IX_ScenarioBudget_SimulationId] ON [dbo].[ScenarioBudget]
([SimulationId] ASC) INCLUDE([Name],[BudgetOrder],[LibraryId],[IsModified])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]



-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ScenarioBudgetAmount_ScenarioBudgetId')
DROP INDEX [IX_ScenarioBudgetAmount_ScenarioBudgetId] ON [dbo].[ScenarioBudgetAmount]

CREATE NONCLUSTERED INDEX [IX_ScenarioBudgetAmount_ScenarioBudgetId] ON [dbo].[ScenarioBudgetAmount]
(
	[ScenarioBudgetId] ASC) INCLUDE ([Year],[Value])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]



-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_BudgetPercentagePair_ScenarioBudgetPriorityId')
DROP INDEX [IX_BudgetPercentagePair_ScenarioBudgetPriorityId] ON [dbo].[BudgetPercentagePair]

CREATE NONCLUSTERED INDEX [IX_BudgetPercentagePair_ScenarioBudgetPriorityId] ON [dbo].[BudgetPercentagePair]
(	[ScenarioBudgetPriorityId] ASC) INCLUDE ([Percentage],[ScenarioBudgetId])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ScenarioCalculatedAttributePair_ScenarioCalculatedAttribute_ScenarioCalculatedAttributeId')
DROP INDEX [IX_ScenarioCalculatedAttributePair_ScenarioCalculatedAttribute_ScenarioCalculatedAttributeId] ON [dbo].[ScenarioCalculatedAttributePair]

CREATE NONCLUSTERED INDEX [IX_ScenarioCalculatedAttributePair_ScenarioCalculatedAttribute_ScenarioCalculatedAttributeId] ON [dbo].[ScenarioCalculatedAttributePair]
([ScenarioCalculatedAttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]



-----------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ScenarioPerformanceCurve_SimulationId')
DROP INDEX [IX_ScenarioPerformanceCurve_SimulationId] ON [dbo].[ScenarioPerformanceCurve]

CREATE NONCLUSTERED INDEX [IX_ScenarioPerformanceCurve_SimulationId] ON [dbo].[ScenarioPerformanceCurve]
(
	[SimulationId] ASC) INCLUDE ([AttributeId],[Name],[Shift],[LibraryId],[IsModified])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatmentId')
DROP INDEX [IX_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioConditionalTreatmentConsequences]


CREATE NONCLUSTERED INDEX [IX_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioConditionalTreatmentConsequences]
(
	[ScenarioSelectableTreatmentId] ASC) INCLUDE ([AttributeId],[ChangeValue])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ScenarioSelectableTreatment_SimulationId')
DROP INDEX [IX_ScenarioSelectableTreatment_SimulationId] ON [dbo].[ScenarioSelectableTreatment]

CREATE NONCLUSTERED INDEX [IX_ScenarioSelectableTreatment_SimulationId] ON [dbo].[ScenarioSelectableTreatment]
(
	[SimulationId] ASC) INCLUDE ([Description],[Name],[ShadowForAnyTreatment],[ShadowForSameTreatment],[AssetType],[Category],[IsModified],[LibraryId],[IsUnselectable])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]



-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_SimulationLog_SimulationId')
DROP INDEX [IX_SimulationLog_SimulationId] ON [dbo].[SimulationLog]

CREATE NONCLUSTERED INDEX [IX_SimulationLog_SimulationId] ON [dbo].[SimulationLog]
(
	[SimulationId] ASC) INCLUDE ([Status],[Subject],[Message])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]



-----------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_MaintainableAssetLocation_MaintainableAssetId')
DROP INDEX [IX_MaintainableAssetLocation_MaintainableAssetId] ON [dbo].[MaintainableAssetLocation]

CREATE UNIQUE NONCLUSTERED INDEX [IX_MaintainableAssetLocation_MaintainableAssetId] ON [dbo].[MaintainableAssetLocation]
(
	[MaintainableAssetId] ASC) INCLUDE ([Discriminator],[LocationIdentifier],[Start],[End],[Direction])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]



-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AttributeDatumLocation_AttributeDatumId')
DROP INDEX [IX_AttributeDatumLocation_AttributeDatumId] ON [dbo].[AttributeDatumLocation]

CREATE UNIQUE NONCLUSTERED INDEX [IX_AttributeDatumLocation_AttributeDatumId] ON [dbo].[AttributeDatumLocation]
(
	[AttributeDatumId] ASC ) INCLUDE ([Discriminator],[LocationIdentifier],[Start],[End],[Direction])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


--------------------------------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AssetSummaryDetail_MaintainableAssetId')
DROP INDEX [IX_AssetSummaryDetail_MaintainableAssetId] ON [dbo].[AssetSummaryDetail]

CREATE NONCLUSTERED INDEX [IX_AssetSummaryDetail_MaintainableAssetId] ON [dbo].[AssetSummaryDetail]
(
	[MaintainableAssetId] ASC ) INCLUDE ([SimulationOutputId])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------

IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CashFlowConsiderationDetail_TreatmentConsiderationDetailId')
DROP INDEX [IX_CashFlowConsiderationDetail_TreatmentConsiderationDetailId] ON [dbo].[CashFlowConsiderationDetail]

CREATE NONCLUSTERED INDEX [IX_CashFlowConsiderationDetail_TreatmentConsiderationDetailId] ON [dbo].[CashFlowConsiderationDetail]
(
	[TreatmentConsiderationDetailId] ASC ) INCLUDE ([CashFlowRuleName],[ReasonAgainstCashFlow])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_BudgetPercentagePair_ScenarioBudgetId')
DROP INDEX [IX_BudgetPercentagePair_ScenarioBudgetId] ON [dbo].[BudgetPercentagePair]

CREATE NONCLUSTERED INDEX [IX_BudgetPercentagePair_ScenarioBudgetId] ON [dbo].[BudgetPercentagePair]
(
	[ScenarioBudgetId] ASC ) INCLUDE ([Percentage],[ScenarioBudgetPriorityId])
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurveId')
DROP INDEX [IX_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurveId] ON [dbo].[CriterionLibrary_ScenarioPerformanceCurve]

CREATE UNIQUE NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurveId] ON [dbo].[CriterionLibrary_ScenarioPerformanceCurve]
(
	[ScenarioPerformanceCurveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------

 
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibraryId') 
DROP INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibraryId] ON [dbo].[CriterionLibrary_ScenarioTreatmentConsequence]


CREATE NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibraryId] ON [dbo].[CriterionLibrary_ScenarioTreatmentConsequence]
(
	[CriterionLibraryId] ASC
)  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


----------------------------------------------------------------------------------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequenceId')
DROP INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequenceId] ON [dbo].[CriterionLibrary_ScenarioTreatmentConsequence]

CREATE UNIQUE NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequenceId] ON [dbo].[CriterionLibrary_ScenarioTreatmentConsequence]
([ScenarioConditionalTreatmentConsequenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

---------------------------------------------------------


IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_ScenarioTreatmentCost_ScenarioSelectableTreatmentId')
DROP INDEX [IX_ScenarioTreatmentCost_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioTreatmentCost]

CREATE NONCLUSTERED INDEX [IX_ScenarioTreatmentCost_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioTreatmentCost]
([ScenarioSelectableTreatmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]




  IF EXISTS (SELECT name FROM sys.indexes  WHERE name = N'IX_TreatmentConsequence_SelectableTreatmentId')   
      DROP INDEX IX_TreatmentConsequence_SelectableTreatmentId ON [dbo].[TreatmentConsequence];   
    
  
  CREATE NonCLUSTERED INDEX [IX_TreatmentConsequence_SelectableTreatmentId]
      ON [dbo].TreatmentConsequence ([SelectableTreatmentId] ASC)
  	INCLUDE ([AttributeId],[ChangeValue]
  	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
  

  ----------------------------------------------------------------------------------------
  
  
  IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_AggregatedResult_AttributeId')   
      DROP INDEX IX_AggregatedResult_AttributeId ON [dbo].[AggregatedResult];   
    
  
  CREATE NonCLUSTERED INDEX [IX_AggregatedResult_AttributeId]  ON [dbo].[AggregatedResult] ([AttributeId] ASC)
  INCLUDE ([Discriminator],[Year],[TextValue],[NumericValue],[MaintainableAssetId]
  	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
   

  ----------------------------------------------------------------------------------------

  
  IF EXISTS (SELECT name FROM sys.indexes  WHERE name = N'IX_AggregatedResult_MaintainableAssetId')  
DROP INDEX [IX_AggregatedResult_MaintainableAssetId] ON [dbo].[AggregatedResult]

CREATE NONCLUSTERED INDEX [IX_AggregatedResult_MaintainableAssetId] ON [dbo].[AggregatedResult]
([MaintainableAssetId] ASC)
INCLUDE([Discriminator],[Year],[TextValue],[NumericValue],[AttributeId] ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


  ----------------------------------------------------------------------------------------

  
  IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_TreatmentConsequence_AttributeId')   
      DROP INDEX IX_TreatmentConsequence_AttributeId ON [dbo].[TreatmentConsequence];   
    
  
  CREATE NonCLUSTERED INDEX [IX_TreatmentConsequence_AttributeId]
      ON [dbo].[TreatmentConsequence] ([AttributeId] ASC)
  INCLUDE ([SelectableTreatmentId],[ChangeValue]
  	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
   

  ----------------------------------------------------------------------------------------
  
  
    IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_SimulationOutput_ChangeTracking')  
DROP INDEX [IX_SimulationOutput_ChangeTracking] ON [dbo].SimulationOutput


CREATE NONCLUSTERED INDEX IX_SimulationOutput_ChangeTracking ON dbo.SimulationOutput(Id)
INCLUDE(CreatedDate,LastModifiedDate,CreatedBy,LastModifiedBy) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


  ----------------------------------------------------------------------------------------
  
 
    IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatmentId')  
DROP INDEX [IX_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatmentId] ON [dbo].[CriterionLibrary_ScenarioTreatment]


CREATE  NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatmentId] ON [dbo].[CriterionLibrary_ScenarioTreatment]
(
	[ScenarioSelectableTreatmentId] ASC
)  INCLUDE( [CriterionLibraryId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


  ----------------------------------------------------------------------------------------
   
   
    IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_CriterionLibrary_ScenarioTreatment_CriterionLibraryId') 
DROP INDEX [IX_CriterionLibrary_ScenarioTreatment_CriterionLibraryId] ON [dbo].[CriterionLibrary_ScenarioTreatment]


CREATE NONCLUSTERED INDEX [IX_CriterionLibrary_ScenarioTreatment_CriterionLibraryId] ON [dbo].[CriterionLibrary_ScenarioTreatment]
(
	[CriterionLibraryId] ASC
)INCLUDE( [ScenarioSelectableTreatmentId])  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


  ----------------------------------------------------------------------------------------
  
  
    IF EXISTS (SELECT name FROM sys.indexes  WHERE name = N'IX_CriterionLibrary_Id') 
DROP INDEX [IX_CriterionLibrary_Id] ON [dbo].[CriterionLibrary]

CREATE NONCLUSTERED INDEX [IX_CriterionLibrary_Id] ON [dbo].[CriterionLibrary]
(
	[Id] ASC
)
INCLUDE([IsSingleUse],[IsShared]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-------------------------------------------------------------------------
--[ScenarioSelectableTreatment_ScenarioBudget] 

 
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudgetId') 
DROP INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudgetId] ON [dbo].[ScenarioSelectableTreatment_ScenarioBudget]


CREATE NONCLUSTERED INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudgetId] ON [dbo].[ScenarioSelectableTreatment_ScenarioBudget]
(
	[ScenarioBudgetId] ASC
) INCLUDE([ScenarioSelectableTreatmentId])   WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


------------------------------------------------------------------------------------------------
--[ScenarioSelectableTreatment_ScenarioBudget] 

  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatmentId') 
DROP INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioSelectableTreatment_ScenarioBudget]


CREATE NONCLUSTERED INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatmentId] ON [dbo].[ScenarioSelectableTreatment_ScenarioBudget]
(
	[ScenarioSelectableTreatmentId] ASC
) INCLUDE([ScenarioBudgetId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


----------------------------------------------------

  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_Equation_Expression') 
DROP INDEX [IX_Equation_Expression] ON [dbo].[Equation]


CREATE NONCLUSTERED INDEX [IX_Equation_Expression] ON [dbo].[Equation]
(
	[CreatedBy] ASC
) INCLUDE([LastModifiedBy])
 WITH  (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-----------------------------------------------------------------------------
  
  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_Attribute_DataSourceId') 
DROP INDEX [IX_Attribute_DataSourceId] ON [dbo].[Attribute]

CREATE NONCLUSTERED INDEX [IX_Attribute_DataSourceId] ON [dbo].[Attribute]
(
	[DataSourceId] ASC
) INCLUDE([Name],[DataType],[AggregationRuleType],[Command],[ConnectionType],[DefaultValue],[Minimum],[Maximum],[IsCalculated],[IsAscending]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-------------------------------------------------------------------------------------------------------------

  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_Attribute_Name') 
DROP INDEX [IX_Attribute_Name] ON [dbo].[Attribute]

CREATE  NONCLUSTERED INDEX [IX_Attribute_Name] ON [dbo].[Attribute]
(
	[Name] ASC
) INCLUDE([DataSourceId],[DataType],[AggregationRuleType],[Command],[ConnectionType],[DefaultValue],[Minimum],[Maximum],[IsCalculated],[IsAscending])  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


----------------------------------------------------------------------

  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_ScenarioTreatmentConsequence_Equation_EquationId') 
DROP INDEX [IX_ScenarioTreatmentConsequence_Equation_EquationId] ON [dbo].[ScenarioTreatmentConsequence_Equation]


CREATE  NONCLUSTERED INDEX [IX_ScenarioTreatmentConsequence_Equation_EquationId] ON [dbo].[ScenarioTreatmentConsequence_Equation]
(
	[EquationId] ASC
) INCLUDE([ScenarioConditionalTreatmentConsequenceId],[CreatedBy],[LastModifiedBy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


------------------------------------------------------------------------------------

  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequenceId') 
DROP INDEX [IX_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequenceId] ON [dbo].[ScenarioTreatmentConsequence_Equation]


CREATE  NONCLUSTERED INDEX [IX_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequenceId] ON [dbo].[ScenarioTreatmentConsequence_Equation]
(
	[ScenarioConditionalTreatmentConsequenceId] ASC
) INCLUDE([EquationId],[CreatedBy],[LastModifiedBy]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


----------------------------------------------------------------------------------------------------------------

  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_AssetSummaryDetail_SimulationOutput_SimulationOutputId') 
DROP INDEX [IX_AssetSummaryDetail_SimulationOutput_SimulationOutputId] ON [dbo].[AssetSummaryDetail]


CREATE NONCLUSTERED INDEX [IX_AssetSummaryDetail_SimulationOutput_SimulationOutputId] ON [dbo].[AssetSummaryDetail]
(
	[SimulationOutputId] ASC
)
INCLUDE([MaintainableAssetId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


----------------------------------------------------------------------------------------------------------------

  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_AssetDetail_MaintainableAsset_MaintainableAssetId') 
DROP INDEX [IX_AssetDetail_MaintainableAsset_MaintainableAssetId] ON [dbo].[AssetDetail]

CREATE NONCLUSTERED INDEX [IX_AssetDetail_MaintainableAsset_MaintainableAssetId] ON [dbo].[AssetDetail]
([MaintainableAssetId] ASC)
INCLUDE([SimulationYearDetailId],[AppliedTreatment],[TreatmentCause],[TreatmentFundingIgnoresSpendingLimit],[TreatmentStatus]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


----------------------------------------------------------------------------------------------------------------


  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_AssetDetail_SimulationYearDetail_SimulationYearDetailId') 
DROP INDEX [IX_AssetDetail_SimulationYearDetail_SimulationYearDetailId] ON [dbo].[AssetDetail]

CREATE NONCLUSTERED INDEX [IX_AssetDetail_SimulationYearDetail_SimulationYearDetailId] ON [dbo].[AssetDetail]
([SimulationYearDetailId] ASC)
INCLUDE([MaintainableAssetId],[AppliedTreatment],[TreatmentCause],[TreatmentFundingIgnoresSpendingLimit],[TreatmentStatus]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]



-------------------------------------------------------------------------------------------------------------------------------------------

  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_AssetDetailValueIntId_Attribute_AttributeId') 
DROP INDEX [IX_AssetDetailValueIntId_Attribute_AttributeId] ON [dbo].[AssetDetailValueIntId]

CREATE NONCLUSTERED INDEX [IX_AssetDetailValueIntId_Attribute_AttributeId] ON [dbo].[AssetDetailValueIntId]
([AttributeId] ASC)
INCLUDE([AssetDetailId],[Discriminator],[TextValue],[NumericValue]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


-------------------------------------------------------------------------------------------------------------------------------------------

  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_AssetDetailValueIntId_AssetDetail_AssetDetailId') 
DROP INDEX [IX_AssetDetailValueIntId_AssetDetail_AssetDetailId] ON [dbo].[AssetDetailValueIntId]

CREATE NONCLUSTERED INDEX [IX_AssetDetailValueIntId_AssetDetail_AssetDetailId] ON [dbo].[AssetDetailValueIntId]
([AssetDetailId])
INCLUDE ([Discriminator],[TextValue],[NumericValue],[AttributeId])  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]


----------------------------------------------------------------------------------------------------------------
 
  
    IF EXISTS (SELECT name FROM sys.indexes  
              WHERE name = N'IX_BudgetDetail_SimulationYearDetailId') 
DROP INDEX [IX_BudgetDetail_SimulationYearDetailId] ON [dbo].[BudgetDetail]

 CREATE NONCLUSTERED INDEX [IX_BudgetDetail_SimulationYearDetailId] ON [dbo].[BudgetDetail]
 ([SimulationYearDetailId] ASC)
 INCLUDE ([AvailableFunding],[BudgetName])WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
 

  ----------------------------------------------------------------------------------------


	END TRY
	BEGIN CATCH
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			RAISERROR  (@ErrorMessage, 16, 1);  
	END CATCH;

END;

-- EXEC usp_master_procedure;
