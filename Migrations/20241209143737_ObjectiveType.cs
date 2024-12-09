using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class ObjectiveType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Objectives",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ObjectiveTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectiveTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_Type",
                table: "Objectives",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_Objectives_ObjectiveTypes_Type",
                table: "Objectives",
                column: "Type",
                principalTable: "ObjectiveTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objectives_ObjectiveTypes_Type",
                table: "Objectives");

            migrationBuilder.DropTable(
                name: "ObjectiveTypes");

            migrationBuilder.DropIndex(
                name: "IX_Objectives_Type",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Objectives");
        }
    }
}
