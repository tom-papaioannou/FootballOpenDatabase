// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerTrainedRoleAdaptaption",
                table: "PlayerTrainedRoles");

            migrationBuilder.DropColumn(
                name: "PlayerTrainedPositionAdaptaption",
                table: "PlayerTrainedPositions");

            migrationBuilder.AddColumn<byte>(
                name: "PlayerTrainedRoleAdaptation",
                table: "PlayerTrainedRoles",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "PlayerTrainedPositionAdaptation",
                table: "PlayerTrainedPositions",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "PlayerStats",
                columns: table => new
                {
                    PlayerStatsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Shooting = table.Column<byte>(type: "tinyint", nullable: false),
                    Passing = table.Column<byte>(type: "tinyint", nullable: false),
                    Crossing = table.Column<byte>(type: "tinyint", nullable: false),
                    Tackling = table.Column<byte>(type: "tinyint", nullable: false),
                    Dribbling = table.Column<byte>(type: "tinyint", nullable: false),
                    Control = table.Column<byte>(type: "tinyint", nullable: false),
                    Kicking = table.Column<byte>(type: "tinyint", nullable: false),
                    Goalkeeping = table.Column<byte>(type: "tinyint", nullable: false),
                    Teamwork = table.Column<byte>(type: "tinyint", nullable: false),
                    Creativity = table.Column<byte>(type: "tinyint", nullable: false),
                    Decisions = table.Column<byte>(type: "tinyint", nullable: false),
                    Positioning = table.Column<byte>(type: "tinyint", nullable: false),
                    Speed = table.Column<byte>(type: "tinyint", nullable: false),
                    Acceleration = table.Column<byte>(type: "tinyint", nullable: false),
                    Strength = table.Column<byte>(type: "tinyint", nullable: false),
                    Jumping = table.Column<byte>(type: "tinyint", nullable: false),
                    Stamina = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStats", x => x.PlayerStatsID);
                    table.ForeignKey(
                        name: "FK_PlayerStats_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStats_PlayerID",
                table: "PlayerStats",
                column: "PlayerID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerStats");

            migrationBuilder.DropColumn(
                name: "PlayerTrainedRoleAdaptation",
                table: "PlayerTrainedRoles");

            migrationBuilder.DropColumn(
                name: "PlayerTrainedPositionAdaptation",
                table: "PlayerTrainedPositions");

            migrationBuilder.AddColumn<int>(
                name: "PlayerTrainedRoleAdaptaption",
                table: "PlayerTrainedRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlayerTrainedPositionAdaptaption",
                table: "PlayerTrainedPositions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
