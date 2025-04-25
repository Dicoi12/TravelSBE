using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Itineraries_Id",
                table: "ItineraryDetails");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ItineraryDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDetails_IdItinerary",
                table: "ItineraryDetails",
                column: "IdItinerary");

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryDetails_Itineraries_IdItinerary",
                table: "ItineraryDetails",
                column: "IdItinerary",
                principalTable: "Itineraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Itineraries_IdItinerary",
                table: "ItineraryDetails");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_IdItinerary",
                table: "ItineraryDetails");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ItineraryDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryDetails_Itineraries_Id",
                table: "ItineraryDetails",
                column: "Id",
                principalTable: "Itineraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
