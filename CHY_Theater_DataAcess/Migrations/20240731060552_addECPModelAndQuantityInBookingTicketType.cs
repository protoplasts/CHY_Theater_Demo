using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHY_Theater_DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class addECPModelAndQuantityInBookingTicketType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BookingTicketTypes_Detail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EcpayOrders",
                columns: table => new
                {
                    EcpayOrdersID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    MerchantTradeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MemberID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RtnCode = table.Column<int>(type: "int", nullable: true),
                    RtnMsg = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TradeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TradeAmt = table.Column<int>(type: "int", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentTypeChargeFee = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TradeDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SimulatePaid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcpayOrders", x => x.EcpayOrdersID);
                    table.ForeignKey(
                        name: "FK_EcpayOrders_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EcpayOrders_BookingId",
                table: "EcpayOrders",
                column: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EcpayOrders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BookingTicketTypes_Detail");
        }
    }
}
