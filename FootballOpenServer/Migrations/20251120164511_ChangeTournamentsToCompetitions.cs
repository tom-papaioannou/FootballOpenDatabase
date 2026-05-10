// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTournamentsToCompetitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamTournament");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropTable(
                name: "TournamentParents");

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

            migrationBuilder.CreateTable(
                name: "CompetitionParents",
                columns: table => new
                {
                    CompetitionParentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompetitionParentType = table.Column<int>(type: "int", nullable: false),
                    NumberOfLeagues = table.Column<int>(type: "int", nullable: false),
                    NumberOfCups = table.Column<int>(type: "int", nullable: false),
                    NumberOfNationalLeagues = table.Column<int>(type: "int", nullable: true),
                    NumberOfNationalCups = table.Column<int>(type: "int", nullable: true),
                    NationalTeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionParents", x => x.CompetitionParentID);
                });

            migrationBuilder.CreateTable(
                name: "Competitions",
                columns: table => new
                {
                    CompetitionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionTeamsType = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CompetitionType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competitions", x => x.CompetitionID);
                    table.ForeignKey(
                        name: "FK_Competitions_CompetitionParents_ParentID",
                        column: x => x.ParentID,
                        principalTable: "CompetitionParents",
                        principalColumn: "CompetitionParentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionTeam",
                columns: table => new
                {
                    CompetitionsCompetitionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamsTeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionTeam", x => new { x.CompetitionsCompetitionID, x.TeamsTeamID });
                    table.ForeignKey(
                        name: "FK_CompetitionTeam_Competitions_CompetitionsCompetitionID",
                        column: x => x.CompetitionsCompetitionID,
                        principalTable: "Competitions",
                        principalColumn: "CompetitionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetitionTeam_Teams_TeamsTeamID",
                        column: x => x.TeamsTeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrainedRoles_PlayerID",
                table: "PlayerTrainedRoles",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTrainedPositions_PlayerID",
                table: "PlayerTrainedPositions",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_ParentID",
                table: "Competitions",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionTeam_TeamsTeamID",
                table: "CompetitionTeam",
                column: "TeamsTeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTrainedPositions_Players_PlayerID",
                table: "PlayerTrainedPositions",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTrainedRoles_Players_PlayerID",
                table: "PlayerTrainedRoles",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTrainedPositions_Players_PlayerID",
                table: "PlayerTrainedPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTrainedRoles_Players_PlayerID",
                table: "PlayerTrainedRoles");

            migrationBuilder.DropTable(
                name: "CompetitionTeam");

            migrationBuilder.DropTable(
                name: "Competitions");

            migrationBuilder.DropTable(
                name: "CompetitionParents");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTrainedRoles_PlayerID",
                table: "PlayerTrainedRoles");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTrainedPositions_PlayerID",
                table: "PlayerTrainedPositions");

            migrationBuilder.DropColumn(
                name: "PlayerTrainedRoleAdaptaption",
                table: "PlayerTrainedRoles");

            migrationBuilder.DropColumn(
                name: "PlayerTrainedPositionAdaptaption",
                table: "PlayerTrainedPositions");

            migrationBuilder.CreateTable(
                name: "TournamentParents",
                columns: table => new
                {
                    TournamentParentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalTeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NumberOfCups = table.Column<int>(type: "int", nullable: false),
                    NumberOfLeagues = table.Column<int>(type: "int", nullable: false),
                    NumberOfNationalCups = table.Column<int>(type: "int", nullable: true),
                    NumberOfNationalLeagues = table.Column<int>(type: "int", nullable: true),
                    TournamentParentType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TournamentParents", x => x.TournamentParentID);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    TournamentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    TournamentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TournamentTeamsType = table.Column<int>(type: "int", nullable: false),
                    TournamentType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.TournamentID);
                    table.ForeignKey(
                        name: "FK_Tournaments_TournamentParents_ParentID",
                        column: x => x.ParentID,
                        principalTable: "TournamentParents",
                        principalColumn: "TournamentParentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamTournament",
                columns: table => new
                {
                    TeamsTeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TournamentsTournamentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamTournament", x => new { x.TeamsTeamID, x.TournamentsTournamentID });
                    table.ForeignKey(
                        name: "FK_TeamTournament_Teams_TeamsTeamID",
                        column: x => x.TeamsTeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamTournament_Tournaments_TournamentsTournamentID",
                        column: x => x.TournamentsTournamentID,
                        principalTable: "Tournaments",
                        principalColumn: "TournamentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamTournament_TournamentsTournamentID",
                table: "TeamTournament",
                column: "TournamentsTournamentID");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_ParentID",
                table: "Tournaments",
                column: "ParentID");
        }
    }
}
