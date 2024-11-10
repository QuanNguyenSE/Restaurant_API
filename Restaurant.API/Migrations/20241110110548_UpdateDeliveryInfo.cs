using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeliveryInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryInfo_StreetAddress",
                table: "OrderHeaders",
                newName: "StreetAddress");

            migrationBuilder.RenameColumn(
                name: "DeliveryInfo_State",
                table: "OrderHeaders",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "DeliveryInfo_PhoneNumber",
                table: "OrderHeaders",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "DeliveryInfo_Name",
                table: "OrderHeaders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DeliveryInfo_City",
                table: "OrderHeaders",
                newName: "City");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StreetAddress",
                table: "OrderHeaders",
                newName: "DeliveryInfo_StreetAddress");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "OrderHeaders",
                newName: "DeliveryInfo_State");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "OrderHeaders",
                newName: "DeliveryInfo_PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OrderHeaders",
                newName: "DeliveryInfo_Name");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "OrderHeaders",
                newName: "DeliveryInfo_City");
        }
    }
}
