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

