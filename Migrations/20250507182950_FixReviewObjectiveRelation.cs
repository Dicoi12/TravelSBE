using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class FixReviewObjectiveRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Objectives_Id",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Reviews",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_IdObjective",
                table: "Reviews",
                column: "IdObjective");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Objectives_IdObjective",
                table: "Reviews",
                column: "IdObjective",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Objectives_IdObjective",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_IdObjective",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Reviews",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Objectives_Id",
                table: "Reviews",
                column: "Id",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
