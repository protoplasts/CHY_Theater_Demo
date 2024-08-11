using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHY_Theater_DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardPoint_AspNetUsers_UserId",
                table: "RewardPoint");

            migrationBuilder.DropForeignKey(
                name: "FK_RewardPoint_PaymentTransactions_TransactionId",
                table: "RewardPoint");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RewardPoint",
                table: "RewardPoint");

            migrationBuilder.RenameTable(
                name: "RewardPoint",
                newName: "RewardPoints");

            migrationBuilder.RenameIndex(
                name: "IX_RewardPoint_UserId",
                table: "RewardPoints",
                newName: "IX_RewardPoints_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RewardPoint_TransactionId",
                table: "RewardPoints",
                newName: "IX_RewardPoints_TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RewardPoints",
                table: "RewardPoints",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardPoints_AspNetUsers_UserId",
                table: "RewardPoints",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RewardPoints_PaymentTransactions_TransactionId",
                table: "RewardPoints",
                column: "TransactionId",
                principalTable: "PaymentTransactions",
                principalColumn: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardPoints_AspNetUsers_UserId",
                table: "RewardPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_RewardPoints_PaymentTransactions_TransactionId",
                table: "RewardPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RewardPoints",
                table: "RewardPoints");

            migrationBuilder.RenameTable(
                name: "RewardPoints",
                newName: "RewardPoint");

            migrationBuilder.RenameIndex(
                name: "IX_RewardPoints_UserId",
                table: "RewardPoint",
                newName: "IX_RewardPoint_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RewardPoints_TransactionId",
                table: "RewardPoint",
                newName: "IX_RewardPoint_TransactionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RewardPoint",
                table: "RewardPoint",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardPoint_AspNetUsers_UserId",
                table: "RewardPoint",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RewardPoint_PaymentTransactions_TransactionId",
                table: "RewardPoint",
                column: "TransactionId",
                principalTable: "PaymentTransactions",
                principalColumn: "TransactionId");
        }
    }
}
