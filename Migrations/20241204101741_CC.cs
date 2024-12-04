using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class CC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Events_IdEvent",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Objectives_IdObjective",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Events_EventId",
                table: "ObjectiveImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Itineraries_IdItinerary",
                table: "ObjectiveImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Objectives_IdObjective",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ObjectiveImages_EventId",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_IdEvent",
                table: "ItineraryDetails");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_IdObjective",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "ObjectiveImages");

            migrationBuilder.AlterColumn<int>(
                name: "IdObjective",
                table: "ObjectiveImages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "IdItinerary",
                table: "ObjectiveImages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "ItineraryDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdItinerary",
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
                name: "IX_ObjectiveImages_IdEvent",
                table: "ObjectiveImages",
                column: "IdEvent");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryDetails_EventId",
                table: "ItineraryDetails",
                column: "EventId");

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
                name: "FK_ItineraryDetails_Objectives_ObjectiveId",
                table: "ItineraryDetails",
                column: "ObjectiveId",
                principalTable: "Objectives",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Events_IdEvent",
                table: "ObjectiveImages",
                column: "IdEvent",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Itineraries_IdItinerary",
                table: "ObjectiveImages",
                column: "IdItinerary",
                principalTable: "Itineraries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Objectives_IdObjective",
                table: "ObjectiveImages",
                column: "IdObjective",
                principalTable: "Objectives",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Events_EventId",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ItineraryDetails_Objectives_ObjectiveId",
                table: "ItineraryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Events_IdEvent",
                table: "ObjectiveImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Itineraries_IdItinerary",
                table: "ObjectiveImages");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectiveImages_Objectives_IdObjective",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ObjectiveImages_IdEvent",
                table: "ObjectiveImages");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_EventId",
                table: "ItineraryDetails");

            migrationBuilder.DropIndex(
                name: "IX_ItineraryDetails_ObjectiveId",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "IdItinerary",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "ObjectiveId",
                table: "ItineraryDetails");

            migrationBuilder.AlterColumn<int>(
                name: "IdObjective",
                table: "ObjectiveImages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdItinerary",
                table: "ObjectiveImages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "ObjectiveImages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectiveImages_EventId",
                table: "ObjectiveImages",
                column: "EventId");

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
                name: "FK_ItineraryDetails_Objectives_IdObjective",
                table: "ItineraryDetails",
                column: "IdObjective",
                principalTable: "Objectives",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Events_EventId",
                table: "ObjectiveImages",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Itineraries_IdItinerary",
                table: "ObjectiveImages",
                column: "IdItinerary",
                principalTable: "Itineraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectiveImages_Objectives_IdObjective",
                table: "ObjectiveImages",
                column: "IdObjective",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
