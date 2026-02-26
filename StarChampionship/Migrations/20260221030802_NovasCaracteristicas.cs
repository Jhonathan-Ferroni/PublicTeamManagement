using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarChampionship.Migrations
{
    /// <inheritdoc />
    public partial class NovasCaracteristicas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Player");

            migrationBuilder.AddColumn<double>(
                name: "BallControl",
                table: "Player",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Dribble",
                table: "Player",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FirstTouch",
                table: "Player",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BallControl",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Dribble",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "FirstTouch",
                table: "Player");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Player",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
