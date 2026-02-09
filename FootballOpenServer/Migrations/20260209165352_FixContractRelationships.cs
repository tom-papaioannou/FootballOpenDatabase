using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class FixContractRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Teams_TeamID",
                table: "Staffs");

            migrationBuilder.DropIndex(
                name: "IX_Staffs_TeamID",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "TeamID",
                table: "Staffs");

            migrationBuilder.DropColumn(
                name: "ContractID",
                table: "People");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_PersonID",
                table: "Contracts",
                column: "PersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_People_PersonID",
                table: "Contracts",
                column: "PersonID",
                principalTable: "People",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_People_PersonID",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_PersonID",
                table: "Contracts");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamID",
                table: "Staffs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContractID",
                table: "People",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Staffs_TeamID",
                table: "Staffs",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Teams_TeamID",
                table: "Staffs",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "TeamID");
        }
    }
}
