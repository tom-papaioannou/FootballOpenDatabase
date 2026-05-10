// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTacticName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tactics",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tactics");
        }
    }
}
