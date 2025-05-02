using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class experienceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Experiences_ExperienceId",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ObjectiveImages_ExperienceId",
                table: "ObjectiveImages");

            migrationBuilder.DropColumn(
                name: "ExperienceId",
                table: "ObjectiveImages");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveImages_IdExperienta",
                table: "ObjectiveImages",
                column: "IdExperienta");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Experiences_IdExperienta",
                table: "ObjectiveImages",
                column: "IdExperienta",
                principalTable: "Experiences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Experiences_IdExperienta",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ObjectiveImages_IdExperienta",
                table: "ObjectiveImages");

            migrationBuilder.AddColumn<int>(
                name: "ExperienceId",
                table: "ObjectiveImages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveImages_ExperienceId",
                table: "ObjectiveImages",
                column: "ExperienceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Experiences_ExperienceId",
                table: "ObjectiveImages",
                column: "ExperienceId",
                principalTable: "Experiences",
                principalColumn: "Id");
        }
    }
}
