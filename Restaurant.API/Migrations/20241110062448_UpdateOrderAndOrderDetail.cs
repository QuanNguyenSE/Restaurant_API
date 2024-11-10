using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderAndOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "OrderHeaders");

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

            migrationBuilder.AlterColumn<int>(
                name: "OrderStatus",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryInfo_PhoneNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryInfo_Name",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<double>(
                name: "DeliveryFee",
                table: "OrderHeaders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "StreetAddress",
                table: "OrderHeaders",
                newName: "StreetAddress");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "OrderHeaders",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "OrderHeaders",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OrderHeaders",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "OrderHeaders",
                newName: "City");

            migrationBuilder.AlterColumn<string>(
                name: "OrderStatus",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
