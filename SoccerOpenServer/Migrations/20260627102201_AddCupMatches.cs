using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddCupMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CupRounds",
                columns: table => new
                {
                    CupRoundID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    TeamCount = table.Column<int>(type: "int", nullable: false),
                    RoundType = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CupRounds", x => x.CupRoundID);
                    table.ForeignKey(
                        name: "FK_CupRounds_Competitions_CompetitionID",
                        column: x => x.CompetitionID,
                        principalTable: "Competitions",
                        principalColumn: "CompetitionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CupTies",
                columns: table => new
                {
                    CupTieID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CupRoundID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TieNumber = table.Column<int>(type: "int", nullable: false),
                    HomeTeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AwayTeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WinnerTeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NextCupTieID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdvancesAsHomeTeam = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CupTies", x => x.CupTieID);
                    table.ForeignKey(
                        name: "FK_CupTies_CupRounds_CupRoundID",
                        column: x => x.CupRoundID,
                        principalTable: "CupRounds",
                        principalColumn: "CupRoundID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CupTies_CupTies_NextCupTieID",
                        column: x => x.NextCupTieID,
                        principalTable: "CupTies",
                        principalColumn: "CupTieID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CupRounds_CompetitionID_RoundNumber",
                table: "CupRounds",
                columns: new[] { "CompetitionID", "RoundNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CupTies_CupRoundID_TieNumber",
                table: "CupTies",
                columns: new[] { "CupRoundID", "TieNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CupTies_NextCupTieID",
                table: "CupTies",
                column: "NextCupTieID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CupTies");

            migrationBuilder.DropTable(
                name: "CupRounds");
        }
    }
}
