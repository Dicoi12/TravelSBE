using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelsBE.Migrations
{
    /// <inheritdoc />
    public partial class AddClusterCentroids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClusterCentroids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClusterId = table.Column<int>(type: "integer", nullable: false),
                    CentroidX = table.Column<double>(type: "double precision", nullable: false),
                    CentroidY = table.Column<double>(type: "double precision", nullable: false),
                    CentroidRating = table.Column<double>(type: "double precision", nullable: false),
                    CentroidPrice = table.Column<double>(type: "double precision", nullable: false),
                    CentroidType = table.Column<double>(type: "double precision", nullable: false),
                    ObjectiveCount = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClusterCentroids", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClusterCentroids_ClusterId",
                table: "ClusterCentroids",
                column: "ClusterId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClusterCentroids");
        }
    }
}
