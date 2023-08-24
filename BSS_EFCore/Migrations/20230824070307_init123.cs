using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSS_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "city",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                newName: "City");

            migrationBuilder.AddColumn<float>(
                name: "ParkingFee",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "BINARY_FLOAT",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeSpan",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParkingFee",
                schema: "C##CAR",
                table: "SWITCH_STATION");

            migrationBuilder.DropColumn(
                name: "Tags",
                schema: "C##CAR",
                table: "SWITCH_STATION");

            migrationBuilder.DropColumn(
                name: "TimeSpan",
                schema: "C##CAR",
                table: "SWITCH_STATION");

            migrationBuilder.RenameColumn(
                name: "City",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                newName: "city");
        }
    }
}
