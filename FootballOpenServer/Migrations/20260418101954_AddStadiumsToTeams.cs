using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddStadiumsToTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StadiumID",
                table: "Teams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Stadium",
                columns: table => new
                {
                    StadiumID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    StadiumState = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stadium", x => x.StadiumID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_StadiumID",
                table: "Teams",
                column: "StadiumID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Stadium_StadiumID",
                table: "Teams",
                column: "StadiumID",
                principalTable: "Stadium",
                principalColumn: "StadiumID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Stadium_StadiumID",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "Stadium");

            migrationBuilder.DropIndex(
                name: "IX_Teams_StadiumID",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "StadiumID",
                table: "Teams");
        }
    }
}
