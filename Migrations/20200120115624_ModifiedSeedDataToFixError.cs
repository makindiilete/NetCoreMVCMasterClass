using Microsoft.EntityFrameworkCore.Migrations;

namespace Asp_Net_Core_Masterclass.Migrations
{
    public partial class ModifiedSeedDataToFixError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Michaelz Omoakin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Michaelz");
        }
    }
}
