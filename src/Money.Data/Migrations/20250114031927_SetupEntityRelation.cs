using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Money.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetupEntityRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomeTransactions_IncomeTypes_IncomeTypeId",
                table: "IncomeTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "IncomeTypeId",
                table: "IncomeTransactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeTransactions_IncomeTypes_IncomeTypeId",
                table: "IncomeTransactions",
                column: "IncomeTypeId",
                principalTable: "IncomeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomeTransactions_IncomeTypes_IncomeTypeId",
                table: "IncomeTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "IncomeTypeId",
                table: "IncomeTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeTransactions_IncomeTypes_IncomeTypeId",
                table: "IncomeTransactions",
                column: "IncomeTypeId",
                principalTable: "IncomeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
