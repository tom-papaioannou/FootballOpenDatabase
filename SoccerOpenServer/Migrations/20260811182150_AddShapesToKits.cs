using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddShapesToKits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KitShape",
                table: "Kits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KitShape",
                table: "Kits");
        }
    }
}
