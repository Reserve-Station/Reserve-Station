using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class PatronTierAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rmc_patron_tier_assignments",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    tier_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rmc_patron_tier_assignments", x => new { x.player_id, x.tier_id });
                    table.ForeignKey(
                        name: "FK_rmc_patron_tier_assignments_rmc_patron_tiers_tier_id",
                        column: x => x.tier_id,
                        principalTable: "rmc_patron_tiers",
                        principalColumn: "rmc_patron_tiers_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rmc_patron_tier_assignments_rmc_patrons_patron_player_id",
                        column: x => x.player_id,
                        principalTable: "rmc_patrons",
                        principalColumn: "player_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rmc_patron_tier_assignments_tier_id",
                table: "rmc_patron_tier_assignments",
                column: "tier_id");

            migrationBuilder.Sql(
                "INSERT INTO rmc_patron_tier_assignments (player_id, tier_id) " +
                "SELECT player_id, tier_id FROM rmc_patrons;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rmc_patron_tier_assignments");
        }
    }
}
