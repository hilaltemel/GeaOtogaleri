using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Car_Dealership.Migrations
{
    /// <inheritdoc />
    public partial class RenamePrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OldPrice",
                table: "Cars",
                newName: "FirstPrice");

            migrationBuilder.RenameColumn(
                name: "NewPrice",
                table: "Cars",
                newName: "CurrentPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstPrice",
                table: "Cars",
                newName: "OldPrice");

            migrationBuilder.RenameColumn(
                name: "CurrentPrice",
                table: "Cars",
                newName: "NewPrice");
        }
    }
}
