using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamKits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "KitID",
                table: "Teams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Kits",
                columns: table => new
                {
                    KitID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HomeShirtColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeShortsColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwayShirtColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwayShortsColor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kits", x => x.KitID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_KitID",
                table: "Teams",
                column: "KitID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Kits_KitID",
                table: "Teams",
                column: "KitID",
                principalTable: "Kits",
                principalColumn: "KitID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Kits_KitID",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "Kits");

            migrationBuilder.DropIndex(
                name: "IX_Teams_KitID",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "KitID",
                table: "Teams");
        }
    }
}
