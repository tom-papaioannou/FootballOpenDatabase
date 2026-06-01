using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class TacticsAddFreeKickTakersAndMentalities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LeftFreeKickTakerID",
                table: "Tactics",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassingMentality",
                table: "Tactics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RightFreeKickTakerID",
                table: "Tactics",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TacticMentality",
                table: "Tactics",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftFreeKickTakerID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "PassingMentality",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "RightFreeKickTakerID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "TacticMentality",
                table: "Tactics");
        }
    }
}
