using System;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class IAMContext : DbContext
    {
        public static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

        public IAMContext() { }

        public IAMContext(DbContextOptions<IAMContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!IsRunningFromNUnit)
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Repositories\\MSSQL", "migrationConnection.json");
                string contents = File.ReadAllText(filePath);
                var migrationConnection = JsonConvert
                    .DeserializeAnonymousType(contents, new { ConnectionStrings = default(MigrationConnection) })
                    .ConnectionStrings;

                optionsBuilder.UseSqlServer(migrationConnection.BridgeCareConnex);
            }
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

        public virtual DbSet<BudgetLibrarySimulationEntity> BudgetLibrarySimulation { get; set; }

        public virtual DbSet<BudgetAmountEntity> BudgetAmount { get; set; }

        public virtual DbSet<BudgetPercentagePairEntity> BudgetPercentagePair { get; set; }

        public virtual DbSet<BudgetPriorityEntity> BudgetPriority { get; set; }

        public virtual DbSet<BudgetPriorityLibraryEntity> BudgetPriorityLibrary { get; set; }

        public virtual DbSet<BudgetPriorityLibrarySimulationEntity> BudgetPriorityLibrarySimulation { get; set; }

        public virtual DbSet<CashFlowDistributionRuleEntity> CashFlowDistributionRule { get; set; }

        public virtual DbSet<CashFlowRuleEntity> CashFlowRule { get; set; }

        public virtual DbSet<CashFlowRuleLibraryEntity> CashFlowRuleLibrary { get; set; }

        public virtual DbSet<CashFlowRuleLibrarySimulationEntity> CashFlowRuleLibrarySimulation { get; set; }

        public virtual DbSet<CommittedProjectEntity> CommittedProject { get; set; }

        public virtual DbSet<CommittedProjectConsequenceEntity> CommittedProjectConsequence { get; set; }

        public virtual DbSet<CriterionLibraryEntity> CriterionLibrary { get; set; }

        public virtual DbSet<CriterionLibraryAnalysisMethodEntity> CriterionLibraryAnalysisMethod { get; set; }

        public virtual DbSet<CriterionLibraryBudgetEntity> CriterionLibraryBudget { get; set; }

        public virtual DbSet<CriterionLibraryBudgetPriorityEntity> CriterionLibraryBudgetPriority { get; set; }

        public virtual DbSet<CriterionLibraryCashFlowRuleEntity> CriterionLibraryCashFlowRule { get; set; }

        public virtual DbSet<CriterionLibraryDeficientConditionGoalEntity> CriterionLibraryDeficientConditionGoal { get; set; }

        public virtual DbSet<CriterionLibraryPerformanceCurveEntity> CriterionLibraryPerformanceCurve { get; set; }

        public virtual DbSet<CriterionLibraryRemainingLifeLimitEntity> CriterionLibraryRemainingLifeLimit { get; set; }

        public virtual DbSet<CriterionLibraryTargetConditionGoalEntity> CriterionLibraryTargetConditionGoal { get; set; }

        public virtual DbSet<CriterionLibrarySelectableTreatmentEntity> CriterionLibrarySelectableTreatment { get; set; }

        public virtual DbSet<CriterionLibraryConditionalTreatmentConsequenceEntity> CriterionLibraryTreatmentConsequence { get; set; }

        public virtual DbSet<CriterionLibraryTreatmentCostEntity> CriterionLibraryTreatmentCost { get; set; }

        public virtual DbSet<CriterionLibraryTreatmentSupersessionEntity> CriterionLibraryTreatmentSupersession { get; set; }

        public virtual DbSet<DeficientConditionGoalEntity> DeficientConditionGoal { get; set; }

        public virtual DbSet<DeficientConditionGoalLibraryEntity> DeficientConditionGoalLibrary { get; set; }

        public virtual DbSet<DeficientConditionGoalLibrarySimulationEntity> DeficientConditionGoalLibrarySimulation { get; set; }

        public virtual DbSet<EquationEntity> Equation { get; set; }

        public virtual DbSet<FacilityEntity> Facility { get; set; }

        public virtual DbSet<InvestmentPlanEntity> InvestmentPlan { get; set; }

        public virtual DbSet<MaintainableAssetEntity> MaintainableAsset { get; set; }

        public virtual DbSet<MaintainableAssetLocationEntity> MaintainableAssetLocation { get; set; }

        public virtual DbSet<NetworkEntity> Network { get; set; }

        public virtual DbSet<PerformanceCurveEntity> PerformanceCurve { get; set; }

        public virtual DbSet<PerformanceCurveEquationEntity> PerformanceCurveEquation { get; set; }

        public virtual DbSet<PerformanceCurveLibraryEntity> PerformanceCurveLibrary { get; set; }

        public virtual DbSet<PerformanceCurveLibrarySimulationEntity> PerformanceCurveLibrarySimulation { get; set; }

        public virtual DbSet<RemainingLifeLimitEntity> RemainingLifeLimit { get; set; }

        public virtual DbSet<RemainingLifeLimitLibraryEntity> RemainingLifeLimitLibrary { get; set; }

        public virtual DbSet<RemainingLifeLimitLibrarySimulationEntity> RemainingLifeLimitLibrarySimulation { get; set; }

        public virtual DbSet<SectionEntity> Section { get; set; }

        public virtual DbSet<SimulationEntity> Simulation { get; set; }

        public virtual DbSet<SimulationOutputEntity> SimulationOutput { get; set; }

        public virtual DbSet<TargetConditionGoalEntity> TargetConditionGoal { get; set; }

        public virtual DbSet<TargetConditionGoalLibraryEntity> TargetConditionGoalLibrary { get; set; }

        public virtual DbSet<TargetConditionGoalLibrarySimulationEntity> TargetConditionGoalLibrarySimulation { get; set; }

        public virtual DbSet<SelectableTreatmentEntity> SelectableTreatment { get; set; }

        public virtual DbSet<SelectableTreatmentBudgetEntity> TreatmentBudget { get; set; }

        public virtual DbSet<ConditionalTreatmentConsequenceEntity> TreatmentConsequence { get; set; }

        public virtual DbSet<ConditionalTreatmentConsequenceEquationEntity> TreatmentConsequenceEquation { get; set; }

        public virtual DbSet<TreatmentCostEntity> TreatmentCost { get; set; }

        public virtual DbSet<TreatmentCostEquationEntity> TreatmentCostEquation { get; set; }

        public virtual DbSet<TreatmentLibraryEntity> TreatmentLibrary { get; set; }

        public virtual DbSet<TreatmentLibrarySimulationEntity> TreatmentLibrarySimulation { get; set; }

        public virtual DbSet<TreatmentSchedulingEntity> TreatmentScheduling { get; set; }

        public virtual DbSet<TreatmentSupersessionEntity> TreatmentSupersession { get; set; }

        public virtual DbSet<NumericAttributeValueHistoryEntity> NumericAttributeValueHistory { get; set; }

        public virtual DbSet<TextAttributeValueHistoryEntity> TextAttributeValueHistory { get; set; }

        public virtual DbSet<SimulationAnalysisDetailEntity> SimulationAnalysisDetail { get; set; }

        public virtual DbSet<UserEntity> User { get; set; }

        public virtual DbSet<SimulationUserEntity> SimulationUser { get; set; }

        public virtual DbSet<CriterionLibraryUserEntity> CriterionLibraryUser { get; set; }

        public virtual DbSet<NetworkRollupDetailEntity> NetworkRollupDetail { get; set; }

        public virtual DbSet<SimulationReportDetailEntity> SimulationReportDetail { get; set; }

        public virtual DbSet<UserCriteriaFilterEntity> UserCriteria { get; set; }

        public virtual DbSet<ReportIndexEntity> ReportIndex { get; set; }

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
                    .OnDelete(DeleteBehavior.SetNull);
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

            modelBuilder.Entity<BudgetLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<BudgetLibrarySimulationEntity>(entity =>
            {
                entity.HasKey(e => new { e.BudgetLibraryId, e.SimulationId });

                entity.ToTable("BudgetLibrary_Simulation");

                entity.HasIndex(e => e.BudgetLibraryId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.BudgetLibrary)
                    .WithMany(p => p.BudgetLibrarySimulationJoins)
                    .HasForeignKey(d => d.BudgetLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.BudgetLibrarySimulationJoin)
                    .HasForeignKey<BudgetLibrarySimulationEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<BudgetPercentagePairEntity>(entity =>
            {
                entity.HasIndex(e => e.BudgetId);

                entity.HasIndex(e => e.BudgetPriorityId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Percentage).IsRequired();

                entity.HasOne(d => d.Budget)
                    .WithMany(p => p.BudgetPercentagePairs)
                    .HasForeignKey(d => d.BudgetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.BudgetPriority)
                    .WithMany(p => p.BudgetPercentagePairs)
                    .HasForeignKey(d => d.BudgetPriorityId)
                    .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<BudgetPriorityLibraryEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<BudgetPriorityLibrarySimulationEntity>(entity =>
            {
                entity.HasKey(e => new { e.BudgetPriorityLibraryId, e.SimulationId });

                entity.ToTable("BudgetPriorityLibrary_Simulation");

                entity.HasIndex(e => e.BudgetPriorityLibraryId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.BudgetPriorityLibrary)
                    .WithMany(p => p.BudgetPriorityLibrarySimulationJoins)
                    .HasForeignKey(d => d.BudgetPriorityLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.BudgetPriorityLibrarySimulationJoin)
                    .HasForeignKey<BudgetPriorityLibrarySimulationEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<CashFlowRuleLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<CashFlowRuleLibrarySimulationEntity>(entity =>
            {
                entity.HasKey(e => new { e.CashFlowRuleLibraryId, e.SimulationId });

                entity.ToTable("CashFlowRuleLibrary_Simulation");

                entity.HasIndex(e => e.CashFlowRuleLibraryId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.CashFlowRuleLibrary)
                    .WithMany(p => p.CashFlowRuleLibrarySimulationJoins)
                    .HasForeignKey(d => d.CashFlowRuleLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.CashFlowRuleLibrarySimulationJoin)
                    .HasForeignKey<CashFlowRuleLibrarySimulationEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CommittedProjectEntity>(entity =>
            {
                entity.HasIndex(e => e.SimulationId);

                entity.HasIndex(e => e.BudgetId);

                entity.HasIndex(e => e.SectionId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.ShadowForAnyTreatment).IsRequired();

                entity.Property(e => e.ShadowForSameTreatment).IsRequired();

                entity.Property(e => e.Cost).IsRequired();

                entity.Property(e => e.Year).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Simulation)
                    .WithMany(p => p.CommittedProjects)
                    .HasForeignKey(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Budget)
                    .WithMany(p => p.CommittedProjects)
                    .HasForeignKey(d => d.BudgetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.CommittedProjects)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientCascade);
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

            modelBuilder.Entity<DeficientConditionGoalLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DeficientConditionGoalLibrarySimulationEntity>(entity =>
            {
                entity.HasKey(e => new { e.DeficientConditionGoalLibraryId, e.SimulationId });

                entity.ToTable("DeficientConditionGoalLibrary_Simulation");

                entity.HasIndex(e => e.DeficientConditionGoalLibraryId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.DeficientConditionGoalLibrary)
                    .WithMany(p => p.DeficientConditionGoalLibrarySimulationJoins)
                    .HasForeignKey(d => d.DeficientConditionGoalLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.DeficientConditionGoalLibrarySimulationJoin)
                    .HasForeignKey<DeficientConditionGoalLibrarySimulationEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EquationEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Expression).IsRequired();

                /*entity.HasOne(d => d.AttributeEquationCriterionLibraryJoin)
                    .WithOne(p => p.Equation)
                    .HasForeignKey<AttributeEquationCriterionLibraryEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ConditionalTreatmentConsequenceEquationJoin)
                    .WithOne(p => p.Equation)
                    .HasForeignKey<ConditionalTreatmentConsequenceEquationEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.PerformanceCurveEquationJoin)
                    .WithOne(p => p.Equation)
                    .HasForeignKey<PerformanceCurveEquationEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.TreatmentCostEquationJoin)
                    .WithOne(p => p.Equation)
                    .HasForeignKey<TreatmentCostEquationEntity>(d => d.EquationId)
                    .OnDelete(DeleteBehavior.Cascade);*/
            });

            modelBuilder.Entity<FacilityEntity>(entity =>
            {
                entity.HasIndex(e => e.NetworkId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.Facilities)
                    .HasForeignKey(d => d.NetworkId)
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

            modelBuilder.Entity<PerformanceCurveLibraryEntity>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<PerformanceCurveLibrarySimulationEntity>(entity =>
            {
                entity.HasKey(e => new { e.PerformanceCurveLibraryId, e.SimulationId });

                entity.ToTable("PerformanceCurveLibrary_Simulation");

                entity.HasIndex(e => e.PerformanceCurveLibraryId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.PerformanceCurveLibrary)
                    .WithMany(p => p.PerformanceCurveLibrarySimulationJoins)
                    .HasForeignKey(d => d.PerformanceCurveLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.PerformanceCurveLibrarySimulationJoin)
                    .HasForeignKey<PerformanceCurveLibrarySimulationEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<RemainingLifeLimitLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<RemainingLifeLimitLibrarySimulationEntity>(entity =>
            {
                entity.HasKey(e => new { e.RemainingLifeLimitLibraryId, e.SimulationId });

                entity.ToTable("RemainingLifeLimitLibrary_Simulation");

                entity.HasIndex(e => e.RemainingLifeLimitLibraryId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.RemainingLifeLimitLibrary)
                    .WithMany(p => p.RemainingLifeLimitLibrarySimulationJoins)
                    .HasForeignKey(d => d.RemainingLifeLimitLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.RemainingLifeLimitLibrarySimulationJoin)
                    .HasForeignKey<RemainingLifeLimitLibrarySimulationEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SectionEntity>(entity =>
            {
                entity.HasIndex(e => e.FacilityId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Facility)
                    .WithMany(p => p.Sections)
                    .HasForeignKey(d => d.FacilityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SelectableTreatmentEntity>(entity =>
            {
                entity.HasIndex(e => e.TreatmentLibraryId);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.ShadowForAnyTreatment).IsRequired();

                entity.Property(e => e.ShadowForSameTreatment).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.TreatmentLibrary)
                    .WithMany(p => p.Treatments)
                    .HasForeignKey(d => d.TreatmentLibraryId)
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

            modelBuilder.Entity<SimulationOutputEntity>(entity =>
            {
                entity.HasKey(e => e.SimulationId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.SimulationOutput)
                    .HasForeignKey<SimulationOutputEntity>(d => d.SimulationId)
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

            modelBuilder.Entity<TargetConditionGoalLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TargetConditionGoalLibrarySimulationEntity>(entity =>
            {
                entity.HasKey(e => new { e.TargetConditionGoalLibraryId, e.SimulationId });

                entity.ToTable("TargetConditionGoalLibrary_Simulation");

                entity.HasIndex(e => e.TargetConditionGoalLibraryId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.TargetConditionGoalLibrarySimulationJoin)
                    .HasForeignKey<TargetConditionGoalLibrarySimulationEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.TargetConditionGoalLibrary)
                    .WithMany(p => p.TargetConditionGoalLibrarySimulationJoins)
                    .HasForeignKey(d => d.TargetConditionGoalLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SelectableTreatmentBudgetEntity>(entity =>
            {
                entity.HasKey(e => new { TreatmentId = e.SelectableTreatmentId, e.BudgetId });

                entity.ToTable("Treatment_Budget");

                entity.HasIndex(e => e.SelectableTreatmentId);

                entity.HasIndex(e => e.BudgetId);

                entity.HasOne(d => d.Budget)
                    .WithMany(p => p.TreatmentBudgetJoins)
                    .HasForeignKey(d => d.BudgetId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.SelectableTreatment)
                    .WithMany(p => p.TreatmentBudgetJoins)
                    .HasForeignKey(d => d.SelectableTreatmentId)
                    .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<TreatmentCostEntity>(entity =>
            {
                entity.HasIndex(e => e.TreatmentId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.SelectableTreatment)
                    .WithMany(p => p.TreatmentCosts)
                    .HasForeignKey(d => d.TreatmentId)
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

            modelBuilder.Entity<TreatmentLibraryEntity>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<TreatmentLibrarySimulationEntity>(entity =>
            {
                entity.HasKey(e => new { e.TreatmentLibraryId, e.SimulationId });

                entity.ToTable("TreatmentLibrary_Simulation");

                entity.HasIndex(e => e.TreatmentLibraryId);

                entity.HasIndex(e => e.SimulationId).IsUnique();

                entity.HasOne(d => d.Simulation)
                    .WithOne(p => p.TreatmentLibrarySimulationJoin)
                    .HasForeignKey<TreatmentLibrarySimulationEntity>(d => d.SimulationId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.TreatmentLibrary)
                    .WithMany(p => p.TreatmentLibrarySimulationJoins)
                    .HasForeignKey(d => d.TreatmentLibraryId)
                    .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity<TreatmentSupersessionEntity>(entity =>
            {
                entity.HasIndex(e => e.TreatmentId);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(d => d.SelectableTreatment)
                    .WithMany(p => p.TreatmentSupersessions)
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

                entity.HasOne(d => d.Section)
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

                entity.HasOne(d => d.Section)
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

            modelBuilder.Entity<UserCriteriaFilterEntity>(entity =>
            {
                entity.HasKey(e => e.UserCriteriaId);

                entity.ToTable("UserCriteria_Filter");

                entity.HasIndex(e => e.UserCriteriaId).IsUnique();

                entity.HasOne(d => d.User)
                .WithOne(p => p.UserCriteriaFilterJoin)
                .HasForeignKey<UserCriteriaFilterEntity>(f => f.UserId);

            });

            modelBuilder.Entity<ReportIndexEntity>(entity =>
            {
                entity.Property(e => e.ReportTypeName).IsRequired();
            });
        }
    }
}
