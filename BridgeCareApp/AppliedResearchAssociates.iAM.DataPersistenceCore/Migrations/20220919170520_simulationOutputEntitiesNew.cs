using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class simulationOutputEntitiesNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Output",
                table: "SimulationOutput");

            migrationBuilder.DropColumn(
                name: "OutputType",
                table: "SimulationOutput");

            migrationBuilder.AddColumn<double>(
                name: "InitialConditionOfNetwork",
                table: "SimulationOutput",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "AssetSummaryDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintainableAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationOutputId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSummaryDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetSummaryDetail_MaintainableAsset_MaintainableAssetId",
                        column: x => x.MaintainableAssetId,
                        principalTable: "MaintainableAsset",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetSummaryDetail_SimulationOutput_SimulationOutputId",
                        column: x => x.SimulationOutputId,
                        principalTable: "SimulationOutput",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimulationYearDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationOutputId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConditionOfNetwork = table.Column<double>(type: "float", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationYearDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimulationYearDetail_SimulationOutput_SimulationOutputId",
                        column: x => x.SimulationOutputId,
                        principalTable: "SimulationOutput",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetSummaryDetailValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetSummaryDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "char(1)", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSummaryDetailValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetSummaryDetailValue_AssetSummaryDetail_AssetSummaryDetailId",
                        column: x => x.AssetSummaryDetailId,
                        principalTable: "AssetSummaryDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetSummaryDetailValue_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintainableAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationYearDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppliedTreatment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TreatmentCause = table.Column<int>(type: "int", nullable: false),
                    TreatmentFundingIgnoresSpendingLimit = table.Column<bool>(type: "bit", nullable: false),
                    TreatmentStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetDetail_MaintainableAsset_MaintainableAssetId",
                        column: x => x.MaintainableAssetId,
                        principalTable: "MaintainableAsset",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetDetail_SimulationYearDetail_SimulationYearDetailId",
                        column: x => x.SimulationYearDetailId,
                        principalTable: "SimulationYearDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationYearDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailableFunding = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BudgetName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetDetail_SimulationYearDetail_SimulationYearDetailId",
                        column: x => x.SimulationYearDetailId,
                        principalTable: "SimulationYearDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeficientConditionGoalDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationYearDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActualDeficientPercentage = table.Column<double>(type: "float", nullable: false),
                    AllowedDeficientPercentage = table.Column<double>(type: "float", nullable: false),
                    DeficientLimit = table.Column<double>(type: "float", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalIsMet = table.Column<bool>(type: "bit", nullable: false),
                    GoalName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeficientConditionGoalDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoalDetail_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoalDetail_SimulationYearDetail_SimulationYearDetailId",
                        column: x => x.SimulationYearDetailId,
                        principalTable: "SimulationYearDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TargetConditionGoalDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationYearDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActualValue = table.Column<double>(type: "float", nullable: false),
                    TargetValue = table.Column<double>(type: "float", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoalIsMet = table.Column<bool>(type: "bit", nullable: false),
                    GoalName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetConditionGoalDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetConditionGoalDetail_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TargetConditionGoalDetail_SimulationYearDetail_SimulationYearDetailId",
                        column: x => x.SimulationYearDetailId,
                        principalTable: "SimulationYearDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetDetailValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "char(1)", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetDetailValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetDetailValue_AssetDetail_AssetDetailId",
                        column: x => x.AssetDetailId,
                        principalTable: "AssetDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetDetailValue_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TreatmentConsiderationDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetPriorityLevel = table.Column<int>(type: "int", nullable: true),
                    TreatmentName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentConsiderationDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentConsiderationDetail_AssetDetail_AssetDetailId",
                        column: x => x.AssetDetailId,
                        principalTable: "AssetDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentOptionDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Benefit = table.Column<double>(type: "float", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    RemainingLife = table.Column<double>(type: "float", nullable: true),
                    TreatmentName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentOptionDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentOptionDetail_AssetDetail_AssetDetailId",
                        column: x => x.AssetDetailId,
                        principalTable: "AssetDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentRejectionDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TreatmentRejectionReason = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentRejectionDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentRejectionDetail_AssetDetail_AssetDetailId",
                        column: x => x.AssetDetailId,
                        principalTable: "AssetDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentSchedulingCollisionDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameOfUnscheduledTreatment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentSchedulingCollisionDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentSchedulingCollisionDetail_AssetDetail_AssetDetailId",
                        column: x => x.AssetDetailId,
                        principalTable: "AssetDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetUsageDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentConsiderationDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoveredCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetUsageDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetUsageDetail_TreatmentConsiderationDetail_TreatmentConsiderationDetailId",
                        column: x => x.TreatmentConsiderationDetailId,
                        principalTable: "TreatmentConsiderationDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashFlowConsiderationDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentConsiderationDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFlowRuleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonAgainstCashFlow = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowConsiderationDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashFlowConsiderationDetail_TreatmentConsiderationDetail_TreatmentConsiderationDetailId",
                        column: x => x.TreatmentConsiderationDetailId,
                        principalTable: "TreatmentConsiderationDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetail_Id",
                table: "AssetDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetail_MaintainableAssetId",
                table: "AssetDetail",
                column: "MaintainableAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetail_SimulationYearDetailId",
                table: "AssetDetail",
                column: "SimulationYearDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValue_AssetDetailId",
                table: "AssetDetailValue",
                column: "AssetDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValue_AttributeId",
                table: "AssetDetailValue",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetDetailValue_Id",
                table: "AssetDetailValue",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetail_Id",
                table: "AssetSummaryDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetail_MaintainableAssetId",
                table: "AssetSummaryDetail",
                column: "MaintainableAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetail_SimulationOutputId",
                table: "AssetSummaryDetail",
                column: "SimulationOutputId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValue_AssetSummaryDetailId",
                table: "AssetSummaryDetailValue",
                column: "AssetSummaryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValue_AttributeId",
                table: "AssetSummaryDetailValue",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSummaryDetailValue_Id",
                table: "AssetSummaryDetailValue",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetDetail_Id",
                table: "BudgetDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetDetail_SimulationYearDetailId",
                table: "BudgetDetail",
                column: "SimulationYearDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetUsageDetail_Id",
                table: "BudgetUsageDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetUsageDetail_TreatmentConsiderationDetailId",
                table: "BudgetUsageDetail",
                column: "TreatmentConsiderationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowConsiderationDetail_Id",
                table: "CashFlowConsiderationDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowConsiderationDetail_TreatmentConsiderationDetailId",
                table: "CashFlowConsiderationDetail",
                column: "TreatmentConsiderationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalDetail_AttributeId",
                table: "DeficientConditionGoalDetail",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalDetail_Id",
                table: "DeficientConditionGoalDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalDetail_SimulationYearDetailId",
                table: "DeficientConditionGoalDetail",
                column: "SimulationYearDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationYearDetail_Id",
                table: "SimulationYearDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SimulationYearDetail_SimulationOutputId",
                table: "SimulationYearDetail",
                column: "SimulationOutputId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalDetail_AttributeId",
                table: "TargetConditionGoalDetail",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalDetail_Id",
                table: "TargetConditionGoalDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalDetail_SimulationYearDetailId",
                table: "TargetConditionGoalDetail",
                column: "SimulationYearDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsiderationDetail_AssetDetailId",
                table: "TreatmentConsiderationDetail",
                column: "AssetDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsiderationDetail_Id",
                table: "TreatmentConsiderationDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentOptionDetail_AssetDetailId",
                table: "TreatmentOptionDetail",
                column: "AssetDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentOptionDetail_Id",
                table: "TreatmentOptionDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentRejectionDetail_AssetDetailId",
                table: "TreatmentRejectionDetail",
                column: "AssetDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentRejectionDetail_Id",
                table: "TreatmentRejectionDetail",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSchedulingCollisionDetail_AssetDetailId",
                table: "TreatmentSchedulingCollisionDetail",
                column: "AssetDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSchedulingCollisionDetail_Id",
                table: "TreatmentSchedulingCollisionDetail",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetDetailValue");

            migrationBuilder.DropTable(
                name: "AssetSummaryDetailValue");

            migrationBuilder.DropTable(
                name: "BudgetDetail");

            migrationBuilder.DropTable(
                name: "BudgetUsageDetail");

            migrationBuilder.DropTable(
                name: "CashFlowConsiderationDetail");

            migrationBuilder.DropTable(
                name: "DeficientConditionGoalDetail");

            migrationBuilder.DropTable(
                name: "TargetConditionGoalDetail");

            migrationBuilder.DropTable(
                name: "TreatmentOptionDetail");

            migrationBuilder.DropTable(
                name: "TreatmentRejectionDetail");

            migrationBuilder.DropTable(
                name: "TreatmentSchedulingCollisionDetail");

            migrationBuilder.DropTable(
                name: "AssetSummaryDetail");

            migrationBuilder.DropTable(
                name: "TreatmentConsiderationDetail");

            migrationBuilder.DropTable(
                name: "AssetDetail");

            migrationBuilder.DropTable(
                name: "SimulationYearDetail");

            migrationBuilder.DropColumn(
                name: "InitialConditionOfNetwork",
                table: "SimulationOutput");

            migrationBuilder.AddColumn<string>(
                name: "Output",
                table: "SimulationOutput",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutputType",
                table: "SimulationOutput",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
