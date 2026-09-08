using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroRegulate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSectorTypeToFactory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "Quotas");

            migrationBuilder.DropColumn(
                name: "MonthlyCapacityTons",
                table: "Factories");

            migrationBuilder.RenameColumn(
                name: "UnitPriceTL",
                table: "SalesAnnouncements",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "TotalQuantityTons",
                table: "SalesAnnouncements",
                newName: "TotalOfferedTons");

            migrationBuilder.RenameColumn(
                name: "RemainingQuantityTons",
                table: "SalesAnnouncements",
                newName: "AvailableTons");

            migrationBuilder.RenameColumn(
                name: "Product",
                table: "SalesAnnouncements",
                newName: "ProductType");

            migrationBuilder.RenameColumn(
                name: "UsedQuotaTons",
                table: "Quotas",
                newName: "TotalAllocatedTons");

            migrationBuilder.RenameColumn(
                name: "Product",
                table: "Quotas",
                newName: "ProductType");

            migrationBuilder.RenameColumn(
                name: "MaxQuotaTons",
                table: "Quotas",
                newName: "RemainingTons");

            migrationBuilder.RenameColumn(
                name: "Sector",
                table: "Factories",
                newName: "SectorType");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SalesAnnouncements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SalesAnnouncementId",
                table: "AllocationRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotas_FactoryId",
                table: "Quotas",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationRequests_FactoryId",
                table: "AllocationRequests",
                column: "FactoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationRequests_SalesAnnouncementId",
                table: "AllocationRequests",
                column: "SalesAnnouncementId");

            migrationBuilder.AddForeignKey(
                name: "FK_AllocationRequests_Factories_FactoryId",
                table: "AllocationRequests",
                column: "FactoryId",
                principalTable: "Factories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllocationRequests_SalesAnnouncements_SalesAnnouncementId",
                table: "AllocationRequests",
                column: "SalesAnnouncementId",
                principalTable: "SalesAnnouncements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotas_Factories_FactoryId",
                table: "Quotas",
                column: "FactoryId",
                principalTable: "Factories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllocationRequests_Factories_FactoryId",
                table: "AllocationRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AllocationRequests_SalesAnnouncements_SalesAnnouncementId",
                table: "AllocationRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotas_Factories_FactoryId",
                table: "Quotas");

            migrationBuilder.DropIndex(
                name: "IX_Quotas_FactoryId",
                table: "Quotas");

            migrationBuilder.DropIndex(
                name: "IX_AllocationRequests_FactoryId",
                table: "AllocationRequests");

            migrationBuilder.DropIndex(
                name: "IX_AllocationRequests_SalesAnnouncementId",
                table: "AllocationRequests");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SalesAnnouncements");

            migrationBuilder.DropColumn(
                name: "SalesAnnouncementId",
                table: "AllocationRequests");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "SalesAnnouncements",
                newName: "UnitPriceTL");

            migrationBuilder.RenameColumn(
                name: "TotalOfferedTons",
                table: "SalesAnnouncements",
                newName: "TotalQuantityTons");

            migrationBuilder.RenameColumn(
                name: "ProductType",
                table: "SalesAnnouncements",
                newName: "Product");

            migrationBuilder.RenameColumn(
                name: "AvailableTons",
                table: "SalesAnnouncements",
                newName: "RemainingQuantityTons");

            migrationBuilder.RenameColumn(
                name: "TotalAllocatedTons",
                table: "Quotas",
                newName: "UsedQuotaTons");

            migrationBuilder.RenameColumn(
                name: "RemainingTons",
                table: "Quotas",
                newName: "MaxQuotaTons");

            migrationBuilder.RenameColumn(
                name: "ProductType",
                table: "Quotas",
                newName: "Product");

            migrationBuilder.RenameColumn(
                name: "SectorType",
                table: "Factories",
                newName: "Sector");

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Quotas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyCapacityTons",
                table: "Factories",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
