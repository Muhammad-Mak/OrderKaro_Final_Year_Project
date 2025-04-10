using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledTimeAndNullableDeliveryLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableNumber",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryLocation",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledTime",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryLocation",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ScheduledTime",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "TableNumber",
                table: "Orders",
                type: "int",
                nullable: true);
        }
    }
}
