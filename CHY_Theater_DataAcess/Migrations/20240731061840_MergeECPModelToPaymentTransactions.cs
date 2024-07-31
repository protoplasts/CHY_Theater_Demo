using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHY_Theater_DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class MergeECPModelToPaymentTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "PaymentTransactions",
                newName: "PaymentType");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "PaymentTransactions",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<string>(
                name: "MemberID",
                table: "PaymentTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MerchantTradeNo",
                table: "PaymentTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentTypeChargeFee",
                table: "PaymentTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RtnCode",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RtnMsg",
                table: "PaymentTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SimulatePaid",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TradeAmt",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradeDate",
                table: "PaymentTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TradeNo",
                table: "PaymentTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberID",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "MerchantTradeNo",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentTypeChargeFee",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "RtnCode",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "RtnMsg",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "SimulatePaid",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "TradeAmt",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "TradeDate",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "TradeNo",
                table: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "PaymentTransactions",
                newName: "PaymentMethod");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "PaymentTransactions",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}
