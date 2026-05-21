using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ingressinhos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class orderitemseatandcheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatCode",
                table: "OrderItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SeatId",
                table: "OrderItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_SeatId",
                table: "OrderItems",
                column: "SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Seats_SeatId",
                table: "OrderItems",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Seats_SeatId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_SeatId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SeatCode",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SeatId",
                table: "OrderItems");
        }
    }
}
