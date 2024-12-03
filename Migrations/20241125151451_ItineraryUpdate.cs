using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class ItineraryUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Itineraries_Users_IdUser",
                table: "Itineraries");

            migrationBuilder.AddColumn<int>(
                name: "IdItinerary",
                table: "ObjectiveImages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Descriere",
                table: "ItineraryDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItineraryId",
                table: "ItineraryDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ItineraryDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "IdUser",
                table: "Itineraries",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveImages_IdItinerary",
                table: "ObjectiveImages",
                column: "IdItinerary");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDetails_ItineraryId",
                table: "ItineraryDetails",
                column: "ItineraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Itineraries_Users_IdUser",
                table: "Itineraries",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryDetails_Itineraries_ItineraryId",
                table: "ItineraryDetails",
                column: "ItineraryId",
                principalTable: "Itineraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Itineraries_IdItinerary",
                table: "ObjectiveImages",
                column: "IdItinerary",
                principalTable: "Itineraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Itineraries_Users_IdUser",
                table: "Itineraries");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Itineraries_ItineraryId",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Itineraries_IdItinerary",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ObjectiveImages_IdItinerary",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_ItineraryId",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "IdItinerary",
                table: "ObjectiveImages");

            migrationBuilder.DropColumn(
                name: "Descriere",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "ItineraryId",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ItineraryDetails");

            migrationBuilder.AlterColumn<int>(
                name: "IdUser",
                table: "Itineraries",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Itineraries_Users_IdUser",
                table: "Itineraries",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
