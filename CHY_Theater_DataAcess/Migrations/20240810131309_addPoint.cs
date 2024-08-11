using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHY_Theater_DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class addPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "PaymentTransactions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "PaymentTransactions");
        }
    }
}
