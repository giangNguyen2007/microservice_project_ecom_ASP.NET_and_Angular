using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductStockColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("55513f64-08e6-4e48-839a-097664a6c3d0"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("0193bf62-dc7f-45c9-8667-3ee7ce5a7e11"));

            migrationBuilder.AddColumn<int>(
                name: "ReservedStock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "ProductId", "ProductModelId", "Rating", "userId" },
                values: new object[] { new Guid("3e1a33b3-f75d-425e-ae82-625e98822d4c"), "Great product!", new Guid("3ed6018b-0017-446b-9fec-0214b0821b4e"), null, 5, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "PhotoUrl", "Price", "ReservedStock", "Stock", "Title" },
                values: new object[] { new Guid("3ed6018b-0017-446b-9fec-0214b0821b4e"), "Shoes", "Nike Shoes", null, 50, 0, 100, "Nike Shoes" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: new Guid("3e1a33b3-f75d-425e-ae82-625e98822d4c"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("3ed6018b-0017-446b-9fec-0214b0821b4e"));

            migrationBuilder.DropColumn(
                name: "ReservedStock",
                table: "Products");

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "ProductId", "ProductModelId", "Rating", "userId" },
                values: new object[] { new Guid("55513f64-08e6-4e48-839a-097664a6c3d0"), "Great product!", new Guid("0193bf62-dc7f-45c9-8667-3ee7ce5a7e11"), null, 5, new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "PhotoUrl", "Price", "Stock", "Title" },
                values: new object[] { new Guid("0193bf62-dc7f-45c9-8667-3ee7ce5a7e11"), "Shoes", "Nike Shoes", null, 50, 100, "Nike Shoes" });
        }
    }
}
