using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class removeBaseEntitySuperclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TreatmentSchedulingCollisionDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TreatmentSchedulingCollisionDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "TreatmentSchedulingCollisionDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "TreatmentSchedulingCollisionDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TreatmentRejectionDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TreatmentRejectionDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "TreatmentRejectionDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "TreatmentRejectionDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TreatmentOptionDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TreatmentOptionDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "TreatmentOptionDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "TreatmentOptionDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TreatmentConsiderationDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TreatmentConsiderationDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "TreatmentConsiderationDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "TreatmentConsiderationDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TargetConditionGoalDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TargetConditionGoalDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "TargetConditionGoalDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "TargetConditionGoalDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SimulationYearDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SimulationYearDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "SimulationYearDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "SimulationYearDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DeficientConditionGoalDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DeficientConditionGoalDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "DeficientConditionGoalDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "DeficientConditionGoalDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CashFlowConsiderationDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CashFlowConsiderationDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "CashFlowConsiderationDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "CashFlowConsiderationDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BudgetUsageDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BudgetUsageDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "BudgetUsageDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "BudgetUsageDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BudgetDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BudgetDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "BudgetDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "BudgetDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetSummaryDetailValue");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AssetSummaryDetailValue");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "AssetSummaryDetailValue");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "AssetSummaryDetailValue");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetSummaryDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AssetSummaryDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "AssetSummaryDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "AssetSummaryDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetDetailValue");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AssetDetailValue");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "AssetDetailValue");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "AssetDetailValue");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AssetDetail");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AssetDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "AssetDetail");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "AssetDetail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "TreatmentSchedulingCollisionDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TreatmentSchedulingCollisionDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "TreatmentSchedulingCollisionDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "TreatmentSchedulingCollisionDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "TreatmentRejectionDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TreatmentRejectionDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "TreatmentRejectionDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "TreatmentRejectionDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "TreatmentOptionDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TreatmentOptionDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "TreatmentOptionDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "TreatmentOptionDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "TreatmentConsiderationDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TreatmentConsiderationDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "TreatmentConsiderationDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "TreatmentConsiderationDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "TargetConditionGoalDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TargetConditionGoalDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "TargetConditionGoalDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "TargetConditionGoalDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "SimulationYearDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SimulationYearDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "SimulationYearDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "SimulationYearDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "DeficientConditionGoalDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DeficientConditionGoalDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "DeficientConditionGoalDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "DeficientConditionGoalDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "CashFlowConsiderationDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CashFlowConsiderationDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "CashFlowConsiderationDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "CashFlowConsiderationDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "BudgetUsageDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BudgetUsageDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "BudgetUsageDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "BudgetUsageDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "BudgetDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BudgetDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "BudgetDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "BudgetDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AssetSummaryDetailValue",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AssetSummaryDetailValue",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "AssetSummaryDetailValue",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "AssetSummaryDetailValue",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AssetSummaryDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AssetSummaryDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "AssetSummaryDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "AssetSummaryDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AssetDetailValue",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AssetDetailValue",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "AssetDetailValue",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "AssetDetailValue",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AssetDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AssetDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "AssetDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "AssetDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
