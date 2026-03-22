using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class TransferPlayersAndStaffToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStats_Players_PlayerID",
                table: "PlayerStats");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTactics_Players_PlayerID",
                table: "PlayerTactics");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTrainedPositions_Players_PlayerID",
                table: "PlayerTrainedPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTrainedRoles_Players_PlayerID",
                table: "PlayerTrainedRoles");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.RenameColumn(
                name: "PlayerID",
                table: "PlayerTrainedRoles",
                newName: "PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerTrainedRoles_PlayerID",
                table: "PlayerTrainedRoles",
                newName: "IX_PlayerTrainedRoles_PersonID");

            migrationBuilder.RenameColumn(
                name: "PlayerID",
                table: "PlayerTrainedPositions",
                newName: "PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerTrainedPositions_PlayerID",
                table: "PlayerTrainedPositions",
                newName: "IX_PlayerTrainedPositions_PersonID");

            migrationBuilder.RenameColumn(
                name: "PlayerID",
                table: "PlayerTactics",
                newName: "PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerTactics_PlayerID",
                table: "PlayerTactics",
                newName: "IX_PlayerTactics_PersonID");

            migrationBuilder.RenameColumn(
                name: "PlayerID",
                table: "PlayerStats",
                newName: "PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerStats_PlayerID",
                table: "PlayerStats",
                newName: "IX_PlayerStats_PersonID");

            migrationBuilder.AddColumn<int>(
                name: "StaffRole",
                table: "People",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStats_People_PersonID",
                table: "PlayerStats",
                column: "PersonID",
                principalTable: "People",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTactics_People_PersonID",
                table: "PlayerTactics",
                column: "PersonID",
                principalTable: "People",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTrainedPositions_People_PersonID",
                table: "PlayerTrainedPositions",
                column: "PersonID",
                principalTable: "People",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTrainedRoles_People_PersonID",
                table: "PlayerTrainedRoles",
                column: "PersonID",
                principalTable: "People",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerStats_People_PersonID",
                table: "PlayerStats");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTactics_People_PersonID",
                table: "PlayerTactics");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTrainedPositions_People_PersonID",
                table: "PlayerTrainedPositions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTrainedRoles_People_PersonID",
                table: "PlayerTrainedRoles");

            migrationBuilder.DropColumn(
                name: "StaffRole",
                table: "People");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "PlayerTrainedRoles",
                newName: "PlayerID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerTrainedRoles_PersonID",
                table: "PlayerTrainedRoles",
                newName: "IX_PlayerTrainedRoles_PlayerID");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "PlayerTrainedPositions",
                newName: "PlayerID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerTrainedPositions_PersonID",
                table: "PlayerTrainedPositions",
                newName: "IX_PlayerTrainedPositions_PlayerID");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "PlayerTactics",
                newName: "PlayerID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerTactics_PersonID",
                table: "PlayerTactics",
                newName: "IX_PlayerTactics_PlayerID");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "PlayerStats",
                newName: "PlayerID");

            migrationBuilder.RenameIndex(
                name: "IX_PlayerStats_PersonID",
                table: "PlayerStats",
                newName: "IX_PlayerStats_PlayerID");

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

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staffs", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_Staffs_People_PersonID",
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
                name: "IX_Staffs_PersonID",
                table: "Staffs",
                column: "PersonID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerStats_Players_PlayerID",
                table: "PlayerStats",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTactics_Players_PlayerID",
                table: "PlayerTactics",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);

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
    }
}
