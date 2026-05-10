// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTactics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlaceOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.PersonID);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTactics",
                columns: table => new
                {
                    PlayerTacticID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TacticID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerPosition = table.Column<int>(type: "int", nullable: false),
                    PlayerRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTactics", x => x.PlayerTacticID);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTrainedPositions",
                columns: table => new
                {
                    PlayerTrainedPositionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerPosition = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTrainedPositions", x => x.PlayerTrainedPositionID);
                });

            migrationBuilder.CreateTable(
                name: "PlayerTrainedRoles",
                columns: table => new
                {
                    PlayerTrainedRoleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerPosition = table.Column<int>(type: "int", nullable: false),
                    PlayerRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerTrainedRoles", x => x.PlayerTrainedRoleID);
                });

            migrationBuilder.CreateTable(
                name: "Tactics",
                columns: table => new
                {
                    TacticID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    isMain = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tactics", x => x.TacticID);
                    table.ForeignKey(
                        name: "FK_Tactics_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerID);
                    table.ForeignKey(
                        name: "FK_Players_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_PersonID",
                table: "Players",
                column: "PersonID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tactics_TeamID",
                table: "Tactics",
                column: "TeamID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "PlayerTactics");

            migrationBuilder.DropTable(
                name: "PlayerTrainedPositions");

            migrationBuilder.DropTable(
                name: "PlayerTrainedRoles");

            migrationBuilder.DropTable(
                name: "Tactics");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Contracts");
        }
    }
}
