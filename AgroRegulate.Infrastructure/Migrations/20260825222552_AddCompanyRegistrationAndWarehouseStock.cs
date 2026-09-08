using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroRegulate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyRegistrationAndWarehouseStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountStatus",
                table: "Factories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Factories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Factories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "AllocationRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WarehouseStocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductType = table.Column<int>(type: "integer", nullable: false),
                    AvailableTons = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseStocks", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseStocks");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "Factories");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Factories");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Factories");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "AllocationRequests");
        }
    }
}
