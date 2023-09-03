using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSS_EFCore.Migrations
{
    /// <inheritdoc />
    
    public partial class Final2 : Migration
    {
        /// <inheritdoc />
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Period",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                nullable: true);
            migrationBuilder.RenameColumn(
                name: "Period",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                newName: "PERIOD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PERIOD",
                schema: "C##CAR",
                table: "SWITCH_REQUEST",
                newName: "Period");
            migrationBuilder.DropColumn(
                name: "Period",
                schema: "C##CAR",
                table: "SWITCH_REQUEST");
        }
    }
}
