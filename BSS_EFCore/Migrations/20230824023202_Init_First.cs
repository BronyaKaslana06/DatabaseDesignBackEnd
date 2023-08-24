using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSS_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class Init_First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NICKNAME",
                schema: "C##CAR",
                table: "VEHICLE_OWNER",
                newName: "USERNAME");

            migrationBuilder.RenameColumn(
                name: "Score",
                schema: "C##CAR",
                table: "SWITCH_LOG",
                newName: "SCORE");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                newName: "TITLE");

            migrationBuilder.RenameColumn(
                name: "Score",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                newName: "SCORE");

            migrationBuilder.RenameColumn(
                name: "Note",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                newName: "NOTE");

            migrationBuilder.RenameColumn(
                name: "email",
                schema: "C##CAR",
                table: "ADMINISTRATOR",
                newName: "Email");

            migrationBuilder.AddColumn<byte[]>(
                name: "SINP",
                schema: "C##CAR",
                table: "VEHICLE_PARAM",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PLATE_NUMBER",
                schema: "C##CAR",
                table: "VEHICLE",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SINP",
                schema: "C##CAR",
                table: "VEHICLE_PARAM");

            migrationBuilder.DropColumn(
                name: "PLATE_NUMBER",
                schema: "C##CAR",
                table: "VEHICLE");

            migrationBuilder.RenameColumn(
                name: "USERNAME",
                schema: "C##CAR",
                table: "VEHICLE_OWNER",
                newName: "NICKNAME");

            migrationBuilder.RenameColumn(
                name: "SCORE",
                schema: "C##CAR",
                table: "SWITCH_LOG",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "TITLE",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "SCORE",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "NOTE",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "Email",
                schema: "C##CAR",
                table: "ADMINISTRATOR",
                newName: "email");
        }
    }
}
