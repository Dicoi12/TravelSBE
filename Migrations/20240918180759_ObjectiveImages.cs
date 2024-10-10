using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class ObjectiveImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectiveImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageData = table.Column<byte[]>(type: "bytea", nullable: false),
                    ImageMimeType = table.Column<string>(type: "text", nullable: false),
                    IdObjective = table.Column<int>(type: "integer", nullable: true),
                    ObjectiveId = table.Column<int>(type: "integer", nullable: true),
                    IdEvent = table.Column<int>(type: "integer", nullable: true),
                    EventId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectiveImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectiveImages_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ObjectiveImages_Objectives_ObjectiveId",
                        column: x => x.ObjectiveId,
                        principalTable: "Objectives",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveImages_EventId",
                table: "ObjectiveImages",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveImages_ObjectiveId",
                table: "ObjectiveImages",
                column: "ObjectiveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectiveImages");
        }
    }
}
