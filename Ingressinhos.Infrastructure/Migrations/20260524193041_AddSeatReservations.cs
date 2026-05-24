using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ingressinhos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeatReservations",
                schema: "sales",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    SeatId = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    OrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReservedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OccupiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeatReservations_Events_EventId",
                        column: x => x.EventId,
                        principalSchema: "catalog",
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatReservations_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalSchema: "sales",
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatReservations_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "sales",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatReservations_Seats_SeatId",
                        column: x => x.SeatId,
                        principalSchema: "catalog",
                        principalTable: "Seats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO sales."SeatReservations"
                    ("EventId", "SeatId", "OrderId", "OrderItemId", "Status", "ReservedAt", "OccupiedAt", "CancelledAt", "CreatedAt", "UpdatedAt")
                SELECT
                    ticket."EventId",
                    orderItem."SeatId",
                    orderItem."OrderId",
                    orderItem."Id",
                    CASE WHEN orderEntity."Status" = 3 THEN 2 ELSE 1 END,
                    COALESCE(orderEntity."UpdatedAt", orderItem."UpdatedAt"),
                    CASE WHEN orderEntity."Status" = 3 THEN COALESCE(orderEntity."PaidAt", orderEntity."UpdatedAt") ELSE NULL END,
                    NULL,
                    COALESCE(orderItem."CreatedAt", orderEntity."CreatedAt"),
                    COALESCE(orderItem."UpdatedAt", orderEntity."UpdatedAt")
                FROM sales."OrderItems" orderItem
                INNER JOIN sales."Orders" orderEntity ON orderEntity."Id" = orderItem."OrderId"
                INNER JOIN catalog."Tickets" ticket ON ticket."Id" = orderItem."TicketId"
                WHERE orderItem."SeatId" IS NOT NULL
                  AND orderEntity."Status" IN (2, 3);
                """);

            migrationBuilder.Sql(
                """
                UPDATE catalog."Seats"
                SET "Status" = 1,
                    "UpdatedAt" = NOW()
                WHERE "Status" IN (2, 3);
                """);

            migrationBuilder.CreateIndex(
                name: "IX_SeatReservations_EventId_SeatId_Active",
                schema: "sales",
                table: "SeatReservations",
                columns: new[] { "EventId", "SeatId" },
                unique: true,
                filter: "\"Status\" IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_SeatReservations_OrderId",
                schema: "sales",
                table: "SeatReservations",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatReservations_OrderItemId",
                schema: "sales",
                table: "SeatReservations",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatReservations_SeatId",
                schema: "sales",
                table: "SeatReservations",
                column: "SeatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeatReservations",
                schema: "sales");
        }
    }
}
