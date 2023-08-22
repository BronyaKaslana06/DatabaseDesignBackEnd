using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSS_EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OWNERPOS_VEHICLE_OWNER_vehicleownerOwnerId",
                schema: "C##CAR",
                table: "OWNERPOS");

            migrationBuilder.DropIndex(
                name: "IX_OWNERPOS_vehicleownerOwnerId",
                schema: "C##CAR",
                table: "OWNERPOS");

            migrationBuilder.DropColumn(
                name: "vehicleownerOwnerId",
                schema: "C##CAR",
                table: "OWNERPOS");

            migrationBuilder.RenameColumn(
                name: "Owner_ID",
                schema: "C##CAR",
                table: "OWNERPOS",
                newName: "OwnerId");

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                schema: "C##CAR",
                table: "OWNERPOS",
                type: "NUMBER(19)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "NUMBER(19)")
                .OldAnnotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AddForeignKey(
                name: "FK_OWNERPOS_VEHICLE_OWNER_OwnerId",
                schema: "C##CAR",
                table: "OWNERPOS",
                column: "OwnerId",
                principalSchema: "C##CAR",
                principalTable: "VEHICLE_OWNER",
                principalColumn: "OWNER_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OWNERPOS_VEHICLE_OWNER_OwnerId",
                schema: "C##CAR",
                table: "OWNERPOS");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "C##CAR",
                table: "OWNERPOS",
                newName: "Owner_ID");

            migrationBuilder.AlterColumn<long>(
                name: "Owner_ID",
                schema: "C##CAR",
                table: "OWNERPOS",
                type: "NUMBER(19)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "NUMBER(19)")
                .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1");

            migrationBuilder.AddColumn<long>(
                name: "vehicleownerOwnerId",
                schema: "C##CAR",
                table: "OWNERPOS",
                type: "NUMBER(19)",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_OWNERPOS_vehicleownerOwnerId",
                schema: "C##CAR",
                table: "OWNERPOS",
                column: "vehicleownerOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_OWNERPOS_VEHICLE_OWNER_vehicleownerOwnerId",
                schema: "C##CAR",
                table: "OWNERPOS",
                column: "vehicleownerOwnerId",
                principalSchema: "C##CAR",
                principalTable: "VEHICLE_OWNER",
                principalColumn: "OWNER_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
