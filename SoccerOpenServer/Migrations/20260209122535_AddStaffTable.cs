// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staff_People_PersonID",
                table: "Staff");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Teams_TeamID",
                table: "Staff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staff",
                table: "Staff");

            migrationBuilder.RenameTable(
                name: "Staff",
                newName: "Staffs");

            migrationBuilder.RenameIndex(
                name: "IX_Staff_TeamID",
                table: "Staffs",
                newName: "IX_Staffs_TeamID");

            migrationBuilder.RenameIndex(
                name: "IX_Staff_PersonID",
                table: "Staffs",
                newName: "IX_Staffs_PersonID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs",
                column: "StaffID");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_People_PersonID",
                table: "Staffs",
                column: "PersonID",
                principalTable: "People",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Teams_TeamID",
                table: "Staffs",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "TeamID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_People_PersonID",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Teams_TeamID",
                table: "Staffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staffs",
                table: "Staffs");

            migrationBuilder.RenameTable(
                name: "Staffs",
                newName: "Staff");

            migrationBuilder.RenameIndex(
                name: "IX_Staffs_TeamID",
                table: "Staff",
                newName: "IX_Staff_TeamID");

            migrationBuilder.RenameIndex(
                name: "IX_Staffs_PersonID",
                table: "Staff",
                newName: "IX_Staff_PersonID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staff",
                table: "Staff",
                column: "StaffID");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_People_PersonID",
                table: "Staff",
                column: "PersonID",
                principalTable: "People",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Teams_TeamID",
                table: "Staff",
                column: "TeamID",
                principalTable: "Teams",
                principalColumn: "TeamID");
        }
    }
}
