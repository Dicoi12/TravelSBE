using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class ImageForObjectives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Objectives_ObjectiveId",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ObjectiveImages_ObjectiveId",
                table: "ObjectiveImages");

            migrationBuilder.DropColumn(
                name: "ObjectiveId",
                table: "ObjectiveImages");

            migrationBuilder.AlterColumn<int>(
                name: "IdObjective",
                table: "ObjectiveImages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveImages_IdObjective",
                table: "ObjectiveImages",
                column: "IdObjective");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Objectives_IdObjective",
                table: "ObjectiveImages",
                column: "IdObjective",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Objectives_IdObjective",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ObjectiveImages_IdObjective",
                table: "ObjectiveImages");

            migrationBuilder.AlterColumn<int>(
                name: "IdObjective",
                table: "ObjectiveImages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ObjectiveId",
                table: "ObjectiveImages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveImages_ObjectiveId",
                table: "ObjectiveImages",
                column: "ObjectiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Objectives_ObjectiveId",
                table: "ObjectiveImages",
                column: "ObjectiveId",
                principalTable: "Objectives",
                principalColumn: "Id");
        }
    }
}
