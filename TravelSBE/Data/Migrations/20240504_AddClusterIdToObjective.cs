using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelSBE.Data.Migrations
{
    public partial class AddClusterIdToObjective : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClusterId",
                table: "Objectives",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClusterId",
                table: "Objectives");
        }
    }
} 