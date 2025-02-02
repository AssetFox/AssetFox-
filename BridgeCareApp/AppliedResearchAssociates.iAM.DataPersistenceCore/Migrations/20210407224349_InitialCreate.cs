﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AggregationRuleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Command = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConnectionType = table.Column<int>(type: "int", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Minimum = table.Column<double>(type: "float", nullable: true),
                    Maximum = table.Column<double>(type: "float", nullable: true),
                    IsCalculated = table.Column<bool>(type: "bit", nullable: false),
                    IsAscending = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attribute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetPriorityLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPriorityLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashFlowRuleLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowRuleLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MergedCriteriaExpression = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeficientConditionGoalLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeficientConditionGoalLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Expression = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Network",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Network", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceCurveLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceCurveLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RemainingLifeLimitLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemainingLifeLimitLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetConditionGoalLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetConditionGoalLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentLibrary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasInventoryAccess = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budget_BudgetLibrary_BudgetLibraryId",
                        column: x => x.BudgetLibraryId,
                        principalTable: "BudgetLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetPriority",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetPriorityLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriorityLevel = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPriority", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetPriority_BudgetPriorityLibrary_BudgetPriorityLibraryId",
                        column: x => x.BudgetPriorityLibraryId,
                        principalTable: "BudgetPriorityLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashFlowRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFlowRuleLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashFlowRule_CashFlowRuleLibrary_CashFlowRuleLibraryId",
                        column: x => x.CashFlowRuleLibraryId,
                        principalTable: "CashFlowRuleLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeficientConditionGoal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeficientConditionGoalLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowedDeficientPercentage = table.Column<double>(type: "float", nullable: false),
                    DeficientLimit = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeficientConditionGoal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoal_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoal_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId",
                        column: x => x.DeficientConditionGoalLibraryId,
                        principalTable: "DeficientConditionGoalLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attribute_Equation_CriterionLibrary",
                columns: table => new
                {
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attribute_Equation_CriterionLibrary", x => new { x.AttributeId, x.EquationId });
                    table.ForeignKey(
                        name: "FK_Attribute_Equation_CriterionLibrary_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attribute_Equation_CriterionLibrary_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Attribute_Equation_CriterionLibrary_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BenefitQuantifier",
                columns: table => new
                {
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenefitQuantifier", x => x.NetworkId);
                    table.ForeignKey(
                        name: "FK_BenefitQuantifier_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BenefitQuantifier_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facility_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintainableAsset",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpatialWeighting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintainableAsset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintainableAsset_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NetworkRollupDetail",
                columns: table => new
                {
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkRollupDetail", x => x.NetworkId);
                    table.ForeignKey(
                        name: "FK_NetworkRollupDetail_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Simulation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NetworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfYearsOfTreatmentOutlook = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulation_Network_NetworkId",
                        column: x => x.NetworkId,
                        principalTable: "Network",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceCurve",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformanceCurveLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Shift = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceCurve", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerformanceCurve_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceCurve_PerformanceCurveLibrary_PerformanceCurveLibraryId",
                        column: x => x.PerformanceCurveLibraryId,
                        principalTable: "PerformanceCurveLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RemainingLifeLimit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemainingLifeLimitLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemainingLifeLimit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RemainingLifeLimit_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemainingLifeLimit_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId",
                        column: x => x.RemainingLifeLimitLibraryId,
                        principalTable: "RemainingLifeLimitLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TargetConditionGoal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetConditionGoalLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Target = table.Column<double>(type: "float", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetConditionGoal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetConditionGoal_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TargetConditionGoal_TargetConditionGoalLibrary_TargetConditionGoalLibraryId",
                        column: x => x.TargetConditionGoalLibraryId,
                        principalTable: "TargetConditionGoalLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SelectableTreatment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TreatmentLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_SelectableTreatment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectableTreatment_TreatmentLibrary_TreatmentLibraryId",
                        column: x => x.TreatmentLibraryId,
                        principalTable: "TreatmentLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_User",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_User", x => new { x.CriterionLibraryId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_User_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCriteria_Filter",
                columns: table => new
                {
                    UserCriteriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Criteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasCriteria = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCriteria_Filter", x => x.UserCriteriaId);
                    table.ForeignKey(
                        name: "FK_UserCriteria_Filter_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetAmount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAmount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetAmount_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_Budget",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_Budget", x => new { x.CriterionLibraryId, x.BudgetId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_Budget_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_Budget_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetPercentagePair",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetPriorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPercentagePair", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetPercentagePair_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetPercentagePair_BudgetPriority_BudgetPriorityId",
                        column: x => x.BudgetPriorityId,
                        principalTable: "BudgetPriority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_BudgetPriority",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetPriorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_BudgetPriority", x => new { x.CriterionLibraryId, x.BudgetPriorityId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_BudgetPriority_BudgetPriority_BudgetPriorityId",
                        column: x => x.BudgetPriorityId,
                        principalTable: "BudgetPriority",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_BudgetPriority_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashFlowDistributionRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFlowRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DurationInYears = table.Column<int>(type: "int", nullable: false),
                    CostCeiling = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YearlyPercentages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowDistributionRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashFlowDistributionRule_CashFlowRule_CashFlowRuleId",
                        column: x => x.CashFlowRuleId,
                        principalTable: "CashFlowRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_CashFlowRule",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashFlowRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_CashFlowRule", x => new { x.CriterionLibraryId, x.CashFlowRuleId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_CashFlowRule_CashFlowRule_CashFlowRuleId",
                        column: x => x.CashFlowRuleId,
                        principalTable: "CashFlowRule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_CashFlowRule_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_DeficientConditionGoal",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeficientConditionGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_DeficientConditionGoal", x => new { x.CriterionLibraryId, x.DeficientConditionGoalId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_DeficientConditionGoal_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_DeficientConditionGoal_DeficientConditionGoal_DeficientConditionGoalId",
                        column: x => x.DeficientConditionGoalId,
                        principalTable: "DeficientConditionGoal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpatialWeightingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Section_Equation_SpatialWeightingId",
                        column: x => x.SpatialWeightingId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Section_Facility_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "Facility",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AggregatedResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    MaintainableAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedResult_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AggregatedResult_MaintainableAsset_MaintainableAssetId",
                        column: x => x.MaintainableAssetId,
                        principalTable: "MaintainableAsset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeDatum",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintainableAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeDatum", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeDatum_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttributeDatum_MaintainableAsset_MaintainableAssetId",
                        column: x => x.MaintainableAssetId,
                        principalTable: "MaintainableAsset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaintainableAssetLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintainableAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start = table.Column<double>(type: "float", nullable: true),
                    End = table.Column<double>(type: "float", nullable: true),
                    Direction = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintainableAssetLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintainableAssetLocation_MaintainableAsset_MaintainableAssetId",
                        column: x => x.MaintainableAssetId,
                        principalTable: "MaintainableAsset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalysisMethod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptimizationStrategy = table.Column<int>(type: "int", nullable: false),
                    SpendingStrategy = table.Column<int>(type: "int", nullable: false),
                    ShouldApplyMultipleFeasibleCosts = table.Column<bool>(type: "bit", nullable: false),
                    ShouldDeteriorateDuringCashFlow = table.Column<bool>(type: "bit", nullable: false),
                    ShouldUseExtraFundsAcrossBudgets = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalysisMethod_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AnalysisMethod_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetLibrary_Simulation",
                columns: table => new
                {
                    BudgetLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetLibrary_Simulation", x => new { x.BudgetLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_BudgetLibrary_Simulation_BudgetLibrary_BudgetLibraryId",
                        column: x => x.BudgetLibraryId,
                        principalTable: "BudgetLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetPriorityLibrary_Simulation",
                columns: table => new
                {
                    BudgetPriorityLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetPriorityLibrary_Simulation", x => new { x.BudgetPriorityLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_BudgetPriorityLibrary_Simulation_BudgetPriorityLibrary_BudgetPriorityLibraryId",
                        column: x => x.BudgetPriorityLibraryId,
                        principalTable: "BudgetPriorityLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetPriorityLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashFlowRuleLibrary_Simulation",
                columns: table => new
                {
                    CashFlowRuleLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashFlowRuleLibrary_Simulation", x => new { x.CashFlowRuleLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_CashFlowRuleLibrary_Simulation_CashFlowRuleLibrary_CashFlowRuleLibraryId",
                        column: x => x.CashFlowRuleLibraryId,
                        principalTable: "CashFlowRuleLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CashFlowRuleLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeficientConditionGoalLibrary_Simulation",
                columns: table => new
                {
                    DeficientConditionGoalLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeficientConditionGoalLibrary_Simulation", x => new { x.DeficientConditionGoalLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoalLibrary_Simulation_DeficientConditionGoalLibrary_DeficientConditionGoalLibraryId",
                        column: x => x.DeficientConditionGoalLibraryId,
                        principalTable: "DeficientConditionGoalLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeficientConditionGoalLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstYearOfAnalysisPeriod = table.Column<int>(type: "int", nullable: false),
                    InflationRatePercentage = table.Column<double>(type: "float", nullable: false),
                    MinimumProjectCostLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumberOfYearsInAnalysisPeriod = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentPlan_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceCurveLibrary_Simulation",
                columns: table => new
                {
                    PerformanceCurveLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceCurveLibrary_Simulation", x => new { x.PerformanceCurveLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_PerformanceCurveLibrary_Simulation_PerformanceCurveLibrary_PerformanceCurveLibraryId",
                        column: x => x.PerformanceCurveLibraryId,
                        principalTable: "PerformanceCurveLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceCurveLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RemainingLifeLimitLibrary_Simulation",
                columns: table => new
                {
                    RemainingLifeLimitLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemainingLifeLimitLibrary_Simulation", x => new { x.RemainingLifeLimitLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_RemainingLifeLimitLibrary_Simulation_RemainingLifeLimitLibrary_RemainingLifeLimitLibraryId",
                        column: x => x.RemainingLifeLimitLibraryId,
                        principalTable: "RemainingLifeLimitLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RemainingLifeLimitLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportIndex",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReportTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportIndex", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportIndex_Simulation_SimulationID",
                        column: x => x.SimulationID,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Simulation_User",
                columns: table => new
                {
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanModify = table.Column<bool>(type: "bit", nullable: false),
                    IsOwner = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulation_User", x => new { x.SimulationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Simulation_User_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Simulation_User_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimulationAnalysisDetail",
                columns: table => new
                {
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastRun = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RunTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationAnalysisDetail", x => x.SimulationId);
                    table.ForeignKey(
                        name: "FK_SimulationAnalysisDetail_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimulationOutput",
                columns: table => new
                {
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationOutput", x => x.SimulationId);
                    table.ForeignKey(
                        name: "FK_SimulationOutput_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimulationReportDetail",
                columns: table => new
                {
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimulationReportDetail", x => x.SimulationId);
                    table.ForeignKey(
                        name: "FK_SimulationReportDetail_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TargetConditionGoalLibrary_Simulation",
                columns: table => new
                {
                    TargetConditionGoalLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetConditionGoalLibrary_Simulation", x => new { x.TargetConditionGoalLibraryId, x.SimulationId });
                    table.ForeignKey(
                        name: "FK_TargetConditionGoalLibrary_Simulation_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TargetConditionGoalLibrary_Simulation_TargetConditionGoalLibrary_TargetConditionGoalLibraryId",
                        column: x => x.TargetConditionGoalLibraryId,
                        principalTable: "TargetConditionGoalLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentLibrary_Simulation",
                columns: table => new
                {
                    TreatmentLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_PerformanceCurve",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformanceCurveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_PerformanceCurve", x => new { x.CriterionLibraryId, x.PerformanceCurveId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_PerformanceCurve_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_PerformanceCurve_PerformanceCurve_PerformanceCurveId",
                        column: x => x.PerformanceCurveId,
                        principalTable: "PerformanceCurve",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerformanceCurve_Equation",
                columns: table => new
                {
                    PerformanceCurveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceCurve_Equation", x => new { x.PerformanceCurveId, x.EquationId });
                    table.ForeignKey(
                        name: "FK_PerformanceCurve_Equation_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerformanceCurve_Equation_PerformanceCurve_PerformanceCurveId",
                        column: x => x.PerformanceCurveId,
                        principalTable: "PerformanceCurve",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_RemainingLifeLimit",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RemainingLifeLimitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_RemainingLifeLimit", x => new { x.CriterionLibraryId, x.RemainingLifeLimitId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_RemainingLifeLimit_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_RemainingLifeLimit_RemainingLifeLimit_RemainingLifeLimitId",
                        column: x => x.RemainingLifeLimitId,
                        principalTable: "RemainingLifeLimit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_TargetConditionGoal",
                columns: table => new
                {
                    TargetConditionGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_TargetConditionGoal", x => new { x.CriterionLibraryId, x.TargetConditionGoalId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TargetConditionGoal_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TargetConditionGoal_TargetConditionGoal_TargetConditionGoalId",
                        column: x => x.TargetConditionGoalId,
                        principalTable: "TargetConditionGoal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_Treatment",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_Treatment", x => new { x.CriterionLibraryId, x.SelectableTreatmentId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_Treatment_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_Treatment_SelectableTreatment_SelectableTreatmentId",
                        column: x => x.SelectableTreatmentId,
                        principalTable: "SelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Treatment_Budget",
                columns: table => new
                {
                    SelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatment_Budget", x => new { x.SelectableTreatmentId, x.BudgetId });
                    table.ForeignKey(
                        name: "FK_Treatment_Budget_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Treatment_Budget_SelectableTreatment_SelectableTreatmentId",
                        column: x => x.SelectableTreatmentId,
                        principalTable: "SelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentConsequence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectableTreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentConsequence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentConsequence_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentConsequence_SelectableTreatment_SelectableTreatmentId",
                        column: x => x.SelectableTreatmentId,
                        principalTable: "SelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentCost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentCost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentCost_SelectableTreatment_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "SelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentScheduling",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OffsetToFutureYear = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentScheduling", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentScheduling_SelectableTreatment_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "SelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentSupersession",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentSupersession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentSupersession_SelectableTreatment_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "SelectableTreatment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommittedProject",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SimulationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaintainableAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    SectionEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_CommittedProject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommittedProject_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommittedProject_MaintainableAsset_MaintainableAssetId",
                        column: x => x.MaintainableAssetId,
                        principalTable: "MaintainableAsset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommittedProject_Section_SectionEntityId",
                        column: x => x.SectionEntityId,
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommittedProject_Simulation_SimulationId",
                        column: x => x.SimulationId,
                        principalTable: "Simulation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NumericAttributeValueHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumericAttributeValueHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumericAttributeValueHistory_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NumericAttributeValueHistory_Section_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TextAttributeValueHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextAttributeValueHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextAttributeValueHistory_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TextAttributeValueHistory_Section_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Section",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttributeDatumLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeDatumId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start = table.Column<double>(type: "float", nullable: true),
                    End = table.Column<double>(type: "float", nullable: true),
                    Direction = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeDatumLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeDatumLocation_AttributeDatum_AttributeDatumId",
                        column: x => x.AttributeDatumId,
                        principalTable: "AttributeDatum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Benefit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnalysisMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Limit = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Benefit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Benefit_AnalysisMethod_AnalysisMethodId",
                        column: x => x.AnalysisMethodId,
                        principalTable: "AnalysisMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Benefit_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_AnalysisMethod",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnalysisMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_AnalysisMethod", x => new { x.CriterionLibraryId, x.AnalysisMethodId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_AnalysisMethod_AnalysisMethod_AnalysisMethodId",
                        column: x => x.AnalysisMethodId,
                        principalTable: "AnalysisMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_AnalysisMethod_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_TreatmentConsequence",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConditionalTreatmentConsequenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_TreatmentConsequence", x => new { x.CriterionLibraryId, x.ConditionalTreatmentConsequenceId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentConsequence_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentConsequence_TreatmentConsequence_ConditionalTreatmentConsequenceId",
                        column: x => x.ConditionalTreatmentConsequenceId,
                        principalTable: "TreatmentConsequence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentConsequence_Equation",
                columns: table => new
                {
                    ConditionalTreatmentConsequenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentConsequence_Equation", x => new { x.ConditionalTreatmentConsequenceId, x.EquationId });
                    table.ForeignKey(
                        name: "FK_TreatmentConsequence_Equation_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentConsequence_Equation_TreatmentConsequence_ConditionalTreatmentConsequenceId",
                        column: x => x.ConditionalTreatmentConsequenceId,
                        principalTable: "TreatmentConsequence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_TreatmentCost",
                columns: table => new
                {
                    CriterionLibraryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentCostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionLibrary_TreatmentCost", x => new { x.CriterionLibraryId, x.TreatmentCostId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentCost_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentCost_TreatmentCost_TreatmentCostId",
                        column: x => x.TreatmentCostId,
                        principalTable: "TreatmentCost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentCost_Equation",
                columns: table => new
                {
                    TreatmentCostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentCost_Equation", x => new { x.TreatmentCostId, x.EquationId });
                    table.ForeignKey(
                        name: "FK_TreatmentCost_Equation_Equation_EquationId",
                        column: x => x.EquationId,
                        principalTable: "Equation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentCost_Equation_TreatmentCost_TreatmentCostId",
                        column: x => x.TreatmentCostId,
                        principalTable: "TreatmentCost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionLibrary_TreatmentSupersession",
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
                    table.PrimaryKey("PK_CriterionLibrary_TreatmentSupersession", x => new { x.CriterionLibraryId, x.TreatmentSupersessionId });
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentSupersession_CriterionLibrary_CriterionLibraryId",
                        column: x => x.CriterionLibraryId,
                        principalTable: "CriterionLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionLibrary_TreatmentSupersession_TreatmentSupersession_TreatmentSupersessionId",
                        column: x => x.TreatmentSupersessionId,
                        principalTable: "TreatmentSupersession",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommittedProjectConsequence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommittedProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommittedProjectConsequence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommittedProjectConsequence_Attribute_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommittedProjectConsequence_CommittedProject_CommittedProjectId",
                        column: x => x.CommittedProjectId,
                        principalTable: "CommittedProject",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedResult_AttributeId",
                table: "AggregatedResult",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedResult_MaintainableAssetId",
                table: "AggregatedResult",
                column: "MaintainableAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisMethod_AttributeId",
                table: "AnalysisMethod",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisMethod_SimulationId",
                table: "AnalysisMethod",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attribute_Name",
                table: "Attribute",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attribute_Equation_CriterionLibrary_AttributeId",
                table: "Attribute_Equation_CriterionLibrary",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Attribute_Equation_CriterionLibrary_CriterionLibraryId",
                table: "Attribute_Equation_CriterionLibrary",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Attribute_Equation_CriterionLibrary_EquationId",
                table: "Attribute_Equation_CriterionLibrary",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDatum_AttributeId",
                table: "AttributeDatum",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDatum_MaintainableAssetId",
                table: "AttributeDatum",
                column: "MaintainableAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDatumLocation_AttributeDatumId",
                table: "AttributeDatumLocation",
                column: "AttributeDatumId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Benefit_AnalysisMethodId",
                table: "Benefit",
                column: "AnalysisMethodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Benefit_AttributeId",
                table: "Benefit",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_BenefitQuantifier_EquationId",
                table: "BenefitQuantifier",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BenefitQuantifier_NetworkId",
                table: "BenefitQuantifier",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_BudgetLibraryId",
                table: "Budget",
                column: "BudgetLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmount_BudgetId",
                table: "BudgetAmount",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLibrary_Simulation_BudgetLibraryId",
                table: "BudgetLibrary_Simulation",
                column: "BudgetLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLibrary_Simulation_SimulationId",
                table: "BudgetLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPercentagePair_BudgetId",
                table: "BudgetPercentagePair",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPercentagePair_BudgetPriorityId",
                table: "BudgetPercentagePair",
                column: "BudgetPriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPriority_BudgetPriorityLibraryId",
                table: "BudgetPriority",
                column: "BudgetPriorityLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPriorityLibrary_Simulation_BudgetPriorityLibraryId",
                table: "BudgetPriorityLibrary_Simulation",
                column: "BudgetPriorityLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetPriorityLibrary_Simulation_SimulationId",
                table: "BudgetPriorityLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowDistributionRule_CashFlowRuleId",
                table: "CashFlowDistributionRule",
                column: "CashFlowRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowRule_CashFlowRuleLibraryId",
                table: "CashFlowRule",
                column: "CashFlowRuleLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowRuleLibrary_Simulation_CashFlowRuleLibraryId",
                table: "CashFlowRuleLibrary_Simulation",
                column: "CashFlowRuleLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CashFlowRuleLibrary_Simulation_SimulationId",
                table: "CashFlowRuleLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProject_BudgetId",
                table: "CommittedProject",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProject_MaintainableAssetId",
                table: "CommittedProject",
                column: "MaintainableAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProject_SectionEntityId",
                table: "CommittedProject",
                column: "SectionEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProject_SimulationId",
                table: "CommittedProject",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProjectConsequence_AttributeId",
                table: "CommittedProjectConsequence",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommittedProjectConsequence_CommittedProjectId",
                table: "CommittedProjectConsequence",
                column: "CommittedProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_AnalysisMethod_AnalysisMethodId",
                table: "CriterionLibrary_AnalysisMethod",
                column: "AnalysisMethodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_AnalysisMethod_CriterionLibraryId",
                table: "CriterionLibrary_AnalysisMethod",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_Budget_BudgetId",
                table: "CriterionLibrary_Budget",
                column: "BudgetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_Budget_CriterionLibraryId",
                table: "CriterionLibrary_Budget",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_BudgetPriority_BudgetPriorityId",
                table: "CriterionLibrary_BudgetPriority",
                column: "BudgetPriorityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_BudgetPriority_CriterionLibraryId",
                table: "CriterionLibrary_BudgetPriority",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_CashFlowRule_CashFlowRuleId",
                table: "CriterionLibrary_CashFlowRule",
                column: "CashFlowRuleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_CashFlowRule_CriterionLibraryId",
                table: "CriterionLibrary_CashFlowRule",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_DeficientConditionGoal_CriterionLibraryId",
                table: "CriterionLibrary_DeficientConditionGoal",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_DeficientConditionGoal_DeficientConditionGoalId",
                table: "CriterionLibrary_DeficientConditionGoal",
                column: "DeficientConditionGoalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_PerformanceCurve_CriterionLibraryId",
                table: "CriterionLibrary_PerformanceCurve",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_PerformanceCurve_PerformanceCurveId",
                table: "CriterionLibrary_PerformanceCurve",
                column: "PerformanceCurveId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_RemainingLifeLimit_CriterionLibraryId",
                table: "CriterionLibrary_RemainingLifeLimit",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_RemainingLifeLimit_RemainingLifeLimitId",
                table: "CriterionLibrary_RemainingLifeLimit",
                column: "RemainingLifeLimitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TargetConditionGoal_CriterionLibraryId",
                table: "CriterionLibrary_TargetConditionGoal",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TargetConditionGoal_TargetConditionGoalId",
                table: "CriterionLibrary_TargetConditionGoal",
                column: "TargetConditionGoalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_Treatment_CriterionLibraryId",
                table: "CriterionLibrary_Treatment",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_Treatment_SelectableTreatmentId",
                table: "CriterionLibrary_Treatment",
                column: "SelectableTreatmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentConsequence_ConditionalTreatmentConsequenceId",
                table: "CriterionLibrary_TreatmentConsequence",
                column: "ConditionalTreatmentConsequenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentConsequence_CriterionLibraryId",
                table: "CriterionLibrary_TreatmentConsequence",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentCost_CriterionLibraryId",
                table: "CriterionLibrary_TreatmentCost",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentCost_TreatmentCostId",
                table: "CriterionLibrary_TreatmentCost",
                column: "TreatmentCostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentSupersession_CriterionLibraryId",
                table: "CriterionLibrary_TreatmentSupersession",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_TreatmentSupersession_TreatmentSupersessionId",
                table: "CriterionLibrary_TreatmentSupersession",
                column: "TreatmentSupersessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_User_CriterionLibraryId",
                table: "CriterionLibrary_User",
                column: "CriterionLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionLibrary_User_UserId",
                table: "CriterionLibrary_User",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoal_AttributeId",
                table: "DeficientConditionGoal",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoal_DeficientConditionGoalLibraryId",
                table: "DeficientConditionGoal",
                column: "DeficientConditionGoalLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalLibrary_Simulation_DeficientConditionGoalLibraryId",
                table: "DeficientConditionGoalLibrary_Simulation",
                column: "DeficientConditionGoalLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_DeficientConditionGoalLibrary_Simulation_SimulationId",
                table: "DeficientConditionGoalLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Facility_NetworkId",
                table: "Facility",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentPlan_SimulationId",
                table: "InvestmentPlan",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintainableAsset_NetworkId",
                table: "MaintainableAsset",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintainableAssetLocation_MaintainableAssetId",
                table: "MaintainableAssetLocation",
                column: "MaintainableAssetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetworkRollupDetail_NetworkId",
                table: "NetworkRollupDetail",
                column: "NetworkId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NumericAttributeValueHistory_AttributeId",
                table: "NumericAttributeValueHistory",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_NumericAttributeValueHistory_SectionId",
                table: "NumericAttributeValueHistory",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurve_AttributeId",
                table: "PerformanceCurve",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurve_PerformanceCurveLibraryId",
                table: "PerformanceCurve",
                column: "PerformanceCurveLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurve_Equation_EquationId",
                table: "PerformanceCurve_Equation",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurve_Equation_PerformanceCurveId",
                table: "PerformanceCurve_Equation",
                column: "PerformanceCurveId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurveLibrary_Simulation_PerformanceCurveLibraryId",
                table: "PerformanceCurveLibrary_Simulation",
                column: "PerformanceCurveLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_PerformanceCurveLibrary_Simulation_SimulationId",
                table: "PerformanceCurveLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RemainingLifeLimit_AttributeId",
                table: "RemainingLifeLimit",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_RemainingLifeLimit_RemainingLifeLimitLibraryId",
                table: "RemainingLifeLimit",
                column: "RemainingLifeLimitLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_RemainingLifeLimitLibrary_Simulation_RemainingLifeLimitLibraryId",
                table: "RemainingLifeLimitLibrary_Simulation",
                column: "RemainingLifeLimitLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_RemainingLifeLimitLibrary_Simulation_SimulationId",
                table: "RemainingLifeLimitLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportIndex_SimulationID",
                table: "ReportIndex",
                column: "SimulationID");

            migrationBuilder.CreateIndex(
                name: "IX_Section_FacilityId",
                table: "Section",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Section_SpatialWeightingId",
                table: "Section",
                column: "SpatialWeightingId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectableTreatment_TreatmentLibraryId",
                table: "SelectableTreatment",
                column: "TreatmentLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulation_NetworkId",
                table: "Simulation",
                column: "NetworkId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulation_User_SimulationId",
                table: "Simulation_User",
                column: "SimulationId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulation_User_UserId",
                table: "Simulation_User",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SimulationAnalysisDetail_SimulationId",
                table: "SimulationAnalysisDetail",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SimulationOutput_SimulationId",
                table: "SimulationOutput",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SimulationReportDetail_SimulationId",
                table: "SimulationReportDetail",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoal_AttributeId",
                table: "TargetConditionGoal",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoal_TargetConditionGoalLibraryId",
                table: "TargetConditionGoal",
                column: "TargetConditionGoalLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_Simulation_SimulationId",
                table: "TargetConditionGoalLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TargetConditionGoalLibrary_Simulation_TargetConditionGoalLibraryId",
                table: "TargetConditionGoalLibrary_Simulation",
                column: "TargetConditionGoalLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TextAttributeValueHistory_AttributeId",
                table: "TextAttributeValueHistory",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_TextAttributeValueHistory_SectionId",
                table: "TextAttributeValueHistory",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatment_Budget_BudgetId",
                table: "Treatment_Budget",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatment_Budget_SelectableTreatmentId",
                table: "Treatment_Budget",
                column: "SelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsequence_AttributeId",
                table: "TreatmentConsequence",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsequence_SelectableTreatmentId",
                table: "TreatmentConsequence",
                column: "SelectableTreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsequence_Equation_ConditionalTreatmentConsequenceId",
                table: "TreatmentConsequence_Equation",
                column: "ConditionalTreatmentConsequenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentConsequence_Equation_EquationId",
                table: "TreatmentConsequence_Equation",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCost_TreatmentId",
                table: "TreatmentCost",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCost_Equation_EquationId",
                table: "TreatmentCost_Equation",
                column: "EquationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentCost_Equation_TreatmentCostId",
                table: "TreatmentCost_Equation",
                column: "TreatmentCostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_Simulation_SimulationId",
                table: "TreatmentLibrary_Simulation",
                column: "SimulationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentLibrary_Simulation_TreatmentLibraryId",
                table: "TreatmentLibrary_Simulation",
                column: "TreatmentLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentScheduling_TreatmentId",
                table: "TreatmentScheduling",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSupersession_TreatmentId",
                table: "TreatmentSupersession",
                column: "TreatmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCriteria_Filter_UserCriteriaId",
                table: "UserCriteria_Filter",
                column: "UserCriteriaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCriteria_Filter_UserId",
                table: "UserCriteria_Filter",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AggregatedResult");

            migrationBuilder.DropTable(
                name: "Attribute_Equation_CriterionLibrary");

            migrationBuilder.DropTable(
                name: "AttributeDatumLocation");

            migrationBuilder.DropTable(
                name: "Benefit");

            migrationBuilder.DropTable(
                name: "BenefitQuantifier");

            migrationBuilder.DropTable(
                name: "BudgetAmount");

            migrationBuilder.DropTable(
                name: "BudgetLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "BudgetPercentagePair");

            migrationBuilder.DropTable(
                name: "BudgetPriorityLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "CashFlowDistributionRule");

            migrationBuilder.DropTable(
                name: "CashFlowRuleLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "CommittedProjectConsequence");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_AnalysisMethod");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_Budget");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_BudgetPriority");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_CashFlowRule");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_DeficientConditionGoal");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_PerformanceCurve");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_RemainingLifeLimit");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_TargetConditionGoal");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_Treatment");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_TreatmentConsequence");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_TreatmentCost");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_TreatmentSupersession");

            migrationBuilder.DropTable(
                name: "CriterionLibrary_User");

            migrationBuilder.DropTable(
                name: "DeficientConditionGoalLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "InvestmentPlan");

            migrationBuilder.DropTable(
                name: "MaintainableAssetLocation");

            migrationBuilder.DropTable(
                name: "NetworkRollupDetail");

            migrationBuilder.DropTable(
                name: "NumericAttributeValueHistory");

            migrationBuilder.DropTable(
                name: "PerformanceCurve_Equation");

            migrationBuilder.DropTable(
                name: "PerformanceCurveLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "RemainingLifeLimitLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "ReportIndex");

            migrationBuilder.DropTable(
                name: "Simulation_User");

            migrationBuilder.DropTable(
                name: "SimulationAnalysisDetail");

            migrationBuilder.DropTable(
                name: "SimulationOutput");

            migrationBuilder.DropTable(
                name: "SimulationReportDetail");

            migrationBuilder.DropTable(
                name: "TargetConditionGoalLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "TextAttributeValueHistory");

            migrationBuilder.DropTable(
                name: "Treatment_Budget");

            migrationBuilder.DropTable(
                name: "TreatmentConsequence_Equation");

            migrationBuilder.DropTable(
                name: "TreatmentCost_Equation");

            migrationBuilder.DropTable(
                name: "TreatmentLibrary_Simulation");

            migrationBuilder.DropTable(
                name: "TreatmentScheduling");

            migrationBuilder.DropTable(
                name: "UserCriteria_Filter");

            migrationBuilder.DropTable(
                name: "AttributeDatum");

            migrationBuilder.DropTable(
                name: "CommittedProject");

            migrationBuilder.DropTable(
                name: "AnalysisMethod");

            migrationBuilder.DropTable(
                name: "BudgetPriority");

            migrationBuilder.DropTable(
                name: "CashFlowRule");

            migrationBuilder.DropTable(
                name: "DeficientConditionGoal");

            migrationBuilder.DropTable(
                name: "RemainingLifeLimit");

            migrationBuilder.DropTable(
                name: "TargetConditionGoal");

            migrationBuilder.DropTable(
                name: "TreatmentSupersession");

            migrationBuilder.DropTable(
                name: "CriterionLibrary");

            migrationBuilder.DropTable(
                name: "PerformanceCurve");

            migrationBuilder.DropTable(
                name: "TreatmentConsequence");

            migrationBuilder.DropTable(
                name: "TreatmentCost");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "MaintainableAsset");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.DropTable(
                name: "Simulation");

            migrationBuilder.DropTable(
                name: "BudgetPriorityLibrary");

            migrationBuilder.DropTable(
                name: "CashFlowRuleLibrary");

            migrationBuilder.DropTable(
                name: "DeficientConditionGoalLibrary");

            migrationBuilder.DropTable(
                name: "RemainingLifeLimitLibrary");

            migrationBuilder.DropTable(
                name: "TargetConditionGoalLibrary");

            migrationBuilder.DropTable(
                name: "PerformanceCurveLibrary");

            migrationBuilder.DropTable(
                name: "Attribute");

            migrationBuilder.DropTable(
                name: "SelectableTreatment");

            migrationBuilder.DropTable(
                name: "BudgetLibrary");

            migrationBuilder.DropTable(
                name: "Equation");

            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropTable(
                name: "TreatmentLibrary");

            migrationBuilder.DropTable(
                name: "Network");
        }
    }
}
