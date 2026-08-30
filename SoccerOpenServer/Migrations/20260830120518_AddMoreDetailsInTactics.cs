using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreDetailsInTactics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AttackLeft",
                table: "Tactics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AttackMiddle",
                table: "Tactics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AttackRight",
                table: "Tactics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EarlyCrosses",
                table: "Tactics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OffsideTrap",
                table: "Tactics",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttackLeft",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "AttackMiddle",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "AttackRight",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "EarlyCrosses",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "OffsideTrap",
                table: "Tactics");
        }
    }
}
