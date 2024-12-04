using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class FKChangesv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Itineraries_Users_IdUser",
                table: "Itineraries");

            migrationBuilder.DropIndex(
                name: "IX_Itineraries_IdUser",
                table: "Itineraries");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Itineraries",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Itineraries_UserId",
                table: "Itineraries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Itineraries_Users_UserId",
                table: "Itineraries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Itineraries_Users_UserId",
                table: "Itineraries");

            migrationBuilder.DropIndex(
                name: "IX_Itineraries_UserId",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Itineraries");

            migrationBuilder.CreateIndex(
                name: "IX_Itineraries_IdUser",
                table: "Itineraries",
                column: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Itineraries_Users_IdUser",
                table: "Itineraries",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
