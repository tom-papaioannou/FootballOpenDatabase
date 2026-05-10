// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddNationsAndContinents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_CompetitionParents_ParentID",
                table: "Competitions");

            migrationBuilder.DropTable(
                name: "CompetitionParents");

            migrationBuilder.DropIndex(
                name: "IX_Competitions_ParentID",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "Competitions");

            migrationBuilder.AddColumn<Guid>(
                name: "NationID",
                table: "People",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContinentID",
                table: "Competitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NationID",
                table: "Competitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Continents",
                columns: table => new
                {
                    ContinentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SymbolUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Continents", x => x.ContinentID);
                });

            migrationBuilder.CreateTable(
                name: "Nations",
                columns: table => new
                {
                    NationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISO2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISO3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlagUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContinentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nations", x => x.NationID);
                    table.ForeignKey(
                        name: "FK_Nations_Continents_ContinentID",
                        column: x => x.ContinentID,
                        principalTable: "Continents",
                        principalColumn: "ContinentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_People_NationID",
                table: "People",
                column: "NationID");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_ContinentID",
                table: "Competitions",
                column: "ContinentID");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_NationID",
                table: "Competitions",
                column: "NationID");

            migrationBuilder.CreateIndex(
                name: "IX_Nations_ContinentID",
                table: "Nations",
                column: "ContinentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_Nations_ContinentID",
                table: "Competitions",
                column: "ContinentID",
                principalTable: "Nations",
                principalColumn: "NationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_Nations_NationID",
                table: "Competitions",
                column: "NationID",
                principalTable: "Nations",
                principalColumn: "NationID");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Nations_NationID",
                table: "People",
                column: "NationID",
                principalTable: "Nations",
                principalColumn: "NationID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_Nations_ContinentID",
                table: "Competitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_Nations_NationID",
                table: "Competitions");

            migrationBuilder.DropForeignKey(
                name: "FK_People_Nations_NationID",
                table: "People");

            migrationBuilder.DropTable(
                name: "Nations");

            migrationBuilder.DropTable(
                name: "Continents");

            migrationBuilder.DropIndex(
                name: "IX_People_NationID",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_Competitions_ContinentID",
                table: "Competitions");

            migrationBuilder.DropIndex(
                name: "IX_Competitions_NationID",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "NationID",
                table: "People");

            migrationBuilder.DropColumn(
                name: "ContinentID",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "NationID",
                table: "Competitions");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentID",
                table: "Competitions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CompetitionParents",
                columns: table => new
                {
                    CompetitionParentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionParentType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalTeamID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NumberOfCups = table.Column<int>(type: "int", nullable: false),
                    NumberOfLeagues = table.Column<int>(type: "int", nullable: false),
                    NumberOfNationalCups = table.Column<int>(type: "int", nullable: true),
                    NumberOfNationalLeagues = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionParents", x => x.CompetitionParentID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_ParentID",
                table: "Competitions",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_CompetitionParents_ParentID",
                table: "Competitions",
                column: "ParentID",
                principalTable: "CompetitionParents",
                principalColumn: "CompetitionParentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
