using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SchemaAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ET");

            migrationBuilder.RenameTable(
                name: "UserPreference",
                newName: "UserPreference",
                newSchema: "ET");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "User",
                newSchema: "ET");

            migrationBuilder.RenameTable(
                name: "ExpenseCategory",
                newName: "ExpenseCategory",
                newSchema: "ET");

            migrationBuilder.RenameTable(
                name: "Expense",
                newName: "Expense",
                newSchema: "ET");

            migrationBuilder.RenameTable(
                name: "Currency",
                newName: "Currency",
                newSchema: "ET");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserPreference",
                schema: "ET",
                newName: "UserPreference");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "ET",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "ExpenseCategory",
                schema: "ET",
                newName: "ExpenseCategory");

            migrationBuilder.RenameTable(
                name: "Expense",
                schema: "ET",
                newName: "Expense");

            migrationBuilder.RenameTable(
                name: "Currency",
                schema: "ET",
                newName: "Currency");
        }
    }
}
