// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOwnerToTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppUserID",
                table: "Teams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_AppUserID",
                table: "Teams",
                column: "AppUserID",
                unique: true,
                filter: "[AppUserID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AppUsers_AppUserID",
                table: "Teams",
                column: "AppUserID",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AppUsers_AppUserID",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_AppUserID",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "AppUserID",
                table: "Teams");
        }
    }
}
