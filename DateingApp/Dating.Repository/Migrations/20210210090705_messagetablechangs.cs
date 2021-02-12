using Microsoft.EntityFrameworkCore.Migrations;

namespace Dating.Repository.Migrations
{
    public partial class messagetablechangs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecipientUsername",
                table: "Message",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderUsername",
                table: "Message",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecipientUsername",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "SenderUsername",
                table: "Message");
        }
    }
}
