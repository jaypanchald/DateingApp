using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.Repository.Migrations
{
    public partial class UserGenderLengthUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "User",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "User",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);
        }
    }
}
