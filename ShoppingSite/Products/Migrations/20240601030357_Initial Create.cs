using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Product_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Product_Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Product_Quantity = table.Column<int>(type: "int", nullable: false),
                    Product_Rating = table.Column<decimal>(type: "decimal(3,1)", nullable: true),
                    Product_Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Product_Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Product_Image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__9834FBBA37139518", x => x.Product_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
