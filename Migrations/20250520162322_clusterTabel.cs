using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelsBE.Migrations
{
    /// <inheritdoc />
    public partial class clusterTabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClusterNeighbors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SourceObjectiveId = table.Column<int>(type: "integer", nullable: false),
                    TargetObjectiveId = table.Column<int>(type: "integer", nullable: false),
                    ClusterId = table.Column<int>(type: "integer", nullable: false),
                    Distance = table.Column<double>(type: "double precision", nullable: false),
                    SimilarityScore = table.Column<double>(type: "double precision", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClusterNeighbors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClusterNeighbors_Objectives_SourceObjectiveId",
                        column: x => x.SourceObjectiveId,
                        principalTable: "Objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClusterNeighbors_Objectives_TargetObjectiveId",
                        column: x => x.TargetObjectiveId,
                        principalTable: "Objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClusterNeighbors_ClusterId",
                table: "ClusterNeighbors",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_ClusterNeighbors_SourceObjectiveId_TargetObjectiveId",
                table: "ClusterNeighbors",
                columns: new[] { "SourceObjectiveId", "TargetObjectiveId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClusterNeighbors_TargetObjectiveId",
                table: "ClusterNeighbors",
                column: "TargetObjectiveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClusterNeighbors");
        }
    }
}
