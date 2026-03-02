using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddServers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_Nations_ContinentID",
                table: "Competitions");

            migrationBuilder.AddColumn<Guid>(
                name: "ServerID",
                table: "People",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ServerID",
                table: "Competitions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    ServerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.ServerID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_People_ServerID",
                table: "People",
                column: "ServerID");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_ServerID",
                table: "Competitions",
                column: "ServerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_Continents_ContinentID",
                table: "Competitions",
                column: "ContinentID",
                principalTable: "Continents",
                principalColumn: "ContinentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_Servers_ServerID",
                table: "Competitions",
                column: "ServerID",
                principalTable: "Servers",
                principalColumn: "ServerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_People_Servers_ServerID",
                table: "People",
                column: "ServerID",
                principalTable: "Servers",
                principalColumn: "ServerID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_Continents_ContinentID",
                table: "Competitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_Servers_ServerID",
                table: "Competitions");

            migrationBuilder.DropForeignKey(
                name: "FK_People_Servers_ServerID",
                table: "People");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_People_ServerID",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_Competitions_ServerID",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "ServerID",
                table: "People");

            migrationBuilder.DropColumn(
                name: "ServerID",
                table: "Competitions");

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_Nations_ContinentID",
                table: "Competitions",
                column: "ContinentID",
                principalTable: "Nations",
                principalColumn: "NationID");
        }
    }
}
