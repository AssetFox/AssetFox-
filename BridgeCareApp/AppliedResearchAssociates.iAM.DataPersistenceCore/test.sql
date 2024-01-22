IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Attribute] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [DataType] nvarchar(max) NOT NULL,
        [AggregationRuleType] nvarchar(max) NOT NULL,
        [Command] nvarchar(max) NOT NULL,
        [ConnectionType] int NOT NULL,
        [DefaultValue] nvarchar(max) NULL,
        [Minimum] float NULL,
        [Maximum] float NULL,
        [IsCalculated] bit NOT NULL,
        [IsAscending] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Attribute] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [BudgetLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_BudgetLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [BudgetPriorityLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_BudgetPriorityLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CashFlowRuleLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_CashFlowRuleLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [MergedCriteriaExpression] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_CriterionLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [DeficientConditionGoalLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_DeficientConditionGoalLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Equation] (
        [Id] uniqueidentifier NOT NULL,
        [Expression] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Equation] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Network] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Network] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [PerformanceCurveLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_PerformanceCurveLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [RemainingLifeLimitLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_RemainingLifeLimitLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TargetConditionGoalLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_TargetConditionGoalLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TreatmentLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_TreatmentLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [User] (
        [Id] uniqueidentifier NOT NULL,
        [Username] nvarchar(max) NULL,
        [HasInventoryAccess] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_User] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Budget] (
        [Id] uniqueidentifier NOT NULL,
        [BudgetLibraryId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Budget] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Budget_BudgetLibrary_BudgetLibraryId] FOREIGN KEY ([BudgetLibraryId]) REFERENCES [BudgetLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [BudgetPriority] (
        [Id] uniqueidentifier NOT NULL,
        [BudgetPriorityLibraryId] uniqueidentifier NOT NULL,
        [PriorityLevel] int NOT NULL,
        [Year] int NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BudgetPriority] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BudgetPriority_BudgetPriorityLibrary_BudgetPriorityLibraryId] FOREIGN KEY ([BudgetPriorityLibraryId]) REFERENCES [BudgetPriorityLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CashFlowRule] (
        [Id] uniqueidentifier NOT NULL,
        [CashFlowRuleLibraryId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CashFlowRule] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CashFlowRule_CashFlowRuleLibrary_CashFlowRuleLibraryId] FOREIGN KEY ([CashFlowRuleLibraryId]) REFERENCES [CashFlowRuleLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [DeficientConditionGoal] (
        [Id] uniqueidentifier NOT NULL,
        [DeficientConditionGoalLibraryId] uniqueidentifier NOT NULL,
        [AllowedDeficientPercentage] float NOT NULL,
        [DeficientLimit] float NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_DeficientConditionGoal] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeficientConditionGoal_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DeficientConditionGoal_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId] FOREIGN KEY ([DeficientConditionGoalLibraryId]) REFERENCES [DeficientConditionGoalLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Attribute_Equation_CriterionLibrary] (
        [AttributeId] uniqueidentifier NOT NULL,
        [EquationId] uniqueidentifier NOT NULL,
        [CriterionLibraryId] uniqueidentifier NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Attribute_Equation_CriterionLibrary] PRIMARY KEY ([AttributeId], [EquationId]),
        CONSTRAINT [FK_Attribute_Equation_CriterionLibrary_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Attribute_Equation_CriterionLibrary_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Attribute_Equation_CriterionLibrary_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [BenefitQuantifier] (
        [NetworkId] uniqueidentifier NOT NULL,
        [EquationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BenefitQuantifier] PRIMARY KEY ([NetworkId]),
        CONSTRAINT [FK_BenefitQuantifier_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BenefitQuantifier_Network_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [Network] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Facility] (
        [Id] uniqueidentifier NOT NULL,
        [NetworkId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Facility] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Facility_Network_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [Network] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [MaintainableAsset] (
        [Id] uniqueidentifier NOT NULL,
        [NetworkId] uniqueidentifier NOT NULL,
        [FacilityName] nvarchar(max) NULL,
        [SectionName] nvarchar(max) NULL,
        [SpatialWeighting] nvarchar(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_MaintainableAsset] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MaintainableAsset_Network_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [Network] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [NetworkRollupDetail] (
        [NetworkId] uniqueidentifier NOT NULL,
        [Status] nvarchar(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NetworkRollupDetail] PRIMARY KEY ([NetworkId]),
        CONSTRAINT [FK_NetworkRollupDetail_Network_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [Network] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Simulation] (
        [Id] uniqueidentifier NOT NULL,
        [NetworkId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [NumberOfYearsOfTreatmentOutlook] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Simulation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Simulation_Network_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [Network] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [PerformanceCurve] (
        [Id] uniqueidentifier NOT NULL,
        [PerformanceCurveLibraryId] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Shift] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PerformanceCurve] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PerformanceCurve_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PerformanceCurve_PerformanceCurveLibrary_PerformanceCurveLibraryId] FOREIGN KEY ([PerformanceCurveLibraryId]) REFERENCES [PerformanceCurveLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [RemainingLifeLimit] (
        [Id] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [RemainingLifeLimitLibraryId] uniqueidentifier NOT NULL,
        [Value] float NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RemainingLifeLimit] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RemainingLifeLimit_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RemainingLifeLimit_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId] FOREIGN KEY ([RemainingLifeLimitLibraryId]) REFERENCES [RemainingLifeLimitLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TargetConditionGoal] (
        [Id] uniqueidentifier NOT NULL,
        [TargetConditionGoalLibraryId] uniqueidentifier NOT NULL,
        [Target] float NOT NULL,
        [Year] int NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TargetConditionGoal] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TargetConditionGoal_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TargetConditionGoal_TargetConditionGoalLibrary_TargetConditionGoalLibraryId] FOREIGN KEY ([TargetConditionGoalLibraryId]) REFERENCES [TargetConditionGoalLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [SelectableTreatment] (
        [Id] uniqueidentifier NOT NULL,
        [Description] nvarchar(max) NULL,
        [TreatmentLibraryId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [ShadowForAnyTreatment] int NOT NULL,
        [ShadowForSameTreatment] int NOT NULL,
        CONSTRAINT [PK_SelectableTreatment] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SelectableTreatment_TreatmentLibrary_TreatmentLibraryId] FOREIGN KEY ([TreatmentLibraryId]) REFERENCES [TreatmentLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_User] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_User] PRIMARY KEY ([CriterionLibraryId], [UserId]),
        CONSTRAINT [FK_CriterionLibrary_User_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [UserCriteria_Filter] (
        [UserCriteriaId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Criteria] nvarchar(max) NULL,
        [HasCriteria] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_UserCriteria_Filter] PRIMARY KEY ([UserCriteriaId]),
        CONSTRAINT [FK_UserCriteria_Filter_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [BudgetAmount] (
        [Id] uniqueidentifier NOT NULL,
        [BudgetId] uniqueidentifier NOT NULL,
        [Year] int NOT NULL,
        [Value] decimal(18,2) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BudgetAmount] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BudgetAmount_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [Budget] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_Budget] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [BudgetId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_Budget] PRIMARY KEY ([CriterionLibraryId], [BudgetId]),
        CONSTRAINT [FK_CriterionLibrary_Budget_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [Budget] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_Budget_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [BudgetPercentagePair] (
        [Id] uniqueidentifier NOT NULL,
        [BudgetId] uniqueidentifier NOT NULL,
        [BudgetPriorityId] uniqueidentifier NOT NULL,
        [Percentage] decimal(18,2) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BudgetPercentagePair] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BudgetPercentagePair_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [Budget] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BudgetPercentagePair_BudgetPriority_BudgetPriorityId] FOREIGN KEY ([BudgetPriorityId]) REFERENCES [BudgetPriority] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_BudgetPriority] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [BudgetPriorityId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_BudgetPriority] PRIMARY KEY ([CriterionLibraryId], [BudgetPriorityId]),
        CONSTRAINT [FK_CriterionLibrary_BudgetPriority_BudgetPriority_BudgetPriorityId] FOREIGN KEY ([BudgetPriorityId]) REFERENCES [BudgetPriority] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_BudgetPriority_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CashFlowDistributionRule] (
        [Id] uniqueidentifier NOT NULL,
        [CashFlowRuleId] uniqueidentifier NOT NULL,
        [DurationInYears] int NOT NULL,
        [CostCeiling] decimal(18,2) NOT NULL,
        [YearlyPercentages] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CashFlowDistributionRule] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CashFlowDistributionRule_CashFlowRule_CashFlowRuleId] FOREIGN KEY ([CashFlowRuleId]) REFERENCES [CashFlowRule] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_CashFlowRule] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [CashFlowRuleId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_CashFlowRule] PRIMARY KEY ([CriterionLibraryId], [CashFlowRuleId]),
        CONSTRAINT [FK_CriterionLibrary_CashFlowRule_CashFlowRule_CashFlowRuleId] FOREIGN KEY ([CashFlowRuleId]) REFERENCES [CashFlowRule] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_CashFlowRule_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_DeficientConditionGoal] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [DeficientConditionGoalId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_DeficientConditionGoal] PRIMARY KEY ([CriterionLibraryId], [DeficientConditionGoalId]),
        CONSTRAINT [FK_CriterionLibrary_DeficientConditionGoal_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_DeficientConditionGoal_DeficientConditionGoal_DeficientConditionGoalId] FOREIGN KEY ([DeficientConditionGoalId]) REFERENCES [DeficientConditionGoal] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Section] (
        [Id] uniqueidentifier NOT NULL,
        [FacilityId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [SpatialWeightingId] uniqueidentifier NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Section] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Section_Equation_SpatialWeightingId] FOREIGN KEY ([SpatialWeightingId]) REFERENCES [Equation] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Section_Facility_FacilityId] FOREIGN KEY ([FacilityId]) REFERENCES [Facility] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [AggregatedResult] (
        [Id] uniqueidentifier NOT NULL,
        [Discriminator] nvarchar(max) NOT NULL,
        [Year] int NOT NULL,
        [TextValue] nvarchar(max) NULL,
        [NumericValue] float NULL,
        [MaintainableAssetId] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AggregatedResult] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AggregatedResult_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AggregatedResult_MaintainableAsset_MaintainableAssetId] FOREIGN KEY ([MaintainableAssetId]) REFERENCES [MaintainableAsset] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [AttributeDatum] (
        [Id] uniqueidentifier NOT NULL,
        [Discriminator] nvarchar(max) NOT NULL,
        [TimeStamp] datetime2 NOT NULL,
        [NumericValue] float NULL,
        [TextValue] nvarchar(max) NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [MaintainableAssetId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AttributeDatum] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttributeDatum_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AttributeDatum_MaintainableAsset_MaintainableAssetId] FOREIGN KEY ([MaintainableAssetId]) REFERENCES [MaintainableAsset] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [MaintainableAssetLocation] (
        [Id] uniqueidentifier NOT NULL,
        [MaintainableAssetId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Discriminator] nvarchar(max) NOT NULL,
        [LocationIdentifier] nvarchar(max) NOT NULL,
        [Start] float NULL,
        [End] float NULL,
        [Direction] int NULL,
        CONSTRAINT [PK_MaintainableAssetLocation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MaintainableAssetLocation_MaintainableAsset_MaintainableAssetId] FOREIGN KEY ([MaintainableAssetId]) REFERENCES [MaintainableAsset] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [AnalysisMethod] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NULL,
        [Description] nvarchar(max) NULL,
        [OptimizationStrategy] int NOT NULL,
        [SpendingStrategy] int NOT NULL,
        [ShouldApplyMultipleFeasibleCosts] bit NOT NULL,
        [ShouldDeteriorateDuringCashFlow] bit NOT NULL,
        [ShouldUseExtraFundsAcrossBudgets] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AnalysisMethod] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AnalysisMethod_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_AnalysisMethod_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [BudgetLibrary_Simulation] (
        [BudgetLibraryId] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BudgetLibrary_Simulation] PRIMARY KEY ([BudgetLibraryId], [SimulationId]),
        CONSTRAINT [FK_BudgetLibrary_Simulation_BudgetLibrary_BudgetLibraryId] FOREIGN KEY ([BudgetLibraryId]) REFERENCES [BudgetLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BudgetLibrary_Simulation_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [BudgetPriorityLibrary_Simulation] (
        [BudgetPriorityLibraryId] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BudgetPriorityLibrary_Simulation] PRIMARY KEY ([BudgetPriorityLibraryId], [SimulationId]),
        CONSTRAINT [FK_BudgetPriorityLibrary_Simulation_BudgetPriorityLibrary_BudgetPriorityLibraryId] FOREIGN KEY ([BudgetPriorityLibraryId]) REFERENCES [BudgetPriorityLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BudgetPriorityLibrary_Simulation_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CashFlowRuleLibrary_Simulation] (
        [CashFlowRuleLibraryId] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CashFlowRuleLibrary_Simulation] PRIMARY KEY ([CashFlowRuleLibraryId], [SimulationId]),
        CONSTRAINT [FK_CashFlowRuleLibrary_Simulation_CashFlowRuleLibrary_CashFlowRuleLibraryId] FOREIGN KEY ([CashFlowRuleLibraryId]) REFERENCES [CashFlowRuleLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CashFlowRuleLibrary_Simulation_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [DeficientConditionGoalLibrary_Simulation] (
        [DeficientConditionGoalLibraryId] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_DeficientConditionGoalLibrary_Simulation] PRIMARY KEY ([DeficientConditionGoalLibraryId], [SimulationId]),
        CONSTRAINT [FK_DeficientConditionGoalLibrary_Simulation_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId] FOREIGN KEY ([DeficientConditionGoalLibraryId]) REFERENCES [DeficientConditionGoalLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DeficientConditionGoalLibrary_Simulation_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [InvestmentPlan] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [FirstYearOfAnalysisPeriod] int NOT NULL,
        [InflationRatePercentage] float NOT NULL,
        [MinimumProjectCostLimit] decimal(18,2) NOT NULL,
        [NumberOfYearsInAnalysisPeriod] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_InvestmentPlan] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InvestmentPlan_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [PerformanceCurveLibrary_Simulation] (
        [PerformanceCurveLibraryId] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PerformanceCurveLibrary_Simulation] PRIMARY KEY ([PerformanceCurveLibraryId], [SimulationId]),
        CONSTRAINT [FK_PerformanceCurveLibrary_Simulation_PerformanceCurveLibrary_PerformanceCurveLibraryId] FOREIGN KEY ([PerformanceCurveLibraryId]) REFERENCES [PerformanceCurveLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PerformanceCurveLibrary_Simulation_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [RemainingLifeLimitLibrary_Simulation] (
        [RemainingLifeLimitLibraryId] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RemainingLifeLimitLibrary_Simulation] PRIMARY KEY ([RemainingLifeLimitLibraryId], [SimulationId]),
        CONSTRAINT [FK_RemainingLifeLimitLibrary_Simulation_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId] FOREIGN KEY ([RemainingLifeLimitLibraryId]) REFERENCES [RemainingLifeLimitLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RemainingLifeLimitLibrary_Simulation_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [ReportIndex] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationID] uniqueidentifier NULL,
        [ReportTypeName] nvarchar(max) NOT NULL,
        [Result] nvarchar(max) NULL,
        [ExpirationDate] datetime2 NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ReportIndex] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ReportIndex_Simulation_SimulationID] FOREIGN KEY ([SimulationID]) REFERENCES [Simulation] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Simulation_User] (
        [SimulationId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [CanModify] bit NOT NULL,
        [IsOwner] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Simulation_User] PRIMARY KEY ([SimulationId], [UserId]),
        CONSTRAINT [FK_Simulation_User_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Simulation_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [SimulationAnalysisDetail] (
        [SimulationId] uniqueidentifier NOT NULL,
        [LastRun] datetime2 NOT NULL,
        [Status] nvarchar(max) NULL,
        [RunTime] nvarchar(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SimulationAnalysisDetail] PRIMARY KEY ([SimulationId]),
        CONSTRAINT [FK_SimulationAnalysisDetail_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [SimulationOutput] (
        [SimulationId] uniqueidentifier NOT NULL,
        [Output] nvarchar(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SimulationOutput] PRIMARY KEY ([SimulationId]),
        CONSTRAINT [FK_SimulationOutput_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [SimulationReportDetail] (
        [SimulationId] uniqueidentifier NOT NULL,
        [Status] nvarchar(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SimulationReportDetail] PRIMARY KEY ([SimulationId]),
        CONSTRAINT [FK_SimulationReportDetail_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TargetConditionGoalLibrary_Simulation] (
        [TargetConditionGoalLibraryId] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TargetConditionGoalLibrary_Simulation] PRIMARY KEY ([TargetConditionGoalLibraryId], [SimulationId]),
        CONSTRAINT [FK_TargetConditionGoalLibrary_Simulation_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TargetConditionGoalLibrary_Simulation_TargetConditionGoalLibrary_TargetConditionGoalLibraryId] FOREIGN KEY ([TargetConditionGoalLibraryId]) REFERENCES [TargetConditionGoalLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TreatmentLibrary_Simulation] (
        [TreatmentLibraryId] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TreatmentLibrary_Simulation] PRIMARY KEY ([TreatmentLibraryId], [SimulationId]),
        CONSTRAINT [FK_TreatmentLibrary_Simulation_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TreatmentLibrary_Simulation_TreatmentLibrary_TreatmentLibraryId] FOREIGN KEY ([TreatmentLibraryId]) REFERENCES [TreatmentLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_PerformanceCurve] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [PerformanceCurveId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_PerformanceCurve] PRIMARY KEY ([CriterionLibraryId], [PerformanceCurveId]),
        CONSTRAINT [FK_CriterionLibrary_PerformanceCurve_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_PerformanceCurve_PerformanceCurve_PerformanceCurveId] FOREIGN KEY ([PerformanceCurveId]) REFERENCES [PerformanceCurve] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [PerformanceCurve_Equation] (
        [PerformanceCurveId] uniqueidentifier NOT NULL,
        [EquationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PerformanceCurve_Equation] PRIMARY KEY ([PerformanceCurveId], [EquationId]),
        CONSTRAINT [FK_PerformanceCurve_Equation_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PerformanceCurve_Equation_PerformanceCurve_PerformanceCurveId] FOREIGN KEY ([PerformanceCurveId]) REFERENCES [PerformanceCurve] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_RemainingLifeLimit] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [RemainingLifeLimitId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_RemainingLifeLimit] PRIMARY KEY ([CriterionLibraryId], [RemainingLifeLimitId]),
        CONSTRAINT [FK_CriterionLibrary_RemainingLifeLimit_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_RemainingLifeLimit_RemainingLifeLimit_RemainingLifeLimitId] FOREIGN KEY ([RemainingLifeLimitId]) REFERENCES [RemainingLifeLimit] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_TargetConditionGoal] (
        [TargetConditionGoalId] uniqueidentifier NOT NULL,
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_TargetConditionGoal] PRIMARY KEY ([CriterionLibraryId], [TargetConditionGoalId]),
        CONSTRAINT [FK_CriterionLibrary_TargetConditionGoal_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_TargetConditionGoal_TargetConditionGoal_TargetConditionGoalId] FOREIGN KEY ([TargetConditionGoalId]) REFERENCES [TargetConditionGoal] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_Treatment] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [SelectableTreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_Treatment] PRIMARY KEY ([CriterionLibraryId], [SelectableTreatmentId]),
        CONSTRAINT [FK_CriterionLibrary_Treatment_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_Treatment_SelectableTreatment_SelectableTreatmentId] FOREIGN KEY ([SelectableTreatmentId]) REFERENCES [SelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Treatment_Budget] (
        [SelectableTreatmentId] uniqueidentifier NOT NULL,
        [BudgetId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Treatment_Budget] PRIMARY KEY ([SelectableTreatmentId], [BudgetId]),
        CONSTRAINT [FK_Treatment_Budget_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [Budget] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Treatment_Budget_SelectableTreatment_SelectableTreatmentId] FOREIGN KEY ([SelectableTreatmentId]) REFERENCES [SelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TreatmentConsequence] (
        [Id] uniqueidentifier NOT NULL,
        [SelectableTreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [ChangeValue] nvarchar(max) NULL,
        CONSTRAINT [PK_TreatmentConsequence] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentConsequence_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TreatmentConsequence_SelectableTreatment_SelectableTreatmentId] FOREIGN KEY ([SelectableTreatmentId]) REFERENCES [SelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TreatmentCost] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TreatmentCost] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentCost_SelectableTreatment_TreatmentId] FOREIGN KEY ([TreatmentId]) REFERENCES [SelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TreatmentScheduling] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [OffsetToFutureYear] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TreatmentScheduling] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentScheduling_SelectableTreatment_TreatmentId] FOREIGN KEY ([TreatmentId]) REFERENCES [SelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TreatmentSupersession] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TreatmentSupersession] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentSupersession_SelectableTreatment_TreatmentId] FOREIGN KEY ([TreatmentId]) REFERENCES [SelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CommittedProject] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [BudgetId] uniqueidentifier NOT NULL,
        [MaintainableAssetId] uniqueidentifier NOT NULL,
        [Cost] float NOT NULL,
        [Year] int NOT NULL,
        [SectionEntityId] uniqueidentifier NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [ShadowForAnyTreatment] int NOT NULL,
        [ShadowForSameTreatment] int NOT NULL,
        CONSTRAINT [PK_CommittedProject] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CommittedProject_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [Budget] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CommittedProject_MaintainableAsset_MaintainableAssetId] FOREIGN KEY ([MaintainableAssetId]) REFERENCES [MaintainableAsset] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CommittedProject_Section_SectionEntityId] FOREIGN KEY ([SectionEntityId]) REFERENCES [Section] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CommittedProject_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [NumericAttributeValueHistory] (
        [Id] uniqueidentifier NOT NULL,
        [Year] int NOT NULL,
        [Value] float NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NumericAttributeValueHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NumericAttributeValueHistory_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_NumericAttributeValueHistory_Section_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Section] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TextAttributeValueHistory] (
        [Id] uniqueidentifier NOT NULL,
        [Year] int NOT NULL,
        [Value] nvarchar(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [SectionId] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TextAttributeValueHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TextAttributeValueHistory_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TextAttributeValueHistory_Section_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Section] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [AttributeDatumLocation] (
        [Id] uniqueidentifier NOT NULL,
        [AttributeDatumId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Discriminator] nvarchar(max) NOT NULL,
        [LocationIdentifier] nvarchar(max) NOT NULL,
        [Start] float NULL,
        [End] float NULL,
        [Direction] int NULL,
        CONSTRAINT [PK_AttributeDatumLocation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AttributeDatumLocation_AttributeDatum_AttributeDatumId] FOREIGN KEY ([AttributeDatumId]) REFERENCES [AttributeDatum] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [Benefit] (
        [Id] uniqueidentifier NOT NULL,
        [AnalysisMethodId] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [Limit] float NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Benefit] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Benefit_AnalysisMethod_AnalysisMethodId] FOREIGN KEY ([AnalysisMethodId]) REFERENCES [AnalysisMethod] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Benefit_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_AnalysisMethod] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [AnalysisMethodId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_AnalysisMethod] PRIMARY KEY ([CriterionLibraryId], [AnalysisMethodId]),
        CONSTRAINT [FK_CriterionLibrary_AnalysisMethod_AnalysisMethod_AnalysisMethodId] FOREIGN KEY ([AnalysisMethodId]) REFERENCES [AnalysisMethod] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_AnalysisMethod_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_TreatmentConsequence] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ConditionalTreatmentConsequenceId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_TreatmentConsequence] PRIMARY KEY ([CriterionLibraryId], [ConditionalTreatmentConsequenceId]),
        CONSTRAINT [FK_CriterionLibrary_TreatmentConsequence_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_TreatmentConsequence_TreatmentConsequence_ConditionalTreatmentConsequenceId] FOREIGN KEY ([ConditionalTreatmentConsequenceId]) REFERENCES [TreatmentConsequence] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TreatmentConsequence_Equation] (
        [ConditionalTreatmentConsequenceId] uniqueidentifier NOT NULL,
        [EquationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TreatmentConsequence_Equation] PRIMARY KEY ([ConditionalTreatmentConsequenceId], [EquationId]),
        CONSTRAINT [FK_TreatmentConsequence_Equation_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TreatmentConsequence_Equation_TreatmentConsequence_ConditionalTreatmentConsequenceId] FOREIGN KEY ([ConditionalTreatmentConsequenceId]) REFERENCES [TreatmentConsequence] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_TreatmentCost] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [TreatmentCostId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_TreatmentCost] PRIMARY KEY ([CriterionLibraryId], [TreatmentCostId]),
        CONSTRAINT [FK_CriterionLibrary_TreatmentCost_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_TreatmentCost_TreatmentCost_TreatmentCostId] FOREIGN KEY ([TreatmentCostId]) REFERENCES [TreatmentCost] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [TreatmentCost_Equation] (
        [TreatmentCostId] uniqueidentifier NOT NULL,
        [EquationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TreatmentCost_Equation] PRIMARY KEY ([TreatmentCostId], [EquationId]),
        CONSTRAINT [FK_TreatmentCost_Equation_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TreatmentCost_Equation_TreatmentCost_TreatmentCostId] FOREIGN KEY ([TreatmentCostId]) REFERENCES [TreatmentCost] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CriterionLibrary_TreatmentSupersession] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [TreatmentSupersessionId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_TreatmentSupersession] PRIMARY KEY ([CriterionLibraryId], [TreatmentSupersessionId]),
        CONSTRAINT [FK_CriterionLibrary_TreatmentSupersession_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_TreatmentSupersession_TreatmentSupersession_TreatmentSupersessionId] FOREIGN KEY ([TreatmentSupersessionId]) REFERENCES [TreatmentSupersession] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE TABLE [CommittedProjectConsequence] (
        [Id] uniqueidentifier NOT NULL,
        [CommittedProjectId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [ChangeValue] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_CommittedProjectConsequence] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CommittedProjectConsequence_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CommittedProjectConsequence_CommittedProject_CommittedProjectId] FOREIGN KEY ([CommittedProjectId]) REFERENCES [CommittedProject] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_AggregatedResult_AttributeId] ON [AggregatedResult] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_AggregatedResult_MaintainableAssetId] ON [AggregatedResult] ([MaintainableAssetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_AnalysisMethod_AttributeId] ON [AnalysisMethod] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_AnalysisMethod_SimulationId] ON [AnalysisMethod] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_Attribute_Name] ON [Attribute] ([Name]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Attribute_Equation_CriterionLibrary_AttributeId] ON [Attribute_Equation_CriterionLibrary] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Attribute_Equation_CriterionLibrary_CriterionLibraryId] ON [Attribute_Equation_CriterionLibrary] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_Attribute_Equation_CriterionLibrary_EquationId] ON [Attribute_Equation_CriterionLibrary] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_AttributeDatum_AttributeId] ON [AttributeDatum] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_AttributeDatum_MaintainableAssetId] ON [AttributeDatum] ([MaintainableAssetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_AttributeDatumLocation_AttributeDatumId] ON [AttributeDatumLocation] ([AttributeDatumId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_Benefit_AnalysisMethodId] ON [Benefit] ([AnalysisMethodId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Benefit_AttributeId] ON [Benefit] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_BenefitQuantifier_EquationId] ON [BenefitQuantifier] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_BenefitQuantifier_NetworkId] ON [BenefitQuantifier] ([NetworkId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Budget_BudgetLibraryId] ON [Budget] ([BudgetLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_BudgetAmount_BudgetId] ON [BudgetAmount] ([BudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_BudgetLibrary_Simulation_BudgetLibraryId] ON [BudgetLibrary_Simulation] ([BudgetLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_BudgetLibrary_Simulation_SimulationId] ON [BudgetLibrary_Simulation] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_BudgetPercentagePair_BudgetId] ON [BudgetPercentagePair] ([BudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_BudgetPercentagePair_BudgetPriorityId] ON [BudgetPercentagePair] ([BudgetPriorityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_BudgetPriority_BudgetPriorityLibraryId] ON [BudgetPriority] ([BudgetPriorityLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_BudgetPriorityLibrary_Simulation_BudgetPriorityLibraryId] ON [BudgetPriorityLibrary_Simulation] ([BudgetPriorityLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_BudgetPriorityLibrary_Simulation_SimulationId] ON [BudgetPriorityLibrary_Simulation] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CashFlowDistributionRule_CashFlowRuleId] ON [CashFlowDistributionRule] ([CashFlowRuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CashFlowRule_CashFlowRuleLibraryId] ON [CashFlowRule] ([CashFlowRuleLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CashFlowRuleLibrary_Simulation_CashFlowRuleLibraryId] ON [CashFlowRuleLibrary_Simulation] ([CashFlowRuleLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CashFlowRuleLibrary_Simulation_SimulationId] ON [CashFlowRuleLibrary_Simulation] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CommittedProject_BudgetId] ON [CommittedProject] ([BudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CommittedProject_MaintainableAssetId] ON [CommittedProject] ([MaintainableAssetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CommittedProject_SectionEntityId] ON [CommittedProject] ([SectionEntityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CommittedProject_SimulationId] ON [CommittedProject] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CommittedProjectConsequence_AttributeId] ON [CommittedProjectConsequence] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CommittedProjectConsequence_CommittedProjectId] ON [CommittedProjectConsequence] ([CommittedProjectId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_AnalysisMethod_AnalysisMethodId] ON [CriterionLibrary_AnalysisMethod] ([AnalysisMethodId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_AnalysisMethod_CriterionLibraryId] ON [CriterionLibrary_AnalysisMethod] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_Budget_BudgetId] ON [CriterionLibrary_Budget] ([BudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_Budget_CriterionLibraryId] ON [CriterionLibrary_Budget] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_BudgetPriority_BudgetPriorityId] ON [CriterionLibrary_BudgetPriority] ([BudgetPriorityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_BudgetPriority_CriterionLibraryId] ON [CriterionLibrary_BudgetPriority] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_CashFlowRule_CashFlowRuleId] ON [CriterionLibrary_CashFlowRule] ([CashFlowRuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_CashFlowRule_CriterionLibraryId] ON [CriterionLibrary_CashFlowRule] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_DeficientConditionGoal_CriterionLibraryId] ON [CriterionLibrary_DeficientConditionGoal] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_DeficientConditionGoal_DeficientConditionGoalId] ON [CriterionLibrary_DeficientConditionGoal] ([DeficientConditionGoalId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_PerformanceCurve_CriterionLibraryId] ON [CriterionLibrary_PerformanceCurve] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_PerformanceCurve_PerformanceCurveId] ON [CriterionLibrary_PerformanceCurve] ([PerformanceCurveId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_RemainingLifeLimit_CriterionLibraryId] ON [CriterionLibrary_RemainingLifeLimit] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_RemainingLifeLimit_RemainingLifeLimitId] ON [CriterionLibrary_RemainingLifeLimit] ([RemainingLifeLimitId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_TargetConditionGoal_CriterionLibraryId] ON [CriterionLibrary_TargetConditionGoal] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_TargetConditionGoal_TargetConditionGoalId] ON [CriterionLibrary_TargetConditionGoal] ([TargetConditionGoalId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_Treatment_CriterionLibraryId] ON [CriterionLibrary_Treatment] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_Treatment_SelectableTreatmentId] ON [CriterionLibrary_Treatment] ([SelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_TreatmentConsequence_ConditionalTreatmentConsequenceId] ON [CriterionLibrary_TreatmentConsequence] ([ConditionalTreatmentConsequenceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_TreatmentConsequence_CriterionLibraryId] ON [CriterionLibrary_TreatmentConsequence] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_TreatmentCost_CriterionLibraryId] ON [CriterionLibrary_TreatmentCost] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_TreatmentCost_TreatmentCostId] ON [CriterionLibrary_TreatmentCost] ([TreatmentCostId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_TreatmentSupersession_CriterionLibraryId] ON [CriterionLibrary_TreatmentSupersession] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_TreatmentSupersession_TreatmentSupersessionId] ON [CriterionLibrary_TreatmentSupersession] ([TreatmentSupersessionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_User_CriterionLibraryId] ON [CriterionLibrary_User] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_User_UserId] ON [CriterionLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_DeficientConditionGoal_AttributeId] ON [DeficientConditionGoal] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_DeficientConditionGoal_DeficientConditionGoalLibraryId] ON [DeficientConditionGoal] ([DeficientConditionGoalLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_DeficientConditionGoalLibrary_Simulation_DeficientConditionGoalLibraryId] ON [DeficientConditionGoalLibrary_Simulation] ([DeficientConditionGoalLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_DeficientConditionGoalLibrary_Simulation_SimulationId] ON [DeficientConditionGoalLibrary_Simulation] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Facility_NetworkId] ON [Facility] ([NetworkId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_InvestmentPlan_SimulationId] ON [InvestmentPlan] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_MaintainableAsset_NetworkId] ON [MaintainableAsset] ([NetworkId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_MaintainableAssetLocation_MaintainableAssetId] ON [MaintainableAssetLocation] ([MaintainableAssetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_NetworkRollupDetail_NetworkId] ON [NetworkRollupDetail] ([NetworkId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_NumericAttributeValueHistory_AttributeId] ON [NumericAttributeValueHistory] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_NumericAttributeValueHistory_SectionId] ON [NumericAttributeValueHistory] ([SectionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_PerformanceCurve_AttributeId] ON [PerformanceCurve] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_PerformanceCurve_PerformanceCurveLibraryId] ON [PerformanceCurve] ([PerformanceCurveLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_PerformanceCurve_Equation_EquationId] ON [PerformanceCurve_Equation] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_PerformanceCurve_Equation_PerformanceCurveId] ON [PerformanceCurve_Equation] ([PerformanceCurveId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_PerformanceCurveLibrary_Simulation_PerformanceCurveLibraryId] ON [PerformanceCurveLibrary_Simulation] ([PerformanceCurveLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_PerformanceCurveLibrary_Simulation_SimulationId] ON [PerformanceCurveLibrary_Simulation] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_RemainingLifeLimit_AttributeId] ON [RemainingLifeLimit] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_RemainingLifeLimit_RemainingLifeLimitLibraryId] ON [RemainingLifeLimit] ([RemainingLifeLimitLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_RemainingLifeLimitLibrary_Simulation_RemainingLifeLimitLibraryId] ON [RemainingLifeLimitLibrary_Simulation] ([RemainingLifeLimitLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_RemainingLifeLimitLibrary_Simulation_SimulationId] ON [RemainingLifeLimitLibrary_Simulation] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_ReportIndex_SimulationID] ON [ReportIndex] ([SimulationID]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Section_FacilityId] ON [Section] ([FacilityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Section_SpatialWeightingId] ON [Section] ([SpatialWeightingId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_SelectableTreatment_TreatmentLibraryId] ON [SelectableTreatment] ([TreatmentLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Simulation_NetworkId] ON [Simulation] ([NetworkId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Simulation_User_SimulationId] ON [Simulation_User] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Simulation_User_UserId] ON [Simulation_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_SimulationAnalysisDetail_SimulationId] ON [SimulationAnalysisDetail] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_SimulationOutput_SimulationId] ON [SimulationOutput] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_SimulationReportDetail_SimulationId] ON [SimulationReportDetail] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TargetConditionGoal_AttributeId] ON [TargetConditionGoal] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TargetConditionGoal_TargetConditionGoalLibraryId] ON [TargetConditionGoal] ([TargetConditionGoalLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_TargetConditionGoalLibrary_Simulation_SimulationId] ON [TargetConditionGoalLibrary_Simulation] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TargetConditionGoalLibrary_Simulation_TargetConditionGoalLibraryId] ON [TargetConditionGoalLibrary_Simulation] ([TargetConditionGoalLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TextAttributeValueHistory_AttributeId] ON [TextAttributeValueHistory] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TextAttributeValueHistory_SectionId] ON [TextAttributeValueHistory] ([SectionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Treatment_Budget_BudgetId] ON [Treatment_Budget] ([BudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_Treatment_Budget_SelectableTreatmentId] ON [Treatment_Budget] ([SelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TreatmentConsequence_AttributeId] ON [TreatmentConsequence] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TreatmentConsequence_SelectableTreatmentId] ON [TreatmentConsequence] ([SelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_TreatmentConsequence_Equation_ConditionalTreatmentConsequenceId] ON [TreatmentConsequence_Equation] ([ConditionalTreatmentConsequenceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_TreatmentConsequence_Equation_EquationId] ON [TreatmentConsequence_Equation] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TreatmentCost_TreatmentId] ON [TreatmentCost] ([TreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_TreatmentCost_Equation_EquationId] ON [TreatmentCost_Equation] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_TreatmentCost_Equation_TreatmentCostId] ON [TreatmentCost_Equation] ([TreatmentCostId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_TreatmentLibrary_Simulation_SimulationId] ON [TreatmentLibrary_Simulation] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TreatmentLibrary_Simulation_TreatmentLibraryId] ON [TreatmentLibrary_Simulation] ([TreatmentLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TreatmentScheduling_TreatmentId] ON [TreatmentScheduling] ([TreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE INDEX [IX_TreatmentSupersession_TreatmentId] ON [TreatmentSupersession] ([TreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_UserCriteria_Filter_UserCriteriaId] ON [UserCriteria_Filter] ([UserCriteriaId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    CREATE UNIQUE INDEX [IX_UserCriteria_Filter_UserId] ON [UserCriteria_Filter] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210407224349_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210407224349_InitialCreate', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210514210021_AddAnnouncements')
BEGIN
    CREATE TABLE [Announcement] (
        [Id] uniqueidentifier NOT NULL,
        [Title] nvarchar(max) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Announcement] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210514210021_AddAnnouncements')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210514210021_AddAnnouncements', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210526200731_SimulationLog')
BEGIN
    CREATE TABLE [SimulationLog] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [Status] int NOT NULL,
        [Subject] int NOT NULL,
        [Message] nvarchar(max) NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SimulationLog] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SimulationLog_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210526200731_SimulationLog')
BEGIN
    CREATE INDEX [IX_SimulationLog_SimulationId] ON [SimulationLog] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210526200731_SimulationLog')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210526200731_SimulationLog', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210607193722_fromMerge')
BEGIN
    ALTER TABLE [CriterionLibrary] ADD [IsSingleUse] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210607193722_fromMerge')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210607193722_fromMerge', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    CREATE TABLE [ScenarioPerformanceCurve] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Shift] bit NOT NULL,
        CONSTRAINT [PK_ScenarioPerformanceCurve] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioPerformanceCurve_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioPerformanceCurve_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioPerformanceCurve] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioPerformanceCurveId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioPerformanceCurve] PRIMARY KEY ([CriterionLibraryId], [ScenarioPerformanceCurveId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioPerformanceCurve_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurve_ScenarioPerformanceCurveId] FOREIGN KEY ([ScenarioPerformanceCurveId]) REFERENCES [ScenarioPerformanceCurve] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    CREATE TABLE [ScenarioPerformanceCurve_Equation] (
        [EquationId] uniqueidentifier NOT NULL,
        [ScenarioPerformanceCurveId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioPerformanceCurve_Equation] PRIMARY KEY ([ScenarioPerformanceCurveId], [EquationId]),
        CONSTRAINT [FK_ScenarioPerformanceCurve_Equation_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioPerformanceCurve_Equation_ScenarioPerformanceCurve_ScenarioPerformanceCurveId] FOREIGN KEY ([ScenarioPerformanceCurveId]) REFERENCES [ScenarioPerformanceCurve] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioPerformanceCurve_CriterionLibraryId] ON [CriterionLibrary_ScenarioPerformanceCurve] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioPerformanceCurve_ScenarioPerformanceCurveId] ON [CriterionLibrary_ScenarioPerformanceCurve] ([ScenarioPerformanceCurveId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    CREATE INDEX [IX_ScenarioPerformanceCurve_AttributeId] ON [ScenarioPerformanceCurve] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    CREATE INDEX [IX_ScenarioPerformanceCurve_SimulationId] ON [ScenarioPerformanceCurve] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    CREATE UNIQUE INDEX [IX_ScenarioPerformanceCurve_Equation_EquationId] ON [ScenarioPerformanceCurve_Equation] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    CREATE UNIQUE INDEX [IX_ScenarioPerformanceCurve_Equation_ScenarioPerformanceCurveId] ON [ScenarioPerformanceCurve_Equation] ([ScenarioPerformanceCurveId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210728223214_AddScenarioPerformanceCurveTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210728223214_AddScenarioPerformanceCurveTables', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210802212629_RemovePerformanceCurveLibrarySimulationJoinTbl')
BEGIN
    DROP TABLE [PerformanceCurveLibrary_Simulation];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210802212629_RemovePerformanceCurveLibrarySimulationJoinTbl')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210802212629_RemovePerformanceCurveLibrarySimulationJoinTbl', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    DROP TABLE [TreatmentLibrary_Simulation];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    ALTER TABLE [TreatmentSupersession] ADD [CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    ALTER TABLE [TreatmentSupersession] ADD [CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [ScenarioSelectableTreatment] (
        [Id] uniqueidentifier NOT NULL,
        [Description] nvarchar(max) NULL,
        [ScenarioTreatmentId] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [ShadowForAnyTreatment] int NOT NULL,
        [ShadowForSameTreatment] int NOT NULL,
        CONSTRAINT [PK_ScenarioSelectableTreatment] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioSelectableTreatment_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioTreatment] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioSelectableTreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioTreatment] PRIMARY KEY ([CriterionLibraryId], [ScenarioSelectableTreatmentId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatment_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId] FOREIGN KEY ([ScenarioSelectableTreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [ScenarioConditionalTreatmentConsequences] (
        [Id] uniqueidentifier NOT NULL,
        [ScenarioSelectableTreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [ChangeValue] nvarchar(max) NULL,
        CONSTRAINT [PK_ScenarioConditionalTreatmentConsequences] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioConditionalTreatmentConsequences_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId] FOREIGN KEY ([ScenarioSelectableTreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [ScenarioTreatment_Budget] (
        [ScenarioSelectableTreatmentId] uniqueidentifier NOT NULL,
        [BudgetId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatment_Budget] PRIMARY KEY ([ScenarioSelectableTreatmentId], [BudgetId]),
        CONSTRAINT [FK_ScenarioTreatment_Budget_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [Budget] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioTreatment_Budget_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId] FOREIGN KEY ([ScenarioSelectableTreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [ScenarioTreatmentCost] (
        [Id] uniqueidentifier NOT NULL,
        [ScenarioTreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatmentCost] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioTreatmentCost_ScenarioSelectableTreatment_ScenarioTreatmentId] FOREIGN KEY ([ScenarioTreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [ScenarioTreatmentSchedulingEntity] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [OffsetToFutureYear] int NOT NULL,
        [ScenarioSelectableTreatmentId] uniqueidentifier NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatmentSchedulingEntity] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioTreatmentSchedulingEntity_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId] FOREIGN KEY ([ScenarioSelectableTreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [ScenarioTreatmentSupersessionEntity] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [ScenarioSelectableTreatmentId] uniqueidentifier NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatmentSupersessionEntity] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioTreatmentSupersessionEntity_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId] FOREIGN KEY ([ScenarioSelectableTreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioTreatmentConsequence] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioConditionalTreatmentConsequenceId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioTreatmentConsequence] PRIMARY KEY ([CriterionLibraryId], [ScenarioConditionalTreatmentConsequenceId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequences_ScenarioConditionalTreatmentConsequen~] FOREIGN KEY ([ScenarioConditionalTreatmentConsequenceId]) REFERENCES [ScenarioConditionalTreatmentConsequences] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [ScenarioTreatmentConsequence_Equation] (
        [EquationId] uniqueidentifier NOT NULL,
        [ScenarioConditionalTreatmentConsequenceId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatmentConsequence_Equation] PRIMARY KEY ([ScenarioConditionalTreatmentConsequenceId], [EquationId]),
        CONSTRAINT [FK_ScenarioTreatmentConsequence_Equation_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequences_ScenarioConditionalTreatmentConsequenceId] FOREIGN KEY ([ScenarioConditionalTreatmentConsequenceId]) REFERENCES [ScenarioConditionalTreatmentConsequences] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioTreatmentCost] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioTreatmentCostId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioTreatmentCost] PRIMARY KEY ([CriterionLibraryId], [ScenarioTreatmentCostId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentCost_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentCost_ScenarioTreatmentCost_ScenarioTreatmentCostId] FOREIGN KEY ([ScenarioTreatmentCostId]) REFERENCES [ScenarioTreatmentCost] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [ScenarioTreatmentCost_Equation] (
        [EquationId] uniqueidentifier NOT NULL,
        [ScenarioTreatmentCostId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatmentCost_Equation] PRIMARY KEY ([ScenarioTreatmentCostId], [EquationId]),
        CONSTRAINT [FK_ScenarioTreatmentCost_Equation_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioTreatmentCost_Equation_ScenarioTreatmentCost_ScenarioTreatmentCostId] FOREIGN KEY ([ScenarioTreatmentCostId]) REFERENCES [ScenarioTreatmentCost] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioTreatmentSupersession] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [TreatmentSupersessionId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioTreatmentSupersession] PRIMARY KEY ([CriterionLibraryId], [TreatmentSupersessionId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentSupersession_ScenarioTreatmentSupersessionEntity_TreatmentSupersessionId] FOREIGN KEY ([TreatmentSupersessionId]) REFERENCES [ScenarioTreatmentSupersessionEntity] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_TreatmentSupersession_CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId_CriterionLibraryScenarioTreatmentS~] ON [TreatmentSupersession] ([CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId], [CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioTreatment_CriterionLibraryId] ON [CriterionLibrary_ScenarioTreatment] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatmentId] ON [CriterionLibrary_ScenarioTreatment] ([ScenarioSelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibraryId] ON [CriterionLibrary_ScenarioTreatmentConsequence] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequenceId] ON [CriterionLibrary_ScenarioTreatmentConsequence] ([ScenarioConditionalTreatmentConsequenceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioTreatmentCost_CriterionLibraryId] ON [CriterionLibrary_ScenarioTreatmentCost] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioTreatmentCost_ScenarioTreatmentCostId] ON [CriterionLibrary_ScenarioTreatmentCost] ([ScenarioTreatmentCostId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibraryId] ON [CriterionLibrary_ScenarioTreatmentSupersession] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioTreatmentSupersession_TreatmentSupersessionId] ON [CriterionLibrary_ScenarioTreatmentSupersession] ([TreatmentSupersessionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_ScenarioConditionalTreatmentConsequences_AttributeId] ON [ScenarioConditionalTreatmentConsequences] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatmentId] ON [ScenarioConditionalTreatmentConsequences] ([ScenarioSelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_ScenarioSelectableTreatment_SimulationId] ON [ScenarioSelectableTreatment] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_ScenarioTreatment_Budget_BudgetId] ON [ScenarioTreatment_Budget] ([BudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_ScenarioTreatment_Budget_ScenarioSelectableTreatmentId] ON [ScenarioTreatment_Budget] ([ScenarioSelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE UNIQUE INDEX [IX_ScenarioTreatmentConsequence_Equation_EquationId] ON [ScenarioTreatmentConsequence_Equation] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE UNIQUE INDEX [IX_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequenceId] ON [ScenarioTreatmentConsequence_Equation] ([ScenarioConditionalTreatmentConsequenceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_ScenarioTreatmentCost_ScenarioTreatmentId] ON [ScenarioTreatmentCost] ([ScenarioTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE UNIQUE INDEX [IX_ScenarioTreatmentCost_Equation_EquationId] ON [ScenarioTreatmentCost_Equation] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE UNIQUE INDEX [IX_ScenarioTreatmentCost_Equation_ScenarioTreatmentCostId] ON [ScenarioTreatmentCost_Equation] ([ScenarioTreatmentCostId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_ScenarioTreatmentSchedulingEntity_ScenarioSelectableTreatmentId] ON [ScenarioTreatmentSchedulingEntity] ([ScenarioSelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    CREATE INDEX [IX_ScenarioTreatmentSupersessionEntity_ScenarioSelectableTreatmentId] ON [ScenarioTreatmentSupersessionEntity] ([ScenarioSelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    ALTER TABLE [TreatmentSupersession] ADD CONSTRAINT [FK_TreatmentSupersession_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibraryScenarioTreatmentSupersessionJoinCriter~] FOREIGN KEY ([CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId], [CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId]) REFERENCES [CriterionLibrary_ScenarioTreatmentSupersession] ([CriterionLibraryId], [TreatmentSupersessionId]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210803201905_TreatmentChanges')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210803201905_TreatmentChanges', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    ALTER TABLE [CriterionLibrary_ScenarioTreatmentSupersession] DROP CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentSupersession_ScenarioTreatmentSupersessionEntity_TreatmentSupersessionId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    ALTER TABLE [ScenarioTreatmentCost] DROP CONSTRAINT [FK_ScenarioTreatmentCost_ScenarioSelectableTreatment_ScenarioTreatmentId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    DROP TABLE [BudgetLibrary_Simulation];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    DROP TABLE [ScenarioTreatment_Budget];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    DROP TABLE [ScenarioTreatmentSchedulingEntity];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    DROP TABLE [ScenarioTreatmentSupersessionEntity];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    DROP TABLE [Treatment_Budget];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ScenarioSelectableTreatment]') AND [c].[name] = N'ScenarioTreatmentId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ScenarioSelectableTreatment] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [ScenarioSelectableTreatment] DROP COLUMN [ScenarioTreatmentId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    EXEC sp_rename N'[ScenarioTreatmentCost].[ScenarioTreatmentId]', N'ScenarioSelectableTreatmentId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    EXEC sp_rename N'[ScenarioTreatmentCost].[IX_ScenarioTreatmentCost_ScenarioTreatmentId]', N'IX_ScenarioTreatmentCost_ScenarioSelectableTreatmentId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    ALTER TABLE [CommittedProject] ADD [ScenarioBudgetId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    ALTER TABLE [BudgetPercentagePair] ADD [ScenarioBudgetId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE TABLE [ScenarioBudget] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ScenarioBudget] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioBudget_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE TABLE [ScenarioTreatmentScheduling] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [OffsetToFutureYear] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatmentScheduling] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioTreatmentScheduling_ScenarioSelectableTreatment_TreatmentId] FOREIGN KEY ([TreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE TABLE [ScenarioTreatmentSupersession] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatmentSupersession] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioTreatmentSupersession_ScenarioSelectableTreatment_TreatmentId] FOREIGN KEY ([TreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioBudget] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioBudgetId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioBudget] PRIMARY KEY ([CriterionLibraryId], [ScenarioBudgetId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioBudget_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioBudget_ScenarioBudget_ScenarioBudgetId] FOREIGN KEY ([ScenarioBudgetId]) REFERENCES [ScenarioBudget] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE TABLE [ScenarioBudgetAmount] (
        [Id] uniqueidentifier NOT NULL,
        [ScenarioBudgetId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Year] int NOT NULL,
        [Value] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_ScenarioBudgetAmount] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioBudgetAmount_ScenarioBudget_ScenarioBudgetId] FOREIGN KEY ([ScenarioBudgetId]) REFERENCES [ScenarioBudget] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE TABLE [ScenarioSelectableTreatment_ScenarioBudget] (
        [ScenarioSelectableTreatmentId] uniqueidentifier NOT NULL,
        [ScenarioBudgetId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioSelectableTreatment_ScenarioBudget] PRIMARY KEY ([ScenarioSelectableTreatmentId], [ScenarioBudgetId]),
        CONSTRAINT [FK_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudget_ScenarioBudgetId] FOREIGN KEY ([ScenarioBudgetId]) REFERENCES [ScenarioBudget] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId] FOREIGN KEY ([ScenarioSelectableTreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE INDEX [IX_CommittedProject_ScenarioBudgetId] ON [CommittedProject] ([ScenarioBudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE INDEX [IX_BudgetPercentagePair_ScenarioBudgetId] ON [BudgetPercentagePair] ([ScenarioBudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioBudget_CriterionLibraryId] ON [CriterionLibrary_ScenarioBudget] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioBudget_ScenarioBudgetId] ON [CriterionLibrary_ScenarioBudget] ([ScenarioBudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE INDEX [IX_ScenarioBudget_SimulationId] ON [ScenarioBudget] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE INDEX [IX_ScenarioBudgetAmount_ScenarioBudgetId] ON [ScenarioBudgetAmount] ([ScenarioBudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioBudgetId] ON [ScenarioSelectableTreatment_ScenarioBudget] ([ScenarioBudgetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE INDEX [IX_ScenarioSelectableTreatment_ScenarioBudget_ScenarioSelectableTreatmentId] ON [ScenarioSelectableTreatment_ScenarioBudget] ([ScenarioSelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE INDEX [IX_ScenarioTreatmentScheduling_TreatmentId] ON [ScenarioTreatmentScheduling] ([TreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    CREATE INDEX [IX_ScenarioTreatmentSupersession_TreatmentId] ON [ScenarioTreatmentSupersession] ([TreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    ALTER TABLE [CriterionLibrary_ScenarioTreatmentSupersession] ADD CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentSupersession_ScenarioTreatmentSupersession_TreatmentSupersessionId] FOREIGN KEY ([TreatmentSupersessionId]) REFERENCES [ScenarioTreatmentSupersession] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    ALTER TABLE [ScenarioTreatmentCost] ADD CONSTRAINT [FK_ScenarioTreatmentCost_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId] FOREIGN KEY ([ScenarioSelectableTreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210809211618_AddScenarioBudgetTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210809211618_AddScenarioBudgetTables', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    DROP TABLE [DeficientConditionGoalLibrary_Simulation];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    DROP TABLE [TargetConditionGoalLibrary_Simulation];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE TABLE [ScenarioDeficientConditionGoal] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [AllowedDeficientPercentage] float NOT NULL,
        [DeficientLimit] float NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioDeficientConditionGoal] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioDeficientConditionGoal_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioDeficientConditionGoal_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE TABLE [ScenarioTargetConditionGoals] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [Target] float NOT NULL,
        [Year] int NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTargetConditionGoals] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioTargetConditionGoals_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioTargetConditionGoals_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioDeficientConditionGoal] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioDeficientConditionGoalId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioDeficientConditionGoal] PRIMARY KEY ([CriterionLibraryId], [ScenarioDeficientConditionGoalId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioDeficientConditionGoal_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioDeficientConditionGoal_ScenarioDeficientConditionGoal_ScenarioDeficientConditionGoalId] FOREIGN KEY ([ScenarioDeficientConditionGoalId]) REFERENCES [ScenarioDeficientConditionGoal] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioTargetConditionGoal] (
        [ScenarioTargetConditionGoalId] uniqueidentifier NOT NULL,
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioTargetConditionGoal] PRIMARY KEY ([CriterionLibraryId], [ScenarioTargetConditionGoalId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioTargetConditionGoal_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioTargetConditionGoal_ScenarioTargetConditionGoals_ScenarioTargetConditionGoalId] FOREIGN KEY ([ScenarioTargetConditionGoalId]) REFERENCES [ScenarioTargetConditionGoals] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioDeficientConditionGoal_CriterionLibraryId] ON [CriterionLibrary_ScenarioDeficientConditionGoal] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioDeficientConditionGoal_ScenarioDeficientConditionGoalId] ON [CriterionLibrary_ScenarioDeficientConditionGoal] ([ScenarioDeficientConditionGoalId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioTargetConditionGoal_CriterionLibraryId] ON [CriterionLibrary_ScenarioTargetConditionGoal] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioTargetConditionGoal_ScenarioTargetConditionGoalId] ON [CriterionLibrary_ScenarioTargetConditionGoal] ([ScenarioTargetConditionGoalId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE INDEX [IX_ScenarioDeficientConditionGoal_AttributeId] ON [ScenarioDeficientConditionGoal] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE INDEX [IX_ScenarioDeficientConditionGoal_SimulationId] ON [ScenarioDeficientConditionGoal] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE INDEX [IX_ScenarioTargetConditionGoals_AttributeId] ON [ScenarioTargetConditionGoals] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    CREATE INDEX [IX_ScenarioTargetConditionGoals_SimulationId] ON [ScenarioTargetConditionGoals] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210813210309_AddTargetAndDeficientTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210813210309_AddTargetAndDeficientTables', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    DROP TABLE [BudgetPriorityLibrary_Simulation];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    ALTER TABLE [BudgetPercentagePair] DROP CONSTRAINT [FK_BudgetPercentagePair_BudgetPriority_BudgetPriorityId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    DROP INDEX [IX_BudgetPercentagePair_BudgetPriorityId] ON [BudgetPercentagePair];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BudgetPercentagePair]') AND [c].[name] = N'BudgetPriorityId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [BudgetPercentagePair] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [BudgetPercentagePair] DROP COLUMN [BudgetPriorityId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    ALTER TABLE [BudgetPercentagePair] ADD [ScenarioBudgetPriorityId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    CREATE TABLE [ScenarioBudgetPriority] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [PriorityLevel] int NOT NULL,
        [Year] int NULL,
        CONSTRAINT [PK_ScenarioBudgetPriority] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioBudgetPriority_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioBudgetPriority] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioBudgetPriorityId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioBudgetPriority] PRIMARY KEY ([CriterionLibraryId], [ScenarioBudgetPriorityId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioBudgetPriority_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioBudgetPriority_ScenarioBudgetPriority_ScenarioBudgetPriorityId] FOREIGN KEY ([ScenarioBudgetPriorityId]) REFERENCES [ScenarioBudgetPriority] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    ALTER TABLE [BudgetPercentagePair] ADD CONSTRAINT [FK_BudgetPercentagePair_ScenarioBudgetPriority_ScenarioBudgetPriorityId] FOREIGN KEY ([ScenarioBudgetPriorityId]) REFERENCES [ScenarioBudgetPriority] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    CREATE INDEX [IX_BudgetPercentagePair_ScenarioBudgetPriorityId] ON [BudgetPercentagePair] ([ScenarioBudgetPriorityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioBudgetPriority_CriterionLibraryId] ON [CriterionLibrary_ScenarioBudgetPriority] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioBudgetPriority_ScenarioBudgetPriorityId] ON [CriterionLibrary_ScenarioBudgetPriority] ([ScenarioBudgetPriorityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    CREATE INDEX [IX_ScenarioBudgetPriority_SimulationId] ON [ScenarioBudgetPriority] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210817141936_AddScenarioBudgetPriorityTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210817141936_AddScenarioBudgetPriorityTables', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210818170114_AddRemainingLifeLimitTables')
BEGIN
    DROP TABLE [RemainingLifeLimitLibrary_Simulation];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210818170114_AddRemainingLifeLimitTables')
BEGIN
    DROP INDEX [IX_BudgetPercentagePair_ScenarioBudgetPriorityId] ON [BudgetPercentagePair];
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BudgetPercentagePair]') AND [c].[name] = N'ScenarioBudgetPriorityId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [BudgetPercentagePair] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [BudgetPercentagePair] ALTER COLUMN [ScenarioBudgetPriorityId] uniqueidentifier NOT NULL;
    ALTER TABLE [BudgetPercentagePair] ADD DEFAULT '00000000-0000-0000-0000-000000000000' FOR [ScenarioBudgetPriorityId];
    CREATE INDEX [IX_BudgetPercentagePair_ScenarioBudgetPriorityId] ON [BudgetPercentagePair] ([ScenarioBudgetPriorityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210818170114_AddRemainingLifeLimitTables')
BEGIN
    CREATE TABLE [ScenarioRemainingLifeLimit] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [Value] float NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioRemainingLifeLimit] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioRemainingLifeLimit_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioRemainingLifeLimit_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210818170114_AddRemainingLifeLimitTables')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioRemainingLifeLimit] (
        [ScenarioRemainingLifeLimitId] uniqueidentifier NOT NULL,
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioRemainingLifeLimit] PRIMARY KEY ([CriterionLibraryId], [ScenarioRemainingLifeLimitId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioRemainingLifeLimit_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioRemainingLifeLimit_ScenarioRemainingLifeLimit_ScenarioRemainingLifeLimitId] FOREIGN KEY ([ScenarioRemainingLifeLimitId]) REFERENCES [ScenarioRemainingLifeLimit] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210818170114_AddRemainingLifeLimitTables')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioRemainingLifeLimit_CriterionLibraryId] ON [CriterionLibrary_ScenarioRemainingLifeLimit] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210818170114_AddRemainingLifeLimitTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioRemainingLifeLimit_ScenarioRemainingLifeLimitId] ON [CriterionLibrary_ScenarioRemainingLifeLimit] ([ScenarioRemainingLifeLimitId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210818170114_AddRemainingLifeLimitTables')
BEGIN
    CREATE INDEX [IX_ScenarioRemainingLifeLimit_AttributeId] ON [ScenarioRemainingLifeLimit] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210818170114_AddRemainingLifeLimitTables')
BEGIN
    CREATE INDEX [IX_ScenarioRemainingLifeLimit_SimulationId] ON [ScenarioRemainingLifeLimit] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210818170114_AddRemainingLifeLimitTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210818170114_AddRemainingLifeLimitTables', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210820014111_AddScenarioCashFlowRuleTables')
BEGIN
    DROP TABLE [CashFlowRuleLibrary_Simulation];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210820014111_AddScenarioCashFlowRuleTables')
BEGIN
    CREATE TABLE [ScenarioCashFlowRule] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ScenarioCashFlowRule] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioCashFlowRule_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210820014111_AddScenarioCashFlowRuleTables')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioCashFlowRule] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioCashFlowRuleId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioCashFlowRule] PRIMARY KEY ([CriterionLibraryId], [ScenarioCashFlowRuleId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioCashFlowRule_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioCashFlowRule_ScenarioCashFlowRule_ScenarioCashFlowRuleId] FOREIGN KEY ([ScenarioCashFlowRuleId]) REFERENCES [ScenarioCashFlowRule] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210820014111_AddScenarioCashFlowRuleTables')
BEGIN
    CREATE TABLE [ScenarioCashFlowDistributionRule] (
        [Id] uniqueidentifier NOT NULL,
        [ScenarioCashFlowRuleId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [DurationInYears] int NOT NULL,
        [CostCeiling] decimal(18,2) NOT NULL,
        [YearlyPercentages] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ScenarioCashFlowDistributionRule] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioCashFlowDistributionRule_ScenarioCashFlowRule_ScenarioCashFlowRuleId] FOREIGN KEY ([ScenarioCashFlowRuleId]) REFERENCES [ScenarioCashFlowRule] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210820014111_AddScenarioCashFlowRuleTables')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioCashFlowRule_CriterionLibraryId] ON [CriterionLibrary_ScenarioCashFlowRule] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210820014111_AddScenarioCashFlowRuleTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioCashFlowRule_ScenarioCashFlowRuleId] ON [CriterionLibrary_ScenarioCashFlowRule] ([ScenarioCashFlowRuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210820014111_AddScenarioCashFlowRuleTables')
BEGIN
    CREATE INDEX [IX_ScenarioCashFlowDistributionRule_ScenarioCashFlowRuleId] ON [ScenarioCashFlowDistributionRule] ([ScenarioCashFlowRuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210820014111_AddScenarioCashFlowRuleTables')
BEGIN
    CREATE INDEX [IX_ScenarioCashFlowRule_SimulationId] ON [ScenarioCashFlowRule] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210820014111_AddScenarioCashFlowRuleTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210820014111_AddScenarioCashFlowRuleTables', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210908172620_AddTreatmentCategories')
BEGIN
    ALTER TABLE [SelectableTreatment] ADD [AssetType] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210908172620_AddTreatmentCategories')
BEGIN
    ALTER TABLE [SelectableTreatment] ADD [Category] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210908172620_AddTreatmentCategories')
BEGIN
    ALTER TABLE [ScenarioSelectableTreatment] ADD [AssetType] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210908172620_AddTreatmentCategories')
BEGIN
    ALTER TABLE [ScenarioSelectableTreatment] ADD [Category] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210908172620_AddTreatmentCategories')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210908172620_AddTreatmentCategories', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE TABLE [CalculatedAttributeLibrary] (
        [Id] uniqueidentifier NOT NULL,
        [IsDefault] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_CalculatedAttributeLibrary] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE TABLE [ScenarioCalculatedAttribute] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [CalculationTiming] int NOT NULL,
        CONSTRAINT [PK_ScenarioCalculatedAttribute] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioCalculatedAttribute_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioCalculatedAttribute_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE TABLE [CalculatedAttribute] (
        [Id] uniqueidentifier NOT NULL,
        [CalculatedAttributeLibraryId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [CalculationTiming] int NOT NULL,
        CONSTRAINT [PK_CalculatedAttribute] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CalculatedAttribute_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CalculatedAttribute_CalculatedAttributeLibrary_CalculatedAttributeLibraryId] FOREIGN KEY ([CalculatedAttributeLibraryId]) REFERENCES [CalculatedAttributeLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE TABLE [ScenarioCalculatedAttributePair] (
        [Id] uniqueidentifier NOT NULL,
        [ScenarioCalculatedAttributeId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioCalculatedAttributePair] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioCalculatedAttributePair_ScenarioCalculatedAttribute_ScenarioCalculatedAttributeId] FOREIGN KEY ([ScenarioCalculatedAttributeId]) REFERENCES [ScenarioCalculatedAttribute] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE TABLE [CalculatedAttributePair] (
        [Id] uniqueidentifier NOT NULL,
        [CalculatedAttributeId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CalculatedAttributePair] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CalculatedAttributePair_CalculatedAttribute_CalculatedAttributeId] FOREIGN KEY ([CalculatedAttributeId]) REFERENCES [CalculatedAttribute] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE TABLE [ScenarioCalculatedAttributePair_Criteria] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioCalculatedAttributePairId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioCalculatedAttributePair_Criteria] PRIMARY KEY ([ScenarioCalculatedAttributePairId], [CriterionLibraryId]),
        CONSTRAINT [FK_ScenarioCalculatedAttributePair_Criteria_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioCalculatedAttributePair_Criteria_ScenarioCalculatedAttributePair_ScenarioCalculatedAttributePairId] FOREIGN KEY ([ScenarioCalculatedAttributePairId]) REFERENCES [ScenarioCalculatedAttributePair] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE TABLE [ScenarioCalculatedAttributePair_Equation] (
        [EquationId] uniqueidentifier NOT NULL,
        [ScenarioCalculatedAttributePairId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioCalculatedAttributePair_Equation] PRIMARY KEY ([ScenarioCalculatedAttributePairId], [EquationId]),
        CONSTRAINT [FK_ScenarioCalculatedAttributePair_Equation_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ScenarioCalculatedAttributePair_Equation_ScenarioCalculatedAttributePair_ScenarioCalculatedAttributePairId] FOREIGN KEY ([ScenarioCalculatedAttributePairId]) REFERENCES [ScenarioCalculatedAttributePair] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE TABLE [CalculatedAttributePair_Criteria] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [CalculatedAttributePairId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CalculatedAttributePair_Criteria] PRIMARY KEY ([CalculatedAttributePairId], [CriterionLibraryId]),
        CONSTRAINT [FK_CalculatedAttributePair_Criteria_CalculatedAttributePair_CalculatedAttributePairId] FOREIGN KEY ([CalculatedAttributePairId]) REFERENCES [CalculatedAttributePair] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CalculatedAttributePair_Criteria_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE TABLE [CalculatedAttributePair_Equation] (
        [EquationId] uniqueidentifier NOT NULL,
        [CalculatedAttributePairId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CalculatedAttributePair_Equation] PRIMARY KEY ([CalculatedAttributePairId], [EquationId]),
        CONSTRAINT [FK_CalculatedAttributePair_Equation_CalculatedAttributePair_CalculatedAttributePairId] FOREIGN KEY ([CalculatedAttributePairId]) REFERENCES [CalculatedAttributePair] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CalculatedAttributePair_Equation_Equation_EquationId] FOREIGN KEY ([EquationId]) REFERENCES [Equation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE INDEX [IX_CalculatedAttribute_AttributeId] ON [CalculatedAttribute] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE INDEX [IX_CalculatedAttribute_CalculatedAttributeLibraryId] ON [CalculatedAttribute] ([CalculatedAttributeLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE INDEX [IX_CalculatedAttributePair_CalculatedAttributeId] ON [CalculatedAttributePair] ([CalculatedAttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CalculatedAttributePair_Criteria_CalculatedAttributePairId] ON [CalculatedAttributePair_Criteria] ([CalculatedAttributePairId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE INDEX [IX_CalculatedAttributePair_Criteria_CriterionLibraryId] ON [CalculatedAttributePair_Criteria] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CalculatedAttributePair_Equation_CalculatedAttributePairId] ON [CalculatedAttributePair_Equation] ([CalculatedAttributePairId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE UNIQUE INDEX [IX_CalculatedAttributePair_Equation_EquationId] ON [CalculatedAttributePair_Equation] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE INDEX [IX_ScenarioCalculatedAttribute_AttributeId] ON [ScenarioCalculatedAttribute] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE INDEX [IX_ScenarioCalculatedAttribute_SimulationId] ON [ScenarioCalculatedAttribute] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE INDEX [IX_ScenarioCalculatedAttributePair_ScenarioCalculatedAttributeId] ON [ScenarioCalculatedAttributePair] ([ScenarioCalculatedAttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE INDEX [IX_ScenarioCalculatedAttributePair_Criteria_CriterionLibraryId] ON [ScenarioCalculatedAttributePair_Criteria] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE UNIQUE INDEX [IX_ScenarioCalculatedAttributePair_Criteria_ScenarioCalculatedAttributePairId] ON [ScenarioCalculatedAttributePair_Criteria] ([ScenarioCalculatedAttributePairId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE UNIQUE INDEX [IX_ScenarioCalculatedAttributePair_Equation_EquationId] ON [ScenarioCalculatedAttributePair_Equation] ([EquationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    CREATE UNIQUE INDEX [IX_ScenarioCalculatedAttributePair_Equation_ScenarioCalculatedAttributePairId] ON [ScenarioCalculatedAttributePair_Equation] ([ScenarioCalculatedAttributePairId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210909151343_AddCalculatedAttributesTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210909151343_AddCalculatedAttributesTables', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    ALTER TABLE [SimulationOutput] DROP CONSTRAINT [PK_SimulationOutput];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    DROP INDEX [IX_SimulationOutput_SimulationId] ON [SimulationOutput];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    ALTER TABLE [SimulationOutput] ADD [Id] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    ALTER TABLE [SimulationOutput] ADD [OutputType] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SelectableTreatment]') AND [c].[name] = N'Category');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [SelectableTreatment] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [SelectableTreatment] ADD DEFAULT N'Preservation' FOR [Category];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SelectableTreatment]') AND [c].[name] = N'AssetType');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [SelectableTreatment] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [SelectableTreatment] ADD DEFAULT N'Bridge' FOR [AssetType];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ScenarioSelectableTreatment]') AND [c].[name] = N'Category');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [ScenarioSelectableTreatment] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [ScenarioSelectableTreatment] ADD DEFAULT N'Preservation' FOR [Category];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ScenarioSelectableTreatment]') AND [c].[name] = N'AssetType');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [ScenarioSelectableTreatment] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [ScenarioSelectableTreatment] ADD DEFAULT N'Bridge' FOR [AssetType];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    ALTER TABLE [SimulationOutput] ADD CONSTRAINT [PK_SimulationOutput] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    CREATE UNIQUE INDEX [IX_SimulationOutput_Id] ON [SimulationOutput] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    CREATE INDEX [IX_SimulationOutput_SimulationId] ON [SimulationOutput] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211014203946_ModifiedSimulationOutputTable_1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211014203946_ModifiedSimulationOutputTable_1', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220322153556_AddLastNewsAccessDate')
BEGIN
    ALTER TABLE [User] ADD [LastNewsAccessDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220322153556_AddLastNewsAccessDate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220322153556_AddLastNewsAccessDate', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [TreatmentLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [TargetConditionGoalLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [RemainingLifeLimitLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [PerformanceCurveLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [DeficientConditionGoalLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [CriterionLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [CashFlowRuleLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [CalculatedAttributeLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [BudgetPriorityLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    ALTER TABLE [BudgetLibrary] ADD [IsShared] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220323151522_SharedLibraries')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220323151522_SharedLibraries', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220516170947_DataSource')
BEGIN
    ALTER TABLE [Attribute] ADD [DataSourceId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220516170947_DataSource')
BEGIN
    CREATE TABLE [DataSource] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Type] nvarchar(max) NOT NULL,
        [Secure] bit NOT NULL,
        [Details] nvarchar(max) NULL,
        CONSTRAINT [PK_DataSource] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220516170947_DataSource')
BEGIN
    CREATE INDEX [IX_Attribute_DataSourceId] ON [Attribute] ([DataSourceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220516170947_DataSource')
BEGIN
    ALTER TABLE [Attribute] ADD CONSTRAINT [FK_Attribute_DataSource_DataSourceId] FOREIGN KEY ([DataSourceId]) REFERENCES [DataSource] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220516170947_DataSource')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220516170947_DataSource', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220523193954_UpdateSectionMaintainableAssetRemoveFacility')
BEGIN
    ALTER TABLE [Section] DROP CONSTRAINT [FK_Section_Facility_FacilityId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220523193954_UpdateSectionMaintainableAssetRemoveFacility')
BEGIN
    DROP TABLE [Facility];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220523193954_UpdateSectionMaintainableAssetRemoveFacility')
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MaintainableAsset]') AND [c].[name] = N'FacilityName');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [MaintainableAsset] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [MaintainableAsset] DROP COLUMN [FacilityName];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220523193954_UpdateSectionMaintainableAssetRemoveFacility')
BEGIN
    EXEC sp_rename N'[Section].[FacilityId]', N'NetworkId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220523193954_UpdateSectionMaintainableAssetRemoveFacility')
BEGIN
    EXEC sp_rename N'[Section].[IX_Section_FacilityId]', N'IX_Section_NetworkId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220523193954_UpdateSectionMaintainableAssetRemoveFacility')
BEGIN
    EXEC sp_rename N'[MaintainableAsset].[SectionName]', N'AssetName', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220523193954_UpdateSectionMaintainableAssetRemoveFacility')
BEGIN
    ALTER TABLE [Section] ADD CONSTRAINT [FK_Section_Network_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [Network] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220523193954_UpdateSectionMaintainableAssetRemoveFacility')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220523193954_UpdateSectionMaintainableAssetRemoveFacility', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    ALTER TABLE [CommittedProject] DROP CONSTRAINT [FK_CommittedProject_Section_SectionEntityId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    ALTER TABLE [NumericAttributeValueHistory] DROP CONSTRAINT [FK_NumericAttributeValueHistory_Section_SectionId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    ALTER TABLE [TextAttributeValueHistory] DROP CONSTRAINT [FK_TextAttributeValueHistory_Section_SectionId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    DROP TABLE [Section];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    EXEC sp_rename N'[CommittedProject].[SectionEntityId]', N'AnalysisMaintainableAssetEntityId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    EXEC sp_rename N'[CommittedProject].[IX_CommittedProject_SectionEntityId]', N'IX_CommittedProject_AnalysisMaintainableAssetEntityId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    CREATE TABLE [AnalysisMaintainableAsset] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [SpatialWeightingId] uniqueidentifier NULL,
        [NetworkId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AnalysisMaintainableAsset] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AnalysisMaintainableAsset_Equation_SpatialWeightingId] FOREIGN KEY ([SpatialWeightingId]) REFERENCES [Equation] ([Id]),
        CONSTRAINT [FK_AnalysisMaintainableAsset_Network_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [Network] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    CREATE INDEX [IX_AnalysisMaintainableAsset_NetworkId] ON [AnalysisMaintainableAsset] ([NetworkId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    CREATE INDEX [IX_AnalysisMaintainableAsset_SpatialWeightingId] ON [AnalysisMaintainableAsset] ([SpatialWeightingId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    ALTER TABLE [CommittedProject] ADD CONSTRAINT [FK_CommittedProject_AnalysisMaintainableAsset_AnalysisMaintainableAssetEntityId] FOREIGN KEY ([AnalysisMaintainableAssetEntityId]) REFERENCES [AnalysisMaintainableAsset] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    ALTER TABLE [NumericAttributeValueHistory] ADD CONSTRAINT [FK_NumericAttributeValueHistory_AnalysisMaintainableAsset_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [AnalysisMaintainableAsset] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    ALTER TABLE [TextAttributeValueHistory] ADD CONSTRAINT [FK_TextAttributeValueHistory_AnalysisMaintainableAsset_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [AnalysisMaintainableAsset] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220606191202_RenameSectionEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220606191202_RenameSectionEntity', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220616134804_databaseSpreadsheet')
BEGIN
    CREATE TABLE [ExcelWorksheets] (
        [Id] uniqueidentifier NOT NULL,
        [SerializedWorksheetContent] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ExcelWorksheets] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220616134804_databaseSpreadsheet')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220616134804_databaseSpreadsheet', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    ALTER TABLE [CommittedProject] DROP CONSTRAINT [FK_CommittedProject_MaintainableAsset_MaintainableAssetId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    DROP INDEX [IX_CommittedProject_MaintainableAssetId] ON [CommittedProject];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CommittedProject]') AND [c].[name] = N'MaintainableAssetId');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [CommittedProject] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [CommittedProject] DROP COLUMN [MaintainableAssetId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CommittedProject]') AND [c].[name] = N'ScenarioBudgetId');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [CommittedProject] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [CommittedProject] ALTER COLUMN [ScenarioBudgetId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    ALTER TABLE [CommittedProject] ADD [MaintainableAssetEntityId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    CREATE TABLE [CommittedProjectLocation] (
        [Id] uniqueidentifier NOT NULL,
        [CommittedProjectId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [Discriminator] nvarchar(max) NOT NULL,
        [LocationIdentifier] nvarchar(max) NOT NULL,
        [Start] float NULL,
        [End] float NULL,
        [Direction] int NULL,
        CONSTRAINT [PK_CommittedProjectLocation] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CommittedProjectLocation_CommittedProject_CommittedProjectId] FOREIGN KEY ([CommittedProjectId]) REFERENCES [CommittedProject] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    CREATE INDEX [IX_CommittedProject_MaintainableAssetEntityId] ON [CommittedProject] ([MaintainableAssetEntityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    CREATE UNIQUE INDEX [IX_CommittedProjectLocation_CommittedProjectId] ON [CommittedProjectLocation] ([CommittedProjectId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    ALTER TABLE [CommittedProject] ADD CONSTRAINT [FK_CommittedProject_MaintainableAsset_MaintainableAssetEntityId] FOREIGN KEY ([MaintainableAssetEntityId]) REFERENCES [MaintainableAsset] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220618173650_CommittedProjectDTO')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220618173650_CommittedProjectDTO', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220621131953_databaseSpreadsheetRename')
BEGIN
    ALTER TABLE [ExcelWorksheets] DROP CONSTRAINT [PK_ExcelWorksheets];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220621131953_databaseSpreadsheetRename')
BEGIN
    EXEC sp_rename N'[ExcelWorksheets]', N'ExcelWorksheet';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220621131953_databaseSpreadsheetRename')
BEGIN
    ALTER TABLE [ExcelWorksheet] ADD CONSTRAINT [PK_ExcelWorksheet] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220621131953_databaseSpreadsheetRename')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220621131953_databaseSpreadsheetRename', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220622192551_dataSourceIdForeignKey')
BEGIN
    ALTER TABLE [ExcelWorksheet] ADD [DataSourceId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220622192551_dataSourceIdForeignKey')
BEGIN
    CREATE UNIQUE INDEX [IX_ExcelWorksheet_DataSourceId] ON [ExcelWorksheet] ([DataSourceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220622192551_dataSourceIdForeignKey')
BEGIN
    ALTER TABLE [ExcelWorksheet] ADD CONSTRAINT [FK_ExcelWorksheet_DataSource_DataSourceId] FOREIGN KEY ([DataSourceId]) REFERENCES [DataSource] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220622192551_dataSourceIdForeignKey')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220622192551_dataSourceIdForeignKey', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    DROP INDEX [IX_ExcelWorksheet_DataSourceId] ON [ExcelWorksheet];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    ALTER TABLE [ExcelWorksheet] ADD [CreatedBy] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    ALTER TABLE [ExcelWorksheet] ADD [CreatedDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    ALTER TABLE [ExcelWorksheet] ADD [LastModifiedBy] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    ALTER TABLE [ExcelWorksheet] ADD [LastModifiedDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    ALTER TABLE [DataSource] ADD [CreatedBy] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    ALTER TABLE [DataSource] ADD [CreatedDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    ALTER TABLE [DataSource] ADD [LastModifiedBy] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    ALTER TABLE [DataSource] ADD [LastModifiedDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    CREATE INDEX [IX_ExcelWorksheet_DataSourceId] ON [ExcelWorksheet] ([DataSourceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    CREATE UNIQUE INDEX [IX_ExcelWorksheet_Id] ON [ExcelWorksheet] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    CREATE INDEX [IX_DataSource_Id] ON [DataSource] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623161542_inheritBaseEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220623161542_inheritBaseEntity', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623210827_excelImportRenames')
BEGIN
    DROP TABLE [ExcelWorksheet];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623210827_excelImportRenames')
BEGIN
    CREATE TABLE [ExcelRawData] (
        [Id] uniqueidentifier NOT NULL,
        [SerializedContent] nvarchar(max) NOT NULL,
        [DataSourceId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ExcelRawData] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ExcelRawData_DataSource_DataSourceId] FOREIGN KEY ([DataSourceId]) REFERENCES [DataSource] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623210827_excelImportRenames')
BEGIN
    CREATE INDEX [IX_ExcelRawData_DataSourceId] ON [ExcelRawData] ([DataSourceId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623210827_excelImportRenames')
BEGIN
    CREATE UNIQUE INDEX [IX_ExcelRawData_Id] ON [ExcelRawData] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220623210827_excelImportRenames')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220623210827_excelImportRenames', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220624195332_DataSourceWithBase')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220624195332_DataSourceWithBase', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220714154914_NetworkAttributes')
BEGIN
    ALTER TABLE [Network] ADD [KeyAttributeId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220714154914_NetworkAttributes')
BEGIN
    CREATE TABLE [NetworkAttribute] (
        [NetworkId] uniqueidentifier NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_NetworkAttribute] PRIMARY KEY ([AttributeId], [NetworkId]),
        CONSTRAINT [FK_NetworkAttribute_Network_NetworkId] FOREIGN KEY ([NetworkId]) REFERENCES [Network] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220714154914_NetworkAttributes')
BEGIN
    CREATE INDEX [IX_NetworkAttribute_NetworkId] ON [NetworkAttribute] ([NetworkId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220714154914_NetworkAttributes')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220714154914_NetworkAttributes', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220727205801_addCategoryColumnToCommittedProjects')
BEGIN
    ALTER TABLE [CommittedProject] ADD [Category] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220727205801_addCategoryColumnToCommittedProjects')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220727205801_addCategoryColumnToCommittedProjects', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SimulationOutput]') AND [c].[name] = N'Output');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [SimulationOutput] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [SimulationOutput] DROP COLUMN [Output];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SimulationOutput]') AND [c].[name] = N'OutputType');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [SimulationOutput] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [SimulationOutput] DROP COLUMN [OutputType];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    ALTER TABLE [SimulationOutput] ADD [InitialConditionOfNetwork] float NOT NULL DEFAULT 0.0E0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [AssetSummaryDetail] (
        [Id] uniqueidentifier NOT NULL,
        [MaintainableAssetId] uniqueidentifier NOT NULL,
        [SimulationOutputId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AssetSummaryDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AssetSummaryDetail_MaintainableAsset_MaintainableAssetId] FOREIGN KEY ([MaintainableAssetId]) REFERENCES [MaintainableAsset] ([Id]),
        CONSTRAINT [FK_AssetSummaryDetail_SimulationOutput_SimulationOutputId] FOREIGN KEY ([SimulationOutputId]) REFERENCES [SimulationOutput] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [SimulationYearDetail] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationOutputId] uniqueidentifier NOT NULL,
        [ConditionOfNetwork] float NOT NULL,
        [Year] int NOT NULL,
        CONSTRAINT [PK_SimulationYearDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SimulationYearDetail_SimulationOutput_SimulationOutputId] FOREIGN KEY ([SimulationOutputId]) REFERENCES [SimulationOutput] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [AssetSummaryDetailValue] (
        [Id] uniqueidentifier NOT NULL,
        [AssetSummaryDetailId] uniqueidentifier NOT NULL,
        [Discriminator] char(1) NOT NULL,
        [TextValue] nvarchar(max) NULL,
        [NumericValue] float NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AssetSummaryDetailValue] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AssetSummaryDetailValue_AssetSummaryDetail_AssetSummaryDetailId] FOREIGN KEY ([AssetSummaryDetailId]) REFERENCES [AssetSummaryDetail] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AssetSummaryDetailValue_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [AssetDetail] (
        [Id] uniqueidentifier NOT NULL,
        [MaintainableAssetId] uniqueidentifier NOT NULL,
        [SimulationYearDetailId] uniqueidentifier NOT NULL,
        [AppliedTreatment] nvarchar(max) NULL,
        [TreatmentCause] int NOT NULL,
        [TreatmentFundingIgnoresSpendingLimit] bit NOT NULL,
        [TreatmentStatus] int NOT NULL,
        CONSTRAINT [PK_AssetDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AssetDetail_MaintainableAsset_MaintainableAssetId] FOREIGN KEY ([MaintainableAssetId]) REFERENCES [MaintainableAsset] ([Id]),
        CONSTRAINT [FK_AssetDetail_SimulationYearDetail_SimulationYearDetailId] FOREIGN KEY ([SimulationYearDetailId]) REFERENCES [SimulationYearDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [BudgetDetail] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationYearDetailId] uniqueidentifier NOT NULL,
        [AvailableFunding] decimal(18,2) NOT NULL,
        [BudgetName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_BudgetDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BudgetDetail_SimulationYearDetail_SimulationYearDetailId] FOREIGN KEY ([SimulationYearDetailId]) REFERENCES [SimulationYearDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [DeficientConditionGoalDetail] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationYearDetailId] uniqueidentifier NOT NULL,
        [ActualDeficientPercentage] float NOT NULL,
        [AllowedDeficientPercentage] float NOT NULL,
        [DeficientLimit] float NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [GoalIsMet] bit NOT NULL,
        [GoalName] nvarchar(max) NULL,
        CONSTRAINT [PK_DeficientConditionGoalDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeficientConditionGoalDetail_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DeficientConditionGoalDetail_SimulationYearDetail_SimulationYearDetailId] FOREIGN KEY ([SimulationYearDetailId]) REFERENCES [SimulationYearDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [TargetConditionGoalDetail] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationYearDetailId] uniqueidentifier NOT NULL,
        [ActualValue] float NOT NULL,
        [TargetValue] float NOT NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        [GoalIsMet] bit NOT NULL,
        [GoalName] nvarchar(max) NULL,
        CONSTRAINT [PK_TargetConditionGoalDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TargetConditionGoalDetail_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TargetConditionGoalDetail_SimulationYearDetail_SimulationYearDetailId] FOREIGN KEY ([SimulationYearDetailId]) REFERENCES [SimulationYearDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [AssetDetailValue] (
        [Id] uniqueidentifier NOT NULL,
        [AssetDetailId] uniqueidentifier NOT NULL,
        [Discriminator] char(1) NOT NULL,
        [TextValue] nvarchar(max) NULL,
        [NumericValue] float NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AssetDetailValue] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AssetDetailValue_AssetDetail_AssetDetailId] FOREIGN KEY ([AssetDetailId]) REFERENCES [AssetDetail] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AssetDetailValue_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [TreatmentConsiderationDetail] (
        [Id] uniqueidentifier NOT NULL,
        [AssetDetailId] uniqueidentifier NOT NULL,
        [BudgetPriorityLevel] int NULL,
        [TreatmentName] nvarchar(max) NULL,
        CONSTRAINT [PK_TreatmentConsiderationDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentConsiderationDetail_AssetDetail_AssetDetailId] FOREIGN KEY ([AssetDetailId]) REFERENCES [AssetDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [TreatmentOptionDetail] (
        [Id] uniqueidentifier NOT NULL,
        [AssetDetailId] uniqueidentifier NOT NULL,
        [Benefit] float NOT NULL,
        [Cost] float NOT NULL,
        [RemainingLife] float NULL,
        [TreatmentName] nvarchar(max) NULL,
        CONSTRAINT [PK_TreatmentOptionDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentOptionDetail_AssetDetail_AssetDetailId] FOREIGN KEY ([AssetDetailId]) REFERENCES [AssetDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [TreatmentRejectionDetail] (
        [Id] uniqueidentifier NOT NULL,
        [AssetDetailId] uniqueidentifier NOT NULL,
        [TreatmentName] nvarchar(max) NULL,
        [TreatmentRejectionReason] int NOT NULL,
        CONSTRAINT [PK_TreatmentRejectionDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentRejectionDetail_AssetDetail_AssetDetailId] FOREIGN KEY ([AssetDetailId]) REFERENCES [AssetDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [TreatmentSchedulingCollisionDetail] (
        [Id] uniqueidentifier NOT NULL,
        [AssetDetailId] uniqueidentifier NOT NULL,
        [NameOfUnscheduledTreatment] nvarchar(max) NULL,
        CONSTRAINT [PK_TreatmentSchedulingCollisionDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentSchedulingCollisionDetail_AssetDetail_AssetDetailId] FOREIGN KEY ([AssetDetailId]) REFERENCES [AssetDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [BudgetUsageDetail] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentConsiderationDetailId] uniqueidentifier NOT NULL,
        [BudgetName] nvarchar(max) NULL,
        [CoveredCost] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_BudgetUsageDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BudgetUsageDetail_TreatmentConsiderationDetail_TreatmentConsiderationDetailId] FOREIGN KEY ([TreatmentConsiderationDetailId]) REFERENCES [TreatmentConsiderationDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE TABLE [CashFlowConsiderationDetail] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentConsiderationDetailId] uniqueidentifier NOT NULL,
        [CashFlowRuleName] nvarchar(max) NULL,
        [ReasonAgainstCashFlow] int NOT NULL,
        CONSTRAINT [PK_CashFlowConsiderationDetail] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CashFlowConsiderationDetail_TreatmentConsiderationDetail_TreatmentConsiderationDetailId] FOREIGN KEY ([TreatmentConsiderationDetailId]) REFERENCES [TreatmentConsiderationDetail] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_AssetDetail_Id] ON [AssetDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_AssetDetail_MaintainableAssetId] ON [AssetDetail] ([MaintainableAssetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_AssetDetail_SimulationYearDetailId] ON [AssetDetail] ([SimulationYearDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_AssetDetailValue_AssetDetailId] ON [AssetDetailValue] ([AssetDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_AssetDetailValue_AttributeId] ON [AssetDetailValue] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_AssetDetailValue_Id] ON [AssetDetailValue] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_AssetSummaryDetail_Id] ON [AssetSummaryDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_AssetSummaryDetail_MaintainableAssetId] ON [AssetSummaryDetail] ([MaintainableAssetId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_AssetSummaryDetail_SimulationOutputId] ON [AssetSummaryDetail] ([SimulationOutputId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_AssetSummaryDetailValue_AssetSummaryDetailId] ON [AssetSummaryDetailValue] ([AssetSummaryDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_AssetSummaryDetailValue_AttributeId] ON [AssetSummaryDetailValue] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_AssetSummaryDetailValue_Id] ON [AssetSummaryDetailValue] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_BudgetDetail_Id] ON [BudgetDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_BudgetDetail_SimulationYearDetailId] ON [BudgetDetail] ([SimulationYearDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_BudgetUsageDetail_Id] ON [BudgetUsageDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_BudgetUsageDetail_TreatmentConsiderationDetailId] ON [BudgetUsageDetail] ([TreatmentConsiderationDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_CashFlowConsiderationDetail_Id] ON [CashFlowConsiderationDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_CashFlowConsiderationDetail_TreatmentConsiderationDetailId] ON [CashFlowConsiderationDetail] ([TreatmentConsiderationDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_DeficientConditionGoalDetail_AttributeId] ON [DeficientConditionGoalDetail] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_DeficientConditionGoalDetail_Id] ON [DeficientConditionGoalDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_DeficientConditionGoalDetail_SimulationYearDetailId] ON [DeficientConditionGoalDetail] ([SimulationYearDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_SimulationYearDetail_Id] ON [SimulationYearDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_SimulationYearDetail_SimulationOutputId] ON [SimulationYearDetail] ([SimulationOutputId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_TargetConditionGoalDetail_AttributeId] ON [TargetConditionGoalDetail] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_TargetConditionGoalDetail_Id] ON [TargetConditionGoalDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_TargetConditionGoalDetail_SimulationYearDetailId] ON [TargetConditionGoalDetail] ([SimulationYearDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_TreatmentConsiderationDetail_AssetDetailId] ON [TreatmentConsiderationDetail] ([AssetDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_TreatmentConsiderationDetail_Id] ON [TreatmentConsiderationDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_TreatmentOptionDetail_AssetDetailId] ON [TreatmentOptionDetail] ([AssetDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_TreatmentOptionDetail_Id] ON [TreatmentOptionDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_TreatmentRejectionDetail_AssetDetailId] ON [TreatmentRejectionDetail] ([AssetDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_TreatmentRejectionDetail_Id] ON [TreatmentRejectionDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE INDEX [IX_TreatmentSchedulingCollisionDetail_AssetDetailId] ON [TreatmentSchedulingCollisionDetail] ([AssetDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    CREATE UNIQUE INDEX [IX_TreatmentSchedulingCollisionDetail_Id] ON [TreatmentSchedulingCollisionDetail] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20220919170520_simulationOutputEntitiesNew')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20220919170520_simulationOutputEntitiesNew', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221006221925_UpdateInvestmentPlanEntity')
BEGIN
    ALTER TABLE [InvestmentPlan] ADD [ShouldAccumulateUnusedBudgetAmounts] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221006221925_UpdateInvestmentPlanEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221006221925_UpdateInvestmentPlanEntity', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221010210006_AddOrderColumnToBudgets')
BEGIN
    ALTER TABLE [ScenarioBudget] ADD [BudgetOrder] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221010210006_AddOrderColumnToBudgets')
BEGIN
    ALTER TABLE [Budget] ADD [BudgetOrder] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221010210006_AddOrderColumnToBudgets')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221010210006_AddOrderColumnToBudgets', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221012144242_CreateBudgetLibraryUserEntity')
BEGIN
    CREATE TABLE [BudgetLibrary_User] (
        [BudgetLibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [AccessLevel] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_BudgetLibrary_User] PRIMARY KEY ([BudgetLibraryId], [UserId]),
        CONSTRAINT [FK_BudgetLibrary_User_BudgetLibrary_BudgetLibraryId] FOREIGN KEY ([BudgetLibraryId]) REFERENCES [BudgetLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BudgetLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221012144242_CreateBudgetLibraryUserEntity')
BEGIN
    CREATE INDEX [IX_BudgetLibrary_User_BudgetLibraryId] ON [BudgetLibrary_User] ([BudgetLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221012144242_CreateBudgetLibraryUserEntity')
BEGIN
    CREATE INDEX [IX_BudgetLibrary_User_UserId] ON [BudgetLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221012144242_CreateBudgetLibraryUserEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221012144242_CreateBudgetLibraryUserEntity', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128151512_AssetDetailValueIntId')
BEGIN
    CREATE TABLE [AssetDetailValueIntId] (
        [Id] int NOT NULL IDENTITY,
        [AssetDetailId] uniqueidentifier NOT NULL,
        [Discriminator] char(1) NOT NULL,
        [TextValue] nvarchar(max) NULL,
        [NumericValue] float NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AssetDetailValueIntId] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AssetDetailValueIntId_AssetDetail_AssetDetailId] FOREIGN KEY ([AssetDetailId]) REFERENCES [AssetDetail] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AssetDetailValueIntId_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128151512_AssetDetailValueIntId')
BEGIN
    CREATE TABLE [AssetSummaryDetailValueIntId] (
        [Id] int NOT NULL IDENTITY,
        [AssetSummaryDetailId] uniqueidentifier NOT NULL,
        [Discriminator] char(1) NOT NULL,
        [TextValue] nvarchar(max) NULL,
        [NumericValue] float NULL,
        [AttributeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_AssetSummaryDetailValueIntId] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AssetSummaryDetailValueIntId_AssetSummaryDetail_AssetSummaryDetailId] FOREIGN KEY ([AssetSummaryDetailId]) REFERENCES [AssetSummaryDetail] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AssetSummaryDetailValueIntId_Attribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [Attribute] ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128151512_AssetDetailValueIntId')
BEGIN
    CREATE INDEX [IX_AssetDetailValueIntId_AssetDetailId] ON [AssetDetailValueIntId] ([AssetDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128151512_AssetDetailValueIntId')
BEGIN
    CREATE INDEX [IX_AssetDetailValueIntId_AttributeId] ON [AssetDetailValueIntId] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128151512_AssetDetailValueIntId')
BEGIN
    CREATE UNIQUE INDEX [IX_AssetDetailValueIntId_Id] ON [AssetDetailValueIntId] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128151512_AssetDetailValueIntId')
BEGIN
    CREATE INDEX [IX_AssetSummaryDetailValueIntId_AssetSummaryDetailId] ON [AssetSummaryDetailValueIntId] ([AssetSummaryDetailId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128151512_AssetDetailValueIntId')
BEGIN
    CREATE INDEX [IX_AssetSummaryDetailValueIntId_AttributeId] ON [AssetSummaryDetailValueIntId] ([AttributeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128151512_AssetDetailValueIntId')
BEGIN
    CREATE UNIQUE INDEX [IX_AssetSummaryDetailValueIntId_Id] ON [AssetSummaryDetailValueIntId] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128151512_AssetDetailValueIntId')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221128151512_AssetDetailValueIntId', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128183458_AssetDetailValueGuidIdDeleteAttributeFk')
BEGIN
    ALTER TABLE [AssetDetailValue] DROP CONSTRAINT [FK_AssetDetailValue_Attribute_AttributeId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128183458_AssetDetailValueGuidIdDeleteAttributeFk')
BEGIN
    ALTER TABLE [AssetSummaryDetailValue] DROP CONSTRAINT [FK_AssetSummaryDetailValue_Attribute_AttributeId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128183458_AssetDetailValueGuidIdDeleteAttributeFk')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221128183458_AssetDetailValueGuidIdDeleteAttributeFk', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128184858_DeleteOldValueEntities')
BEGIN
    DROP TABLE [AssetDetailValue];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128184858_DeleteOldValueEntities')
BEGIN
    DROP TABLE [AssetSummaryDetailValue];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221128184858_DeleteOldValueEntities')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221128184858_DeleteOldValueEntities', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221216165035_NoTreatmentBeforeCommittedProjects')
BEGIN
    ALTER TABLE [Simulation] ADD [NoTreatmentBeforeCommittedProjects] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20221216165035_NoTreatmentBeforeCommittedProjects')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20221216165035_NoTreatmentBeforeCommittedProjects', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230116152730_CreateRemainingLifeLimitLibraryUser')
BEGIN
    CREATE TABLE [RemainingLifeLimitLibrary_User] (
        [RemainingLifeLimitLibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [AccessLevel] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RemainingLifeLimitLibrary_User] PRIMARY KEY ([RemainingLifeLimitLibraryId], [UserId]),
        CONSTRAINT [FK_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId] FOREIGN KEY ([RemainingLifeLimitLibraryId]) REFERENCES [RemainingLifeLimitLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RemainingLifeLimitLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230116152730_CreateRemainingLifeLimitLibraryUser')
BEGIN
    CREATE INDEX [IX_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibraryId] ON [RemainingLifeLimitLibrary_User] ([RemainingLifeLimitLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230116152730_CreateRemainingLifeLimitLibraryUser')
BEGIN
    CREATE INDEX [IX_RemainingLifeLimitLibrary_User_UserId] ON [RemainingLifeLimitLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230116152730_CreateRemainingLifeLimitLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230116152730_CreateRemainingLifeLimitLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117151617_Create_SimulationOutputJson_Table')
BEGIN
    CREATE TABLE [SimulationOutputJson] (
        [Id] uniqueidentifier NOT NULL,
        [SimulationId] uniqueidentifier NOT NULL,
        [Output] nvarchar(max) NOT NULL,
        [OutputType] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_SimulationOutputJson] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SimulationOutputJson_Simulation_SimulationId] FOREIGN KEY ([SimulationId]) REFERENCES [Simulation] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117151617_Create_SimulationOutputJson_Table')
BEGIN
    CREATE UNIQUE INDEX [IX_SimulationOutputJson_Id] ON [SimulationOutputJson] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117151617_Create_SimulationOutputJson_Table')
BEGIN
    CREATE INDEX [IX_SimulationOutputJson_SimulationId] ON [SimulationOutputJson] ([SimulationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117151617_Create_SimulationOutputJson_Table')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230117151617_Create_SimulationOutputJson_Table', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117225145_CreateDeficientConditionGoalLibraryUser')
BEGIN
    CREATE TABLE [DeficientConditionGoalLibrary_User] (
        [DeficientConditionGoalLibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [AccessLevel] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_DeficientConditionGoalLibrary_User] PRIMARY KEY ([DeficientConditionGoalLibraryId], [UserId]),
        CONSTRAINT [FK_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId] FOREIGN KEY ([DeficientConditionGoalLibraryId]) REFERENCES [DeficientConditionGoalLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DeficientConditionGoalLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117225145_CreateDeficientConditionGoalLibraryUser')
BEGIN
    CREATE INDEX [IX_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibraryId] ON [DeficientConditionGoalLibrary_User] ([DeficientConditionGoalLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117225145_CreateDeficientConditionGoalLibraryUser')
BEGIN
    CREATE INDEX [IX_DeficientConditionGoalLibrary_User_UserId] ON [DeficientConditionGoalLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117225145_CreateDeficientConditionGoalLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230117225145_CreateDeficientConditionGoalLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117234957_Relational_Json_Relation')
BEGIN
    ALTER TABLE [SimulationOutputJson] ADD [SimulationOutputId] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117234957_Relational_Json_Relation')
BEGIN
    CREATE INDEX [IX_SimulationOutputJson_SimulationOutputId] ON [SimulationOutputJson] ([SimulationOutputId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117234957_Relational_Json_Relation')
BEGIN
    ALTER TABLE [SimulationOutputJson] ADD CONSTRAINT [FK_SimulationOutputJson_SimulationOutput_SimulationOutputId] FOREIGN KEY ([SimulationOutputId]) REFERENCES [SimulationOutput] ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230117234957_Relational_Json_Relation')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230117234957_Relational_Json_Relation', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230123225843_CreateCashFlowRuleLibraryUser')
BEGIN
    CREATE TABLE [CashFlowRuleLibrary_User] (
        [CashFlowRuleLibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [AccessLevel] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CashFlowRuleLibrary_User] PRIMARY KEY ([CashFlowRuleLibraryId], [UserId]),
        CONSTRAINT [FK_CashFlowRuleLibrary_User_CashFlowRuleLibrary_CashFlowRuleLibraryId] FOREIGN KEY ([CashFlowRuleLibraryId]) REFERENCES [CashFlowRuleLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CashFlowRuleLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230123225843_CreateCashFlowRuleLibraryUser')
BEGIN
    CREATE INDEX [IX_CashFlowRuleLibrary_User_CashFlowRuleLibraryId] ON [CashFlowRuleLibrary_User] ([CashFlowRuleLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230123225843_CreateCashFlowRuleLibraryUser')
BEGIN
    CREATE INDEX [IX_CashFlowRuleLibrary_User_UserId] ON [CashFlowRuleLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230123225843_CreateCashFlowRuleLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230123225843_CreateCashFlowRuleLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230125222113_CreateCalculatedAttributeLibraryUser')
BEGIN
    CREATE TABLE [CalculatedAttributeLibrary_User] (
        [CalculatedAttributeLibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [AccessLevel] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CalculatedAttributeLibrary_User] PRIMARY KEY ([CalculatedAttributeLibraryId], [UserId]),
        CONSTRAINT [FK_CalculatedAttributeLibrary_User_CalculatedAttributeLibrary_CalculatedAttributeLibraryId] FOREIGN KEY ([CalculatedAttributeLibraryId]) REFERENCES [CalculatedAttributeLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CalculatedAttributeLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230125222113_CreateCalculatedAttributeLibraryUser')
BEGIN
    CREATE INDEX [IX_CalculatedAttributeLibrary_User_CalculatedAttributeLibraryId] ON [CalculatedAttributeLibrary_User] ([CalculatedAttributeLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230125222113_CreateCalculatedAttributeLibraryUser')
BEGIN
    CREATE INDEX [IX_CalculatedAttributeLibrary_User_UserId] ON [CalculatedAttributeLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230125222113_CreateCalculatedAttributeLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230125222113_CreateCalculatedAttributeLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230201211824_AddTreatmentCategoryToCommittedProjects')
BEGIN
    ALTER TABLE [CommittedProject] ADD [treatmentCategory] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230201211824_AddTreatmentCategoryToCommittedProjects')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230201211824_AddTreatmentCategoryToCommittedProjects', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230219235254_UpdateDeficientConditionGoalLibraryUser')
BEGIN
    ALTER TABLE [DeficientConditionGoalLibrary_User] DROP CONSTRAINT [FK_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230219235254_UpdateDeficientConditionGoalLibraryUser')
BEGIN
    EXEC sp_rename N'[DeficientConditionGoalLibrary_User].[DeficientConditionGoalLibraryId]', N'LibraryId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230219235254_UpdateDeficientConditionGoalLibraryUser')
BEGIN
    EXEC sp_rename N'[DeficientConditionGoalLibrary_User].[IX_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibraryId]', N'IX_DeficientConditionGoalLibrary_User_LibraryId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230219235254_UpdateDeficientConditionGoalLibraryUser')
BEGIN
    ALTER TABLE [DeficientConditionGoalLibrary_User] ADD CONSTRAINT [FK_DeficientConditionGoalLibrary_User_DeficientConditionGoalLibrary_LibraryId] FOREIGN KEY ([LibraryId]) REFERENCES [DeficientConditionGoalLibrary] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230219235254_UpdateDeficientConditionGoalLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230219235254_UpdateDeficientConditionGoalLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220170936_CreateTargetConditionGoalLibraryUser')
BEGIN
    CREATE TABLE [TargetConditionGoalLibrary_User] (
        [LibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AccessLevel] int NOT NULL,
        CONSTRAINT [PK_TargetConditionGoalLibrary_User] PRIMARY KEY ([LibraryId], [UserId]),
        CONSTRAINT [FK_TargetConditionGoalLibrary_User_TargetConditionGoalLibrary_LibraryId] FOREIGN KEY ([LibraryId]) REFERENCES [TargetConditionGoalLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TargetConditionGoalLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220170936_CreateTargetConditionGoalLibraryUser')
BEGIN
    CREATE INDEX [IX_TargetConditionGoalLibrary_User_LibraryId] ON [TargetConditionGoalLibrary_User] ([LibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220170936_CreateTargetConditionGoalLibraryUser')
BEGIN
    CREATE INDEX [IX_TargetConditionGoalLibrary_User_UserId] ON [TargetConditionGoalLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220170936_CreateTargetConditionGoalLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230220170936_CreateTargetConditionGoalLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220205404_CreatePerformanceCurveLibraryUser')
BEGIN
    CREATE TABLE [PerformanceCurveLibrary_User] (
        [LibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [UserEntityId] uniqueidentifier NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AccessLevel] int NOT NULL,
        CONSTRAINT [PK_PerformanceCurveLibrary_User] PRIMARY KEY ([LibraryId], [UserId]),
        CONSTRAINT [FK_PerformanceCurveLibrary_User_PerformanceCurveLibrary_LibraryId] FOREIGN KEY ([LibraryId]) REFERENCES [PerformanceCurveLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PerformanceCurveLibrary_User_User_UserEntityId] FOREIGN KEY ([UserEntityId]) REFERENCES [User] ([Id]),
        CONSTRAINT [FK_PerformanceCurveLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220205404_CreatePerformanceCurveLibraryUser')
BEGIN
    CREATE INDEX [IX_PerformanceCurveLibrary_User_LibraryId] ON [PerformanceCurveLibrary_User] ([LibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220205404_CreatePerformanceCurveLibraryUser')
BEGIN
    CREATE INDEX [IX_PerformanceCurveLibrary_User_UserEntityId] ON [PerformanceCurveLibrary_User] ([UserEntityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220205404_CreatePerformanceCurveLibraryUser')
BEGIN
    CREATE INDEX [IX_PerformanceCurveLibrary_User_UserId] ON [PerformanceCurveLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220205404_CreatePerformanceCurveLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230220205404_CreatePerformanceCurveLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220223331_CreateTreatmentLibraryUser')
BEGIN
    CREATE TABLE [TreatmentLibrary_User] (
        [LibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [UserEntityId] uniqueidentifier NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AccessLevel] int NOT NULL,
        CONSTRAINT [PK_TreatmentLibrary_User] PRIMARY KEY ([LibraryId], [UserId]),
        CONSTRAINT [FK_TreatmentLibrary_User_TreatmentLibrary_LibraryId] FOREIGN KEY ([LibraryId]) REFERENCES [TreatmentLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TreatmentLibrary_User_User_UserEntityId] FOREIGN KEY ([UserEntityId]) REFERENCES [User] ([Id]),
        CONSTRAINT [FK_TreatmentLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220223331_CreateTreatmentLibraryUser')
BEGIN
    CREATE INDEX [IX_TreatmentLibrary_User_LibraryId] ON [TreatmentLibrary_User] ([LibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220223331_CreateTreatmentLibraryUser')
BEGIN
    CREATE INDEX [IX_TreatmentLibrary_User_UserEntityId] ON [TreatmentLibrary_User] ([UserEntityId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220223331_CreateTreatmentLibraryUser')
BEGIN
    CREATE INDEX [IX_TreatmentLibrary_User_UserId] ON [TreatmentLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230220223331_CreateTreatmentLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230220223331_CreateTreatmentLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221161929_UpdateCashFlowRuleLibraryUser')
BEGIN
    ALTER TABLE [CashFlowRuleLibrary_User] DROP CONSTRAINT [FK_CashFlowRuleLibrary_User_CashFlowRuleLibrary_CashFlowRuleLibraryId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221161929_UpdateCashFlowRuleLibraryUser')
BEGIN
    EXEC sp_rename N'[CashFlowRuleLibrary_User].[CashFlowRuleLibraryId]', N'LibraryId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221161929_UpdateCashFlowRuleLibraryUser')
BEGIN
    EXEC sp_rename N'[CashFlowRuleLibrary_User].[IX_CashFlowRuleLibrary_User_CashFlowRuleLibraryId]', N'IX_CashFlowRuleLibrary_User_LibraryId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221161929_UpdateCashFlowRuleLibraryUser')
BEGIN
    ALTER TABLE [CashFlowRuleLibrary_User] ADD CONSTRAINT [FK_CashFlowRuleLibrary_User_CashFlowRuleLibrary_LibraryId] FOREIGN KEY ([LibraryId]) REFERENCES [CashFlowRuleLibrary] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221161929_UpdateCashFlowRuleLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230221161929_UpdateCashFlowRuleLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221205535_UpdateRemainingLifeLimitLibraryUser')
BEGIN
    ALTER TABLE [RemainingLifeLimitLibrary_User] DROP CONSTRAINT [FK_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221205535_UpdateRemainingLifeLimitLibraryUser')
BEGIN
    EXEC sp_rename N'[RemainingLifeLimitLibrary_User].[RemainingLifeLimitLibraryId]', N'LibraryId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221205535_UpdateRemainingLifeLimitLibraryUser')
BEGIN
    EXEC sp_rename N'[RemainingLifeLimitLibrary_User].[IX_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibraryId]', N'IX_RemainingLifeLimitLibrary_User_LibraryId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221205535_UpdateRemainingLifeLimitLibraryUser')
BEGIN
    ALTER TABLE [RemainingLifeLimitLibrary_User] ADD CONSTRAINT [FK_RemainingLifeLimitLibrary_User_RemainingLifeLimitLibrary_LibraryId] FOREIGN KEY ([LibraryId]) REFERENCES [RemainingLifeLimitLibrary] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221205535_UpdateRemainingLifeLimitLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230221205535_UpdateRemainingLifeLimitLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221230122_UpdateCalculatedAttributeLibraryUser')
BEGIN
    ALTER TABLE [CalculatedAttributeLibrary_User] DROP CONSTRAINT [FK_CalculatedAttributeLibrary_User_CalculatedAttributeLibrary_CalculatedAttributeLibraryId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221230122_UpdateCalculatedAttributeLibraryUser')
BEGIN
    EXEC sp_rename N'[CalculatedAttributeLibrary_User].[CalculatedAttributeLibraryId]', N'LibraryId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221230122_UpdateCalculatedAttributeLibraryUser')
BEGIN
    EXEC sp_rename N'[CalculatedAttributeLibrary_User].[IX_CalculatedAttributeLibrary_User_CalculatedAttributeLibraryId]', N'IX_CalculatedAttributeLibrary_User_LibraryId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221230122_UpdateCalculatedAttributeLibraryUser')
BEGIN
    ALTER TABLE [CalculatedAttributeLibrary_User] ADD CONSTRAINT [FK_CalculatedAttributeLibrary_User_CalculatedAttributeLibrary_LibraryId] FOREIGN KEY ([LibraryId]) REFERENCES [CalculatedAttributeLibrary] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230221230122_UpdateCalculatedAttributeLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230221230122_UpdateCalculatedAttributeLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230223224303_CreateBudgetPriorityLibraryUser')
BEGIN
    CREATE TABLE [BudgetPriorityLibrary_User] (
        [LibraryId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        [AccessLevel] int NOT NULL,
        CONSTRAINT [PK_BudgetPriorityLibrary_User] PRIMARY KEY ([LibraryId], [UserId]),
        CONSTRAINT [FK_BudgetPriorityLibrary_User_BudgetPriorityLibrary_LibraryId] FOREIGN KEY ([LibraryId]) REFERENCES [BudgetPriorityLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BudgetPriorityLibrary_User_User_UserId] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230223224303_CreateBudgetPriorityLibraryUser')
BEGIN
    CREATE INDEX [IX_BudgetPriorityLibrary_User_LibraryId] ON [BudgetPriorityLibrary_User] ([LibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230223224303_CreateBudgetPriorityLibraryUser')
BEGIN
    CREATE INDEX [IX_BudgetPriorityLibrary_User_UserId] ON [BudgetPriorityLibrary_User] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230223224303_CreateBudgetPriorityLibraryUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230223224303_CreateBudgetPriorityLibraryUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230307223358_AddConditionChangeToAnalysisOutput')
BEGIN
    ALTER TABLE [TreatmentRejectionDetail] ADD [PotentialConditionChange] float NOT NULL DEFAULT 0.0E0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230307223358_AddConditionChangeToAnalysisOutput')
BEGIN
    ALTER TABLE [TreatmentOptionDetail] ADD [ConditionChange] float NOT NULL DEFAULT 0.0E0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230307223358_AddConditionChangeToAnalysisOutput')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230307223358_AddConditionChangeToAnalysisOutput', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230321024237_AddLibraryIdToBudgetPriority')
BEGIN
    ALTER TABLE [ScenarioBudgetPriority] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230321024237_AddLibraryIdToBudgetPriority')
BEGIN
    ALTER TABLE [BudgetPriority] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230321024237_AddLibraryIdToBudgetPriority')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230321024237_AddLibraryIdToBudgetPriority', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230321210827_AddLibraryIdToScenarioTargetConditionGoal')
BEGIN
    ALTER TABLE [ScenarioTargetConditionGoals] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230321210827_AddLibraryIdToScenarioTargetConditionGoal')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230321210827_AddLibraryIdToScenarioTargetConditionGoal', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230322165439_AddLibraryIdToScenarioSelectableTreatment')
BEGIN
    ALTER TABLE [ScenarioSelectableTreatment] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230322165439_AddLibraryIdToScenarioSelectableTreatment')
BEGIN
    ALTER TABLE [ScenarioSelectableTreatment] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230322165439_AddLibraryIdToScenarioSelectableTreatment')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230322165439_AddLibraryIdToScenarioSelectableTreatment', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230322190745_AddLibraryIdToDeficientConditionGoal')
BEGIN
    ALTER TABLE [ScenarioDeficientConditionGoal] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230322190745_AddLibraryIdToDeficientConditionGoal')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230322190745_AddLibraryIdToDeficientConditionGoal', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230322201850_AddLibraryIdToScenarioRemainingLifeLimit')
BEGIN
    ALTER TABLE [ScenarioRemainingLifeLimit] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230322201850_AddLibraryIdToScenarioRemainingLifeLimit')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230322201850_AddLibraryIdToScenarioRemainingLifeLimit', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230322214905_AddLibraryIdToScenarioCashFlowRule')
BEGIN
    ALTER TABLE [ScenarioCashFlowRule] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230322214905_AddLibraryIdToScenarioCashFlowRule')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230322214905_AddLibraryIdToScenarioCashFlowRule', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323022116_AddLibraryIdToScenarioPerformanceCurve')
BEGIN
    ALTER TABLE [ScenarioPerformanceCurve] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323022116_AddLibraryIdToScenarioPerformanceCurve')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230323022116_AddLibraryIdToScenarioPerformanceCurve', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323022845_AddLibraryIdToScenarioCalculatedAttribute')
BEGIN
    ALTER TABLE [ScenarioCalculatedAttribute] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323022845_AddLibraryIdToScenarioCalculatedAttribute')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230323022845_AddLibraryIdToScenarioCalculatedAttribute', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323023915_AddLibraryIdToScenarioBudget')
BEGIN
    ALTER TABLE [ScenarioBudget] ADD [LibraryId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323023915_AddLibraryIdToScenarioBudget')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230323023915_AddLibraryIdToScenarioBudget', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323165115_AddIsModifiedToScenarioBudget')
BEGIN
    ALTER TABLE [ScenarioBudget] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323165115_AddIsModifiedToScenarioBudget')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230323165115_AddIsModifiedToScenarioBudget', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323222938_AddIsModifiedToScenarioPerformanceCurve')
BEGIN
    ALTER TABLE [ScenarioPerformanceCurve] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323222938_AddIsModifiedToScenarioPerformanceCurve')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230323222938_AddIsModifiedToScenarioPerformanceCurve', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323223652_AddIsModifiedToScenarioCalculatedAttribute')
BEGIN
    ALTER TABLE [ScenarioCalculatedAttribute] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323223652_AddIsModifiedToScenarioCalculatedAttribute')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230323223652_AddIsModifiedToScenarioCalculatedAttribute', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323225028_AddIsModifiedToScenarioBudgetPriority')
BEGIN
    ALTER TABLE [ScenarioBudgetPriority] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323225028_AddIsModifiedToScenarioBudgetPriority')
BEGIN
    ALTER TABLE [BudgetPriority] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230323225028_AddIsModifiedToScenarioBudgetPriority')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230323225028_AddIsModifiedToScenarioBudgetPriority', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230324011902_AddIsModifiedToScenarioTargetConditionGoal')
BEGIN
    ALTER TABLE [ScenarioTargetConditionGoals] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230324011902_AddIsModifiedToScenarioTargetConditionGoal')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230324011902_AddIsModifiedToScenarioTargetConditionGoal', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230324012415_AddIsModifiedToScenarioDeficientConditionGoal')
BEGIN
    ALTER TABLE [ScenarioDeficientConditionGoal] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230324012415_AddIsModifiedToScenarioDeficientConditionGoal')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230324012415_AddIsModifiedToScenarioDeficientConditionGoal', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230324020904_AddIsModifiedToScenarioRemainingLifeLimit')
BEGIN
    ALTER TABLE [ScenarioRemainingLifeLimit] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230324020904_AddIsModifiedToScenarioRemainingLifeLimit')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230324020904_AddIsModifiedToScenarioRemainingLifeLimit', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230324021439_AddIsModifiedToScenarioCashFlowRule')
BEGIN
    ALTER TABLE [ScenarioCashFlowRule] ADD [IsModified] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230324021439_AddIsModifiedToScenarioCashFlowRule')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230324021439_AddIsModifiedToScenarioCashFlowRule', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230403024020_AddPerformanceFactorToScenarioTreatment')
BEGIN
    ALTER TABLE [SelectableTreatment] ADD [PerformanceFactor] float NOT NULL DEFAULT 1.0E0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230403024020_AddPerformanceFactorToScenarioTreatment')
BEGIN
    ALTER TABLE [ScenarioSelectableTreatment] ADD [PerformanceFactor] float NOT NULL DEFAULT 1.0E0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230403024020_AddPerformanceFactorToScenarioTreatment')
BEGIN
    ALTER TABLE [CommittedProject] ADD [PerformanceFactor] float NOT NULL DEFAULT 1.0E0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230403024020_AddPerformanceFactorToScenarioTreatment')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230403024020_AddPerformanceFactorToScenarioTreatment', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230427220947_AddPerformanceFactorToCommittedProjectConsequence')
BEGIN
    ALTER TABLE [CommittedProjectConsequence] ADD [PerformanceFactor] real NOT NULL DEFAULT CAST(1.2 AS real);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230427220947_AddPerformanceFactorToCommittedProjectConsequence')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230427220947_AddPerformanceFactorToCommittedProjectConsequence', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508151730_AddNewAdminSettingsTable')
BEGIN
    CREATE TABLE [AdminSettings] (
        [Key] nvarchar(50) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AdminSettings] PRIMARY KEY ([Key])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230508151730_AddNewAdminSettingsTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230508151730_AddNewAdminSettingsTable', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230509170432_AddPerformanceFactorToSelectedTreatment')
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SelectableTreatment]') AND [c].[name] = N'PerformanceFactor');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [SelectableTreatment] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [SelectableTreatment] DROP COLUMN [PerformanceFactor];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230509170432_AddPerformanceFactorToSelectedTreatment')
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ScenarioSelectableTreatment]') AND [c].[name] = N'PerformanceFactor');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [ScenarioSelectableTreatment] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [ScenarioSelectableTreatment] DROP COLUMN [PerformanceFactor];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230509170432_AddPerformanceFactorToSelectedTreatment')
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CommittedProject]') AND [c].[name] = N'PerformanceFactor');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [CommittedProject] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [CommittedProject] DROP COLUMN [PerformanceFactor];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230509170432_AddPerformanceFactorToSelectedTreatment')
BEGIN
    CREATE TABLE [ScenarioTreatmentPerformanceFactor] (
        [Id] uniqueidentifier NOT NULL,
        [ScenarioSelectableTreatmentId] uniqueidentifier NOT NULL,
        [Attribute] nvarchar(max) NULL,
        [PerformanceFactor] real NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatmentPerformanceFactor] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioTreatmentPerformanceFactor_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId] FOREIGN KEY ([ScenarioSelectableTreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230509170432_AddPerformanceFactorToSelectedTreatment')
BEGIN
    CREATE TABLE [TreatmentPerformanceFactor] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [Attribute] nvarchar(max) NULL,
        [PerformanceFactor] real NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TreatmentPerformanceFactor] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentPerformanceFactor_SelectableTreatment_TreatmentId] FOREIGN KEY ([TreatmentId]) REFERENCES [SelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230509170432_AddPerformanceFactorToSelectedTreatment')
BEGIN
    CREATE INDEX [IX_ScenarioTreatmentPerformanceFactor_ScenarioSelectableTreatmentId] ON [ScenarioTreatmentPerformanceFactor] ([ScenarioSelectableTreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230509170432_AddPerformanceFactorToSelectedTreatment')
BEGIN
    CREATE INDEX [IX_TreatmentPerformanceFactor_TreatmentId] ON [TreatmentPerformanceFactor] ([TreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230509170432_AddPerformanceFactorToSelectedTreatment')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230509170432_AddPerformanceFactorToSelectedTreatment', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230720050816_AddNameDescriptionToUser')
BEGIN
    ALTER TABLE [User] ADD [Description] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230720050816_AddNameDescriptionToUser')
BEGIN
    ALTER TABLE [User] ADD [Name] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230720050816_AddNameDescriptionToUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230720050816_AddNameDescriptionToUser', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230808194921_AddUnselectableFlag')
BEGIN
    ALTER TABLE [SelectableTreatment] ADD [IsUnselectable] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230808194921_AddUnselectableFlag')
BEGIN
    ALTER TABLE [ScenarioSelectableTreatment] ADD [IsUnselectable] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230808194921_AddUnselectableFlag')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230808194921_AddUnselectableFlag', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230814181410_AddingCommittedProjectSettingsTable')
BEGIN
    CREATE TABLE [CommittedProjectSettings] (
        [Key] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_CommittedProjectSettings] PRIMARY KEY ([Key])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230814181410_AddingCommittedProjectSettingsTable')
BEGIN
    CREATE UNIQUE INDEX [IX_CommittedProjectSettings_Key] ON [CommittedProjectSettings] ([Key]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230814181410_AddingCommittedProjectSettingsTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230814181410_AddingCommittedProjectSettingsTable', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230918053320_CommittedProjectTemplates')
BEGIN
    CREATE TABLE [CommittedProjectTemplates] (
        [Key] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_CommittedProjectTemplates] PRIMARY KEY ([Key])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230918053320_CommittedProjectTemplates')
BEGIN
    CREATE UNIQUE INDEX [IX_CommittedProjectTemplates_Key] ON [CommittedProjectTemplates] ([Key]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230918053320_CommittedProjectTemplates')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230918053320_CommittedProjectTemplates', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230926195745_AddNetworkIDToReportIndex')
BEGIN
    ALTER TABLE [ReportIndex] ADD [NetworkID] uniqueidentifier NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20230926195745_AddNetworkIDToReportIndex')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20230926195745_AddNetworkIDToReportIndex', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231003185812_AddProjectSourcesToCommittedProjects')
BEGIN
    ALTER TABLE [CommittedProject] ADD [ProjectSource] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231003185812_AddProjectSourcesToCommittedProjects')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231003185812_AddProjectSourcesToCommittedProjects', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    DROP TABLE [CriterionLibrary_TreatmentSupersession];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    DROP TABLE [TreatmentSupersession];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    DROP TABLE [CriterionLibrary_ScenarioTreatmentSupersession];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    DROP TABLE [ScenarioTreatmentSupersession];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE TABLE [ScenarioTreatmentSupersedeRule] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [PreventTreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ScenarioTreatmentSupersedeRule] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ScenarioTreatmentSupersedeRule_ScenarioSelectableTreatment_TreatmentId] FOREIGN KEY ([TreatmentId]) REFERENCES [ScenarioSelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE TABLE [TreatmentSupersedeRule] (
        [Id] uniqueidentifier NOT NULL,
        [TreatmentId] uniqueidentifier NOT NULL,
        [PreventTreatmentId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TreatmentSupersedeRule] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TreatmentSupersedeRule_SelectableTreatment_TreatmentId] FOREIGN KEY ([TreatmentId]) REFERENCES [SelectableTreatment] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE TABLE [CriterionLibrary_ScenarioTreatmentSupersedeRule] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [ScenarioTreatmentSupersedeRuleId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_ScenarioTreatmentSupersedeRule] PRIMARY KEY ([CriterionLibraryId], [ScenarioTreatmentSupersedeRuleId]),
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentSupersedeRule_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRuleId] FOREIGN KEY ([ScenarioTreatmentSupersedeRuleId]) REFERENCES [ScenarioTreatmentSupersedeRule] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE TABLE [CriterionLibrary_TreatmentSupersedeRule] (
        [CriterionLibraryId] uniqueidentifier NOT NULL,
        [TreatmentSupersedeRuleId] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [LastModifiedDate] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [LastModifiedBy] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_CriterionLibrary_TreatmentSupersedeRule] PRIMARY KEY ([CriterionLibraryId], [TreatmentSupersedeRuleId]),
        CONSTRAINT [FK_CriterionLibrary_TreatmentSupersedeRule_CriterionLibrary_CriterionLibraryId] FOREIGN KEY ([CriterionLibraryId]) REFERENCES [CriterionLibrary] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_CriterionLibrary_TreatmentSupersedeRule_TreatmentSupersedeRule_TreatmentSupersedeRuleId] FOREIGN KEY ([TreatmentSupersedeRuleId]) REFERENCES [TreatmentSupersedeRule] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_ScenarioTreatmentSupersedeRule_CriterionLibraryId] ON [CriterionLibrary_ScenarioTreatmentSupersedeRule] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_ScenarioTreatmentSupersedeRule_ScenarioTreatmentSupersedeRuleId] ON [CriterionLibrary_ScenarioTreatmentSupersedeRule] ([ScenarioTreatmentSupersedeRuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE INDEX [IX_CriterionLibrary_TreatmentSupersedeRule_CriterionLibraryId] ON [CriterionLibrary_TreatmentSupersedeRule] ([CriterionLibraryId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE UNIQUE INDEX [IX_CriterionLibrary_TreatmentSupersedeRule_TreatmentSupersedeRuleId] ON [CriterionLibrary_TreatmentSupersedeRule] ([TreatmentSupersedeRuleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE INDEX [IX_ScenarioTreatmentSupersedeRule_TreatmentId] ON [ScenarioTreatmentSupersedeRule] ([TreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    CREATE INDEX [IX_TreatmentSupersedeRule_TreatmentId] ON [TreatmentSupersedeRule] ([TreatmentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231031151842_AddUpdateTreatmentSupersedeRules')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231031151842_AddUpdateTreatmentSupersedeRules', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231117220510_RemoveColumnBudgetIdFromTableBudgetPercentagePair')
BEGIN
    Delete FROM dbo.[BudgetPercentagePair] WHERE NOT EXISTS(SELECT * FROM dbo.[ScenarioBudget] AS parent WHERE parent.Id = [BudgetPercentagePair].ScenarioBudgetId);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231117220510_RemoveColumnBudgetIdFromTableBudgetPercentagePair')
BEGIN
    Delete FROM dbo.[BudgetPercentagePair] WHERE NOT EXISTS(SELECT * FROM dbo.[ScenarioBudgetPriority] AS parent WHERE parent.Id = [BudgetPercentagePair].ScenarioBudgetPriorityId);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231117220510_RemoveColumnBudgetIdFromTableBudgetPercentagePair')
BEGIN
    ALTER TABLE [BudgetPercentagePair] NOCHECK CONSTRAINT all;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231117220510_RemoveColumnBudgetIdFromTableBudgetPercentagePair')
BEGIN
    ALTER TABLE [BudgetPercentagePair] DROP CONSTRAINT IF EXISTS [FK_BudgetPercentagePair_Budget_BudgetId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231117220510_RemoveColumnBudgetIdFromTableBudgetPercentagePair')
BEGIN
    DROP INDEX IF EXISTS [IX_BudgetPercentagePair_BudgetId] ON [BudgetPercentagePair]
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231117220510_RemoveColumnBudgetIdFromTableBudgetPercentagePair')
BEGIN
    ALTER TABLE [BudgetPercentagePair] DROP COLUMN IF EXISTS [BudgetId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231117220510_RemoveColumnBudgetIdFromTableBudgetPercentagePair')
BEGIN
    ALTER TABLE [BudgetPercentagePair] WITH CHECK CHECK CONSTRAINT all;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231117220510_RemoveColumnBudgetIdFromTableBudgetPercentagePair')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231117220510_RemoveColumnBudgetIdFromTableBudgetPercentagePair', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231206220510_RemoveColumnBudgetIdFromTableCommittedProject')
BEGIN
    ALTER TABLE [CommittedProject] NOCHECK CONSTRAINT all;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231206220510_RemoveColumnBudgetIdFromTableCommittedProject')
BEGIN
    DROP INDEX IF EXISTS [IX_CommittedProject_BudgetId] ON [CommittedProject]
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231206220510_RemoveColumnBudgetIdFromTableCommittedProject')
BEGIN
    ALTER TABLE [CommittedProject] DROP CONSTRAINT IF EXISTS [FK_CommittedProject_Budget_BudgetId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231206220510_RemoveColumnBudgetIdFromTableCommittedProject')
BEGIN
    ALTER TABLE [CommittedProject] DROP COLUMN IF EXISTS [BudgetId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231206220510_RemoveColumnBudgetIdFromTableCommittedProject')
BEGIN
    ALTER TABLE [CommittedProject] WITH CHECK CHECK CONSTRAINT all;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231206220510_RemoveColumnBudgetIdFromTableCommittedProject')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231206220510_RemoveColumnBudgetIdFromTableCommittedProject', N'6.0.1');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231213214806_AddDeleteSProcsandIndexes')
BEGIN
    EXEC('CREATE OR ALTER PROCEDURE dbo.usp_orphan_cleanup(@RetMessage VARCHAR(250) OUTPUT)
     	          AS
    	           BEGIN 
    	            DECLARE @CustomErrorMessage NVARCHAR(MAX),
    	           @ErrorNumber int,
    	           @ErrorSeverity int,
    	           @ErrorState int,
    	           @ErrorProcedure nvarchar(126),
    	           @ErrorLine int,
    	           @ErrorMessage nvarchar(4000);
    	           Set  @RetMessage = ''Success'';
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
    				        Set @RetMessage = ''Failed '' + @RetMessage;
    		        Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()
    		        Print ''Rolled Back Orphan SP:  '' + @ErrorMessage;
    		        ROLLBACK TRANSACTION;
    		        RAISERROR  (@RetMessage, 16, 1);  
    	          END CATCH
                END')
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231213214806_AddDeleteSProcsandIndexes')
BEGIN
    EXEC('CREATE OR ALTER PROCEDURE dbo.usp_delete_network(@NetworkId AS uniqueidentifier=NULL,@RetMessage VARCHAR(250) OUTPUT)
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
    	            Set  @RetMessage = ''Success'';
    	            DECLARE @CurrentDateTime DATETIME;
    	            DECLARE @BatchSize INT = 100000;
    	            DECLARE @RowsDeleted INT = 0;

                -----Start BenefitQuantifier Path-----------------------------------------

    			            --Network --> BenefitQuantifier

                            BEGIN TRY

                          ALTER TABLE BenefitQuantifier NOCHECK CONSTRAINT all

    		  	            Print ''BenefitQuantifier '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in Network --> BenefitQuantifier''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------End BenefitQuantifier------------------------------------------

    			            -----Start NetworkRollupDetail Path-----------------------------------------

    			            --Network --> NetworkRollupDetail

                           BEGIN TRY

    			            ALTER TABLE NetworkRollupDetail NOCHECK CONSTRAINT all

    			            Print ''NetworkRollupDetail '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in Network -->  NetworkRollupDetail''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------End NetworkRollupDetail------------------------------------------
    			            -----Start NetworkAttribute Path-----------------------------------------

    			            --Network --> NetworkAttribute

                            BEGIN TRY

                           ALTER TABLE NetworkAttribute NOCHECK CONSTRAINT all

    		               Print ''NetworkAttribute '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in Network --> NetworkAttribute''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------End NetworkAttribute------------------------------------------
    			            -----Start AnalysisMaintainableAsset Path-----------------------------------------

    			            --Network --> AnalysisMaintainableAsset

                            BEGIN TRY

                          ALTER TABLE AnalysisMaintainableAsset NOCHECK CONSTRAINT all

    		              Print ''AnalysisMaintainableAsset '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in Network --> AnalysisMaintainableAsset''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH

                   ------End AnalysisMaintainableAsset------------------------------------------
    	            ----- Start MaintainableAsset Path
    	            -----Start AggregatedResult Path-----------------------------------------

    			            --MaintainableAsset --> AggregatedResult

    	            BEGIN TRY

    		            Set @RowsDeleted = 1;

    		            ALTER TABLE AggregatedResult NOCHECK CONSTRAINT all;
    		            --ALTER INDEX ALL ON AggregatedResult DISABLE;
    		
    		            Print ''AggregatedResult '';

    				            WHILE @RowsDeleted > 0
    				            BEGIN
    					            BEGIN TRY
    						            --Begin Transaction
    					
    						            --Delete TOP (@BatchSize) l3
    						            SELECT TOP (@BatchSize) l3.Id  INTO #tempAggregatedResult
    						            FROM Network AS l1
    						            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    						            JOIN AggregatedResult AS l3 ON l3.MaintainableAssetId = l2.Id
    						            WHERE l1.Id IN (@NetworkId);
    						
    						            DELETE FROM AggregatedResult WHERE Id in (SELECT Id FROM #tempAggregatedResult);

    						            SET @RowsDeleted = @@ROWCOUNT;

    						            DROP TABLE #tempAggregatedResult;
    						            --WAITFOR DELAY ''00:00:01'';
    						
    						            --COMMIT TRANSACTION
    						
    						            Print ''Rows Affected Network --> MaintainableAsset-->AggregatedResult: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    							            ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all
      							            Set @RetMessage = ''Failed'';
    							            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							            --Print ''Rolled Back AggregatedResult Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
    							            --ROLLBACK TRANSACTION;
    							            RAISERROR  (@RetMessage, 16, 1); 
    							            Return -1;
    					            END CATCH;
    				            END

                            ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all
    					
    	            END TRY
    	            BEGIN CATCH
    		            ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all;
    		            --ALTER INDEX ALL ON AggregatedResult REBUILD;
    		            Print ''Query Error in  Network --> MaintainableAsset-->AggregatedResult ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network --> MaintainableAsset-->AggregatedResult''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;
    	            END CATCH;

                -----End AggregatedResult -----------------------------------------------------------------


                -------Start AttributeDatum Path-

                --MaintainableAsset --> AttributeDatum --> AttributeDatumLocation -->  -->  --> 

    	            BEGIN TRY

                          ALTER TABLE AttributeDatumLocation NOCHECK CONSTRAINT all
    		              SET @RowsDeleted = 1;
    		              Print ''AttributeDatumLocation '';

    		              WHILE @RowsDeleted > 0
    				            BEGIN
    					            BEGIN TRY
    						            --Begin Transaction

    							            SELECT TOP (@BatchSize) l4.Id  INTO #tempAttributeDatumLocation
    							            FROM Network AS l1
    							            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    							            JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
    							            Join AttributeDatumLocation As l4 ON l4.AttributeDatumId = l3.Id
    							            WHERE l1.Id IN (@NetworkId);

    						            DELETE FROM AttributeDatumLocation WHERE Id in (SELECT Id FROM #tempAttributeDatumLocation);

    						            SET @RowsDeleted = @@ROWCOUNT;

    						            DROP TABLE #tempAttributeDatumLocation;
    						            --WAITFOR DELAY ''00:00:01'';

    						            --COMMIT TRANSACTION
    						
    						            Print ''Rows Affected Network --> MaintainableAsset-->AttributeDatumLocation: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    							            ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all
      							            Set @RetMessage = ''Failed'';
    							            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							            --Print ''Rolled Back AttributeDatumLocation Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
    							            --ROLLBACK TRANSACTION;
    							            RAISERROR  (@RetMessage, 16, 1); 
    							            Return -1;
    					            END CATCH;
    				            END

                            ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    	            BEGIN CATCH
    			            ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all;
    			            Print ''Query Error in Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocationt ***Failed***'';
    			            SELECT ERROR_NUMBER() AS ErrorNumber
    			            ,ERROR_SEVERITY() AS ErrorSeverity
    			            ,ERROR_STATE() AS ErrorState
    			            ,ERROR_PROCEDURE() AS ErrorProcedure
    			            ,ERROR_LINE() AS ErrorLine
    			            ,ERROR_MESSAGE() AS ErrorMessage;

    			            SELECT @CustomErrorMessage = ''Query Error in  Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocation''
    			            RAISERROR (@CustomErrorMessage, 16, 1);
    			            Set @RetMessage = @CustomErrorMessage;
    	            END CATCH;



                --------End AttributeDatumLocation---------------------------------------------------------------

                --------MaintainableAsset --> AttributeDatum--
              
    	            BEGIN TRY

    			            ALTER TABLE AttributeDatum NOCHECK CONSTRAINT all
    			            SET @RowsDeleted = 1;
    			            Print ''AttributeDatum '';

    			            WHILE @RowsDeleted > 0
    				            BEGIN
    					            BEGIN TRY
    					            --Begin Transaction

    						            Select  TOP (@BatchSize) l3.Id  INTO #tempAttributeDatum
    						            FROM Network AS l1
    						            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    						            JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
    						            WHERE l1.Id IN (@NetworkId);

    						            DELETE FROM AttributeDatum WHERE Id in (SELECT Id FROM #tempAttributeDatum);

    						            SET @RowsDeleted = @@ROWCOUNT;

    						            DROP TABLE #tempAttributeDatum;
    						            --WAITFOR DELAY ''00:00:01'';

    						            --COMMIT TRANSACTION
    						
    						            Print ''Rows Affected Network --> MaintainableAsset-->AttributeDatum: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    						            ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all
    						            Set @RetMessage = ''Failed'';
    						            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    						            --Print ''Rolled Back AttributeDatum Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
    						            --ROLLBACK TRANSACTION;
    						            RAISERROR  (@RetMessage, 16, 1); 
    						            Return -1;
    					            END CATCH;
    				            END

                            ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    	            BEGIN CATCH
    		            ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all;
    		            --ALTER INDEX ALL ON AttributeDatum REBUILD
    		            Print ''Query Error in Network --> MaintainableAsset-->AttributeDatum ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network --> MaintainableAsset-->AttributeDatum''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;
    	            END CATCH

    	            ----------------------------------------------------------------------
    	            -----End AttributeDatum Path--------------------------------------

    	            -----Start AssetSummaryDetail Path-----------------------------------------
    	            --MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId -->  -->  --> 

    	            BEGIN TRY

    			            ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
    			            SET @RowsDeleted = 1;
    			            Print ''AssetSummaryDetailValueIntId '';

    			            WHILE @RowsDeleted > 0
    				            BEGIN
    					            BEGIN TRY
    					            --Begin Transaction

    						            Select TOP (@BatchSize) l4.Id  INTO #tempAssetSummaryDetailValueIntId
    						            FROM Network AS l1
    						            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    						            JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
    						            Join AssetSummaryDetailValueIntId As l4 ON l4.AssetSummaryDetailId = l3.Id
    						            WHERE l1.Id IN (@NetworkId);

    						            DELETE FROM AssetSummaryDetailValueIntId WHERE Id in (SELECT Id FROM #tempAssetSummaryDetailValueIntId);

    						            SET @RowsDeleted = @@ROWCOUNT;

    						            DROP TABLE #tempAssetSummaryDetailValueIntId;
    						            --WAITFOR DELAY ''00:00:01'';

    						            --COMMIT TRANSACTION
    						
    						            Print ''Rows Affected MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    						            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all
    						            Set @RetMessage = ''Failed'';
    						            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    						            --Print ''Rolled Back AssetSummaryDetailValueIntId Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
    						            --ROLLBACK TRANSACTION;
    						            RAISERROR  (@RetMessage, 16, 1); 
    						            Return -1;
    					            END CATCH;
    				            END

                            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all

    	            END TRY 
    	            BEGIN CATCH
    		            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
    		            --ALTER INDEX ALL ON AssetSummaryDetailValueIntId REBUILD;
    		            Print ''Query Error in Network --> MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network -->MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;

    	            END CATCH

    			            -----------------------------------------------------------------------
        		            -------MaintainableAsset --> AssetSummaryDetail -----

    	            BEGIN TRY

    		            ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all
    		            SET @RowsDeleted = 1;
    		    
    		            Print ''AssetSummaryDetail '';

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
    					
    		            --Print ''Rows Affected --MaintainableAsset --> AssetSummaryDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);
     
    		            END
    		
    		            ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all

    	            END TRY 
    	            BEGIN CATCH
    		            ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all;
    		            --ALTER INDEX ALL ON AssetSummaryDetail REBUILD;
    		            Print ''Query Error in Network --> MaintainableAsset --> AssetSummaryDetail ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network -->MaintainableAsset --> AssetSummaryDetail''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;
    	            END CATCH

    	            -----End AssetSummaryDetail Path--------------------------------------

                --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail -->  --> 
                ---Start BudgetUsageDetail --------------------------------------------------------------------

    		            BEGIN TRY

    		            --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail -->  --> 

                            ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all

    		              SET @RowsDeleted = 1;
    		              Print ''BudgetUsageDetail '';

    			            WHILE @RowsDeleted > 0
    				            BEGIN
    					            BEGIN TRY
    						            --Begin Transaction

    						            SELECT TOP (@BatchSize) l5.Id INTO #tempBudgetUsageDetail
    						            FROM Network AS l1
    						            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    						            Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
    						            JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
    						            JOIN BudgetUsageDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
    						            WHERE l1.Id IN (@NetworkId);

    						            DELETE FROM BudgetUsageDetail WHERE Id in (SELECT Id FROM #tempBudgetUsageDetail);

    						            SET @RowsDeleted = @@ROWCOUNT;

    						            DROP TABLE #tempBudgetUsageDetail;
    						            --WAITFOR DELAY ''00:00:01'';

    						            --COMMIT TRANSACTION
    						
    						            Print ''Rows Affected --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);

      					            END TRY
    					            BEGIN CATCH
    						            ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all
    						            Set @RetMessage = ''Failed'';
    						            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    						            --Print ''Rolled Back BudgetUsageDetail Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
    						            --ROLLBACK TRANSACTION;
    						            RAISERROR  (@RetMessage, 16, 1); 
    						            Return -1;
    					            END CATCH;
    				            END
      
     			            ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
    			 	            ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all
    				            Print ''Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail ***Failed***'';
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    	            ---End BudgetUsageDetail----------------------------------------------------------------------

    	            -- MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail -->  --> 
    	            ---Start CashFlowConsiderationDetail --------------------------------------------------------------------

    				            BEGIN TRY

                            ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all

    			              SET @RowsDeleted = 1;
    			              Print ''CashFlowConsiderationDetail '';

    				            WHILE @RowsDeleted > 0
    					            BEGIN
    						            BEGIN TRY
    						            --Begin Transaction

    						            SELECT TOP (@BatchSize) l5.Id INTO #tempCashFlowConsiderationDetail
    						            FROM Network AS l1
    						            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    						            Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
    						            JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
    						            JOIN CashFlowConsiderationDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
    						            WHERE l1.Id IN (@NetworkId);

    						            SET @RowsDeleted = @@ROWCOUNT;
    						            Print ''Rows Affected --MaintainableAsset --> AssetDetail --> CashFlowConsiderationDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);

    						            DELETE FROM CashFlowConsiderationDetail WHERE Id in (SELECT Id FROM #tempCashFlowConsiderationDetail);

    						            SET @RowsDeleted = @@ROWCOUNT;

    						            DROP TABLE #tempCashFlowConsiderationDetail;
    						            --WAITFOR DELAY ''00:00:01'';

    						            --COMMIT TRANSACTION
    						
    						            Print ''Rows Affected MaintainableAsset -->AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    						            ALTER TABLE CashFlowConsiderationDetail WITH CHECK CHECK CONSTRAINT all
    						            Set @RetMessage = ''Failed'';
    						            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    						            --Print ''Rolled Back CashFlowConsiderationDetail Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
    						            --ROLLBACK TRANSACTION;
    						            RAISERROR  (@RetMessage, 16, 1); 
    						            Return -1;
    					            END CATCH;
    				            END

                            ALTER TABLE CashFlowConsiderationDetail WITH CHECK CHECK CONSTRAINT all
    	            END TRY 
    	            BEGIN CATCH
                            ALTER TABLE CashFlowConsiderationDetail WITH CHECK CHECK CONSTRAINT all
    			            Print ''Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail--> CashFlowConsiderationDetail ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail--> CashFlowConsiderationDetail''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;
    	            END CATCH
    	
    	            ---End CashFlowConsiderationDetail --------------------------------------------------------------------
    	            ---Start TreatmentConsiderationDetail --------------------------------------------------------------------
    	
    		            --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail 

    			            BEGIN TRY

                            ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all

    		              SET @RowsDeleted = 1;
    		              Print ''TreatmentConsiderationDetail '';

    			            WHILE @RowsDeleted > 0
    				            BEGIN
    					            BEGIN TRY
    					            --Begin Transaction

    						            SELECT TOP (@BatchSize) l4.Id INTO #tempTreatmentConsiderationDetail
    						            --Delete l4
    						            FROM Network AS l1
    						            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    						            Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
    						            JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
    						            WHERE l1.Id IN (@NetworkId);

    						            DELETE FROM TreatmentConsiderationDetail WHERE Id in (SELECT Id FROM #tempTreatmentConsiderationDetail);

    						            SET @RowsDeleted = @@ROWCOUNT;

    						            DROP TABLE #tempTreatmentConsiderationDetail;
    						            --WAITFOR DELAY ''00:00:01'';

    						            --COMMIT TRANSACTION;
    						
    						            Print ''Rows Affected --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    						            ALTER TABLE TreatmentConsiderationDetail WITH CHECK CHECK CONSTRAINT all
    						            Set @RetMessage = ''Failed'';
    						            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    						            --Print ''Rolled Back TreatmentConsiderationDetail Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
    						            --ROLLBACK TRANSACTION;
    						            RAISERROR  (@RetMessage, 16, 1); 
    						            Return -1;
    					            END CATCH;
    				            END
    	

                            ALTER TABLE TreatmentConsiderationDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    	            BEGIN CATCH
                        ALTER TABLE TreatmentConsiderationDetail WITH CHECK CHECK CONSTRAINT all
    		            Print ''Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;
    	            END CATCH

    	            ---End TreatmentConsiderationDetail --------------------------------------------------------------------
    	            ---Start AssetDetailValueIntId --------------------------------------------------------------------

    	            -- Network --> MaintainableAsset --> AssetDetail --> AssetDetailValueIntId -

    		            BEGIN TRY

                            ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all;

    		              SET @RowsDeleted = 1;
    		              Print ''AssetDetailValueIntId '';

    			            WHILE @RowsDeleted > 0
    				            BEGIN
    					            BEGIN TRY
    					            --Begin Transaction

    						            SELECT TOP (@BatchSize) l4.Id INTO #tempAssetDetailValueIntId
    						            FROM Network AS l1
    						            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    						            Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
    						            JOIN AssetDetailValueIntId AS l4 ON l4.AssetDetailId = l3.Id
    						            WHERE l1.Id IN (@NetworkId);

    						            DELETE FROM AssetDetailValueIntId WHERE Id in (SELECT Id FROM #tempAssetDetailValueIntId);

    						            SET @RowsDeleted = @@ROWCOUNT;

    						            DROP TABLE #tempAssetDetailValueIntId;
    						            --WAITFOR DELAY ''00:00:01'';

    						            --COMMIT TRANSACTION
    						
    					            Print ''Rows Affected Network --> MaintainableAsset --> AssetDetail --> AssetDetailValueIntId: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    						            ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
    						            Set @RetMessage = ''Failed'';
    						            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    						            --Print ''Rolled Back AssetDetailValueIntId Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
    						            --ROLLBACK TRANSACTION;
    						            RAISERROR  (@RetMessage, 16, 1); 
    						            Return -1;
    					            END CATCH;
    				            END

                            ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    	            BEGIN CATCH
    		            ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
    		            --ALTER INDEX ALL ON AssetDetailValueIntId REBUILD;
    		            Print ''Query Error in Network --> MaintainableAsset --> AssetSummaryDetail --> AssetDetailValueIntId ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network -->MaintainableAsset --> AssetSummaryDetail --> AssetDetailValueIntId''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;

    	            END CATCH

    	            ---End AssetDetailValueIntId --------------------------------------------------------------------
    	            ---Start TreatmentOptionDetail --------------------------------------------------------------------

    	            -- MaintainableAsset --> AssetDetail --> TreatmentOptionDetail -->  -->  --> 

    	            BEGIN TRY

    		            ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

    		            SET @RowsDeleted = 1;
    		            Print ''TreatmentOptionDetail '';

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
    		            --WAITFOR DELAY ''00:00:01'';

    		            --Print ''Rows Affected MaintainableAsset --> AssetDetail --> TreatmentOptionDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);
    		            END
    		            ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all

    	            END TRY 
    	            BEGIN CATCH
    		            ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all;
    		            Print ''Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentOptionDetaild ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentOptionDetail''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;
    	            END CATCH


                ---End TreatmentOptionDetail --------------------------------------------------------------------
                ---Start TreatmentRejectionDetail --------------------------------------------------------------------
    	            -----------------------------------------------------------------

    		            -- AssetDetail --> TreatmentRejectionDetail 
    			
    		            BEGIN TRY

                            ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all
      		                Print ''TreatmentRejectionDetail '';

    			            WHILE @RowsDeleted > 0
    				            BEGIN
    					            BEGIN TRY
    					            --Begin Transaction

    				            --Delete l4 
    				            SELECT TOP (@BatchSize) l4.Id  INTO #tempTreatmentRejectionDet
    				            FROM Network AS l1
    				            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    				            Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
    				            JOIN TreatmentRejectionDetail AS l4 ON l4.AssetDetailId = l3.Id
    				            WHERE l1.Id IN (@NetworkId);

    				            DELETE FROM TreatmentRejectionDetail WHERE Id in (SELECT Id FROM #tempTreatmentRejectionDet);

    				            SET @RowsDeleted = @@ROWCOUNT;

    				            DROP TABLE #tempTreatmentRejectionDet;
    				            --WAITFOR DELAY ''00:00:01'';
    				
    				            --COMMIT TRANSACTION
    				
    				            Print ''Rows Affected Network --> MaintainableAsset-->AssetDetail --> TreatmentRejectionDetail : '' +  convert(NVARCHAR(50), @RowsDeleted);
                
    					            END TRY
    					            BEGIN CATCH
    						            ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all
    						            Set @RetMessage = ''Failed'';
    						            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    						            --Print ''Rolled Back Network --> MaintainableAsset-->AssetDetail --> TreatmentRejectionDetail  Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
    						            --ROLLBACK TRANSACTION;
    						            RAISERROR  (@RetMessage, 16, 1); 
    						            Return -1;
    					            END CATCH;
    				            END

                            ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all		
    		            END TRY 
    	            BEGIN CATCH
    		            ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all;
    		            Print ''Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentRejectionDetail ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentRejectionDetail''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;

    	            END CATCH

    	            ---End TreatmentRejectionDetail--------------------------------------------------------------

    	            -- AssetDetail --> TreatmentSchedulingCollisionDetail 

    	            BEGIN TRY

    		            ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all;

    		            SET @RowsDeleted = 1;
    		            Print ''TreatmentSchedulingCollisionDetail '';

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
    		            --WAITFOR DELAY ''00:00:01'';
    						
    		            --Print ''Rows Affected Network --> MaintainableAsset-->AssetDetail --> TreatmentSchedulingCollisionDetail : '' +  convert(NVARCHAR(50), @RowsDeleted);
    		            ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all;
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

    		            SELECT @CustomErrorMessage = ''Query Error in Network --> MaintainableAsset-->AssetDetail -->  TreatmentSchedulingCollisionDetail''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;

    	            END CATCH

    	            -----------------------------------------------------------------
    	
    			            --MaintainableAsset\AssetDetail

    	            BEGIN TRY

    		            ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

    		            SET @RowsDeleted = 1;
    		            Print ''AssetDetail '';

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
    		            ----WAITFOR DELAY ''00:00:01'';
    				
    		            --Print ''Rows Affected Network --> MaintainableAsset-->AssetDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);
    		            END

    		            ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all

    	            END TRY
    	            BEGIN CATCH
    		            ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all;
    		            Print ''Query Error in Network --> MaintainableAsset --> AssetDetail ***Failed***'';
    		            SELECT ERROR_NUMBER() AS ErrorNumber
    		            ,ERROR_SEVERITY() AS ErrorSeverity
    		            ,ERROR_STATE() AS ErrorState
    		            ,ERROR_PROCEDURE() AS ErrorProcedure
    		            ,ERROR_LINE() AS ErrorLine
    		            ,ERROR_MESSAGE() AS ErrorMessage;

    		            SELECT @CustomErrorMessage = ''Query Error in  Network -->MaintainableAsset --> AssetDetail ''
    		            RAISERROR (@CustomErrorMessage, 16, 1);
    		            Set @RetMessage = @CustomErrorMessage;

    	            END CATCH


    		            ---End  --Network -->MaintainableAsset --> AssetDetail Path---------
    	              -----Start CommittedProject Path-----------------------------------------
    			
    		            --Network --> MaintainableAsset --> CommittedProject --> CommittedProjectConsequence -->  -->  -->  -->  -->  --> 

                         BEGIN TRY

                            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

    			            Print ''CommittedProjectConsequence '';

    			            Delete l4 
    			            FROM Network AS l1
    			            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    			            Join CommittedProject  AS l3 ON l3.MaintainableAssetEntityId = l2.Id
    			            JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l3.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network Delete CommittedProjectConsequence: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
     	
                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectConsequence''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -------------------------------------

    			            --Network --> MaintainableAsset --> CommittedProject --> CommittedProjectLocation -->  -->  -->  -->  -->  -->

    			             BEGIN TRY

                            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

    			            Print ''CommittedProjectLocation '';

    			            Delete l4 
    			            FROM Network AS l1
    			            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    			            Join CommittedProject  AS l3 ON l3.MaintainableAssetEntityId = l2.Id
    			            JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l3.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network Delete CommittedProjectLocation: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
     	
                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectLocation''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            --------------------------------------

    			            --Network --> MaintainableAsset --> CommittedProject 

                            BEGIN TRY

    			            ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

    			            Print ''CommittedProject '';

    			            Delete l3 
    			            FROM Network AS l1
    			            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    			            Join CommittedProject AS l3 ON l3.MaintainableAssetEntityId = l2.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network Delete CommittedProject: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE CommittedProject WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProject''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH 

    			            -------------------------------------


    			            -----End CommittedProject Path-----------------------------------------

    		            ----Start Network --> MaintainableAsset --> MaintainableAssetLocation  Path
    			
    			            BEGIN TRY

                            ALTER TABLE MaintainableAssetLocation NOCHECK CONSTRAINT all

    		  	            Print ''MaintainableAssetLocation '';

    			            Delete l3
    			            FROM Network AS l1
    			            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    			            JOIN MaintainableAssetLocation AS l3 ON l3.MaintainableAssetId = l2.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network --> MaintainableAsset-->MaintainableAssetLocation: '' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE MaintainableAssetLocation WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in Network --> MaintainableAsset-->MaintainableAssetLocation''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    	            -------End -------MaintainableAssetLocation----------------------------------------------------

    			            -- Start MaintainableAsset

     			            BEGIN TRY

                            ALTER TABLE MaintainableAsset NOCHECK CONSTRAINT all

    			            Print ''MaintainableAsset'';

    			            Delete l2 
    			            FROM Network AS l1
    			            Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network --> MaintainableAsset: '' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE MaintainableAsset WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in Network --> MaintainableAsset''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------Network --> MaintainableAsset Path--------------------------------------------------------------------
    					   			 
    		            ----- Start Simulation Path
    			
    		                --SET @CurrentDateTime = GETDATE();
    			            --PRINT ''Start Simulation Delete: '' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
    			            -----Start AnalysisMethod Path-----------------------------------------

                            BEGIN TRY
     
                            ALTER TABLE Benefit NOCHECK CONSTRAINT all

    			            Print ''Benefit '';
    			            --AnalysisMethod	Benefit							FK_Benefit_AnalysisMethod_AnalysisMethodId
    			            --AnalysisMethod	CriterionLibrary_AnalysisMethod	FK_CriterionLibrary_AnalysisMethod_AnalysisMethod_AnalysisMethodId

    			            Delete l4 
    			            FROM Network AS l1
    			            JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
    			            JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
    			            JOIN Benefit AS l4 ON l4.AnalysisMethodId = l3.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network --> Benefit: '' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE Benefit WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in Benefit''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -----------------------------------------------------------------------

                            BEGIN TRY


                            ALTER TABLE CriterionLibrary_AnalysisMethod NOCHECK CONSTRAINT all

    			            Print ''CriterionLibrary_AnalysisMethod '';

    			            Delete l4 
    			            FROM Network AS l1
    			            JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
    			            JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
    			            JOIN CriterionLibrary_AnalysisMethod AS l4 ON l4.AnalysisMethodId = l3.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network --> CriterionLibrary_AnalysisMethod: '' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE CriterionLibrary_AnalysisMethod WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_AnalysisMethod''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    			            ------------------------------------------------------------------

                            BEGIN TRY

                            ALTER TABLE AnalysisMethod NOCHECK CONSTRAINT all

    			            Print ''AnalysisMethod '';

    			            Delete l3 
    			            FROM Network AS l1
    			            JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
    			            JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network --> AnalysisMethod: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE AnalysisMethod WITH CHECK CHECK CONSTRAINT all
     
                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in AnalysisMethod''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
     
     			            -----End Simulation --> AnalysisMethod Path-----------------------------------------
     			            -----------------------------------------------------------------------
    			            -----Start Simulation --> CommittedProject Path-----------------------------------------

    			            --Simulation --> CommittedProject --> CommittedProjectConsequence 

                         BEGIN TRY

                            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

    			            Print ''CommittedProjectConsequence '';

    			            Delete l4 
    			            FROM Network AS l1
    			            JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
    			            JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
    			            JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l3.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network --> CommittedProjectConsequence: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
     	
                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectConsequence''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -------------------------------------

    			            --Simulation --> CommittedProject --> CommittedProjectLocation 

    			             BEGIN TRY

                            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

    			            Print ''CommittedProjectLocation '';

    			            Delete l4 
    			            FROM Network AS l1
    			            JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
    			            JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
    			            JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l3.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network --> CommittedProjectLocation: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
     	
                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectLocation''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            --------------------------------------

    			            --Simulation --> CommittedProject

                            BEGIN TRY

    			            ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

    			            Print ''CommittedProject '';

    			            Delete l3 
    			            FROM Network AS l1
    			            JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
    			            JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
    			            WHERE l1.Id IN (@NetworkId)

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected Network --> CommittedProject: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE CommittedProject WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProject''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH 


    			            -----End CommittedProject Path-----------------------------------------
    		               -----Start Simulation --> InvestmentPlan 


                            BEGIN TRY

    			            ALTER TABLE InvestmentPlan NOCHECK CONSTRAINT all

    			            Print ''InvestmentPlan'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in InvestmentPlan''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -----End Simulation --> InvestmentPlan 

    		            -----Start Simulation --> ReportIndex 


                            BEGIN TRY
    			            Print ''ReportIndex'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ReportIndex''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -----End Simulation --> ReportIndex ---------------------------
    			            -- Start Network --> Simulation --> ScenarioBudget --> BudgetPercentagePair -->

                            BEGIN TRY

    			            ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

    			            Print ''BudgetPercentagePair'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetPercentagePair''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -- End Simulation --> ScenarioBudget --> BudgetPercentagePair -->

    			            -- Start Simulation --> ScenarioBudget --> CommittedProjectConsequence -->

                            BEGIN TRY

                            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

    			            Print ''CommittedProjectConsequence'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectConsequence''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -------------------------------------

    			            --Simulation --> ScenarioBudget --> CommittedProject --> CommittedProjectLocation

    			             BEGIN TRY

                            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

    			            Print ''CommittedProjectLocation '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectLocation''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            --------------------------------------

    		            --Simulation --> ScenarioBudget --> CommittedProject

                            BEGIN TRY

    			            ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

    			            Print ''CommittedProject '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProject''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH 

    			            -----End ScenarioBudget -CommittedProject Path------------

    		            -----Start Network --> Simulation --> ScenarioBudget --> CriterionLibrary_ScenarioBudget

                            BEGIN TRY

    			            ALTER TABLE CriterionLibrary_ScenarioBudget NOCHECK CONSTRAINT all

    			            Print ''CriterionLibrary_ScenarioBudget '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioBudget''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    			            -----------------------------------------

    			            --Network --> Simulation --> ScenarioBudget --> ScenarioBudgetAmount --> 

                            BEGIN TRY

    			            ALTER TABLE ScenarioBudgetAmount NOCHECK CONSTRAINT all

    			            Print ''ScenarioBudgetAmount'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioBudgetAmount''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ----------------------------------------------

    		            --Network --> Simulation --> ScenarioBudget --> ScenarioSelectableTreatment_ScenarioBudget --> 

                            BEGIN TRY

    			            Print ''ScenarioSelectableTreatment_ScenarioBudget'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioSelectableTreatment_ScenarioBudget''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
      
    			            ------------------------------------------------------------

    			            --Network --> Simulation --> ScenarioBudget 

                            BEGIN TRY

    			            Print ''ScenarioBudget'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioBudget''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -----End --Network --> Simulation --> ScenarioBudget Path-----------------------------------------

    			            ------Start  --Network --> Simulation --> ScenarioBudgetPriority -----------

    			            --Network --> Simulation --> ScenarioBudgetPriority --> CriterionLibrary_ScenarioBudgetPriority --> 

                            BEGIN TRY

    			            ALTER TABLE CriterionLibrary_ScenarioBudgetPriority NOCHECK CONSTRAINT all

    			            Print ''CriterionLibrary_ScenarioBudgetPriority '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioBudgetPriority''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------------------------------------------------------------

    			            --Network --> Simulation --> ScenarioBudgetPriority --> BudgetPercentagePair --> 

                            BEGIN TRY

    			            Print ''BudgetPercentagePair'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetPercentagePair''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------------------------------------------------------------

    			            --Network --> Simulation --> ScenarioBudgetPriority

                            BEGIN TRY

    			            ALTER TABLE ScenarioBudgetPriority NOCHECK CONSTRAINT all

    			            Print ''ScenarioBudgetPriority '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioBudgetPriority''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------End Network -->  Simulation --> ScenarioBudgetPriority-------------------------------------------------------------------

    			            ------Start  ScenarioCalculatedAttribute-------------------------------------------------------------------
    			            --Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair --> ScenarioCalculatedAttributePair_Criteria
    			            --Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair --> ScenarioCalculatedAttributePair_Equation


                            BEGIN TRY

    			            ALTER TABLE ScenarioCalculatedAttributePair_Criteria NOCHECK CONSTRAINT all

    			            Print ''ScenarioCalculatedAttributePair_Criteria'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCalculatedAttributePair_Criteria''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------------------------------------------------------------

    			            --Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair --> ScenarioCalculatedAttributePair_Equation

                            BEGIN TRY

    			            ALTER TABLE ScenarioCalculatedAttributePair_Equation NOCHECK CONSTRAINT all

    			            Print ''ScenarioCalculatedAttributePair_Equation'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCalculatedAttributePair_Equation''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------------------------------------------------------------
    			            --Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair 

    			            BEGIN TRY

    			            ALTER TABLE ScenarioCalculatedAttributePair NOCHECK CONSTRAINT all

    			            Print ''ScenarioCalculatedAttributePair'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCalculatedAttributePair''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------
    			            --Network --> Simulation --> ScenarioCalculatedAttribute 

                            BEGIN TRY

    			            Print ''ScenarioCalculatedAttribute '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCalculatedAttribute''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ----End --Network --> Simulation --> ScenarioCalculatedAttribute-------------------------------------------------------------------

    			            ------Start  Network --> Simulation --> ScenarioCashFlowRule-------------------------------------------------------------------
    			            ----Network --> Simulation --> ScenarioCashFlowRule --> CriterionLibrary_ScenarioCashFlowRule --> 

                            BEGIN TRY

    			            ALTER TABLE CriterionLibrary_ScenarioCashFlowRule NOCHECK CONSTRAINT all

    			            Print ''CriterionLibrary_ScenarioCashFlowRule '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioCashFlowRule''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------------------------------------------------------------

    			            --Network --> Simulation --> ScenarioCashFlowRule --> ScenarioCashFlowDistributionRule --> 

    			            BEGIN TRY

    			            ALTER TABLE ScenarioCashFlowDistributionRule NOCHECK CONSTRAINT all

    			            Print ''ScenarioCashFlowDistributionRule '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCashFlowDistributionRule''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------
    			            --Network --> Simulation --> ScenarioCashFlowRule 

                            BEGIN TRY

    			            ALTER TABLE ScenarioCashFlowRule NOCHECK CONSTRAINT all

    			            Print ''ScenarioCashFlowRule '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCashFlowRule''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------End ScenarioCashFlowRule-------------------------------------------------------------------
    			            ------Start  ScenarioDeficientConditionGoal-------------------------------------------------------------------
    			            --Network --> Simulation --> ScenarioDeficientConditionGoal --> CriterionLibrary_ScenarioDeficientConditionGoal --> 

                            BEGIN TRY

    			            Print ''CriterionLibrary_ScenarioDeficientConditionGoal'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioDeficientConditionGoal''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------------------------------------------------------------

    			            --Network --> Simulation --> ScenarioDeficientConditionGoal 

                            BEGIN TRY

    			            ALTER TABLE ScenarioDeficientConditionGoal NOCHECK CONSTRAINT all

    			            Print ''ScenarioDeficientConditionGoal '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioDeficientConditionGoal''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------End ScenarioDeficientConditionGoal-------------------------------------------------------------------
    			            ------Start  ScenarioPerformanceCurve-------------------------------------------------------------------
    			            --Network --> Simulation --> ScenarioPerformanceCurve --> CriterionLibrary_ScenarioPerformanceCurve --> 
    			            --Network --> Simulation --> ScenarioPerformanceCurve --> ScenarioPerformanceCurve_Equation --> 

                            BEGIN TRY

    			            Print ''CriterionLibrary_ScenarioPerformanceCurve'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioPerformanceCurve''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------------------------------------------------------------
    			            --Network --> Simulation --> ScenarioPerformanceCurve --> ScenarioPerformanceCurve_Equation --> 

                            BEGIN TRY

    			            ALTER TABLE ScenarioPerformanceCurve_Equation NOCHECK CONSTRAINT all

    			            Print ''ScenarioPerformanceCurve_Equation '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioPerformanceCurve_Equation''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

    			            --Network --> Simulation --> ScenarioPerformanceCurve

                            BEGIN TRY

    			            Print ''ScenarioPerformanceCurve'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioPerformanceCurve''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------End ScenarioPerformanceCurve-------------------------------------------------------------------
    			            ------Start ScenarioRemainingLifeLimit-------------------------------------------------------------------

    			            --Network --> Simulation --> ScenarioRemainingLifeLimit --> CriterionLibrary_ScenarioRemainingLifeLimit --> 

                            BEGIN TRY

    			            ALTER TABLE CriterionLibrary_ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

    			            Print ''CriterionLibrary_ScenarioRemainingLifeLimit'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioRemainingLifeLimit''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

    			            --Network --> Simulation --> ScenarioRemainingLifeLimit 

                            BEGIN TRY

    			            ALTER TABLE ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

    			            Print ''ScenarioRemainingLifeLimit'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioRemainingLifeLimit''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------End ScenarioRemainingLifeLimit----------------------------------------------------------
    			            ------Start ScenarioSelectableTreatment--------------------------------------------------------

     
                  --Network --> Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersedeRule --> CriterionLibrary_ScenarioTreatmentSupersedeRule --> 

    			            BEGIN TRY

    			            ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersedeRule NOCHECK CONSTRAINT all

    			            Print ''CriterionLibrary_ScenarioTreatmentSupersedeRule'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTreatmentSupersedeRule''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

                   --Network --> Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersedeRule  

                         BEGIN TRY

    			            ALTER TABLE ScenarioTreatmentSupersedeRule NOCHECK CONSTRAINT all

    			            Print ''ScenarioTreatmentSupersedeRule '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentSupersedeRule''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

    		            --Simulation --> ScenarioSelectableTreatment --> ScenarioConditionalTreatmentConsequences   --> CriterionLibrary_ScenarioTreatmentConsequence

                            BEGIN TRY

    			            Print ''CriterionLibrary_ScenarioTreatmentConsequence'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTreatmentConsequence''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    		
    		            ---------------------------------------------------------------------------

    		            --Simulation --> ScenarioSelectableTreatment --> ScenarioConditionalTreatmentConsequences   --> ScenarioTreatmentConsequence_Equation

                            BEGIN TRY

    			            --ALTER TABLE ScenarioTreatmentConsequence_Equation NOCHECK CONSTRAINT all

    			            Print ''ScenarioTreatmentConsequence_Equation'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentConsequence_Equation''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------
    			
    			            --Simulation --> ScenarioSelectableTreatment --> ScenarioConditionalTreatmentConsequences

                            BEGIN TRY

    			            ALTER TABLE ScenarioConditionalTreatmentConsequences NOCHECK CONSTRAINT all

    			            Print ''ScenarioConditionalTreatmentConsequences'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioConditionalTreatmentConsequences''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    	  	            --------------------------------------------------------------------------

    			            --Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentCost --> ScenarioTreatmentCost_Equation

                            BEGIN TRY

    			            --ALTER TABLE ScenarioTreatmentCost_Equation NOCHECK CONSTRAINT all

    			            Print ''ScenarioTreatmentCost_Equation'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentCost_Equation''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH


    		            ---------------------------------------------------------------------------

    		            --Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentCost --> CriterionLibrary_ScenarioTreatmentCost

                            BEGIN TRY

    			            Print ''CriterionLibrary_ScenarioTreatmentCost '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTreatmentCost''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------
    		            --simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentCost

                            BEGIN TRY

    			            ALTER TABLE ScenarioTreatmentCost NOCHECK CONSTRAINT all

    			            Print ''ScenarioTreatmentCost '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentCost''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH


    		            ---------------------------------------------------------------------------

    		            --Simulation --> ScenarioSelectableTreatment --> ScenarioSelectableTreatment_ScenarioBudget --> 

                            BEGIN TRY

    			            ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget NOCHECK CONSTRAINT all

    			            Print ''ScenarioSelectableTreatment_ScenarioBudget '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioSelectableTreatment_ScenarioBudget''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

    			            --Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentPerformanceFactor --> 

                            BEGIN TRY

    			            Print ''ScenarioTreatmentPerformanceFactor'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentPerformanceFactor''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------
    			            --Simulation --> ScenarioSelectableTreatment --> CriterionLibrary_ScenarioTreatment --> 

                            BEGIN TRY

    			            ALTER TABLE CriterionLibrary_ScenarioTreatment NOCHECK CONSTRAINT all

    			            Print ''CriterionLibrary_ScenarioTreatment '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTreatment''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

    			            --Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentScheduling -->

                            BEGIN TRY

    			            ALTER TABLE ScenarioTreatmentScheduling NOCHECK CONSTRAINT all

    			            Print ''ScenarioTreatmentScheduling '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentScheduling''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

    			            --Simulation --> ScenarioSelectableTreatment 15

                            BEGIN TRY

    			            ALTER TABLE ScenarioSelectableTreatment NOCHECK CONSTRAINT all

    			            Print ''ScenarioSelectableTreatment '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioSelectableTreatment''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---End ScenarioSelectableTreatment --------------------------------------------
    			            ---------------------------------------------------------------------------
    			            ---Start ScenarioTargetConditionGoals --------------------------------------------

    			            --Simulation --> ScenarioTargetConditionGoals --> CriterionLibrary_ScenarioTargetConditionGoal --> 

                            BEGIN TRY

    			            Print ''CriterionLibrary_ScenarioTargetConditionGoal'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTargetConditionGoal''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

    		            --Simulation --> ScenarioTargetConditionGoals 

                            BEGIN TRY

    			            ALTER TABLE ScenarioTargetConditionGoals NOCHECK CONSTRAINT all

    			            Print ''ScenarioTargetConditionGoals '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTargetConditionGoals''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---End ScenarioTargetConditionGoals --------------------------------------------
    			            ---------------------------------------------------------------------------
    		            --Simulation --> Simulation_User 

                            BEGIN TRY

    			            ALTER TABLE Simulation_User NOCHECK CONSTRAINT all

    			            Print ''Simulation_User '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in Simulation_User''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

    		            --Simulation --> SimulationAnalysisDetail 

                            BEGIN TRY

    			            Print ''SimulationAnalysisDetail'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationAnalysisDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

                            BEGIN TRY

    			            Print ''SimulationLog'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationLog''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -------End SimulationLog-----------------------------------------
    			
    			            ----Start SimulationOutput Path---------------------------------------

    			            -----Start AssetSummaryDetail Path-----------------------------------------

    			            -------SimulationOutput --> AssetSummaryDetail --> AssetSummaryDetailValueIntI----
    		            --SET @CurrentDateTime = GETDATE();
    		            --PRINT ''Start SimulationOutput Delete: '' + CONVERT(NVARCHAR, @CurrentDateTime, 120);

                        BEGIN TRY

                          ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
    		              SET @RowsDeleted = 1;
    		              Print ''AssetSummaryDetailValueIntId '';

    		               WHILE @RowsDeleted > 0
    				            BEGIN

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
    				            --WAITFOR DELAY ''00:00:01'';
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetSummaryDetailValueIntId''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            -----------------------------------------------------------------

        		            -------SimulationOutput --> AssetSummaryDetail -----

                            BEGIN TRY

    			            Print ''AssetSummaryDetail'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetSummaryDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH


    			            ----------------------------------------------------------------------
    			            -----End AssetSummaryDetail Path--------------------------------------

    			            -----Start SimulationYearDetail Path-----------------------------------------

    			            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail 

    			            BEGIN TRY

    			            Print ''BudgetUsageDetail2'';

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
    			            --Print ''Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetUsageDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    		            --Print ''*******BudgetUsageDetail**************''
    	
    	            ------------------------------------------------------------------
        		            -------SimulationOutput --> AssetSummaryDetail -----

                            BEGIN TRY

    			            Print ''AssetSummaryDetail'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetSummaryDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH


    			            ----------------------------------------------------------------------
    			            -----End AssetSummaryDetail Path--------------------------------------

    			            -----Start SimulationYearDetail Path-----------------------------------------

    			            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail 

    			            BEGIN TRY

    			            Print ''BudgetUsageDetail'';

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
    			            --Print ''Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetUsageDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    		            --Print ''*******BudgetUsageDetail**************''
    	
    	            ------------------------------------------------------------------

    				            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail

    			            BEGIN TRY

    			            Print ''CashFlowConsiderationDetail'';

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
    			            --Print ''Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE CashFlowConsiderationDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in CashFlowConsiderationDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    	            ------------------------------------------------------------------
    	
    		            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail 

    			            BEGIN TRY

    			            Print ''TreatmentConsiderationDetail'';

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
    			            --Print ''Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE TreatmentConsiderationDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentConsiderationDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    	            ------------------------------------------------------------------


                --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentOptionDetail -->  -->  -->  -->  -->  --> 


    			            BEGIN TRY

    			            Print ''TreatmentOptionDetail'';

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
    			            --Print ''Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentOptionDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);
    			
                            ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentOptionDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    		            -----------------------------------------------------------------
    		            ---------------------------------------------------------------
    			            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId -->  -->  -->  -->  -->  --> 


    			            BEGIN TRY

    			            Print ''AssetDetailValueIntId'';

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
    					
    						            --Print ''Rows Affected Network --> --SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId: '' +  convert(NVARCHAR(50), @RowsDeleted);
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetDetailValueIntId''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    		            -----------------------------------------------------------------

    		            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail -->  -->  -->  -->  -->  --> 

    			            BEGIN TRY

    			            Print ''TreatmentRejectionDetail'';

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
    						            --WAITFOR DELAY ''00:00:01'';
    						
    						            --Print ''Rows Affected Network --> --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentRejectionDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentRejectionDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    	            ----End TreatmentRejectionDetail-------------------------------------------------------------
    	            -----------------------------------------------------------------
    	            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail 


    			            BEGIN TRY

    			            Print ''TreatmentSchedulingCollisionDetail'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentSchedulingCollisionDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    	            -----------------------------------------------------------------
    	
    			            --SimulationOutput\SimulationYearDetail\AssetDetail
    			
    			            BEGIN TRY

    			            Print ''Simulation\SimulationOutput\SimulationYearDetail\AssetDetail'';

                            ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

    			            Delete l5
    			            FROM Network AS l1
    			            JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
    			            JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
    			            JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
    			            JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected  Network ->  Simulation ->  SimulationOutput --> SimulationYearDetail-->AssetDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);

                            ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all

                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    	            -----------------------------------------------------------------


    			            --SimulationOutput\SimulationYearDetail\BudgetDetail
    			
    			            BEGIN TRY

    			            Print ''SimulationOutput\SimulationYearDetail\BudgetDetailBudgetDetail'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in DeficientConditionGoalDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    	            ------------------------------------------------------------------
    		            --SimulationOutput\SimulationYearDetail\TargetConditionGoalDetail
    			
    			            BEGIN TRY

    			            Print ''TargetConditionGoalDetail'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in TargetConditionGoalDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    	            ------------------------------------------------------------------

    			            --SimulationOutput\SimulationYearDetail
    			
    			            BEGIN TRY

    			            Print ''SimulationYearDetail '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationYearDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ------------------------------------------------------------------
    			            --SimulationOutputJson Delete records where SimulationOutput is the parent

                            BEGIN TRY

    			            ALTER TABLE SimulationOutputJson NOCHECK CONSTRAINT all

    			            Print ''SimulationOutputJson '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationOutputJson''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------
    			            --SimulationOutput

                            BEGIN TRY

                            ALTER TABLE SimulationOutput NOCHECK CONSTRAINT all

    			            Print ''SimulationOutput '';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationOutput''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    		               ---------------------------------------------------------------------------
    		            ----End SimulationOutput Delete----------------------------------------
    		                --SET @CurrentDateTime = GETDATE();
    			            --PRINT ''End SimulationOutput Delete: '' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
     
    			            --SimulationOutputJson Delete records where Simulation is the parent

                            BEGIN TRY

    			            Print ''SimulationOutputJson'';

    			            ALTER TABLE SimulationOutputJson NOCHECK CONSTRAINT all

    			            Delete l3 
    			            FROM Network AS l1
    			            JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
    			            JOIN SimulationOutputJson AS l3 ON l3.SimulationId = l2.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected  Network ->  Simulation ->  SimulationOutputJson: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE SimulationOutputJson WITH CHECK CHECK CONSTRAINT all
     
                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationOutputJson''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

    			            --SimulationReportDetail

                            BEGIN TRY

    			            Print ''SimulationReportDetail'';

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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationReportDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------
    			            --Simulation

                            BEGIN TRY

    			            Print ''Simulation'';

    			            ALTER TABLE Simulation NOCHECK CONSTRAINT all

    			            Delete l2 
    			            FROM Network AS l1
    			            JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected  Network --> Simulation: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE Simulation WITH CHECK CHECK CONSTRAINT all;
     
                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in Simulation''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    		
    		             --   SET @CurrentDateTime = GETDATE();
    			            --PRINT ''End Simulation Delete: '' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
    			            ------End Simulation Delete-------------------------------------------------------------------
    			            -----End  ----Network --> Simulation Path---------

    			            -- Start Network

                            BEGIN TRY

    			            Print ''Network'';

                            ALTER TABLE Network NOCHECK CONSTRAINT all

    			            Delete l1 
    			            FROM  Network AS l1
    			            WHERE l1.Id IN (@NetworkId);

    			            SET @RowsDeleted = @@ROWCOUNT;
    			            --Print ''Rows Affected  Network: '' +  convert(NVARCHAR(50), @RowsDeleted);

    			            ALTER TABLE Network WITH CHECK CHECK CONSTRAINT all
     
                            END TRY 
    			            BEGIN CATCH
                                 SELECT ERROR_NUMBER() AS ErrorNumber
                                       ,ERROR_SEVERITY() AS ErrorSeverity
                                       ,ERROR_STATE() AS ErrorState
                                       ,ERROR_PROCEDURE() AS ErrorProcedure
                                       ,ERROR_LINE() AS ErrorLine
                                       ,ERROR_MESSAGE() AS ErrorMessage;

     		                     SELECT @CustomErrorMessage = ''Query Error in Network''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    		               ---------------------------------------------------------------------------
    			            ------End Network Delete-------------------------------------------------------------------
                    Print ''Delete Network Committed End'';
     	            RAISERROR (@RetMessage, 0, 1);
    	            END TRY
    	            BEGIN CATCH
      			            Set @RetMessage = ''Failed '' + @RetMessage;
    			            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    			            Print ''Overall Catch in Network SP:  '' + @ErrorMessage;

    			            RAISERROR  (@RetMessage, 16, 1);  
    	            END CATCH;

                END')
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231213214806_AddDeleteSProcsandIndexes')
BEGIN
    EXEC('CREATE OR ALTER PROCEDURE dbo.usp_delete_simulation(@SimGuidList NVARCHAR(MAX)=NULL,@RetMessage VARCHAR(250)=NULL OUTPUT)
    	          AS
    	          BEGIN 
     	            DECLARE @CustomErrorMessage NVARCHAR(MAX),
    	            @ErrorNumber int,
    	            @ErrorSeverity int,
    	            @ErrorState int,
    	            @ErrorProcedure nvarchar(126),
    	            @ErrorLine int,
    	            @ErrorMessage nvarchar(4000);
    	            Set  @RetMessage = ''Success'';
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
    		              PRINT ''String is NULL or empty'';
    		              Set  @SimGuidList = ''00000000-0000-0000-0000-000000000000'';
    	            END

                    INSERT INTO #SimTempGuids (Guid)
    	            SELECT LEFT(LTRIM(RTRIM(value)), 36)
                    FROM STRING_SPLIT(@SimGuidList, '','');

    	            --Select *, '''' as ''aaa'' from #SimTempGuids;

    	            UPDATE #SimTempGuids
    	            SET Guid = ''00000000-0000-0000-0000-000000000000''
    	            WHERE TRY_CAST(Guid AS UNIQUEIDENTIFIER) IS NULL OR Guid = '''';
    	
    	            --Select *, '''' as ''bbb'' from #SimTempGuids;

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

     		                     SELECT @CustomErrorMessage = ''Query Error in Benefit''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_AnalysisMethod''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AnalysisMethod''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectConsequence''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectLocation''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProject''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectConsequence''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CommittedProjectLocation''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioBudgetAmount''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetPercentagePair''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioBudget''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioBudgetPriority''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetPercentagePair''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioBudgetPriority''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCalculatedAttributePair_Criteria''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCalculatedAttributePair_Equation''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCalculatedAttributePair''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCalculatedAttribute''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioCashFlowRule''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCashFlowDistributionRule''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioCashFlowRule''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioDeficientConditionGoal''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioDeficientConditionGoal''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioPerformanceCurve''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioPerformanceCurve_Equation''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioPerformanceCurve''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioRemainingLifeLimit''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioRemainingLifeLimit''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTreatmentSupersedeRule''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentSupersedeRule''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTreatmentConsequence''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentConsequence_Equation''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioConditionalTreatmentConsequences''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentCost_Equation''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTreatmentCost''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentCost''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioSelectableTreatment_ScenarioBudget''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTreatment''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTreatmentScheduling''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioSelectableTreatment''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CriterionLibrary_ScenarioTargetConditionGoal''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ScenarioTargetConditionGoals''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in InvestmentPlan''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in ReportIndex''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in Simulation_User''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationAnalysisDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    			            ---------------------------------------------------------------------------

                            BEGIN TRY

                          ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
    		              SET @RowsDeleted = 1;
    		              --Print ''AssetSummaryDetailValueIntId '';

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
    						            Print ''Rows Affected AssetSummaryDetailValueIntId: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    							            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all
      							            Set @RetMessage = ''Failed'';
    							            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							            Print ''Rolled Back AssetSummaryDetailValueIntId Delete Transaction in Simulation SP:  '' + @ErrorMessage;
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetSummaryDetailValueIntId''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetSummaryDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetUsageDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CashFlowConsiderationDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentConsiderationDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentOptionDetail''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    		            -----------------------------------------------------------------

    			            BEGIN TRY

                            ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all

    			            SET @RowsDeleted = 1;
    		  	            --Print ''AssetDetailValueIntId '';

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
    						            Print ''Rows Affected AssetDetailValueIntId: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    							            ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
      							            Set @RetMessage = ''Failed'';
    							            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							            Print ''Rolled Back AssetDetailValueIntId Delete Transaction in Simulation SP  '' + @ErrorMessage;
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetDetailValueIntId''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    		            -----------------------------------------------------------------

    			            BEGIN TRY

                            ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all

    		  	            --Print ''TreatmentRejectionDetail '';

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
    						            Print ''Rows Affected TreatmentRejectionDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    							            ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all
      							            Set @RetMessage = ''Failed'';
    							            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							            Print ''Rolled Back TreatmentRejectionDetail Delete Transaction in Simulation SP  '' + @ErrorMessage;
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentRejectionDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentSchedulingCollisionDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in DeficientConditionGoalDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TargetConditionGoalDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationYearDetail''
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
    					            Print ''Rows Affected SimulationOutputJson: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    							            ALTER TABLE SimulationOutputJson WITH CHECK CHECK CONSTRAINT all
      							            Set @RetMessage = ''Failed'';
    							            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							            Print ''Rolled Back SimulationOutputJson Delete Transaction in Simulation SP  '' + @ErrorMessage;
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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationOutputJson''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in Simulation''
                                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                                 Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    		
    		            -------------------------------------------------------------
    	
    	            DROP TABLE #SimTempGuids;
                    COMMIT TRANSACTION
                    Print ''Simulation  Delete Transaction Committed'';
       	            --RAISERROR (@RetMessage, 0, 1);
    	            END TRY
    	            BEGIN CATCH
      			            Set @RetMessage = ''Failed'';
    			            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    			            Print ''Rolled Back Simulation Delete Transaction in Simulation SP:  '' + @ErrorMessage;
    			            ROLLBACK TRANSACTION;
    			            RAISERROR  (@RetMessage, 16, 1);  
    	            END CATCH;

                END')
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231213214806_AddDeleteSProcsandIndexes')
BEGIN
    EXEC('CREATE OR ALTER PROCEDURE dbo.usp_delete_simulationoutput(@SimOutputGuidList NVARCHAR(MAX)=NULL,@RetMessage VARCHAR(250) OUTPUT)
    	          AS
    	          BEGIN 
    	            DECLARE @CustomErrorMessage NVARCHAR(MAX),
    	            @ErrorNumber int,
    	            @ErrorSeverity int,
    	            @ErrorState int,
    	            @ErrorProcedure nvarchar(126),
    	            @ErrorLine int,
    	            @ErrorMessage nvarchar(4000);
    	            Set  @RetMessage = ''Success'';
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
    		              PRINT ''String is NULL or empty'';
    		              Set  @SimOutputGuidList = ''00000000-0000-0000-0000-000000000000'';
    	            END

                    INSERT INTO #SimOutputTempGuids (Guid)
    	            SELECT LEFT(LTRIM(RTRIM(value)), 36)
                    FROM STRING_SPLIT(@SimOutputGuidList, '','');

    	            UPDATE #SimOutputTempGuids
    	            SET Guid = ''00000000-0000-0000-0000-000000000000''
    	            WHERE TRY_CAST(Guid AS UNIQUEIDENTIFIER) IS NULL OR Guid = '''';
    	
    	            Begin Transaction
    	            BEGIN TRY

    			            -----Start AssetSummaryDetail Path-----------------------------------------

    			            -------SimulationOutput --> AssetSummaryDetail --> AssetSummaryDetailValueIntI----

                            BEGIN TRY

                          ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
    		  
    		              SET @RowsDeleted = 1;
    		              --Print ''AssetSummaryDetailValueIntId '';

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
    						            Print ''Rows Affected AssetSummaryDetailValueIntId: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    							            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all
      							            Set @RetMessage = ''Failed'';
    							            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							            Print ''Rolled Back AssetSummaryDetailValueIntId Delete Transaction in SimulationOutput SP:  '' + @ErrorMessage;
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetSummaryDetailValueIntId''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetSummaryDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetUsageDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in CashFlowConsiderationDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentConsiderationDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentOptionDetail''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    	            -----------------------------------------------------------------

    	            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId -->  -->  -->  -->  -->  --> 
    	
    			            BEGIN TRY

                            ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all

    			            SET @RowsDeleted = 1;
    		  	            --Print ''AssetDetailValueIntId '';

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
    						            Print ''Rows Affected AssetDetailValueIntId: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    							            ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
      							            Set @RetMessage = ''Failed'';
    							            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							            Print ''Rolled Back AssetDetailValueIntId Delete Transaction in SimulationOutput SP  '' + @ErrorMessage;
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetDetailValueIntId''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH
    	
    		            -----------------------------------------------------------------

    		            --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail -->  -->  -->  -->  -->  --> 

    			            BEGIN TRY

                            ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all
    		  
    		               SET @RowsDeleted = 1;
    		              --Print ''TreatmentRejectionDetail '';

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
    						            Print ''Rows Affected TreatmentRejectionDetail: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					            END TRY
    					            BEGIN CATCH
    							            ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all
      							            Set @RetMessage = ''Failed'';
    							            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							            Print ''Rolled Back TreatmentRejectionDetail Delete Transaction in SimulationOutput SP  '' + @ErrorMessage;
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentRejectionDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TreatmentSchedulingCollisionDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in AssetDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in BudgetDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in DeficientConditionGoalDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in TargetConditionGoalDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationYearDetail''
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

     		                     SELECT @CustomErrorMessage = ''Query Error in SimulationOutput''
    		                     RAISERROR (@CustomErrorMessage, 16, 1);
    				             Set @RetMessage = @CustomErrorMessage;

                            END CATCH

    		               ---------------------------------------------------------------------------
    			            ------End SimulationOutput Delete-------------------------------------------------------------------

    	            DROP TABLE #SimOutputTempGuids;
                    COMMIT TRANSACTION
                    Print ''Delete Transaction Committed'';
       	            RAISERROR (@RetMessage, 0, 1);
    	            END TRY
    	            BEGIN CATCH
      			            Set @RetMessage = ''Failed '' + @RetMessage;
    			            Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    			            Print ''Rolled Back Entire Delete Transaction in SimulationOutput SP:  '' + @ErrorMessage;
    			            ROLLBACK TRANSACTION;
    			            RAISERROR  (@RetMessage, 16, 1);  
    	            END CATCH;
                END')
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231213214806_AddDeleteSProcsandIndexes')
BEGIN
    EXEC('CREATE OR ALTER PROCEDURE dbo.usp_delete_aggregations(@NetworkId AS uniqueidentifier=NULL,@RetMessage VARCHAR(250) OUTPUT)
    	      	          AS
    	      	          BEGIN 
    	                    DECLARE @CustomErrorMessage NVARCHAR(MAX),
    	                    @ErrorNumber int,
    	                    @ErrorSeverity int,
    	                    @ErrorState int,
    	                    @ErrorProcedure nvarchar(126),
    	                    @ErrorLine int,
    	                    @ErrorMessage nvarchar(4000);
    	                    Set  @RetMessage = ''Success'';
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
    		                      --Print ''AggregatedResult '';

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
    						                    --WAITFOR DELAY ''00:00:01'';

    						                    COMMIT TRANSACTION
    						
    						                    Print ''Rows Affected Network --> MaintainableAsset-->AggregatedResult: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					                    END TRY
    					                    BEGIN CATCH
    							                    ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all
      							                    Set @RetMessage = ''Failed'';
    							                    Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							                    Print ''Rolled Back AggregatedResult Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
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

     		                             SELECT @CustomErrorMessage = ''Query Error in  Network --> MaintainableAsset-->AggregatedResult''
    		                             RAISERROR (@CustomErrorMessage, 16, 1);
    				                     Set @RetMessage = @CustomErrorMessage;

                                    END CATCH

    			                    ------End -----------------------------------------------------------------


    			                    -------Start AttributeDatum Path-

    			                    --MaintainableAsset --> AttributeDatum --> AttributeDatumLocation -->  -->  --> 

                                    BEGIN TRY

                                  ALTER TABLE AttributeDatumLocation NOCHECK CONSTRAINT all
    		                      SET @RowsDeleted = 1;
    		  	                    --Print ''AttributeDatumLocation '';

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
    						                    --WAITFOR DELAY ''00:00:01'';

    						                    COMMIT TRANSACTION
    						
    						                    Print ''Rows Affected Network --> MaintainableAsset-->AttributeDatumLocation: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					                    END TRY
    					                    BEGIN CATCH
    							                    ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all
      							                    Set @RetMessage = ''Failed'';
    							                    Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    							                    Print ''Rolled Back AttributeDatumLocation Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
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

     		                             SELECT @CustomErrorMessage = ''Query Error in Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocation''
    		                             RAISERROR (@CustomErrorMessage, 16, 1);
    				                     Set @RetMessage = @CustomErrorMessage;

                                    END CATCH

    			                    -----------------------------------------------------------------------

        		                    --------MaintainableAsset --> AttributeDatum--

                                    BEGIN TRY

    			                    ALTER TABLE AttributeDatum NOCHECK CONSTRAINT all
    			                    SET @RowsDeleted = 1;
    			                    Print ''AttributeDatum '';

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
    						                    --WAITFOR DELAY ''00:00:01'';

    						                    COMMIT TRANSACTION
    						
    						                    Print ''Rows Affected Network --> MaintainableAsset-->AttributeDatum: '' +  convert(NVARCHAR(50), @RowsDeleted);
    					                    END TRY
    					                    BEGIN CATCH
    						                    ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all
    						                    Set @RetMessage = ''Failed'';
    						                    Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    						                    Print ''Rolled Back AttributeDatum Delete Transaction in NetworkDelete SP:  '' + @ErrorMessage;
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

     		                             SELECT @CustomErrorMessage = ''Query Error in Network --> MaintainableAsset-->AttributeDatum''
    		                             RAISERROR (@CustomErrorMessage, 16, 1);
    				                     Set @RetMessage = @CustomErrorMessage;

                                    END CATCH


    			                    ----------------------------------------------------------------------
    			                    -----End AttributeDatum Path--------------------------------------

                           --COMMIT TRANSACTION
                            Print ''Delete Attribute End'';
       	                    RAISERROR (@RetMessage, 0, 1);
    	                    END TRY
    	                    BEGIN CATCH
      			                    Set @RetMessage = ''Failed '' + @RetMessage;
    			                    Set @ErrorMessage =  ERROR_PROCEDURE() + '' (Error At Line: '' + cast( ERROR_LINE() as Varchar(5)) + '' ): '' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
    			                    Print ''Error in AttributeDelete SP:  '' + @ErrorMessage;
    			                    --ROLLBACK TRANSACTION;
    			                    RAISERROR  (@RetMessage, 16, 1);  
    	                    END CATCH;

               END')
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231213214806_AddDeleteSProcsandIndexes')
BEGIN
    BEGIN EXEC('
    	                            DECLARE @RetMessage varchar(100);
    	                            EXEC usp_orphan_cleanup @RetMessage OUTPUT;
                                    EXEC usp_orphan_cleanup @RetMessage OUTPUT;
                                    EXEC usp_orphan_cleanup @RetMessage OUTPUT; 
                                ')  END
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231213214806_AddDeleteSProcsandIndexes')
BEGIN
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
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231213214806_AddDeleteSProcsandIndexes')
BEGIN
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
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20231213214806_AddDeleteSProcsandIndexes')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20231213214806_AddDeleteSProcsandIndexes', N'6.0.1');
END;
GO

COMMIT;
GO

