// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffRole = table.Column<int>(type: "int", nullable: false),
                    TeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_Staff_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Staff_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "TeamID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerTactics_PlayerID",
                table: "PlayerTactics",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_PersonID",
                table: "Staff",
                column: "PersonID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staff_TeamID",
                table: "Staff",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerTactics_Players_PlayerID",
                table: "PlayerTactics",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerTactics_Players_PlayerID",
                table: "PlayerTactics");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_PlayerTactics_PlayerID",
                table: "PlayerTactics");
        }
    }
}
