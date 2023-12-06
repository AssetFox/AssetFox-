using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class RemoveColumnBudgetIdFromTableCommittedProject
        : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [CommittedProject] DROP CONSTRAINT IF EXISTS [FK_BudgetPercentagePair_Budget_BudgetId];");
            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_BudgetPercentagePair_BudgetId] ON [BudgetPercentagePair]");
            migrationBuilder.Sql("ALTER TABLE [CommittedProject] DROP COLUMN IF EXISTS [BudgetId];");
            migrationBuilder.Sql("ALTER TABLE [CommittedProject] WITH CHECK CHECK CONSTRAINT all;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
