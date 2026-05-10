// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class ConnectAppUsersWithPeople : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PersonID",
                table: "AppUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_PersonID",
                table: "AppUsers",
                column: "PersonID",
                unique: true,
                filter: "[PersonID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_People_PersonID",
                table: "AppUsers",
                column: "PersonID",
                principalTable: "People",
                principalColumn: "PersonID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_People_PersonID",
                table: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_PersonID",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "PersonID",
                table: "AppUsers");
        }
    }
}
