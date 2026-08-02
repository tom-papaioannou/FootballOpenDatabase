using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamTacticPriorities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamTacticPriorities",
                columns: table => new
                {
                    TeamTacticPriorityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamTacticPriorities", x => x.TeamTacticPriorityID);
                    table.CheckConstraint("CK_TeamTacticPriority_Priority", "[Priority] >= 1");
                    table.ForeignKey(
                        name: "FK_TeamTacticPriorities_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamTacticPriorities_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql("""
                WITH SourcePriorities AS
                (
                    SELECT TeamID, TacticID, 0 AS Type, CaptainID AS PersonID FROM Tactics WHERE CaptainID IS NOT NULL
                    UNION ALL
                    SELECT TeamID, TacticID, 1 AS Type, PenaltyTakerID AS PersonID FROM Tactics WHERE PenaltyTakerID IS NOT NULL
                    UNION ALL
                    SELECT TeamID, TacticID, 2 AS Type, RightFreeKickTakerID AS PersonID FROM Tactics WHERE RightFreeKickTakerID IS NOT NULL
                    UNION ALL
                    SELECT TeamID, TacticID, 3 AS Type, LeftFreeKickTakerID AS PersonID FROM Tactics WHERE LeftFreeKickTakerID IS NOT NULL
                    UNION ALL
                    SELECT TeamID, TacticID, 4 AS Type, RightCornerTakerID AS PersonID FROM Tactics WHERE RightCornerTakerID IS NOT NULL
                    UNION ALL
                    SELECT TeamID, TacticID, 5 AS Type, LeftCornerTakerID AS PersonID FROM Tactics WHERE LeftCornerTakerID IS NOT NULL
                ),
                DeduplicatedPriorities AS
                (
                    SELECT TeamID, Type, PersonID, MIN(TacticID) AS FirstTacticID
                    FROM SourcePriorities
                    GROUP BY TeamID, Type, PersonID
                ),
                SelectedPrimaryPriorities AS
                (
                    SELECT
                        TeamID,
                        Type,
                        PersonID,
                        ROW_NUMBER() OVER (PARTITION BY TeamID, Type ORDER BY FirstTacticID, PersonID) AS PriorityRank
                    FROM DeduplicatedPriorities
                )
                INSERT INTO TeamTacticPriorities (TeamTacticPriorityID, TeamID, Type, PersonID, Priority)
                SELECT NEWID(), TeamID, Type, PersonID, 1
                FROM SelectedPrimaryPriorities
                WHERE PriorityRank = 1;
                """);

            migrationBuilder.DropColumn(
                name: "CaptainID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "LeftCornerTakerID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "LeftFreeKickTakerID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "PenaltyTakerID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "RightCornerTakerID",
                table: "Tactics");

            migrationBuilder.DropColumn(
                name: "RightFreeKickTakerID",
                table: "Tactics");

            migrationBuilder.CreateIndex(
                name: "IX_TeamTacticPriorities_PersonID",
                table: "TeamTacticPriorities",
                column: "PersonID");

            migrationBuilder.CreateIndex(
                name: "IX_TeamTacticPriorities_TeamID_Type_PersonID",
                table: "TeamTacticPriorities",
                columns: new[] { "TeamID", "Type", "PersonID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamTacticPriorities_TeamID_Type_Priority",
                table: "TeamTacticPriorities",
                columns: new[] { "TeamID", "Type", "Priority" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamTacticPriorities");

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
                name: "LeftFreeKickTakerID",
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

            migrationBuilder.AddColumn<Guid>(
                name: "RightFreeKickTakerID",
                table: "Tactics",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
