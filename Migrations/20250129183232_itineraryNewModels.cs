using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class itineraryNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Events_EventId",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Itineraries_ItineraryId",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Objectives_ObjectiveId",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Itineraries_IdItinerary",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ObjectiveImages_IdItinerary",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_EventId",
                table: "ItineraryDetails");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_ItineraryId",
                table: "ItineraryDetails");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_ObjectiveId",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "IdItinerary",
                table: "ObjectiveImages");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "ItineraryId",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "ObjectiveId",
                table: "ItineraryDetails");

            migrationBuilder.AddColumn<string>(
                name: "Interval",
                table: "Objectives",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pret",
                table: "Objectives",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ItineraryDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDetails_IdEvent",
                table: "ItineraryDetails",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDetails_IdObjective",
                table: "ItineraryDetails",
                column: "IdObjective");

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryDetails_Events_IdEvent",
                table: "ItineraryDetails",
                column: "IdEvent",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryDetails_Itineraries_Id",
                table: "ItineraryDetails",
                column: "Id",
                principalTable: "Itineraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryDetails_Objectives_IdObjective",
                table: "ItineraryDetails",
                column: "IdObjective",
                principalTable: "Objectives",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Events_IdEvent",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Itineraries_Id",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Objectives_IdObjective",
                table: "ItineraryDetails");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_IdEvent",
                table: "ItineraryDetails");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_IdObjective",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "Interval",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "Pret",
                table: "Objectives");

            migrationBuilder.AddColumn<int>(
                name: "IdItinerary",
                table: "ObjectiveImages",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ItineraryDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "ItineraryDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItineraryId",
                table: "ItineraryDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ObjectiveId",
                table: "ItineraryDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveImages_IdItinerary",
                table: "ObjectiveImages",
                column: "IdItinerary");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDetails_EventId",
                table: "ItineraryDetails",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDetails_ItineraryId",
                table: "ItineraryDetails",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDetails_ObjectiveId",
                table: "ItineraryDetails",
                column: "ObjectiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryDetails_Events_EventId",
                table: "ItineraryDetails",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryDetails_Itineraries_ItineraryId",
                table: "ItineraryDetails",
                column: "ItineraryId",
                principalTable: "Itineraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItineraryDetails_Objectives_ObjectiveId",
                table: "ItineraryDetails",
                column: "ObjectiveId",
                principalTable: "Objectives",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Itineraries_IdItinerary",
                table: "ObjectiveImages",
                column: "IdItinerary",
                principalTable: "Itineraries",
                principalColumn: "Id");
        }
    }
}
