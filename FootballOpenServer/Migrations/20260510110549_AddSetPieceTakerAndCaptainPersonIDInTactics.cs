using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddSetPieceTakerAndCaptainPersonIDInTactics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CaptainID",
                table: "Tactics",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LeftCornerTakerID",
                table: "Tactics",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PenaltyTakerID",
                table: "Tactics",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RightCornerTakerID",
                table: "Tactics",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaptainID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "LeftCornerTakerID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "PenaltyTakerID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "RightCornerTakerID",
                table: "Tactics");
        }
    }
}
