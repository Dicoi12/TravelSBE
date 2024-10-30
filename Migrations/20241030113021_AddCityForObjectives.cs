using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class AddCityForObjectives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Objectives",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Objectives");
        }
    }
}
