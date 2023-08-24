using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSS_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TimeSpan",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "ParkingFee",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "BINARY_FLOAT");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "C##CAR",
                table: "SWITCH_STATION");

            migrationBuilder.AlterColumn<string>(
                name: "TimeSpan",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "ParkingFee",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "BINARY_FLOAT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);
        }
    }
}
