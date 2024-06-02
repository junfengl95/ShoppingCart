using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartApi.Migrations
{
    /// <inheritdoc />
    public partial class IncludeQuantityinCartItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Cart_Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Cart_Items");
        }
    }
}
