using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddCoaches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoachStats",
                columns: table => new
                {
                    CoachStatsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Attack = table.Column<byte>(type: "tinyint", nullable: false),
                    Defend = table.Column<byte>(type: "tinyint", nullable: false),
                    Control = table.Column<byte>(type: "tinyint", nullable: false),
                    Goalkeeper = table.Column<byte>(type: "tinyint", nullable: false),
                    Tactic = table.Column<byte>(type: "tinyint", nullable: false),
                    Fitness = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoachStats", x => x.CoachStatsID);
                    table.CheckConstraint("CK_CoachStats_Attack", "[Attack] >= 1 AND [Attack] <= 100");
                    table.CheckConstraint("CK_CoachStats_Control", "[Control] >= 1 AND [Control] <= 100");
                    table.CheckConstraint("CK_CoachStats_Defend", "[Defend] >= 1 AND [Defend] <= 100");
                    table.CheckConstraint("CK_CoachStats_Fitness", "[Fitness] >= 1 AND [Fitness] <= 100");
                    table.CheckConstraint("CK_CoachStats_Goalkeeper", "[Goalkeeper] >= 1 AND [Goalkeeper] <= 100");
                    table.CheckConstraint("CK_CoachStats_Tactic", "[Tactic] >= 1 AND [Tactic] <= 100");
                    table.ForeignKey(
                        name: "FK_CoachStats_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoachStats_PersonID",
                table: "CoachStats",
                column: "PersonID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoachStats");
        }
    }
}
