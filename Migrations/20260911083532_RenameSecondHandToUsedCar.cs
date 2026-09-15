using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Car_Dealership.Migrations
{
    /// <inheritdoc />
    public partial class RenameSecondHandToUsedCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSecondHand",
                table: "Cars",
                newName: "IsUsedCar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsUsedCar",
                table: "Cars",
                newName: "IsSecondHand");
        }
    }
}
