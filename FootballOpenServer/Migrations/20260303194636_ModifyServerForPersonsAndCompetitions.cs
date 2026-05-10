// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class ModifyServerForPersonsAndCompetitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_Servers_ServerID",
                table: "Competitions");

            migrationBuilder.DropForeignKey(
                name: "FK_People_Servers_ServerID",
                table: "People");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServerID",
                table: "People",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServerID",
                table: "Competitions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_Servers_ServerID",
                table: "Competitions",
                column: "ServerID",
                principalTable: "Servers",
                principalColumn: "ServerID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_People_Servers_ServerID",
                table: "People",
                column: "ServerID",
                principalTable: "Servers",
                principalColumn: "ServerID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Competitions_Servers_ServerID",
                table: "Competitions");

            migrationBuilder.DropForeignKey(
                name: "FK_People_Servers_ServerID",
                table: "People");

            migrationBuilder.AlterColumn<Guid>(
                name: "ServerID",
                table: "People",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ServerID",
                table: "Competitions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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
    }
}
