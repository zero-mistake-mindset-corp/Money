using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Money.Data.Migrations
{
    /// <inheritdoc />
    public partial class TransactionUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseTransactions_MoneyAccounts_MoneyAccountId",
                table: "ExpenseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomeTransactions_MoneyAccounts_MoneyAccountId",
                table: "IncomeTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "MoneyAccountId",
                table: "IncomeTransactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "IncomeTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "IncomeTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "MoneyAccountId",
                table: "ExpenseTransactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "ExpenseTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ExpenseTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_UserId",
                table: "IncomeTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_UserId",
                table: "ExpenseTransactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseTransactions_MoneyAccounts_MoneyAccountId",
                table: "ExpenseTransactions",
                column: "MoneyAccountId",
                principalTable: "MoneyAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseTransactions_Users_UserId",
                table: "ExpenseTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeTransactions_MoneyAccounts_MoneyAccountId",
                table: "IncomeTransactions",
                column: "MoneyAccountId",
                principalTable: "MoneyAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeTransactions_Users_UserId",
                table: "IncomeTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseTransactions_MoneyAccounts_MoneyAccountId",
                table: "ExpenseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseTransactions_Users_UserId",
                table: "ExpenseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomeTransactions_MoneyAccounts_MoneyAccountId",
                table: "IncomeTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomeTransactions_Users_UserId",
                table: "IncomeTransactions");

            migrationBuilder.DropIndex(
                name: "IX_IncomeTransactions_UserId",
                table: "IncomeTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseTransactions_UserId",
                table: "ExpenseTransactions");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "IncomeTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IncomeTransactions");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "ExpenseTransactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ExpenseTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "MoneyAccountId",
                table: "IncomeTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MoneyAccountId",
                table: "ExpenseTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseTransactions_MoneyAccounts_MoneyAccountId",
                table: "ExpenseTransactions",
                column: "MoneyAccountId",
                principalTable: "MoneyAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeTransactions_MoneyAccounts_MoneyAccountId",
                table: "IncomeTransactions",
                column: "MoneyAccountId",
                principalTable: "MoneyAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
