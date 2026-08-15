using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoccerOpenServer.Migrations
{
    /// <inheritdoc />
    public partial class AddMedics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicStats",
                columns: table => new
                {
                    MedicStatsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Diagnosis = table.Column<byte>(type: "tinyint", nullable: false),
                    Treatment = table.Column<byte>(type: "tinyint", nullable: false),
                    Rehabilitation = table.Column<byte>(type: "tinyint", nullable: false),
                    Prevention = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicStats", x => x.MedicStatsID);
                    table.CheckConstraint("CK_MedicStats_Diagnosis", "[Diagnosis] >= 1 AND [Diagnosis] <= 100");
                    table.CheckConstraint("CK_MedicStats_Prevention", "[Prevention] >= 1 AND [Prevention] <= 100");
                    table.CheckConstraint("CK_MedicStats_Rehabilitation", "[Rehabilitation] >= 1 AND [Rehabilitation] <= 100");
                    table.CheckConstraint("CK_MedicStats_Treatment", "[Treatment] >= 1 AND [Treatment] <= 100");
                    table.ForeignKey(
                        name: "FK_MedicStats_People_PersonID",
                        column: x => x.PersonID,
                        principalTable: "People",
                        principalColumn: "PersonID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicStats_PersonID",
                table: "MedicStats",
                column: "PersonID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicStats");
        }
    }
}
