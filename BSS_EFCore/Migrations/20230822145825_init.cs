using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSS_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "C##CAR");

            migrationBuilder.CreateTable(
                name: "ADMINISTRATOR",
                schema: "C##CAR",
                columns: table => new
                {
                    ADMIN_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PASSWORD = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009148", x => x.ADMIN_ID);
                });

            migrationBuilder.CreateTable(
                name: "BATTERY_TYPE",
                schema: "C##CAR",
                columns: table => new
                {
                    BATTERY_TYPE_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MAX_CHARGE_TIEMS = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TOTAL_CAPACITY = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009070", x => x.BATTERY_TYPE_ID);
                });

            migrationBuilder.CreateTable(
                name: "SWITCH_STATION",
                schema: "C##CAR",
                columns: table => new
                {
                    STATION_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    STATION_NAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    SERVICE_FEE = table.Column<float>(type: "BINARY_FLOAT", nullable: false),
                    ElectricityFee = table.Column<float>(type: "BINARY_FLOAT", nullable: false),
                    QueueLength = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    LONGTITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    FALIURE_STATUS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BATTERY_CAPACITY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AVAILABLE_BATTERY_COUNT = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    city = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009065", x => x.STATION_ID);
                });

            migrationBuilder.CreateTable(
                name: "VEHICLE_OWNER",
                schema: "C##CAR",
                columns: table => new
                {
                    OWNER_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NICKNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    PASSWORD = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    PROFILE_PHOTO = table.Column<byte[]>(type: "BLOB", nullable: true),
                    CREATE_TIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    GENDER = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: true),
                    BIRTHDAY = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009088", x => x.OWNER_ID);
                });

            migrationBuilder.CreateTable(
                name: "VEHICLE_PARAM",
                schema: "C##CAR",
                columns: table => new
                {
                    VEHICLE_MODEL = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ModelName = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TRANSMISSION = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    SERVICE_TERM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    MANUFACTURER = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    MAX_SPEED = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009081", x => x.VEHICLE_MODEL);
                });

            migrationBuilder.CreateTable(
                name: "NEWS",
                schema: "C##CAR",
                columns: table => new
                {
                    ANNOUNCEMENT_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PUBLISH_TIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PUBLISH_POS = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    TITLE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    CONTENTS = table.Column<string>(type: "NCLOB", maxLength: 2500, nullable: true),
                    LIKES = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VIEW_COUNT = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    administratorAdminId = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009098", x => x.ANNOUNCEMENT_ID);
                    table.ForeignKey(
                        name: "FK_NEWS_ADMINISTRATOR_administratorAdminId",
                        column: x => x.administratorAdminId,
                        principalSchema: "C##CAR",
                        principalTable: "ADMINISTRATOR",
                        principalColumn: "ADMIN_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BATTERY",
                schema: "C##CAR",
                columns: table => new
                {
                    BATTERY_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    AVAILABLE_STATUS = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    CURRENT_CAPACITY = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    CURR_CHARGE_TIMES = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MANUFACTURING_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    switchStationStationId = table.Column<long>(type: "NUMBER(19)", nullable: true),
                    BatteryTypeId = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009076", x => x.BATTERY_ID);
                    table.ForeignKey(
                        name: "FK_BATTERY_BATTERY_TYPE_BatteryTypeId",
                        column: x => x.BatteryTypeId,
                        principalSchema: "C##CAR",
                        principalTable: "BATTERY_TYPE",
                        principalColumn: "BATTERY_TYPE_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BATTERY_SWITCH_STATION_switchStationStationId",
                        column: x => x.switchStationStationId,
                        principalSchema: "C##CAR",
                        principalTable: "SWITCH_STATION",
                        principalColumn: "STATION_ID");
                });

            migrationBuilder.CreateTable(
                name: "EMPLOYEE",
                schema: "C##CAR",
                columns: table => new
                {
                    EMPLOYEE_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    PASSWORD = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false, defaultValue: "123456"),
                    PROFILE_PHOTO = table.Column<byte[]>(type: "BLOB", nullable: true),
                    CREATE_TIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    IDENTITYNUMBER = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    NAME = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    GENDER = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: true),
                    POSITIONS = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SALARY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    switchStationStationId = table.Column<long>(type: "NUMBER(19)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009095", x => x.EMPLOYEE_ID);
                    table.ForeignKey(
                        name: "FK_EMPLOYEE_SWITCH_STATION_switchStationStationId",
                        column: x => x.switchStationStationId,
                        principalSchema: "C##CAR",
                        principalTable: "SWITCH_STATION",
                        principalColumn: "STATION_ID");
                });

            migrationBuilder.CreateTable(
                name: "OWNERPOS",
                schema: "C##CAR",
                columns: table => new
                {
                    Owner_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ADDRESS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    vehicleownerOwnerId = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009099", x => x.Owner_ID);
                    table.ForeignKey(
                        name: "FK_OWNERPOS_VEHICLE_OWNER_vehicleownerOwnerId",
                        column: x => x.vehicleownerOwnerId,
                        principalSchema: "C##CAR",
                        principalTable: "VEHICLE_OWNER",
                        principalColumn: "OWNER_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VEHICLE",
                schema: "C##CAR",
                columns: table => new
                {
                    VEHICLE_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PURCHASE_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    BatteryId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    vehicleOwnerOwnerId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    vehicleParamVehicleModelId = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009110", x => x.VEHICLE_ID);
                    table.ForeignKey(
                        name: "FK_VEHICLE_BATTERY_BatteryId",
                        column: x => x.BatteryId,
                        principalSchema: "C##CAR",
                        principalTable: "BATTERY",
                        principalColumn: "BATTERY_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VEHICLE_VEHICLE_OWNER_vehicleOwnerOwnerId",
                        column: x => x.vehicleOwnerOwnerId,
                        principalSchema: "C##CAR",
                        principalTable: "VEHICLE_OWNER",
                        principalColumn: "OWNER_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VEHICLE_VEHICLE_PARAM_vehicleParamVehicleModelId",
                        column: x => x.vehicleParamVehicleModelId,
                        principalSchema: "C##CAR",
                        principalTable: "VEHICLE_PARAM",
                        principalColumn: "VEHICLE_MODEL",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KPI",
                schema: "C##CAR",
                columns: table => new
                {
                    KPI_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TOTAL_PERFORMANCE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    SERVICE_FREQUENCY = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SCORE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    employeeId = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009105", x => x.KPI_ID);
                    table.ForeignKey(
                        name: "FK_KPI_EMPLOYEE_employeeId",
                        column: x => x.employeeId,
                        principalSchema: "C##CAR",
                        principalTable: "EMPLOYEE",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MAINTENANCE_ITEM",
                schema: "C##CAR",
                columns: table => new
                {
                    MAINTENANCE_ITEM_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MAINTENANCE_LOCATION = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Title = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    SERVICE_TIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ORDER_SUBMISSION_TIME = table.Column<DateTime>(type: "TIMESTAMP(6)", precision: 6, nullable: false),
                    ORDER_STATUS = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Score = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    VehicleId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    VehicleOwnerOwnerId = table.Column<long>(type: "NUMBER(19)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009117", x => x.MAINTENANCE_ITEM_ID);
                    table.ForeignKey(
                        name: "FK_MAINTENANCE_ITEM_VEHICLE_OWNER_VehicleOwnerOwnerId",
                        column: x => x.VehicleOwnerOwnerId,
                        principalSchema: "C##CAR",
                        principalTable: "VEHICLE_OWNER",
                        principalColumn: "OWNER_ID");
                    table.ForeignKey(
                        name: "FK_MAINTENANCE_ITEM_VEHICLE_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "C##CAR",
                        principalTable: "VEHICLE",
                        principalColumn: "VEHICLE_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SWITCH_LOG",
                schema: "C##CAR",
                columns: table => new
                {
                    SWITCH_SERVICE_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SWITCH_TIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Score = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    batteryOnBatteryId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    batteryOffBatteryId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    VehicleId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    EmployeeId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    switchlogSwitchServiceId = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C009138", x => x.SWITCH_SERVICE_ID);
                    table.ForeignKey(
                        name: "FK_SWITCH_LOG_BATTERY_batteryOffBatteryId",
                        column: x => x.batteryOffBatteryId,
                        principalSchema: "C##CAR",
                        principalTable: "BATTERY",
                        principalColumn: "BATTERY_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SWITCH_LOG_BATTERY_batteryOnBatteryId",
                        column: x => x.batteryOnBatteryId,
                        principalSchema: "C##CAR",
                        principalTable: "BATTERY",
                        principalColumn: "BATTERY_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SWITCH_LOG_EMPLOYEE_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "C##CAR",
                        principalTable: "EMPLOYEE",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SWITCH_LOG_SWITCH_LOG_switchlogSwitchServiceId",
                        column: x => x.switchlogSwitchServiceId,
                        principalSchema: "C##CAR",
                        principalTable: "SWITCH_LOG",
                        principalColumn: "SWITCH_SERVICE_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SWITCH_LOG_VEHICLE_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "C##CAR",
                        principalTable: "VEHICLE",
                        principalColumn: "VEHICLE_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SWITCH_REQUEST",
                schema: "C##CAR",
                columns: table => new
                {
                    SWITCH_REQUEST_ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SWITCH_TYPE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    REQUEST_TIME = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    POSITION = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    Longtitue = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    NOTES = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    EmployeeId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    vehicleOwnerOwnerId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    VehicleId = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("SYS_C008772", x => x.SWITCH_REQUEST_ID);
                    table.ForeignKey(
                        name: "FK_SWITCH_REQUEST_EMPLOYEE_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "C##CAR",
                        principalTable: "EMPLOYEE",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SWITCH_REQUEST_VEHICLE_OWNER_vehicleOwnerOwnerId",
                        column: x => x.vehicleOwnerOwnerId,
                        principalSchema: "C##CAR",
                        principalTable: "VEHICLE_OWNER",
                        principalColumn: "OWNER_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SWITCH_REQUEST_VEHICLE_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "C##CAR",
                        principalTable: "VEHICLE",
                        principalColumn: "VEHICLE_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employee_MaintenanceItem",
                schema: "C##CAR",
                columns: table => new
                {
                    employeesEmployeeId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    maintenanceItemsMaintenanceItemId = table.Column<long>(type: "NUMBER(19)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee_MaintenanceItem", x => new { x.employeesEmployeeId, x.maintenanceItemsMaintenanceItemId });
                    table.ForeignKey(
                        name: "FK_Employee_MaintenanceItem_EMPLOYEE_employeesEmployeeId",
                        column: x => x.employeesEmployeeId,
                        principalSchema: "C##CAR",
                        principalTable: "EMPLOYEE",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employee_MaintenanceItem_MAINTENANCE_ITEM_maintenanceItemsMaintenanceItemId",
                        column: x => x.maintenanceItemsMaintenanceItemId,
                        principalSchema: "C##CAR",
                        principalTable: "MAINTENANCE_ITEM",
                        principalColumn: "MAINTENANCE_ITEM_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BATTERY_BatteryTypeId",
                schema: "C##CAR",
                table: "BATTERY",
                column: "BatteryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BATTERY_switchStationStationId",
                schema: "C##CAR",
                table: "BATTERY",
                column: "switchStationStationId");

            migrationBuilder.CreateIndex(
                name: "IX_EMPLOYEE_switchStationStationId",
                schema: "C##CAR",
                table: "EMPLOYEE",
                column: "switchStationStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_MaintenanceItem_maintenanceItemsMaintenanceItemId",
                schema: "C##CAR",
                table: "Employee_MaintenanceItem",
                column: "maintenanceItemsMaintenanceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_KPI_employeeId",
                schema: "C##CAR",
                table: "KPI",
                column: "employeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MAINTENANCE_ITEM_VehicleId",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_MAINTENANCE_ITEM_VehicleOwnerOwnerId",
                schema: "C##CAR",
                table: "MAINTENANCE_ITEM",
                column: "VehicleOwnerOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_NEWS_administratorAdminId",
                schema: "C##CAR",
                table: "NEWS",
                column: "administratorAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_OWNERPOS_vehicleownerOwnerId",
                schema: "C##CAR",
                table: "OWNERPOS",
                column: "vehicleownerOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SWITCH_LOG_batteryOffBatteryId",
                schema: "C##CAR",
                table: "SWITCH_LOG",
                column: "batteryOffBatteryId");

            migrationBuilder.CreateIndex(
                name: "IX_SWITCH_LOG_batteryOnBatteryId",
                schema: "C##CAR",
                table: "SWITCH_LOG",
                column: "batteryOnBatteryId");

            migrationBuilder.CreateIndex(
                name: "IX_SWITCH_LOG_EmployeeId",
                schema: "C##CAR",
                table: "SWITCH_LOG",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SWITCH_LOG_switchlogSwitchServiceId",
                schema: "C##CAR",
                table: "SWITCH_LOG",
                column: "switchlogSwitchServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SWITCH_LOG_VehicleId",
                schema: "C##CAR",
                table: "SWITCH_LOG",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_SWITCH_REQUEST_EmployeeId",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SWITCH_REQUEST_VehicleId",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_SWITCH_REQUEST_vehicleOwnerOwnerId",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                column: "vehicleOwnerOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VEHICLE_BatteryId",
                schema: "C##CAR",
                table: "VEHICLE",
                column: "BatteryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VEHICLE_vehicleOwnerOwnerId",
                schema: "C##CAR",
                table: "VEHICLE",
                column: "vehicleOwnerOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VEHICLE_vehicleParamVehicleModelId",
                schema: "C##CAR",
                table: "VEHICLE",
                column: "vehicleParamVehicleModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employee_MaintenanceItem",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "KPI",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "NEWS",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "OWNERPOS",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "SWITCH_LOG",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "SWITCH_REQUEST",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "MAINTENANCE_ITEM",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "ADMINISTRATOR",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "EMPLOYEE",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "VEHICLE",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "BATTERY",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "VEHICLE_OWNER",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "VEHICLE_PARAM",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "BATTERY_TYPE",
                schema: "C##CAR");

            migrationBuilder.DropTable(
                name: "SWITCH_STATION",
                schema: "C##CAR");
        }
    }
}
