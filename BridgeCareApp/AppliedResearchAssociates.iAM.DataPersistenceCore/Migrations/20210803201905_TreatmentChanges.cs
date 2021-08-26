using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class TreatmentChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TreatmentLibrary_Simulation");

            migrationBuilder.AddColumn<Guid>(
                name: "CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId",
                table: "TreatmentSupersession",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId",
                table: "TreatmentSupersession",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ScenarioSelectableTreatment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScenarioTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShadowForAnyTreatment = table.Column<int>(type: "int", nullable: false),
                    ShadowForSameTreatment = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioSelectableTreatment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioSelectableTreatment_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioTreatment",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioSelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioTreatment", x => new { x.CriterionLibraryId, x.ScenarioSelectableTreatmentId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatment_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                        column: x => x.ScenarioSelectableTreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioConditionalTreatmentConsequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioSelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioConditionalTreatmentConsequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioConditionalTreatmentConsequences_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                        column: x => x.ScenarioSelectableTreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioTreatment_Budget",
                columns: table => new
                {
                    ScenarioSelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatment_Budget", x => new { x.ScenarioSelectableTreatmentId, x.BudgetId });
                    table.ForeignKey(
                        name: "FK_ScenarioTreatment_Budget_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatment_Budget_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                        column: x => x.ScenarioSelectableTreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioTreatmentCost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatmentCost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentCost_ScenarioSelectableTreatment_ScenarioTreatmentId",
                        column: x => x.ScenarioTreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioTreatmentSchedulingEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OffsetToFutureYear = table.Column<int>(type: "int", nullable: false),
                    ScenarioSelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatmentSchedulingEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentSchedulingEntity_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                        column: x => x.ScenarioSelectableTreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioTreatmentSupersessionEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioSelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatmentSupersessionEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentSupersessionEntity_ScenarioSelectableTreatment_ScenarioSelectableTreatmentId",
                        column: x => x.ScenarioSelectableTreatmentId,
                        principalTable: "ScenarioSelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioTreatmentConsequence",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioConditionalTreatmentConsequenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioTreatmentConsequence", x => new { x.CriterionLibraryId, x.ScenarioConditionalTreatmentConsequenceId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequences_ScenarioConditionalTreatmentConsequen~",
                        column: x => x.ScenarioConditionalTreatmentConsequenceId,
                        principalTable: "ScenarioConditionalTreatmentConsequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioTreatmentConsequence_Equation",
                columns: table => new
                {
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioConditionalTreatmentConsequenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatmentConsequence_Equation", x => new { x.ScenarioConditionalTreatmentConsequenceId, x.EquationId });
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentConsequence_Equation_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequences_ScenarioConditionalTreatmentConsequenceId",
                        column: x => x.ScenarioConditionalTreatmentConsequenceId,
                        principalTable: "ScenarioConditionalTreatmentConsequences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioTreatmentCost",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioTreatmentCostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioTreatmentCost", x => new { x.CriterionLibraryId, x.ScenarioTreatmentCostId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentCost_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentCost_ScenarioTreatmentCost_ScenarioTreatmentCostId",
                        column: x => x.ScenarioTreatmentCostId,
                        principalTable: "ScenarioTreatmentCost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioTreatmentCost_Equation",
                columns: table => new
                {
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioTreatmentCostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioTreatmentCost_Equation", x => new { x.ScenarioTreatmentCostId, x.EquationId });
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentCost_Equation_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenarioTreatmentCost_Equation_ScenarioTreatmentCost_ScenarioTreatmentCostId",
                        column: x => x.ScenarioTreatmentCostId,
                        principalTable: "ScenarioTreatmentCost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_ScenarioTreatmentSupersession",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentSupersessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_ScenarioTreatmentSupersession", x => new { x.CriterionLibraryId, x.TreatmentSupersessionId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_ScenarioTreatmentSupersession_ScenarioTreatmentSupersessionEntity_TreatmentSupersessionId",
                        column: x => x.TreatmentSupersessionId,
                        principalTable: "ScenarioTreatmentSupersessionEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSupersession_CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId_CriterionLibraryScenarioTreatmentS~",
                table: "TreatmentSupersession",
                columns: new[] { "CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId", "CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatment_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioTreatment",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatment_ScenarioSelectableTreatmentId",
                table: "CriterionLibrary_ScenarioTreatment",
                column: "ScenarioSelectableTreatmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentConsequence_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioTreatmentConsequence",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentConsequence_ScenarioConditionalTreatmentConsequenceId",
                table: "CriterionLibrary_ScenarioTreatmentConsequence",
                column: "ScenarioConditionalTreatmentConsequenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentCost_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioTreatmentCost",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentCost_ScenarioTreatmentCostId",
                table: "CriterionLibrary_ScenarioTreatmentCost",
                column: "ScenarioTreatmentCostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibraryId",
                table: "CriterionLibrary_ScenarioTreatmentSupersession",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_ScenarioTreatmentSupersession_TreatmentSupersessionId",
                table: "CriterionLibrary_ScenarioTreatmentSupersession",
                column: "TreatmentSupersessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioConditionalTreatmentConsequences_AttributeId",
                table: "ScenarioConditionalTreatmentConsequences",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioConditionalTreatmentConsequences_ScenarioSelectableTreatmentId",
                table: "ScenarioConditionalTreatmentConsequences",
                column: "ScenarioSelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioSelectableTreatment_SimulationId",
                table: "ScenarioSelectableTreatment",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatment_Budget_BudgetId",
                table: "ScenarioTreatment_Budget",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatment_Budget_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatment_Budget",
                column: "ScenarioSelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentConsequence_Equation_EquationId",
                table: "ScenarioTreatmentConsequence_Equation",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentConsequence_Equation_ScenarioConditionalTreatmentConsequenceId",
                table: "ScenarioTreatmentConsequence_Equation",
                column: "ScenarioConditionalTreatmentConsequenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentCost_ScenarioTreatmentId",
                table: "ScenarioTreatmentCost",
                column: "ScenarioTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentCost_Equation_EquationId",
                table: "ScenarioTreatmentCost_Equation",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentCost_Equation_ScenarioTreatmentCostId",
                table: "ScenarioTreatmentCost_Equation",
                column: "ScenarioTreatmentCostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentSchedulingEntity_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSchedulingEntity",
                column: "ScenarioSelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioTreatmentSupersessionEntity_ScenarioSelectableTreatmentId",
                table: "ScenarioTreatmentSupersessionEntity",
                column: "ScenarioSelectableTreatmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentSupersession_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibraryScenarioTreatmentSupersessionJoinCriter~",
                table: "TreatmentSupersession",
                columns: new[] { "CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId", "CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId" },
                principalTable: "CriterionLibrary_ScenarioTreatmentSupersession",
                principalColumns: new[] { "CriterionLibraryId", "TreatmentSupersessionId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentSupersession_CriterionLibrary_ScenarioTreatmentSupersession_CriterionLibraryScenarioTreatmentSupersessionJoinCriter~",
                table: "TreatmentSupersession");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioTreatment");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioTreatmentConsequence");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioTreatmentCost");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_ScenarioTreatmentSupersession");

            migrationBuilder.DropTable(
                name: "ScenarioTreatment_Budget");

            migrationBuilder.DropTable(
                name: "ScenarioTreatmentConsequence_Equation");

            migrationBuilder.DropTable(
                name: "ScenarioTreatmentCost_Equation");

            migrationBuilder.DropTable(
                name: "ScenarioTreatmentSchedulingEntity");

            migrationBuilder.DropTable(
                name: "ScenarioTreatmentSupersessionEntity");

            migrationBuilder.DropTable(
                name: "ScenarioConditionalTreatmentConsequences");

            migrationBuilder.DropTable(
                name: "ScenarioTreatmentCost");

            migrationBuilder.DropTable(
                name: "ScenarioSelectableTreatment");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentSupersession_CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId_CriterionLibraryScenarioTreatmentS~",
                table: "TreatmentSupersession");

            migrationBuilder.DropColumn(
                name: "CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId",
                table: "TreatmentSupersession");

            migrationBuilder.DropColumn(
                name: "CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId",
                table: "TreatmentSupersession");

            migrationBuilder.CreateTable(
                name: "TreatmentLibrary_Simulation",
                columns: table => new
                {
                    TreatmentLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentLibrary_Simulation", x => new { x.TreatmentLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_TreatmentLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentLibrary_Simulation_TreatmentLibrary_TreatmentLibraryId",
                        column: x => x.TreatmentLibraryId,
                        principalTable: "TreatmentLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_Simulation_SimulationId",
                table: "TreatmentLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_Simulation_TreatmentLibraryId",
                table: "TreatmentLibrary_Simulation",
                column: "TreatmentLibraryId");
        }
    }
}
