using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Migrations
{
    public partial class RemoveColumnBudgetIdFromTableBudgetPercentagePair : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [BudgetPercentagePair] NOCHECK CONSTRAINT all;");
            migrationBuilder.Sql("ALTER TABLE [BudgetPercentagePair] DROP CONSTRAINT IF EXISTS [FK_BudgetPercentagePair_Budget_BudgetId];");
            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_BudgetPercentagePair_BudgetId] ON [BudgetPercentagePair]");
            migrationBuilder.Sql("ALTER TABLE [BudgetPercentagePair] DROP COLUMN IF EXISTS [BudgetId];");
            migrationBuilder.Sql("ALTER TABLE [BudgetPercentagePair] WITH CHECK CHECK CONSTRAINT all;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
