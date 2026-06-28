// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class RestrictDeletingContinentWithNations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nations_Continents_ContinentID",
                table: "Nations");

            migrationBuilder.AddForeignKey(
                name: "FK_Nations_Continents_ContinentID",
                table: "Nations",
                column: "ContinentID",
                principalTable: "Continents",
                principalColumn: "ContinentID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nations_Continents_ContinentID",
                table: "Nations");

            migrationBuilder.AddForeignKey(
                name: "FK_Nations_Continents_ContinentID",
                table: "Nations",
                column: "ContinentID",
                principalTable: "Continents",
                principalColumn: "ContinentID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
