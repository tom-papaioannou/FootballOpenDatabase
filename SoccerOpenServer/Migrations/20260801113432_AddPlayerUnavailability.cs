using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerUnavailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerCompetitionDisciplines",
                columns: table => new
                {
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YellowCards = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCompetitionDisciplines", x => new { x.PersonID, x.CompetitionID });
                    table.CheckConstraint("CK_PlayerCompetitionDiscipline_YellowCards", "[YellowCards] >= 0 AND [YellowCards] < 3");
                    table.ForeignKey(
                        name: "FK_PlayerCompetitionDisciplines_Competitions_CompetitionID",
                        column: x => x.CompetitionID,
                        principalTable: "Competitions",
                        principalColumn: "CompetitionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerCompetitionDisciplines_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerUnavailabilities",
                columns: table => new
                {
                    PlayerUnavailabilityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MatchesRemaining = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerUnavailabilities", x => x.PlayerUnavailabilityID);
                    table.CheckConstraint("CK_PlayerUnavailability_MatchesRemaining", "[MatchesRemaining] > 0");
                    table.ForeignKey(
                        name: "FK_PlayerUnavailabilities_Competitions_CompetitionID",
                        column: x => x.CompetitionID,
                        principalTable: "Competitions",
                        principalColumn: "CompetitionID");
                    table.ForeignKey(
                        name: "FK_PlayerUnavailabilities_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCompetitionDisciplines_CompetitionID",
                table: "PlayerCompetitionDisciplines",
                column: "CompetitionID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUnavailabilities_CompetitionID",
                table: "PlayerUnavailabilities",
                column: "CompetitionID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerUnavailabilities_PersonID",
                table: "PlayerUnavailabilities",
                column: "PersonID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerCompetitionDisciplines");

            migrationBuilder.DropTable(
                name: "PlayerUnavailabilities");
        }
    }
}
