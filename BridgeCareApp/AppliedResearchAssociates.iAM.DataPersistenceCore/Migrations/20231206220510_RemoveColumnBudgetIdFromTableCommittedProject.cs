using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class RemoveColumnBudgetIdFromTableCommittedProject
        : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [CommittedProject] NOCHECK CONSTRAINT all;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_CommittedProject_BudgetId] ON [CommittedProject]");
            migrationBuilder.Sql("ALTER TABLE [CommittedProject] DROP CONSTRAINT IF EXISTS [FK_CommittedProject_Budget_BudgetId];");
            migrationBuilder.Sql("ALTER TABLE [CommittedProject] DROP COLUMN IF EXISTS [BudgetId];");
            migrationBuilder.Sql("ALTER TABLE [CommittedProject] WITH CHECK CHECK CONSTRAINT all;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
