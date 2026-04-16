using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ingressinhos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SegundaMigracaoSLC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Locations_LocationId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_IssuedTickets_Clients_ClientId",
                table: "IssuedTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_IssuedTickets_Events_EventId",
                table: "IssuedTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_IssuedTickets_Orders_OrderId",
                table: "IssuedTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Tickets_TicketId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Clients_ClientId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Orders_OrderId",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PublishedTickets_Seats_SeatId",
                table: "PublishedTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_PaymentTransactions_PaymentTransactionId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Locations_LocationId",
                table: "Seats");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Events_EventId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_EventId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Seats_LocationId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_PaymentTransactionId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_PublishedTickets_SeatId",
                table: "PublishedTickets");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ClientId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_TicketId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_IssuedTickets_ClientId",
                table: "IssuedTickets");

            migrationBuilder.DropIndex(
                name: "IX_IssuedTickets_EventId",
                table: "IssuedTickets");

            migrationBuilder.DropIndex(
                name: "IX_IssuedTickets_OrderId",
                table: "IssuedTickets");

            migrationBuilder.DropIndex(
                name: "IX_Events_LocationId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "IssuedTickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "IssuedTickets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EventId",
                table: "Tickets",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_LocationId",
                table: "Seats",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_PaymentTransactionId",
                table: "Refunds",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedTickets_SeatId",
                table: "PublishedTickets",
                column: "SeatId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientId",
                table: "Orders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_TicketId",
                table: "OrderItems",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedTickets_ClientId",
                table: "IssuedTickets",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedTickets_EventId",
                table: "IssuedTickets",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedTickets_OrderId",
                table: "IssuedTickets",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LocationId",
                table: "Events",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Locations_LocationId",
                table: "Events",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssuedTickets_Clients_ClientId",
                table: "IssuedTickets",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssuedTickets_Events_EventId",
                table: "IssuedTickets",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssuedTickets_Orders_OrderId",
                table: "IssuedTickets",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Tickets_TicketId",
                table: "OrderItems",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Clients_ClientId",
                table: "Orders",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Orders_OrderId",
                table: "PaymentTransactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublishedTickets_Seats_SeatId",
                table: "PublishedTickets",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_PaymentTransactions_PaymentTransactionId",
                table: "Refunds",
                column: "PaymentTransactionId",
                principalTable: "PaymentTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Locations_LocationId",
                table: "Seats",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Events_EventId",
                table: "Tickets",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
