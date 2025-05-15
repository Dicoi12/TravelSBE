using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelsBE.Migrations
{
    /// <inheritdoc />
    public partial class itineraryMoreDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ItineraryDetails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "EstimatedTime",
                table: "ItineraryDetails",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataStart",
                table: "Itineraries",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataStop",
                table: "Itineraries",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "EstimatedTime",
                table: "ItineraryDetails");

            migrationBuilder.DropColumn(
                name: "DataStart",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "DataStop",
                table: "Itineraries");
        }
    }
}
