using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSS_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LONGTITUDE",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                newName: "LONGITUDE");

            migrationBuilder.RenameColumn(
                name: "FALIURE_STATUS",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                newName: "FAILURE_STATUS");

            migrationBuilder.AddColumn<string>(
                name: "ACCOUNT_SERIAL",
                schema: "C##CAR",
                table: "VEHICLE_OWNER",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BatteryTypeId",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                type: "NUMBER(19)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "RequestStatus",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Evaluation",
                schema: "C##CAR",
                table: "SWITCH_LOG",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "APPOINT_TIME",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Evaluation",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "latitude",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                type: "BINARY_DOUBLE",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                type: "BINARY_DOUBLE",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "ACCOUNT_SERIAL",
                schema: "C##CAR",
                table: "EMPLOYEE",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "C##CAR",
                table: "BATTERY_TYPE",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ACCOUNT_SERIAL",
                schema: "C##CAR",
                table: "ADMINISTRATOR",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SWITCH_REQUEST_BatteryTypeId",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                column: "BatteryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SWITCH_REQUEST_BATTERY_TYPE_BatteryTypeId",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                column: "BatteryTypeId",
                principalSchema: "C##CAR",
                principalTable: "BATTERY_TYPE",
                principalColumn: "BATTERY_TYPE_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SWITCH_REQUEST_BATTERY_TYPE_BatteryTypeId",
                schema: "C##CAR",
                table: "SWITCH_REQUEST");

            migrationBuilder.DropIndex(
                name: "IX_SWITCH_REQUEST_BatteryTypeId",
                schema: "C##CAR",
                table: "SWITCH_REQUEST");

            migrationBuilder.DropColumn(
                name: "ACCOUNT_SERIAL",
                schema: "C##CAR",
                table: "VEHICLE_OWNER");

            migrationBuilder.DropColumn(
                name: "BatteryTypeId",
                schema: "C##CAR",
                table: "SWITCH_REQUEST");

            migrationBuilder.DropColumn(
                name: "RequestStatus",
                schema: "C##CAR",
                table: "SWITCH_REQUEST");

            migrationBuilder.DropColumn(
                name: "Evaluation",
                schema: "C##CAR",
                table: "SWITCH_LOG");

            migrationBuilder.DropColumn(
                name: "APPOINT_TIME",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM");

            migrationBuilder.DropColumn(
                name: "Evaluation",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM");

            migrationBuilder.DropColumn(
                name: "latitude",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM");

            migrationBuilder.DropColumn(
                name: "longitude",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM");

            migrationBuilder.DropColumn(
                name: "ACCOUNT_SERIAL",
                schema: "C##CAR",
                table: "EMPLOYEE");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "C##CAR",
                table: "BATTERY_TYPE");

            migrationBuilder.DropColumn(
                name: "ACCOUNT_SERIAL",
                schema: "C##CAR",
                table: "ADMINISTRATOR");

            migrationBuilder.RenameColumn(
                name: "LONGITUDE",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                newName: "LONGTITUDE");

            migrationBuilder.RenameColumn(
                name: "FAILURE_STATUS",
                schema: "C##CAR",
                table: "SWITCH_STATION",
                newName: "FALIURE_STATUS");
        }
    }
}
