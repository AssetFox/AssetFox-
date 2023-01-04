using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.BudgetPriority;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Deficient;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.TargetConditionGoal;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Deficient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.TargetConditionGoal;
using static AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums.TreatmentEnum;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Enums;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class IAMContext : DbContext
    {
        public static readonly bool IsRunningFromXunit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!IsRunningFromXunit)
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Repositories\\MSSQL", "migrationConnection.json");
                var contents = File.ReadAllText(filePath);
                var migrationConnection = JsonConvert
                    .DeserializeAnonymousType(contents, new { ConnectionStrings = default(MigrationConnection) })
                    .ConnectionStrings;

                optionsBuilder.UseSqlServer(migrationConnection.BridgeCareConnex);
            }
        }



        public IAMContext() { }

        public IAMContext(DbContextOptions<IAMContext> options) : base(options) { }

        protected override void ConfigureConventions(
            ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            configurationBuilder.Properties<decimal>()
                .HavePrecision(18, 2);
        }

        public virtual DbSet<AggregatedResultEntity> AggregatedResult { get; set; }

        public virtual DbSet<AnalysisMethodEntity> AnalysisMethod { get; set; }

        public virtual DbSet<AttributeEntity> Attribute { get; set; }

        public virtual DbSet<AttributeEquationCriterionLibraryEntity> AttributeEquationCriterionLibrary { get; set; }

        public virtual DbSet<AttributeDatumEntity> AttributeDatum { get; set; }

        public virtual DbSet<AttributeDatumLocationEntity> AttributeDatumLocation { get; set; }

        public virtual DbSet<BenefitEntity> Benefit { get; set; }

        public virtual DbSet<BudgetEntity> Budget { get; set; }

        public virtual DbSet<BudgetLibraryEntity> BudgetLibrary { get; set; }
        public virtual DbSet<BudgetLibraryUserEntity> BudgetLibraryUser { get; set; }

        public virtual DbSet<BudgetAmountEntity> BudgetAmount { get; set; }

        public virtual DbSet<BudgetPercentagePairEntity> BudgetPercentagePair { get; set; }

        public virtual DbSet<BudgetPriorityEntity> BudgetPriority { get; set; }

        public virtual DbSet<BudgetPriorityLibraryEntity> BudgetPriorityLibrary { get; set; }

        public virtual DbSet<CashFlowDistributionRuleEntity> CashFlowDistributionRule { get; set; }

        public virtual DbSet<CashFlowRuleEntity> CashFlowRule { get; set; }

        public virtual DbSet<CashFlowRuleLibraryEntity> CashFlowRuleLibrary { get; set; }

        public virtual DbSet<CommittedProjectEntity> CommittedProject { get; set; }

        public virtual DbSet<CommittedProjectConsequenceEntity> CommittedProjectConsequence { get; set; }

        public virtual DbSet<CommittedProjectLocationEntity> CommittedProjectLocation { get; set; }

        public virtual DbSet<CriterionLibraryEntity> CriterionLibrary { get; set; }

        public virtual DbSet<CriterionLibraryAnalysisMethodEntity> CriterionLibraryAnalysisMethod { get; set; }

        public virtual DbSet<CriterionLibraryBudgetEntity> CriterionLibraryBudget { get; set; }

        public virtual DbSet<CriterionLibraryBudgetPriorityEntity> CriterionLibraryBudgetPriority { get; set; }

        public virtual DbSet<CriterionLibraryCashFlowRuleEntity> CriterionLibraryCashFlowRule { get; set; }

        public virtual DbSet<CriterionLibraryDeficientConditionGoalEntity> CriterionLibraryDeficientConditionGoal { get; set; }

        public virtual DbSet<CriterionLibraryScenarioDeficientConditionGoalEntity> CriterionLibraryScenarioDeficientConditionGoal { get; set; }

        public virtual DbSet<CriterionLibraryPerformanceCurveEntity> CriterionLibraryPerformanceCurve { get; set; }

        public virtual DbSet<CriterionLibraryRemainingLifeLimitEntity> CriterionLibraryRemainingLifeLimit { get; set; }

        public virtual DbSet<CriterionLibraryScenarioRemainingLifeLimitEntity> CriterionLibraryScenarioRemainingLifeLimit { get; set; }

        public virtual DbSet<CriterionLibraryTargetConditionGoalEntity> CriterionLibraryTargetConditionGoal { get; set; }

        public virtual DbSet<CriterionLibraryScenarioTargetConditionGoalEntity> CriterionLibraryScenarioTargetConditionGoal { get; set; }

        public virtual DbSet<CriterionLibrarySelectableTreatmentEntity> CriterionLibrarySelectableTreatment { get; set; }

        public virtual DbSet<CriterionLibraryConditionalTreatmentConsequenceEntity> CriterionLibraryTreatmentConsequence { get; set; }

        public virtual DbSet<CriterionLibraryTreatmentCostEntity> CriterionLibraryTreatmentCost { get; set; }

        public virtual DbSet<CriterionLibraryTreatmentSupersessionEntity> CriterionLibraryTreatmentSupersession { get; set; }

        public virtual DbSet<CriterionLibraryScenarioTreatmentSupersessionEntity> CriterionLibraryScenarioTreatmentSupersession { get; set; }

        public virtual DbSet<DataSourceEntity> DataSource { get; set; }

        public virtual DbSet<DeficientConditionGoalEntity> DeficientConditionGoal { get; set; }

        public virtual DbSet<ScenarioDeficientConditionGoalEntity> ScenarioDeficientConditionGoal { get; set; }

        public virtual DbSet<DeficientConditionGoalLibraryEntity> DeficientConditionGoalLibrary { get; set; }

        public virtual DbSet<EquationEntity> Equation { get; set; }
        public virtual DbSet<ExcelRawDataEntity> ExcelRawData { get; set; }

        public virtual DbSet<InvestmentPlanEntity> InvestmentPlan { get; set; }

        public virtual DbSet<MaintainableAssetEntity> MaintainableAsset { get; set; }

        public virtual DbSet<MaintainableAssetLocationEntity> MaintainableAssetLocation { get; set; }

        public virtual DbSet<NetworkEntity> Network { get; set; }

        public virtual DbSet<NetworkAttributeEntity> NetworkAttribute { get; set; }

        public virtual DbSet<PerformanceCurveEntity> PerformanceCurve { get; set; }

        public virtual DbSet<PerformanceCurveEquationEntity> PerformanceCurveEquation { get; set; }

        public virtual DbSet<PerformanceCurveLibraryEntity> PerformanceCurveLibrary { get; set; }

        public virtual DbSet<RemainingLifeLimitEntity> RemainingLifeLimit { get; set; }

        public virtual DbSet<ScenarioRemainingLifeLimitEntity> ScenarioRemainingLifeLimit { get; set; }

        public virtual DbSet<RemainingLifeLimitLibraryEntity> RemainingLifeLimitLibrary { get; set; }

        public virtual DbSet<AnalysisMaintainableAssetEntity> AnalysisMaintainableAsset { get; set; }

        public virtual DbSet<SimulationEntity> Simulation { get; set; }

        public virtual DbSet<SimulationLogEntity> SimulationLog { get; set; }

        public virtual DbSet<SimulationOutputEntity> SimulationOutput { get; set; }

        public virtual DbSet<TargetConditionGoalEntity> TargetConditionGoal { get; set; }

        public virtual DbSet<ScenarioTargetConditionGoalEntity> ScenarioTargetConditionGoals { get; set; }

        public virtual DbSet<TargetConditionGoalLibraryEntity> TargetConditionGoalLibrary { get; set; }

        public virtual DbSet<SelectableTreatmentEntity> SelectableTreatment { get; set; }

        public virtual DbSet<ScenarioSelectableTreatmentScenarioBudgetEntity> ScenarioSelectableTreatmentScenarioBudget { get; set; }

        public virtual DbSet<ConditionalTreatmentConsequenceEntity> TreatmentConsequence { get; set; }

        public virtual DbSet<ConditionalTreatmentConsequenceEquationEntity> TreatmentConsequenceEquation { get; set; }

        public virtual DbSet<TreatmentCostEntity> TreatmentCost { get; set; }

        public virtual DbSet<TreatmentCostEquationEntity> TreatmentCostEquation { get; set; }

        public virtual DbSet<TreatmentLibraryEntity> TreatmentLibrary { get; set; }

        public virtual DbSet<TreatmentSchedulingEntity> TreatmentScheduling { get; set; }

        public virtual DbSet<ScenarioTreatmentSchedulingEntity> ScenarioTreatmentScheduling { get; set; }

        public virtual DbSet<TreatmentSupersessionEntity> TreatmentSupersession { get; set; }

        public virtual DbSet<ScenarioTreatmentSupersessionEntity> ScenarioTreatmentSupersession { get; set; }

        public virtual DbSet<NumericAttributeValueHistoryEntity> NumericAttributeValueHistory { get; set; }

        public virtual DbSet<TextAttributeValueHistoryEntity> TextAttributeValueHistory { get; set; }

        public virtual DbSet<SimulationAnalysisDetailEntity> SimulationAnalysisDetail { get; set; }

        public virtual DbSet<UserEntity> User { get; set; }

        public virtual DbSet<SimulationUserEntity> SimulationUser { get; set; }

        public virtual DbSet<CriterionLibraryUserEntity> CriterionLibraryUser { get; set; }

        public virtual DbSet<NetworkRollupDetailEntity> NetworkRollupDetail { get; set; }

        public virtual DbSet<SimulationReportDetailEntity> SimulationReportDetail { get; set; }

        public virtual DbSet<BenefitQuantifierEntity> BenefitQuantifier { get; set; }

        public virtual DbSet<UserCriteriaFilterEntity> UserCriteria { get; set; }

        public virtual DbSet<ReportIndexEntity> ReportIndex { get; set; }

        public virtual DbSet<AnnouncementEntity> Announcement { get; set; }

        public virtual DbSet<ScenarioPerformanceCurveEntity> ScenarioPerformanceCurve { get; set; }

        public virtual DbSet<ScenarioPerformanceCurveEquationEntity> ScenarioPerformanceCurveEquation { get; set; }

        public virtual DbSet<CriterionLibraryScenarioPerformanceCurveEntity> CriterionLibraryScenarioPerformanceCurve { get; set; }

        public virtual DbSet<ScenarioSelectableTreatmentEntity> ScenarioSelectableTreatment { get; set; }
        public virtual DbSet<ScenarioTreatmentCostEntity> ScenarioTreatmentCost { get; set; }
        public virtual DbSet<ScenarioTreatmentCostEquationEntity> ScenarioTreatmentCostEquation { get; set; }
        public virtual DbSet<ScenarioConditionalTreatmentConsequenceEquationEntity> ScenarioConditionalTreatmentConsequenceEquations { get; set; }
        public virtual DbSet<ScenarioConditionalTreatmentConsequenceEntity> ScenarioConditionalTreatmentConsequences { get; set; }
        public virtual DbSet<CriterionLibraryScenarioTreatmentCostEntity> CriterionLibraryScenarioTreatmentCosts { get; set; }
        public virtual DbSet<CriterionLibraryScenarioSelectableTreatmentEntity> CriterionLibraryScenarioSelectableTreatments { get; set; }
        public virtual DbSet<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity> CriterionLibraryScenarioConditionalTreatmentConsequence { get; set; }

        public virtual DbSet<ScenarioBudgetEntity> ScenarioBudget { get; set; }
        public virtual DbSet<ScenarioBudgetAmountEntity> ScenarioBudgetAmount { get; set; }
        public virtual DbSet<CriterionLibraryScenarioBudgetEntity> CriterionLibraryScenarioBudget { get; set; }

        public virtual DbSet<ScenarioBudgetPriorityEntity> ScenarioBudgetPriority { get; set; }

        public virtual DbSet<CriterionLibraryScenarioBudgetPriorityEntity> CriterionLibraryScenarioBudgetPriority { get; set; }

        public virtual DbSet<ScenarioCashFlowRuleEntity> ScenarioCashFlowRule { get; set; }

        public virtual DbSet<ScenarioCashFlowDistributionRuleEntity> ScenarioCashFlowDistributionRule { get; set; }

        public virtual DbSet<CriterionLibraryScenarioCashFlowRuleEntity> CriterionLibraryScenarioCashFlowRule { get; set; }

        public virtual DbSet<CalculatedAttributeLibraryEntity> CalculatedAttributeLibrary { get; set; }

        public virtual DbSet<CalculatedAttributeEntity> CalculatedAttribute { get; set; }

        public virtual DbSet<CalculatedAttributeEquationCriteriaPairEntity> CalculatedAttributeEquationCriteriaPair { get; set; }

        public virtual DbSet<CriterionLibraryCalculatedAttributePairEntity> CriterionLibraryCalculatedAttributePair { get; set; }

        public virtual DbSet<EquationCalculatedAttributePairEntity> EquationCalculatedAttributePair { get; set; }

        public virtual DbSet<ScenarioCalculatedAttributeEntity> ScenarioCalculatedAttribute { get; set; }

        public virtual DbSet<ScenarioCalculatedAttributeEquationCriteriaPairEntity> ScenarioCalculatedAttributeEquationCriteriaPair { get; set; }

        public virtual DbSet<ScenarioCriterionLibraryCalculatedAttributePairEntity> ScenarioCriterionLibraryCalculatedAttributePair { get; set; }

        public virtual DbSet<ScenarioEquationCalculatedAttributePairEntity> ScenarioEquationCalculatedAttributePair { get; set; }

        public virtual DbSet<AssetDetailEntity> AssetDetail { get; set; }

        public virtual DbSet<AssetDetailValueEntityIntId> AssetDetailValueIntId { get; set; }

        public virtual DbSet<AssetSummaryDetailEntity> AssetSummaryDetail { get; set; }

        public virtual DbSet<AssetSummaryDetailValueEntityIntId> AssetSummaryDetailValueIntId { get; set; }

        public virtual DbSet<BudgetDetailEntity> BudgetDetail { get; set; }

        public virtual DbSet<BudgetUsageDetailEntity> BudgetUsageDetail { get; set; }

        public virtual DbSet<CashFlowConsiderationDetailEntity> CashFlowConsiderationDetail { get; set; }

        public virtual DbSet<DeficientConditionGoalDetailEntity> DeficientConditionGoalDetail { get; set; }

        public virtual DbSet<SimulationYearDetailEntity> SimulationYearDetail { get; set; }

        public virtual DbSet<TargetConditionGoalDetailEntity> TargetConditionGoalDetail { get; set; }

        public virtual DbSet<TreatmentConsiderationDetailEntity> TreatmentConsiderationDetail { get; set; }

        public virtual DbSet<TreatmentOptionDetailEntity> TreatmentOptionDetail { get; set; }

        public virtual DbSet<TreatmentRejectionDetailEntity> TreatmentRejectionDetail { get; set; }

        public virtual DbSet<TreatmentSchedulingCollisionDetailEntity> TreatmentSchedulingCollisionDetail { get; set; }

        private class MigrationConnection
        {
            public string BridgeCareConnex { get; set; }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AggregatedResultEntity>(entity =>
            {
                entity.HasIndex(e => e.AttributeId);

                entity.HasIndex(e => e.MaintainableAssetId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.Property(e => e.Year).IsRequired();

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.AggregatedResults)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.MaintainableAsset)
                    .WithMany(p => p.AggregatedResults)
                    .HasForeignKey(d => d.MaintainableAssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AnalysisMethodEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.AnalysisMethod)
                    .HasForeignKey<AnalysisMethodEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.AnalysisMethods)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<AttributeEntity>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.AggregationRuleType).IsRequired();

                entity.Property(e => e.Command).IsRequired();

                entity.Property(e => e.DataType).IsRequired();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<AttributeEquationCriterionLibraryEntity>(entity =>
            {
                entity.HasKey(e => new { e.AttributeId, e.EquationId });

                entity.HasIndex(e => e.AttributeId);

                entity.HasIndex(e => e.EquationId).IsUnique();

                entity.ToTable("Attribute_Equation_CriterionLibrary");

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.AttributeEquationCriterionLibraryJoins)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.AttributeEquationCriterionLibraryJoin)
                    .HasForeignKey<AttributeEquationCriterionLibraryEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.AttributeEquationCriterionLibraryJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<AttributeDatumEntity>(entity =>
            {
                entity.HasIndex(e => e.AttributeId);

                entity.HasIndex(e => e.MaintainableAssetId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.AttributeData)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.MaintainableAsset)
                    .WithMany(p => p.AssignedData)
                    .HasForeignKey(d => d.MaintainableAssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttributeDatumLocationEntity>(entity =>
            {
                entity.HasIndex(e => e.AttributeDatumId).IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.Property(e => e.LocationIdentifier).IsRequired();

                entity.HasOne(d => d.AttributeDatum)
                    .WithOne(p => p.AttributeDatumLocation)
                    .HasForeignKey<AttributeDatumLocationEntity>(d => d.AttributeDatumId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BenefitEntity>(entity =>
            {
                entity.HasIndex(e => e.AnalysisMethodId).IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Limit).IsRequired();

                entity.HasOne(d => d.AnalysisMethod)
                    .WithOne(p => p.Benefit)
                    .HasForeignKey<BenefitEntity>(d => d.AnalysisMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.Benefits)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BudgetEntity>(entity =>
            {
                entity.HasIndex(e => e.BudgetLibraryId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.BudgetLibrary)
                    .WithMany(p => p.Budgets)
                    .HasForeignKey(d => d.BudgetLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioBudgetEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.Budgets)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BudgetLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<BudgetAmountEntity>(entity =>
            {
                entity.HasIndex(e => e.BudgetId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Year).IsRequired();

                entity.Property(e => e.Value).IsRequired();

                entity.HasOne(d => d.Budget)
                    .WithMany(p => p.BudgetAmounts)
                    .HasForeignKey(d => d.BudgetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioBudgetAmountEntity>(entity =>
            {
                entity.HasIndex(e => e.ScenarioBudgetId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Year).IsRequired();

                entity.Property(e => e.Value).IsRequired();

                entity.HasOne(d => d.ScenarioBudget)
                    .WithMany(p => p.ScenarioBudgetAmounts)
                    .HasForeignKey(d => d.ScenarioBudgetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BudgetPercentagePairEntity>(entity =>
            {
                entity.HasIndex(e => e.ScenarioBudgetId);

                entity.HasIndex(e => e.ScenarioBudgetPriorityId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Percentage).IsRequired();

                entity.HasOne(d => d.ScenarioBudget)
                    .WithMany(p => p.BudgetPercentagePairs)
                    .HasForeignKey(d => d.ScenarioBudgetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ScenarioBudgetPriority)
                    .WithMany(p => p.BudgetPercentagePairs)
                    .HasForeignKey(d => d.ScenarioBudgetPriorityId)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

            modelBuilder.Entity<BudgetPriorityEntity>(entity =>
            {
                entity.HasIndex(e => e.BudgetPriorityLibraryId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PriorityLevel).IsRequired();

                entity.HasOne(d => d.BudgetPriorityLibrary)
                    .WithMany(p => p.BudgetPriorities)
                    .HasForeignKey(d => d.BudgetPriorityLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioBudgetPriorityEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PriorityLevel).IsRequired();

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.BudgetPriorities)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BudgetPriorityLibraryEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<CashFlowDistributionRuleEntity>(entity =>
            {
                entity.HasIndex(e => e.CashFlowRuleId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.DurationInYears).IsRequired();

                entity.Property(e => e.CostCeiling).IsRequired();

                entity.Property(e => e.YearlyPercentages).IsRequired();

                entity.HasOne(d => d.CashFlowRule)
                    .WithMany(p => p.CashFlowDistributionRules)
                    .HasForeignKey(d => d.CashFlowRuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioCashFlowDistributionRuleEntity>(entity =>
            {
                entity.HasIndex(e => e.ScenarioCashFlowRuleId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.DurationInYears).IsRequired();

                entity.Property(e => e.CostCeiling).IsRequired();

                entity.Property(e => e.YearlyPercentages).IsRequired();

                entity.HasOne(d => d.ScenarioCashFlowRule)
                    .WithMany(p => p.ScenarioCashFlowDistributionRules)
                    .HasForeignKey(d => d.ScenarioCashFlowRuleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CashFlowRuleEntity>(entity =>
            {
                entity.HasIndex(e => e.CashFlowRuleLibraryId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.CashFlowRuleLibrary)
                    .WithMany(p => p.CashFlowRules)
                    .HasForeignKey(d => d.CashFlowRuleLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioCashFlowRuleEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.CashFlowRules)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CashFlowRuleLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<CommittedProjectEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.ShadowForAnyTreatment).IsRequired();

                entity.Property(e => e.ShadowForSameTreatment).IsRequired();

                entity.Property(e => e.Cost).IsRequired();

                entity.Property(e => e.Year).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ScenarioBudget)
                    .WithMany(p => p.CommittedProjects)
                    .HasForeignKey(d => d.ScenarioBudgetId)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.CommittedProjects)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.CommittedProjectLocation)
                    .WithOne(p => p.CommittedProject)
                    .HasForeignKey<CommittedProjectLocationEntity>(d => d.CommittedProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(d => d.CommittedProjectConsequences)
                    .WithOne(p => p.CommittedProject)
                    .HasForeignKey(d => d.CommittedProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CommittedProjectLocationEntity>(entity =>
            {
                entity.HasIndex(e => e.CommittedProjectId).IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.Property(e => e.LocationIdentifier).IsRequired();

                entity.HasOne(d => d.CommittedProject)
                    .WithOne(p => p.CommittedProjectLocation)
                    .HasForeignKey<CommittedProjectLocationEntity>(d => d.CommittedProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CommittedProjectConsequenceEntity>(entity =>
            {
                entity.HasIndex(e => e.CommittedProjectId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ChangeValue).IsRequired();

                entity.HasOne(d => d.CommittedProject)
                    .WithMany(p => p.CommittedProjectConsequences)
                    .HasForeignKey(d => d.CommittedProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.MergedCriteriaExpression).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<CriterionLibraryAnalysisMethodEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.AnalysisMethodId });

                entity.ToTable("CriterionLibrary_AnalysisMethod");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.AnalysisMethodId).IsUnique();

                entity.HasOne(d => d.AnalysisMethod)
                    .WithOne(p => p.CriterionLibraryAnalysisMethodJoin)
                    .HasForeignKey<CriterionLibraryAnalysisMethodEntity>(d => d.AnalysisMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryAnalysisMethodJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryBudgetEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.BudgetId });

                entity.ToTable("CriterionLibrary_Budget");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.BudgetId).IsUnique();

                entity.HasOne(d => d.Budget)
                    .WithOne(p => p.CriterionLibraryBudgetJoin)
                    .HasForeignKey<CriterionLibraryBudgetEntity>(d => d.BudgetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryBudgetJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryScenarioBudgetEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.ScenarioBudgetId });

                entity.ToTable("CriterionLibrary_ScenarioBudget");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioBudgetId).IsUnique();

                entity.HasOne(d => d.ScenarioBudget)
                    .WithOne(p => p.CriterionLibraryScenarioBudgetJoin)
                    .HasForeignKey<CriterionLibraryScenarioBudgetEntity>(d => d.ScenarioBudgetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioBudgetJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryBudgetPriorityEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.BudgetPriorityId });

                entity.ToTable("CriterionLibrary_BudgetPriority");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.BudgetPriorityId).IsUnique();

                entity.HasOne(d => d.BudgetPriority)
                    .WithOne(p => p.CriterionLibraryBudgetPriorityJoin)
                    .HasForeignKey<CriterionLibraryBudgetPriorityEntity>(d => d.BudgetPriorityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryBudgetPriorityJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryScenarioBudgetPriorityEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.ScenarioBudgetPriorityId });

                entity.ToTable("CriterionLibrary_ScenarioBudgetPriority");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioBudgetPriorityId).IsUnique();

                entity.HasOne(d => d.ScenarioBudgetPriority)
                    .WithOne(p => p.CriterionLibraryScenarioBudgetPriorityJoin)
                    .HasForeignKey<CriterionLibraryScenarioBudgetPriorityEntity>(d => d.ScenarioBudgetPriorityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioBudgetPriorityJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryCashFlowRuleEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.CashFlowRuleId });

                entity.ToTable("CriterionLibrary_CashFlowRule");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.CashFlowRuleId).IsUnique();

                entity.HasOne(d => d.CashFlowRule)
                    .WithOne(p => p.CriterionLibraryCashFlowRuleJoin)
                    .HasForeignKey<CriterionLibraryCashFlowRuleEntity>(d => d.CashFlowRuleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryCashFlowRuleJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryScenarioCashFlowRuleEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.ScenarioCashFlowRuleId });

                entity.ToTable("CriterionLibrary_ScenarioCashFlowRule");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioCashFlowRuleId).IsUnique();

                entity.HasOne(d => d.ScenarioCashFlowRule)
                    .WithOne(p => p.CriterionLibraryScenarioCashFlowRuleJoin)
                    .HasForeignKey<CriterionLibraryScenarioCashFlowRuleEntity>(d => d.ScenarioCashFlowRuleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioCashFlowRuleJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryDeficientConditionGoalEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.DeficientConditionGoalId });

                entity.ToTable("CriterionLibrary_DeficientConditionGoal");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.DeficientConditionGoalId).IsUnique();

                entity.HasOne(d => d.DeficientConditionGoal)
                    .WithOne(p => p.CriterionLibraryDeficientConditionGoalJoin)
                    .HasForeignKey<CriterionLibraryDeficientConditionGoalEntity>(d => d.DeficientConditionGoalId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryDeficientConditionGoalJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<CriterionLibraryScenarioDeficientConditionGoalEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.ScenarioDeficientConditionGoalId });

                entity.ToTable("CriterionLibrary_ScenarioDeficientConditionGoal");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioDeficientConditionGoalId).IsUnique();

                entity.HasOne(d => d.ScenarioDeficientConditionGoal)
                    .WithOne(p => p.CriterionLibraryScenarioDeficientConditionGoalJoin)
                    .HasForeignKey<CriterionLibraryScenarioDeficientConditionGoalEntity>(d => d.ScenarioDeficientConditionGoalId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioDeficientConditionGoalJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryPerformanceCurveEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.PerformanceCurveId });

                entity.ToTable("CriterionLibrary_PerformanceCurve");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.PerformanceCurveId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryPerformanceCurveJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.PerformanceCurve)
                    .WithOne(p => p.CriterionLibraryPerformanceCurveJoin)
                    .HasForeignKey<CriterionLibraryPerformanceCurveEntity>(d => d.PerformanceCurveId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryScenarioPerformanceCurveEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.ScenarioPerformanceCurveId });

                entity.ToTable("CriterionLibrary_ScenarioPerformanceCurve");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioPerformanceCurveId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioPerformanceCurveJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ScenarioPerformanceCurve)
                    .WithOne(p => p.CriterionLibraryScenarioPerformanceCurveJoin)
                    .HasForeignKey<CriterionLibraryScenarioPerformanceCurveEntity>(d => d.ScenarioPerformanceCurveId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryRemainingLifeLimitEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.RemainingLifeLimitId });

                entity.ToTable("CriterionLibrary_RemainingLifeLimit");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.RemainingLifeLimitId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryRemainingLifeLimitJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.RemainingLifeLimit)
                    .WithOne(p => p.CriterionLibraryRemainingLifeLimitJoin)
                    .HasForeignKey<CriterionLibraryRemainingLifeLimitEntity>(d => d.RemainingLifeLimitId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<CriterionLibraryScenarioRemainingLifeLimitEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.ScenarioRemainingLifeLimitId });

                entity.ToTable("CriterionLibrary_ScenarioRemainingLifeLimit");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioRemainingLifeLimitId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioRemainingLifeLimitJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ScenarioRemainingLifeLimit)
                    .WithOne(p => p.CriterionLibraryScenarioRemainingLifeLimitJoin)
                    .HasForeignKey<CriterionLibraryScenarioRemainingLifeLimitEntity>(d => d.ScenarioRemainingLifeLimitId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryTargetConditionGoalEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.TargetConditionGoalId });

                entity.ToTable("CriterionLibrary_TargetConditionGoal");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.TargetConditionGoalId).IsUnique();

                entity.HasOne(d => d.TargetConditionGoal)
                    .WithOne(p => p.CriterionLibraryTargetConditionGoalJoin)
                    .HasForeignKey<CriterionLibraryTargetConditionGoalEntity>(d => d.TargetConditionGoalId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryTargetConditionGoalJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryScenarioTargetConditionGoalEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.ScenarioTargetConditionGoalId });

                entity.ToTable("CriterionLibrary_ScenarioTargetConditionGoal");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioTargetConditionGoalId).IsUnique();

                entity.HasOne(d => d.ScenarioTargetConditionGoal)
                    .WithOne(p => p.CriterionLibraryScenarioTargetConditionGoalJoin)
                    .HasForeignKey<CriterionLibraryScenarioTargetConditionGoalEntity>(d => d.ScenarioTargetConditionGoalId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioTargetConditionGoalJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibrarySelectableTreatmentEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, TreatmentId = e.SelectableTreatmentId });

                entity.ToTable("CriterionLibrary_Treatment");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.SelectableTreatmentId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibrarySelectableTreatmentJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.SelectableTreatment)
                    .WithOne(p => p.CriterionLibrarySelectableTreatmentJoin)
                    .HasForeignKey<CriterionLibrarySelectableTreatmentEntity>(d => d.SelectableTreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryScenarioSelectableTreatmentEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, TreatmentId = e.ScenarioSelectableTreatmentId });

                entity.ToTable("CriterionLibrary_ScenarioTreatment");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioSelectableTreatmentId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioSelectableTreatmentJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ScenarioSelectableTreatment)
                    .WithOne(p => p.CriterionLibraryScenarioSelectableTreatmentJoin)
                    .HasForeignKey<CriterionLibraryScenarioSelectableTreatmentEntity>(d => d.ScenarioSelectableTreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryConditionalTreatmentConsequenceEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, TreatmentConsequenceId = e.ConditionalTreatmentConsequenceId });

                entity.ToTable("CriterionLibrary_TreatmentConsequence");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ConditionalTreatmentConsequenceId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryTreatmentConsequenceJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ConditionalTreatmentConsequence)
                    .WithOne(p => p.CriterionLibraryConditionalTreatmentConsequenceJoin)
                    .HasForeignKey<CriterionLibraryConditionalTreatmentConsequenceEntity>(d => d.ConditionalTreatmentConsequenceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, TreatmentConsequenceId = e.ScenarioConditionalTreatmentConsequenceId });

                entity.ToTable("CriterionLibrary_ScenarioTreatmentConsequence");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioConditionalTreatmentConsequenceId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioTreatmentConsequenceJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ScenarioConditionalTreatmentConsequence)
                    .WithOne(p => p.CriterionLibraryScenarioConditionalTreatmentConsequenceJoin)
                    .HasForeignKey<CriterionLibraryScenarioConditionalTreatmentConsequenceEntity>(d => d.ScenarioConditionalTreatmentConsequenceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryTreatmentCostEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.TreatmentCostId });

                entity.ToTable("CriterionLibrary_TreatmentCost");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.TreatmentCostId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryTreatmentCostJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.TreatmentCost)
                    .WithOne(p => p.CriterionLibraryTreatmentCostJoin)
                    .HasForeignKey<CriterionLibraryTreatmentCostEntity>(d => d.TreatmentCostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryScenarioTreatmentCostEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.ScenarioTreatmentCostId });

                entity.ToTable("CriterionLibrary_ScenarioTreatmentCost");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.ScenarioTreatmentCostId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                .WithMany(p => p.CriterionLibraryScenarioTreatmentCostJoins)
                .HasForeignKey(d => d.CriterionLibraryId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ScenarioTreatmentCost)
                .WithOne(p => p.CriterionLibraryScenarioTreatmentCostJoin)
                .HasForeignKey<CriterionLibraryScenarioTreatmentCostEntity>(d => d.ScenarioTreatmentCostId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryTreatmentSupersessionEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.TreatmentSupersessionId });

                entity.ToTable("CriterionLibrary_TreatmentSupersession");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.TreatmentSupersessionId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryTreatmentSupersessionJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.TreatmentSupersession)
                    .WithOne(p => p.CriterionLibraryTreatmentSupersessionJoin)
                    .HasForeignKey<CriterionLibraryTreatmentSupersessionEntity>(d => d.TreatmentSupersessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryScenarioTreatmentSupersessionEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.TreatmentSupersessionId });

                entity.ToTable("CriterionLibrary_ScenarioTreatmentSupersession");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.TreatmentSupersessionId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioTreatmentSupersessionJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ScenarioTreatmentSupersession)
                    .WithOne(p => p.CriterionLibraryScenarioTreatmentSupersessionJoin)
                    .HasForeignKey<CriterionLibraryScenarioTreatmentSupersessionEntity>(d => d.TreatmentSupersessionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DataSourceEntity>(entity =>
            {
                entity.HasIndex(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Type).IsRequired();

                entity.Property(e => e.Secure).IsRequired();

                entity.HasMany(d => d.ExcelRawData)
                    .WithOne(p => p.DataSource)
                    .HasForeignKey(d => d.DataSourceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DeficientConditionGoalEntity>(entity =>
            {
                entity.HasIndex(e => e.DeficientConditionGoalLibraryId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.AllowedDeficientPercentage).IsRequired();

                entity.Property(e => e.DeficientLimit).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.DeficientConditionGoals)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.DeficientConditionGoalLibrary)
                    .WithMany(p => p.DeficientConditionGoals)
                    .HasForeignKey(d => d.DeficientConditionGoalLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioDeficientConditionGoalEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.AllowedDeficientPercentage).IsRequired();

                entity.Property(e => e.DeficientLimit).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.ScenarioDeficientConditionGoals)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.ScenarioDeficientConditionGoals)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DeficientConditionGoalLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<EquationEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Expression).IsRequired();
            });


            modelBuilder.Entity<ExcelRawDataEntity>(entity =>
            {
                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasIndex(e => e.DataSourceId);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.SerializedContent).IsRequired();

                entity.Property(e => e.DataSourceId).IsRequired();

                entity.HasOne(e => e.DataSource)
                    .WithMany(ds => ds.ExcelRawData)
                    .HasForeignKey(w => w.DataSourceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<InvestmentPlanEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.Property(e => e.FirstYearOfAnalysisPeriod).IsRequired();

                entity.Property(e => e.InflationRatePercentage).IsRequired();

                entity.Property(e => e.MinimumProjectCostLimit).IsRequired();

                entity.Property(e => e.NumberOfYearsInAnalysisPeriod).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.InvestmentPlan)
                    .HasForeignKey<InvestmentPlanEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintainableAssetEntity>(entity =>
            {
                entity.HasIndex(e => e.NetworkId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.MaintainableAssets)
                    .HasForeignKey(d => d.NetworkId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintainableAssetLocationEntity>(entity =>
            {
                entity.HasIndex(e => e.MaintainableAssetId).IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Discriminator).IsRequired();

                entity.Property(e => e.LocationIdentifier).IsRequired();

                entity.HasOne(d => d.MaintainableAsset)
                    .WithOne(p => p.MaintainableAssetLocation)
                    .HasForeignKey<MaintainableAssetLocationEntity>(d => d.MaintainableAssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NetworkEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.KeyAttributeId).IsRequired();
            });

            modelBuilder.Entity<NetworkAttributeEntity>(entity =>
            {
                entity.HasKey(e => new { e.AttributeId, e.NetworkId });

                entity.Property(e => e.NetworkId).IsRequired();

                entity.Property(e => e.AttributeId).IsRequired();

                entity.HasOne(j => j.Network)
                    .WithMany(n => n.AttributeJoins)
                    .HasForeignKey(j => j.NetworkId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PerformanceCurveEntity>(entity =>
            {
                entity.HasIndex(e => e.PerformanceCurveLibraryId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.PerformanceCurveLibrary)
                    .WithMany(p => p.PerformanceCurves)
                    .HasForeignKey(d => d.PerformanceCurveLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.PerformanceCurves)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioPerformanceCurveEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.PerformanceCurves)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.ScenarioPerformanceCurves)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioCalculatedAttributeEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.CalculationTiming).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.CalculatedAttributes)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.ScenarioCalculatedAttributes)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioCalculatedAttributeEquationCriteriaPairEntity>(entity =>
            {
                entity.ToTable("ScenarioCalculatedAttributePair");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.ScenarioCalculatedAttributeId);

                entity.HasOne(d => d.ScenarioCalculatedAttribute)
                    .WithMany(p => p.Equations)
                    .HasForeignKey(d => d.ScenarioCalculatedAttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioCriterionLibraryCalculatedAttributePairEntity>(entity =>
            {
                entity.HasKey(e => new { e.ScenarioCalculatedAttributePairId, e.CriterionLibraryId });

                entity.ToTable("ScenarioCalculatedAttributePair_Criteria");

                entity.HasIndex(e => e.ScenarioCalculatedAttributePairId).IsUnique();

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasOne(d => d.ScenarioCalculatedAttributePair)
                    .WithOne(p => p.CriterionLibraryCalculatedAttributeJoin)
                    .HasForeignKey<ScenarioCriterionLibraryCalculatedAttributePairEntity>(d => d.ScenarioCalculatedAttributePairId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryScenarioCalculatedAttributePairJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioEquationCalculatedAttributePairEntity>(entity =>
            {
                entity.HasKey(e => new { e.ScenarioCalculatedAttributePairId, e.EquationId });

                entity.ToTable("ScenarioCalculatedAttributePair_Equation");

                entity.HasIndex(e => e.ScenarioCalculatedAttributePairId).IsUnique();

                entity.HasIndex(e => e.EquationId).IsUnique();

                entity.HasOne(d => d.ScenarioCalculatedAttributePair)
                    .WithOne(p => p.EquationCalculatedAttributeJoin)
                    .HasForeignKey<ScenarioEquationCalculatedAttributePairEntity>(d => d.ScenarioCalculatedAttributePairId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.ScenarioCalculatedAttributePairJoin)
                    .HasForeignKey<ScenarioEquationCalculatedAttributePairEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CalculatedAttributeLibraryEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<CalculatedAttributeEntity>(entity =>
            {
                entity.HasIndex(e => e.CalculatedAttributeLibraryId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.CalculationTiming).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.CalculatedAttributeLibrary)
                    .WithMany(p => p.CalculatedAttributes)
                    .HasForeignKey(d => d.CalculatedAttributeLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.CalculatedAttributes)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CalculatedAttributeEquationCriteriaPairEntity>(entity =>
            {
                entity.ToTable("CalculatedAttributePair");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.CalculatedAttributeId);

                entity.HasOne(d => d.CalculatedAttribute)
                    .WithMany(p => p.Equations)
                    .HasForeignKey(d => d.CalculatedAttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryCalculatedAttributePairEntity>(entity =>
            {
                entity.HasKey(e => new { e.CalculatedAttributePairId, e.CriterionLibraryId });

                entity.ToTable("CalculatedAttributePair_Criteria");

                entity.HasIndex(e => e.CalculatedAttributePairId).IsUnique();

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasOne(d => d.CalculatedAttributePair)
                    .WithOne(p => p.CriterionLibraryCalculatedAttributeJoin)
                    .HasForeignKey<CriterionLibraryCalculatedAttributePairEntity>(d => d.CalculatedAttributePairId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryCalculatedAttributePairJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EquationCalculatedAttributePairEntity>(entity =>
            {
                entity.HasKey(e => new { e.CalculatedAttributePairId, e.EquationId });

                entity.ToTable("CalculatedAttributePair_Equation");

                entity.HasIndex(e => e.CalculatedAttributePairId).IsUnique();

                entity.HasIndex(e => e.EquationId).IsUnique();

                entity.HasOne(d => d.CalculatedAttributePair)
                    .WithOne(p => p.EquationCalculatedAttributeJoin)
                    .HasForeignKey<EquationCalculatedAttributePairEntity>(d => d.CalculatedAttributePairId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.CalculatedAttributePairJoin)
                    .HasForeignKey<EquationCalculatedAttributePairEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PerformanceCurveEquationEntity>(entity =>
            {
                entity.HasKey(e => new { e.PerformanceCurveId, e.EquationId });

                entity.ToTable("PerformanceCurve_Equation");

                entity.HasIndex(e => e.PerformanceCurveId).IsUnique();

                entity.HasIndex(e => e.EquationId).IsUnique();

                entity.HasOne(d => d.PerformanceCurve)
                    .WithOne(p => p.PerformanceCurveEquationJoin)
                    .HasForeignKey<PerformanceCurveEquationEntity>(d => d.PerformanceCurveId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.PerformanceCurveEquationJoin)
                    .HasForeignKey<PerformanceCurveEquationEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioPerformanceCurveEquationEntity>(entity =>
            {
                entity.HasKey(e => new { e.ScenarioPerformanceCurveId, e.EquationId });

                entity.ToTable("ScenarioPerformanceCurve_Equation");

                entity.HasIndex(e => e.ScenarioPerformanceCurveId).IsUnique();

                entity.HasIndex(e => e.EquationId).IsUnique();

                entity.HasOne(d => d.ScenarioPerformanceCurve)
                    .WithOne(p => p.ScenarioPerformanceCurveEquationJoin)
                    .HasForeignKey<ScenarioPerformanceCurveEquationEntity>(d => d.ScenarioPerformanceCurveId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.ScenarioPerformanceCurveEquationJoin)
                    .HasForeignKey<ScenarioPerformanceCurveEquationEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PerformanceCurveLibraryEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<RemainingLifeLimitEntity>(entity =>
            {
                entity.HasIndex(e => e.RemainingLifeLimitLibraryId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Value).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.RemainingLifeLimitLibrary)
                    .WithMany(p => p.RemainingLifeLimits)
                    .HasForeignKey(d => d.RemainingLifeLimitLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.RemainingLifeLimits)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ScenarioRemainingLifeLimitEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Value).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.RemainingLifeLimits)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.ScenarioRemainingLifeLimits)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RemainingLifeLimitLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<AnalysisMaintainableAssetEntity>(entity =>
            {
                entity.HasIndex(e => e.NetworkId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.AnalysisMaintainableAssets)
                    .HasForeignKey(d => d.NetworkId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SelectableTreatmentEntity>(entity =>
            {
                entity.HasIndex(e => e.TreatmentLibraryId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.ShadowForAnyTreatment).IsRequired();

                entity.Property(e => e.ShadowForSameTreatment).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Category)
                .HasDefaultValue(TreatmentCategory.Preservation)
                .HasConversion(
                    v => v.ToString(),
                    v => string.IsNullOrWhiteSpace(v) || string.IsNullOrEmpty(v)
                        ? TreatmentCategory.Preservation
                        : (TreatmentCategory)Enum.Parse(typeof(TreatmentCategory), v));

                entity.Property(e => e.AssetType)
                .HasDefaultValue(AssetCategory.Bridge)
                .HasConversion(
                    v => v.ToString(),
                    v => string.IsNullOrWhiteSpace(v) || string.IsNullOrEmpty(v)
                        ? AssetCategory.Bridge
                        : (AssetCategory)Enum.Parse(typeof(AssetCategory), v));

                entity.HasOne(d => d.TreatmentLibrary)
                    .WithMany(p => p.Treatments)
                    .HasForeignKey(d => d.TreatmentLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioSelectableTreatmentEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.ShadowForAnyTreatment).IsRequired();
                entity.Property(e => e.ShadowForSameTreatment).IsRequired();
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Category)
                .HasDefaultValue(TreatmentCategory.Preservation)
                .HasConversion(
                    v => v.ToString(),
                    v => string.IsNullOrWhiteSpace(v) || string.IsNullOrEmpty(v)
                        ? TreatmentCategory.Preservation
                        : (TreatmentCategory)Enum.Parse(typeof(TreatmentCategory), v));

                entity.Property(e => e.AssetType)
                .HasDefaultValue(AssetCategory.Bridge)
                .HasConversion(
                    v => v.ToString(),
                    v => string.IsNullOrWhiteSpace(v) || string.IsNullOrEmpty(v)
                        ? AssetCategory.Bridge
                        : (AssetCategory)Enum.Parse(typeof(AssetCategory), v));

                entity.HasOne(d => d.Simulation)
                .WithMany(p => p.SelectableTreatments)
                .HasForeignKey(d => d.SimulationId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SimulationEntity>(entity =>
            {
                entity.HasIndex(e => e.NetworkId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.NumberOfYearsOfTreatmentOutlook).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.Simulations)
                    .HasForeignKey(d => d.NetworkId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SimulationLogEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.SimulationId);

                entity.HasOne(e => e.Simulation)
                .WithMany(s => s.SimulationLogs)
                .HasForeignKey(e => e.SimulationId);
            });

            modelBuilder.Entity<SimulationOutputEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Id).IsUnique();
                entity.HasIndex(e => e.SimulationId);

                entity.Property(e => e.InitialConditionOfNetwork).IsRequired();

                entity.HasOne(e => e.Simulation)
                    .WithMany(p => p.SimulationOutputs)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TargetConditionGoalEntity>(entity =>
            {
                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Target).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.TargetConditionGoalLibrary)
                    .WithMany(p => p.TargetConditionGoals)
                    .HasForeignKey(d => d.TargetConditionGoalLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioTargetConditionGoalEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);
                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Target).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.ScenarioTargetConditionalGoals)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TargetConditionGoalLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ScenarioSelectableTreatmentScenarioBudgetEntity>(entity =>
            {
                entity.HasKey(e => new { TreatmentId = e.ScenarioSelectableTreatmentId, e.ScenarioBudgetId });

                entity.ToTable("ScenarioSelectableTreatment_ScenarioBudget");

                entity.HasIndex(e => e.ScenarioSelectableTreatmentId);

                entity.HasIndex(e => e.ScenarioBudgetId);

                entity.HasOne(d => d.ScenarioBudget)
                    .WithMany(p => p.ScenarioSelectableTreatmentScenarioBudgetJoins)
                    .HasForeignKey(d => d.ScenarioBudgetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ScenarioSelectableTreatment)
                    .WithMany(p => p.ScenarioSelectableTreatmentScenarioBudgetJoins)
                    .HasForeignKey(d => d.ScenarioSelectableTreatmentId)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });

            modelBuilder.Entity<ConditionalTreatmentConsequenceEntity>(entity =>
            {
                entity.HasIndex(e => e.SelectableTreatmentId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.SelectableTreatment)
                    .WithMany(p => p.TreatmentConsequences)
                    .HasForeignKey(d => d.SelectableTreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.TreatmentConsequences)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioConditionalTreatmentConsequenceEntity>(entity =>
            {
                entity.HasIndex(e => e.ScenarioSelectableTreatmentId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ScenarioSelectableTreatment)
                    .WithMany(p => p.ScenarioTreatmentConsequences)
                    .HasForeignKey(d => d.ScenarioSelectableTreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.ScenarioTreatmentConsequences)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ConditionalTreatmentConsequenceEquationEntity>(entity =>
            {
                entity.HasKey(e => new { TreatmentConsequenceId = e.ConditionalTreatmentConsequenceId, e.EquationId });

                entity.ToTable("TreatmentConsequence_Equation");

                entity.HasIndex(e => e.ConditionalTreatmentConsequenceId).IsUnique();

                entity.HasIndex(e => e.EquationId).IsUnique();

                entity.HasOne(d => d.ConditionalTreatmentConsequence)
                    .WithOne(p => p.ConditionalTreatmentConsequenceEquationJoin)
                    .HasForeignKey<ConditionalTreatmentConsequenceEquationEntity>(d => d.ConditionalTreatmentConsequenceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.ConditionalTreatmentConsequenceEquationJoin)
                    .HasForeignKey<ConditionalTreatmentConsequenceEquationEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioConditionalTreatmentConsequenceEquationEntity>(entity =>
            {
                entity.HasKey(e => new { TreatmentConsequenceId = e.ScenarioConditionalTreatmentConsequenceId, e.EquationId });

                entity.ToTable("ScenarioTreatmentConsequence_Equation");

                entity.HasIndex(e => e.ScenarioConditionalTreatmentConsequenceId).IsUnique();

                entity.HasIndex(e => e.EquationId).IsUnique();

                entity.HasOne(d => d.ScenarioConditionalTreatmentConsequence)
                    .WithOne(p => p.ScenarioConditionalTreatmentConsequenceEquationJoin)
                    .HasForeignKey<ScenarioConditionalTreatmentConsequenceEquationEntity>(d => d.ScenarioConditionalTreatmentConsequenceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.ScenarioConditionalTreatmentConsequenceEquationJoin)
                    .HasForeignKey<ScenarioConditionalTreatmentConsequenceEquationEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TreatmentCostEntity>(entity =>
            {
                entity.HasIndex(e => e.TreatmentId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.SelectableTreatment)
                    .WithMany(p => p.TreatmentCosts)
                    .HasForeignKey(d => d.TreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioTreatmentCostEntity>(entity =>
            {
                entity.HasIndex(e => e.ScenarioSelectableTreatmentId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ScenarioSelectableTreatment)
                    .WithMany(p => p.ScenarioTreatmentCosts)
                    .HasForeignKey(d => d.ScenarioSelectableTreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TreatmentCostEquationEntity>(entity =>
            {
                entity.HasKey(e => new { e.TreatmentCostId, e.EquationId });

                entity.ToTable("TreatmentCost_Equation");

                entity.HasIndex(e => e.TreatmentCostId).IsUnique();

                entity.HasIndex(e => e.EquationId).IsUnique();

                entity.HasOne(d => d.TreatmentCost)
                    .WithOne(p => p.TreatmentCostEquationJoin)
                    .HasForeignKey<TreatmentCostEquationEntity>(d => d.TreatmentCostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.TreatmentCostEquationJoin)
                    .HasForeignKey<TreatmentCostEquationEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioTreatmentCostEquationEntity>(entity =>
            {
                entity.HasKey(e => new { e.ScenarioTreatmentCostId, e.EquationId });

                entity.ToTable("ScenarioTreatmentCost_Equation");

                entity.HasIndex(e => e.ScenarioTreatmentCostId).IsUnique();

                entity.HasIndex(e => e.EquationId).IsUnique();

                entity.HasOne(d => d.ScenarioTreatmentCost)
                    .WithOne(p => p.ScenarioTreatmentCostEquationJoin)
                    .HasForeignKey<ScenarioTreatmentCostEquationEntity>(d => d.ScenarioTreatmentCostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.ScenarioTreatmentCostEquationJoin)
                    .HasForeignKey<ScenarioTreatmentCostEquationEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TreatmentLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TreatmentSchedulingEntity>(entity =>
            {
                entity.HasIndex(e => e.TreatmentId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.OffsetToFutureYear).IsRequired();

                entity.HasOne(d => d.SelectableTreatment)
                    .WithMany(p => p.TreatmentSchedulings)
                    .HasForeignKey(d => d.TreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioTreatmentSchedulingEntity>(entity =>
            {
                entity.HasIndex(e => e.TreatmentId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.OffsetToFutureYear).IsRequired();

                entity.HasOne(d => d.ScenarioSelectableTreatment)
                    .WithMany(p => p.ScenarioTreatmentSchedulings)
                    .HasForeignKey(d => d.TreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TreatmentSupersessionEntity>(entity =>
            {
                entity.HasIndex(e => e.TreatmentId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.SelectableTreatment)
                    .WithMany(p => p.TreatmentSupersessions)
                    .HasForeignKey(d => d.TreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ScenarioTreatmentSupersessionEntity>(entity =>
            {
                entity.HasIndex(e => e.TreatmentId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.ScenarioSelectableTreatment)
                    .WithMany(p => p.ScenarioTreatmentSupersessions)
                    .HasForeignKey(d => d.TreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NumericAttributeValueHistoryEntity>(entity =>
            {
                entity.HasIndex(e => e.SectionId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Year).IsRequired();

                entity.Property(e => e.Value).IsRequired();

                entity.HasOne(d => d.AnalysisMaintainableAsset)
                    .WithMany(p => p.NumericAttributeValueHistories)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.NumericAttributeValueHistories)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TextAttributeValueHistoryEntity>(entity =>
            {
                entity.HasIndex(e => e.SectionId);

                entity.HasIndex(e => e.AttributeId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Year).IsRequired();

                entity.HasOne(d => d.AnalysisMaintainableAsset)
                    .WithMany(p => p.TextAttributeValueHistories)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Attribute)
                    .WithMany(p => p.TextAttributeValueHistories)
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SimulationAnalysisDetailEntity>(entity =>
            {
                entity.HasKey(e => e.SimulationId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.SimulationAnalysisDetail)
                    .HasForeignKey<SimulationAnalysisDetailEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<SimulationUserEntity>(entity =>
            {
                entity.HasKey(e => new { e.SimulationId, e.UserId });

                entity.ToTable("Simulation_User");

                entity.HasIndex(e => e.SimulationId);

                entity.HasIndex(e => e.UserId);

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.SimulationUserJoins)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SimulationUserJoins)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CriterionLibraryUserEntity>(entity =>
            {
                entity.HasKey(e => new { e.CriterionLibraryId, e.UserId });

                entity.ToTable("CriterionLibrary_User");

                entity.HasIndex(e => e.CriterionLibraryId);

                entity.HasIndex(e => e.UserId).IsUnique();

                entity.HasOne(d => d.CriterionLibrary)
                    .WithMany(p => p.CriterionLibraryUserJoins)
                    .HasForeignKey(d => d.CriterionLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.CriterionLibraryUserJoin)
                    .HasForeignKey<CriterionLibraryUserEntity>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NetworkRollupDetailEntity>(entity =>
            {
                entity.HasKey(e => e.NetworkId);

                entity.HasIndex(e => e.NetworkId).IsUnique();

                entity.HasOne(d => d.Network)
                    .WithOne(p => p.NetworkRollupDetail)
                    .HasForeignKey<NetworkRollupDetailEntity>(d => d.NetworkId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SimulationReportDetailEntity>(entity =>
            {
                entity.HasKey(e => e.SimulationId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.SimulationReportDetail)
                    .HasForeignKey<SimulationReportDetailEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BenefitQuantifierEntity>(entity =>
            {
                entity.HasKey(e => e.NetworkId);

                entity.HasIndex(e => e.NetworkId);

                entity.HasIndex(e => e.EquationId);

                entity.HasOne(d => d.Network)
                    .WithOne(p => p.BenefitQuantifier)
                    .HasForeignKey<BenefitQuantifierEntity>(d => d.NetworkId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Equation)
                    .WithOne(p => p.BenefitQuantifier)
                    .HasForeignKey<BenefitQuantifierEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserCriteriaFilterEntity>(entity =>
            {
                entity.HasKey(e => e.UserCriteriaId);

                entity.ToTable("UserCriteria_Filter");

                entity.HasIndex(e => e.UserCriteriaId).IsUnique();

                entity.HasOne(d => d.User)
                .WithOne(p => p.UserCriteriaFilterJoin)
                .HasForeignKey<UserCriteriaFilterEntity>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ReportIndexEntity>(entity =>
            {
                entity.Property(e => e.ReportTypeName).IsRequired();

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.SimulationReports)
                    .HasForeignKey(d => d.SimulationID)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<AnnouncementEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Content).IsRequired();
            });

            modelBuilder.Entity<SimulationLogEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(d => d.Simulation)
                .WithMany(p => p.SimulationLogs)
                .HasForeignKey(d => d.SimulationId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AssetSummaryDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.HasOne(e => e.SimulationOutput)
                .WithMany(so => so.InitialAssetSummaries)
                .HasForeignKey(a => a.SimulationOutputId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MaintainableAsset)
                .WithMany(ma => ma.AssetSummaryDetails)
                .HasForeignKey(e => e.MaintainableAssetId)
                .OnDelete(DeleteBehavior.ClientCascade);
            });

            modelBuilder.Entity<AssetDetailValueEntityIntId>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.HasOne(a => a.Attribute)
                .WithMany(a => a.AssetDetailValuesIntId)
                .HasForeignKey(a => a.AttributeId)
                .OnDelete(DeleteBehavior.ClientCascade)
                ;
                entity.HasIndex(e => e.AttributeId);

                entity.HasOne(e => e.AssetDetail)
                .WithMany(a => a.AssetDetailValuesIntId)
                .HasForeignKey(e => e.AssetDetailId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AssetSummaryDetailValueEntityIntId>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.HasOne(a => a.Attribute)
                .WithMany(a => a.AssetSummaryDetailValuesIntId)
                .HasForeignKey(a => a.AttributeId)
                .OnDelete(DeleteBehavior.ClientCascade)
                ;
                entity.HasIndex(e => e.AttributeId);

                entity.HasOne(e => e.AssetSummaryDetail)
                .WithMany(a => a.AssetSummaryDetailValuesIntId)
                .HasForeignKey(e => e.AssetSummaryDetailId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SimulationYearDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.HasOne(e => e.SimulationOutput)
                .WithMany(so => so.Years)
                .HasForeignKey(a => a.SimulationOutputId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.ConditionOfNetwork)
                .IsRequired();

                entity.Property(e => e.Year)
                .IsRequired();
            });

            modelBuilder.Entity<BudgetDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.Property(e => e.AvailableFunding).IsRequired();

                entity.Property(e => e.BudgetName).IsRequired();

                entity.HasOne(e => e.SimulationYearDetail)
                .WithMany(sy => sy.Budgets)
                .HasForeignKey(e => e.SimulationYearDetailId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DeficientConditionGoalDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.Property(e => e.ActualDeficientPercentage)
                .IsRequired();

                entity.Property(e => e.AllowedDeficientPercentage)
                .IsRequired();

                entity.Property(e => e.DeficientLimit).IsRequired();

                entity.Property(e => e.GoalIsMet).IsRequired();

                entity.HasOne(e => e.Attribute)
                .WithMany(a => a.DeficientConditionGoalDetails)
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SimulationYearDetail)
                .WithMany(sy => sy.DeficientConditionGoalDetails)
                .HasForeignKey(e => e.SimulationYearDetailId)
                .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<TargetConditionGoalDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.Property(e => e.GoalIsMet).IsRequired();

                entity.Property(e => e.ActualValue).IsRequired();

                entity.Property(e => e.TargetValue).IsRequired();

                entity.HasOne(e => e.SimulationYearDetail)
                .WithMany(sy => sy.TargetConditionGoalDetails)
                .HasForeignKey(e => e.SimulationYearDetailId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Attribute)
                .WithMany(a => a.TargetConditionGoalDetails)
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AssetDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(e => e.TreatmentCause).IsRequired();

                entity.Property(e => e.TreatmentFundingIgnoresSpendingLimit).IsRequired();

                entity.Property(e => e.TreatmentStatus).IsRequired();

                entity.HasOne(e => e.SimulationYearDetail)
                .WithMany(sy => sy.Assets)
                .HasForeignKey(e => e.SimulationYearDetailId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.MaintainableAsset)
                .WithMany(ma => ma.AssetDetails)
                .HasForeignKey(e => e.MaintainableAssetId)
                .OnDelete(DeleteBehavior.ClientCascade);
            });

            modelBuilder.Entity<BudgetUsageDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.Property(e => e.CoveredCost).IsRequired();

                entity.Property(e => e.Status).IsRequired();

                entity.HasOne(e => e.TreatmentConsiderationDetail)
                .WithMany(tc => tc.BudgetUsageDetails)
                .HasForeignKey(e => e.TreatmentConsiderationDetailId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CashFlowConsiderationDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.Property(e => e.ReasonAgainstCashFlow).IsRequired();

                entity.HasOne(e => e.TreatmentConsiderationDetail)
                .WithMany(tc => tc.CashFlowConsiderationDetails)
                .HasForeignKey(e => e.TreatmentConsiderationDetailId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TreatmentConsiderationDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.HasOne(e => e.AssetDetail)
                .WithMany(ad => ad.TreatmentConsiderationDetails)
                .HasForeignKey(e => e.AssetDetailId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TreatmentOptionDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.Property(e => e.Benefit).IsRequired();

                entity.Property(e => e.Cost).IsRequired();

                entity.HasOne(e => e.AssetDetail)
                .WithMany(ad => ad.TreatmentOptionDetails)
                .HasForeignKey(e => e.AssetDetailId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TreatmentRejectionDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.Property(e => e.TreatmentRejectionReason).IsRequired();

                entity.HasOne(e => e.AssetDetail)
                .WithMany(ad => ad.TreatmentRejectionDetails)
                .HasForeignKey(e => e.AssetDetailId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TreatmentSchedulingCollisionDetailEntity>(entity =>
            {
                entity.Property(e => e.Id).IsRequired();
                entity.HasIndex(e => e.Id).IsUnique();

                entity.HasOne(e => e.AssetDetail)
                .WithMany(ad => ad.TreatmentSchedulingCollisionDetails)
                .HasForeignKey(e => e.AssetDetailId)
                .OnDelete(DeleteBehavior.Cascade);
            });



            modelBuilder.Entity<BudgetLibraryUserEntity>(entity =>
            {
                entity.HasKey(e => new { e.BudgetLibraryId, e.UserId });

                entity.ToTable("BudgetLibrary_User");

                entity.HasIndex(e => e.BudgetLibraryId);

                entity.HasIndex(e => e.UserId);

                entity.HasOne(d => d.BudgetLibrary)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.BudgetLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BudgetLibraryUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
