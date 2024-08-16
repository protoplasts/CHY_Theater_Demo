using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHY_Theater_DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class ReviseMovieTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IMDb",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MovieImageHorizontal",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IMDb",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "MovieImageHorizontal",
                table: "Movies");
        }
    }
}
