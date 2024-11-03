using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelSBE.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "ObjectiveImages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "ObjectiveImages");
        }
    }
}
