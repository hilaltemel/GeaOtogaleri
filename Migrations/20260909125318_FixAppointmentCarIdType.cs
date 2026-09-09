using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Car_Dealership.Migrations
{
    /// <inheritdoc />
    public partial class FixAppointmentCarIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Cars_CarId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CarId1",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CarId1",
                table: "Appointments");

            migrationBuilder.AlterColumn<long>(
                name: "CarId",
                table: "Appointments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CarId",
                table: "Appointments",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Cars_CarId",
                table: "Appointments",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Cars_CarId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CarId",
                table: "Appointments");

            migrationBuilder.AlterColumn<int>(
                name: "CarId",
                table: "Appointments",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CarId1",
                table: "Appointments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CarId1",
                table: "Appointments",
                column: "CarId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Cars_CarId1",
                table: "Appointments",
                column: "CarId1",
                principalTable: "Cars",
                principalColumn: "Id");
        }
    }
}
