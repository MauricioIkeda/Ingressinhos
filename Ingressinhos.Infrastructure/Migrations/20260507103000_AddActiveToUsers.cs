using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ingressinhos.Infrastructure.Migrations
{
    public partial class AddActiveToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Sellers",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Sellers");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Clients");
        }
    }
}
